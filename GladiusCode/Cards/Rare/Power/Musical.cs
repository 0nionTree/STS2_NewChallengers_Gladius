using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Musical() : GladiusCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 유리 공예
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<MusicalPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<TuningShard>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (!IsUpgraded)
		    await PowerCmd.Apply<MusicalPower>(choiceContext, Owner.Creature, DynamicVars["MusicalPower"].BaseValue, Owner.Creature, this);
        else
		    await PowerCmd.Apply<MusicalPowerUpgraded>(choiceContext, Owner.Creature, DynamicVars["MusicalPower"].BaseValue, Owner.Creature, this);
    }

    //protected override void OnUpgrade()
}