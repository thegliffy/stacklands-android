using System.Collections.Generic;
using UnityEngine;

public class GoblinAttack : CardData
{
	public override void UpdateCard()
	{
		if (!MyGameCard.TimerRunning)
		{
			MyGameCard.StartTimer(30f, SpawnCreature, SokLoc.Translate("card_event_goblin_attack_status_1"), GetActionId("SpawnCreature"));
		}
		base.UpdateCard();
	}

	[TimedAction("spawn_creature")]
	public void SpawnCreature()
	{
		List<EnemySetCardBag> list = new List<EnemySetCardBag>();
		if (CitiesManager.instance.Wellbeing >= 30)
		{
			list.Add(EnemySetCardBag.Cities_BasicEnemy);
			list.Add(EnemySetCardBag.Cities_AdvancedEnemy);
		}
		else
		{
			list.Add(EnemySetCardBag.Cities_BasicEnemy);
		}
		float t = Mathf.InverseLerp(20f, 80f, CitiesManager.instance.Wellbeing);
		int num = Mathf.RoundToInt(Mathf.Lerp(20f, 180f, t));
		foreach (CardIdWithEquipment item in SpawnHelper.GetEnemiesToSpawn(WorldManager.instance.GameDataLoader.GetSetCardBagForEnemyCardBagList(list), num))
		{
			Combatable obj = WorldManager.instance.CreateCard(base.transform.position, item, faceUp: false, checkAddToStack: false) as Combatable;
			obj.HealthPoints = obj.ProcessedCombatStats.MaxHealth;
			obj.MyGameCard.SendIt();
		}
		MyGameCard.DestroyCard(spawnSmoke: true);
	}
}
