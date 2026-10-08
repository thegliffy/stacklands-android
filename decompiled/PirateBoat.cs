using System.Collections.Generic;
using UnityEngine;

public class PirateBoat : CardData
{
	public float SpawnTime = 20f;

	[ExtraData("spawns_remaining")]
	public int SpawnsRemaining = 3;

	public int Demand = 20;

	public override bool CanBeDragged => false;

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		if (!MyGameCard.IsDemoCard)
		{
			Demand = Mathf.Min(100, 3 + WorldManager.instance.CurrentRunVariables.PirateBoatsBribed * 3);
		}
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyGameCard == null)
		{
			return otherCard.Id == "gold";
		}
		if (WorldManager.instance.BoughtWithGoldChest(otherCard.MyGameCard, Demand) || WorldManager.instance.BoughtWithGold(otherCard.MyGameCard, Demand))
		{
			return true;
		}
		return false;
	}

	public void Buy()
	{
		MyGameCard.DestroyCard(spawnSmoke: true, playSound: false);
		QuestManager.instance.SpecialActionComplete("bribe_pirate_boat");
		WorldManager.instance.CurrentRunVariables.PirateBoatsBribed++;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild)
		{
			GameCard child = MyGameCard.Child;
			if (WorldManager.instance.BoughtWithGold(child, Demand))
			{
				WorldManager.instance.RemoveCardsFromStackPred(child, Demand, (GameCard x) => x.CardData.Id == "gold");
				Buy();
			}
			else if (WorldManager.instance.BoughtWithGoldChest(child, Demand))
			{
				WorldManager.instance.BuyWithChest(child, Demand);
				Buy();
			}
		}
		if (!MyGameCard.TimerRunning)
		{
			MyGameCard.StartTimer(SpawnTime, SpawnPirates, SokLoc.Translate("card_pirate_boat_name"), GetActionId("SpawnPirates"));
		}
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		descriptionOverride = SokLoc.Translate("card_pirate_boat_status", LocParam.Create("count", Demand.ToString()));
	}

	[TimedAction("spawn_pirates")]
	public void SpawnPirates()
	{
		float maxStrength = (float)(1 + WorldManager.instance.CurrentRunVariables.PirateBoatsBribed * (2 + Mathf.Min(2, WorldManager.instance.CurrentRunVariables.PirateBoatsSpawned - 1))) * 30f;
		Combatable item = WorldManager.instance.GetCardPrefab("pirate") as Combatable;
		foreach (CardIdWithEquipment item2 in SpawnHelper.GetEnemiesToSpawn(new List<Combatable> { item }, maxStrength))
		{
			WorldManager.instance.CreateCard(base.transform.position, item2, faceUp: false, checkAddToStack: false).MyGameCard.SendIt();
		}
		MyGameCard.DestroyCard(spawnSmoke: true);
	}
}
