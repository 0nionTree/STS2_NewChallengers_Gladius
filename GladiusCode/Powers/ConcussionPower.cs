using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class ConcussionPower : GladiusPower
{
    // 뒤흔들기 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 매 턴 첫 공격이 공격하는 적에게는 약화, 아닌 적에게는 취약 부여
	public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult _, ValueProp props, Creature target, CardModel? cardSource)
	{
		// 대상이 적이 아니라면 종료
		if (target.Monster != null)
		{
			// 이번 턴 사용한 공격 카드의 수 저장
			int num = CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) =>
				e.HappenedThisTurn(CombatState) &&
				e.CardPlay.Card.Type == CardType.Attack &&
				e.CardPlay.Card.Owner.Creature == Owner);
			// 사용한 공격 카드가 없고, 가해자가 파워 보유자일 경우
			if (num == 0 && dealer == Owner && props.IsPoweredAttack())
			{
				// 대상 적이 공격할 의도가 있다면 약화 부여
				if (target.Monster.IntendsToAttack)
					await PowerCmd.Apply<WeakPower>(choiceContext, target, Amount, Owner, null);
				// 아니라면 취약 부여
				else
					await PowerCmd.Apply<VulnerablePower>(choiceContext, target, Amount, Owner, null);
			}
		}
		
	}
}