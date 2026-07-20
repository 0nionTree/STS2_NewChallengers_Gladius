using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsWrath() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 용의 분노
    //protected override IEnumerable<DynamicVar> CanonicalVars => 
    //    [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용기 획득
        var count = Owner.Creature.GetPower<DragonAuraPower>()?.Amount ?? 0;
        if (count > 0)
		    await PowerCmd.Apply<DragonAuraPower>(choiceContext, Owner.Creature, count, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}