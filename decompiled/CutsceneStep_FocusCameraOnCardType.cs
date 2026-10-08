using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CutsceneStep_FocusCameraOnCardType : CutsceneStep
{
	public List<string> ClassNames;

	public override IEnumerator Process()
	{
		List<GameCard> allCardsOnBoard = WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id);
		GameCamera.instance.TargetCardOverride = allCardsOnBoard.FirstOrDefault((GameCard x) => ClassNames.Contains(x.CardData.GetType().ToString()));
		yield break;
	}
}
