using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Keyboard() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 건반
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<TuningShard>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = 0;

        // 뽑을 카드 더미 카드 선택
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(
            choiceContext, 
            PileType.Draw.GetPile(Owner), 
            Owner, 
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, DynamicVars.Cards.IntValue)
        )).ToList();
        // 선택된 카드가 있다면, 반복문을 통해 하나씩 버리기
        if (selection != null && selection.Count > 0)
        {
            count = selection.Count;
            foreach (CardModel item in selection)
            {
                await CardCmd.Discard(choiceContext, item);
            }
        }

        // 버린 카드 수만큼 반복
        for (int i = 0; i < count; i++)
        {
            // 청음편 생성
            CardModel cardModel = CombatState!.CreateCard<TuningShard>(Owner);
            if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
                CardCmd.Upgrade(cardModel);
            // 생성한 카드 손으로 가져오기
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
        }
    }

    //protected override void OnUpgrade()
}