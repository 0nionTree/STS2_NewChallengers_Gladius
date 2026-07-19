using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ConvergingSwirl() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 힘의 소용돌이
    public override bool IsRequiredMaterial => true;

    //protected override IEnumerable<DynamicVar> CanonicalVars => 
    //    [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<VortexSpear>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy),
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];
        
	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[CardKeyword.Innate];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 나선 투창 연성
        await Alchemy<VortexSpear>(choiceContext, IsUpgraded, 0);
    }

    protected override void OnUpgrade()
    {
        Keywords.Append(CardKeyword.Retain);
    }
}