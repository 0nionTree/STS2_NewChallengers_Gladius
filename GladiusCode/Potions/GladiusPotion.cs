using BaseLib.Abstracts;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;

namespace Gladius.GladiusCode.Potions;

[Pool(typeof(GladiusPotionPool))]
public abstract class GladiusPotion : CustomPotionModel;