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
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Byproduct() : GladiusCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 부산물
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, DamageProps.card),
        new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
	{
        
		if (creator != Owner)
		{
			return Task.CompletedTask;
		}
		if (card.Owner != Owner)
		{
			return Task.CompletedTask;
		}
		if (!card.Keywords.Contains(GladiusKeywords.Artifact))
		{
			return Task.CompletedTask;
		}
        EnergyCost.AddUntilPlayed(-1);
		return Task.CompletedTask;
	}

    public override Task OnAlchemyTriggered()
    {   // 연성 시 실행되는 함수 (생성 카드 등의 정보 받아오도록 수정 필요)
        // 사용하기 전까지 비용 1 감소
        EnergyCost.AddUntilPlayed(-1);
        
        return Task.CompletedTask; 
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}