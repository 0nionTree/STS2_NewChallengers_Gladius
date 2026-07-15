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
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Guideline() : GladiusCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 길잡이
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 1), new CardsVar(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손의 내구도가 있는 카드 검색
        var promptString = new LocString("combat_messages", "SELECT_ARTIFECT");

        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: base.Owner, 
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Artifact), 
            source: this
        )).FirstOrDefault();

        // 선택된 카드가 있다면
        if (cardModel != null)
        {
            await DurabilityExtensions.VarianceDurability(cardModel, -DynamicVars["Durability"].IntValue, choiceContext);

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
        // 내구도가 있는 카드가 없다면
        else
        {
            // 내구도 카드가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "DURABLES_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}