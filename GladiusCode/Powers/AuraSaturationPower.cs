using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

public class AuraSaturationPower : GladiusPower
{
    // 용기 포화 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 껍데기 파워. 실질적인 계산은 DragonAuraPower.cs에서
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("TotalBonus", 50m)];

    // 스택에 따라 표기용 수치 변경
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power != this)
            return Task.CompletedTask;

        DynamicVars["TotalBonus"].BaseValue = Amount * 50m;
        return Task.CompletedTask;
    }
}