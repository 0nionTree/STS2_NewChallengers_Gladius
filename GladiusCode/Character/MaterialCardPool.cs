using BaseLib.Abstracts;
using Gladius;
using Godot;

namespace MegaCrit.Sts2.Core.Models.CardPools;

public sealed class MaterialCardPool : CustomCardPoolModel
{
	public const string energyColorName = "colorless";

	public override string Title => "material";

	public override string EnergyColorName => "colorless";

	public override string CardFrameMaterialPath => "card_frame_colorless";

	public override Color DeckEntryCardColor => new Color("A3A3A3FF");

	public override bool IsColorless => true;

	protected override CardModel[] GenerateAllCards()
	{
		return
        [
            ModelDb.Card<WroughtIron>(),
			ModelDb.Card<Clay>(),
			ModelDb.Card<Steel>(),
			ModelDb.Card<WindStone>(),
			ModelDb.Card<ThunderstruckWood>(),
			ModelDb.Card<DragonOrb>(),
			ModelDb.Card<Diamond>(),
			ModelDb.Card<TuningShard>()
		];
	}
}
