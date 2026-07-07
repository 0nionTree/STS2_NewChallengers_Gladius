/*using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.CardSelection;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Reinforce() : GladiusCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(3m, BlockProps.card)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Artifact)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 방어도 획득
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

        // 2. 패에서 수리할 연성물 카드 선택 
        CardModel? selectedModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1), 
            context: choiceContext, 
            player: base.Owner, 
            filter: (CardModel card) => card.Keywords.Contains(GladiusKeywords.Artifact) && card is IDurableCard, 
            source: this
        )).FirstOrDefault();

        // 3. 카드를 정상적으로 선택했다면 내구도 1 회복
        if (selectedModel is IDurableCard durableCard)
        {
            // 순수하게 데이터(논리) 수치만 올립니다.
            durableCard.Durability += 1;
            // 선택 창이 닫히고 패로 돌아오는 과정에서 게임 엔진이 알아서 UI를 한 번 새로고침(Refresh) 해줍니다.
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}*/