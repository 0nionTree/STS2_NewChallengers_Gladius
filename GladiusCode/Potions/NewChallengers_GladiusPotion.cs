using BaseLib.Abstracts;
using BaseLib.Utils;
using NewChallengers_Gladius.GladiusCode.Character;

namespace NewChallengers_Gladius.GladiusCode.Potions;

[Pool(typeof(NewChallengers_GladiusPotionPool))]
public abstract class NewChallengers_GladiusPotion : CustomPotionModel;