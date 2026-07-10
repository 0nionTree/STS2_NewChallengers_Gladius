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

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Cleanup() : GladiusCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 정리
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(9m, BlockProps.card)];

    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { GladiusTags.Alchemy };
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 카드 선택 안내 문구 지정
        var promptString = new LocString("combat_messages", "SELECT_MATERIAL");
		// Material(소재) 카드 선택
        var cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(promptString, 1), 
            context: choiceContext, 
            player: base.Owner, 
            // 필터 조건: Material 키워드가 있는 카드만 선택 가능
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Material), 
            source: this
        )).FirstOrDefault();
        if (cardModel != null)
        {
            // 소재 카드를 소멸 (Exhaust) 처리
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}