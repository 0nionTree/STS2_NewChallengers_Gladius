using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Ordain() : GladiusCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // 점지
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CalculationBaseVar(4m),
		new ExtraDamageVar(6m),
		new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
        {
            int num = PileType.Hand.GetPile(card.Owner).Cards.Count;
            CardPile? pile = card.Pile;
            if (pile != null && pile.Type == PileType.Hand)
                num--;
            return num;
        })
	    ];

    //public override IEnumerable<CardKeyword> CanonicalKeywords => [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.CalculatedDamage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        // 손의 모든 카드 버리기
		await CardCmd.Discard(choiceContext, PileType.Hand.GetPile(Owner).Cards);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(3m);
        DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}