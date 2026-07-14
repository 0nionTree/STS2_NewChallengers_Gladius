using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Welding() : GladiusCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 용접
    public override bool IsRequiredDurable => true;
    public override int RequiredDurableCards => 2;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 3)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 카드 선택 안내 문구 지정
        var promptString1 = new LocString("combat_messages", "SELECT_DURABLE");
        var promptString2 = new LocString("combat_messages", "SELECT_ANOTHER_DURABLE");
		// 손에 있는 내구도가 존재하는 카드 선택
        var cardModel1 = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString1, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.GetCustomData().isDurable, 
            source: this
        )).FirstOrDefault();

        if (cardModel1 != null)
        {
            int durability;
            var customData = cardModel1.GetCustomData();

            // 선택한 카드의 내구도가 n 이상인지 확인
            if (customData.CurrentDurability >= DynamicVars["Durability"].IntValue)
                // 내구도 n 이상이면 n 저장
                durability = DynamicVars["Durability"].IntValue;
            else
                // 내구도 n 미만이면 현재 수치만큼 저장
                durability = customData.CurrentDurability;
            
            // 저장된 수치만큼 내구도 감소
            await DurabilityExtensions.VarianceDurability(cardModel1, -durability, choiceContext);

            // 선택한 카드 이외의 내구도가 존재하는 카드 선택
            var cardModel2 = (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(promptString2, 1), // 안내 문구(promptString) 변경 추천
                context: choiceContext, 
                player: Owner, 
                // 핵심: 내구도가 존재하면서(isDurable) AND 첫 번째 카드(cardModel)가 아닌 카드만 필터링
                filter: (CardModel card) => card.GetCustomData().isDurable && card != cardModel1, 
                source: this
            )).FirstOrDefault();

            if (cardModel2 != null)
            {
                // 두 번째로 선택한 카드의 내구도를 저장된 수치만큼 증가
                await DurabilityExtensions.VarianceDurability(cardModel2, durability, choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}