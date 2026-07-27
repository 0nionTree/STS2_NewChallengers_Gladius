using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonsProtectionPower : GladiusPower
{
    // 용의 수호 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 공격 직전에 켜진 용기의 수치를 기록할 변수
    private int _wasDragonAuraAmount = 0;

    // 공격 카드 사용 직전에 용기를 보유하고 있는지 확인
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;
        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        PowerModel? dragonAura = Owner.GetPower<DragonAuraPower>();

        // 스택을 확인하여 결과 저장
        _wasDragonAuraAmount = dragonAura != null ? dragonAura.Amount : 0;
        
        return Task.CompletedTask;
    }
    
    // 공격 카드 사용 직후 방어도 획득
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_wasDragonAuraAmount <= 0)
            return;
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        
        Flash();
        // 방어도 획득
        int blockValue = Amount * _wasDragonAuraAmount;
        await CreatureCmd.GainBlock(Owner, blockValue, ValueProp.Unpowered, null);
        
        // 처리 후 다음 체크를 위해 초기화
        _wasDragonAuraAmount = 0;
    }
}