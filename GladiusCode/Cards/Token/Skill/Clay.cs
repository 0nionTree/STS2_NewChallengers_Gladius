using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.CardPools;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;

namespace Gladius;

[Pool(typeof(TokenCardPool))]
public class Clay() : GladiusCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // 진흙 - 소재
    public override IEnumerable<CardKeyword> CanonicalKeywords => [GladiusKeywords.Material, CardKeyword.Ethereal, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(2, BlockProps.card)];
        
    protected override async Task Material(PlayerChoiceContext choiceContext, CardModel artifactCard)
    {
        
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}