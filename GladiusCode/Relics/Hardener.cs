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

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class Hardener() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(3, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

	// 연성 시 방어도 획득
	public override async Task OnAlchemyTriggered(CardModel artifact, CardModel material, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
		if (creator == Owner)
		{
			Flash();
			await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
		}
    }
}