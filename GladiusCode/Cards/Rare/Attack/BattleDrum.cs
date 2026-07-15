using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class BattleDrum() : GladiusCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // 개전의 북
    public override bool IsRequiredMaterial => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, DamageProps.card)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<RitualPlumb>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        // 손에 Material 키워드를 가진 카드가 하나라도 있는지 확인
        var hand = PileType.Hand.GetPile(Owner);
        bool hasMaterial = hand?.Cards?.Any(c => c.Keywords.Contains(GladiusKeywords.Material)) ?? false;
        if (hasMaterial)
        {
            // Material 키워드를 가진 카드가 있다면 연성
            await Alchemy<RitualPlumb>(choiceContext, IsUpgraded);
        }
        else
        {
            // Material 키워드를 가진 카드가 없다면 문구 출력
            LocString locString = new LocString("combat_messages", "MATERIALS_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}