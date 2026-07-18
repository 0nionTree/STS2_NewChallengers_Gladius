using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ExtremeSpeed() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 신속
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, BlockProps.card),
        new CardsVar(1)];

    //protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    //    [];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Sly];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        // 뽑을 카드 더미의 1장 선택
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(
            choiceContext, 
            PileType.Draw.GetPile(Owner), 
            Owner, 
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, DynamicVars.Cards.IntValue)
        )).ToList();
        // 선택된 카드가 있다면, 반복문을 통해 하나씩 버리기
        if (selection != null && selection.Count > 0)
        {
            foreach (CardModel item in selection)
            {
                await CardCmd.Discard(choiceContext, item);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}