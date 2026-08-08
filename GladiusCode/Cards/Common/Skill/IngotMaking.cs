using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class IngotMaking() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 조괴
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromCard<Steel>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability),
        HoverTipFactory.FromKeyword(GladiusKeywords.Remain),
        HoverTipFactory.FromKeyword(GladiusKeywords.Fall)];

    //public override IEnumerable<CardKeyword> CanonicalKeywords =>
    //    [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 강철 생성
        CardModel cardModel = CombatState!.CreateCard<Steel>(Owner);
        if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
        {
            CardCmd.Upgrade(cardModel);
        }
        // 생성한 카드 손으로 가져오기
        await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		await Cmd.Wait(0.2f);
    }

    // 선별 실행 시 카드 파일 이동 이후 자동으로 실행되는 함수
    public override async Task OnScreenedCardsMoved(CardModel cardModel, bool isRemain, Player owner, PlayerChoiceContext choiceContext)
    {
        if (cardModel == this && owner == Owner)
        {
            await Cmd.Wait(0.2f);
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }

    protected override void OnUpgrade()
    {
        
    }
}