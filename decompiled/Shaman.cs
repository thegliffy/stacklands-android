using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shaman : CardData
{
	public AudioClip GiveIdea;

	private List<string> AltarBlueprints = new List<string> { "blueprint_altar", "death_recipe", "greed_recipe", "happiness_recipe" };

	public float TalkTime = 20f;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (MyGameCard.Child == null && otherCard is BaseVillager)
		{
			return AltarBlueprints.Where((string x) => !WorldManager.instance.HasFoundCard(x)).Count() > 0;
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.Child != null && !MyGameCard.TimerRunning)
		{
			MyGameCard.StartTimer(TalkTime, Talking, SokLoc.Translate("card_shaman_status"), GetActionId("Talking"));
		}
		if (MyGameCard.Child == null && MyGameCard.TimerRunning)
		{
			MyGameCard.CancelTimer(GetActionId("Talking"));
		}
		base.UpdateCard();
	}

	[TimedAction("talking")]
	public void Talking()
	{
		if (MyGameCard.Child != null)
		{
			MyGameCard.Child.RemoveFromParent();
		}
		string cardId = (WorldManager.instance.HasFoundCard("blueprint_altar") ? AltarBlueprints.Where((string x) => !WorldManager.instance.HasFoundCard(x)).ToList().Choose() : "blueprint_altar");
		AudioManager.me.PlaySound2D(GiveIdea, 1f, 0.2f);
		CardData cardData = WorldManager.instance.CreateCard(base.Position, cardId, faceUp: true, checkAddToStack: false);
		WorldManager.instance.CreateSmoke(base.Position);
		cardData.MyGameCard.SendIt();
		if (AltarBlueprints.Count((string x) => !WorldManager.instance.HasFoundCard(x)) == 0)
		{
			WorldManager.instance.QueueCutscene(Cutscenes.ShamanLeaving(this));
		}
	}
}
