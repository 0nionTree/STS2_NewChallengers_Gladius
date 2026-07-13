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
    public virtual bool isDurable => false;
    public virtual int BaseDurability => 0;
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

    public virtual Task OnAlchemyTriggered(CardModel artifect, CardModel metarial, Player? creator)
    {
        // 기본적으로는 아무 일도 하지 않음
        // 생성 카드 정보 등 전송하도록 수정
        return Task.CompletedTask; 
    }

    protected virtual async Task Material(PlayerChoiceContext choiceContext, CardModel artifectCard){}

    protected async Task<T?> Alchemy<T>(PlayerChoiceContext choiceContext, bool isUpgraded, int durability = 0) where T : CardModel
    {
        var promptString = new LocString("combat_messages", "SELECT_MATERIAL");
		// var를 사용하거나 CardModel?을 사용하여 null 가능성을 명시합니다.
        var metarial = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            // 필터 조건: Material 키워드가 있는 카드만 선택 가능
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Material), 
            source: this
        )).FirstOrDefault();

        // 카드가 정상적으로 선택되었는지 확인 (null 체크)
        if (metarial != null)
        {
            // Artifect(연성물)카드 생성
            T artifect = CombatState!.CreateCard<T>(Owner);
            if (isUpgraded) // 강화 버전을 생성하는지 확인
            {
                CardCmd.Upgrade(artifect);
            }
            // 내구도 증감 지정이 있다면 연성물 카드의 내구도 증감
            if (durability != 0)
            {
                DurabilityExtensions.VarianceDurability(artifect, durability);
            }
            // 선택한 소재 카드의 Material() 함수 실행
            // CardModel을 Material() 함수가 정의된 커스텀 클래스로 캐스팅 (예: GladiusCard)
            if (metarial is GladiusCard gladiusCard)
            {
                await gladiusCard.Material(choiceContext, artifect); // 형변환에 성공했다면 함수 실행
            }
            // 소재 카드를 연성물 카드로 변화
            await CardCmd.Transform(metarial, artifect);
            await Cmd.Wait(0.2f);

            // 3. 전투 중이라면 (전투 상태가 존재한다면) 글로벌 이벤트를 발송합니다.
            if (CombatState != null)
            {
                await AlchemyEventDispatcher.DispatchAlchemyTriggered(CombatState, artifect, metarial, Owner);
            }

            // 최종 연성된 연성물 카드의 내구도가 0 이하라면 소멸
            if (artifect.GetCustomData().CurrentDurability <= 0)
            {
                await DurabilityExtensions.ExhaustArtifect(choiceContext, artifect);
                await Cmd.Wait(0.2f);

                return null;
            }

            return artifect;
        }
        return null;
    }
}