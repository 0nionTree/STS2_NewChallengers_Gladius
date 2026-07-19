using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class GoldenHammer() : GladiusCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 황금 망치
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DynamicVar("Durability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손에 있는 소모품 카드 검색
        foreach (CardModel cardModel in PileType.Hand.GetPile(Owner).Cards)
        {
            // 검색한 카드가 소모품 카드라면 내구도 증가
            var durabilityData = cardModel.GetDurability();
            if (durabilityData.isDurable)
            {
                DurabilityExtensions.VarianceDurability(cardModel, DynamicVars["Durability"].IntValue);
            }
        }
    }

    protected override void OnUpgrade()
    {
        Keywords.Append(CardKeyword.Retain);
    }
}