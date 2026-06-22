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
            Control? cardContainer = __instance.GetNodeOrNull<Control>("CardContainer");
            if (cardContainer == null) return;

            if (__instance.Model is IDurableCard durableCard) 
            {
                TextureRect? durIcon = cardContainer.GetNodeOrNull<TextureRect>("DurabilityIcon");
                Label? durLabel = null;

                if (durIcon == null)
                {
                    durIcon = new TextureRect();
                    durIcon.Name = "DurabilityIcon";
                    
                    // ⭐ 핵심: 이 UI가 마우스 클릭/호버를 가로채지 못하게 투명 취급합니다.
                    durIcon.MouseFilter = Control.MouseFilterEnum.Ignore;
                    
                    // 기준점을 카드의 좌측 상단으로 단단히 고정합니다.
                    durIcon.SetAnchorsPreset(Control.LayoutPreset.TopLeft, true);
                    
                    // 💡 요청하신 크기와 위치! (크기가 58로 줄었으므로 이제 진정한 좌측 상단에 위치하게 됩니다)
                    durIcon.Size = new Vector2(58, 58);
                    durIcon.Position = new Vector2(-125, -230);
                    
                    durIcon.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
                    durIcon.Texture = GD.Load<Texture2D>("res://Gladius/images/durability_icon.png");

                    durLabel = new Label();
                    durLabel.Name = "DurabilityLabel";
                    
                    // 라벨 역시 마우스를 투과하게 만듭니다.
                    durLabel.MouseFilter = Control.MouseFilterEnum.Ignore;
                    
                    // 💡 텍스트를 58x58 아이콘 안에 꽉 채우고 정중앙에 정렬합니다.
                    durLabel.SetAnchorsPreset(Control.LayoutPreset.FullRect, true);
                    durLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    durLabel.VerticalAlignment = VerticalAlignment.Center;
                    
                    durLabel.AddThemeColorOverride("font_color", Colors.White);
                    durLabel.AddThemeColorOverride("font_outline_color", Colors.Black);
                    durLabel.AddThemeConstantOverride("outline_size", 6);
                    
                    // ⭐ 아이콘 크기(58)에 맞춰 폰트 크기를 기존 38에서 28로 줄여서 삐져나오지 않게 합니다.
                    durLabel.AddThemeFontSizeOverride("font_size", 28);
                    
                    durIcon.AddChild(durLabel);
                    cardContainer.AddChild(durIcon);
                }
                else
                {
                    durLabel = durIcon.GetNode<Label>("DurabilityLabel");
                }

                if (durableCard.Durability > 0)
                {
                    durIcon.Visible = true;
                    durLabel.Text = durableCard.Durability.ToString();
                }
                else
                {
                    durIcon.Visible = false;
                }
            }
            else
            {
                TextureRect? durIcon = cardContainer.GetNodeOrNull<TextureRect>("DurabilityIcon");
                if (durIcon != null)
                {
                    durIcon.Visible = false;
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
    // [효과 2] 카드를 '확정적으로 사용'할 때만 내구도를 1 차감합니다. (멀티플레이 안전!)
    // =========================================================================
    // 카드가 사용되는 핵심 래퍼 함수인 OnPlayWrapper가 시작되기 직전에 가로챕니다.
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
    public static class DurableCardDeductPatch
    {
        [HarmonyPrefix]
        public static void Prefix(CardModel __instance)
        {
            if (__instance is IDurableCard durableCard)
            {
                // 다른 모드나 UI가 예측할 때가 아닌, "진짜로 카드를 낼 때"만 깎입니다!
                durableCard.Durability--;
            }
        }
    }
    // =========================================================================
    // [효과 3] 카드의 목적지를 결정합니다. (내구도 차감은 하지 않고 확인만 합니다)
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
    public static class MaterializedPlayPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref PileType __result)
        {
            if (__instance is IDurableCard durableCard)
            {
                // 위 Prefix 패치에서 내구도가 이미 깎인 상태이므로, 
                // 여기서 내구도가 0 이하라면 이번 플레이가 마지막이었다는 뜻입니다!
                if (durableCard.Durability <= 0)
                {
                    __result = PileType.Exhaust; // 깔끔하게 소멸
                }
                else if (__result == PileType.Discard)
                {
                    __result = PileType.Hand; // 아직 내구도가 남았다면 패로 복귀
                }
            }
            // 일반적인 '무한 연성물' (내구도 시스템이 없는 경우)
            else if (__instance.Keywords.Contains(GladiusKeywords.Materialized) && __result == PileType.Discard)
            {
                __result = PileType.Hand;
            }
        }
    }
}