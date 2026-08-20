using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Commands.Builders;

namespace Gladius;

[Pool(typeof(ArtifactCardPool))]
public class DragonScale() : GladiusCard(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
{
    // 용 비늘 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 1;

    private decimal _currentRepeat;

    public override TargetType TargetType
	{
		get
		{
			if (!HasImperialScale)
			{
				return TargetType.AllEnemies;
			}
			return TargetType.AnyEnemy;
		}
	}

    private bool HasImperialScale
	{
		get
		{
			if (IsMutable && Owner != null)
			{
				return Owner.Creature.HasPower<ImperialScalePower>();
			}
			return false;
		}
	}

    public decimal CurrentRepeat
	{
		get
		{
			return _currentRepeat;
		}
		set
		{
			AssertMutable();
			_currentRepeat = value;
            DynamicVars["CurrentRepeat"].BaseValue = _currentRepeat + 1;
		}
	}

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, DamageProps.card),
        new IntVar("CurrentRepeat", 1)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars["CurrentRepeat"].IntValue).FromCard(this);
		if (!HasImperialScale)
		{
			Creature? lastEnemy = CombatState!.HittableEnemies.LastOrDefault();
			attackCommand = attackCommand.TargetingAllOpponents(CombatState).WithHitVfxNode((Creature _) => NShivThrowVfx.Create(Owner.Creature, lastEnemy, Colors.Gold));
		}
		else
		{
			ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
			attackCommand = attackCommand.Targeting(cardPlay.Target).WithHitVfxNode((Creature t) => NShivThrowVfx.Create(Owner.Creature, t, Colors.Gold));
		}
		if (Owner.Character is Silent)
		{
			attackCommand.WithAttackerAnim("Shiv", 0.2f);
		}
		await attackCommand.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		DynamicVars["CurrentRepeat"].BaseValue += CurrentRepeat;
	}
}