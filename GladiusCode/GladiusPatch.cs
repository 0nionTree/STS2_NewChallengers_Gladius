using System;
using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Gladius.GladiusCode.Cards;
using System.Reflection;
using MegaCrit.Sts2.Core.Localization;

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

            CardModel cardModel = __instance.Model!;

            if (cardModel.Keywords.Contains(GladiusKeywords.Artifact)) 
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

                durIcon.Visible = true;
                if (cardModel.DynamicVars["CurrentDurability"].BaseValue > 0)
                {
                    durLabel.Text = cardModel.DynamicVars["CurrentDurability"].BaseValue.ToString();
                }
                else
                {
                    durLabel.Text = "X";
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
    // [효과 1] 턴 종료 시 패 유지
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
            if (__instance.Keywords.Contains(GladiusKeywords.Artifact))
            {
                __instance.DynamicVars["CurrentDurability"].BaseValue--;
            }
        }
    }

    // =========================================================================
    // [효과 3] 카드의 목적지를 결정합니다.
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
    public static class MaterializedPlayPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref PileType __result)
        {
            if (__instance.Keywords.Contains(GladiusKeywords.Artifact))
            {
                if (__instance.DynamicVars["CurrentDurability"].BaseValue <= 0)
                {
                    __result = PileType.Exhaust; 
                    __instance.DynamicVars["CurrentDurability"].BaseValue = __instance.DynamicVars["BaseDurability"].BaseValue;
                }
                else if (__result == PileType.Discard)
                {
                    __result = PileType.Hand; 
                }
            }
        }
    }
    // =========================================================================
    // CanPlay()함수에 Material이 없을 경우 Alchmy 효과가 있는 카드를 사용할 수 없도록 합니다.
    // =========================================================================
    [HarmonyPatch] // 💡 수정됨: 파라미터를 비우고 클래스 단위 타겟팅을 사용합니다.
    public class CanPlayPatch
    {
        // 💡 추가됨: CS0182 에러를 피하기 위해 TargetMethod()를 사용하여 메서드를 명시적으로 찾습니다.
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(CardModel), nameof(CardModel.CanPlay), new Type[] 
            { 
                typeof(UnplayableReason).MakeByRefType(), 
                typeof(AbstractModel).MakeByRefType() 
            });
        }

        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref bool __result, ref UnplayableReason reason, ref AbstractModel preventer)
        {
            // 연금 태그가 있는 카드가 아니라면 종료
            if (!__instance.Tags.Contains(GladiusTags.Alchemy)) return;
            // 1. 이미 다른 이유(에너지 부족 등)로 사용 불가라면 그대로 둡니다.
            if (!__result) return;

            // 2. 모드 전용 조건 체크: 손에 Material 키워드를 가진 카드가 하나라도 있는지 확인
            var hand = PileType.Hand.GetPile(__instance.Owner);
            bool hasMaterial = hand?.Cards?.Any(c => c.Keywords.Contains(GladiusKeywords.Material)) ?? false;
            
            // 3. 재료가 없다면 사용 불가 처리
            if (!hasMaterial)
            {
                __result = false;
                // UnplayableReason은 수정 불가하므로, 
                // 의미적으로 가장 적절한 'BlockedByCardLogic'을 사용합니다.
                reason |= UnplayableReason.BlockedByCardLogic;
                
                // 막은 주체를 지정해줍니다.
                preventer = __instance;
            }
        }
    }
    [HarmonyPatch] // 클래스 타입 대신 TargetMethod를 사용
    public class CustomDialoguePatch
    {
        // 리플렉션을 통해 internal 클래스의 메서드를 찾아옵니다.
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            // 1. internal 클래스 타입을 문자열로 찾음
            var type = AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.UnplayableReasonExtensions");
            // 2. 그 안의 메서드를 찾음
            return AccessTools.Method(type, "GetPlayerDialogueLine");
        }

        [HarmonyPostfix]
        public static void Postfix(UnplayableReason reason, AbstractModel preventer, ref object __result)
        {
            // 1. 우리가 정한 사용 불가 사유(BlockedByCardLogic)이면서
            // 2. 대상이 우리의 커스텀 카드(GladiusCard)인 경우
            if (reason.HasFlag(UnplayableReason.BlockedByCardLogic) && preventer is GladiusCard) 
            {
                // 메시지를 우리가 원하는 키값의 LocString으로 강제 교체합니다.
                __result = new LocString("combat_messages", "MATERIALS_MISSING");
            }
        }
    }
}