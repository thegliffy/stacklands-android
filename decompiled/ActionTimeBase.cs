public class ActionTimeBase
{
	public delegate bool ActionTimeBaseSpeedFunc(ActionTimeParams parameters);

	public ActionTimeBaseSpeedFunc Matches;

	public float BaseSpeed;

	public ActionTimeBase(ActionTimeBaseSpeedFunc baseSpeedFunc, float baseSpeed)
	{
		Matches = baseSpeedFunc;
		BaseSpeed = baseSpeed;
	}
}
