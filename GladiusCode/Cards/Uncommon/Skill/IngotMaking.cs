using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class IngotMaking() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 조괴
    public override bool IsRequiredDurable => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromCard<Steel>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability),
        HoverTipFactory.FromKeyword(GladiusKeywords.Remain),
        HoverTipFactory.FromKeyword(GladiusKeywords.Fall)];

    //public override IEnumerable<CardKeyword> CanonicalKeywords =>
    //    [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 카드 선택 안내 문구 지정
        var promptString = new LocString("combat_messages", "SELECT_DURABLE");
		// 손에 있는 소모품 카드 선택
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.GetDurability().isDurable, 
            source: this
        )).FirstOrDefault();
        int durability = 0;

        if (cardModel != null)
        {
            // 내구도 저장
            durability = cardModel.GetDurability().CurrentDurability;
            // 선택한 카드 내구도 초기화 및 소멸
            DurabilityExtensions.ResetDurability(cardModel);
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
        else
        {
            // 소재가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "DURABLES_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
        // 강철 생성
        CardModel steel = CombatState!.CreateCard<Steel>(Owner);
        // 이 카드가 강화되어있다면 내구도 결과값 +1
        if (IsUpgraded) durability++;
        // 저장된 내구도만큼 강철 강화
        for(int i = 0; i < durability; i++)
        {
            CardCmd.Upgrade(steel);
        }
        // 생성한 카드 손으로 가져오기
        await CardPileCmd.AddGeneratedCardToCombat(steel, PileType.Hand, Owner);
        await Cmd.Wait(0.2f);
    }

    protected override void OnUpgrade()
    {
        
    }
}