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
public class TuningShard() : GladiusCard(1, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 청음편 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("ShrapnelAmount", 4m),
        new DamageVar(5m, DamageProps.card)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        ..HoverTipFactory.FromEnchantment<Shrapnel>(DynamicVars["ShrapnelAmount"].IntValue)];

    protected override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        if (artifactCard != null)
        {
            CardCmd.Enchant<Shrapnel>(artifactCard, DynamicVars["ShrapnelAmount"].IntValue);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 무작위 적에게 피해
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
			.TargetingRandomOpponents(CombatState!)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ShrapnelAmount"].UpgradeValueBy(2m);
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}