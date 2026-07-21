using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class CullingPower : GladiusPower
{
    // 선별 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	private int drawCardBonus = 0;

    // 매 턴 시작 시 뽑을 카드 더미에서 카드 버리기, 이후 그 카드의 비용만큼 추가 드로우
	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner.Player)
		{
			await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner.Player);
			List<CardModel> selection = (await CardSelectCmd.FromCombatPile(
            	choiceContext, 
            	PileType.Draw.GetPile(Owner.Player), 
            	Owner.Player, 
            	new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, Amount)
				)).ToList();
			// 선택된 카드가 있다면, 반복문을 통해 하나씩 버리기
			if (selection != null && selection.Count > 0)
			{
				foreach (CardModel item in selection)
				{
					await CardCmd.Discard(choiceContext, item);
					drawCardBonus += item.EnergyCost.GetAmountToSpend();
				}
			}
		}
	}

	public override decimal ModifyHandDraw(Player player, decimal count)
	{
		if (player != Owner.Player)
		{
			return count;
		}
		decimal value = count + drawCardBonus;
		drawCardBonus = 0;
		return value;
	}
}