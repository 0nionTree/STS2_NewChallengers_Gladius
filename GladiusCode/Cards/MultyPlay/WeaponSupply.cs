using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using Gladius.GladiusCode;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Gladius;

[Pool(typeof(GladiusCardPool))]
public class WeaponSupply() : GladiusCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    // 무기 배급 - 연성
	public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public override bool IsRequiredMaterial => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new IntVar("Durability", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard<HornedSword>(IsUpgraded), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Alchemy), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Artifact), 
        HoverTipFactory.FromKeyword(GladiusKeywords.Material)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        CardModel? card = await Alchemy<HornedSword>(choiceContext, IsUpgraded, DynamicVars["Durability"].IntValue);
        if (card != null)
        {
            Player? targetPlayer = cardPlay.Target!.Player!;
            if (targetPlayer != null)
            {
                CardModel? copy = targetPlayer.RunState.CreateCard(card.CanonicalInstance, targetPlayer);

                // 강화 상태 복제
                if (card.IsUpgraded)
                {
                    CardCmd.Upgrade(copy);
                }

                // 인챈트 상태 복제
                if (card.Enchantment != null)
                {
                    EnchantmentModel clonedEnchant = (EnchantmentModel)card.Enchantment.MutableClone();
                    CardCmd.Enchant(clonedEnchant, copy, clonedEnchant.Amount);
                }

                // 내구도 상태 복제
                var du1 = card.GetDurability();
                var du2 = copy.GetDurability();

                du2.isDurable = du1.isDurable;
                du2.BaseDurability = du1.BaseDurability;
                du2.CurrentDurability = du1.CurrentDurability;
                du2.WasDurability = du1.WasDurability;

                await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, Owner);
            }
        }
    }

    //protected override void OnUpgrade()
}