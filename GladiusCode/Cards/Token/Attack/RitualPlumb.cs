using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius;

[Pool(typeof(ArtifactCardPool))]
public class RitualPlumb() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // 의례 추 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 2;

    protected override bool ShouldGlowGoldInternal => 
        this.GetDurability().WasDurability == 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, DamageProps.card)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];
        
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damage = DynamicVars.Damage.BaseValue;
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        if (this.GetDurability().WasDurability == 1)
		{
			damage *= 2;
            // 피해량 계산 및 이펙트 출력
            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
                .Execute(choiceContext);
		}
        else
        {
            // 피해량 계산 및 이펙트 출력
            await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}