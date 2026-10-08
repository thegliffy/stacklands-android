public class ActionTimeModifier
{
	public delegate bool ActionTimeModifierFunc(ActionTimeParams parameters);

	public float SpeedModifier;

	public ActionTimeModifierFunc Matches;

	public ActionTimeModifier(ActionTimeModifierFunc modifySpeedFunc, float modifier)
	{
		Matches = modifySpeedFunc;
		SpeedModifier = modifier;
	}
}
