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

[Pool(typeof(TokenCardPool))]
public class DragonClaw() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // 용 발톱 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CalculationBaseVar(10m),
		new ExtraDamageVar(2m),
		new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
            card.Owner.Creature?.GetPowerAmount<DragonAuraPower>() ?? 0)
	    ];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];
        
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}