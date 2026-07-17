using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Consecrate() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 축성
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<DragonOrb>(),
        HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Add(await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue),
                context: choiceContext,
                player: Owner,
                filter: null,
                source: this),
            PileType.Draw, CardPilePosition.Bottom);
        
        // 용옥 생성
        CardModel cardModel = CombatState!.CreateCard<DragonOrb>(Owner);
        if (IsUpgraded) // 강화된 상태라면 생성한 카드 강화
        {
            CardCmd.Upgrade(cardModel);
        }
        // 생성한 카드 손으로 가져오기
        await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, Owner);
		await Cmd.Wait(0.2f);
    }

    // protected override void OnUpgrade()
}