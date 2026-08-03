using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Gladius.GladiusCode;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gladius
{
    // 내구도를 보호하는 기능이 있는 모든 파워가 가져야 할 공통 인터페이스
    public interface Screening
    {
        // 현재 이 파워가 내구도를 보호할 수 있는 상태인가? (스택이 남아있는가 등)
        //bool IsProtectionActive();
    }

    public static class ScreeningManager
    {
        public static async Task<(IEnumerable<CardModel>? remains, IEnumerable<CardModel>? falls)> Screening(ICombatState? combatState, PlayerChoiceContext choiceContext, Player owner, int num)
        {
            // 전투 중이 아니라면 종료
            if (combatState == null)
                return (Enumerable.Empty<CardModel>(), Enumerable.Empty<CardModel>());

            IEnumerable<CardModel>? remains = null;
            IEnumerable<CardModel>? falls = null;

            var drawPile = PileType.Draw.GetPile(owner);
            int drawPileCount = drawPile.Cards.Count;
            
            // 뽑을 카드 더미의 카드가 없다면 셔플
            if (drawPileCount <= 0)
            {
		        await CardPileCmd.Shuffle(choiceContext, owner);
            }
            
            // 뽑을 카드 더미의 아래의 n*2장을 IEnumerable화하여 cardOptions로 저장
            IEnumerable<CardModel> cardOptions = PileType.Draw.GetPile(owner).Cards.ToList()
                .TakeLast(num * 2);
            int cardOptionsCount = cardOptions.Count();
            // 뽑을 카드 더미에 카드가 1장 이하라면 카드 선택 과정 스킵
            if (cardOptionsCount > 1)
            {
                // 뽑을 카드 더미의 남은 카드 수가 (선별 수치 * 2) 보다 적다면
                if (cardOptionsCount < num * 2)
                {
                    // 뽑을 카드 더미의 남은 카드 수가 짝수라면
                    if (cardOptionsCount % 2 == 0)
                    {
                        // 선별 수치를 남은 카드의 절반으로 변경
                        num = cardOptionsCount / 2;
                    }
                    // 홀수라면
                    else
                    {
                        // 선별 수치를 남은 카드의 절반+1로 변경
                        num = (cardOptionsCount / 2) + 1;
                    }
                }
                // 선별 리스트에서 선별 수치만큼 카드 선택하여 잔류 리스트에 저장
                remains = (await CardSelectCmd.FromCombatPile(choiceContext,
                    PileType.Draw.GetPile(owner),
                    owner,
                    new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, num),
                    (CardModel c) => cardOptions.Contains(c))).ToList();
                // 선택되지 않은 카드는 퇴출 리스트에 저장
                falls = cardOptions.Except(remains);
            }
            // 카드 선택이 종료 또는 캔슬되면 즉시 훅 송신
            await GladiusEventDispatcher.DispatchScreeningPerformed(combatState, remains, falls, owner, choiceContext);

            // 잔류 카드가 있다면
            if (remains != null && remains.Any())
            {
                // 하나씩 이동 및 훅 송신
                foreach (CardModel item in remains)
                {
                    // 뽑을 카드 더미 위로 이동
                    await CardPileCmd.Add(item, PileType.Draw, CardPilePosition.Top, null, true);
                    // 잔류 카드로서 훅 송신
                    await GladiusEventDispatcher.DispatchScreenedCardsMoved(combatState, item, true, owner, choiceContext);
                }
            }
            // 퇴출 카드가 있다면
            if (falls != null && falls.Any())
            {
                // 하나씩 이동 및 훅 송신
                foreach (CardModel item in falls)
                {
                    // 버린 카드 더미로 이동
                    await CardPileCmd.Add(item, PileType.Discard);
                    // 퇴출 카드로서 훅 송신
                    await GladiusEventDispatcher.DispatchScreenedCardsMoved(combatState, item, false, owner, choiceContext);
                }
            }
        return (remains ?? null, falls ?? null);
        }
    }
}