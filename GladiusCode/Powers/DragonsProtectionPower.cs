using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonsProtectionPower : GladiusPower
{
    // 용의 수호 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
	
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    // 용기 보유 확인용
    private bool _wasDragonAuraActive = false;

    // 공격 카드 사용 직전에 용기를 보유하고 있는지 확인
    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return Task.CompletedTask;
        if (cardPlay.Card.Type != CardType.Attack)
            return Task.CompletedTask;

        var dragonAura = Owner.GetPower<DragonAuraPower>();

        // 용기를 소모량 이상 보유 여부 저장
        _wasDragonAuraActive = dragonAura != null && dragonAura.Amount >= dragonAura.DynamicVars["ConsumAmount"].IntValue;
        
        return Task.CompletedTask;
    }
    
    // 공격 카드 사용 직후 방어도 획득
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!_wasDragonAuraActive)
            return;
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Type != CardType.Attack)
            return;
        
        Flash();
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        
        // 처리 후 다음 체크를 위해 초기화
        _wasDragonAuraActive = false;
    }
}