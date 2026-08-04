using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.ValueProps;
using Gladius.GladiusCode.History;
using Gladius.GladiusCode;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Unity() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    // 합일
    protected override bool ShouldGlowGoldInternal => 
        ScreeningHistory.GetScreenedRemainsThisTurn(Owner) > 0;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new BlockVar(8, ValueProp.Move)
        /*new PowerVar<UnityPower>(100)*/];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Screening),
        HoverTipFactory.FromKeyword(GladiusKeywords.Remain)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 방어도 획득
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        // 이번 턴 잔류시킨 카드가 있다면
        int remainCards = ScreeningHistory.GetScreenedRemainsThisTurn(Owner);
        if(remainCards > 0)
        {
            // 잔류시킨 카드 수만큼 카드 뽑기
            await CardPileCmd.Draw(choiceContext, remainCards, Owner);
        }

        // 다음 턴 용기 획득
		//await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
		//await PowerCmd.Apply<UnityPower>(choiceContext, Owner.Creature, DynamicVars.Power<UnityPower>().BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}