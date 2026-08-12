using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Oath() : GladiusCard(1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    // 맹새 - 연성
    public override bool IsRequiredMaterial => true;

    //protected override IEnumerable<DynamicVar> CanonicalVars =>
    //    [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<TwinDragonHorns>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? a = await Alchemy<TwinDragonHorns>(choiceContext, IsUpgraded);
        /*
        if (a != null)
            a.BaseReplayCount++;
        */
    }

    //protected override void OnUpgrade()
}