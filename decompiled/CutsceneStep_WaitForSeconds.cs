using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class CutsceneStep_WaitForSeconds : CutsceneStep
{
	public float Seconds;

	public override IEnumerator Process()
	{
		yield return new WaitForSeconds(Seconds);
	}
}
