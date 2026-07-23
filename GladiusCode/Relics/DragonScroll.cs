using MegaCrit.Sts2.Core.Entities.Relics;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rooms;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class DragonScroll() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Common;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<DragonAuraPower>(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

	public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
			Flash();
			await PowerCmd.Apply<DragonAuraPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["DragonAuraPower"].BaseValue, Owner.Creature, null);
		}
	}
}