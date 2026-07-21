using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class BreathOfFire() : GladiusCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // 불의 숨결
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<BreathOfFirePower>(1),
        new PowerVar<DragonAuraPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].BaseValue, Owner.Creature, this);
		await PowerCmd.Apply<BreathOfFirePower>(choiceContext, Owner.Creature, DynamicVars["BreathOfFirePower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DragonAuraPower"].UpgradeValueBy(2);
    }
}