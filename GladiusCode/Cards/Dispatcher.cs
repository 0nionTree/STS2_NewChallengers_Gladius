using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Hooks; // IterateCombatHookListeners가 있는 네임스페이스 (경로에 맞게 수정 필요)

namespace Gladius;

public abstract class DIspatcher
{
    // 원본 엔진의 형식을 본딴 커스텀 훅 발송기
    public static async Task DispatchAlchemyTriggered(ICombatState combatState)
    {
        // 엔진의 순정 함수를 사용하여 현재 전투에서 훅을 받을 수 있는 모든 모델(카드, 유물, 파워 등)을 가져옵니다.
        foreach (AbstractModel model in Hook.IterateCombatHookListeners(combatState))
        {
            // 가져온 모델이 우리의 커스텀 카드(GladiusCard)라면
            if (model is GladiusCard gCard)
            {
                // 카드 내부에 만들어둔 훅을 실행합니다.
                await gCard.OnAlchemyTriggered();
                
                // 엔진 원본 코드처럼 실행이 끝났음을 모델에 알려줍니다.
                model.InvokeExecutionFinished(); 
            }
        }
    }
}