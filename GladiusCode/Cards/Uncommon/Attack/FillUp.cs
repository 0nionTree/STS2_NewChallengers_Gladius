using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class FillUp() : GladiusCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 메우기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, DamageProps.card),
        new IntVar("HandCards", 8),
        new CardsVar(2)];
        
    protected override bool ShouldGlowGoldInternal => CanDrawCard;

    private bool CanDrawCard => CardPile.GetCards(Owner, PileType.Hand).Count() > DynamicVars["HandCards"].IntValue;
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
		decimal baseValue = DynamicVars["HandCards"].IntValue;
        int count = Owner.PlayerCombatState!.Hand.Cards.Count;
        if (count >= baseValue)
        {
		    await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["HandCards"].UpgradeValueBy(-1);
    }
}