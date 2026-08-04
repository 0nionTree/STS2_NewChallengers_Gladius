using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class Keyboard() : GladiusCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // 건반
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new IntVar("Screening", 3),
        new BlockVar(4m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(GladiusKeywords.Screening),
        HoverTipFactory.FromKeyword(GladiusKeywords.Remain)];
        
	//public override IEnumerable<CardKeyword> CanonicalKeywords =>
	//	[];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 선별 실행
        var (remains, _) = await ScreeningManager.Screening(CombatState, choiceContext, Owner, DynamicVars["Screening"].IntValue);
        // 잔류한 카드가 있다면
        if (remains != null && remains.Any())
        {
            int count = remains.Count();

            // 잔류한 카드 수만큼 방어도 획득 반복
            for (int i = 0; i < count; i++)
            {
                // 방어도 획득
                await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1m);
    }
}