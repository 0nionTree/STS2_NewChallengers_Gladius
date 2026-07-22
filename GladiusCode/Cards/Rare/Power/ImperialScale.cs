using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ImperialScale() : GladiusCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 역린
    public override bool IsRequiredMaterial => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<ImperialScalePower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<DragonScale>(),
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy),
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<ImperialScalePower>(choiceContext, Owner.Creature, DynamicVars["ImperialScalePower"].BaseValue, Owner.Creature, this);
        await Alchemy<DragonScale>(choiceContext, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ImperialScalePower"].UpgradeValueBy(1);
    }
}