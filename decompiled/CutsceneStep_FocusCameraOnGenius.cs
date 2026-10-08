using System;
using System.Collections;

[Serializable]
public class CutsceneStep_FocusCameraOnGenius : CutsceneStep
{
	private CardData GetGenius()
	{
		CardData card = WorldManager.instance.GetCard("genius");
		if (card != null)
		{
			return card;
		}
		CardData card2 = WorldManager.instance.GetCard("robot_genius");
		if (card2 != null)
		{
			return card2;
		}
		return null;
	}

	public override IEnumerator Process()
	{
		CardData genius = GetGenius();
		if (genius != null)
		{
			GameCamera.instance.TargetCardOverride = genius;
		}
		yield break;
	}
}
