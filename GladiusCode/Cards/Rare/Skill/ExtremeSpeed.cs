using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ExtremeSpeed() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 신속
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, BlockProps.card),
        new IntVar("Screening", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Screening)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Innate];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        // 선별
        await ScreeningManager.Screening(CombatState, choiceContext, Owner, DynamicVars["Screening"].IntValue);
        // 뽑을 카드 더미 아래로 이동
        await CardPileCmd.Add(this, PileType.Draw, CardPilePosition.Bottom);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}