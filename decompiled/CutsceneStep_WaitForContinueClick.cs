using System;
using System.Collections;

[Serializable]
public class CutsceneStep_WaitForContinueClick : CutsceneStep
{
	[Term]
	public string ButtonTerm;

	public override IEnumerator Process()
	{
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate(ButtonTerm));
	}
}
