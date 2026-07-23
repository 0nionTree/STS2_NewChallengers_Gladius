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
public class OverInput() : GladiusCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 초과 투입
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<OverInputPower>(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromCard<DragonScale>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<OverInputPower>(choiceContext, Owner.Creature, DynamicVars["OverInputPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}