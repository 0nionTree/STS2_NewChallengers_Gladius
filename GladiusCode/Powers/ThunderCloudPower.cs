using BaseLib.Patches.Localization;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

public class ThunderCloudPower : GladiusPower
{
    // 벼락 구름 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 턴 종료 시 카드를 선택하여 벽조목으로 변환
	public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState!, player))
		{
			return;
		}
		List<CardModel> list = (await CardSelectCmd.FromHand(
			prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, Amount),
			context: choiceContext,
			player: Owner.Player,
			filter: null,
			source: this
			)).ToList();
		if (list.Count == 0)
		{
			return;
		}
		foreach (CardModel item in list)
		{
			CardModel cardModel = CombatState!.CreateCard<ThunderstruckWood>(Owner.Player);
			await CardCmd.Transform(item, cardModel);
		}
	}
}