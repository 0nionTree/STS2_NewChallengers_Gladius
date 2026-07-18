using Gladius.GladiusCode.Enchantments;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class Shrapnel : GladiusEnchantment
{
    public override bool HasExtraCardText => true;

	public override bool ShowAmount => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DamageVar(0m, DamageProps.card)];
    
	public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
	{
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(Card)
			.TargetingRandomOpponents(Card.CombatState!)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
	}

	public override void RecalculateValues()
	{
		DynamicVars.Damage.BaseValue = Amount;
	}
}