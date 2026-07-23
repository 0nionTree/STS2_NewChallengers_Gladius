using Gladius.GladiusCode;
using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class OverInputPower : GladiusPower
{
    // 초과 투입 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];
        
    // 연성 시 모든 아군에게 용 비늘 생성
	public override async Task OnAlchemyTriggered(CardModel artifact, CardModel material, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
		// 연성 실행자가 파워 보유자가 아니라면 종료
		if (creator != Owner.Player) return;
        // 소재가 없다면 종료
        if (material == null) return;
        // 첫 번째 연성이 아니라면 종료
        if (!isFirstThisTurn) return;

        // 자신을 제외한 아군 수만큼 반복
        IEnumerable<Creature> enumerable = from c in CombatState.GetTeammatesOf(Owner)
			where c != null && c.IsAlive && c.IsPlayer && c.Player!.Creature != Owner
			select c;
		foreach (Creature item in enumerable)
		{
            // 소재를 생성하여 해당 플레이어 손에 추가
            CardModel? copy = item.Player!.RunState.CreateCard(material.CanonicalInstance, item.Player);

            // 강화 상태 복제
            if (material.IsUpgraded)
            {
                CardCmd.Upgrade(copy);
            }
			await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner.Player);

            GladiusCard gladiusCard = (GladiusCard)copy;
            CardModel? copyArtifact = await gladiusCard.Alchemy<DragonScale>(choiceContext, false, 0, copy, Owner.Player);
		}
    }
}