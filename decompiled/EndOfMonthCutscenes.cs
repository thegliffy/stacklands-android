using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EndOfMonthCutscenes
{
	public static string CutsceneTitle
	{
		get
		{
			return WorldManager.instance.CutsceneTitle;
		}
		set
		{
			WorldManager.instance.CutsceneTitle = value;
		}
	}

	public static string CutsceneText
	{
		get
		{
			return WorldManager.instance.CutsceneText;
		}
		set
		{
			WorldManager.instance.CutsceneText = value;
		}
	}

	public static int CurrentMonth => WorldManager.instance.CurrentMonth;

	private static float CalculateWaitFromSpeedup(float f)
	{
		return Mathf.Max(0.01f, 0.12f - f * 0.04f);
	}

	private static Food GetFoodToUseUp()
	{
		List<Food> cards = WorldManager.instance.GetCards<Food>();
		if (cards.Count == 0)
		{
			return null;
		}
		Demand currentDemand = ((WorldManager.instance.CurrentRunVariables.ActiveDemand != null) ? DemandManager.instance.GetDemandById(WorldManager.instance.CurrentRunVariables.ActiveDemand.DemandId) : null);
		return cards.OrderBy(delegate(Food c)
		{
			bool flag = c.MyGameCard.GetCardWithStatusInStack() != null;
			if ((bool)c.MyGameCard.HasCardInStack((CardData x) => x is MessHall))
			{
				return -1000 + c.MyGameCard.GetCardIndex();
			}
			if (c is Hotpot)
			{
				return -100 + c.FoodValue;
			}
			if (c.IsSpoiling && !flag)
			{
				return -5;
			}
			if (currentDemand != null)
			{
				if (currentDemand.CardToGet == "royal_banquet" && (c.Id == "fruit_salad" || c.Id == "wine" || c.Id == "roasted_meat" || c.Id == "olive_oil"))
				{
					return 5;
				}
				if (currentDemand.CardToGet == c.Id)
				{
					return 5;
				}
			}
			if (flag)
			{
				return 4;
			}
			if (WorldManager.instance.GetCardCount(c.Id) == 1 && !c.IsCookedFood)
			{
				return 3;
			}
			return (!c.IsCookedFood) ? 2 : 0;
		}).ThenBy((Food x) => x.FoodValue).ThenBy((Food x) => x.GetValue())
			.FirstOrDefault((Food x) => x.FoodValue > 0);
	}

	public static List<BaseVillager> GetVillagersToAge()
	{
		List<BaseVillager> list = new List<BaseVillager>();
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.IsCurrent && allCard.CardData is BaseVillager item)
			{
				list.Add(item);
			}
		}
		return list.OrderBy(delegate(BaseVillager x)
		{
			if (x is TeenageVillager)
			{
				return 0;
			}
			if (x is Villager)
			{
				return 1;
			}
			return (x is OldVillager) ? 2 : 3;
		}).ToList();
	}

	public static bool AnyVillagerWillChangeLifeStage(List<BaseVillager> villagers)
	{
		foreach (BaseVillager villager in villagers)
		{
			if (villager.WillChangeLifeStage() || villager.MyLifeStage == LifeStage.Dead)
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyAnimalWillDie(List<Animal> animals)
	{
		foreach (Animal animal in animals)
		{
			if (WorldManager.instance.CurrentMonth - animal.CreationMonth >= 5)
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerator CheckMakeSick()
	{
		List<BaseVillager> list = (from x in WorldManager.instance.GetCards<BaseVillager>()
			where !x.HasStatusEffectOfType<StatusEffect_Sick>() && !x.HasEquipableWithId("plague_mask")
			select x).ToList();
		List<Poop> cards = WorldManager.instance.GetCards<Poop>();
		List<BaseVillager> list2 = new List<BaseVillager>();
		foreach (Poop item in cards)
		{
			if (item.CanMakeSick)
			{
				if (list.Count <= 0)
				{
					break;
				}
				if (Random.value * 100f < item.SickChance)
				{
					BaseVillager baseVillager = list.Choose();
					baseVillager.AddStatusEffect(new StatusEffect_Sick());
					AudioManager.me.PlaySound2D(AudioManager.me.GetSick, 1f, 0.5f);
					list2.Add(baseVillager);
				}
			}
		}
		if (list2.Count == 0)
		{
			yield break;
		}
		foreach (BaseVillager item2 in list2)
		{
			CutsceneTitle = SokLoc.Translate("label_uh_oh");
			CutsceneText = SokLoc.Translate("label_villager_sick", LocParam.Create("villager", item2.Name));
			GameCamera.instance.TargetCardOverride = item2;
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
	}

	public static IEnumerator AgeVillagers(List<BaseVillager> villagersToAge)
	{
		CutsceneTitle = SokLoc.Translate("label_villager_age_birthday");
		CutsceneText = "";
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_age_villagers"));
		WorldManager.instance.EndOfMonthSpeedup = 0f;
		for (int i = 0; i < villagersToAge.Count; i++)
		{
			BaseVillager baseVill = villagersToAge[i];
			WorldManager.instance.EndOfMonthSpeedup += 1f;
			LifeStage num = baseVill.DetermineLifeStageFromAge(baseVill.Age);
			baseVill.Age++;
			LifeStage newLifeStage = baseVill.DetermineLifeStageFromAge(baseVill.Age);
			if (num == newLifeStage && newLifeStage != LifeStage.Dead)
			{
				continue;
			}
			if (newLifeStage == LifeStage.Dead)
			{
				QuestManager.instance.SpecialActionComplete("villager_old_age_dead");
				CutsceneTitle = SokLoc.Translate("label_villager_old_age_death", LocParam.Create("villager", baseVill.Name));
				yield return WorldManager.instance.KillVillagerCoroutine(baseVill, null, null);
				if (!WorldManager.instance.CheckAllVillagersDead())
				{
					if (WorldManager.instance.GetCardCount<BaseVillager>() > 1)
					{
						WorldManager.instance.QueueCutsceneIfNotPlayed("death_middle");
					}
					else
					{
						WorldManager.instance.QueueCutsceneIfNotPlayed("death_middle_villager");
					}
				}
			}
			else if (baseVill.ChangesCardOnStage)
			{
				string nextCardId = baseVill.DetermineCardFromStage(newLifeStage);
				GameCamera.instance.TargetPositionOverride = baseVill.MyGameCard.transform.position;
				yield return new WaitForSeconds(1f);
				if (newLifeStage == LifeStage.Teenager)
				{
					AudioManager.me.PlaySound2D(AudioManager.me.BecomeTeenager, 1f, 0.3f);
				}
				if (newLifeStage == LifeStage.Adult)
				{
					AudioManager.me.PlaySound2D(AudioManager.me.BecomeAdult, 1f, 0.3f);
				}
				if (newLifeStage == LifeStage.Elderly)
				{
					AudioManager.me.PlaySound2D(AudioManager.me.BecomeOld, 1f, 0.3f);
					QuestManager.instance.SpecialActionComplete("villager_old");
				}
				WorldManager.instance.CreateSmoke(baseVill.transform.position);
				WorldManager.instance.ChangeToCard(baseVill.MyGameCard, nextCardId);
				yield return new WaitForSeconds(1f);
			}
			yield return new WaitForSeconds(CalculateWaitFromSpeedup(WorldManager.instance.EndOfMonthSpeedup));
		}
		if (WorldManager.instance.CheckAllVillagersDead())
		{
			WorldManager.instance.VillagersStarvedAtEndOfMoon = true;
			yield return Cutscenes.EveryoneInSpiritWorldDead(WorldManager.instance.CurrentBoard.Id);
		}
	}

	public static IEnumerator KillAnimals(List<Animal> AnimalsToAge)
	{
		CutsceneTitle = SokLoc.Translate("label_animal_die");
		CutsceneText = "";
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
		WorldManager.instance.EndOfMonthSpeedup = 4f;
		for (int i = 0; i < AnimalsToAge.Count; i++)
		{
			Animal animal = AnimalsToAge[i];
			WorldManager.instance.EndOfMonthSpeedup += 1f;
			if (WorldManager.instance.CurrentMonth - animal.CreationMonth >= 5)
			{
				CutsceneTitle = SokLoc.Translate("label_villager_old_age_death", LocParam.Create("villager", animal.Name));
				yield return WorldManager.instance.KillVillagerCoroutine(animal, null, null, resetTargetOnDeath: false);
			}
			yield return new WaitForSeconds(CalculateWaitFromSpeedup(WorldManager.instance.EndOfMonthSpeedup));
		}
		GameCamera.instance.TargetPositionOverride = null;
	}

	public static List<CardData> GetCardsToFeed()
	{
		List<CardData> list = new List<CardData>();
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.IsCurrent)
			{
				CardData cardData = allCard.CardData;
				if (cardData is BaseVillager)
				{
					list.Add(cardData);
				}
				if (cardData is Kid)
				{
					list.Add(cardData);
				}
			}
		}
		return list.OrderBy(delegate(CardData x)
		{
			if (x is Kid)
			{
				return 0;
			}
			if (x.Id == "dog" || x.Id == "cat")
			{
				return 1;
			}
			if (x is TeenageVillager)
			{
				return 2;
			}
			return (x is OldVillager) ? 4 : 3;
		}).ToList();
	}

	public static IEnumerator FeedVillagers()
	{
		CutsceneText = SokLoc.Translate("label_time_to_eat");
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_feed_villagers"));
		WorldManager.instance.InEatingAnimation = true;
		CutsceneText = SokLoc.Translate("label_eating");
		int requiredFoodCount = WorldManager.instance.GetRequiredFoodCount();
		List<CardData> cardsToFeed = GetCardsToFeed();
		List<CardData> fedCards = new List<CardData>();
		yield return new WaitForSeconds(1f);
		WorldManager.instance.EndOfMonthSpeedup = 0f;
		for (int j = 0; j < cardsToFeed.Count; j++)
		{
			CardData cardToFeed = cardsToFeed[j];
			if (cardToFeed is BaseVillager baseVillager)
			{
				baseVillager.AteUncookedFood = false;
			}
			WorldManager.instance.EndOfMonthSpeedup += 1f;
			int foodForVillager = WorldManager.instance.GetCardRequiredFoodCount(cardToFeed.MyGameCard);
			for (int k = 0; k < foodForVillager; k++)
			{
				Food food = GetFoodToUseUp();
				if (food == null)
				{
					break;
				}
				GameCard foodCard = food.MyGameCard;
				foodCard.PushEnabled = false;
				foodCard.SetY = false;
				foodCard.Velocity = null;
				GameCamera.instance.TargetPositionOverride = cardToFeed.transform.position;
				GameCard originalParent = foodCard.Parent;
				GameCard originalChild = foodCard.Child;
				Vector3 originalPosition = foodCard.TargetPosition;
				List<GameCard> originalStack = foodCard.GetAllCardsInStack();
				foodCard.RemoveFromStack();
				foodCard.TargetPosition = cardToFeed.transform.position + new Vector3(0f, 0.1f, 0f);
				Vector3 diff;
				do
				{
					diff = foodCard.TargetPosition - foodCard.transform.position;
					yield return null;
				}
				while (diff.magnitude > 0.001f);
				AudioManager.me.PlaySound2D(AudioManager.me.Eat, Random.Range(0.8f, 1.2f), 0.3f);
				food.FoodValue--;
				requiredFoodCount--;
				foodCard.SetHitEffect();
				foodCard.transform.localScale *= 0.9f;
				yield return new WaitForSeconds(CalculateWaitFromSpeedup(WorldManager.instance.EndOfMonthSpeedup));
				if (cardToFeed is BaseVillager baseVillager2)
				{
					baseVillager2.HealthPoints = Mathf.Min(baseVillager2.HealthPoints + 3, baseVillager2.ProcessedCombatStats.MaxHealth);
					food.ConsumedBy(baseVillager2);
					TryCreatePoop(baseVillager2);
					if (!food.IsCookedFood)
					{
						baseVillager2.AteUncookedFood = true;
					}
				}
				if (food.FoodValue <= 0 && !(food is Hotpot))
				{
					food.FullyConsumed(cardToFeed);
					originalStack.Remove(foodCard);
					WorldManager.instance.Restack(originalStack);
					foodCard.DestroyCard(spawnSmoke: true);
				}
				else
				{
					foodCard.PushEnabled = true;
					foodCard.SetY = true;
					if (originalParent != null)
					{
						foodCard.SetParent(originalParent);
					}
					if (originalChild != null)
					{
						foodCard.SetChild(originalChild);
					}
					foodCard.TargetPosition = originalPosition;
				}
				if (k == foodForVillager - 1)
				{
					fedCards.Add(cardToFeed);
				}
			}
		}
		yield return new WaitForSeconds(1f);
		WorldManager.instance.InEatingAnimation = false;
		int num = requiredFoodCount;
		List<CardData> unfedVillagers = new List<CardData>();
		foreach (CardData item in cardsToFeed)
		{
			if (!fedCards.Contains(item) && !(item is Kid))
			{
				unfedVillagers.Add(item);
			}
		}
		int humansToDie = unfedVillagers.Count;
		if (num <= 0)
		{
			CutsceneText = SokLoc.Translate("label_everyone_fed");
			yield break;
		}
		SetStarvingHumanStatus(humansToDie);
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		for (int j = 0; j < unfedVillagers.Count; j++)
		{
			CardData cardData = unfedVillagers[j];
			if (!(cardData is Kid))
			{
				yield return WorldManager.instance.KillVillagerCoroutine(cardData as BaseVillager, null, null);
				SetStarvingHumanStatus(humansToDie - j);
			}
		}
		if (WorldManager.instance.CheckAllVillagersDead())
		{
			WorldManager.instance.VillagersStarvedAtEndOfMoon = true;
			if (WorldManager.instance.CurrentBoard.Id == "main")
			{
				CutsceneText = SokLoc.Translate("label_everyone_starved");
				yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_game_over"));
				GameCanvas.instance.SetScreen<GameOverScreen>();
				WorldManager.instance.currentAnimationRoutine = null;
			}
			else if (WorldManager.instance.CurrentBoard.Id == "island")
			{
				yield return Cutscenes.EveryoneOnIslandDead();
			}
			else if (WorldManager.instance.CurrentBoard.Id == "forest")
			{
				yield return Cutscenes.EveryoneInForestDead();
			}
			else if (WorldManager.instance.CurrentBoard.BoardOptions.IsSpiritWorld)
			{
				yield return Cutscenes.EveryoneInSpiritWorldDead(WorldManager.instance.CurrentBoard.Id);
			}
			else if (!(WorldManager.instance.CurrentBoard.Id == "cities"))
			{
				yield return Cutscenes.EveryoneOnIslandDead();
			}
		}
	}

	private static void TryCreatePoop(CardData vill)
	{
		if (WorldManager.instance.CurseIsActive(CurseType.Death))
		{
			Poop poop = WorldManager.instance.CreateCard(vill.transform.position, "human_poop", faceUp: true, checkAddToStack: false) as Poop;
			AudioManager.me.PlaySound2D(poop.PoopSound, 1f, 0.5f);
			WorldManager.instance.StackSend(poop.MyGameCard, vill.OutputDir);
		}
	}

	private static List<CardData> CardsThatNeedHappiness()
	{
		List<CardData> list = new List<CardData>();
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.IsCurrent && WorldManager.instance.GetCardRequiredHappinessCount(allCard) > 0)
			{
				list.Add(allCard.CardData);
			}
		}
		return list.OrderBy(delegate(CardData x)
		{
			if (x is Unhappiness || x is ResourceChest { HeldCardId: "unhappiness" })
			{
				return -1;
			}
			if (x is Kid)
			{
				return 0;
			}
			return (x.Id == "dog") ? 1 : 3;
		}).ToList();
	}

	public static IEnumerator HappinessWarning()
	{
		int requiredHappinessCount = WorldManager.instance.GetRequiredHappinessCount();
		CutsceneText = SokLoc.Translate("label_happiness_warning", LocParam.Create("count", requiredHappinessCount.ToString()));
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
	}

	public static IEnumerator NewVillagerBecauseOfHappiness()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "villager", faceUp: true, checkAddToStack: false);
		GameCamera.instance.TargetPositionOverride = cardData.transform.position;
		CutsceneText = SokLoc.Translate("label_new_villager_happiness");
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
	}

	public static IEnumerator NewVillagerSpawnsInDeath()
	{
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "villager", faceUp: true, checkAddToStack: false);
		GameCamera.instance.TargetPositionOverride = cardData.transform.position;
		CutsceneTitle = SokLoc.Translate("label_new_villager");
		CutsceneText = SokLoc.Translate("label_new_villager_death");
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
	}

	public static IEnumerator IndustrialRevolutionEvent()
	{
		CutsceneTitle = SokLoc.Translate("label_cutscene_industrial_revolution_title");
		CutsceneText = SokLoc.Translate("label_cutscene_industrial_revolution_text");
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_nice"));
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "event_industrial_revolution", faceUp: true, checkAddToStack: false);
		WorldManager.instance.CreateSmoke(cardData.Position);
		GameCamera.instance.TargetPositionOverride = cardData.transform.position;
		CutsceneTitle = SokLoc.Translate("label_cutscene_industrial_revolution_title");
		CutsceneText = SokLoc.Translate("label_cutscene_industrial_revolution_text_1");
		yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
	}

	private static List<GameCard> GetHappinessProviders()
	{
		List<GameCard> list = (from x in WorldManager.instance.GetCards<Happiness>()
			select x.MyGameCard).ToList();
		IEnumerable<GameCard> collection = from x in WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id)
			where x.CardData is ResourceChest { HeldCardId: "happiness" } resourceChest && resourceChest.ResourceCount > 0
			select x;
		list.AddRange(collection);
		return list.OrderBy(delegate(GameCard x)
		{
			if (x.CardData is ResourceChest)
			{
				return 1;
			}
			GameCard rootCard = x.GetRootCard();
			return (rootCard != null && rootCard.TimerRunning) ? (-1) : 0;
		}).ToList();
	}

	public static IEnumerator UseHappiness()
	{
		CutsceneText = SokLoc.Translate("label_giving_happiness");
		List<CardData> list = CardsThatNeedHappiness();
		List<GameCard> happinessProviders = GetHappinessProviders();
		List<(CardData card, int missingHappiness)> unhappyCards = new List<(CardData, int)>();
		WorldManager.instance.EndOfMonthSpeedup = 0f;
		int missingUnhappiness = 0;
		foreach (CardData needsHappiness in list)
		{
			int takenHappiness = 0;
			int requiredHappinessForCard = WorldManager.instance.GetCardRequiredHappinessCount(needsHappiness.MyGameCard);
			for (int i = 0; i < requiredHappinessForCard; i++)
			{
				for (int j = happinessProviders.Count - 1; j >= 0; j--)
				{
					GameCard happinessCard = happinessProviders[j];
					happinessCard.PushEnabled = false;
					happinessCard.SetY = false;
					happinessCard.Velocity = null;
					GameCamera.instance.TargetPositionOverride = needsHappiness.transform.position;
					GameCard originalParent = happinessCard.Parent;
					GameCard originalChild = happinessCard.Child;
					Vector3 originalPosition = happinessCard.TargetPosition;
					happinessCard.GetAllCardsInStack();
					happinessCard.RemoveFromStack();
					happinessCard.TargetPosition = needsHappiness.transform.position + new Vector3(0f, 0.1f, 0f);
					Vector3 diff;
					do
					{
						diff = happinessCard.TargetPosition - happinessCard.transform.position;
						yield return null;
					}
					while (diff.magnitude > 0.001f);
					AudioManager.me.PlaySound2D(AudioManager.me.ConsumeHappiness, Random.Range(0.8f, 1.2f), 0.1f);
					happinessCard.SetHitEffect();
					happinessCard.transform.localScale *= 0.9f;
					yield return new WaitForSeconds(CalculateWaitFromSpeedup(WorldManager.instance.EndOfMonthSpeedup));
					takenHappiness++;
					if (happinessCard.CardData is Happiness)
					{
						happinessProviders.RemoveAt(j);
						happinessCard.DestroyCard(spawnSmoke: true);
						WorldManager.instance.EndOfMonthSpeedup += 1f;
						break;
					}
					if (happinessCard.CardData is ResourceChest resourceChest)
					{
						resourceChest.ResourceCount--;
						if (resourceChest.ResourceCount <= 0)
						{
							happinessProviders.RemoveAt(j);
						}
						happinessCard.PushEnabled = true;
						happinessCard.SetY = true;
						if (originalParent != null)
						{
							happinessCard.SetParent(originalParent);
						}
						if (originalChild != null)
						{
							happinessCard.SetChild(originalChild);
						}
						happinessCard.TargetPosition = originalPosition;
						WorldManager.instance.EndOfMonthSpeedup += 1f;
						break;
					}
				}
			}
			if (needsHappiness is BaseVillager && takenHappiness < requiredHappinessForCard)
			{
				GameCamera.instance.TargetPositionOverride = needsHappiness.transform.position;
				unhappyCards.Add((needsHappiness, requiredHappinessForCard - takenHappiness));
			}
			if (needsHappiness is Unhappiness)
			{
				if (takenHappiness >= requiredHappinessForCard)
				{
					needsHappiness.MyGameCard.DestroyCard(spawnSmoke: true);
					AudioManager.me.PlaySound2D(AudioManager.me.CancelSadness, 1f, 0.5f);
				}
				else
				{
					missingUnhappiness += requiredHappinessForCard - takenHappiness;
				}
			}
		}
		if (unhappyCards.Count > 0)
		{
			WorldManager.instance.CurrentRunVariables.VillagersHappyMonthCount = 0;
			WorldManager.instance.CurrentRunVariables.VillagersUnhappyMonthCount++;
			CutsceneText = SokLoc.Translate("label_not_everyone_happy");
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
			foreach (var item in unhappyCards)
			{
				if (!(item.card is BaseVillager))
				{
					continue;
				}
				if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
				{
					if (item.card is BaseVillager baseVillager)
					{
						baseVillager.Damage(3);
					}
					GameCamera.instance.TargetCardOverride = item.card;
					yield return new WaitForSeconds(0.5f);
					continue;
				}
				float num = (float)item.missingHappiness * 2.5f;
				num += (float)missingUnhappiness / (float)unhappyCards.Count * 3f;
				num += (float)WorldManager.instance.CurrentRunVariables.VillagersUnhappyMonthCount * 10f;
				foreach (CardIdWithEquipment item2 in SpawnHelper.GetEnemiesToSpawn(SetCardBagType.Happiness_Enemy.AsList(), num))
				{
					WorldManager.instance.CreateCard(item.card.transform.position, item2, faceUp: true, checkAddToStack: false).MyGameCard.SendIt();
				}
				GameCamera.instance.TargetCardOverride = item.card;
				yield return new WaitForSeconds(0.5f);
			}
		}
		else
		{
			WorldManager.instance.CurrentRunVariables.VillagersHappyMonthCount++;
			if (WorldManager.instance.CurrentRunVariables.VillagersHappyMonthCount >= 4 && WorldManager.instance.GetCardCount<BaseVillager>() < 3)
			{
				WorldManager.instance.CurrentRunVariables.VillagersHappyMonthCount = 0;
				yield return NewVillagerBecauseOfHappiness();
				CutsceneText = "";
			}
		}
		if (WorldManager.instance.CheckAllVillagersDead())
		{
			WorldManager.instance.VillagersAngryAtEndOfMoon = true;
			if (WorldManager.instance.CurrentBoard.Id == "main")
			{
				CutsceneText = SokLoc.Translate("label_everyone_angry");
				yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_game_over"));
				GameCanvas.instance.SetScreen<GameOverScreen>();
				WorldManager.instance.currentAnimationRoutine = null;
			}
			else if (WorldManager.instance.CurrentBoard.Id == "island")
			{
				yield return Cutscenes.EveryoneOnIslandDead();
			}
			else if (WorldManager.instance.CurrentBoard.Id == "forest")
			{
				yield return Cutscenes.EveryoneInForestDead();
			}
			else if (WorldManager.instance.CurrentBoard.BoardOptions.IsSpiritWorld)
			{
				yield return Cutscenes.EveryoneInSpiritWorldDead(WorldManager.instance.CurrentBoard.Id);
			}
			else if (!(WorldManager.instance.CurrentBoard.Id == "cities"))
			{
				yield return Cutscenes.EveryoneOnIslandDead();
			}
		}
	}

	private static void SetStarvingHumanStatus(int deathCount)
	{
		CutsceneText = SokLoc.Translate("label_starving_humans", LocParam.Plural("count", deathCount));
	}

	private static bool AnyCardCanBeSold()
	{
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.IsCurrent && !allCard.IsEquipped)
			{
				MonthlyRequirementResult monthlyRequirementResult = allCard.CardData.MonthlyRequirementResult;
				if ((monthlyRequirementResult == null || !(monthlyRequirementResult.results?.Count > 0)) && allCard.CardData.GetValue() != -1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static IEnumerator MaxCardCount()
	{
		int cardCount = WorldManager.instance.GetCardCount();
		int maxCardCount = WorldManager.instance.GetMaxCardCount(WorldManager.instance.CurrentBoard);
		int num = cardCount - maxCardCount;
		if (!AnyCardCanBeSold())
		{
			num = 0;
		}
		if (num <= 0)
		{
			yield break;
		}
		CutsceneText = SokLoc.Translate("label_too_many_cards", LocParam.Plural("count", num));
		string text = SokLoc.Translate("label_sell_x_cards", LocParam.Plural("count", num));
		yield return Cutscenes.WaitForContinueClicked(text);
		WorldManager.instance.RemovingCards = true;
		while (WorldManager.instance.GetCardCount() > WorldManager.instance.GetMaxCardCount(WorldManager.instance.CurrentBoard))
		{
			GameCamera.instance.TargetPositionOverride = null;
			int pluralCount = WorldManager.instance.GetCardCount() - WorldManager.instance.GetMaxCardCount(WorldManager.instance.CurrentBoard);
			if (!AnyCardCanBeSold())
			{
				break;
			}
			CutsceneText = SokLoc.Translate("label_too_many_cards", LocParam.Plural("count", pluralCount));
			CutsceneTitle = SokLoc.Translate("label_sell_cards_to_continue");
			yield return null;
		}
		int pluralCount2 = Mathf.Max(0, WorldManager.instance.GetCardCount() - WorldManager.instance.GetMaxCardCount(WorldManager.instance.CurrentBoard));
		CutsceneText = SokLoc.Translate("label_too_many_cards", LocParam.Plural("count", pluralCount2));
		CutsceneTitle = SokLoc.Translate("label_sell_cards_to_continue");
		WorldManager.instance.RemovingCards = false;
	}

	public static IEnumerator SpecialEvents()
	{
		CutsceneTitle = "";
		CutsceneText = "";
		bool flag = CurrentMonth > 8 && CurrentMonth % 4 == 0;
		bool spawnTravellingCart = (Random.value <= 0.1f && CurrentMonth >= 8 && CurrentMonth % 2 == 1) || CurrentMonth == 19;
		bool spawnPirateBoat = WorldManager.instance.BoardMonths.IslandMonth % 7 == 0 && WorldManager.instance.CurrentBoard.BoardOptions.CanSpawnPirateBoat;
		bool spawnShaman = (WorldManager.instance.CurrentRunVariables.FinishedDemon || QuestManager.instance.QuestIsComplete("kill_demon")) && WorldManager.instance.IsSpiritDlcActive() && !WorldManager.instance.CurrentRunVariables.ShamanVisited;
		bool spawnSadEvent = WorldManager.instance.CurrentBoard.Id == "happiness" && CurrentMonth > 4 && CurrentMonth % 4 == 0;
		if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			flag = false;
			spawnPirateBoat = false;
		}
		if (!WorldManager.instance.CurrentBoard.BoardOptions.CanSpawnPirateBoat)
		{
			spawnPirateBoat = false;
		}
		if (!WorldManager.instance.CurrentBoard.BoardOptions.CanSpawnPortals)
		{
			flag = false;
		}
		if (!WorldManager.instance.CurrentBoard.BoardOptions.CanSpawnTravellingCart)
		{
			spawnTravellingCart = false;
		}
		if (!WorldManager.instance.CurrentBoard.BoardOptions.CanSpawnShaman || (WorldManager.instance.HasFoundCard("blueprint_altar") && WorldManager.instance.HasFoundCard("greed_recipe") && WorldManager.instance.HasFoundCard("happiness_recipe") && WorldManager.instance.HasFoundCard("death_recipe")))
		{
			spawnShaman = false;
		}
		if (flag)
		{
			WorldManager.instance.CurrentRunVariables.StrangePortalSpawns++;
			Vector3 randomSpawnPosition = WorldManager.instance.GetRandomSpawnPosition();
			CardData cardData;
			if (WorldManager.instance.CurrentRunVariables.StrangePortalSpawns % 4 == 0)
			{
				CutsceneText = SokLoc.Translate("label_strange_portal_appeared_strong");
				cardData = WorldManager.instance.CreateCard(randomSpawnPosition, "rare_portal", faceUp: true, checkAddToStack: false);
			}
			else
			{
				cardData = WorldManager.instance.CreateCard(randomSpawnPosition, "strange_portal", faceUp: true, checkAddToStack: false);
			}
			CutsceneTitle = SokLoc.Translate("label_strange_portal_appeared");
			if (cardData != null)
			{
				GameCamera.instance.TargetPositionOverride = cardData.transform.position;
			}
			yield return new WaitForSeconds(2f);
			GameCamera.instance.TargetPositionOverride = null;
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		}
		if (spawnPirateBoat)
		{
			WorldManager.instance.ShowContinueButton = false;
			Vector3 randomSpawnPosition2 = WorldManager.instance.GetRandomSpawnPosition();
			CardData cardData2 = WorldManager.instance.CreateCard(randomSpawnPosition2, "pirate_boat", faceUp: true, checkAddToStack: false);
			CutsceneText = SokLoc.Translate("label_pirate_boat_appeared");
			GameCamera.instance.TargetPositionOverride = cardData2.transform.position;
			yield return new WaitForSeconds(2f);
			GameCamera.instance.TargetPositionOverride = null;
			WorldManager.instance.CurrentRunVariables.PirateBoatsSpawned++;
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		}
		if (spawnTravellingCart)
		{
			WorldManager.instance.ShowContinueButton = false;
			Vector3 randomSpawnPosition3 = WorldManager.instance.GetRandomSpawnPosition();
			CardData cardData3 = WorldManager.instance.CreateCard(randomSpawnPosition3, "travelling_cart", faceUp: true, checkAddToStack: false);
			CutsceneText = SokLoc.Translate("label_travelling_cart_appeared");
			GameCamera.instance.TargetPositionOverride = cardData3.transform.position;
			yield return new WaitForSeconds(2f);
			GameCamera.instance.TargetPositionOverride = null;
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		if (spawnShaman)
		{
			CutsceneTitle = SokLoc.Translate("label_shaman_intro_title");
			WorldManager.instance.CurrentRunVariables.ShamanVisited = true;
			WorldManager.instance.ShowContinueButton = false;
			Vector3 randomPos = WorldManager.instance.GetRandomSpawnPosition();
			GameCamera.instance.TargetPositionOverride = randomPos;
			CutsceneText = SokLoc.Translate("label_shaman_intro");
			yield return new WaitForSeconds(2f);
			CardData cardData4 = WorldManager.instance.CreateCard(randomPos, "shaman", faceUp: true, checkAddToStack: false);
			AudioManager.me.PlaySound2D(AudioManager.me.ShamanSpawn, 1f, 0.2f);
			WorldManager.instance.CreateSmoke(randomPos);
			GameCamera.instance.TargetPositionOverride = cardData4.transform.position;
			CutsceneText = SokLoc.Translate("label_shaman_appeared");
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_wow"));
			if (!WorldManager.instance.CurrentRunVariables.VisitedIsland || !QuestManager.instance.AnyIslandQuestComplete())
			{
				if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
				{
					CutsceneText = SokLoc.Translate("label_shaman_intro_peaceful");
				}
				else
				{
					CutsceneText = SokLoc.Translate("label_shaman_intro_demon");
				}
				yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_nice"));
				CutsceneText = SokLoc.Translate("label_shaman_intro_island");
				yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
			}
			CutsceneText = SokLoc.Translate("label_shaman_intro_cursed");
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
			GameCamera.instance.TargetPositionOverride = null;
		}
		if (spawnSadEvent)
		{
			WorldManager.instance.ShowContinueButton = false;
			Vector3 randomSpawnPosition4 = WorldManager.instance.GetRandomSpawnPosition();
			CardData cardData5 = WorldManager.instance.CreateCard(randomSpawnPosition4, "sad_event", faceUp: true, checkAddToStack: false);
			CutsceneText = SokLoc.Translate("label_sad_event_appeared");
			GameCamera.instance.TargetPositionOverride = cardData5.transform.position;
			yield return new WaitForSeconds(2f);
			GameCamera.instance.TargetPositionOverride = null;
			yield return Cutscenes.WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
	}
}
