using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Saves;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class StraightPunch() : GladiusCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 정권 지르기
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, DamageProps.card), new IntVar("DragonAuraPower", 3m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];
    
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].IntValue, Owner.Creature, this);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["DragonAuraPower"].UpgradeValueBy(1m);
    }
}