using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GreedCutscenes
{
	public static string Title
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

	public static string Text
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

	private static QueuedAnimation currentAnimation
	{
		set
		{
			WorldManager.instance.currentAnimation = value;
		}
	}

	private static void Stop(bool keepCameraPosition = false)
	{
		Text = "";
		Title = "";
		GameCamera.instance.TargetPositionOverride = null;
		GameCamera.instance.CameraPositionDistanceOverride = null;
		GameCamera.instance.TargetCardOverride = null;
		CutsceneScreen.instance.IsAdvisorCutscene = false;
		CutsceneScreen.instance.IsEndOfMonthCutscene = false;
		CutsceneScreen.instance.CheckAdvisorCutscene();
		if (keepCameraPosition)
		{
			GameCamera.instance.KeepCameraAtCurrentPos();
		}
		GameCanvas.instance.SetScreen<GameScreen>();
		currentAnimation = null;
	}

	public static IEnumerator FinalDemandStart(Demand demand)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Vector3 randomPos = WorldManager.instance.GetRandomSpawnPosition();
		GameCamera.instance.TargetPositionOverride = randomPos;
		yield return new WaitForSeconds(2f);
		FindOrCreateGameCard("merchant", randomPos);
		WorldManager.instance.CreateSmoke(randomPos);
		Text = SokLoc.Translate("label_final_demand_merchant");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Royal royal = FindOrCreateGameCard("royal").CardData as Royal;
		Title = "";
		GameCamera.instance.TargetPositionOverride = royal.transform.position;
		Text = SokLoc.Translate("label_final_demand_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_final_demand_text_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_final_demand_text_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_final_demand_text_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		DemandManager.instance.QuestStarted(demand);
	}

	public static IEnumerator FinalDemandEndSuccess(bool shouldStop)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Royal royal = FindOrCreateGameCard("royal").CardData as Royal;
		DragonEgg egg = FindOrCreateGameCard("dragon_egg").CardData as DragonEgg;
		Title = "";
		GameCamera.instance.TargetPositionOverride = royal.transform.position;
		Text = SokLoc.Translate("label_final_demand_end_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = "...";
		GameCamera.instance.TargetPositionOverride = egg.transform.position;
		yield return new WaitForSeconds(0.5f);
		egg.CrackedState = 1;
		AudioManager.me.PlaySound2D(egg.CrackedSound, Random.Range(1.1f, 1.3f), 0.5f);
		yield return new WaitForSeconds(0.5f);
		Text = SokLoc.Translate("label_final_demand_end_text_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = royal.transform.position;
		Text = SokLoc.Translate("label_final_demand_end_text_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		Text = "...";
		GameCamera.instance.TargetPositionOverride = egg.transform.position;
		yield return new WaitForSeconds(0.5f);
		egg.CrackedState = 2;
		AudioManager.me.PlaySound2D(egg.CrackedSound2, Random.Range(1.1f, 1.3f), 0.5f);
		yield return new WaitForSeconds(0.5f);
		Text = SokLoc.Translate("label_final_demand_end_text_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = royal.transform.position;
		AudioManager.me.PlaySound2D(DemandManager.instance.FailedDemandSound, Random.Range(1.1f, 1.3f), 0.5f);
		AngryRoyal angryRoyal = WorldManager.instance.ChangeToCard(royal.MyGameCard, "angry_royal") as AngryRoyal;
		WorldManager.instance.CreateSmoke(royal.Position);
		Text = SokLoc.Translate("label_final_demand_end_text_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = egg.transform.position;
		AudioManager.me.PlaySound2D(egg.CrackedSound2, Random.Range(1.1f, 1.3f), 0.5f);
		Text = SokLoc.Translate("label_final_demand_end_text_6");
		yield return new WaitForSeconds(1f);
		Combatable dragon = WorldManager.instance.ChangeToCard(egg.MyGameCard, "baby_dragon") as Combatable;
		WorldManager.instance.CreateSmoke(dragon.transform.position);
		AudioManager.me.PlaySound2D(dragon.PickupSound, Random.Range(1.1f, 1.3f), 0.4f);
		Text = SokLoc.Translate("label_final_demand_end_text_7");
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		dragon.MyGameCard.CardAnimations.Add(new CardAnimation_FakeMeleeAttack(dragon.MyGameCard, angryRoyal.MyGameCard));
		AudioManager.me.PlaySound2D(AudioManager.me.Crit, Random.Range(0.8f, 1f), 0.1f);
		Text = SokLoc.Translate("label_final_demand_end_text_8");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		yield return new WaitForSeconds(1f);
		AudioManager.me.PlaySound2D(AudioManager.me.Crit, Random.Range(0.8f, 1f), 0.1f);
		angryRoyal.DieInCutscene();
		Text = "";
		yield return new WaitForSeconds(1f);
		yield return FinalDemandLiftCurse(shouldStop);
		if (shouldStop)
		{
			Stop();
		}
	}

	public static IEnumerator GreedWearCrown()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		GameCard spirit = FindOrCreateGameCard("greed_spirit");
		GameCamera.instance.TargetPositionOverride = spirit.transform.position;
		Text = SokLoc.Translate("label_greed_outro_wear_crown");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		spirit.DestroyCard();
		Stop();
	}

	public static IEnumerator NewVillager()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		GameCard targetCardOverride = FindOrCreateGameCard("royal");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		Text = SokLoc.Translate("label_greed_new_villager");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_greed_new_villager_2");
		CardData targetCardOverride2 = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "villager", faceUp: true, checkAddToStack: false);
		GameCamera.instance.TargetCardOverride = targetCardOverride2;
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
	}

	public static IEnumerator FinalDemandLiftCurse(bool shouldStop)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		CardData spirit = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "greed_spirit");
		GameCamera.instance.TargetPositionOverride = spirit.transform.position;
		Text = SokLoc.Translate("label_greed_lift_curse");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCard targetCardOverride = FindOrCreateGameCard("royal_crown");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		Text = SokLoc.Translate("label_greed_lift_curse_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = WorldManager.instance.GetCard<Curse>().transform.position;
		Text = SokLoc.Translate("label_greed_lift_curse_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		if (spirit != null)
		{
			spirit.MyGameCard.DestroyCard();
		}
		yield return new WaitForSeconds(0.5f);
		if (shouldStop)
		{
			Stop();
		}
	}

	public static IEnumerator KillRoyalLiftCurse()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		CardData spirit = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "greed_spirit");
		GameCamera.instance.TargetPositionOverride = spirit.transform.position;
		Text = SokLoc.Translate("label_greed_lift_curse_kill_royal");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCard targetCardOverride = FindOrCreateGameCard("royal_crown");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		Text = SokLoc.Translate("label_greed_lift_curse_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = WorldManager.instance.GetCard<BaseVillager>().transform.position;
		Text = SokLoc.Translate("label_greed_lift_curse_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		if (spirit != null)
		{
			spirit.MyGameCard.DestroyCard();
		}
		yield return new WaitForSeconds(0.5f);
		Stop();
	}

	public static IEnumerator StartDemand(Demand demand)
	{
		Title = SokLoc.Translate("greed_quest_demand_title");
		foreach (GreedAnimationState questStartAnimationState in demand.QuestStartAnimationStates)
		{
			Title = "";
			Text = "";
			GameCard gameCard = FindOrCreateGameCard(questStartAnimationState.CameraTargetId);
			if (gameCard != null)
			{
				GameCamera.instance.TargetPositionOverride = gameCard.transform.position;
			}
			if (!string.IsNullOrEmpty(questStartAnimationState.TitleTerm))
			{
				Title = SokLoc.Translate(questStartAnimationState.TitleTerm);
			}
			if (!string.IsNullOrEmpty(questStartAnimationState.DescriptionTerm))
			{
				Text = SokLoc.Translate(questStartAnimationState.DescriptionTerm);
			}
			yield return WaitForContinueClicked(SokLoc.Translate(questStartAnimationState.ContinueTerm));
		}
		Text = DemandManager.instance.GetDemandStartDescription(demand);
		GameCard royal = FindOrCreateGameCard("royal");
		GameCamera.instance.TargetCardOverride = royal;
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		if (demand.BlueprintIds.Any((string id) => !string.IsNullOrEmpty(id) && !WorldManager.instance.HasFoundCard(id)))
		{
			Text = SokLoc.Translate("greed_quest_demand_description_not_found");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			foreach (string blueprintId in demand.BlueprintIds)
			{
				CardData cardData = WorldManager.instance.CreateCard(royal.transform.position, blueprintId);
				GameCamera.instance.TargetCardOverride = cardData;
				cardData.MyGameCard.SendIt();
			}
		}
		DemandManager.instance.QuestStarted(demand);
	}

	public static IEnumerator FinishDemandSuccess(DemandEvent demandEvent)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("greed_quest_demand_title");
		Text = SokLoc.Translate("label_demand_complete_start");
		GameCamera.instance.TargetCardOverride = FindOrCreateGameCard("royal");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		foreach (GreedAnimationState questSuccessAnimationState in demandEvent.Demand.QuestSuccessAnimationStates)
		{
			GameCard gameCard = FindOrCreateGameCard(questSuccessAnimationState.CameraTargetId);
			if (gameCard != null)
			{
				GameCamera.instance.TargetPositionOverride = gameCard.transform.position;
			}
			Title = "";
			Text = "";
			if (!string.IsNullOrEmpty(questSuccessAnimationState.TitleTerm))
			{
				Title = SokLoc.Translate(questSuccessAnimationState.TitleTerm);
			}
			if (!string.IsNullOrEmpty(questSuccessAnimationState.DescriptionTerm))
			{
				Text = SokLoc.Translate(questSuccessAnimationState.DescriptionTerm);
			}
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		Text = DemandManager.instance.GetRandomSuccessDescription(demandEvent.Demand);
		GameCard gameCard2 = FindOrCreateGameCard("royal");
		if (gameCard2 != null)
		{
			GameCamera.instance.TargetPositionOverride = gameCard2.transform.position;
		}
		GameCamera.instance.CameraPositionDistanceOverride = null;
		if (demandEvent.Demand.ShouldDestroyOnComplete)
		{
			Text = "";
			float speedup2 = 1f;
			for (int i = 0; i < demandEvent.Demand.Amount - demandEvent.AmountGiven; i++)
			{
				CardData card = WorldManager.instance.GetCard(demandEvent.Demand.CardToGet);
				if (card != null)
				{
					GameCamera.instance.TargetPositionOverride = card.Position;
					yield return new WaitForSeconds(0.2f * speedup2);
					WorldManager.instance.CreateSmoke(card.Position);
					card.MyGameCard.DestroyCard();
					yield return new WaitForSeconds(0.3f * speedup2);
					speedup2 -= 0.1f;
					speedup2 = Mathf.Max(0.4f, speedup2);
				}
			}
			GameCamera.instance.TargetPositionOverride = null;
			yield return new WaitForSeconds(0.5f);
			Text = SokLoc.Translate("label_demand_collected");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		else
		{
			CardData card2 = WorldManager.instance.GetCard(demandEvent.Demand.CardToGet);
			if (card2 != null)
			{
				GameCamera.instance.TargetCardOverride = card2;
			}
			Text = SokLoc.Translate("label_demand_collected_2");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		DemandManager.instance.DemandFinishedSuccess(demandEvent.Demand);
	}

	public static IEnumerator FinishDemandSuccessPreMoon(Demand demand)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("greed_quest_demand_title");
		Text = "";
		foreach (GreedAnimationState questSuccessAnimationState in demand.QuestSuccessAnimationStates)
		{
			GameCard gameCard = FindOrCreateGameCard(questSuccessAnimationState.CameraTargetId);
			if (gameCard != null)
			{
				GameCamera.instance.TargetPositionOverride = gameCard.transform.position;
			}
			Title = "";
			Text = "";
			if (!string.IsNullOrEmpty(questSuccessAnimationState.TitleTerm))
			{
				Title = SokLoc.Translate(questSuccessAnimationState.TitleTerm);
			}
			if (!string.IsNullOrEmpty(questSuccessAnimationState.DescriptionTerm))
			{
				Text = SokLoc.Translate(questSuccessAnimationState.DescriptionTerm);
			}
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		Title = SokLoc.Translate("greed_quest_demand_title");
		Text = DemandManager.instance.GetRandomSuccessDescription(demand);
		GameCard gameCard2 = FindOrCreateGameCard("royal");
		if (gameCard2 != null)
		{
			GameCamera.instance.TargetPositionOverride = gameCard2.transform.position;
		}
		GameCamera.instance.CameraPositionDistanceOverride = null;
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		DemandManager.instance.DemandFinishedSuccess(demand);
		Stop();
	}

	public static IEnumerator FinishDemandFailed(Demand demand)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("greed_quest_demand_title");
		Text = SokLoc.Translate("label_demand_complete_start");
		GameCamera.instance.TargetCardOverride = FindOrCreateGameCard("royal");
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		int amountToTake = WorldManager.instance.GetCardCount((CardData x) => x.Id == demand.CardToGet);
		if (demand.ShouldDestroyOnComplete && amountToTake > 0)
		{
			Text = "";
			float speedup2 = 1f;
			for (int i = 0; i < amountToTake; i++)
			{
				CardData card = WorldManager.instance.GetCard(demand.CardToGet);
				GameCamera.instance.TargetPositionOverride = card.Position;
				yield return new WaitForSeconds(0.2f * speedup2);
				WorldManager.instance.CreateSmoke(card.Position);
				card.MyGameCard.DestroyCard();
				yield return new WaitForSeconds(0.3f * speedup2);
				speedup2 -= 0.1f;
				speedup2 = Mathf.Max(0.4f, speedup2);
			}
			GameCamera.instance.TargetPositionOverride = null;
			yield return new WaitForSeconds(0.5f);
		}
		foreach (GreedAnimationState questFailedAnimationState in demand.QuestFailedAnimationStates)
		{
			GameCard gameCard = FindOrCreateGameCard(questFailedAnimationState.CameraTargetId);
			if (gameCard != null)
			{
				GameCamera.instance.TargetPositionOverride = gameCard.transform.position;
			}
			Title = "";
			Text = "";
			if (!string.IsNullOrEmpty(questFailedAnimationState.TitleTerm))
			{
				Title = SokLoc.Translate(questFailedAnimationState.TitleTerm);
			}
			if (!string.IsNullOrEmpty(questFailedAnimationState.DescriptionTerm))
			{
				Text = SokLoc.Translate(questFailedAnimationState.DescriptionTerm);
			}
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		Title = SokLoc.Translate("greed_quest_demand_title");
		Text = DemandManager.instance.GetRandomFailedDescription(demand);
		GameCard gameCard2 = FindOrCreateGameCard("royal");
		if (gameCard2 != null)
		{
			GameCamera.instance.TargetCardOverride = gameCard2;
		}
		GameCamera.instance.CameraPositionDistanceOverride = null;
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		AudioManager.me.PlaySound2D(DemandManager.instance.FailedDemandSound, 0.9f, 0.3f);
		if (WorldManager.instance.CurrentRunVariables.PreviousDemandEvents.Count == 0)
		{
			Title = "";
			Text = SokLoc.Translate("label_greed_demand_failed_first_time");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			yield break;
		}
		Title = "";
		if (WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			Text = SokLoc.Translate("label_greed_demand_failed_fight_peaceful");
			yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
			float speedup2 = 1f;
			int i = 0;
			int coinsToTake = 3 * DemandManager.instance.GetTimesDemandFailed();
			for (int j = 0; j < coinsToTake; j++)
			{
				CardData card = WorldManager.instance.GetCard("gold");
				if (card != null)
				{
					GameCamera.instance.TargetPositionOverride = card.Position;
					yield return new WaitForSeconds(0.2f * speedup2);
					WorldManager.instance.CreateSmoke(card.Position);
					card.MyGameCard.DestroyCard();
					i++;
					yield return new WaitForSeconds(0.3f * speedup2);
				}
				else
				{
					foreach (Chest chest in WorldManager.instance.GetCards<Chest>())
					{
						if (coinsToTake == i)
						{
							break;
						}
						if (chest != null && chest.CoinCount > 0)
						{
							int b = coinsToTake - i;
							int take = Mathf.Min(chest.CoinCount, b);
							if (take > 0)
							{
								GameCamera.instance.TargetPositionOverride = card.Position;
								yield return new WaitForSeconds(0.2f * speedup2);
								WorldManager.instance.CreateSmoke(chest.Position);
								chest.CoinCount -= take;
								i += take;
								yield return new WaitForSeconds(0.3f * speedup2);
							}
						}
					}
				}
				if (coinsToTake != i)
				{
					speedup2 -= 0.1f;
					speedup2 = Mathf.Max(0.4f, speedup2);
					continue;
				}
				break;
			}
		}
		else
		{
			Text = SokLoc.Translate("label_greed_demand_failed_fight");
			List<Combatable> source = DemandManager.instance.SpawnEnemies();
			GameCamera.instance.TargetCardOverride = source.FirstOrDefault();
			yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		}
	}

	public static IEnumerator WaitForAnswer(params string[] answers)
	{
		CutsceneScreen.instance.CreateMultipleOptions(answers);
		WorldManager.instance.ContinueClicked = false;
		while (!WorldManager.instance.ContinueClicked)
		{
			yield return null;
			if (!(GameCanvas.instance.CurrentScreen is CutsceneScreen))
			{
				GameCanvas.instance.SetScreen<CutsceneScreen>();
			}
		}
		CutsceneScreen.instance.ClearMultipleOptions();
		WorldManager.instance.ShowContinueButton = false;
	}

	public static IEnumerator WaitForContinueClicked(string text)
	{
		WorldManager.instance.ContinueClicked = false;
		WorldManager.instance.ContinueButtonText = text;
		WorldManager.instance.ShowContinueButton = true;
		while (!WorldManager.instance.ContinueClicked)
		{
			yield return null;
			if (!(GameCanvas.instance.CurrentScreen is CutsceneScreen))
			{
				GameCanvas.instance.SetScreen<CutsceneScreen>();
			}
		}
		WorldManager.instance.ShowContinueButton = false;
	}

	public static IEnumerator TryAttackRoyal(Royal royal, int tries)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = royal.transform.position;
		Title = SokLoc.Translate("label_try_attack_royal_title");
		if (tries < 4)
		{
			Text = SokLoc.Translate("label_try_attack_royal_description");
		}
		if (tries >= 4 && tries < 8)
		{
			Text = SokLoc.Translate("label_try_attack_royal_description_4");
		}
		if (tries == 8)
		{
			Text = SokLoc.Translate("label_try_attack_royal_description_8");
		}
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Stop();
	}

	public static GameCard FindOrCreateGameCard(string cardId, Vector3? position = null)
	{
		CardData cardData = WorldManager.instance.GetCard(cardId);
		if (cardData == null)
		{
			cardData = ((!position.HasValue) ? WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), cardId, faceUp: true, checkAddToStack: false) : WorldManager.instance.CreateCard(position.Value, cardId, faceUp: true, checkAddToStack: false));
		}
		if (cardData == null)
		{
			return null;
		}
		return cardData.MyGameCard;
	}
}
