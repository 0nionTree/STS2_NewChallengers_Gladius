using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Uppercut() : GladiusCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 올려치기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, DamageProps.card),
        new CardsVar(1)];

    //protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    //    [];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_thrash")
            .Execute(choiceContext);
        // 버린 카드 더미의 카드 선택
        IEnumerable<CardModel>? cards = await CardSelectCmd.FromCombatPile(prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue), context: choiceContext, pile: PileType.Discard.GetPile(Owner), player: Owner);
		if (cards != null && cards.Any())
		{
            foreach (CardModel card in cards)
                // 선택한 카드를 차례로 뽑을 카드 더미 아래로 이동
			    await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Bottom);
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}