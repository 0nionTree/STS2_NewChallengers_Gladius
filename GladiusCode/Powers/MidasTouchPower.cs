using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

public class MidasTouchPower : GladiusPower
{
    // 미다스의 손 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter; 

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
	{
		if (card.Type != CardType.Attack)
            return;
        if (card.GetDurability().isDurable)
            return;

        card.EnergyCost.AddThisCombat(-1);

        var durability = card.GetDurability();
        
        durability.isDurable = true;
        durability.BaseDurability = Amount;
        durability.CurrentDurability = durability.BaseDurability;
        durability.WasDurability = durability.BaseDurability;

        if (CombatState != null)
        {
            await AlchemyEventDispatcher.DispatchAlchemyTriggered(CombatState, card, null, Owner.Player, choiceContext, false);
        }

		return;
	}
}