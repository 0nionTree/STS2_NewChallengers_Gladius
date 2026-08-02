using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Gladius.GladiusCode.Character;
using Gladius.GladiusCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gladius.GladiusCode.Relics;

/// <summary>
/// This is the base class for your mod's relics, which is set up to load the relic's images from your mod's resources.
/// When creating a relic, right click the Relics folder and create a new file with the Custom Relic template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
///
/// The [Pool] annotation marks this relic as being tied to your specific character. Inheriting from this class means
/// that your relics don't need to invidually say which pool they should be in.
/// </summary>
[Pool(typeof(GladiusRelicPool))]
public abstract class GladiusRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    public virtual Task OnAlchemyTriggered(CardModel artifact, CardModel metarial, Player? creator, PlayerChoiceContext choiceContext, bool isFirstThisTurn)
    {
        return Task.CompletedTask; 
    }
    // 선별 실행 시 카드 선택 완료 직후 자동으로 실행되는 함수
    public virtual Task OnScreeningPerformed(IEnumerable<CardModel>? remain, IEnumerable<CardModel>? falls, Player owner, PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
    // 선별 실행 시 카드 파일 이동 이후 자동으로 실행되는 함수
    public virtual Task OnScreenedCardsMoved(CardModel cardModel, bool isRemain, Player owner, PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
}