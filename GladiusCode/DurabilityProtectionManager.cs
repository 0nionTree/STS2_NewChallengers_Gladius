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

        public static bool IsProtected(Creature creature)
        {
            if (creature == null || !ProtectionMap.TryGetValue(creature, out var list)) return false;
            
            // 캐릭터가 가진 전체 파워가 아니라, '보호 파워 리스트(기껏해야 1~2개)'만 순회하므로 여전히 매우 빠릅니다.
            foreach (var power in list)
            {
                // 단 하나라도 활성화된 보호 파워가 있다면 보호됨(true)
                if (power.IsProtectionActive()) return true;
            }
            return false;
        }
    }
}