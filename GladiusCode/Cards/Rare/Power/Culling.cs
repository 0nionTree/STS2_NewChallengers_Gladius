using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Culling() : GladiusCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 선별
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<CullingPower>(1)];

    //protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    //    [];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<CullingPower>(choiceContext, Owner.Creature, DynamicVars["CullingPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}