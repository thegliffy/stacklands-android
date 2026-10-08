using System;

[Serializable]
public class RunOptions
{
	public MoonLength MoonLength = MoonLength.Normal;

	public bool IsPeacefulMode;

	public bool IsGreedEnabled;

	public bool IsHappinessEnabled;

	public bool IsDeathEnabled;
}
