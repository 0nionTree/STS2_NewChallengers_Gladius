using BaseLib.Abstracts;
using NewChallengers_Gladius.GladiusCode.Extensions;
using Godot;

namespace NewChallengers_Gladius.GladiusCode.Character;

public class NewChallengers_GladiusPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Gladius.Color;
    

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}