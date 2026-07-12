using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class LightningBlow() : GladiusCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 뇌격
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, DamageProps.card),
        new CardsVar(1)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<ThunderstruckWood>(),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 버린 카드 더미의 카드 선택
        List<CardModel> selection = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Discard.GetPile(Owner), Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue))).ToList();
		foreach (CardModel item in selection)
		{
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
                // 카드 손으로 가져오기
                await CardPileCmd.Add(cardModel, PileType.Hand);
            }
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}