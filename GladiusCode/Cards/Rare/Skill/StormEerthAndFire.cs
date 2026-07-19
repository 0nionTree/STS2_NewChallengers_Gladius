using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class StormEerthAndFire() : GladiusCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 폭풍, 대지, 불꽃
    //protected override IEnumerable<DynamicVar> CanonicalVars => 
    //    [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<WindStone>(IsUpgraded),
        HoverTipFactory.FromCard<Diamond>(IsUpgraded),
        HoverTipFactory.FromCard<DragonOrb>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 각 카드 생성
        List<CardModel> cards = 
        [
            CombatState!.CreateCard<WindStone>(Owner),
            CombatState!.CreateCard<Diamond>(Owner),
            CombatState!.CreateCard<DragonOrb>(Owner)
        ];
        foreach (CardModel cardModel in cards)
        {
            if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
                CardCmd.Upgrade(cardModel);
            // 카드 비용 감소
            cardModel.EnergyCost.SetUntilPlayed(0);
            // 생성한 카드 손으로 가져오기
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		    await Cmd.Wait(0.1f);
        }
        
    }

    //protected override void OnUpgrade()
}