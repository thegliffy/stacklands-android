using System;

[Serializable]
public class CardRequirement_WorkerAmountMet : CardRequirement
{
	public override string RequirementDescriptionNeed(int multiplier)
	{
		return SokLoc.Translate("label_requirement_worker_amount_met");
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return SokLoc.Translate("label_requirement_worker_amount_met_negative");
	}

	public override bool Satisfied(GameCard card)
	{
		if (card.CardData.WorkerAmount > 0 && card.CardData.WorkerAmountMet())
		{
			return true;
		}
		return false;
	}
}
