using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Wallop() : GladiusCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 맹공
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, DamageProps.card)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 대미지 실행
		AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
        // 준 피해만큼 방어력 획득
		await CreatureCmd.GainBlock(Owner.Creature, attackCommand.Results.SelectMany((List<DamageResult> r) => r).Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage), ValueProp.Move, cardPlay);
	}

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}