using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
	public enum ScreenPosition
	{
		Left,
		Right,
		Bottom,
		Top,
		None
	}

	public static GameCanvas instance;

	public RectTransform FreeMoveParent;

	public SokScreen CurrentScreen;

	public RectTransform Modal;

	public Canvas Canvas;

	private List<ScreenPosition> screenPositions;

	private List<bool> screenInTransition;

	private Dictionary<RectTransform, GameObject> lastSelectedObject = new Dictionary<RectTransform, GameObject>();

	private List<SokScreen> screens;

	private List<RaycastResult> raycastResults = new List<RaycastResult>();

	public GameObject Transition;

	public bool ModalIsOpen;

	public GameObject MouseOverObject;

	private CustomButton previousHovered;

	private PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

	private GetComponentCacher<CustomButton> getCustomButtonCacher = new GetComponentCacher<CustomButton>();

	public Ease easeType = Ease.InOutCubic;

	private RectTransform rectTransform => base.transform as RectTransform;

	public bool NothingSelected
	{
		get
		{
			if (!(EventSystem.current.currentSelectedGameObject == null))
			{
				return !EventSystem.current.currentSelectedGameObject.activeInHierarchy;
			}
			return true;
		}
	}

	public GameObject SelectedObject => EventSystem.current.currentSelectedGameObject;

	private void Awake()
	{
		instance = this;
		screens = GetComponentsInChildren<SokScreen>(includeInactive: true).ToList();
		screenPositions = new List<ScreenPosition>();
		screenInTransition = new List<bool>();
		foreach (SokScreen screen in screens)
		{
			_ = screen;
			screenPositions.Add(ScreenPosition.Bottom);
			screenInTransition.Add(item: false);
		}
		foreach (SokScreen screen2 in screens)
		{
			screen2.gameObject.SetActive(value: true);
			screen2.gameObject.SetActive(value: false);
		}
		Modal.gameObject.SetActive(value: true);
		Modal.gameObject.SetActive(value: false);
		FreeMoveParent.gameObject.SetActive(value: true);
		Transition.gameObject.SetActive(value: true);
		SetScreen<MainMenu>();
	}

	private void Update()
	{
		if ((!(CurrentScreen is GameScreen) || GameScreen.instance.ControllerIsInUI) && InputController.instance.CurrentSchemeIsController && NothingSelected)
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				_ = !EventSystem.current.currentSelectedGameObject.activeInHierarchy;
			}
			else
				_ = 0;
			SelectFirstSelectable(CurrentScreen);
		}
		if (InputController.instance.CancelTriggered() && !ModalIsOpen)
		{
			SokScreen previousScreen = GetPreviousScreen(CurrentScreen);
			if (previousScreen != null)
			{
				SetScreen(previousScreen);
			}
		}
		PerformMouseRaycast();
		UpdateMouseOverObject();
		UpdateCustomButtonHovered();
	}

	private void UpdateMouseOverObject()
	{
		if (!(EventSystem.current == null))
		{
			if (raycastResults.Count > 0)
			{
				MouseOverObject = raycastResults[0].gameObject;
			}
			else
			{
				MouseOverObject = null;
			}
		}
	}

	private void PerformMouseRaycast()
	{
		eventDataCurrentPosition.Reset();
		eventDataCurrentPosition.position = InputController.instance.ClampedMousePosition();
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
	}

	private void UpdateCustomButtonHovered()
	{
		if (previousHovered != null)
		{
			previousHovered.IsHovered = false;
		}
		if (!InputController.instance.IsUsingMouse || TransitionScreen.InTransition || InputController.instance.MouseIsDragging || EventSystem.current == null)
		{
			return;
		}
		foreach (RaycastResult raycastResult in raycastResults)
		{
			CustomButton component = getCustomButtonCacher.GetComponent(raycastResult.gameObject);
			if (!(component == null) && (!(component.parentScreen != null) || !ModalIsOpen) && RectTransformUtility.RectangleContainsScreenPoint(component.RectTransform, InputController.instance.ClampedMousePosition(), null) && IsSelected(component.RectTransform, raycastResults))
			{
				component.IsHovered = true;
				previousHovered = component;
				break;
			}
		}
	}

	private SokScreen GetPreviousScreen(SokScreen screen)
	{
		if (screen is CardopediaScreen)
		{
			if (WorldManager.instance.CurrentGameState == WorldManager.GameState.Paused)
			{
				return GetScreen<PauseScreen>();
			}
			return GetScreen<MainMenu>();
		}
		if (screen is CreditsScreen || screen is SelectResolutionScreen || screen is SelectLanguageScreen || screen is SelectSaveScreen || screen is ControlsScreen || screen is AccessibilityScreen || screen is AdvancedSettingsScreen)
		{
			return GetScreen<OptionsScreen>();
		}
		return null;
	}

	public static string FormatTimeLeft(float timeLeft)
	{
		return SokLoc.Translate("label_seconds_left_format", LocParam.Create("seconds", timeLeft.ToString("0.0")));
	}

	public static string FormatTime(float time)
	{
		return SokLoc.Translate("label_seconds_format", LocParam.Create("seconds", time.ToString("0.0")));
	}

	public static string FormatTimeShort(float time)
	{
		return SokLoc.Translate("label_seconds_format", LocParam.Create("seconds", time.ToString("0")));
	}

	public void SetScreen(SokScreen nextScreen)
	{
		if (nextScreen == CurrentScreen)
		{
			return;
		}
		SokScreen currentScreen = CurrentScreen;
		if (currentScreen != null)
		{
			lastSelectedObject[currentScreen.Rect] = EventSystem.current.currentSelectedGameObject;
		}
		HandleTransition(CurrentScreen, nextScreen);
		CurrentScreen = nextScreen;
		foreach (SokScreen screen in screens)
		{
			if (!(screen == currentScreen))
			{
				screen.gameObject.SetActive(nextScreen == screen);
			}
		}
		if (lastSelectedObject.ContainsKey(nextScreen.Rect))
		{
			GameObject gameObject = lastSelectedObject[nextScreen.Rect];
			if (gameObject != null && gameObject.gameObject.activeInHierarchy)
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}
		SetFrameRateCap(CurrentScreen);
	}

	public SokScreen GetScreen<T>() where T : SokScreen
	{
		return screens.Find((SokScreen x) => x is T);
	}

	public void SetScreen<T>() where T : SokScreen
	{
		SokScreen screen = GetScreen<T>();
		if (screen == CurrentScreen)
		{
			return;
		}
		SokScreen currentScreen = CurrentScreen;
		if (currentScreen != null)
		{
			lastSelectedObject[currentScreen.Rect] = EventSystem.current.currentSelectedGameObject;
		}
		HandleTransition(CurrentScreen, screen);
		CurrentScreen = screen;
		foreach (SokScreen screen2 in screens)
		{
			if (!(screen2 == currentScreen))
			{
				screen2.gameObject.SetActive(screen == screen2);
			}
		}
		if (lastSelectedObject.ContainsKey(screen.Rect))
		{
			GameObject gameObject = lastSelectedObject[screen.Rect];
			if (gameObject != null && gameObject.activeInHierarchy)
			{
				EventSystem.current.SetSelectedGameObject(gameObject);
			}
		}
		SetFrameRateCap(CurrentScreen);
	}

	private void SetFrameRateCap(SokScreen currentScreen)
	{
		if (currentScreen.IsFrameRateUncapped)
		{
			int @int = PlayerPrefs.GetInt("framerate", -1);
			Application.targetFrameRate = Mathf.Max(-1, @int);
			if (@int == -1)
			{
				QualitySettings.vSyncCount = 1;
			}
			else
			{
				QualitySettings.vSyncCount = 0;
			}
		}
		else
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 1;
		}
	}

	private void HandleTransition(SokScreen prevScreen, SokScreen nextScreen)
	{
		if (!HasTransition(prevScreen, nextScreen))
		{
			prevScreen?.Rect.DOKill(complete: true);
			nextScreen.Rect.DOKill(complete: true);
			if (prevScreen != null)
			{
				prevScreen.gameObject.SetActive(value: false);
			}
			nextScreen.gameObject.SetActive(value: true);
			nextScreen.Rect.anchoredPosition = Vector2.zero;
		}
		else
		{
			ScreenPosition pos = ScreenPosition.Left;
			ScreenPosition pos2 = ScreenPosition.Left;
			LeaveTo(prevScreen, pos);
			EnterFrom(nextScreen, pos2);
		}
	}

	public bool MousePositionIsOverUI()
	{
		return raycastResults.Count > 0;
	}

	public bool PositionIsOverUI(Vector2 screenPos)
	{
		if (EventSystem.current == null)
		{
			return false;
		}
		eventDataCurrentPosition.Reset();
		eventDataCurrentPosition.position = screenPos;
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
		return raycastResults.Count > 0;
	}

	public Vector3 ScreenPosToLocalPos(Vector3 pos)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(FreeMoveParent, pos, null, out var localPoint);
		return localPoint;
	}

	public bool AboveMeOrMyChildrenRaycast(Transform t, Vector2 position)
	{
		if (EventSystem.current == null)
		{
			return false;
		}
		eventDataCurrentPosition.Reset();
		eventDataCurrentPosition.position = position;
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
		foreach (RaycastResult raycastResult in raycastResults)
		{
			if (raycastResult.gameObject.transform.IsChildOf(t) || raycastResult.gameObject.transform == t)
			{
				return true;
			}
		}
		return false;
	}

	public bool AboveMeOrMyChildren(Transform t, GameObject mouseOverObject)
	{
		if (mouseOverObject == null)
		{
			return false;
		}
		if (mouseOverObject.transform.IsChildOf(t) || mouseOverObject.transform == t)
		{
			return true;
		}
		return false;
	}

	public static Rect GetWorldRect2(RectTransform rt)
	{
		Vector3[] array = new Vector3[4];
		rt.GetWorldCorners(array);
		Vector3 vector = array[0];
		return new Rect(size: array[2] - array[0], position: vector);
	}

	public Rect GetWorldRect(RectTransform rt)
	{
		return GetWorldRect2(rt);
	}

	public static void SetScrollRectPosition(ScrollRect scrollRect, RectTransform rectTransform, bool centerInView = false)
	{
		if (centerInView)
		{
			scrollRect.ScrollToCenter(rectTransform);
			return;
		}
		Vector2 vector = scrollRect.transform.InverseTransformPoint(scrollRect.content.position);
		Rect worldRect = instance.GetWorldRect(rectTransform);
		Vector3 position = new Vector3(worldRect.xMin, worldRect.yMax);
		Vector2 vector2 = scrollRect.transform.InverseTransformPoint(position);
		Vector2 anchoredPosition = vector - vector2;
		anchoredPosition.x = scrollRect.content.anchoredPosition.x;
		scrollRect.content.anchoredPosition = anchoredPosition;
		scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
	}

	public void OpenModal()
	{
		Modal.gameObject.SetActive(value: true);
		ModalIsOpen = true;
	}

	public void CloseModal()
	{
		Modal.gameObject.SetActive(value: false);
		ModalIsOpen = false;
	}

	public void ShowDlcNotInstalledModal()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_spirit_dlc_not_installed_title"), SokLoc.Translate("label_spirit_dlc_not_installed_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowCantChangeBoardSpirit()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_change_board_disabled_title"), SokLoc.Translate("label_change_board_disabled_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowNameCombatableModal(CardData cb, Action onDone)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_name_villager_title"), SokLoc.Translate("label_name_villager_text"));
		TMP_InputField input = ModalScreen.instance.AddInputNoButton();
		if (!string.IsNullOrEmpty(cb.CustomName))
		{
			input.text = cb.CustomName;
		}
		else
		{
			input.text = cb.Name;
		}
		input.characterLimit = 12;
		ModalScreen.instance.AddOption(SokLoc.Translate("label_random_name"), delegate
		{
			input.text = GetRandomName();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			ProfanityChecker profanityChecker = WorldManager.instance.GameDataLoader.ProfanityChecker;
			string text = input.text;
			if (profanityChecker.IsProfanityInLanguage(SokLoc.instance.CurrentLanguage, text))
			{
				text = "Bobba";
			}
			text = StringUtils.RemoveRichText(text);
			cb.CustomName = text;
			CloseModal();
			onDone();
		});
		OpenModal();
	}

	public string GetRandomName()
	{
		return WorldManager.instance.GameDataLoader.VillagerNames.Choose();
	}

	public void ShowSimpleModal(string textTerm, string titleTerm)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(titleTerm, textTerm);
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowClearSaveModal()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_clear_save"), SokLoc.Translate("label_clear_save_confirm"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			TransitionScreen.instance.StartTransition(delegate
			{
				WorldManager.instance.ClearSaveAndRestart();
				CloseModal();
			});
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowEarlyAccessModal()
	{
		ModalScreen.instance.Clear();
		string text = SokLoc.Translate("label_youtube_cities_dlc_text", LocParam.Create("saveDirectory", PlatformHelper.CurrentSavesDirectory));
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_youtube_cities_dlc_title"), text);
		ModalScreen.instance.AddOption(SokLoc.Translate("label_load_run"), delegate
		{
			SaveGame saveFromResources = SaveManager.instance.GetSaveFromResources("Saves/2000_BetaSave");
			saveFromResources.SaveId = "4";
			SaveManager.instance.Save(saveFromResources);
			WorldManager.RestartGame();
			CloseModal();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_exit"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ShowStartNewRunModal(Action onYes)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_start_new_run"), SokLoc.Translate("label_new_run_confirm"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void OneVillagerNeedsToStayPrompt(string term)
	{
		ModalScreen.instance.Clear();
		if (WorldManager.instance.CurrentBoard.Id == "main")
		{
			ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_one_villager_needs_to_stay"));
		}
		else if (WorldManager.instance.CurrentBoard.Id == "island")
		{
			ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_one_villager_needs_to_stay_island"));
		}
		else
		{
			ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_one_villager_needs_to_stay"));
		}
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void MaxVillagerCountPrompt(string term, int amount)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate(term), SokLoc.Translate("label_max_villager_in_portal", LocParam.Create("amount", amount.ToString())));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void NotEnoughFoodToUsePortalPrompt()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_taking_portal_title"), SokLoc.Translate("label_not_enough_food_before_portal"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void NotEnoughFoodToSailOffPrompt()
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_sailing_off_title"), SokLoc.Translate("label_not_enough_food_before_sailing"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_okay"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void LeaveSpiritWorldPrompt(Action onYes, Action onNo)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_return_from_spirit_world_title"), SokLoc.Translate("label_return_from_spirit_world_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo();
		});
		OpenModal();
	}

	public void AbandonCityPrompt(Action onYes, Action onNo)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_abandon_city_title"), SokLoc.Translate("label_abandon_city_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo?.Invoke();
		});
		OpenModal();
	}

	public void GoToCityPrompt(Action onYes, Action onNo)
	{
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_go_to_city_title"), SokLoc.Translate("label_go_to_city_text"));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo?.Invoke();
		});
		OpenModal();
	}

	public void MissingCardsInSavePrompt(Action onYes, HashSet<string> missingCardIds)
	{
		ModalScreen.instance.Clear();
		string text = string.Join(", ", missingCardIds.Take(3));
		if (missingCardIds.Count > 3)
		{
			text = text + " " + SokLoc.Translate("label_missing_cards_more_text", LocParam.Create("amount", (missingCardIds.Count - 3).ToString()));
		}
		ModalScreen.instance.SetTexts(SokLoc.Translate("label_missing_cards_title"), SokLoc.Translate("label_missing_cards_text", LocParam.Create("missing", text)));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
		});
		OpenModal();
	}

	public void ChangeLocationPrompt(Action onYes, Action onNo, string destinationBoard)
	{
		string termId = "label_return_to_mainland_prompt";
		string termId2 = "label_sailing_off_title";
		if (WorldManager.instance.CurrentBoard.Id == "main")
		{
			switch (destinationBoard)
			{
			case "island":
				termId = "label_go_to_island_prompt";
				break;
			case "forest":
				termId2 = "label_taking_portal_title";
				termId = "label_go_to_forest_prompt";
				break;
			case "cities":
				termId2 = "label_take_time_machine";
				termId = "label_go_to_cities_mainland";
				break;
			}
		}
		else if (WorldManager.instance.CurrentBoard.Id == "island")
		{
			if (destinationBoard == "main")
			{
				termId = "label_return_to_mainland_prompt";
			}
			else if (destinationBoard == "forest")
			{
				termId2 = "label_taking_portal_title";
				termId = "label_go_to_forest_prompt";
			}
		}
		else if (WorldManager.instance.CurrentBoard.Id == "cities" && destinationBoard == "main")
		{
			termId2 = "label_take_time_machine";
			termId = "label_go_to_mainland_cities";
		}
		ModalScreen.instance.Clear();
		ModalScreen.instance.SetTexts(SokLoc.Translate(termId2), SokLoc.Translate(termId));
		ModalScreen.instance.AddOption(SokLoc.Translate("label_yes"), delegate
		{
			CloseModal();
			onYes();
		});
		ModalScreen.instance.AddOption(SokLoc.Translate("label_no"), delegate
		{
			CloseModal();
			onNo();
		});
		OpenModal();
	}

	private bool HasTransition(SokScreen prevScreen, SokScreen nextScreen)
	{
		if (prevScreen == null)
		{
			return false;
		}
		if (nextScreen is GameScreen || prevScreen is GameScreen)
		{
			return false;
		}
		if (prevScreen is CreditsScreen || nextScreen is CreditsScreen)
		{
			return false;
		}
		return true;
	}

	private float GetEaseDuration(SokScreen screen, bool leaving)
	{
		return 0.25f;
	}

	private static Vector2 GetPosition(ScreenPosition pos, Vector2 size)
	{
		Vector2 vector = pos switch
		{
			ScreenPosition.Left => Vector2.left, 
			ScreenPosition.Right => Vector2.right, 
			ScreenPosition.Bottom => Vector2.down, 
			ScreenPosition.Top => Vector2.up, 
			ScreenPosition.None => new Vector2(0f, 0f), 
			_ => throw new Exception($"Can't get screen position for {pos}"), 
		};
		return new Vector2(vector.x * size.x, vector.y * size.y);
	}

	private void EnterFrom(SokScreen screen, ScreenPosition pos)
	{
		if (!(screen == null))
		{
			RectTransform rect = screen.Rect;
			TrackScreenPosition(screen, ScreenPosition.None);
			float easeDuration = GetEaseDuration(screen, leaving: false);
			screenInTransition[ScreenIndex(screen)] = true;
			rect.DOKill();
			rect.DOAnchorPos(Vector2.zero, easeDuration, snapping: true).From(GetPosition(pos, rectTransform.sizeDelta) * 1.2f).SetEase(easeType)
				.OnComplete(delegate
				{
					screenInTransition[ScreenIndex(screen)] = false;
				});
		}
	}

	private int ScreenIndex(SokScreen s)
	{
		return screens.IndexOf(s);
	}

	private void TrackScreenPosition(SokScreen screen, ScreenPosition pos)
	{
		if (!(screen == null))
		{
			screenPositions[screens.IndexOf(screen)] = pos;
		}
	}

	private void LeaveTo(SokScreen screen, ScreenPosition pos)
	{
		RectTransform rect = screen.Rect;
		if (!(rect == null))
		{
			TrackScreenPosition(screen, pos);
			float easeDuration = GetEaseDuration(screen, leaving: true);
			screenInTransition[ScreenIndex(screen)] = true;
			rect.DOKill();
			rect.DOAnchorPos(GetPosition(pos, rectTransform.sizeDelta), easeDuration, snapping: true).SetEase(easeType).OnComplete(delegate
			{
				rect.gameObject.SetActive(value: false);
				screenInTransition[ScreenIndex(screen)] = false;
			});
		}
	}

	public void EaseRectTransformPosition(RectTransform rectTransform, ScreenPosition target, Vector2 size, Action onComplete = null)
	{
		rectTransform.DOKill();
		Vector2 position = GetPosition(target, size);
		rectTransform.DOAnchorPos(position, 0.4f).SetEase(easeType).OnComplete(delegate
		{
			onComplete?.Invoke();
		});
	}

	public void SetRectTransformPosition(RectTransform rectTransform, ScreenPosition target, Vector2 size)
	{
		Vector2 position = GetPosition(target, size);
		rectTransform.anchoredPosition = position;
	}

	public SokScreen GetParentScreen(RectTransform obj)
	{
		return obj.gameObject.GetComponentInParent<SokScreen>(includeInactive: true);
	}

	private bool ScreenInTransition(SokScreen screen)
	{
		return screenInTransition[ScreenIndex(screen)];
	}

	public bool AnyScreenInTransition()
	{
		for (int i = 0; i < screenInTransition.Count; i++)
		{
			if (screenInTransition[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool ScreenIsInteractable(SokScreen screen)
	{
		if (CurrentScreen == screen)
		{
			return !ScreenInTransition(screen);
		}
		return false;
	}

	public bool ScreenIsInteractable<T>() where T : SokScreen
	{
		SokScreen screen = GetScreen<T>();
		if (CurrentScreen == screen)
		{
			return !ScreenInTransition(screen);
		}
		return false;
	}

	public bool IsSelected(Transform t, Vector2 position)
	{
		if (EventSystem.current == null)
		{
			return false;
		}
		eventDataCurrentPosition.Reset();
		eventDataCurrentPosition.position = position;
		raycastResults.Clear();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
		if (raycastResults.Count == 0)
		{
			return false;
		}
		RaycastResult raycastResult = raycastResults.OrderBy((RaycastResult x) => x.displayIndex).First();
		if (!(raycastResult.gameObject.transform == t))
		{
			return IsDeepChildOf(raycastResult.gameObject.transform, t);
		}
		return true;
	}

	public bool IsSelected(Transform t, List<RaycastResult> raycastResults)
	{
		RaycastResult raycastResult = raycastResults.OrderBy((RaycastResult x) => x.displayIndex).First();
		if (!(raycastResult.gameObject.transform == t))
		{
			return IsDeepChildOf(raycastResult.gameObject.transform, t);
		}
		return true;
	}

	private bool IsDeepChildOf(Transform a, Transform parent)
	{
		Transform transform = a;
		while (transform.parent != null)
		{
			if (transform.parent == parent)
			{
				return true;
			}
			transform = transform.parent;
		}
		return false;
	}

	public void ReselectFirstSelectable()
	{
		if (CurrentScreen != null)
		{
			SelectFirstSelectable(CurrentScreen);
		}
	}

	private void SelectFirstSelectable(SokScreen screen)
	{
		RectTransform rectTransform = screen.Rect;
		if (ModalIsOpen)
		{
			rectTransform = ModalScreen.instance.transform as RectTransform;
		}
		if (rectTransform == null)
		{
			return;
		}
		Selectable selectable = rectTransform.GetComponentInChildren<Selectable>();
		if (selectable == null)
		{
			if (Application.isEditor)
			{
				Debug.Log("Could not find a Selectable child in " + rectTransform.name);
			}
			return;
		}
		int num = 0;
		while (selectable.FindSelectableOnUp() != null && num < 100)
		{
			selectable = selectable.FindSelectableOnUp();
			num++;
		}
		if (selectable is CustomButton && !(selectable as CustomButton).SelectableWithController)
		{
			CustomButton[] componentsInChildren = rectTransform.GetComponentsInChildren<CustomButton>();
			foreach (CustomButton customButton in componentsInChildren)
			{
				if (customButton.SelectableWithController)
				{
					selectable = customButton;
					break;
				}
			}
		}
		if (selectable.interactable)
		{
			selectable.Select();
			EventSystem.current.SetSelectedGameObject(selectable.gameObject);
			selectable.Select();
		}
	}

	public void SetUIToggle(bool enabled)
	{
		Canvas.enabled = enabled;
	}
}
