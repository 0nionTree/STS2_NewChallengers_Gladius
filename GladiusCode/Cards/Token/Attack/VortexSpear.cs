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

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class VortexSpear() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // 나선 투창 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 1;

	private decimal _extraDamageFromPlays;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, DamageProps.card),
        new DynamicVar("Increase", 3)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];
        
    private decimal ExtraDamageFromPlays
	{
		get
		{
			return _extraDamageFromPlays;
		}
		set
		{
			AssertMutable();
			_extraDamageFromPlays = value;
		}
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner != Owner)
			return Task.CompletedTask;
        if (!cardPlay.Card.Keywords.Contains(GladiusKeywords.Artifact))
            return Task.CompletedTask;
        if (cardPlay.Card == this)
            return Task.CompletedTask;

		DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
		ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;

		return Task.CompletedTask;
	}

    protected override void AfterDowngraded()
	{
		AfterDowngraded();
		DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
	}

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Increase"].UpgradeValueBy(1m);
    }
}