using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class GoodSign() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 길조
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Screening", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Screening),
        HoverTipFactory.FromKeyword(GladiusKeywords.Fall),
        HoverTipFactory.FromCard<ThunderstruckWood>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        ..HoverTipFactory.FromEnchantment<Sown>(IsUpgraded?3:2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 선별
        var (_, falls) = await ScreeningManager.Screening(CombatState, choiceContext, Owner, DynamicVars["Screening"].IntValue);
        // 버린 카드 더미의 카드 선택
        if (falls != null)
        {
            foreach (CardModel item in falls)
            {
                // 카드 손으로 가져오기
                await CardPileCmd.Add(item, PileType.Hand);
                await Cmd.Wait(0.2f);
                // 카드 변화
                CardPileAddResult? cardPileAddResult = await CardCmd.TransformTo<ThunderstruckWood>(item);
                // 변화된 카드가 정상적으로 존재하는지 확인
                if (cardPileAddResult.HasValue)
                {
                    CardModel cardModel = cardPileAddResult.Value.cardAdded;
                    // 이 카드가 강화되어있다면 변화한 카드 강화
                    if (IsUpgraded)
                    {
                        CardCmd.Upgrade(cardModel);
                    }
                }
            }
        }
		
    }

    protected override void OnUpgrade()
    {
        
    }
}