using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Gladius
{
    // 내구도를 보호하는 기능이 있는 모든 파워가 가져야 할 공통 인터페이스
    public interface IDurabilityProtector
    {
        // 현재 이 파워가 내구도를 보호할 수 있는 상태인가? (스택이 남아있는가 등)
        bool IsProtectionActive();
        // 현재 이 파워가 막아줄 수 있는 남은 내구도 보호 횟수(스택)를 반환
        int GetProtectionStacks();
        // 내구도 보호를 1회 소모시킴 (파워 스택 1 감소 처리 등)
        void ConsumeOneStack();
    }

    public static class DurabilityProtectionManager
    {
        // 특정 파워 클래스가 아니라, IDurabilityProtector를 상속받은 '모든 보호 파워들의 리스트'를 저장하도록 변경
        private static readonly ConditionalWeakTable<Creature, List<IDurabilityProtector>> ProtectionMap = new();

        public static void Register(Creature creature, IDurabilityProtector power)
        {
            if (creature == null) return;
            if (!ProtectionMap.TryGetValue(creature, out var list))
            {
                list = new List<IDurabilityProtector>();
                ProtectionMap.Add(creature, list);
            }
            if (!list.Contains(power)) list.Add(power);
        }

        public static void Unregister(Creature creature, IDurabilityProtector power)
        {
            if (creature != null && ProtectionMap.TryGetValue(creature, out var list))
            {
                list.Remove(power);
            }
        }

        // =========================================================
        // 단 하나라도 스택이 남아있으면 보호 상태(true)
        // =========================================================
        public static bool IsProtected(Creature creature)
        {
            return GetProtectionStacks(creature) > 0;
        }

        // =========================================================
        // 캐릭터가 가진 전체 보호 파워의 총합 스택 계산 (예측용)
        // =========================================================
        public static int GetProtectionStacks(Creature creature)
        {
            if (creature == null || !ProtectionMap.TryGetValue(creature, out var list)) return 0;
            
            int maxStacks = 0; // 가장 높은 스택을 저장할 변수

            foreach (var power in list)
            {
                if (power.IsProtectionActive())
                {
                    // 현재 저장된 최대값과 이번 파워의 스택 중 더 큰 값을 다시 최대값으로 저장합니다.
                    maxStacks = Math.Max(maxStacks, power.GetProtectionStacks());
                }
            }
            
            return maxStacks; // 가장 큰 스택 수치만 반환합니다.
        }

        // =========================================================
        // 보호 스택을 1 소모 (OnPlay 실시간 차감용)
        // =========================================================
        public static void ConsumeOneProtectionStack(Creature creature)
        {
            if (creature == null || !ProtectionMap.TryGetValue(creature, out var list)) return;

            foreach (var power in list.ToList())
            {
                // 스택이 남아있는 첫 번째 보호 파워를 찾아서 1을 깎습니다.
                if (power.IsProtectionActive() && power.GetProtectionStacks() > 0)
                {
                    power.ConsumeOneStack();
                }
            }
        }
    }
}