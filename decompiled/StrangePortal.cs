using System.Collections.Generic;
using UnityEngine;

public class StrangePortal : Portal
{
	public float SpawnTime = 10f;

	public float TravelTime = 5f;

	public bool IsRarePortal;

	private float SpawnTimer;

	private float TravelTimer;

	[ExtraData("spawns_remaining")]
	public int SpawnsRemaining = 3;

	public override bool CanBeDragged => false;

	public override void UpdateCard()
	{
		if (!TransitionScreen.InTransition && !WorldManager.instance.InAnimation)
		{
			int num = ChildrenMatchingPredicateCount((CardData x) => x is BaseVillager);
			if (!MyGameCard.TimerRunning && num == 0)
			{
				MyGameCard.StartTimer(SpawnTime, SpawnCreature, SokLoc.Translate("new_portal_shaking"), GetActionId("SpawnCreature"));
				if (SpawnTimer > 0f)
				{
					MyGameCard.CurrentTimerTime = SpawnTimer;
				}
			}
			if (num > 0)
			{
				if (!WorldManager.instance.CurrentBoard.BoardOptions.CanTravelToForest)
				{
					GameCanvas.instance.ShowCantChangeBoardSpirit();
					Stay();
					return;
				}
				RemoveNonHuman();
				int cardCount = WorldManager.instance.GetCardCount((CardData x) => x is BaseVillager);
				if (ChildrenMatchingPredicateCount((CardData x) => x is BaseVillager) > MaxVillagerCount)
				{
					if (MyGameCard.TimerRunning && MyGameCard.TimerActionId == GetActionId("TakePortal"))
					{
						TravelTimer = MyGameCard.CurrentTimerTime;
					}
					MyGameCard.CancelTimer(GetActionId("TakePortal"));
					GameCanvas.instance.MaxVillagerCountPrompt("label_taking_portal_title", MaxVillagerCount);
					RemoveExcessVillagersInPortal();
				}
				if (num == cardCount)
				{
					if (MyGameCard.TimerRunning && MyGameCard.TimerActionId == GetActionId("TakePortal"))
					{
						TravelTimer = MyGameCard.CurrentTimerTime;
					}
					MyGameCard.CancelTimer(GetActionId("TakePortal"));
					GameCanvas.instance.OneVillagerNeedsToStayPrompt("label_taking_portal_title");
					RemoveLastVillagerInPortal();
				}
				else if (MyGameCard.TimerRunning && MyGameCard.TimerActionId != GetActionId("TakePortal"))
				{
					SpawnTimer = MyGameCard.CurrentTimerTime;
					MyGameCard.CancelTimer(GetActionId("SpawnCreature"));
					MyGameCard.StartTimer(TravelTime, base.TakePortal, SokLoc.Translate("card_stable_portal_status"), GetActionId("TakePortal"));
					if (TravelTimer > 0f)
					{
						MyGameCard.CurrentTimerTime = TravelTimer;
					}
				}
				else if (!MyGameCard.TimerRunning)
				{
					MyGameCard.StartTimer(TravelTime, base.TakePortal, SokLoc.Translate("card_stable_portal_status"), GetActionId("TakePortal"));
					if (TravelTimer > 0f)
					{
						MyGameCard.CurrentTimerTime = TravelTimer;
					}
				}
			}
			else
			{
				if (MyGameCard.TimerRunning && MyGameCard.TimerActionId == GetActionId("TakePortal"))
				{
					TravelTimer = MyGameCard.CurrentTimerTime;
				}
				MyGameCard.CancelTimer(GetActionId("TakePortal"));
			}
		}
		base.UpdateCard();
	}

	[TimedAction("spawn_creature")]
	public void SpawnCreature()
	{
		List<EnemySetCardBag> list = new List<EnemySetCardBag>();
		if (WorldManager.instance.CurrentMonth >= 24)
		{
			list.Add(EnemySetCardBag.BasicEnemy);
			list.Add(EnemySetCardBag.AdvancedEnemy);
			list.Add(EnemySetCardBag.Forest_BasicEnemy);
		}
		else if (WorldManager.instance.CurrentMonth >= 16)
		{
			list.Add(EnemySetCardBag.BasicEnemy);
			list.Add(EnemySetCardBag.AdvancedEnemy);
		}
		else
		{
			list.Add(EnemySetCardBag.BasicEnemy);
		}
		int value = Mathf.RoundToInt((float)Mathf.Max(12, WorldManager.instance.CurrentMonth) * 1.5f);
		value = Mathf.Clamp(value, 0, 70);
		if (IsRarePortal)
		{
			value = Mathf.RoundToInt((float)value * 1.5f);
		}
		foreach (CardIdWithEquipment item in SpawnHelper.GetEnemiesToSpawn(WorldManager.instance.GameDataLoader.GetSetCardBagForEnemyCardBagList(list), value))
		{
			Combatable obj = WorldManager.instance.CreateCard(base.transform.position, item, faceUp: false, checkAddToStack: false) as Combatable;
			obj.HealthPoints = obj.ProcessedCombatStats.MaxHealth;
			obj.MyGameCard.SendIt();
		}
		MyGameCard.DestroyCard(spawnSmoke: true);
	}
}
