using NewChallengers_Gladius.GladiusCode.Character;
using NewChallengers_Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;

namespace NewChallengers_Gladius;

[Pool(typeof(GladiusCardPool))]
public class SwordGirding() : GladiusCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<HornedSword>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await CardPileCmd.AddGeneratedCardToCombat(CombatState!.CreateCard<HornedSword>(Owner), PileType.Hand, Owner);
		await Cmd.Wait(0.3f);
    }

    protected override void OnUpgrade()
    {
        
    }
}