using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Gladius.GladiusCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using Godot;
using BaseLib.Patches.UI;

namespace Gladius.GladiusCode.Character;

public class Gladius : PlaceholderCharacterModel
{
    public const string CharacterId = "Gladius";
    
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 70;
    
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeGladius>(),
        ModelDb.Card<StrikeGladius>(),
        ModelDb.Card<StrikeGladius>(),
        ModelDb.Card<StrikeGladius>(),
        ModelDb.Card<DefendGladius>(),
        ModelDb.Card<DefendGladius>(),
        ModelDb.Card<DefendGladius>(),
        ModelDb.Card<DefendGladius>(),
        ModelDb.Card<SwordGirding>(),
        ModelDb.Card<Mine>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<MineralPouch>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<GladiusCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<GladiusRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<GladiusPotionPool>();
    
    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets. 
        These are just some of the simplest assets, given some placeholders to differentiate your character with. 
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    // 캐릭터 아이콘
    public override string CustomIconTexturePath => "character_icon_gladius.png".CharacterUiPath();
    public override string CustomIconOutlineTexturePath => "character_icon_gladius_outline.png".CharacterUiPath();
    // 캐릭터 선택 초상화
    public override string CustomCharacterSelectIconPath => "char_select_gladius.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_gladius_locked.png".CharacterUiPath();
    // 캐릭터 맵 마커
    public override string CustomMapMarkerPath => "map_marker_gladius.png".CharacterUiPath();

    // 냠냠 쿠키
    /*
    RelicIconData YummyCookie = new RelicIconData(
        ".png".RelicImagePath(),
        ".png".RelicImagePath(),
        ".png".RelicImagePath()
    );
    public override RelicIconData CustomYummyCookie => YummyCookie;
    */

    // 멀티플레이 손 - 가리키기, 바위, 가위, 보
    public override string CustomArmPointingTexturePath => "multiplayer_hand_gladius_point.png".CharacterPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_gladius_rock.png".CharacterPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_gladius_scissor.png".CharacterPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_gladius_paper.png".CharacterPath();

    // 에너지 카운터 씬
    public override string CustomEnergyCounterPath => "gladius_energy_counter.tscn".ScenesPath();
    // 선택창 배경 씬
    public override string CustomCharacterSelectBg => "char_select_bg_gladius.tscn".ScenesPath();

    // 캐릭터 비주얼 씬
    public override string CustomVisualPath => "gladius_visual.tscn".ScenesPath();
    public override string CustomMerchantAnimPath => "gladius_merchant.tscn".ScenesPath();
    public override string CustomRestSiteAnimPath => "gladius_rest_site.tscn".ScenesPath();
    //public override string CustomCorpseAnimPath => "gladius_corpse.tscn".ScenesPath();
}