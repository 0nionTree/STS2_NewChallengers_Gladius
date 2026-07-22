using Gladius.GladiusCode;
using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Gladius;

public class SamsaraPowerUpgraded : GladiusPower
{
    // 윤회 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];

    // 연성 시 복사본 생성
	public override async Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator)
    {
		// 연성 실행자가 파워 보유자가 아니라면 종료
		if (creator != Owner.Player) return;

        for (int i = 0; i < Amount; i++)
		{
            CardModel card = artifact.CreateClone();
			DurabilityExtensions.SetDurability(card, 1);
			card.AddKeyword(CardKeyword.Ethereal);
            card.EnergyCost.SetThisCombat(0);
			await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player);
		}
    }
}