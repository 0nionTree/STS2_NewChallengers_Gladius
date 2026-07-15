using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Possession() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 깃들이기
    public override bool IsRequiredMaterial => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<DragonClaw>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용 발톱 연성
        await Alchemy<DragonClaw>(choiceContext, IsUpgraded);
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        
    }
}