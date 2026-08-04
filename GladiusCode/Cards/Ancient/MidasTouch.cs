using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using BaseLib.Extensions;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class MidasTouch() : GladiusCard(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    // 미다스의 손
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<MidasTouchPower>(2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability),
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<MidasTouchPower>(choiceContext, Owner.Creature, DynamicVars["MidasTouchPower"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        //DynamicVars.Power<MidasTouchPower>().UpgradeValueBy(1);
    }
}