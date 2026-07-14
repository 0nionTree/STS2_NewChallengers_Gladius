using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class WheelKick() : GladiusCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    // 돌려차기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, DamageProps.card)];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[CardKeyword.Ethereal];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 여러 적 공격
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState!)
			.WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
			.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}