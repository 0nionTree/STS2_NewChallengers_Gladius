using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class WroughtIron() : GladiusCard(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Material];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
    }

    protected override void OnUpgrade()
    {
    }
}