using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class GoldenPath() : GladiusCard(8, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    // 황금의 궤적
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(30, DamageProps.card),
        new EnergyVar(2)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 여러 적 공격
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(CombatState!)
			.WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
			.Execute(choiceContext);
    }

    public override Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator)
    {
        // 이번 전투동안 비용 2 감소
        if (creator != Owner)
		{
			return Task.CompletedTask;
		}
		if (artifact.Owner != Owner)
		{
			return Task.CompletedTask;
		}
        EnergyCost.AddThisCombat(-DynamicVars.Energy.IntValue);
        
        return Task.CompletedTask; 
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-2);
    }
}