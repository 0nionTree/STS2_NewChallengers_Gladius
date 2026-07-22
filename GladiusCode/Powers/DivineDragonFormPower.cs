using Gladius.GladiusCode;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Gladius;

public class DivineDragonFormPower : GladiusPower
{
    // 재구성 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability),
        HoverTipFactory.FromPower<PlatingPower>(),
		HoverTipFactory.Static(StaticHoverTip.Block)];

    // 용기 수치가 음수로 변동 시 용기 획득
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 변동한 파워가 역린인지
        if (power is not DragonAuraPower)
			return;
        // 변동한 파워 소유자가 이 파워 소유자인지
		if (power.Owner != Owner)
			return;
		// 변동 수치가 음수라면 용기 획득
        if (amount < 0)
		{
			Flash();
			await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner, Amount, Owner, null);
		}
    }
}