using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsRoar() : GladiusCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // 용의 문양
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("DragonAuraPower", 2m), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.DragonAura)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용기 획득
		await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, DynamicVars["DragonAuraPower"].IntValue, Owner.Creature, this);
        // 카드 뽑기
		await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DragonAuraPower"].UpgradeValueBy(1m);
    }
}