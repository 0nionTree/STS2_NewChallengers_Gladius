using BaseLib.Abstracts;
using Gladius.GladiusCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Gladius.GladiusCode.Character;

public class GladiusCardPool : CustomCardPoolModel
{
    public override string Title => Gladius.CharacterId; //This is not a display name.
    
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 1f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness
    
    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load Gladius/images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffffff");
    
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            // 기본 공격 카드
            ModelDb.Card<StrikeGladius>(),
            // 기본 스킬 카드
            ModelDb.Card<DefendGladius>(),
            ModelDb.Card<SwordGirding>(),
            ModelDb.Card<GatherIngredients>(),
            // 일반 공격 카드
            ModelDb.Card<Breakdown>(),
            ModelDb.Card<Eradication>(),
            ModelDb.Card<Overhand>(),
            ModelDb.Card<WindGuidance>(),
            ModelDb.Card<AlchemicStrike>(),
            ModelDb.Card<StraightPunch>(),
            ModelDb.Card<StraightPunch>(),
            ModelDb.Card<WheelKick>(),
            ModelDb.Card<Uppercut>(),
            ModelDb.Card<Pulverize>(),
            // 일반 스킬 카드
            ModelDb.Card<Cleanup>()
            // 고급 공격 카드
            // 고급 스킬 카드
            // 고급 파워 카드
            // 희귀 공격 카드
            // 희귀 스킬 카드
            // 희귀 파워 카드
        ];
    }
}