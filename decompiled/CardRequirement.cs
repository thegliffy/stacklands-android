using System;

[Serializable]
public abstract class CardRequirement
{
	public abstract bool Satisfied(GameCard card);

	public abstract string RequirementDescriptionNeed(int multiplier);

	public abstract string RequirementDescriptionNeedNegative(int multiplier);
}
