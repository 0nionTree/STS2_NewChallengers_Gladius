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

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class DragonOrb() : GladiusCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 용옥 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("DragonAuraPower", 2),
        new IntVar("AlchemyDragonAuraPower", 2)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromPower<DragonAuraPower>()];

    public override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        // 용기 획득
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["AlchemyDragonAuraPower"].IntValue, Owner.Creature, this);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용기 획득
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DragonAuraPower"].UpgradeValueBy(1);
        DynamicVars["AlchemyDragonAuraPower"].UpgradeValueBy(1);
    }
}