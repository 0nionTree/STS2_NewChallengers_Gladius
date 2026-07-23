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

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class ThunderstruckWood() : GladiusCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 벽조목 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("SownAmount", 1m), new EnergyVar(1)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        EnergyHoverTip,
        ..HoverTipFactory.FromEnchantment<Sown>(DynamicVars["SownAmount"].IntValue)];

    public override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        if (artifactCard != null)
        {
            CardCmd.Enchant<Sown>(artifactCard, DynamicVars["SownAmount"].IntValue);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 에너지 획득
		await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SownAmount"].UpgradeValueBy(1m);
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}