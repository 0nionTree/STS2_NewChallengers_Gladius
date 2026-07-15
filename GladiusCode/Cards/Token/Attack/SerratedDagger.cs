using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class SerratedDagger() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // 톱날 단검 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(3m, DamageProps.card),
        new CalculationBaseVar(0m),
		new CalculationExtraVar(1m),
		new CalculatedVar("CalculatedHits").WithMultiplier((CardModel card, Creature? _) => 
            card.GetCustomData().WasDurability
            )
        ];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];
        
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(this.GetCustomData().WasDurability).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}