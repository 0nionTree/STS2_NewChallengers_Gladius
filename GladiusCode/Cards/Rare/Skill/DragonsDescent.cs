using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using BaseLib.Extensions;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsDescent() : GladiusCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 강룡
	protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<DragonAuraPower>(2m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int value = ResolveEnergyXValue();
        int count = DynamicVars.Power<DragonAuraPower>().IntValue;
        if (IsUpgraded) count += 1;
        value *= count;
        await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, value, Owner.Creature, this);
    }

    //protected override void OnUpgrade()
}