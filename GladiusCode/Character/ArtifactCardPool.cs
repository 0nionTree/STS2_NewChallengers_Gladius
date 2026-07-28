using BaseLib.Abstracts;
using Gladius;
using Gladius.GladiusCode.Extensions;
using Godot;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class ArtifactCardPool : CustomCardPoolModel
{
	public override string Title => "artifact";

	public override Color DeckEntryCardColor => new Color("0e0f37");

	public override bool IsColorless => true;

    public override bool IsShared => true;
	
    public override bool SeenByDefault => true;

    public override float H => 1.0f; //Hue; changes the color.
    public override float S => 1.0f; //Saturation
    public override float V => 1.0f; //Brightness
	
    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

	// 1. 이미지 로드 (필수)
	// 게임 성능을 위해 static readonly로 선언하여 메모리에 한 번만 올려둡니다.
	// 이미지 경로는 실제 프로젝트 환경에 맞게 수정해 주세요.
	private static readonly Texture2D ArtifactFrame = GD.Load<Texture2D>("res://Gladius/images/frames/artifact_frame.png");

	// ... (기존 설정들: ShaderColor, BigEnergyIconPath 등) ...

	// 2. 프레임 교체 훅 (필수)
	// 이 풀에 속한 카드를 렌더링할 때 무조건 방금 불러온 커스텀 이미지를 씌웁니다.
	public override Texture2D? CustomFrame(CustomCardModel card)
	{
		return ArtifactFrame;
	}

	protected override CardModel[] GenerateAllCards()
	{
		return
        [
            ModelDb.Card<HornedSword>(),
            ModelDb.Card<TwinDragonHorns>(),
			ModelDb.Card<DragonScale>(),
			ModelDb.Card<ShoulderGuards>(),
			ModelDb.Card<SerratedDagger>(),
			ModelDb.Card<DragonClaw>(),
			ModelDb.Card<PurifyingLantern>(),
			ModelDb.Card<VortexSpear>(),
			ModelDb.Card<RitualPlumb>()
		];
	}
}
