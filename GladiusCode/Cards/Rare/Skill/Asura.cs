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
public class Asura() : GladiusCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 수라
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new IntVar("Durability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손에 있는 소모품 카드를 선택
        CardSelectorPrefs prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1)
		{
			PretendCardsCanBePlayed = true
		};
		CardModel? card = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, (CardModel c) => c.GetDurability().isDurable && !c.Keywords.Contains(CardKeyword.Unplayable), this)).FirstOrDefault();
		
        if (card != null)
		{
            await DurabilityExtensions.VarianceDurability(card, DynamicVars["Durability"].IntValue, choiceContext);

			for (int i = card.GetDurability().CurrentDurability; i > 0; i--)
			{
				await CardCmd.AutoPlay(choiceContext, card, null);
			}
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Durability"].UpgradeValueBy(1);
    }
}