using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class AuraSaturation() : GladiusCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 용기 포화
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<AuraSaturationPower>(1),
        new PowerVar<DragonAuraPower>(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<AuraSaturationPower>(choiceContext, Owner.Creature, DynamicVars["AuraSaturationPower"].BaseValue, Owner.Creature, this);
        if (IsUpgraded)
            await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].BaseValue, Owner.Creature, this);
    }

    //protected override void OnUpgrade()
}