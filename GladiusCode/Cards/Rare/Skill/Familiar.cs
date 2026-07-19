using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Familiar() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 권속
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(1),
        new IntVar("Durability", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 손에 있는 소모품이 아닌 카드를 선택 후, 선택된 카드마다 반복
        foreach (CardModel item in await CardSelectCmd.FromHand
            (
                prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue),
                context: choiceContext,
                player: Owner,
                filter: (CardModel card) => !card.GetDurability().isDurable, 
                source: this
            ))
		{
            // 선택된 카드에 소모성 및 내구도 부여
            item.AddKeyword(GladiusKeywords.Artifact);
            item.AddKeyword(GladiusKeywords.Durability);
            var durabilityData = item.GetDurability();
            durabilityData.isDurable = true;
            durabilityData.BaseDurability = DynamicVars["Durability"].IntValue;
            durabilityData.CurrentDurability = durabilityData.BaseDurability;
            durabilityData.WasDurability = durabilityData.BaseDurability;
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Durability"].UpgradeValueBy(1);
    }
}