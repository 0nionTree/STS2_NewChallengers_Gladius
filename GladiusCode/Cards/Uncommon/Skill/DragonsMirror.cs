using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsMirror() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 길잡이
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Discard", 1),
        new CardsVar(2)];
    
    //protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 뽑을 카드 더미의 1장 선택
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(
            choiceContext, 
            PileType.Draw.GetPile(Owner), 
            Owner, 
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars["Discard"].IntValue)
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