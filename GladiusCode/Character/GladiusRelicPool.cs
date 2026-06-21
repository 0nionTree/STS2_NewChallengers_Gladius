using BaseLib.Abstracts;
using Gladius.GladiusCode.Extensions;
using Godot;

namespace Gladius.GladiusCode.Character;

public class GladiusRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Gladius.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}