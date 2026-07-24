using MegaCrit.Sts2.Core.Entities.Relics;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class CinnamonGum() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<Clay>()];

	// 카드를 버릴 때마다 점토 생성
	public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
	{
		if (card.Owner == Owner && Owner.Creature.Side == Owner.Creature.CombatState!.CurrentSide)
		{
			Flash();
			List<CardModel> list = new List<CardModel>();
			for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
			{
				list.Add(Owner.Creature.CombatState!.CreateCard<Clay>(Owner));
			}
			await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, Owner);
		}
	}
}