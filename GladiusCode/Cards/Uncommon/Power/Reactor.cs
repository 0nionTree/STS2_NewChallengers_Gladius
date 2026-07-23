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
public class Reactor() : GladiusCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // 반응로
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<ReactorPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<ReactorPower>(choiceContext, Owner.Creature, DynamicVars["ReactorPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ReactorPower"].UpgradeValueBy(1);
    }
}