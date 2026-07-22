using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

public class ImperialScalePower : GladiusPower
{
    // 역린 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    // 수치가 변동 시
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 변동한 파워가 역린인지
        if (power is not ImperialScalePower)
			return Task.CompletedTask;
        // 변동한 파워 소유자가 이 파워 소유자인지
		if (power.Owner != Owner)
			return Task.CompletedTask;
        
        // 모든 카드 중에 파워 보유자의 용 비늘을 찾아서 CurrentRepeat 설정
		IEnumerable<CardModel> enumerable = Owner.Player?.PlayerCombatState?.AllCards ?? [];
		foreach (CardModel item in enumerable)
		{
			TryAddRepeat(item, (int)amount);
		}
        return Task.CompletedTask;
    }

    // 전투 중 카드가 생성되었다면
    public override Task AfterCardEnteredCombat(CardModel card)
	{
        // 복제품이 아니라면
		if (card.IsClone)
			return Task.CompletedTask;
		TryAddRepeat(card, Amount);
		return Task.CompletedTask;
	}

    // 파워가 제거된 뒤
	public override Task AfterRemoved(Creature oldOwner)
	{
		IEnumerable<CardModel> enumerable = oldOwner.Player?.PlayerCombatState?.AllCards ?? Array.Empty<CardModel>();
		foreach (CardModel item in enumerable)
		{
			if (item is DragonScale dragonScale)
			{
				dragonScale.CurrentRepeat -= Amount;
			}
		}
		return Task.CompletedTask;
	}

    private bool TryAddRepeat(CardModel card, int amount)
	{
		if (card.Owner != Owner.Player)
		{
			return false;
		}
		if (card is not DragonScale dragonScale)
		{
			return false;
		}
		dragonScale.CurrentRepeat += amount;
		return true;
	}
}