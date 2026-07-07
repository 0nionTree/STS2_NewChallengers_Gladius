using System;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Gladius.GladiusCode.Cards;

namespace Gladius.GladiusCode.Patches
{
    // =========================================================================
    // [효과 1 최적화] UI 렌더링 상태를 추적하는 전역 플래그
    // =========================================================================
    public static class CardTextRenderState
    {
        // UI가 카드 설명을 생성 중이거나 시각 요소를 그릴 때 true가 됩니다.
        public static bool IsGeneratingDescription = false;
    }

    // =========================================================================
    // [UI 패치] 카드 내구도 아이콘 표시 & 보존 키워드 텍스트 렌더링 방지
    // =========================================================================
    [HarmonyPatch(typeof(NCard), "UpdateVisuals")]
    public static class DurableCardUIPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            // 카드 UI 렌더링 시작 시 시스템이 보존 텍스트/아이콘을 붙이는 것을 막기 위해 플래그 켬
            CardTextRenderState.IsGeneratingDescription = true;
        }

        [HarmonyPostfix]
        public static void Postfix(NCard __instance)
        {
            // 카드 UI 렌더링 종료 시 플래그 끔 (원래 상태 복구)
            CardTextRenderState.IsGeneratingDescription = false;

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
    // [텍스트 생성 방어 패치] 툴팁 등에서 보존 키워드가 뜨는 것을 차단
    // =========================================================================
    // Ambiguous Match 에러 수정을 위해 정확한 매개변수 타입 명시 완료
    [HarmonyPatch(typeof(CardModel), "GetDescriptionForPile", new Type[] { typeof(PileType), typeof(Creature) })]
    public static class DescriptionForPilePatch
    {
        [HarmonyPrefix]
        public static void Prefix() { CardTextRenderState.IsGeneratingDescription = true; }

        [HarmonyPostfix]
        public static void Postfix() { CardTextRenderState.IsGeneratingDescription = false; }
    }

    [HarmonyPatch(typeof(CardModel), "GetDescriptionForUpgradePreview")]
    public static class DescriptionForUpgradePreviewPatch
    {
        [HarmonyPrefix]
        public static void Prefix() { CardTextRenderState.IsGeneratingDescription = true; }

        [HarmonyPostfix]
        public static void Postfix() { CardTextRenderState.IsGeneratingDescription = false; }
    }

    // =========================================================================
    // [효과 1] 턴 종료 시 패 유지 (성능 렉 없음!)
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "get_ShouldRetainThisTurn")]
    public static class MaterializedShouldRetainPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref bool __result)
        {
            if (__instance.Keywords.Contains(GladiusKeywords.Artifact) || __instance.Keywords.Contains(GladiusKeywords.Material))
            {
                // UI가 텍스트를 그리기 위해 물어본 게 "아닐 때만" 실제 엔진상으로 유지(true) 처리합니다.
                if (!CardTextRenderState.IsGeneratingDescription)
                {
                    __result = true;
                }
            }
        }
    }

    // =========================================================================
    // [효과 2] 카드를 '확정적으로 사용'할 때만 내구도를 1 차감합니다. 
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
    public static class DurableCardDeductPatch
    {
        [HarmonyPrefix]
        public static void Prefix(CardModel __instance)
        {
            if (__instance is IDurableCard durableCard)
            {
                durableCard.Durability--;
            }
        }
    }

    // =========================================================================
    // [효과 3] 카드의 목적지를 결정합니다. (Material 조건 추가됨)
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
    public static class MaterializedPlayPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref PileType __result)
        {
            if (__instance is IDurableCard durableCard)
            {
                if (durableCard.Durability <= 0)
                {
                    __result = PileType.Exhaust; 
                }
                else if (__result == PileType.Discard)
                {
                    __result = PileType.Hand; 
                }
            }
            // 일반적인 '무한 연성물' (내구도 시스템이 없는 경우)
            else if ((__instance.Keywords.Contains(GladiusKeywords.Artifact) || __instance.Keywords.Contains(GladiusKeywords.Material)) 
                     && __result == PileType.Discard)
            {
                __result = PileType.Hand;
            }
        }
    }
}