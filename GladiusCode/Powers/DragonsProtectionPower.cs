using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class DragonsProtectionPower : GladiusPower
{
    // 용의 수호 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 공격 시작 시 보유 용기만큼 방어도 획득
    public override async Task BeforeAttack(AttackCommand command)
    {
        // 공격 주체가 이 효과를 보유한 대상이 아니라면 로직을 수행하지 않고 종료합니다.
		if (command.Attacker != Owner) return;
		// 해당 공격이 스탯이나 파워의 영향을 받는 공격 유형이 아니라면 종료합니다.
		if (!command.DamageProps.IsPoweredAttack()) return;
		// 공격의 발생 출처가 존재하지만 그 출처가 카드가 아닐 경우 무시하고 종료합니다.
		if (command.ModelSource != null && command.ModelSource is not CardModel) return;
        
        int blockValue = Amount;
        PowerModel? dragonAura = Owner.GetPower<DragonAuraPower>();
        if (dragonAura != null)
            blockValue *= dragonAura.Amount;
		await CreatureCmd.GainBlock(Owner, blockValue, ValueProp.Unpowered, null);
    }
}