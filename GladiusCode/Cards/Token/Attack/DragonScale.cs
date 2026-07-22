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

namespace Gladius;

[Pool(typeof(TokenCardPool))]
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
        [new DamageVar(5m, DamageProps.card),
        new IntVar("CurrentRepeat", 1)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (HasImperialScale)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(DynamicVars["CurrentRepeat"].IntValue).FromCard(this).Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        }
        else
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState!)
                .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    protected override void AfterDowngraded()
	{
		AfterDowngraded();
		DynamicVars["CurrentRepeat"].BaseValue += CurrentRepeat;
	}
}