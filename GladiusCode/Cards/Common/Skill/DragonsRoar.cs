using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsRoar() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 용의 문양
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<EnergyNextTurnPower>(1),
        new PowerVar<DragonAuraNextTurnPower>(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용기 획득
		await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["EnergyNextTurnPower"].IntValue, Owner.Creature, this);
		await PowerCmd.Apply<DragonAuraNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraNextTurnPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DragonAuraNextTurnPower"].UpgradeValueBy(1m);
    }
}