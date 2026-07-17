using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Eruption() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 용기 분출
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new IntVar("DragonAuraPower", 4m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용기 분출 획득
		await PowerCmd.Apply<EruptionPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        // 용기 획득
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        Keywords.Append(CardKeyword.Retain);
    }
}