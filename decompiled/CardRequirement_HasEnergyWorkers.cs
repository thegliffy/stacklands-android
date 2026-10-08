using System;

[Serializable]
public class CardRequirement_HasEnergyWorkers : CardRequirement
{
	public override string RequirementDescriptionNeed(int multiplier)
	{
		return SokLoc.Translate("label_requirement_energy_workers");
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return SokLoc.Translate("label_requirement_energy_workers_negative");
	}

	public override bool Satisfied(GameCard card)
	{
		if (card.CardData.HasEnergyInput() && card.CardData.WorkerAmount > 0)
		{
			return card.CardData.WorkerAmountMet();
		}
		return false;
	}
}
