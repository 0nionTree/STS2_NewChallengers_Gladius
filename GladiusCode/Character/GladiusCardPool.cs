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
            ModelDb.Card<Cleanup>(),
            ModelDb.Card<Sculpting>(),
            ModelDb.Card<DustOff>(),
            ModelDb.Card<Guideline>(),
            ModelDb.Card<GoodSign>(),
            ModelDb.Card<DuelingStance>(),
            ModelDb.Card<IngotMaking>(),
            ModelDb.Card<ScoopingUp>(),
            ModelDb.Card<Prediction>(),
            ModelDb.Card<DragonsRoar>(),
            // 고급 공격 카드
            ModelDb.Card<Punishment>(),
            ModelDb.Card<FillUp>(),
            ModelDb.Card<CrossCutting>(),
            ModelDb.Card<RainforceShock>(),
            ModelDb.Card<AlchemicBurst>(),
            ModelDb.Card<LightningBlow>(),
            ModelDb.Card<ShoulderTackle>(),
            ModelDb.Card<PersistentStrike>(),
            ModelDb.Card<MartialArts>(),
            ModelDb.Card<Byproduct>(),
            ModelDb.Card<Wallop>(),
            ModelDb.Card<ForcePalm>(),
            // 고급 스킬 카드
            ModelDb.Card<Welding>(),
            ModelDb.Card<Ecdysis>(),
            ModelDb.Card<Possession>(),
            ModelDb.Card<Eruption>(),
            ModelDb.Card<DismantlingTool>(),
            ModelDb.Card<WeaponAbsorption>(),
            ModelDb.Card<Consecrate>(),
            ModelDb.Card<Replica>(),
            ModelDb.Card<Miner>(),
            ModelDb.Card<ExorcismRitual>(),
            ModelDb.Card<Condensation>(),
            ModelDb.Card<DragonsMirror>(),
            ModelDb.Card<Unity>(),
            ModelDb.Card<EarthenRampart>(),
            ModelDb.Card<Adversity>(),
            // 고급 파워 카드
            ModelDb.Card<Collection>(),
            ModelDb.Card<SlagExplosion>(),
            ModelDb.Card<Grinding>(),
            ModelDb.Card<DragonsProtection>(),
            ModelDb.Card<BreathOfFire>(),
            ModelDb.Card<ThunderCloud>(),
            ModelDb.Card<Knead>(),
            // 희귀 공격 카드
            ModelDb.Card<FallingPetals>(),
            ModelDb.Card<DragonsFang>(),
            ModelDb.Card<BattleDrum>(),
            ModelDb.Card<GoldenPath>(),
            ModelDb.Card<Ordain>(),
            // 희귀 스킬 카드
            ModelDb.Card<DragonsWrath>(),
            ModelDb.Card<ExtremeSpeed>(),
            ModelDb.Card<GoldenHammer>(),
            ModelDb.Card<ConvergingSwirl>(),
            ModelDb.Card<Keyboard>(),
            ModelDb.Card<StormEerthAndFire>(),
            ModelDb.Card<Familiar>(),
            ModelDb.Card<StrangeMass>(),
            ModelDb.Card<Asura>(),
            ModelDb.Card<DragonsDescent>(),
            // 희귀 파워 카드
            ModelDb.Card<MastersReach>(),
            ModelDb.Card<Concussion>()
        ];
    }
}