using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class DragonsWrath() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 용의 분노
    //protected override IEnumerable<DynamicVar> CanonicalVars => 
    //    [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonAuraPower>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Retain,
        CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 용기 획득
        var count = Owner.Creature.GetPower<DragonAuraPower>()?.Amount ?? 0;
        await PowerCmd.Remove<DragonAuraPower>(Owner.Creature);
        if (count > 0)
        {
            count += count;
		    await PowerCmd.Apply<DragonAuraNextTurnPower>(choiceContext, Owner.Creature, count, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}