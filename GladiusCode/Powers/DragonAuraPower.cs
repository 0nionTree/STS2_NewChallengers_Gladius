using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonAuraPower : GladiusPower
{
    
    private class Data
    {
        public AttackCommand? commandToModify;
        public int amountWhenAttackStarted;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("TotalBonus", 20m)];

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        DynamicVars["TotalBonus"].BaseValue = Amount * 20m;
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    // 1. 공격이 시작될 때 어떤 공격인지 추적 (수정 없음, 원본 유지)
    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != base.Owner) return Task.CompletedTask;
        if (!command.DamageProps.IsPoweredAttack()) return Task.CompletedTask;

        Data internalData = GetInternalData<Data>();
        if (internalData.commandToModify != null) return Task.CompletedTask;
        if (command.ModelSource != null && !(command.ModelSource is CardModel)) return Task.CompletedTask;
        if (!command.DamageProps.IsPoweredAttack()) return Task.CompletedTask;

        internalData.commandToModify = command;
        internalData.amountWhenAttackStarted = base.Amount;
        return Task.CompletedTask;
    }

    // 2. 대미지 증가 적용 (합연산 -> 곱연산으로 변경)
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 곱연산이므로 조건에 맞지 않을 때 대미지 변동이 없게 하려면 1m(100%)을 반환해야 합니다.
        if (base.Owner != dealer) return 1m; 
        if (!props.IsPoweredAttack()) return 1m;

        Data internalData = GetInternalData<Data>();
        if (internalData.commandToModify != null && cardSource != null && cardSource != internalData.commandToModify.ModelSource)
        {
            return 1m;
        }
        if (internalData.commandToModify != null && internalData.commandToModify.Attacker != dealer)
        {
            return 1m;
        }

        // 스택(base.Amount) 1당 20%(0.2배) 증가
        // 예: 스택이 3이라면 -> 1m + (3 * 0.2m) = 1.6m (160%)
        return 1m + ((decimal)base.Amount * 0.2m);
    }

    // 3. 공격 종료 시 처리 (스택 전체 소모 -> 1 소모로 변경)
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        Data internalData = GetInternalData<Data>();
        if (command == internalData.commandToModify)
        {
            // 가지고 있던 스택 전체(-internalData.amountWhenAttackStarted)를 빼는 대신, -1만 뺍니다.
            await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
            
            // 다음 공격 카드 발동을 위해 추적 초기화
            internalData.commandToModify = null; 
        }
    }

    // 4. 턴 종료 시 스택 1 감소 추가
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(base.Owner))
		{
            await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
		}
    }
}