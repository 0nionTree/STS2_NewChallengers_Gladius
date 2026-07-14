using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Hammering() : GladiusCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 망치질
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, DamageProps.card)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 카드 강화
        if (IsUpgraded)
		{
			foreach (CardModel item in PileType.Hand.GetPile(Owner).Cards.Where((CardModel c) => c.IsUpgradable))
			{
				CardCmd.Upgrade(item);
			}
			return;
		}
		CardModel? cardModel = await CardSelectCmd.FromHandForUpgrade(choiceContext, Owner, this);
		if (cardModel != null)
		{
			CardCmd.Upgrade(cardModel);
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}