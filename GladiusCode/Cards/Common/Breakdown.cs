using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Breakdown() : GladiusCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 깨뜨리기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 1), new DamageVar(12m, DamageProps.card)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    protected override bool IsPlayable => PileType.Hand.GetPile(Owner)?.Cards?.Any(c => c.Keywords.Contains(GladiusKeywords.Artifact)) ?? false;
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var promptString = new LocString("combat_messages", "SELECT_ARTIFECT");

        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Artifact), 
            source: this
        )).FirstOrDefault();

        if (cardModel != null)
        {
            cardModel.DynamicVars["CurrentDurability"].BaseValue -= DynamicVars["Durability"].IntValue;
            if (cardModel.DynamicVars["CurrentDurability"].BaseValue <= 0)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);
                cardModel.DynamicVars["CurrentDurability"].BaseValue = cardModel.DynamicVars["BaseDurability"].BaseValue;
            }
        }
        
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}