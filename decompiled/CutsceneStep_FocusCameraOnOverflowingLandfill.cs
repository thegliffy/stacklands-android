using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CutsceneStep_FocusCameraOnOverflowingLandfill : CutsceneStep
{
	public override IEnumerator Process()
	{
		List<GameCard> allCardsOnBoard = WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id);
		for (int i = 0; i < allCardsOnBoard.Count; i++)
		{
			if (allCardsOnBoard[i].CardData is RecyclingCenter { IsOverflowing: not false } || allCardsOnBoard[i].CardData is Landfill { IsOverflowing: not false })
			{
				GameCamera.instance.TargetCardOverride = allCardsOnBoard[i];
				break;
			}
		}
		yield break;
	}
}
