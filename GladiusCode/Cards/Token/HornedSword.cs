using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class HornedSword() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy), IDurableCard
{
	// 용각 검 - 연성물
    int IDurableCard.Durability { get; set; } = 3;

	private const string _increaseKey = "Increase";

	private decimal _extraDamageFromPlays;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, DamageProps.card), new DynamicVar("Increase", 2m)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact];

    private decimal ExtraDamageFromPlays
	{
		get
		{
			return _extraDamageFromPlays;
		}
		set
		{
			AssertMutable();
			_extraDamageFromPlays = value;
		}
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
		base.DynamicVars.Damage.BaseValue += base.DynamicVars["Increase"].BaseValue;
		ExtraDamageFromPlays += base.DynamicVars["Increase"].BaseValue;
    }

    protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		base.DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
	}

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["Increase"].UpgradeValueBy(1m);
    }
}