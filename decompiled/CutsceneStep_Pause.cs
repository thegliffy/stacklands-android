using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CutsceneStep_Pause : CutsceneStep
{
	public float Time;

	public override IEnumerator Process()
	{
		yield return new WaitForSeconds(Time);
	}
}
