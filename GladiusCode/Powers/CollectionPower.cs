using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class CollectionPower : GladiusPower
{
    // 회수 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 잔류한 카드마다 무작위 적에게 피해
    public override async Task OnScreenedCardsMoved(CardModel cardModel, bool isRemain, Player owner, PlayerChoiceContext choiceContext)
    {
        if (owner == Owner.Player && isRemain)
        {
			IReadOnlyList<Creature> hittableEnemies = CombatState.HittableEnemies;
			if (hittableEnemies.Count != 0)
			{
				Creature? target = Owner.Player!.RunState.Rng.CombatTargets.NextItem(hittableEnemies);
				Flash();
				await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target!, Amount, ValueProp.Unpowered, Owner, null);
			}
        }
    }
}