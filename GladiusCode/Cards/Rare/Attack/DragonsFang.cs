using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using BaseLib.Extensions;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsFang() : GladiusCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // 용의 이빨
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(12m, DamageProps.card),
        new PowerVar<DragonAuraPower>(2),
        new CardsVar(1)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 용기 획득
        await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars.Power<DragonAuraPower>().BaseValue, Owner.Creature, this);
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Power<DragonAuraPower>().UpgradeValueBy(1);
    }
}