using System;

[Serializable]
public class VariableFilterCurseActive : VariableFilter
{
	public CurseType Curse;

	public override bool IsMet()
	{
		if (WorldManager.instance == null)
		{
			return true;
		}
		return WorldManager.instance.CurseIsActive(Curse);
	}
}
