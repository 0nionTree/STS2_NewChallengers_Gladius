using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Gladius.GladiusCode.Cards
{
    // 내구도(Durability) 시스템을 가지는 카드의 명찰입니다.
    public interface IDurableCard
    {
        // 겟터(get)와 셋터(set)를 통해 내구도 수치를 자유롭게 변경할 수 있게 합니다.
        int Durability { get; set; }

        public async Task ReduceDur(PlayerChoiceContext choiceContext, int value)
        {
            Durability -= value;
            if (Durability == 0)
            {
                await CardCmd.Exhaust(choiceContext, (MegaCrit.Sts2.Core.Models.CardModel)this);
            }
        }
    }
}