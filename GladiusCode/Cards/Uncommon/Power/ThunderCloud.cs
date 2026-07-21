using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Enchantments;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ThunderCloud() : GladiusCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    // 벼락 구름
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<ThunderCloudPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<ThunderstruckWood>(IsUpgraded),
        HoverTipFactory.FromKeyword(GladiusKeywords.Material),
        ..HoverTipFactory.FromEnchantment<Sown>(IsUpgraded?2:1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 파워 획득
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (IsUpgraded)
            await PowerCmd.Apply<ThunderCloudPowerUpgraded>(choiceContext, Owner.Creature, DynamicVars["ThunderCloudPower"].BaseValue, Owner.Creature, this);
        else
		    await PowerCmd.Apply<ThunderCloudPower>(choiceContext, Owner.Creature, DynamicVars["ThunderCloudPower"].BaseValue, Owner.Creature, this);
    }

    //protected override void OnUpgrade()
}