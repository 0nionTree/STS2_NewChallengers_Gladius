using Gladius.GladiusCode;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class ScreeningNextTurnPower : GladiusPower
{
    // 다음 턴 카드 버리고 용기 획득
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Screening),
		HoverTipFactory.FromKeyword(GladiusKeywords.Remain),
		HoverTipFactory.FromPower<DragonAuraPower>()];
        
    // 매 턴 시작 시 선별, 잔류한 카드만큼 용기 획득
	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner.Player)
		{
            var (remains, _) = await ScreeningManager.Screening(combatState, choiceContext, Owner.Player, Amount);

			if (remains != null)
            	await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner, remains.Count(), Owner, null);

			await PowerCmd.Remove(this);
		}
	}
}