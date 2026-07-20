using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius.GladiusCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
[Pool(typeof(GladiusCardPool))]
public abstract class GladiusCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true
) : CustomCardModel(cost, type, rarity, target, showInCardLibrary, autoAdd)
{
    public virtual bool IsDurable => false;
    public virtual int BaseDurability => 0;
    public virtual bool IsRequiredMaterial => false;
    public virtual bool IsRequiredDurable => false;

    // 카드 사용에 소재 또는 내구도가 존재하는 카드가 필요하다면, 조건 불만족 시 붉은 테두리로 표시
    protected override bool ShouldGlowRedInternal
    {
        get
        {
            // 손에 있는 카드 목록 확인
            bool result = false;
            var cards = PileType.Hand.GetPile(Owner).Cards;
            // 사용에 소재 카드가 필요할 경우
            if (IsRequiredMaterial)
            {
                // 카드 목록에 소재 카드가 있다면 false 반환
                result = !cards.Any(c => c.Keywords.Contains(GladiusKeywords.Material));
            }
            // 사용에 내구도가 존재하는 카드가 필요할 경우
            if (IsRequiredDurable)
            {
                // 카드 목록에 내구도가 있는 카드가 있다면 false 반환
                result = !cards.Any(c => c.GetDurability().isDurable);
            }
            // 사용 조건이 부족하다면 true(붉게 발광), 아니라면 false(기본 상태) 반환
            return result;
        }
    }


    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // 연성 함수 실행 시 수신받아 자동으로 실행되는 함수
    public virtual Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator)
    {
        return Task.CompletedTask; 
    }

    protected virtual async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard){}

    /// <summary>
    /// 연성 : 소재(Material) 키워드가 존재하는 카드를 선택하여 지정된 연성물(Artifact) 카드로 변환(Transform) 시킨다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="choiceContext">실행중인 PlayerChoiceContext</param>
    /// <param name="isUpgraded">생성할 연성물의 강화 유무</param>
    /// <param name="durability">생성할 연성물의 내구도 증감 (없거나 0이어도 무방)</param>
    /// <param name="material">소재 카드의 사전 지정(지정해둘 경우 선택창이 나오지 않는다)</param>
    /// <returns></returns>
    protected async Task<T?> Alchemy<T>(PlayerChoiceContext choiceContext, bool isUpgraded, int durability = 0, CardModel? material = null) where T : CardModel
    {
        // 소재 선택 메시지를 지정
        var promptString = new LocString("combat_messages", "SELECT_MATERIAL");

        // 지정된 소재(Material)이 있다면 사용하고, 없다면 손에서 선택
        if (material == null)
        {
            material = (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(promptString, 1), 
                context: choiceContext, 
                player: Owner, 
                // 필터 조건: Material 키워드가 있는 카드만 선택 가능
                filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Material), 
                source: this
            )).FirstOrDefault();
        }

        // 선택된 소재가 존재하며, 소재 키워드를 가지고있다면 계속
        if (material != null && material.Keywords.Contains(GladiusKeywords.Material))
        {
            // Artifact(연성물)카드 생성
            T? artifact = CombatState!.CreateCard<T>(Owner);
            if (isUpgraded) // 강화 버전을 생성하는지 확인
            {
                CardCmd.Upgrade(artifact);
            }
            // 내구도 증감 지정이 있다면 연성물 카드의 내구도 증감
            if (durability != 0)
            {
                DurabilityExtensions.VarianceDurability(artifact, durability);
            }
            // 선택한 소재 카드의 Material() 함수 실행
            // CardModel을 Material() 함수가 정의된 커스텀 클래스로 캐스팅 (예: GladiusCard)
            if (material is GladiusCard gladiusCard)
            {
                await gladiusCard.Material(choiceContext, artifact); // 형변환에 성공했다면 함수 실행
            }

            // 전투 중이라면 (전투 상태가 존재한다면) 글로벌 이벤트를 발송
            if (CombatState != null)
            {
                await AlchemyEventDispatcher.DispatchAlchemyTriggered(CombatState, artifact, material, Owner);
            }
            
            // 소재 카드를 연성물 카드로 변화
            await CardCmd.Transform(material, artifact);
            await Cmd.Wait(0.2f);

            // 최종 연성된 연성물 카드의 내구도가 0 이하라면 소멸
            if (artifact.GetDurability().CurrentDurability <= 0)
            {
                await DurabilityExtensions.ExhaustArtifact(choiceContext, artifact);
                await Cmd.Wait(0.2f);

                artifact = null;
            }

            

            return artifact;
        }
        // 소재가 없다면
        else
        {
            // 소재가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "MATERIALS_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
        
        return null;
    }
}