using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

public class AuraCirculationPower : GladiusPower
{
    // 용기 순환 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    // 공격 직전에 용기가 켜져 있었는지 기억할 변수
    private bool _wasDragonAuraActive = false;

    // 공격 카드 사용 직전에 용기를 보유하고 있는지 확인
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;
        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        PowerModel? dragonAura = Owner.GetPower<DragonAuraPower>();

        // 스택이 1 이상인지 확인하여 결과 저장
        _wasDragonAuraActive = dragonAura != null && dragonAura.Amount > 0;
        
        return Task.CompletedTask;
    }
    
    // 공격 카드 사용 직후 힘 증가
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_wasDragonAuraActive)
            return;
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        
        Flash();
        // 용기 순환의 스택만큼 힘 증가
        await PowerCmd.Apply<StrengthPower>(
            choiceContext, 
            Owner, 
            Amount, 
            Owner, 
            null, 
            silent: true
        );
        
        // 처리 후 다음 체크를 위해 초기화
        _wasDragonAuraActive = false;
    }
}