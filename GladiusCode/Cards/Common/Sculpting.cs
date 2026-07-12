using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Sculpting() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 조형 - 연성
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { GladiusTags.Alchemy };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<DragonScale>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Alchemy<DragonScale>(choiceContext, IsUpgraded, DynamicVars["Durability"].IntValue);
    }

    protected override void OnUpgrade()
    {
        
    }
}