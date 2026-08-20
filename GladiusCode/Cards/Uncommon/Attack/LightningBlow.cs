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
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models.Enchantments;

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
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        ..HoverTipFactory.FromEnchantment<Sown>(IsUpgraded?3:2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_lightning")
            .Execute(choiceContext);

        // 손에 있는 카드 벽조목으로 변화
        CardModel? cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue), context: choiceContext, player: Owner, filter: null, source: this)).FirstOrDefault();
		if (cardModel != null)
		{
			CardModel cardModel2 = CombatState!.CreateCard<ThunderstruckWood>(Owner);
			if (IsUpgraded)
			{
				CardCmd.Upgrade(cardModel2);
			}
			await CardCmd.Transform(cardModel, cardModel2);
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}