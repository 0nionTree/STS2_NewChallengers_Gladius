using MegaCrit.Sts2.Core.Entities.Relics;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Gladius.GladiusCode.History;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class CarvingKnife() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Shop;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Screening", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Screening)];

	public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner && Owner.PlayerCombatState!.TurnNumber == 1)
		{
			// 선별
			await ScreeningManager.Screening(combatState, choiceContext, Owner, DynamicVars["Screening"].IntValue);
		}
	}
}