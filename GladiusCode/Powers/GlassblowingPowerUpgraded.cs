using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class GlassblowingPowerUpgraded : GladiusPower
{
    // 회수 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 카드를 버린 뒤 비용0에 휘발성인 청음편 생성
    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
	{
        if (card.Owner.Creature == Owner)
        {
			Flash();
            for (int i = 0; i < Amount; i++)
			{
				// 청음편 생성
				CardModel cardModel = CombatState!.CreateCard<TuningShard>(Owner.Player!);
                CardCmd.Upgrade(cardModel);
                cardModel.EnergyCost.SetThisCombat(0);
                cardModel.AddKeyword(CardKeyword.Ethereal);
				// 생성한 카드 손으로 가져오기
				await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner.Player);
			}
        }
	}
}