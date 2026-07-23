using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Gladius;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Players; // DurabilityExtensions가 있는 네임스페이스 (필요 시 수정)

namespace Gladius.GladiusCode.History
{
    public static class DurabilityHistory
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

    public static class AlchemyHistory
    {
        // 마지막으로 연성했을 때의 턴 번호를 기억
        private static int _lastAlchemyTurnNumber = -1;
        // 이번 턴에 진행한 연성 횟수
        private static int _alchemiesThisTurn = 0;

        /// <summary>
        /// 연성을 실행할 때마다 호출하여 기록합니다.
        /// </summary>
        public static void RecordAlchemy(Player player)
        {
            if (player?.PlayerCombatState == null) return;

            int currentTurn = player.PlayerCombatState.TurnNumber;

            // 마지막으로 연성했던 턴과 현재 턴이 다르면 (즉, 턴이 바뀌었다면) 카운트 초기화
            if (currentTurn != _lastAlchemyTurnNumber)
            {
                _lastAlchemyTurnNumber = currentTurn;
                _alchemiesThisTurn = 0;
            }

            // 이번 턴 연성 횟수 1 증가
            _alchemiesThisTurn++;
        }

        /// <summary>
        /// 이번 턴에 연성을 몇 번 했는지 반환합니다.
        /// </summary>
        public static int GetAlchemiesThisTurn(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;

            int currentTurn = player.PlayerCombatState.TurnNumber;

            // 이번 턴에 한 번도 연성을 안 했다면 0 반환
            if (currentTurn != _lastAlchemyTurnNumber)
            {
                return 0; 
            }

            return _alchemiesThisTurn;
        }

        /// <summary>
        /// 전투가 완전히 끝났을 때 안전하게 기록을 초기화합니다.
        /// (CombatManager의 전투 종료 훅이나 패치에서 호출해주세요)
        /// </summary>
        public static void ClearHistory()
        {
            _lastAlchemyTurnNumber = -1;
            _alchemiesThisTurn = 0;
        }
    }
}