using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Hooks;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Prediction() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 예측
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar("BlockNextTurn", 10m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
		BlockVar blockVar = (BlockVar)DynamicVars["BlockNextTurn"];
		IEnumerable<AbstractModel> modifiers;
		decimal blockNextTurnAmount = Hook.ModifyBlock(CombatState!, Owner.Creature, blockVar.BaseValue, blockVar.Props, this, cardPlay, out modifiers);
		await PowerCmd.Apply<BlockNextTurnPower>(choiceContext, Owner.Creature, blockNextTurnAmount, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
		DynamicVars["BlockNextTurn"].UpgradeValueBy(5m);
    }
}