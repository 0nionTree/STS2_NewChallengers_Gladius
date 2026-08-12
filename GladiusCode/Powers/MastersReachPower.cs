using Gladius.GladiusCode;
using Gladius.GladiusCode.History;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class MastersReachPower : GladiusPower, IDurabilityProtector
{
    // 달인의 간격 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
	
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    // 이번 턴 남아있는 보호 회수
    private int _currentTurnProtections;

    // 파워 획득 시
    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        DurabilityProtectionManager.Register(target, this); // 매니저 등록

        // 이번 턴 사용한 소모품 카드 수 계산
        int num = DurabilityHistory.GetDurableCardsPlayedThisTurn(target.CombatState!, target);

        // 획득한 파워 수치 - 이번 턴 사용한 소모품 카드수 만큼 보호 획득
        _currentTurnProtections = (int)amount - num;

        return Task.CompletedTask;
    }

    // 파워 수치 증감 시
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 변동한 파워가 이 파워인지
        if (power != this)
			return Task.CompletedTask;

        // 이번 턴 사용한 소모품 카드 수 계산
        int num = DurabilityHistory.GetDurableCardsPlayedThisTurn(CombatState, Owner);

        // 최종 보유한 파워 수치 - 이번 턴 사용한 소모품 카드수 만큼 보호 획득
        _currentTurnProtections = Amount - num;

        return Task.CompletedTask;
    }

    // 턴 시작 시 보호 수치 초기화
	public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        _currentTurnProtections = Amount;
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

    public bool IsProtectionActive()
    {
        return _currentTurnProtections > 0;
    }

    public int GetProtectionStacks()
    {
        return _currentTurnProtections;
    }

    public void ConsumeOneStack()
    {
        // 보호 스택 감소
        _currentTurnProtections--;
    }
}