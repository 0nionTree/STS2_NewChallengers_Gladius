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
    // UI 렌더링 상태를 추적하는 전역 플래그
    // =========================================================================
    public static class CardTextRenderState
    {
        // UI가 카드 설명을 생성 중이거나 시각 요소를 그릴 때 true가 됩니다.
        public static bool IsGeneratingDescription = false;
    }

    // =========================================================================
    // [UI 패치] 카드 내구도 아이콘 표시
    // =========================================================================
    [HarmonyPatch(typeof(NCard), "UpdateVisuals")]
    public static class DurableCardUIPatch
    {
        private static Texture2D? _cachedNormalIcon = null;
        private static Texture2D? _cachedProtectedIcon = null;

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<NCard, TextureRect> _uiCache = new();

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

            CardModel cardModel = __instance.Model!;
            bool isDurable = cardModel.GetDurability().isDurable;

            if (!_uiCache.TryGetValue(__instance, out TextureRect? durIcon)) 
            {
                if (!isDurable) return;

                Control? cardContainer = __instance.GetNodeOrNull<Control>("CardContainer");
                if (cardContainer == null) return;

                durIcon = cardContainer.GetNodeOrNull<TextureRect>("DurabilityIcon");
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
                    
                    durIcon.Texture = _cachedNormalIcon;

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

                // 만든(혹은 찾은) UI 노드를 캐시에 등록해 다음 호출부터는 검색하지 않음
                _uiCache.Add(__instance, durIcon);
            }

            // UI 업데이트
            if (isDurable)
            {
                durIcon.Visible = true;

                bool isProtected = false;
                if (!__instance.Model!.IsCanonical && cardModel.Owner?.Creature?.Powers != null)
                {
                    isProtected = DurabilityProtectionManager.IsProtected(cardModel.Owner.Creature);
                }

                _cachedNormalIcon ??= GD.Load<Texture2D>("res://Gladius/images/durability_icon.png");
                _cachedProtectedIcon ??= GD.Load<Texture2D>("res://Gladius/images/durability_icon_protected.png");

                // 텍스처 재할당 비용 최소화
                durIcon.Texture = isProtected ? _cachedProtectedIcon : _cachedNormalIcon;

                // 캐싱된 아이콘의 자식 노드를 바로 가져옴
                Label durLabel = durIcon.GetNode<Label>("DurabilityLabel");

                int displayDurability = cardModel.GetDurability().CurrentDurability;
                if (cardModel.Pile != null && cardModel.Pile.Type == PileType.Play)
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
                // 내구도 부여 효과가 사라졌을 때 숨김 처리
                durIcon.Visible = false;
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
    // 턴 종료 시 패 유지
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
    // OnPlay에서 카드 효과 실행 시작 시 내구도를 1 차감
    // =========================================================================
    [HarmonyPatch] // 단일 타겟 지정 대신 TargetMethods를 사용합니다.
    public static class RealTimeDurabilityDeductPatch
    {
        // 부모 클래스뿐만 아니라, 오버라이드(재정의)된 모든 OnPlay 메서드를 찾아서 패치 타겟으로 지정합니다.
        public static IEnumerable<MethodBase> TargetMethods()
        {
            // 원본 게임 어셈블리와 현재 모드 어셈블리(Gladius)의 모든 클래스 타입을 가져옵니다.
            var allTypes = typeof(CardModel).Assembly.GetTypes()
                           .Concat(Assembly.GetExecutingAssembly().GetTypes());

            foreach (var type in allTypes)
            {
                // CardModel을 상속받은 커스텀 카드 또는 기본 카드인지 확인
                if (type.IsSubclassOf(typeof(CardModel)))
                {
                    // 해당 클래스 내부에서 직접 재정의(Override)한 OnPlay 메서드만 정확히 찾습니다.
                    var method = type.GetMethod("OnPlay", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    
                    if (method != null)
                    {
                        yield return method; // 찾은 모든 OnPlay에 패치를 예약합니다.
                    }
                }
            }
        }

        // 위의 TargetMethods로 모인 수백 개의 OnPlay 메서드 직전에 이 코드가 실행됩니다.
        [HarmonyPrefix]
        public static bool Prefix(CardModel __instance, ref Task __result)
        {
            // 복사본이라면 종료
            if (__instance.IsClone) return true;

            var durabilityData = __instance.GetDurability();

            // 내구도가 존재하는 카드라면 로직 개입
            if (durabilityData != null && durabilityData.isDurable)
            {
                // 1. 이미 내구도가 0 이하라면? 카드 효과(원본 OnPlay) 발동을 강제로 막습니다.
                if (durabilityData.CurrentDurability <= 0)
                {
                    __result = Task.CompletedTask; // Task 에러 방지용 더미 완료 반환
                    return false; // 원본 함수 스킵 (헛스윙 처리)
                }

                // 2. 내구도 차감 전 보호(파워 등) 여부 확인
                if (__instance.Owner?.Creature != null && DurabilityProtectionManager.GetProtectionStacks(__instance.Owner.Creature) > 0)
                {
                    DurabilityProtectionManager.ConsumeOneProtectionStack(__instance.Owner.Creature);
                }
                else
                {
                    // 보호 횟수가 없다면 내구도를 실시간으로 1 차감합니다.
                    durabilityData.CurrentDurability = Math.Max(0, durabilityData.CurrentDurability - 1);
                }
            }

            // 내구도가 충분하거나 내구도 카드가 아니면 정상적으로 카드 효과 발동
            return true;
        }
    
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref Task __result)
        {
            var durabilityData = __instance.GetDurability();

            // 내구도가 존재하는 카드라면 사용 전 내구도를 현재 내구도로 변경
            if (durabilityData != null && durabilityData.isDurable)
            {
                durabilityData.WasDurability = durabilityData.CurrentDurability;
            }
        }
    }

    // =========================================================================
    // OnPlayWrapper가 끝난 뒤, 모든 내구도가 소진된 카드의 내구도 초기화
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
    public static class DurableCardDeductPatch
    {
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
                {
                    if (__instance.Keywords.Contains(GladiusKeywords.Artifact))
                    {
                        durabilityData.CurrentDurability = durabilityData.BaseDurability;
                        durabilityData.WasDurability = durabilityData.BaseDurability;
                    }
                    else
                    {
                        DurabilityExtensions.ResetDurability(__instance);
                    }
                }
            }
        }
    }
    
    // =========================================================================
    // 카드의 소모될 내구도를 미리 계산하여 목적지 결정
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), "GetResultPileTypeForCardPlay")]
    public static class MaterializedPlayPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref PileType __result)
        {
            var durabilityData = __instance.GetDurability();

            if (durabilityData != null && durabilityData.isDurable)
            {
                // 1. 현재 적용된 '내구도 보호'의 총 횟수(스택)를 가져옵니다. 
                // (※ DurabilityProtectionManager에 스택을 int로 반환하는 메서드를 새로 만들어주세요)
                int protectionStacks = 0;
                if (__instance.Owner?.Creature != null)
                {
                    protectionStacks = DurabilityProtectionManager.GetProtectionStacks(__instance.Owner.Creature);
                }

                // 2. 이번 사용으로 '실제로 차감될' 예상 내구도 계산 
                // (총 발동 횟수에서 보호받는 횟수를 뺍니다. 단, 0보단 작아질 수 없음)
                int expectedDeduction = Math.Max(0, 1 - protectionStacks);

                // 3. 카드 사용 완료 시점의 '예상 내구도' 계산
                int predictedDurability = durabilityData.CurrentDurability - expectedDeduction;

                // 3. 사용 후 내구도가 0 이하가 될 예정이라면 소멸로 예약
                if (predictedDurability <= 0)
                {
                    __result = PileType.Exhaust;
                }
                // 소멸하지 않을 경우, 손으로 돌아옴
                else
                {
                    __result = PileType.Hand;
                }
            }
        }
    }
    
    // =========================================================================
    // 고대 존재의 유물로 인한 카드 및 유물 교체
    // =========================================================================
    // 오로바스의 ArchaicTooth 클래스의 TranscendenceUpgrades 프로퍼티의 Getter를 패치 타겟으로 지정합니다.
    [HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceUpgrades")]
    public static class ArchaicToothTranscendencePatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Dictionary<ModelId, CardModel> __result)
        {
            // __result는 원본 엔진이 반환하려는 딕셔너리입니다.
            // 여기에 Gladius의 기본 카드와 이에 대응하는 고대 카드 매핑을 추가합니다.

            // 채굴 -> 세공
            __result.Add(ModelDb.Card<SwordGirding>().Id, ModelDb.Card<Oath>());
        }
    }
    // 오로바스의 TouchOfOrobas 클래스의 TranscendenceUpgrades 프로퍼티의 Getter를 패치 타겟으로 지정합니다.
    [HarmonyPatch(typeof(TouchOfOrobas), "get_RefinementUpgrades")]
    public static class TouchOfOrobasTranscendencePatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref Dictionary<ModelId, RelicModel> __result)
        {
            // __result는 원본 엔진이 반환하려는 딕셔너리입니다.
            // 여기에 Gladius의 기본 유물과 이에 대응하는 고대 유물 매핑을 추가합니다.

            // 채굴 -> 세공
            __result.Add(ModelDb.Relic<MineralPouch>().Id, ModelDb.Relic<DimensionalPouch>());
        }
    }

    // =========================================================================
    // 카드 복사본 생성 시 내구도 정보도 복사
    // =========================================================================
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CreateClone))]
    public static class CloneDurabilityPatch
    {
        [HarmonyPostfix]
        public static void Postfix(CardModel __instance, ref CardModel __result)
        {
            var mainDurability = __instance.GetDurability();
            var copyDurability = __result.GetDurability();

            if (!mainDurability.isDurable) return;

            copyDurability.isDurable = mainDurability.isDurable;
            copyDurability.BaseDurability = mainDurability.BaseDurability;
            copyDurability.CurrentDurability = mainDurability.CurrentDurability;
            copyDurability.WasDurability = mainDurability.CurrentDurability;
        }
    }
}