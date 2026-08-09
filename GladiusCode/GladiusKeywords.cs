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
	
	[CustomEnum("Alchemy")]
	[KeywordProperties(AutoKeywordPosition.Before, true)]
	public static CardKeyword Alchemy;

	[CustomEnum("Durability")]
	[KeywordProperties(AutoKeywordPosition.None, true)]
	public static CardKeyword Durability;
	
	[CustomEnum("Screening")]
	[KeywordProperties(AutoKeywordPosition.None, true)]
	public static CardKeyword Screening;
	
	[CustomEnum("Remain")]
	[KeywordProperties(AutoKeywordPosition.None, true)]
	public static CardKeyword Remain;
	
	[CustomEnum("Fall")]
	[KeywordProperties(AutoKeywordPosition.None, true)]
	public static CardKeyword Fall;
}