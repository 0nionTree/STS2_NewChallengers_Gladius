using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Hooks;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class WroughtIron() : GladiusCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 연철 - 소재
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(4, BlockProps.card),
        new BlockVar("AlchemyBlock", 2, BlockProps.card)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars["AlchemyBlock"], null);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["AlchemyBlock"].UpgradeValueBy(1);
    }
}