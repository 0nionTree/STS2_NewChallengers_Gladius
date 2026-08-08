using Gladius.GladiusCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

public class EruptionPower : GladiusPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 용기 소모 시
    public override async Task OnConsumDragonAura(CardModel cardModel, int amount, Player player, PlayerChoiceContext choiceContext)
    {
        // 감소한 용기 스택만큼 이 디버프 스택 감소
        if (player == Owner.Player)
            await PowerCmd.ModifyAmount(choiceContext, this, -amount, null, null);
    }

    // 턴 종료 시 남은 스택만큼 용기 손실
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
		{
            var dragonAuraPower = Owner.GetPower<DragonAuraPower>();

            // 용기를 보유하고 있다면
            if (dragonAuraPower != null)
            {
                Flash();
                // 용기 스택이 이 디버프 스택보다 높거나 같다면, 이 디버프 스택만큼 용기 스택 감소
                if (dragonAuraPower.Amount >= Amount)
                    await PowerCmd.ModifyAmount(choiceContext, dragonAuraPower, -Amount, null, null);
                // 용기 스택이 적다면 용기 제거
                else
                    await PowerCmd.Remove(dragonAuraPower);
            }
            // 디버프 제거
            await PowerCmd.Remove(this);
		}
    }

    public void Flashing()
    {
        Flash();
    }
}