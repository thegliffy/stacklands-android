using System;
using System.Collections;

[Serializable]
public class CutsceneStep_PlaceGeniusOnTimeMachine : CutsceneStep
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
		CardData card = WorldManager.instance.GetCard("time_machine");
		if (genius != null && card != null)
		{
			genius.MyGameCard.RemoveFromStack();
			genius.MyGameCard.SetParent(card.MyGameCard);
			card.MyGameCard.RotWobble(1f);
		}
		yield break;
	}
}
