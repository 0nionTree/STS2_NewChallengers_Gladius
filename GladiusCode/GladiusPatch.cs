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
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;

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

            if (cardModel.GetDurability().isDurable) 
            {
                bool isProtected = false;
                if (!__instance.Model!.IsCanonical && cardModel.Owner?.Creature?.Powers != null)
                {
                    isProtected = DurabilityProtectionManager.IsProtected(cardModel.Owner.Creature);
                }

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

                // 내구도 감소 면역 상태일 경우
                if (isProtected)
                {
                    // 내구도가 보호받을 때 사용할 새 이미지 경로 (미리 폴더에 넣어두세요)
                    durIcon.Texture = GD.Load<Texture2D>("res://Gladius/images/durability_icon_protected.png");
                }
                else
                {
                    // 기존 일반 내구도 이미지
                    durIcon.Texture = GD.Load<Texture2D>("res://Gladius/images/durability_icon.png");
                }

                // 표시할 내구도 : 기본적으론 현재 내구도
                int displayDurability = cardModel.GetDurability().CurrentDurability;
                // 카드가 사용 중인 카드 파일에 있다면 사용 전 내구도 표시
                if (cardModel.Pile != null && cardModel.Pile!.Type == PileType.Play)
                    displayDurability = cardModel.GetDurability().WasDurability;

                if (displayDurability > 0)
                {
                    durLabel.Text = displayDurability.ToString();
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
            var durabilityData = __instance.GetDurability();

            // 내구도가 존재하는 카드인지 확인
            if (durabilityData.isDurable)
            {
                // 사용 전 내구도 저장
                durabilityData.WasDurability = durabilityData.CurrentDurability;

                // 내구도 감소 체크
                bool isProtected = false;

                // 보유한 내구도 감소 무효 파워 확인
                if (__instance.Owner?.Creature != null)
                {
                    isProtected = DurabilityProtectionManager.IsProtected(__instance.Owner.Creature);
                }

                // 최종적으로 내구도 감소 체크 후 내구도 감소
                if (!isProtected)
                    durabilityData.CurrentDurability = Math.Max(0, durabilityData.CurrentDurability - 1);
            }
        }
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref Task __result)
        {
            // 1. 원본 OnPlayWrapper가 생성한 비동기 작업(Task)을 가져옵니다.
            Task originalTask = __result;

            // 2. __result를 "원본 작업을 끝까지 기다렸다가 내구도를 동기화하는 새로운 작업"으로 교체합니다.
            __result = WaitForTaskAndSyncDurability(__instance, originalTask);
        }

        // 실제 비동기 대기 및 사후 처리를 담당할 도우미 함수
        private static async Task WaitForTaskAndSyncDurability(CardModel __instance, Task originalTask)
        {
            // 3. 카드의 발동 로직(데미지 계산 등)이 완전히 끝날 때까지 대기합니다.
            await originalTask;

            // 4. 모든 작업이 끝난 후(카드 효과 발동 완료 후) 표기용 내구도를 동기화합니다.
            var durabilityData = __instance.GetDurability();
            if (durabilityData != null && durabilityData.isDurable)
            {
                if (durabilityData.CurrentDurability == 0)
                    durabilityData.WasDurability = durabilityData.BaseDurability;
                else
                    durabilityData.WasDurability = durabilityData.CurrentDurability;
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
            // 카드 사용이 끝난 뒤 내구도가 존재하는 카드라면
            if (__instance.GetDurability().isDurable)
            {
                // 현재 내구도에 따라 보낼 카드 더미 변경
                // 현재 내구도가 0이라면 소멸 후 내구도 복구
                if (__instance.GetDurability().CurrentDurability <= 0)
                {
                    __result = PileType.Exhaust;
                    // 연성물 카드라면 
                    if (__instance.Keywords.Contains(GladiusKeywords.Artifact))
                        __instance.GetDurability().CurrentDurability = __instance.GetDurability().BaseDurability;
                    // 연성물 카드가 아니라면(별도의 효과로 내구도를 부여받았다면)
                    else
                    {
                        DurabilityExtensions.ResetDurability(__instance);
                    }
                }
                // 현재 내구도가 0이 아니라면 버리지 않고 손으로 다시 가져옴
                else if (__result == PileType.Discard)
                {
                    __result = PileType.Hand; 
                }
                // 사용 전 내구도 초기화
                //__instance.GetDurability().WasDurability = __instance.GetDurability().CurrentDurability;
            }
        }
    }
    // 오로바스의 ArchaicTooth 클래스의 TranscendenceUpgrades 프로퍼티의 Getter를 패치 타겟으로 지정합니다.
    [HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceUpgrades")]
    public static class ArchaicToothTranscendencePatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Dictionary<ModelId, CardModel> __result)
        {
            // __result는 원본 엔진이 반환하려는 딕셔너리입니다.
            // 여기에 Gladius의 타격/수비 카드와 이에 대응하는 고대 카드 매핑을 추가합니다.

            // 채굴 -> 세공
            __result.Add(ModelDb.Card<Mine>().Id, ModelDb.Card<Engraving>());
        }
    }
}