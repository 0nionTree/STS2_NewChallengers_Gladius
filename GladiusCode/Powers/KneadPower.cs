using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

public class KneadPower : GladiusPower
{
    // 반죽 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 턴 시작 시 진흙 생성
	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner.Player)
		{
			Flash();
			for (int i = 0; i < Amount; i++)
			{
				// 진흙 생성
				CardModel cardModel = CombatState!.CreateCard<Clay>(Owner.Player);
				// 생성한 카드 손으로 가져오기
				await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner.Player);
			}
			
		}
	}
}