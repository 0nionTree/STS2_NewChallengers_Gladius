using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DustOff() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 털어내기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, BlockProps.card),
        new IntVar("Screening", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        // 선별
        await Screening(choiceContext, DynamicVars["Screening"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Screening"].UpgradeValueBy(1);
    }
}