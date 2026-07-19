using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class WeaponAbsorption() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 무기 흡수
    public override bool IsRequiredDurable => true;

    //protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Durability),
        EnergyHoverTip];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 카드 선택 안내 문구 지정
        var promptString = new LocString("combat_messages", "SELECT_DURABLE");
		// 손에 있는 내구도가 존재하는 카드 선택
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.GetDurability().isDurable, 
            source: this
        )).FirstOrDefault();

        if (cardModel != null)
        {
            // 선택한 카드의 내구도만큼 에너지 획득
		    await PlayerCmd.GainEnergy(cardModel.GetDurability().CurrentDurability, Owner);
            // 선택한 카드 소멸
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
        else
        {
            // 소재가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "DURABLES_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}