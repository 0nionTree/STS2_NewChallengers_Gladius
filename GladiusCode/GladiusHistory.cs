using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Gladius;
using MegaCrit.Sts2.Core.Combat.History.Entries; // DurabilityExtensions가 있는 네임스페이스 (필요 시 수정)

namespace Gladius.GladiusCode.History
{
    public static class GladiusHistory
    {
        /// <summary>
        /// 이번 턴에 주체가 사용한 '내구도 카드'의 수를 반환합니다.
        /// </summary>
        public static int GetDurableCardsPlayedThisTurn(ICombatState combatState, Creature owner)
        {
            if (CombatManager.Instance?.History?.CardPlaysFinished == null) return 0;

            return CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => 
                e.HappenedThisTurn(combatState) && 
                e.CardPlay.Card.GetDurability().isDurable && 
                e.CardPlay.Card.Owner?.Creature == owner
            );
        }

        /// <summary>
        /// 이번 전투 전체에서 주체가 사용한 '내구도 카드'의 수를 반환합니다.
        /// </summary>
        public static int GetDurableCardsPlayedThisCombat(Creature owner)
        {
            if (CombatManager.Instance?.History?.CardPlaysFinished == null) return 0;

            // HappensThisTurn 검사를 제외하면 이번 전투 전체의 카운트가 됩니다.
            return CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => 
                e.CardPlay.Card.GetDurability().isDurable && 
                e.CardPlay.Card.Owner?.Creature == owner
            );
        }

        /// <summary>
        /// 참고용: 이번 턴에 사용한 '연성물(Artifact/Material)' 카드의 수도 필요하다면 이렇게 만들 수 있습니다.
        /// </summary>
        public static int GetArtifactCardsPlayedThisTurn(ICombatState combatState, Creature owner)
        {
            if (CombatManager.Instance?.History?.CardPlaysFinished == null) return 0;

            return CombatManager.Instance.History.CardPlaysFinished.Count((CardPlayFinishedEntry e) => 
                e.HappenedThisTurn(combatState) && 
                (e.CardPlay.Card.Keywords.Contains(GladiusKeywords.Artifact) || e.CardPlay.Card.Keywords.Contains(GladiusKeywords.Material)) && 
                e.CardPlay.Card.Owner?.Creature == owner
            );
        }
    }
}