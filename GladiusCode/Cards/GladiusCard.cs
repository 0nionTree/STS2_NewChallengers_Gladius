using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gladius.GladiusCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
[Pool(typeof(GladiusCardPool))]
public abstract class GladiusCard(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType target,
    bool showInCardLibrary = true,
    bool autoAdd = true
) : CustomCardModel(cost, type, rarity, target, showInCardLibrary, autoAdd)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    
    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190
    
    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    protected virtual async Task Material(PlayerChoiceContext choiceContext, CardPlay cardPlay){}

    protected async Task Alchemy(
        PlayerChoiceContext choiceContext, 
        int amountToExhaust, 
        string targetCardId, 
        Action<CardModel> onMaterialExhausted)
    {
        CardPile pile = PileType.Hand.GetPile(base.Owner);
        
        /*
        var selection = await choiceContext.SelectCardsFromHand(
            amountToExhaust, 
            c => c.Tags.Contains(CardTag.Material), // 가정된 태그
            "재료로 사용할 카드를 선택하세요."
        );
        */
    }
}