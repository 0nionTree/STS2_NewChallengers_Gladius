using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

[Pool(typeof(MaterialCardPool))]
public class TuningShard() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.Self)
{
    // 청음편 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, DamageProps.card),
        new IntVar("HitCount", 2),
        new IntVar("ShrapnelAmount", 4m)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        ..HoverTipFactory.FromEnchantment<Shrapnel>(DynamicVars["ShrapnelAmount"].IntValue)];

    public override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
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
            .WithHitCount(DynamicVars["HitCount"].IntValue)
			.TargetingRandomOpponents(CombatState!)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["ShrapnelAmount"].UpgradeValueBy(2m);
    }
}