using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DismantlingTool() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 해체 도구 - 연성
    public override bool IsRequiredMaterial => true;

    private decimal _extraDurabilityFromDiscard;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Durability", 0),
        new DynamicVar("IncreaseDurability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<SerratedDagger>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    private decimal ExtraDurabilityFromDiscard
	{
		get
		{
			return _extraDurabilityFromDiscard;
		}
		set
		{
			AssertMutable();
			_extraDurabilityFromDiscard = value;
		}
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Alchemy<SerratedDagger>(choiceContext, IsUpgraded, DynamicVars["Durability"].IntValue);
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card != this) return;
        if (card.Pile?.Type != PileType.Discard) return;
		DynamicVars["Durability"].BaseValue += DynamicVars["IncreaseDurability"].BaseValue;
        ExtraDurabilityFromDiscard += DynamicVars["IncreaseDurability"].BaseValue;
    }

    //protected override void OnUpgrade()

    protected override void AfterDowngraded()
	{
		AfterDowngraded();
		DynamicVars["Durability"].BaseValue += ExtraDurabilityFromDiscard;
	}
}