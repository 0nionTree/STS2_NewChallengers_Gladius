using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius;

[Pool(typeof(ArtifactCardPool))]
public class TestArtifact() : GladiusCard(0, CardType.Attack, CardRarity.Token, TargetType.AllEnemies)
{
    // 연성물 테스트용 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 5;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact,
        GladiusKeywords.Durability];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		if (this.GetDurability().CurrentDurability == 2)
        this.BaseReplayCount += 1;
    }
}