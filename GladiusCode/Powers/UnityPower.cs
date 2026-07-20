using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Combat.History.Entries;

namespace Gladius;

public class UnityPower : GladiusPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.Static(StaticHoverTip.Block)];
        
    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        // 합일 - 파워
        // 플레이어가 얻는 방어도가 아니면 무시
        if (target.IsMonster)
            return 1m;
        // 카드로 얻는 방어도가 아니면 무시 (포션, 유물 효과 방지)
        if (!props.IsCardOrMonsterMove())
            return 1m;
        // 멀티플레이어 환경에서 다른 사람의 카드로 얻는 방어도가 아니면 무시
        if (cardSource != null && cardSource.Owner.Creature != Owner)
            return 1m;
        
        // Amount 1당 1% 증가 (0.01)
        // 예: Amount가 25라면 1m + 0.25m = 1.25m 반환 (25% 증가)
        return 1m + (Amount / 100m);
    }

    // 카드를 낸 직후에 실행되는 훅 (예시)
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 낸 카드가 방어도를 제공하는 카드인지 확인 (방어도가 있는 카드일 때만 소모되도록)
        if (cardPlay.Card.GainsBlock)
        {
            // 자기 자신(이 파워)을 대상에서 제거
            await PowerCmd.Remove(this);
        }
    }
}