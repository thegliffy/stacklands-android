using System;
using System.Collections;

[Serializable]
public class CutsceneStep_SetView : CutsceneStep
{
	public ViewType ViewType;

	public override IEnumerator Process()
	{
		WorldManager.instance.SetViewType(ViewType);
		yield break;
	}
}
