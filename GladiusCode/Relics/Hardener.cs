using MegaCrit.Sts2.Core.Entities.Relics;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class Hardener() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(2, ValueProp.Unpowered)];

    //protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    //    [];

    // 소모품 카드 사용 시 방어도 획득
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		if (cardPlay.Card.Owner == Owner && cardPlay.Card.GetDurability().isDurable)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
		}
    }
}