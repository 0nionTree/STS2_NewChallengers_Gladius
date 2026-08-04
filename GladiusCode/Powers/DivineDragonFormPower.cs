using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

public class DivineDragonFormPower : GladiusPower
{
    // 용신의 형상 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    // 껍데기 파워. 용기 소모량 변경은 DragonAuraPower.cs 에서 계산
}