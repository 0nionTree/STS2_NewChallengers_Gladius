using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Hooks;
using HarmonyLib;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players; // IterateCombatHookListeners가 있는 네임스페이스 (경로에 맞게 수정 필요)

namespace Gladius;

// 연성(Alchemy) 발생 시 관련 효과를 가진 객체들에게 이벤트를 전달하는 전역 클래스입니다.
public static class AlchemyEventDispatcher
{
    // Hook 클래스 내부에 숨겨진(private) IterateCombatHookListeners 메서드의 정보를 리플렉션(Harmony AccessTools)으로 찾아 저장합니다.
    private static readonly MethodInfo IterateHooksMethod = 
        AccessTools.Method(typeof(Hook), "IterateCombatHookListeners");

    // 전투 중에 연성(Alchemy)이 실행되었을 때 호출할 비동기 이벤트 전달 함수입니다.
    public static async Task DispatchAlchemyTriggered(ICombatState combatState, CardModel artifect, CardModel metarial, Player? creator)
    {
        // 찾고자 하는 메서드가 정상적으로 로드되지 않았을 경우, 튕김 방지(안전 장치)를 위해 함수를 즉시 종료합니다.
        if (IterateHooksMethod == null) 
        {
            return;
        }

        // 찾아둔 private 메서드를 강제로 실행하여 현재 전투에서 이벤트를 수신할 수 있는 모든 객체(리스너) 목록을 가져옵니다.
        var listeners = (IEnumerable<AbstractModel>?)IterateHooksMethod.Invoke(null, new object[] { combatState });

        // 리스너 목록을 성공적으로 가져왔는지(null이 아닌지) 확인합니다.
        if (listeners != null)
        {
            // 가져온 리스너 목록을 하나씩 순회하며 검사합니다.
            foreach (AbstractModel model in listeners)
            {
                // 순회 중인 객체가 우리가 만든 커스텀 카드(GladiusCard) 타입인지 확인합니다.
                if (model is GladiusCard gCard)
                {
                    // 카드 내부에 정의된 연성 발동 시 작동할 훅(OnAlchemyTriggered)을 비동기로 실행합니다.
                    await gCard.OnAlchemyTriggered(artifect, metarial, creator);
                    
                    // 엔진의 정상적인 훅 처리 흐름에 맞춰 해당 객체의 이벤트 실행이 끝났음을 게임 시스템에 알립니다.
                    model.InvokeExecutionFinished();
                }
            }
        }
    }

    internal static async Task DispatchAlchemyTriggered(object combatState)
    {
        throw new NotImplementedException();
    }
}