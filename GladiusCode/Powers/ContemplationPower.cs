using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Gladius;

public class ContemplationPower : GladiusPower
{
    // 사색 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 전투 종료 후 묘약 보상 추가
	public override Task AfterCombatEnd(CombatRoom room)
	{
		if (Owner.Player == null)
			return Task.CompletedTask;
		for (int i = 0; i < Amount; i++)
		{
			PotionModel potion = ModelDb.Potion<CureAll>().ToMutable();
			room.AddExtraReward(Owner.Player, new PotionReward(potion, Owner.Player));
		}
		return Task.CompletedTask;
	}
}