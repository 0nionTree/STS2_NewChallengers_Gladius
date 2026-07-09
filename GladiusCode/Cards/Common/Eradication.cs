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

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Eradication() : GladiusCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 퇴치
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, DamageProps.card), new PowerVar<WeakPower>(1m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WeakPower>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 무력화와 동일한 이펙트 딜레이
		NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NThinSliceVfx.Create(cardPlay.Target));
		float num = base.Owner.Character.AttackAnimDelay;
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Normal)
		{
			num += 0.2f;
		}
        // 피해량 계산
		await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
			.WithAttackerAnim("Attack", num)
			.Execute(choiceContext);
        // 약화 부여
		await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Weak.UpgradeValueBy(1m);
    }
}