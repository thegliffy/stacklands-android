using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CitiesCutscenes
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

	public static IEnumerator CitiesTornado()
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

	public static IEnumerator CitiesFinancialCrisis()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("cutscene_cities_financial_crisis_title");
		GameCard targetCardOverride = FindOrCreateGameCard("financial_crisis");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		Text = SokLoc.Translate("cutscene_cities_financial_crisis_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		Text = SokLoc.Translate("cutscene_cities_financial_crisis_text_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Stop();
	}

	private static List<GameCard> GetCardsToDamage()
	{
		List<GameCard> source = (from x in WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id)
			where x.CardData.IsBuilding && !x.CardData.IsDamaged
			select x).ToList();
		source = source.OrderBy((GameCard x) => Random.value).ToList();
		float num = CitiesManager.instance.Wellbeing / 10;
		int a = Mathf.Clamp(Mathf.RoundToInt((float)source.Count / Random.Range(10f - num, 5f - num)), 2, 5);
		return source.Take(Mathf.Min(a, source.Count)).ToList();
	}

	public static IEnumerator CitiesEarthQuake(GameCard origin)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCard targetCardOverride = FindOrCreateGameCard("earthquake");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		Title = SokLoc.Translate("label_cities_earthquake_title");
		Text = SokLoc.Translate("label_cities_earthquake_text");
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		List<GameCard> cardsToDamage = GetCardsToDamage();
		foreach (GameCard item in cardsToDamage)
		{
			GameCamera.instance.TargetCardOverride = item;
			item.CardData.SetCardDamaged(CardDamageType.Damaged);
			item.CancelAnyTimer();
			item.RotWobble(2f);
			AudioManager.me.PlaySound2D(AudioManager.me.DamagedCardSound, Random.Range(0.9f, 1.1f), 0.4f);
			GameCamera.instance.Screenshake = 0.5f;
			yield return new WaitForSeconds(1f);
		}
		GameCamera.instance.TargetCardOverride = null;
		Text = SokLoc.Translate("label_cities_earthquake_text_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		origin.DestroyCard(spawnSmoke: true);
		Stop();
	}

	public static IEnumerator CitiesDrought(GameCard origin)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("label_cities_drought_title");
		Text = SokLoc.Translate("label_cities_drought_text");
		GameCard targetCardOverride = FindOrCreateGameCard("drought");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		List<GameCard> list = (from x in WorldManager.instance.GetAllCardsOnBoard(WorldManager.instance.CurrentBoard.Id)
			where x.CardData is Farmland && !x.CardData.IsDamaged
			select x).ToList();
		list.Sort((GameCard a, GameCard b) => Random.Range(-1, 1));
		float num = CitiesManager.instance.Wellbeing / 10;
		int a2 = Mathf.Clamp(Mathf.RoundToInt((float)list.Count / Random.Range(10f - num, 5f - num)), 2, 5);
		list = list.Take(Mathf.Min(a2, list.Count)).ToList();
		foreach (GameCard item in list)
		{
			GameCamera.instance.TargetCardOverride = item;
			item.CardData.SetCardDamaged(CardDamageType.Drought);
			item.CancelAnyTimer();
			item.RotWobble(2f);
			AudioManager.me.PlaySound2D(AudioManager.me.DroughtStart, Random.Range(0.9f, 1.1f), 0.4f);
			GameCamera.instance.Screenshake = 0.5f;
			yield return new WaitForSeconds(1f);
		}
		GameCamera.instance.TargetCardOverride = null;
		Text = SokLoc.Translate("label_cities_drought_text_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		origin.DestroyCard(spawnSmoke: true);
		Stop();
	}

	public static IEnumerator CitiesWildFire(GameCard origin)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = SokLoc.Translate("label_cities_wildfire_title");
		Text = SokLoc.Translate("label_cities_wildfire_text");
		GameCard targetCardOverride = FindOrCreateGameCard("wildfire");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		yield return WaitForContinueClicked(SokLoc.Translate("label_uh_oh"));
		List<GameCard> cardsToDamage = GetCardsToDamage();
		foreach (GameCard item in cardsToDamage)
		{
			GameCamera.instance.TargetCardOverride = item;
			item.CardData.SetCardDamaged(CardDamageType.Fire);
			item.CancelAnyTimer();
			AudioManager.me.PlaySound2D(AudioManager.me.OnFireCardSound, Random.Range(0.9f, 1.1f), 0.4f);
			GameCamera.instance.Screenshake = 0.2f;
			yield return new WaitForSeconds(1f);
		}
		GameCamera.instance.TargetCardOverride = null;
		Text = SokLoc.Translate("label_cities_wildfire_text_1");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		origin.DestroyCard(spawnSmoke: true);
		Stop();
	}

	public static IEnumerator DinoBoss(Laboratory laboratory, CardData fossil)
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetPositionOverride = laboratory.transform.position;
		Text = SokLoc.Translate("label_dino_laboratory");
		Title = "";
		if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
		{
			string text = SokLoc.Translate("label_start_dino");
			string text2 = SokLoc.Translate("label_cancel_dino");
			yield return WaitForAnswer(text, text2);
		}
		else
		{
			string text = SokLoc.Translate("label_well_done");
			yield return WaitForAnswer(text);
		}
		if (WorldManager.instance.ContinueButtonIndex == 0)
		{
			fossil.MyGameCard.DestroyCard(spawnSmoke: true);
			if (!WorldManager.instance.CurrentRunOptions.IsPeacefulMode)
			{
				WorldManager.instance.CreateCard(laboratory.transform.position, "dino", faceUp: true, checkAddToStack: false);
			}
			else
			{
				QuestManager.instance.ActionComplete(laboratory, "cities_defeat_trex");
			}
		}
		else
		{
			fossil.MyGameCard.RemoveFromStack();
		}
		laboratory.InCutscene = false;
		Stop();
	}

	public static IEnumerator CitiesParticleCollider(GameCard origin)
	{
		ParticleCollider collider = origin.CardData as ParticleCollider;
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetCardOverride = origin;
		Title = SokLoc.Translate("cutscene_cities_particle_collider_title");
		Text = SokLoc.Translate("cutscene_cities_particle_collider_text");
		yield return WaitForContinueClicked(SokLoc.Translate("cutscene_cities_particle_collider_switch"));
		collider.ColliderRunning = true;
		origin.CardData.DestroyChildrenMatchingPredicateAndRestack((CardData x) => x.Id == "uranium", 2);
		AudioManager.me.PlaySound2D(AudioManager.me.ColliderRunningSound, 1f, 0.5f);
		GameCamera.instance.Screenshake = 0.5f;
		yield return new WaitForSeconds(4.5f);
		CardData targetCardOverride = WorldManager.instance.CreateCard(origin.Position, "quantum_entangled_uranium");
		collider.ColliderRunning = false;
		GameCamera.instance.Screenshake = 0f;
		GameCamera.instance.TargetCardOverride = null;
		Text = SokLoc.Translate("cutscene_cities_particle_collider_text_1");
		GameCamera.instance.TargetCardOverride = targetCardOverride;
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		collider.CutsceneQueued = false;
		Stop();
	}

	public static IEnumerator CitiesStopDisaster()
	{
		CutsceneScreen.instance.IsAdvisorCutscene = true;
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		Title = "";
		Text = SokLoc.Translate("label_greed_outro_wear_crown");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
		Stop();
	}

	private static void Stop(bool keepCameraPosition = false)
	{
		Text = "";
		Title = "";
		GameCamera.instance.TargetPositionOverride = null;
		GameCamera.instance.CameraPositionDistanceOverride = null;
		GameCamera.instance.TargetCardOverride = null;
		CutsceneScreen.instance.IsEndOfMonthCutscene = false;
		CutsceneScreen.instance.IsAdvisorCutscene = false;
		CutsceneScreen.instance.CheckAdvisorCutscene();
		if (keepCameraPosition)
		{
			GameCamera.instance.KeepCameraAtCurrentPos();
		}
		GameCanvas.instance.SetScreen<GameScreen>();
		currentAnimation = null;
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

	public static IEnumerator CitiesGameOver()
	{
		GameCanvas.instance.SetScreen<CutsceneScreen>();
		GameCamera.instance.TargetCardOverride = null;
		GameCamera.instance.TargetPositionOverride = null;
		yield return new WaitForSeconds(2f);
		Text = SokLoc.Translate("label_final_demand_merchant");
		yield return WaitForContinueClicked(SokLoc.Translate("label_okay"));
	}
}
