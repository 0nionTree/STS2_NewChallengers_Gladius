using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class WhatIsThis() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 털어내기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, BlockProps.card),
        new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        // 무작위 소재 카드 생성
        CardModel? cardModel = CardFactory.GetDistinctForCombat(Owner, ModelDb.CardPool<MaterialCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint),
            1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();

		if (cardModel != null)
		{
            // 강화 상태라면 생성한 카드 강화
			if (IsUpgraded)
			{
				CardCmd.Upgrade(cardModel);
			}
            // 생성한 카드 손에 추가
			await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}