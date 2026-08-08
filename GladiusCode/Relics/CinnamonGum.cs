using MegaCrit.Sts2.Core.Entities.Relics;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Gladius;

[BaseLib.Utils.Pool(typeof(GladiusRelicPool))]
public class CinnamonGum() : GladiusCode.Relics.GladiusRelic {
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    //protected override IEnumerable<DynamicVar> CanonicalVars =>
    //    [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<Clay>()];
	
    // 선별 실행 시 카드가 추락할 때마다 점토 생성 후 손에 추가
    public override async Task OnScreenedCardsMoved(CardModel cardModel, bool isRemain, Player owner, PlayerChoiceContext choiceContext)
    {
		if (owner != Owner) return;
		if (isRemain) return;

		var clay = Owner.Creature.CombatState!.CreateCard<Clay>(Owner);

		await CardPileCmd.AddGeneratedCardToCombat(clay, PileType.Hand, Owner);

		await Cmd.Wait(0.2f);

        return;
    }
}