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

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class PenForSigning() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy),
		HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

	
	private bool _activatedThisCombat;

	private bool ActivatedThisCombat
	{
		get
		{
			return _activatedThisCombat;
		}
		set
		{
			AssertMutable();
			_activatedThisCombat = value;
		}
	}

	// 전투 중 처음으로 연성한 연성물의 내구도 증가
	public override async Task OnAlchemyTriggered(CardModel artifact, CardModel material, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
		// 연성 실행자가 파워 보유자이며, 유물이 활성화 되어있다면
		if (CombatManager.Instance.IsInProgress && creator == Owner && !ActivatedThisCombat)
		{
			Flash();
			DurabilityExtensions.VarianceDurability(artifact, DynamicVars["Durability"].IntValue);
			ActivatedThisCombat = true;
		}
	}

	// 방에서 나가면 유물 활성화
	public override Task AfterRoomEntered(AbstractRoom room)
	{
		if (!(room is CombatRoom))
		{
			return Task.CompletedTask;
		}
		ActivatedThisCombat = false;
		return Task.CompletedTask;
	}
}