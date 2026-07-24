using BaseLib.Abstracts;
using Gladius.GladiusCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Gladius.GladiusCode.Character;

public class GladiusRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Gladius.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    protected override RelicModel[] GenerateAllRelics()
    {
        return
        [
            ModelDb.Relic<MineralPouch>(),
            ModelDb.Relic<IngotCase>(),
            ModelDb.Relic<DragonScroll>(),
            ModelDb.Relic<PaperDragon>(),
            ModelDb.Relic<Hardener>(),
            ModelDb.Relic<CinnamonGum>(),
            ModelDb.Relic<Magatama>(),
            ModelDb.Relic<PenForSigning>(),
            ModelDb.Relic<CarvingKnife>()
        ];
    }
}