using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Gladius.GladiusCode;

public static class GladiusKeywords
{
	[CustomEnum("Artifact")]
	[KeywordProperties(AutoKeywordPosition.Before, true)]
	public static CardKeyword Artifact;

	[CustomEnum("Material")]
	[KeywordProperties(AutoKeywordPosition.Before, true)]
	public static CardKeyword Material;
	
	[CustomEnum("DragonAura")]
	[KeywordProperties(AutoKeywordPosition.Before, true)]
	public static CardKeyword DragonAura;
	
	[CustomEnum("Alchemy")]
	[KeywordProperties(AutoKeywordPosition.Before, true)]
	public static CardKeyword Alchemy;
}