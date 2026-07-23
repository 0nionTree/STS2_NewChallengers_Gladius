using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Bestow() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    // 전수
	public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("DragonAuraPower", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.DragonAura)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[CardKeyword.Retain];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 대상 아군 용기 획득
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, cardPlay.Target, DynamicVars["DragonAuraPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DragonAuraPower"].UpgradeValueBy(2m);
    }
}