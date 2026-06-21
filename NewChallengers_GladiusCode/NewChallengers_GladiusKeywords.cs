using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace NewChallengers_Gladius.NewChallengers_GladiusCode;

public static class NewChallengers_GladiusKeywords
{
	[CustomEnum("Materialized")]
	[KeywordProperties(AutoKeywordPosition.After, true)]
	public static CardKeyword Materialized;
}