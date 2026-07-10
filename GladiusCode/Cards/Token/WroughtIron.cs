using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using HarmonyLib;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class WroughtIron() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.Self)
{
    // 연철 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    protected override async Task Material(PlayerChoiceContext choiceContext, CardModel artifectCard)
    {
        // 강화된 상태일 경우
        if (IsUpgraded)
        {
            // 미강화 연철 생성
            CardModel cardModel = CombatState!.CreateCard<WroughtIron>(Owner);
            // 생성한 카드 손으로 가져오기
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
            await Cmd.Wait(0.2f);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

    }

    protected override void OnUpgrade()
    {
        
    }
}