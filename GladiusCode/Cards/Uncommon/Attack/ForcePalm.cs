using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class ForcePalm() : GladiusCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 발경
	protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new CalculationBaseVar(0m),
        new ExtraDamageVar(8m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>
            {
                int value = card.Owner.PlayerCombatState!.Energy;
                bool hasChemicalX = card.Owner.Relics.OfType<ChemicalX>().Any();
                int finalValue = value + (hasChemicalX?2:0);
                return finalValue;
            }
        ),
        new DamageVar(0m, DamageProps.card)
        ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 소모된 X에 비례하여 대미지 재계산
        DynamicVars.Damage.BaseValue = DynamicVars.ExtraDamage.IntValue * ResolveEnergyXValue();
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
	}

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(2m);
    }
}