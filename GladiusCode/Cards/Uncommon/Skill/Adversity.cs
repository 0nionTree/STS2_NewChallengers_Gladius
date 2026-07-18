using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Adversity() : GladiusCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 역경
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(10m, BlockProps.card)];

    //protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    //    [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IReadOnlyList<Creature> hittableEnemies = CombatState!.HittableEnemies;
		foreach (Creature item in hittableEnemies)
		{
            _ = item;
            // 방어도 획득
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
		}
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}