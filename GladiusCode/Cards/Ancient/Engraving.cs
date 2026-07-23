using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Engraving() : GladiusCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // 세공
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(9m, BlockProps.card)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromCard<TuningShard>(IsUpgraded),
        ..HoverTipFactory.FromEnchantment<Shrapnel>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        // 청음편 생성
        CardModel cardModel = CombatState!.CreateCard<TuningShard>(Owner);
        if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
        {
            CardCmd.Upgrade(cardModel);
        }
        // 생성한 카드 손으로 가져오기
        await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		await Cmd.Wait(0.2f);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}