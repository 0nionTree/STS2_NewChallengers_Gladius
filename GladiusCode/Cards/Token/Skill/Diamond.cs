using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class Diamond() : GladiusCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 금강석 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("AdroitAmount", 3m),
        new BlockVar(5m, BlockProps.card)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        ..HoverTipFactory.FromEnchantment<Adroit>(DynamicVars["AdroitAmount"].IntValue)];

    protected override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        if (artifactCard != null)
        {
            CardCmd.Enchant<Adroit>(artifactCard, DynamicVars["AdroitAmount"].IntValue);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AdroitAmount"].UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(2);
    }
}