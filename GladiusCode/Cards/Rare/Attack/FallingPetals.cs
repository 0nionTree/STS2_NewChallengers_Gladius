using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class FallingPetals() : GladiusCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // 낙화
	private decimal _extraDamageFromPlays;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, DamageProps.card),
		new DynamicVar("Increase", 3m)];

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

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    // 용기가 소모되면 실행
    public override async Task OnConsumDragonAura(CardModel cardModel, int amount, Player player, PlayerChoiceContext choiceContext)
    {
        // 용기 소모자가 카드 소유자가 아니라면 종료
        if (player != Owner)
            return;
        
        // 피해량 증가
		DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
		ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;
        
        // 손으로 회수
        await CardPileCmd.Add(this, PileType.Hand);
    }

    protected override void AfterDowngraded()
	{
		base.AfterDowngraded();
		DynamicVars.Damage.BaseValue += ExtraDamageFromPlays;
	}

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}