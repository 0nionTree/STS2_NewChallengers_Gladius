using MegaCrit.Sts2.Core.Entities.Relics;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using Gladius.GladiusCode;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class DimensionalPouch() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<WroughtIron>(true), 
		HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
	{
		if (player == Owner && Owner.PlayerCombatState!.TurnNumber == 1)
		{
			List<CardModel> list = new List<CardModel>();
			for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
			{
				var card = combatState.CreateCard<WroughtIron>(Owner);
				CardCmd.Upgrade(card);
				list.Add(card);
			}
			await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
		}
	}
}