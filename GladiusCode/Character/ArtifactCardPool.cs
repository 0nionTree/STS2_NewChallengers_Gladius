using BaseLib.Abstracts;
using Gladius;
using Godot;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class ArtifactCardPool : CustomCardPoolModel
{
	public const string energyColorName = "colorless";

	public override string Title => "artifact";

	public override string EnergyColorName => "colorless";

	public override string CardFrameMaterialPath => "card_frame_colorless";

	public override Color DeckEntryCardColor => new Color("A3A3A3FF");

	public override bool IsColorless => true;

	protected override CardModel[] GenerateAllCards()
	{
		return
        [
            ModelDb.Card<HornedSword>(),
			ModelDb.Card<DragonScale>(),
			ModelDb.Card<ShoulderGuards>(),
			ModelDb.Card<SerratedDagger>(),
			ModelDb.Card<DragonClaw>(),
			ModelDb.Card<VortexSpear>(),
			ModelDb.Card<RitualPlumb>(),
			
		];
	}
}
