using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonAuraPower : GladiusPower
{
    // 용기
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("TotalBonus", 50),
        new IntVar("ConsumAmount", 2)];

    private int _consumAmountBase = 2;
    private bool _wasActive = false;

    // 용기 수치 변화에 따라 현재 피해량 증가 수치를 수정(설명 문구용)
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        int num = 50;
        // 종이 용 보유 시 증가
        if (Owner.Player!.Relics.OfType<PaperDragon>().Any())
            num += 25;

        // 용기 포화 보유 및 용기 스택 4 이상이라면, 용기 포화 스택에 따른 피해량 증가
        var auraSaturation = Owner.GetPower<AuraSaturationPower>();
        if (Amount >= 4 && auraSaturation != null)
            num += auraSaturation.Amount * 50;

        DynamicVars["TotalBonus"].BaseValue = num;
        
        // 용신의 형상 스택에 따라 스택 소모량 변화
        var divineDragonForm = Owner.GetPower<DivineDragonFormPower>();
        if (divineDragonForm != null && divineDragonForm.Amount > 0)
            DynamicVars["ConsumAmount"].BaseValue = Math.Max(0, _consumAmountBase - divineDragonForm.Amount);

        return Task.CompletedTask;
    }

    // 대미지 증가 적용
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        decimal damage = 1m;
        // 곱연산이므로 조건에 맞지 않을 때 대미지 변동이 없게 하려면 1m(100%)을 반환.
        // 공격자과 파워 보유자가 다르면 취소
        if (Owner != dealer) return damage;
        // 공격 카드의 피해가 아니라면 취소
        if (!props.IsPoweredAttack()) return damage;
        // 용기가 소모량 미만이라면 취소
        if (Amount < DynamicVars["ConsumAmount"].IntValue) return damage;
        
        // 종이 용이 있다면 기본 배수 증가
        if (!Owner.Player!.Relics.OfType<PaperDragon>().Any())
            damage += 0.5m;
        else
            damage += 0.75m;
        
        // 용기 포화 보유 및 용기 스택 4 이상이라면, 용기 포화 스택에 따른 피해량 증가
        var auraSaturation = Owner.GetPower<AuraSaturationPower>();
        if (Amount >= 4 && auraSaturation != null)
            damage += auraSaturation.Amount * 0.5m;

        return damage;
    }

    // 공격 카드 사용 직전에 용기를 소모량 이상 보유하고 있는지 확인
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;
        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        // 스택이 소모량 이상인지 확인하여 결과 저장
        _wasActive = Amount >= DynamicVars["ConsumAmount"].IntValue;
        
        return Task.CompletedTask;
    }

    // 스택이 소모량 이상이며, 사용한 카드가 공격 카드라면 스택 2감소
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_wasActive)
            return;
        if (Amount < DynamicVars["ConsumAmount"].IntValue)
            return;
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        // 스택 감소
        await PowerCmd.ModifyAmount(choiceContext, this, -DynamicVars["ConsumAmount"].IntValue, null, null);
        // 용기 소모 이벤트 실행
        await GladiusEventDispatcher.DispatchConsumDragonAura(CombatState, cardPlay.Card, DynamicVars["ConsumAmount"].IntValue, Owner.Player, choiceContext);
    }
}