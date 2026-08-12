using Gladius.GladiusCode;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class SavingPower : GladiusPower
{
    // 절약 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
	
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];

    // 연성 시 방어도 획득
    public override async Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
		// 연성 실행자가 파워 보유자가 아니라면 종료
		if (creator != Owner.Player) return;

        if (Amount > 0)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
		}
    }
}