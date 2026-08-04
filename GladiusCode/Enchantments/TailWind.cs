using Gladius.GladiusCode.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

public class TailWind : GladiusEnchantment
{
    public override bool HasExtraCardText => true;

	public override bool ShowAmount => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(1)];
    
	public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
	{
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, cardPlay!.Card.Owner);
	}

	public override void RecalculateValues()
	{
		DynamicVars.Cards.BaseValue = Amount;
	}
}