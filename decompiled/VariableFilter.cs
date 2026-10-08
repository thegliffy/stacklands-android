using System;

[Serializable]
public abstract class VariableFilter
{
	public virtual bool IsMet()
	{
		return true;
	}
}
