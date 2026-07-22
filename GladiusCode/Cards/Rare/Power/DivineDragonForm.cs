using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DivineDragonForm() : GladiusCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 용신의 형상
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<DivineDragonFormPower>(1),
        new PowerVar<DragonAuraPower>(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<DivineDragonFormPower>(choiceContext, Owner.Creature, DynamicVars["DivineDragonFormPower"].BaseValue, Owner.Creature, this);
		if (IsUpgraded)
            await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].BaseValue, Owner.Creature, this);
    }

    //protected override void OnUpgrade()
}