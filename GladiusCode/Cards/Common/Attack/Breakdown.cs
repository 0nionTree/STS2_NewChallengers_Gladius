using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Breakdown() : GladiusCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // 깨뜨리기
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 1), new DamageVar(12m, DamageProps.card)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 카드 선택 메시지 초기화
        var promptString = new LocString("combat_messages", "SELECT_ARTIFACT");

        // 연성물 카드 선택
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.GetCustomData().isDurable, 
            source: this
        )).FirstOrDefault();

        if (cardModel != null)
        {
            // 선택된 연성물 카드가 존재한다면 내구도 감소
            await DurabilityExtensions.VarianceDurability(cardModel, -DynamicVars["Durability"].IntValue, choiceContext);
            // 타겟 확인 후 대미지
            ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        // 연성물 카드가 없다면 
        else
        {
            // 내구도 카드가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "DURABLES_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}