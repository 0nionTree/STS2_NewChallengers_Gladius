using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class SlagExplosionPower : GladiusPower
{
    // 잔여물 폭발 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 연성 시 무작위 대상 피해
    public override async Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
		// 연성 실행자가 파워 보유자가 아니라면 종료
		if (creator != Owner.Player) return;

        if (!(Amount <= 0m))
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