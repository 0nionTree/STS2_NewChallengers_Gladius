using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Saving() : GladiusCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // 절약
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<SavingPower>(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy),
        HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<SavingPower>(choiceContext, Owner.Creature, DynamicVars["SavingPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SavingPower"].UpgradeValueBy(1);
    }
}