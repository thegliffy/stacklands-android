using System.Collections;
using System.Collections.Generic;

public class CutsceneStep_DestroyCard : CutsceneStep
{
	[Card]
	public string CardId;

	public override IEnumerator Process()
	{
		List<CardData> cards = WorldManager.instance.GetCards(CardId);
		if (cards.Count > 0)
		{
			cards[cards.Count - 1].MyGameCard.DestroyCard(spawnSmoke: true);
		}
		yield break;
	}
}
