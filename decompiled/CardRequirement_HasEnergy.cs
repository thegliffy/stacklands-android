using System;

[Serializable]
public class CardRequirement_HasEnergy : CardRequirement
{
	public override string RequirementDescriptionNeed(int multiplier)
	{
		return SokLoc.Translate("label_requirement_has_energy");
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return SokLoc.Translate("label_requirement_has_energy_negative");
	}

	public override bool Satisfied(GameCard card)
	{
		return card.CardData.HasEnergyInput();
	}
}
