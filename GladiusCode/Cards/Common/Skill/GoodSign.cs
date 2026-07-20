using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class GoodSign() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 길조
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<ThunderstruckWood>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1), context: choiceContext, player: Owner, filter: null, source: this)).FirstOrDefault();
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
        
    }
}