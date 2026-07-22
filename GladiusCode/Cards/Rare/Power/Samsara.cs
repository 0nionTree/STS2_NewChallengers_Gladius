using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Potions;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Samsara() : GladiusCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 사색
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<SamsaraPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (!IsUpgraded)
		    await PowerCmd.Apply<SamsaraPower>(choiceContext, Owner.Creature, DynamicVars["SamsaraPower"].BaseValue, Owner.Creature, this);
        else
		    await PowerCmd.Apply<SamsaraPowerUpgraded>(choiceContext, Owner.Creature, DynamicVars["SamsaraPower"].BaseValue, Owner.Creature, this);
    }

    //protected override void OnUpgrade()
}