using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Replica() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 모조품
    public override bool IsRequiredDurable => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 1),
        new CardsVar(1)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact),
        HoverTipFactory.FromKeyword(GladiusKeywords.Durability),
        HoverTipFactory.FromKeyword(CardKeyword.Ethereal)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 카드 선택 안내 문구 지정
        var promptString = new LocString("combat_messages", "SELECT_ARTIFACT");
		// 손에 있는 연성물 카드 선택
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: Owner, 
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Artifact), 
            source: this
        )).FirstOrDefault();

        if (cardModel != null)
        {
            var durabilityData = cardModel.GetDurability();

            for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
			{
				CardModel card = cardModel.CreateClone();
                DurabilityExtensions.SetDurability(card, DynamicVars["Durability"].IntValue);
                card.AddKeyword(CardKeyword.Ethereal);
				await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
			}
        }
        else
        {
            // 연성물 카드가 없다고 안내 문구 출력
            LocString locString = new LocString("combat_messages", "ARTIFACTS_MISSING");
            TalkCmd.Play(locString, Owner.Creature, VfxColor.White);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}