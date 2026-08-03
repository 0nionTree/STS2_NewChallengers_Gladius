using System.Buffers;
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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonAuraPower : GladiusPower
{
    // 용기
    /*
    private class Data
    {
        public AttackCommand? commandToModify;
        public int amountWhenAttackStarted;
    }*/

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("TotalBonus", 50m)];
/*
    protected override object InitInternalData()
    {
        return new Data();
    }
*/
/*
    // 용기 수치 변화에 따라 현재 피해량 증가 수치를 수정(설명 문구용)
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!Owner.Player!.Relics.OfType<PaperDragon>().Any())
            DynamicVars["TotalBonus"].BaseValue = Amount * 20m;
        else
            DynamicVars["TotalBonus"].BaseValue = Amount * 25m;
        return base.AfterPowerAmountChanged(choiceContext, power, amount, applier, cardSource);
    }
*/
/*
    // 공격이 시작될 때 어떤 공격인지 추적
    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.Attacker != Owner) return Task.CompletedTask;
        if (!command.DamageProps.IsPoweredAttack()) return Task.CompletedTask;

        Data internalData = GetInternalData<Data>();
        if (internalData.commandToModify != null) return Task.CompletedTask;
        if (command.ModelSource != null && command.ModelSource is not CardModel) return Task.CompletedTask;

        internalData.commandToModify = command;
        internalData.amountWhenAttackStarted = Amount;
        return Task.CompletedTask;
    }
*/
    // 대미지 증가 적용
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        decimal damage = 1m;
        // 곱연산이므로 조건에 맞지 않을 때 대미지 변동이 없게 하려면 1m(100%)을 반환.
        // 용기가 2 이하라면 취소
        if (Amount < 2) return damage;
        // 피해를 준 대상과 파워 보유자가 다르면 취소
        if (Owner != dealer) return damage;
        // 공격 카드의 피해가 아니라면 취소
        if (!props.IsPoweredAttack()) return damage;
/*
        Data internalData = GetInternalData<Data>();
        if (internalData.commandToModify != null && cardSource != null && cardSource != internalData.commandToModify.ModelSource)
        {
            return 1m;
        }
        if (internalData.commandToModify != null && internalData.commandToModify.Attacker != dealer)
        {
            return 1m;
        }*/
        
        // 종이 용이 있다면 기본 배수 증가
        if (!Owner.Player!.Relics.OfType<PaperDragon>().Any())
            damage += 0.5m;
        else
            damage += 0.75m;
        
        // 용기 포화 보유 및 용기 스택 4 이상이라면, 용기 포화 스택에 따른 피해량 증가
        // if (Amount < 4 && ...)

        return damage;
    }/*

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
    }*/
/*
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        
    }*/

    // 스택 2 이상이며, 사용한 카드가 공격 카드라면 스택 2감소
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int consum = 2;
        if (Amount < 2)
            return;
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        // 스택 감소
        await PowerCmd.ModifyAmount(choiceContext, this, -consum, null, null);
        // 용기 소모 이벤트 실행
        await GladiusEventDispatcher.DispatchConsumDragonAura(CombatState, cardPlay.Card, consum, Owner.Player, choiceContext);
    }
/*
    // 턴 종료 시 스택 1 감소
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
		{

            // 용기 분출 보유 시 용기 보유량만큼 용기 감소
            EruptionPower? eruptionPower = Owner.GetPower<EruptionPower>();
            if (eruptionPower != null && eruptionPower.Amount > 0)
            {
                eruptionPower.Flashing();
                await PowerCmd.Remove(eruptionPower);
                await PowerCmd.ModifyAmount(choiceContext, this, Amount, null, null);
            }
            else
                await PowerCmd.Decrement(this);
		}
    }
*/
}