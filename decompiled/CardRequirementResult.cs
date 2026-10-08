using System;
using System.Collections;

[Serializable]
public abstract class CardRequirementResult
{
	public abstract IEnumerator Perform(GameCard card);

	public abstract IEnumerator EndOfCutscenePerform(GameCard card);

	public abstract RequirementType GetRequirementType();

	public abstract string RequirementDescriptionNegative(int multiplier, GameCard card);

	public abstract string RequirementDescriptionPositive(int multiplier, GameCard card);
}
