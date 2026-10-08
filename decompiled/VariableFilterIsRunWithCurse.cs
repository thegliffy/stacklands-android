using System;

[Serializable]
public class VariableFilterIsRunWithCurse : VariableFilter
{
	public CurseType Curse;

	public override bool IsMet()
	{
		if (WorldManager.instance == null)
		{
			return true;
		}
		if (Curse == CurseType.Death && WorldManager.instance.CurrentRunOptions.IsDeathEnabled)
		{
			return true;
		}
		if (Curse == CurseType.Happiness && WorldManager.instance.CurrentRunOptions.IsHappinessEnabled)
		{
			return true;
		}
		return false;
	}
}
