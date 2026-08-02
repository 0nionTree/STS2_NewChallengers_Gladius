using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class CrossCutting() : GladiusCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 십자 가르기
	private decimal _extraDamageFromPlays;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, DamageProps.card),
		new DynamicVar("Increase", 2m),
        new IntVar("HitCount", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(CardKeyword.Sly)];

    //public override IEnumerable<CardKeyword> CanonicalKeywords => [];

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
        // 피해량만큼 HitCount회 공격
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
            
        // 피해량 증가
		DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
		ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;
    
        // 뽑을 카드 더미 아래로 이동
        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Bottom);
    }

    protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
	}

    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(2m);
    }
}