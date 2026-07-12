using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
public class Guideline() : GladiusCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 길잡이
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("durability", 1), new CardsVar(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    protected override bool IsPlayable => PileType.Hand.GetPile(base.Owner)?.Cards?.Any(c => c.Keywords.Contains(GladiusKeywords.Artifact)) ?? false;
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손패의 연성물 검색
        var promptString = new LocString("combat_messages", "SELECT_ARTIFECT");

        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: base.Owner, 
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Artifact), 
            source: this
        )).FirstOrDefault();
        // 연성물 내구도 감소
        if (cardModel != null)
        {
            cardModel.DynamicVars["CurrentDurability"].BaseValue -= DynamicVars["durability"].IntValue;
            if (cardModel.DynamicVars["CurrentDurabilisy"].BaseValue <= 0)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);
                cardModel.DynamicVars["CurrentDurabilisy"].BaseValue = cardModel.DynamicVars["BaseDurability"].BaseValue;
            }
        }
        // 뽑을 카드 더미의 1장 선택
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(
            choiceContext, 
            PileType.Draw.GetPile(Owner), 
            Owner, 
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1)
        )).ToList();
        // 선택된 카드가 있다면, 반복문을 통해 하나씩 버리기
        if (selection != null && selection.Count > 0)
        {
            foreach (CardModel item in selection)
            {
                await CardCmd.Discard(choiceContext, item);
            }
        }
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}