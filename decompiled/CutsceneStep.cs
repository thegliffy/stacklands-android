using System;
using System.Collections;

[Serializable]
public class CutsceneStep
{
	[NonSerialized]
	public int StepIndex;

	public static string Title
	{
		get
		{
			return WorldManager.instance.CutsceneTitle;
		}
		set
		{
			WorldManager.instance.CutsceneTitle = value;
		}
	}

	public static string Text
	{
		get
		{
			return WorldManager.instance.CutsceneText;
		}
		set
		{
			WorldManager.instance.CutsceneText = value;
		}
	}

	public virtual IEnumerator Process()
	{
		yield break;
	}
}
