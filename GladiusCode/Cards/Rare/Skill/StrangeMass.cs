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
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class StrangeMass() : GladiusCard(-1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    // 기괴한 덩어리
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(1),
        new IntVar("CorruptedAmount", 1),
        new IntVar("Durability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy),
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        ..HoverTipFactory.FromEnchantment<Corrupted>(1)];
        
	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Material,
        CardKeyword.Unplayable];

    public override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        if (artifactCard != null)
        {
            CardCmd.Enchant<Corrupted>(artifactCard, DynamicVars["CorruptedAmount"].IntValue);
            if (IsUpgraded)
                DurabilityExtensions.VarianceDurability(artifactCard, DynamicVars["Durability"].IntValue);
        }
        CardModel card = CreateClone();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        await CardPileCmd.Add(this, PileType.Hand, CardPilePosition.Bottom);
    }

    //protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)

    //protected override void OnUpgrade()
}