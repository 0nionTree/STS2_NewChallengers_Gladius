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

public class ReactorPower : GladiusPower
{
    // 초과 투입 - 파워
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy)];
        
    // 연성 시 드로우
	public override async Task OnAlchemyTriggered(CardModel artifact, CardModel material, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
        if (Owner.Player == null || Owner.Player != creator) return;
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}