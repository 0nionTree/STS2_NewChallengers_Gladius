using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Condensation() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 응축
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new IntVar("Screening", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>(),
        EnergyHoverTip];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 다음 턴 카드 선별 및 용기 획득
		await PowerCmd.Apply<ScreeningNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["Screening"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Screening"].UpgradeValueBy(1);
    }
}