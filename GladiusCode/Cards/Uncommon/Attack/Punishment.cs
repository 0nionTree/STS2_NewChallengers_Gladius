using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat.History.Entries;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Punishment() : GladiusCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 응징
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CalculationBaseVar(4m),
        new ExtraDamageVar(3m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
            CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => 
            e.HappenedThisTurn(card.CombatState) && 
            e.CardPlay.Card.Type == CardType.Attack && 
            e.CardPlay.Card.Owner == card.Owner))
        ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
			.WithHitFx("vfx/vfx_attack_slash")
			.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(1m);
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}