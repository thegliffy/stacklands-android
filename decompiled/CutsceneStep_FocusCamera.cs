using System;
using System.Collections;
using System.Linq;

[Serializable]
public class CutsceneStep_FocusCamera : CutsceneStep
{
	[Card]
	public string CardId;

	public bool FocusType;

	public CardType Type;

	public override IEnumerator Process()
	{
		if (!FocusType && !string.IsNullOrEmpty(CardId))
		{
			GameCamera.instance.TargetCardOverride = WorldManager.instance.GetCard(CardId);
		}
		else if (FocusType)
		{
			GameCamera.instance.TargetCardOverride = WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id).FirstOrDefault((GameCard x) => x.CardData.MyCardType == Type);
		}
		yield break;
	}
}
