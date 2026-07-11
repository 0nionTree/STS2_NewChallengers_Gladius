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
public class ScoopingUp() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 퍼올리기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, BlockProps.card), new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Material), HoverTipFactory.FromCard<Clay>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            // 진흙 생성
            CardModel cardModel = CombatState!.CreateCard<Clay>(Owner);
            // 생성한 카드 손으로 가져오기
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
            await Cmd.Wait(0.1f);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}