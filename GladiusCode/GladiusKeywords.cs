using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Gladius.GladiusCode;

public static class GladiusKeywords
{
	[CustomEnum("Materialized")]
	[KeywordProperties(AutoKeywordPosition.Before, true)]
	public static CardKeyword Materialized;
}