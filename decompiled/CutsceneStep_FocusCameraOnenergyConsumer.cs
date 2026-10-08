using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CutsceneStep_FocusCameraOnenergyConsumer : CutsceneStep
{
	public override IEnumerator Process()
	{
		List<GameCard> allCardsOnBoard = WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id);
		GameCamera.instance.TargetCardOverride = allCardsOnBoard.FirstOrDefault((GameCard x) => x.CardConnectorChildren.Any((CardConnector x) => (x.ConnectionType == ConnectionType.LV || x.ConnectionType == ConnectionType.HV) && x.CardDirection == CardDirection.input));
		yield break;
	}
}
