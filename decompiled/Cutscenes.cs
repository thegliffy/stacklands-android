using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Cutscenes
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

	public static IEnumerator SpawnTentacles()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		Text = SokLoc.Translate("label_awakened_something");
		List<Tentacle> tentacles = new List<Tentacle>();
		for (int i = 0; i < 5; i++)
		{
			Tentacle tentacle = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "tentacle", faceUp: true, checkAddToStack: false) as Tentacle;
			GameCamera.instance.TargetPositionOverride = tentacle.transform.position;
			tentacles.Add(tentacle);
			yield return new WaitForSeconds(1f);
		}
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		Stop();
	}

	public static IEnumerator SpawnKraken()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		Text = SokLoc.Translate("label_awakened_the_kraken");
		CardData cardData = WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), "kraken", faceUp: false, checkAddToStack: false);
		GameCamera.instance.TargetPositionOverride = cardData.transform.position;
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		Stop();
	}

	public static IEnumerator JustUnlockedPack(BoosterpackData justUnlockedPack)
	{
		BuyBoosterBox sellbox = UnityEngine.Object.FindObjectsOfType<BuyBoosterBox>().FirstOrDefault((BuyBoosterBox x) => x.BoosterId == justUnlockedPack.BoosterId);
		if (sellbox == null)
		{
			Debug.LogError("No sellbox found for booster pack " + justUnlockedPack.BoosterId);
			yield break;
		}
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		if (justUnlockedPack != null)
		{
			Title = SokLoc.Translate("label_new_pack_unlocked");
			Text = SokLoc.Translate("label_pack_now_available", LocParam.Create("pack", justUnlockedPack.Name));
			yield return new WaitForSeconds(0.5f);
			GameCamera.instance.TargetPositionOverride = sellbox.transform.position;
			yield return WaitForContinueClicked(SokLoc.Translate("label_continue"));
			if (justUnlockedPack.BoosterId == "structures")
			{
				QuestManager.instance.SpecialActionComplete("unlocked_all_packs");
			}
			if (justUnlockedPack.BoosterId == "island_locations")
			{
				QuestManager.instance.SpecialActionComplete("unlocked_all_island_packs");
			}
			if (justUnlockedPack.BoosterId == "cities_ideas_2")
			{
				QuestManager.instance.SpecialActionComplete("unlocked_all_cities_packs");
			}
			QuestManager.instance.SpecialActionComplete("unlocked_" + justUnlockedPack.BoosterId + "_pack");
			if (justUnlockedPack.BoosterId == "basic")
			{
				GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("main"));
			}
		}
		Stop();
	}

	public static IEnumerator EveryoneInForestDead()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("label_everyone_in_forest_dead");
		Text = SokLoc.Translate("label_everyone_in_forest_dead_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_return_to_mainland"));
		WorldManager.instance.GoToBoard(WorldManager.instance.GetBoardWithId("main"), delegate
		{
			RemoveEnemiesFromBoard("forest");
			Stop();
			WorldManager.instance.currentAnimationRoutine = null;
		});
	}

	public static IEnumerator EveryoneOnIslandDead()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("label_everyone_on_island_dead");
		Text = SokLoc.Translate("label_everyone_on_island_dead_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_return_to_mainland"));
		WorldManager.instance.GoToBoard(WorldManager.instance.GetBoardWithId("main"), delegate
		{
			RemoveEnemiesFromBoard("island");
			Stop();
			WorldManager.instance.currentAnimationRoutine = null;
		});
	}

	public static IEnumerator EveryoneInSpiritWorldDead(string boardId)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("label_everyone_spirit_world_dead");
		Text = SokLoc.Translate("label_everyone_spirit_world_dead_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Title = SokLoc.Translate("label_everyone_spirit_world_dead");
		Text = SokLoc.Translate("label_everyone_spirit_world_dead_text_retry");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameBoard currentBoard = WorldManager.instance.GetBoardWithId(boardId);
		GameBoard boardWithId = WorldManager.instance.GetBoardWithId(WorldManager.instance.CurrentRunVariables.PreviouseBoard);
		WorldManager.instance.GoToBoard(boardWithId, delegate
		{
			WorldManager.instance.RemoveAllCardsFromBoard(boardId);
			WorldManager.instance.ResetBoughtBoostersOnLocation(currentBoard.Location);
			if (currentBoard.Id == "greed")
			{
				DemandManager.instance.ResetDemands();
				WorldManager.instance.BoardMonths.GreedMonth = 1;
			}
			if (currentBoard.Id == "happiness")
			{
				WorldManager.instance.BoardMonths.HappinessMonth = 1;
				WorldManager.instance.CurrentRunVariables.VillagersUnhappyMonthCount = 0;
				WorldManager.instance.CurrentRunVariables.VillagersHappyMonthCount = 0;
			}
			if (currentBoard.Id == "death")
			{
				WorldManager.instance.BoardMonths.DeathMonth = 1;
			}
			Stop();
			WorldManager.instance.currentAnimationRoutine = null;
		}, "spirit");
	}

	private static void RemoveEnemiesFromBoard(string board)
	{
		List<GameCard> list = new List<GameCard>();
		foreach (GameCard allCard in WorldManager.instance.AllCards)
		{
			if (allCard.MyBoard.Id == board && (allCard.CardData is Enemy || allCard.CardData is Mob { IsAggressive: not false }))
			{
				list.Add(allCard);
			}
		}
		foreach (GameCard item in list)
		{
			item.DestroyCard();
		}
	}

	public static IEnumerator MommaCrab(CardData mommaCrab)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = mommaCrab.transform.position;
		Title = SokLoc.Translate("label_momma_crab_appeared_title");
		Text = SokLoc.Translate("label_momma_crab_appeared_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		Stop();
	}

	public static IEnumerator IslandIntroPack()
	{
		Boosterpack boosterpack = WorldManager.instance.CreateBoosterpack(WorldManager.instance.GetRandomSpawnPosition(), "island_intro");
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = boosterpack.transform.position;
		Title = SokLoc.Translate("label_island_intro_pack_title");
		Text = SokLoc.Translate("label_island_intro_pack_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		WorldManager.instance.CurrentSave.GotIslandIntroPack = true;
		Stop();
	}

	public static IEnumerator IslandIntro()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("island"));
		Title = "";
		SokLoc.Translate("label_island_intro_title");
		Text = SokLoc.Translate("label_island_intro_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_island_intro_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		CardData card = WorldManager.instance.GetCard("rowboat");
		if (card != null)
		{
			GameCamera.instance.TargetPositionOverride = card.transform.position;
		}
		Text = SokLoc.Translate("label_island_intro_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_island_intro_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = WorldManager.instance.GetCard<BaseVillager>().transform.position;
		Text = SokLoc.Translate("label_island_intro_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = null;
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("island"));
		Text = SokLoc.Translate("label_island_intro_6");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Stop();
	}

	public static IEnumerator ForestIntro()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		Title = SokLoc.Translate("label_forest_intro_title");
		Text = SokLoc.Translate("label_forest_intro_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_forest_intro_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		WickedWitch witch = FindOrCreateWitch();
		witch.IsOldLady = true;
		WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
		GameCamera.instance.TargetPositionOverride = witch.transform.position;
		Text = SokLoc.Translate("label_forest_intro_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_forest_intro_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
		WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
		GameCamera.instance.Screenshake = 0.3f;
		witch.IsOldLady = false;
		Text = SokLoc.Translate("label_forest_intro_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_forest_intro_6");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
		witch.MyGameCard.DestroyCard();
		AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
		GameCamera.instance.TargetPositionOverride = null;
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		ForestCombatManager.instance.PrepareWave();
		Text = SokLoc.Translate("label_forest_fight");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Stop();
		ForestCombatManager.instance.StartWave();
	}

	public static IEnumerator GreedIntro()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		Spirit spirit = FindOrCreateSpirit(CurseType.Greed);
		GameCamera.instance.TargetCardOverride = spirit;
		Text = SokLoc.Translate("label_greed_intro_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Royal targetCardOverride = FindOrCreateGameCard("royal", WorldManager.instance.MiddleOfBoard()).CardData as Royal;
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		Text = SokLoc.Translate("label_greed_intro_2");
		AudioManager.me.PlaySound2D(DemandManager.instance.StartDemandSound, 1f, 0.4f);
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_greed_intro_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_greed_intro_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetCardOverride = spirit;
		Text = SokLoc.Translate("label_greed_intro_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("greed"));
		spirit.MyGameCard.DestroyCard();
		yield return new WaitForSeconds(0.5f);
		WorldManager.instance.CurrentRunVariables.VisitedGreed = true;
		Stop();
	}

	public static IEnumerator HappinessIntro()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("happiness"));
		Title = "";
		Spirit spirit = FindOrCreateSpirit(CurseType.Happiness);
		GameCamera.instance.TargetCardOverride = spirit;
		Text = SokLoc.Translate("label_happiness_intro_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Happiness card = WorldManager.instance.GetCard<Happiness>();
		if (card != null)
		{
			GameCamera.instance.TargetPositionOverride = card.transform.position;
		}
		Text = SokLoc.Translate("label_happiness_intro_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_happiness_intro_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetCardOverride = WorldManager.instance.GetCard<BaseVillager>();
		Text = SokLoc.Translate("label_happiness_intro_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetCardOverride = spirit;
		Text = SokLoc.Translate("label_happiness_intro_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		spirit.MyGameCard.DestroyCard();
		yield return new WaitForSeconds(0.5f);
		WorldManager.instance.CurrentRunVariables.VisitedHappiness = true;
		Stop();
	}

	public static IEnumerator DeathIntro()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("death"));
		Title = "";
		SokLoc.Translate("label_death_intro_title");
		Spirit spirit = FindOrCreateSpirit(CurseType.Death);
		GameCamera.instance.TargetCardOverride = spirit;
		Text = SokLoc.Translate("label_death_intro_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_death_intro_2");
		GameCamera.instance.TargetCardOverride = WorldManager.instance.GetCard<BaseVillager>();
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_death_intro_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_death_intro_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetCardOverride = spirit;
		Text = SokLoc.Translate("label_death_intro_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		spirit.MyGameCard.DestroyCard();
		yield return new WaitForSeconds(0.5f);
		WorldManager.instance.CurrentRunVariables.VisitedDeath = true;
		Stop();
	}

	public static IEnumerator ForestResumeIntro()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		Title = SokLoc.Translate("label_forest_resume_title");
		if (WorldManager.instance.CurrentRunVariables.ForestWave <= ForestCombatManager.instance.WickedWitchWave)
		{
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
			WickedWitch witch = FindOrCreateWitch();
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			FocusCameraOnWitchAndVillagers();
			Text = SokLoc.Translate("label_forest_resume");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			ForestCombatManager.instance.PrepareWave();
			Text = SokLoc.Translate("label_forest_fight");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			witch.MyGameCard.DestroyCard();
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
		}
		else
		{
			ForestCombatManager.instance.PrepareWave();
			Text = SokLoc.Translate("label_forest_fight");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		GameCamera.instance.TargetPositionOverride = null;
		ForestCombatManager.instance.StartWave();
		Stop();
	}

	public static IEnumerator ForestWaveEnd()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		Title = SokLoc.Translate("label_forest_wave_title");
		AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
		WickedWitch witch = FindOrCreateWitch();
		WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
		FocusCameraOnWitchAndVillagers();
		Text = SokLoc.Translate("label_forest_wave_end");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_forest_wave_end2");
		yield return WaitForAnswer(SokLoc.Translate("label_forest_wave_end_next_wave", LocParam.Create("wave", WorldManager.instance.CurrentRunVariables.ForestWave.ToString())), SokLoc.Translate("label_forest_wave_end_leave"));
		if (WorldManager.instance.ContinueButtonIndex == 0)
		{
			ForestCombatManager.instance.PrepareWave();
			Text = SokLoc.Translate("label_forest_fight");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			witch.MyGameCard.DestroyCard();
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
			GameCamera.instance.TargetPositionOverride = null;
			ForestCombatManager.instance.StartWave();
		}
		else
		{
			Text = SokLoc.Translate("label_forest_leave");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			witch.MyGameCard.DestroyCard();
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
			GameCamera.instance.TargetPositionOverride = null;
			ForestCombatManager.instance.LeaveForest();
		}
		Stop();
	}

	public static IEnumerator ForestLastWaveEnd()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		Title = SokLoc.Translate("label_forest_wave_title");
		AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
		WickedWitch witch = FindOrCreateWitch();
		WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
		FocusCameraOnWitchAndVillagers();
		Text = SokLoc.Translate("label_forest_wave_10_end");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		GameCamera.instance.TargetPositionOverride = WorldManager.instance.GetCard<BaseVillager>().transform.position;
		Text = SokLoc.Translate("label_forest_wave_10_end2");
		yield return WaitForAnswer(SokLoc.Translate("label_forest_wave_end_wave_10"), SokLoc.Translate("label_forest_wave_end_leave"));
		if (WorldManager.instance.ContinueButtonIndex == 0)
		{
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			witch.MyGameCard.DestroyCard();
			ForestCombatManager.instance.PrepareWave();
			Text = SokLoc.Translate("label_forest_fight_wave_10");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
			GameCamera.instance.TargetPositionOverride = null;
			ForestCombatManager.instance.StartWave();
		}
		else
		{
			Text = SokLoc.Translate("label_forest_leave");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			witch.MyGameCard.DestroyCard();
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
			GameCamera.instance.TargetPositionOverride = null;
			ForestCombatManager.instance.LeaveForest();
		}
		Stop();
	}

	public static IEnumerator ForestEndlessWaveEnd()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		Title = SokLoc.Translate("label_forest_wave_title");
		Text = SokLoc.Translate("label_forest_wave_endless_end");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		if (WorldManager.instance.CurrentRunVariables.ForestWave == ForestCombatManager.instance.WickedWitchWave + 1)
		{
			Text = SokLoc.Translate("label_forest_wave_endless_end2");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		GameCamera.instance.TargetPositionOverride = WorldManager.instance.GetCard<BaseVillager>().transform.position;
		Text = SokLoc.Translate("label_forest_wave_endless_end3");
		yield return WaitForAnswer(SokLoc.Translate("label_forest_wave_end_next_wave", LocParam.Create("wave", WorldManager.instance.CurrentRunVariables.ForestWave.ToString())), SokLoc.Translate("label_forest_wave_end_leave"));
		if (WorldManager.instance.ContinueButtonIndex == 0)
		{
			ForestCombatManager.instance.PrepareWave();
			Text = SokLoc.Translate("label_forest_fight");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			GameCamera.instance.TargetPositionOverride = null;
			ForestCombatManager.instance.StartWave();
		}
		else
		{
			Text = SokLoc.Translate("label_forest_leave");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			GameCamera.instance.TargetPositionOverride = null;
			ForestCombatManager.instance.LeaveForest();
		}
		Stop();
	}

	public static IEnumerator ForestWaveLost()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.CenterOnBoard(WorldManager.instance.GetBoardWithId("forest"));
		Title = SokLoc.Translate("label_forest_lost_title");
		if (WorldManager.instance.CurrentRunVariables.ForestWave <= ForestCombatManager.instance.WickedWitchWave)
		{
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
			WickedWitch witch = FindOrCreateWitch();
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			FocusCameraOnWitchAndVillagers();
			Text = SokLoc.Translate("label_forest_lost");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			Text = SokLoc.Translate("label_forest_lost_2");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
			WorldManager.instance.CreateSmoke(witch.MyGameCard.transform.position);
			witch.MyGameCard.DestroyCard();
			AudioManager.me.PlaySound2D(ForestCombatManager.instance.WitchSounds, UnityEngine.Random.Range(1.1f, 1.3f), 0.5f);
		}
		else
		{
			Text = SokLoc.Translate("label_forest_lost_2");
			yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		}
		GameCamera.instance.TargetPositionOverride = null;
		ForestCombatManager.instance.ForestWaveLost();
		Stop();
	}

	public static IEnumerator BossFight(Temple temple, CardData goblet)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = temple.transform.position;
		Title = SokLoc.Translate("label_goblet_brought_to_temple");
		Text = "";
		if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			string text = SokLoc.Translate("label_start_the_ritual");
			string text2 = SokLoc.Translate("label_cancel_the_ritual");
			yield return WaitForAnswer(text, text2);
		}
		else
		{
			string text = SokLoc.Translate("label_well_done");
			yield return WaitForAnswer(text);
		}
		Title = "";
		yield return new WaitForSeconds(0.5f);
		if (WorldManager.instance.ContinueButtonIndex == 0)
		{
			temple.MyGameCard.DestroyCard(spawnSmoke: true);
			goblet.MyGameCard.DestroyCard(spawnSmoke: true);
			if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
			{
				WorldManager.instance.CreateCard(temple.transform.position, "demon", faceUp: true, checkAddToStack: false);
			}
			else
			{
				WorldManager.instance.CurrentRunVariables.FinishedDemon = true;
			}
		}
		else
		{
			goblet.MyGameCard.RemoveFromStack();
		}
		Stop();
	}

	public static IEnumerator BossFight2(Cathedral cathedral, CardData goblet)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = cathedral.transform.position;
		Title = SokLoc.Translate("label_relic_brought_to_cathedral");
		Text = "";
		if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			string text = SokLoc.Translate("label_start_the_ritual");
			string text2 = SokLoc.Translate("label_cancel_the_ritual");
			yield return WaitForAnswer(text, text2);
		}
		else
		{
			string text = SokLoc.Translate("label_well_done");
			yield return WaitForAnswer(text);
		}
		Title = "";
		yield return new WaitForSeconds(0.5f);
		if (WorldManager.instance.ContinueButtonIndex == 0)
		{
			cathedral.MyGameCard.DestroyCard(spawnSmoke: true);
			goblet.MyGameCard.DestroyCard(spawnSmoke: true);
			if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
			{
				WorldManager.instance.CreateCard(cathedral.transform.position, "demon_lord", faceUp: true, checkAddToStack: false);
			}
			else
			{
				WorldManager.instance.CurrentRunVariables.FinishedDemonLord = true;
			}
		}
		else
		{
			goblet.MyGameCard.RemoveFromStack();
		}
		Stop();
	}

	public static IEnumerator BossFightComplete(Demon demon)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = demon.transform.position;
		Title = SokLoc.Translate("label_you_killed_the_demon");
		Text = "";
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		yield return new WaitForSeconds(1f);
		demon.MyGameCard.DestroyCard(spawnSmoke: true);
		if (!WorldManager.instance.CurrentSave.GotIslandIntroPack)
		{
			WorldManager.instance.QueueCutscene(IslandIntroPack());
		}
		Stop();
	}

	public static IEnumerator BossFight2Complete(Demon demon)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = demon.transform.position;
		Title = SokLoc.Translate("label_you_killed_the_demon_lord");
		Text = "";
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		yield return new WaitForSeconds(1f);
		demon.MyGameCard.DestroyCard(spawnSmoke: true);
		yield return new WaitForSeconds(1f);
		Title = SokLoc.Translate("label_completed_main_quest");
		Text = SokLoc.Translate("label_keep_playing");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
	}

	public static IEnumerator Wish1(WishingWell well)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = well.transform.position;
		Title = SokLoc.Translate("card_wishing_well_name");
		Text = SokLoc.Translate("card_wishing_well_wish_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
	}

	public static IEnumerator Wish2(WishingWell well)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = well.transform.position;
		Title = SokLoc.Translate("card_wishing_well_name");
		Text = SokLoc.Translate("card_wishing_well_wish_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
	}

	public static IEnumerator Wish5(WishingWell well)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = well.transform.position;
		Title = SokLoc.Translate("card_wishing_well_name");
		Text = SokLoc.Translate("card_wishing_well_wish_5");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
	}

	public static IEnumerator Wish10(WishingWell well)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = well.transform.position;
		Title = SokLoc.Translate("card_wishing_well_name");
		Text = SokLoc.Translate("card_wishing_well_wish_10");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
	}

	public static IEnumerator Wish20(WishingWell well)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = well.transform.position;
		Title = SokLoc.Translate("card_wishing_well_name");
		Text = SokLoc.Translate("card_wishing_well_wish_20");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
	}

	public static IEnumerator Wish50(WishingWell well)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = well.transform.position;
		Title = SokLoc.Translate("card_wishing_well_name");
		Text = SokLoc.Translate("card_wishing_well_wish_50");
		yield return WaitForContinueClicked(SokLoc.Translate("label_nice"));
		Stop();
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

	public static IEnumerator RunScriptableCutscene(ScriptableCutscene scriptableCutscene)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		CutsceneScreen.instance.IsAdvisorCutscene = scriptableCutscene.IsAdvisorCutscene;
		if (scriptableCutscene.IsAdvisorCutscene)
		{
			if (scriptableCutscene.AdvisorWarning)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.AdvisorWarning, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
			else
			{
				AudioManager.me.PlaySound2D(AudioManager.me.AdvisorAppears, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
		}
		for (int i = 0; i < scriptableCutscene.CutsceneSteps.Count; i++)
		{
			CutsceneStep cutsceneStep = scriptableCutscene.CutsceneSteps[i];
			cutsceneStep.StepIndex = i;
			yield return cutsceneStep.Process();
		}
		Stop();
	}

	public static IEnumerator RunScriptableCutsceneInCutscene(ScriptableCutscene scriptableCutscene)
	{
		CutsceneScreen.instance.IsAdvisorCutscene = scriptableCutscene.IsAdvisorCutscene;
		if (scriptableCutscene.IsAdvisorCutscene)
		{
			if (scriptableCutscene.AdvisorWarning)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.AdvisorWarning, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
			else
			{
				AudioManager.me.PlaySound2D(AudioManager.me.AdvisorAppears, UnityEngine.Random.Range(0.9f, 1.1f), 0.3f);
			}
		}
		foreach (CutsceneStep cutsceneStep in scriptableCutscene.CutsceneSteps)
		{
			yield return cutsceneStep.Process();
		}
	}

	public static WickedWitch FindOrCreateWitch()
	{
		CardData cardData = WorldManager.instance.GetAllCardsOnBoard("forest").FirstOrDefault((GameCard x) => x.CardData.Id == "wicked_witch")?.CardData;
		if (cardData == null)
		{
			cardData = WorldManager.instance.CreateCard(ForestCombatManager.GetWitchPosition(), "wicked_witch", faceUp: true, checkAddToStack: false);
		}
		return cardData as WickedWitch;
	}

	public static Spirit FindOrCreateSpirit(CurseType curse, Vector3? spawnPos = null)
	{
		string cardId = "";
		switch (curse)
		{
		case CurseType.Happiness:
			cardId = "happiness_spirit";
			break;
		case CurseType.Greed:
			cardId = "greed_spirit";
			break;
		case CurseType.Death:
			cardId = "death_spirit";
			break;
		}
		CardData cardData = WorldManager.instance.GetCard(cardId);
		if (cardData == null)
		{
			Vector3 position = (spawnPos.HasValue ? spawnPos.Value : WorldManager.instance.MiddleOfBoard());
			cardData = WorldManager.instance.CreateCard(position, cardId, faceUp: true, checkAddToStack: false);
			cardData.MyGameCard.SendIt();
		}
		return cardData as Spirit;
	}

	public static GameCard FindOrCreateGameCard(string cardId, Vector3 position)
	{
		CardData cardData = WorldManager.instance.GetCard(cardId);
		if (cardData == null)
		{
			cardData = WorldManager.instance.CreateCard(position, cardId);
		}
		return cardData?.MyGameCard;
	}

	private static void FocusCameraOnWitchAndVillagers()
	{
		List<BaseVillager> cards = WorldManager.instance.GetCards<BaseVillager>();
		WickedWitch wickedWitch = FindOrCreateWitch();
		if (cards.Count == 0)
		{
			GameCamera.instance.TargetPositionOverride = wickedWitch.transform.position;
			return;
		}
		Vector3 value = new Vector3(wickedWitch.transform.position.x, wickedWitch.transform.position.y, Mathf.Lerp(wickedWitch.transform.position.z, cards[0].transform.position.z, 0.5f));
		GameCamera.instance.TargetPositionOverride = value;
	}

	public static IEnumerator SpiritOutro(CurseType curseType)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		if (curseType == CurseType.Happiness)
		{
			Text = SokLoc.Translate("label_spirit_outro_0_happiness");
		}
		if (curseType == CurseType.Greed)
		{
			Text = SokLoc.Translate("label_spirit_outro_0_greed");
		}
		if (curseType == CurseType.Death)
		{
			Text = SokLoc.Translate("label_spirit_outro_0_death");
		}
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_spirit_outro_1");
		Spirit targetCardOverride = FindOrCreateSpirit(curseType);
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_spirit_outro_2");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_spirit_outro_3");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = SokLoc.Translate("label_spirit_outro_4");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Stop();
	}

	public static IEnumerator ReturnToBoardCutscene()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Text = SokLoc.Translate("label_happiness_return_0");
		yield return WaitForAnswer(SokLoc.Translate("label_spirit_outro_choice_0"), SokLoc.Translate("label_spirit_outro_choice_1"));
		if (WorldManager.instance.ContinueButtonIndex == 1)
		{
			WorldManager.instance.GoToBoard(WorldManager.instance.GetBoardWithId("main"));
		}
		Stop();
	}

	private static string GetCurseName(CurseType curseType)
	{
		return curseType switch
		{
			CurseType.Death => SokLoc.Translate("card_death_curse_name"), 
			CurseType.Greed => SokLoc.Translate("card_greed_curse_name"), 
			CurseType.Happiness => SokLoc.Translate("card_happiness_curse_name"), 
			_ => throw new ArgumentException("curseType"), 
		};
	}

	public static IEnumerator AltarIntro(Altar altar, CurseType curseType)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("label_altar_cutscene_title");
		GameCamera.instance.TargetCardOverride = altar;
		GameCamera.instance.Screenshake = 0.5f;
		yield return new WaitForSeconds(2f);
		Title = "...";
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		WorldManager.instance.CreateSmoke(altar.transform.position);
		altar.MyGameCard.Child.DestroyCard();
		yield return new WaitForSeconds(1f);
		bool doMainRun = false;
		if ((curseType == CurseType.Death && WorldManager.instance.CurrentSave.FinishedDeath) || (curseType == CurseType.Happiness && WorldManager.instance.CurrentSave.FinishedHappiness))
		{
			Title = "";
			Text = SokLoc.Translate("label_altar_cutscene_mainland");
			yield return WaitForAnswer(SokLoc.Translate("label_altar_run_board", LocParam.Create("curse", GetCurseName(curseType))), SokLoc.Translate("label_altar_run_redo"));
			if (WorldManager.instance.ContinueButtonIndex == 0)
			{
				doMainRun = true;
			}
		}
		if (doMainRun)
		{
			GameCard gameCard = null;
			if (curseType == CurseType.Greed)
			{
				gameCard = FindOrCreateGameCard("greed_curse", altar.transform.position);
			}
			if (curseType == CurseType.Death)
			{
				gameCard = FindOrCreateGameCard("death_curse", altar.transform.position);
			}
			if (curseType == CurseType.Happiness)
			{
				gameCard = FindOrCreateGameCard("happiness_curse", altar.transform.position);
			}
			if (gameCard != null)
			{
				gameCard.SendIt();
				yield return CurseActivatedOnBoard(gameCard, curseType);
			}
		}
		else
		{
			Title = "";
			Text = "...";
			Spirit spirit = FindOrCreateSpirit(curseType, altar.transform.position);
			spirit.CreateBackgroundPlane();
			GameCamera.instance.TargetCardOverride = spirit;
			yield return WaitForContinueClicked(SokLoc.Translate("label_wow"));
			Title = "";
			Text = SokLoc.Translate("label_altar_cutscene_text", LocParam.Create("spirit", spirit.FullName));
			yield return WaitForAnswer(SokLoc.Translate("label_altar_cutscene_help_true"), SokLoc.Translate("label_altar_cutscene_help_false"));
			if (WorldManager.instance.ContinueButtonIndex == 0)
			{
				Text = SokLoc.Translate("label_altar_cutscene_help_spirit_true");
				yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
				Stop(keepCameraPosition: true);
				WorldManager.instance.StartCursePlaythrough(curseType, delegate
				{
					GameCamera.instance.TargetCardOverride = null;
					spirit.MyGameCard.DestroyCard();
				});
			}
			else
			{
				Text = SokLoc.Translate("label_altar_cutscene_help_spirit_false");
				yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
				GameCamera.instance.TargetCardOverride = null;
				spirit.MyGameCard.DestroyCard();
				yield return new WaitForSeconds(0.5f);
				Stop();
			}
		}
		altar.inCutscene = false;
	}

	public static IEnumerator DemonOfSadness()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "..";
		Text = "";
		GameCamera.instance.TargetCardOverride = WorldManager.instance.GetCard<Unhappiness>();
		yield return new WaitForSeconds(1f);
		foreach (Unhappiness card in WorldManager.instance.GetCards<Unhappiness>())
		{
			card.MyGameCard.DestroyCard();
		}
		Title = SokLoc.Translate("label_sadness_demon_title");
		Text = SokLoc.Translate("label_sadness_demon_title");
		CardData targetCardOverride = WorldManager.instance.CreateCard(WorldManager.instance.GetRandomSpawnPosition(), "sadness_demon", faceUp: true, checkAddToStack: false);
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		Stop();
	}

	public static IEnumerator ShamanLeaving(Shaman shaman)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = shaman.transform.position;
		Title = SokLoc.Translate("label_shaman_leaving_title");
		Text = SokLoc.Translate("label_shaman_leaving_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Text = "";
		WorldManager.instance.CreateSmoke(shaman.transform.position);
		shaman.MyGameCard.DestroyCard();
		yield return new WaitForSeconds(2f);
		Stop();
	}

	public static IEnumerator CurseActivatedOnBoard(GameCard curse, CurseType curseType)
	{
		Title = "...";
		Text = "";
		Spirit spirit = FindOrCreateSpirit(curseType, curse.transform.position);
		spirit.CreateBackgroundPlane();
		GameCamera.instance.TargetCardOverride = spirit;
		yield return new WaitForSeconds(1f);
		Title = "";
		Text = SokLoc.Translate("label_altar_cutscene_good_luck", LocParam.Create("spirit", spirit.FullName));
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		spirit.MyGameCard.DestroyCard();
		yield return new WaitForSeconds(0.5f);
		Stop();
	}
}
