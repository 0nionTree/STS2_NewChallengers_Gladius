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
public class Keyboard() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 건반
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<TuningShard>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손에 있는 카드를 선택 후, 선택된 카드마다 반복
        foreach (CardModel item in await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue), context: choiceContext, player: Owner, filter: null, source: this))
		{
            // 선택된 카드 버리기
			await CardCmd.Discard(choiceContext, item);
            CardModel cardModel = CombatState!.CreateCard<TuningShard>(Owner);
            if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
                CardCmd.Upgrade(cardModel);
            // 생성한 카드 손으로 가져오기
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		}
    }

    //protected override void OnUpgrade()
}