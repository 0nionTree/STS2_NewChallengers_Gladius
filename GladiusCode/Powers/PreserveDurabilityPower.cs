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

    private CardModel? _applyCard;

    public bool IsProtectionActive() => true;

    // 파워 획득 시
    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        DurabilityProtectionManager.Register(target, this); // 매니저 등록
        // 증가시킨 카드가 존재하는지
        if (cardSource == null)
            return Task.CompletedTask;

        // 증가시킨 카드를 저장
        _applyCard = cardSource;

        return Task.CompletedTask;
    }

    // 이 파워 수치를 증가시킨 카드를 저장
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 변동한 파워가 이 파워인지
        if (power != this)
			return Task.CompletedTask;
        // 변동한 파워 소유자가 이 파워 소유자인지
		if (power.Owner != Owner)
			return Task.CompletedTask;
		// 변동 수치가 양수인지
        if (amount > 0)
            return Task.CompletedTask;
        // 증가시킨 카드가 존재하는지
        if (cardSource == null)
            return Task.CompletedTask;
            
        // 증가시킨 카드를 저장
        _applyCard = cardSource;
        
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

    // 카드 사용 이후 수치 감소
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 사용한 카드가 파워 획득시킨 카드가 아니며, 소모품 카드라면 파워 감소
        if (cardPlay.Card != _applyCard && cardPlay.Card.GetDurability().isDurable)
        {
            await PowerCmd.Decrement(this);
        }
        // 저장한 카드 초기화
        _applyCard = null;
    }

    // 턴 종료 시 제거
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
		{
            await PowerCmd.Remove(this);
		}
        _applyCard = null;
    }
}