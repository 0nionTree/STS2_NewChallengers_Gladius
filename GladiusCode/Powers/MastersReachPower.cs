using Gladius.GladiusCode.History;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class MastersReachPower : GladiusPower, IDurabilityProtector
{
    // 달인의 간격 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

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

    // 인터페이스 구현: 이번 턴 사용한 소모품 카드 수가 스택 미만이라면 활성화
    public bool IsProtectionActive()
    {
        int num = DurabilityHistory.GetDurableCardsPlayedThisTurn(CombatState, Owner);
        if (num < Amount) return true; 
        return false;
    }
    
}