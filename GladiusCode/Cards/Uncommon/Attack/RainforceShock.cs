using Gladius.GladiusCode.Cards;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class RainforceShock() : GladiusCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // 충격요법
    public override bool IsRequiredDurable => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, DamageProps.card),
        new IntVar("Durability", 1)];
        
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 대상 확인 후 단일 공격
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        // 연성물 선택 문구 생성
        var promptString = new LocString("combat_messages", "SELECT_DURABLE");
        // 내구도가 존재하는 카드 선택
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.GetDurability().isDurable, 
            source: this
        )).FirstOrDefault();
        // 선택된 내구도 카드가 있다면
        if (cardModel != null)
        {
            // 현재 내구도 증가
            DurabilityExtensions.VarianceDurability(cardModel, DynamicVars["Durability"].IntValue);
        }
        else
        {
            // 내구도 카드가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "DURABLES_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Durability"].UpgradeValueBy(1);
    }
}