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
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Random;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class Magatama() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DragonAuraPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

	// 파워 사용 시 용기 획득
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (!CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (cardPlay.Card.Owner != Owner)
		{
			return;
		}
		if (cardPlay.Card.Type != CardType.Power)
		{
			return;
		}
        Flash();
        await PowerCmd.Apply<DragonAuraPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["DragonAuraPower"].BaseValue, Owner.Creature, null);

		return;
	}
}