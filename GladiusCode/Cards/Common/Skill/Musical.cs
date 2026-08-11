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
public class Musical() : GladiusCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 연주회
    //protected override IEnumerable<DynamicVar> CanonicalVars =>
    //    [];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<TuningShard>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        ..HoverTipFactory.FromEnchantment<Shrapnel>(IsUpgraded?6:4)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 청음편 생성
        CardModel cardModel = CombatState!.CreateCard<TuningShard>(Owner);
        if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
        {
            CardCmd.Upgrade(cardModel);
        }
        // 생성한 카드 손으로 가져오기
        await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		await Cmd.Wait(0.2f);
    }

    //protected override void OnUpgrade()
}