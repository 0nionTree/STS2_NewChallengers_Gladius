using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using System.Diagnostics;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Godot;

namespace Gladius.GladiusCode.Patches
{
    [HarmonyPatch(typeof(NCard), "UpdateVisuals")] 
    public static class DurableCardUIPatch
    {
        [HarmonyPostfix]
        public static void Postfix(NCard __instance)
        {
            // ⭐ 핵심 수정 부분: __instance.Card 가 아니라 __instance.Model 을 사용합니다!
            // (GodotObject나 Node의 속성에 맞게 CardModel 데이터를 가져옵니다)
            if (__instance.Model is IDurableCard durableCard) 
            {
                // 라벨 노드를 찾습니다.
                Label labelNode = __instance.GetNodeOrNull<Label>("DurabilityLabel");

                // 라벨이 없으면 최초 1회 생성합니다.
                if (labelNode == null)
                {
                    labelNode = new Label();
                    labelNode.Name = "DurabilityLabel";
                    
                    // 우측 상단 닻(Anchor) 고정
                    labelNode.SetAnchorsPreset(Control.LayoutPreset.TopRight);
                    labelNode.Position = new Vector2(160, -30); 
                    
                    // 시안색(Cyan) 텍스트 디자인
                    labelNode.AddThemeColorOverride("font_color", new Color("00ffff")); 
                    labelNode.AddThemeColorOverride("font_outline_color", Colors.Black);
                    labelNode.AddThemeConstantOverride("outline_size", 4);
                    labelNode.AddThemeFontSizeOverride("font_size", 45); 
                    
                    __instance.AddChild(labelNode); 
                }

                // 내구도 숫자를 업데이트합니다.
                if (durableCard.Durability > 0)
                    labelNode.Text = $"★ {durableCard.Durability}";
                else
                    labelNode.Text = ""; 
            }
            else
            {
                // 구체화 카드가 아닌 일반 카드인데 라벨이 남아있다면 숨깁니다.
                // (카드 객체는 게임 내에서 재활용(Pool)되기 때문에 지워주는 처리가 필요합니다)
                Label labelNode = __instance.GetNodeOrNull<Label>("DurabilityLabel");
                if (labelNode != null)
                {
                    labelNode.Text = "";
                }
            }
        }
    }    
    // =========================================================================
    // [효과 1] 턴 종료 시 패 유지 (UI 표시 완벽 차단)
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "get_ShouldRetainThisTurn")]
    public static class MaterializedShouldRetainPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref bool __result)
        {
            if (__instance.Keywords.Contains(GladiusKeywords.Materialized))
            {
                // 1. 현재 이 프로퍼티를 누가 불렀는지 호출 스택을 역추적합니다.
                StackTrace stackTrace = new StackTrace(false);
                bool isCalledByUI = false;

                // 2. 나를 호출한 부모 함수들의 이름을 하나하나 검사합니다.
                foreach (var frame in stackTrace.GetFrames())
                {
                    // GetMethod()가 null일 수 있으므로 변수에 먼저 담고 확인합니다.
                    var method = frame.GetMethod();
                    if (method == null) continue; // 메서드 정보가 없으면 무시하고 다음 프레임으로 넘어감

                    string methodName = method.Name;
                    
                    // 만약 '카드 설명을 가져오는 함수'가 이 프로퍼티를 불렀다면?
                    if (methodName.Contains("GetDescriptionForPile") || methodName.Contains("GetDescriptionForUpgradePreview"))
                    {
                        isCalledByUI = true; // "아, 지금 텍스트 그리는 중이구나!"
                        break;
                    }
                }

                // 3. UI가 텍스트를 그리기 위해 물어본 게 "아닐 때만" true를 줍니다.
                // 즉, 턴이 끝나서 시스템이 카드를 버릴지 말지 기계적으로 판단할 때만 true가 됩니다!
                if (!isCalledByUI)
                {
                    __result = true;
                }
            }
        }
    }

    // =========================================================================
    // [효과 2] 카드를 사용했을 때 버린 카드 더미로 갈 경우에만 패로 되돌아옴
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
    public static class MaterializedPlayPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref PileType __result)
        {
            // 1. 이 카드가 '내구도' 명찰(IDurableCard)을 달고 있다면?
            if (__instance is IDurableCard durableCard)
            {
                // 카드를 냈으므로 내구도를 1 깎습니다.
                durableCard.Durability--;

                // 내구도가 0 이하가 되었다면 무조건 소멸(Exhaust)시킵니다.
                if (durableCard.Durability <= 0)
                {
                    __result = PileType.Exhaust;
                }
                // 내구도가 남았고, 원래 버려질(Discard) 예정이었다면 패(Hand)로 되돌립니다.
                else if (__result == PileType.Discard)
                {
                    __result = PileType.Hand;
                }
            }
            // 2. 내구도 시스템은 없지만 구체화 키워드만 있는 '무한 연성물' 카드인 경우 (선택 사항)
            else if (__instance.Keywords.Contains(GladiusKeywords.Materialized) && __result == PileType.Discard)
            {
                __result = PileType.Hand;
            }
        }
    }
}