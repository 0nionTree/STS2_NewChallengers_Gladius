using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ExorcismRitual() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 퇴마 의식 - 연성
    public override bool IsRequiredMaterial => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Durability", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<PurifyingLantern>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromPower<VulnerablePower>()];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Alchemy<PurifyingLantern>(choiceContext, false, DynamicVars["Durability"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Durability"].UpgradeValueBy(1);
    }
}