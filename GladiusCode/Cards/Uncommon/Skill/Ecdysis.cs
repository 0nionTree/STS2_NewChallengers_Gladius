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
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Ecdysis() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 탈피
    public override bool IsRequiredMaterial => true;
    
    // protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<DragonScale>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손에 있는 카드 목록 받아오기
        var handCards = PileType.Hand.GetPile(Owner).Cards;
        // 손에 있는 소재 카드 목록 생성
        var handMaterialCards = handCards.Where(c => c.Keywords.Contains(GladiusKeywords.Material)).ToList();
        // 소재 카드가 하나라도 있다면
        if (handMaterialCards.Count() > 0)
        {
            // 손에 있는 모든 소재 카드를 용 비늘로 연성
            foreach (CardModel cardModel in handMaterialCards)
            {
                await Alchemy<DragonScale>(choiceContext, IsUpgraded, 0, cardModel);
            }
        }
        // 소재 카드가 하나도 없다면
        else
        {
            // 소재가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "MATERIALS_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    /*protected override void OnUpgrade()
    {
        
    }*/
}