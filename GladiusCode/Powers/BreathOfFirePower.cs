using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gladius;

public class BreathOfFirePower : GladiusPower
{
    // 불의 숨결 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 턴 시작 시 용기 획득
	public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
	{
		if (participants.Contains(Owner))
		{
			Flash();
			await PowerCmd.Apply<DragonAuraPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
		}
	}
}