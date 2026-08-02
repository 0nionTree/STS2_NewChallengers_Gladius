using System.Runtime.CompilerServices;
using Gladius.GladiusCode;
using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.History;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gladius;

public static class DurabilityExtensions
{
    // 내구도 관련 데이터
    public class Durability
    {
        public bool isDurable = false;
        public int BaseDurability = 0;
        public int CurrentDurability = 0;
        public int WasDurability = 0;
        public bool isRequiredMaterial = false;
        public bool isRequiredDurable = false;
    }

    // CardModel에 Durability를 연결
    private static readonly ConditionalWeakTable<CardModel, Durability> DurabilityDataMap = new();

    // 내구도 데이터 호출
    public static Durability GetDurability(this CardModel card)
    {
        // 호출한 카드의 내구도 데이터가 없다면
        if (!DurabilityDataMap.TryGetValue(card, out var data))
        {
            // 내구도 데이터 새로 생성
            data = new Durability();

            // 호출한 카드가 내구도가 지정된 GladiusCard라면 수치 초기화 진행
            if (card is GladiusCard gCard)
            {
                data.isDurable = gCard.IsDurable;
                data.BaseDurability = gCard.BaseDurability;
                data.CurrentDurability = gCard.BaseDurability;
                data.WasDurability = gCard.BaseDurability;
                data.isRequiredMaterial = gCard.IsRequiredMaterial;
                data.isRequiredDurable = gCard.IsRequiredDurable;
            }

            // 새로 생성된 내구도 데이터 등록
            DurabilityDataMap.Add(card, data);
        }
        
        return data;
    }
    
    // 내구도 증감만, 소멸은 하지 않음(별도 소멸 매커니즘 구현 필요)
    public static void VarianceDurability(CardModel cardModel, int index)
    {   // 소멸 불가
        var durabilityData = cardModel.GetDurability();
        // 내구도가 없는 카드라면 즉시 종료
        if (!durabilityData.isDurable) return;
        // 현재 내구도를 index만큼 증감(0 미만으로 감소하지 않음)
        durabilityData.CurrentDurability = Math.Max(0, durabilityData.CurrentDurability + index);
        durabilityData.WasDurability = durabilityData.CurrentDurability;
    }
    // 내구도 증감 뒤 자동 소멸
    public static async Task VarianceDurability(CardModel cardModel, int index, PlayerChoiceContext choiceContext)
    {
        var durabilityData = cardModel.GetDurability();
        // 내구도가 없는 카드라면 즉시 종료
        if (!durabilityData.isDurable) return;
        // 현재 내구도를 index만큼 증감(0 미만으로 감소하지 않음)
        durabilityData.CurrentDurability = Math.Max(0, durabilityData.CurrentDurability + index);
        durabilityData.WasDurability = durabilityData.CurrentDurability;

        // 내구도가 0 이하라면 소멸 진행
        if (durabilityData.CurrentDurability > 0) return;
        await ExhaustArtifact(choiceContext, cardModel);
    }
    // 내구도 지정만, 소멸은 하지 않음(별도 소멸 매커니즘 구현 필요)
    public static void SetDurability(CardModel cardModel, int index)
    {
        var durabilityData = cardModel.GetDurability();
        // 내구도가 없는 카드라면 즉시 종료
        if (!durabilityData.isDurable) return;
        // 현재 내구도를 index와 같게 변경
        durabilityData.CurrentDurability = index;
        durabilityData.WasDurability = durabilityData.CurrentDurability;
    }
    // 내구도 지정 후 0일 경우 소멸
    public static async Task SetDurability(CardModel cardModel, int index, PlayerChoiceContext choiceContext)
    {
        var durabilityData = cardModel.GetDurability();
        // 내구도가 없는 카드라면 즉시 종료
        if (!durabilityData.isDurable) return;
        // 현재 내구도를 index와 같게 변경
        durabilityData.CurrentDurability = index;
        durabilityData.WasDurability = durabilityData.CurrentDurability;

        // 내구도가 0 이하라면 소멸 진행
        if (durabilityData.CurrentDurability > 0) return;
        await ExhaustArtifact(choiceContext, cardModel);
    }
    public static async Task ExhaustArtifact(PlayerChoiceContext choiceContext, CardModel cardModel)
    {
        var durabilityData = cardModel.GetDurability();
        // 카드 소멸 후 내구도 복구
        await CardCmd.Exhaust(choiceContext, cardModel);
        durabilityData.CurrentDurability = durabilityData.BaseDurability;
        
        // 사용 전 내구도를 현재 내구도로 변경
        durabilityData.WasDurability = durabilityData.CurrentDurability;
    }
    // 소모품이 아니도록 초기화
    public static void ResetDurability(CardModel cardModel)
    {
        var durabilityData = cardModel.GetDurability();

        if (cardModel.Keywords.Contains(GladiusKeywords.Artifact))
        {
            cardModel.RemoveKeyword(GladiusKeywords.Artifact);
        }
        
        if (cardModel.Keywords.Contains(GladiusKeywords.Durability))
        {
            cardModel.RemoveKeyword(GladiusKeywords.Durability);
        }

        durabilityData.isDurable = false;
        durabilityData.BaseDurability = 0;
        durabilityData.CurrentDurability = 0;
    }
    // 소모품 카드로 변경 및 연성 시너지
    public static async Task MakeDurable(PlayerChoiceContext choiceContext, ICombatState combatState, CardModel targetCard, int durability, Player creater, bool addArtifactKeyword = false)
    {
        var durabilityData = targetCard.GetDurability();

        if (!durabilityData.isDurable)
        {
            durabilityData.isDurable = true;
        }
        durabilityData.BaseDurability = durability;
        durabilityData.CurrentDurability = durabilityData.BaseDurability;
        durabilityData.WasDurability = durabilityData.BaseDurability;

        if (addArtifactKeyword && !targetCard.Keywords.Contains(GladiusKeywords.Durability))
        {
            targetCard.AddKeyword(GladiusKeywords.Durability);
        }

        if (!targetCard.Keywords.Contains(GladiusKeywords.Artifact))
        {
            targetCard.AddKeyword(GladiusKeywords.Artifact);
        }
        
        bool isFirstAlchemyThisTurn = AlchemyHistory.GetAlchemiesThisTurn(creater) == 1;

        // 연성 훅 보내기
        if (combatState != null)
        {
            await GladiusEventDispatcher.DispatchAlchemyTriggered(combatState, targetCard, null, creater, choiceContext, isFirstAlchemyThisTurn);
        }
    } 
}