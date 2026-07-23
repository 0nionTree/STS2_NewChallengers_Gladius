using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class Steel() : GladiusCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 강철 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, BlockProps.card),
        new IntVar("Durability", 1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    public override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        // 생성된 카드가 존재할 경우
        if (artifactCard != null)
        {
            // 내구도 증가
            artifactCard.GetDurability().CurrentDurability += DynamicVars["Durability"].IntValue;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["Durability"].UpgradeValueBy(1);
    }
}