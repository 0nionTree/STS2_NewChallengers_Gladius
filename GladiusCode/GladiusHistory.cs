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
        // 마지막으로 연성한 전투를 기억
        private static PlayerCombatState? _lastAlchemyPlayerCombat = null;
        // 이번 턴에 진행한 연성 횟수
        private static int _alchemiesThisTurn = 0;
        // 이번 전투동안 진행한 연성 횟수
        private static int _alchemiesThisCombat = 0;

        /// <summary>
        /// 연성을 실행할 때마다 호출하여 기록합니다.
        /// </summary>
        public static void RecordAlchemy(Player player)
        {
            if (player?.PlayerCombatState == null) return;

            int currentTurn = player.PlayerCombatState.TurnNumber;
            PlayerCombatState playerCombatState = player.PlayerCombatState;

            // 마지막으로 연성했던 턴과 현재 턴이 다르면 (즉, 턴이 바뀌었다면) 카운트 초기화
            if (currentTurn != _lastAlchemyTurnNumber)
            {
                _lastAlchemyTurnNumber = currentTurn;
                _alchemiesThisTurn = 0;
            }
            // 마지막으로 연성했던 전투와 현재 전투가 다르면 카운트 초기화
            if (_lastAlchemyPlayerCombat == null || _lastAlchemyPlayerCombat != playerCombatState)
            {
                _lastAlchemyPlayerCombat = playerCombatState;
                _alchemiesThisCombat = 0;
            }

            // 이번 턴 연성 횟수 1 증가
            _alchemiesThisTurn++;
            // 이번 전투 연성 횟수 1 증가
            _alchemiesThisCombat++;
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

    // 선별 기록
    public static class ScreeningHistory
    {
        private static int _lastScreeningTurnNumber = -1;
        private static PlayerCombatState? _lastScreeningPlayerCombat = null;

        private static int _screeningsThisTurn = 0;
        private static int _screeningsThisCombat = 0;
        private static int _screenedRemainsThisTurn = 0;
        private static int _screenedRemainsThisCombat = 0;

        /// <summary>
        /// 선별(Screening) 실행 시 기록을 업데이트합니다.
        /// </summary>
        public static void RecordScreening(Player player, int remainsCount = 0)
        {
            if (player?.PlayerCombatState == null) return;

            int currentTurn = player.PlayerCombatState.TurnNumber;
            PlayerCombatState playerCombatState = player.PlayerCombatState;

            // 턴이 바뀌면 턴 카운터 초기화
            if (currentTurn != _lastScreeningTurnNumber)
            {
                _lastScreeningTurnNumber = currentTurn;
                _screeningsThisTurn = 0;
                _screenedRemainsThisTurn = 0;
            }

            // 전투가 바뀌면 전투 카운터 초기화
            if (_lastScreeningPlayerCombat == null || _lastScreeningPlayerCombat != playerCombatState)
            {
                _lastScreeningPlayerCombat = playerCombatState;
                _screeningsThisCombat = 0;
                _screenedRemainsThisCombat = 0;
            }

            _screeningsThisTurn++;
            _screeningsThisCombat++;
            _screenedRemainsThisTurn += remainsCount;
            _screenedRemainsThisCombat += remainsCount;
        }

        public static int GetScreeningsThisTurn(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (player.PlayerCombatState.TurnNumber != _lastScreeningTurnNumber) return 0;
            return _screeningsThisTurn;
        }

        public static int GetScreeningsThisCombat(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (_lastScreeningPlayerCombat != player.PlayerCombatState) return 0;
            return _screeningsThisCombat;
        }

        public static int GetScreenedRemainsThisTurn(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (player.PlayerCombatState.TurnNumber != _lastScreeningTurnNumber) return 0;
            return _screenedRemainsThisTurn;
        }

        public static int GetScreenedRemainsThisCombat(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (_lastScreeningPlayerCombat != player.PlayerCombatState) return 0;
            return _screenedRemainsThisCombat;
        }
    }
    // 용기 소모 기록
    public static class DragonAuraHistory
    {
        private static int _lastAuraConsumeTurnNumber = -1;
        private static PlayerCombatState? _lastAuraConsumePlayerCombat = null;

        // 용기 소모 횟수
        private static int _auraConsumesThisTurn = 0;
        private static int _auraConsumesThisCombat = 0;
        // 용기 소모량
        private static int _auraConsumedAmountThisTurn = 0;
        private static int _auraConsumedAmountThisCombat = 0;

        /// <summary>
        /// Dragon Aura의 Amount가 소모된 횟수와 총량 기록
        /// </summary>
        public static void RecordDragonAuraConsumed(Player player, int consumedAmount)
        {
            if (player?.PlayerCombatState == null) return;

            int currentTurn = player.PlayerCombatState.TurnNumber;
            PlayerCombatState playerCombatState = player.PlayerCombatState;

            // 마지막으로 기록한 턴과 현재 턴이 다르면 기록 초기화
            if (currentTurn != _lastAuraConsumeTurnNumber)
            {
                _lastAuraConsumeTurnNumber = currentTurn;
                _auraConsumesThisTurn = 0;
                _auraConsumedAmountThisTurn = 0;
            }

            // 마지막으로 기록한 전투와 현재 전투가 다르면 기록 초기화
            if (_lastAuraConsumePlayerCombat == null || _lastAuraConsumePlayerCombat != playerCombatState)
            {
                _lastAuraConsumePlayerCombat = playerCombatState;
                _auraConsumesThisCombat = 0;
                _auraConsumedAmountThisCombat = 0;
            }

            // 소모 횟수 누적
            _auraConsumesThisTurn++;
            _auraConsumesThisCombat++;
            // 소모량 누적
            _auraConsumedAmountThisTurn += consumedAmount;
            _auraConsumedAmountThisCombat += consumedAmount;
        }

        // 이번 턴 용기 소모 횟수 반환
        public static int GetDragonAuraConsumesThisTurn(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (player.PlayerCombatState.TurnNumber != _lastAuraConsumeTurnNumber) return 0;
            return _auraConsumesThisTurn;
        }
        // 이번 전투 용기 소모 횟수 반환
        public static int GetDragonAuraConsumesThisCombat(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (_lastAuraConsumePlayerCombat != player.PlayerCombatState) return 0;
            return _auraConsumesThisCombat;
        }
        // 이번 턴 용기 소모량 반환
        public static int GetDragonAuraConsumedAmountThisTurn(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (player.PlayerCombatState.TurnNumber != _lastAuraConsumeTurnNumber) return 0;
            return _auraConsumedAmountThisTurn;
        }
        // 이번 전투 용기 소모량 반환
        public static int GetDragonAuraConsumedAmountThisCombat(Player player)
        {
            if (player?.PlayerCombatState == null) return 0;
            if (_lastAuraConsumePlayerCombat != player.PlayerCombatState) return 0;
            return _auraConsumedAmountThisCombat;
        }
    }
}