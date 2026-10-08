using UnityEngine;

public class Hoverable : MonoBehaviour
{
	public virtual string GetTitle()
	{
		return "Title";
	}

	public virtual string GetDescription()
	{
		return "Text";
	}
}
