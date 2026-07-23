using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Gladius;

public class GrindingPower : GladiusPower
{
    // 연마 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 연성한 카드가 인챈트되어있지 않다면 예리 인챈트
    public override async Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
		// 연성 실행자가 파워 보유자가 아니라면 종료
		if (creator != Owner.Player) return;
		// 생성한 연성물이 존재하지 않으면 종료
		if (artifact == null) return;
		// 생성한 연성물의 소유자가 파워 보유자가 아니라면 종료
		if (artifact.Owner != Owner.Player) return;
		// 생성한 연성물이 인챈트 되어있다면 종료
		if (artifact.Enchantment != null) return;

        if (!(Amount <= 0m))
		{
			CardCmd.Enchant<Sharp>(artifact, Amount);
		}
    }
}