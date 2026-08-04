using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class GoldenHammer() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 황금 망치
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new DynamicVar("Durability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 미강화 1장 선택
        if (!IsUpgraded)
        {
            // 카드 선택 안내 문구 지정
            var promptString = new LocString("combat_messages", "SELECT_DURABLE");
            // 손에 있는 내구도가 존재하는 카드 선택
            var cardModel = (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(promptString, 1), 
                context: choiceContext, 
                player: Owner, 
                filter: (CardModel card) => card.GetDurability().isDurable, 
                source: this
            )).FirstOrDefault();

            if (cardModel != null)
            {
                // 내구도 증가
                DurabilityExtensions.VarianceDurability(cardModel, DynamicVars["Durability"].IntValue);
            }
            else
            {
                // 소모품이 없다고 안내 문구 출력
                LocString locString = new LocString("combat_messages", "DURABLES_MISSING");
                TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
            }
        }
        // 강화 손 전체
        else
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
    }

    //protected override void OnUpgrade()
}