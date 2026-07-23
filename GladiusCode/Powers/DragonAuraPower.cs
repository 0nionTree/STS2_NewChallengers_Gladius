using System.Buffers;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonAuraPower : GladiusPower
{
    // 용기
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
        if (!Owner.Player!.Relics.OfType<PaperDragon>().Any())
            DynamicVars["TotalBonus"].BaseValue = Amount * 20m;
        else
            DynamicVars["TotalBonus"].BaseValue = Amount * 25m;
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }

    // 공격이 시작될 때 어떤 공격인지 추적
    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner) return Task.CompletedTask;
        if (!command.DamageProps.IsPoweredAttack()) return Task.CompletedTask;

        Data internalData = GetInternalData<Data>();
        if (internalData.commandToModify != null) return Task.CompletedTask;
        if (command.ModelSource != null && command.ModelSource is not CardModel) return Task.CompletedTask;
        if (!command.DamageProps.IsPoweredAttack()) return Task.CompletedTask;

        internalData.commandToModify = command;
        internalData.amountWhenAttackStarted = Amount;
        return Task.CompletedTask;
    }

    // 대미지 증가 적용
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 곱연산이므로 조건에 맞지 않을 때 대미지 변동이 없게 하려면 1m(100%)을 반환해야 합니다.
        if (Owner != dealer) return 1m; 
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

        // 스택(Amount) 1당 20%(0.2배) 증가
        // 예: 스택이 3이라면 -> 1m + (3 * 0.2m) = 1.6m (160%)
        if (!Owner.Player!.Relics.OfType<PaperDragon>().Any())
            return 1m + (Amount * 0.2m);
        else
            return 1m + (Amount * 0.25m);
    }

    // 공격 종료 시 처리
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        Data internalData = GetInternalData<Data>();
        if (command == internalData.commandToModify)
        {
            // 스택 1 감소
            await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
            
            // 다음 공격 카드 발동을 위해 추적 초기화
            internalData.commandToModify = null; 
        }
    }

    // 턴 종료 시 스택 1 감소 추가
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
		{
            await PowerCmd.Decrement(this);
		}
    }
}