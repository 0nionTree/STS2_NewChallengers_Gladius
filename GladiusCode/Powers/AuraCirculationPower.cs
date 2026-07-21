using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

public class AuraCirculationPower : GladiusPower
{
    // 용기 순환 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    // 공격 직전에 용기가 켜져 있었는지 기억할 변수
    private bool _wasDragonAuraActive = false;

    // 1. 대미지 계산 전 (BeforeAttack)
    // 힘을 올리지는 않고, 오직 '용기(DragonAuraPower)가 존재하는지'만 확인해둡니다.
    public override Task BeforeAttack(AttackCommand command) 
    {
        // LINQ를 사용하여 용기 파워가 있는지 O(N)으로 빠르고 깔끔하게 탐색
        var dragonAura = Owner.Powers.OfType<DragonAuraPower>().FirstOrDefault();
        
        // 스택이 1 이상인지 확인하여 결과 저장
        _wasDragonAuraActive = dragonAura != null && dragonAura.Amount > 0;
        
        return Task.CompletedTask;
    }

    // 2. 공격 완료 후 (AfterAttack)
    // 대미지가 이미 들어갔으므로 여기서 힘을 올려도 이번 공격에 영향을 주지 않습니다.
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        // 아까 공격 직전에 용기가 켜져 있었다면?
        if (_wasDragonAuraActive)
        {
            Flash();
            // 오라 순환의 스택만큼 힘 증가
            await PowerCmd.Apply<StrengthPower>(
                choiceContext, 
                Owner, 
                Amount, 
                Owner, 
                null, 
                silent: true
            );
            
            // 처리 후 다음 공격을 위해 변수 초기화
            _wasDragonAuraActive = false;
        }
    }
}