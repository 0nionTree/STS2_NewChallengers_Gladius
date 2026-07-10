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

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Overhand() : GladiusCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 내려치기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, DamageProps.card), new CardsVar(1)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 뽑을 카드 더미의 n장 선택
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(
            choiceContext, 
            PileType.Draw.GetPile(base.Owner), 
            base.Owner, 
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1)
        )).ToList();
        // 선택된 카드가 있다면, 반복문을 통해 하나씩 버리기
        if (selection != null && selection.Count > 0)
        {
            foreach (CardModel item in selection)
            {
                await CardCmd.Discard(choiceContext, item);
            }
        }
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}