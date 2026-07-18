using BaseLib.Abstracts;
using BaseLib.Extensions;
using Gladius.GladiusCode.Extensions;

namespace Gladius.GladiusCode.Enchantments;

/// <summary>
/// 모드의 커스텀 인챈트를 위한 베이스 클래스입니다.
/// 커스텀 파워와 마찬가지로 모드 리소스에서 인챈트 이미지를 자동으로 불러오도록 설정되어 있습니다.
/// </summary>
public abstract class GladiusEnchantment : CustomEnchantmentModel
{
    // 아이콘 이미지 경로 (예: Gladius/images/enchantments/your_enchantment.png)
    // 확장 메서드인 EnchantmentImagePath()가 StringExtensions.cs에 구현되어 있어야 합니다.
    protected override string CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();

    /// <summary>
    /// 이 인챈트가 카드에 여러 번 부여될 수 있는지(스택형인지) 여부를 결정합니다.
    /// 기본값은 false이며, 필요에 따라 상속받은 클래스에서 true로 오버라이드할 수 있습니다.
    /// </summary>
    public override bool IsStackable => false;

    /// <summary>
    /// 카드 타입에 따라 인챈트 가능 여부를 결정할 수 있습니다.
    /// 특정 카드 타입(예: 공격, 스킬)에만 부여하고 싶다면 이 메서드를 오버라이드하세요.
    /// </summary>
    /* public override bool CanEnchantCardType(CardType cardType)
    {
        return true; 
    }
    */
}