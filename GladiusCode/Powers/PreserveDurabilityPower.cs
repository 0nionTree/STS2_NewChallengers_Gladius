using Gladius.GladiusCode;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class PreserveDurabilityPower : GladiusPower, IDurabilityProtector
{
    // 이번 턴 내구도 보호
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public bool IsProtectionActive() => true;

    // 파워 획득 시
    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        DurabilityProtectionManager.Register(target, this); // 매니저 등록

        return Task.CompletedTask;
    }

    // 파워 소멸 시
    public override Task AfterRemoved(Creature oldOwner) 
    {
        DurabilityProtectionManager.Unregister(oldOwner, this); // 매니저 해제

        return Task.CompletedTask;
    }

    // 방에서 떠날 시
    public override Task AfterCombatEnd(CombatRoom room)
	{
        DurabilityProtectionManager.Unregister(Owner, this); // 매니저 해제

		return Task.CompletedTask;
	}

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.GetDurability().isDurable)
        {
            await PowerCmd.Decrement(this);
        }
    }

    // 턴 종료 시 제거
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
		{
            await PowerCmd.Remove(this);
		}
    }
}