using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class ShoulderGuards() : GladiusCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // 견갑 - 연성물
    public override bool IsDurable => true;
    public override int BaseDurability => 1;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(2m, DamageProps.card),
        new BlockVar(2m, BlockProps.card),
		new IntVar("Durability", 1)];

	public override IEnumerable<CardKeyword> CanonicalKeywords =>
		[GladiusKeywords.Artifact, GladiusKeywords.Durability];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];
        
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 피해량 계산 및 이펙트 출력
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Pile?.Type == PileType.Hand)
        {
            // 현재 내구도 증가
            this.GetCustomData().CurrentDurability += DynamicVars["Durability"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}