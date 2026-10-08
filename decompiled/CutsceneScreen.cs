using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CutsceneScreen : SokScreen
{
	public static CutsceneScreen instance;

	public TextMeshProUGUI TitleText;

	public TextMeshProUGUI StatusText;

	public Image Background;

	private List<CustomButton> multipleButtons = new List<CustomButton>();

	public CustomButton ContinueButton;

	public RectTransform ButtonSpacer;

	public RectTransform MultipleButtonsParent;

	public RectTransform WellbeingBar;

	public Image WellbeingFill;

	public Image WellbeingFill2;

	public Image WellbeingBackground;

	public float TargetWellbeingFillAmount;

	public TextMeshProUGUI WellbeingFillText;

	public TextMeshProUGUI WellbeingStateText;

	public int WellbeingAmount;

	public RectTransform WellbeingLineParent;

	public bool CanMoveScreen;

	public bool IsEndOfMonthCutscene;

	public bool IsAdvisorCutscene;

	private bool blink;

	private float blinkTimer;

	public Image Crosshair;

	public Color Color2;

	private float fillVelo;

	private float fillVelo2;

	private bool isPossitive;

	private float wellbeingCounter;

	private float prevWellbeingCounter;

	private float timer;

	private int index;

	private string lastCutsceneText;

	public override bool IsFrameRateUncapped => true;

	private void Awake()
	{
		instance = this;
		GenerateWellbeingBarLines();
	}

	private void GenerateWellbeingBarLines()
	{
		CityState[] array = (CityState[])Enum.GetValues(typeof(CityState));
		float num = 50f;
		for (int i = 0; i < array.Length - 1; i++)
		{
			LayoutElement layoutElement = new GameObject
			{
				name = "Spacer"
			}.AddComponent<LayoutElement>();
			layoutElement.transform.SetParentClean(WellbeingLineParent);
			float flexibleWidth = ((float)array[i + 1] - (float)array[i]) / num * 10f;
			layoutElement.flexibleWidth = flexibleWidth;
			if (i < array.Length - 2)
			{
				UnityEngine.Object.Instantiate(PrefabManager.instance.WellbeingLinePrefab).transform.SetParentClean(WellbeingLineParent);
			}
		}
	}

	private void OnEnable()
	{
		Background.rectTransform.anchoredPosition = new Vector2(20f, -200f);
		ContinueButton.Clicked += delegate
		{
			WorldManager.instance.ContinueClicked = true;
		};
	}

	public void ClearMultipleOptions()
	{
		foreach (CustomButton multipleButton in multipleButtons)
		{
			UnityEngine.Object.Destroy(multipleButton.gameObject);
		}
		multipleButtons.Clear();
	}

	public void EnableWellbeingBar(int wellbeingAmount)
	{
		IsEndOfMonthCutscene = true;
		wellbeingCounter = wellbeingAmount;
		WellbeingAmount = wellbeingAmount;
		WellbeingFill.fillAmount = (float)wellbeingAmount / 50f;
		WellbeingFill2.fillAmount = (float)wellbeingAmount / 50f;
		WellbeingFillText.text = wellbeingCounter.ToString();
	}

	public void CreateMultipleOptions(params string[] options)
	{
		for (int i = 0; i < options.Length; i++)
		{
			string text = options[i];
			int cap = i;
			CustomButton customButton = UnityEngine.Object.Instantiate(PrefabManager.instance.CutsceneButtonPrefab);
			customButton.transform.SetParentClean(MultipleButtonsParent);
			customButton.HardSetText(text);
			customButton.Clicked += delegate
			{
				WorldManager.instance.ContinueButtonIndex = cap;
				WorldManager.instance.ContinueClicked = true;
			};
			multipleButtons.Add(customButton);
		}
	}

	private void Update()
	{
		Crosshair.gameObject.SetActive(InputController.instance.CurrentSchemeIsController && (WorldManager.instance.RemovingCards || CanMoveScreen || WorldManager.instance.ConnectConnectors));
		WellbeingBar.gameObject.SetActive(WorldManager.instance.CurrentBoard?.Id == "cities" && IsEndOfMonthCutscene && WorldManager.instance.currentAnimationRoutine != null);
		if (WellbeingBar.gameObject.activeSelf)
		{
			WellbeingStateText.text = CitiesManager.GetCityStateTranslated(CitiesManager.GetCityStateForWellbeing(WellbeingAmount));
			float num = (float)WellbeingAmount / 50f;
			if ((double)Mathf.Abs(fillVelo2) < 0.001 && (double)Mathf.Abs(fillVelo) < 0.001)
			{
				if (num > WellbeingFill.fillAmount)
				{
					WellbeingFill.color = ColorManager.instance.FloatingTextColorSuccess;
					isPossitive = true;
				}
				else
				{
					WellbeingFill.color = ColorManager.instance.FloatingTextColorFailed;
					isPossitive = false;
				}
			}
			if (!isPossitive)
			{
				WellbeingFill2.fillAmount = FRILerp.Spring(WellbeingFill2.fillAmount, num, 2f, 5f, ref fillVelo2);
			}
			else
			{
				WellbeingFill.fillAmount = FRILerp.Spring(WellbeingFill.fillAmount, num, 2f, 5f, ref fillVelo);
			}
			wellbeingCounter = Mathf.Lerp(wellbeingCounter, WellbeingAmount, Time.deltaTime * 2f);
			if (Mathf.Abs(prevWellbeingCounter - (float)Mathf.RoundToInt(wellbeingCounter)) >= 1f)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.WellbeingCounter, 1f, 0.7f);
				prevWellbeingCounter = Mathf.RoundToInt(wellbeingCounter);
			}
			WellbeingFillText.text = Mathf.RoundToInt(wellbeingCounter).ToString();
		}
		blinkTimer += Time.deltaTime;
		if (blinkTimer >= 0.25f)
		{
			blinkTimer = 0f;
			blink = !blink;
		}
		foreach (CustomButton multipleButton in multipleButtons)
		{
			multipleButton.Image.color = (blink ? ColorManager.instance.ButtonColor : Color2);
		}
		ContinueButton.Image.color = (blink ? ColorManager.instance.ButtonColor : Color2);
		Background.rectTransform.anchoredPosition = Vector3.Lerp(Background.rectTransform.anchoredPosition, new Vector2(10f, 10f), Time.deltaTime * 20f);
		ContinueButton.TextMeshPro.text = WorldManager.instance.ContinueButtonText;
		if (ContinueButton.gameObject.activeInHierarchy && (InputController.instance.GetKeyDown(Key.Space) || InputController.instance.SubmitTriggered()))
		{
			WorldManager.instance.ContinueClicked = true;
		}
		CheckAdvisorCutscene();
	}

	public void CheckAdvisorCutscene()
	{
		CardData card = WorldManager.instance.GetCard("city_advisor");
		if (IsAdvisorCutscene && card == null)
		{
			Vector3 vector = GameCamera.instance.ScreenPosToWorldPos(new Vector2(Screen.width, Screen.height) * 0.5f);
			WorldManager.instance.CreateCard(vector, "city_advisor", faceUp: true, checkAddToStack: false);
			WorldManager.instance.CreateSmoke(vector);
		}
		if (!IsAdvisorCutscene && card != null)
		{
			card.MyGameCard.DestroyCard(spawnSmoke: true);
		}
	}

	private bool IsPronounced(char c)
	{
		return c switch
		{
			' ' => false, 
			'.' => false, 
			',' => false, 
			_ => true, 
		};
	}

	private void LateUpdate()
	{
		TitleText.text = WorldManager.instance.CutsceneTitle ?? "";
		StatusText.gameObject.SetActive(!string.IsNullOrEmpty(WorldManager.instance.CutsceneText));
		if (IsAdvisorCutscene)
		{
			if (lastCutsceneText != WorldManager.instance.CutsceneText)
			{
				index = 0;
			}
			char c = ((index < WorldManager.instance.CutsceneText.Length) ? WorldManager.instance.CutsceneText[index] : ' ');
			timer += Time.deltaTime;
			if (timer >= 0.02f)
			{
				index++;
				timer = 0f;
				CardData card = WorldManager.instance.GetCard("city_advisor");
				if (card != null)
				{
					if (index >= WorldManager.instance.CutsceneText.Length)
					{
						card.MyGameCard.ZRotOffset = 0f;
					}
					else if (index % 4 == 0)
					{
						if (IsPronounced(c))
						{
							card.MyGameCard.transform.localScale *= 0.98f;
							if (card.MyGameCard.ZRotOffset == 5f)
							{
								card.MyGameCard.ZRotOffset = -5f;
							}
							else
							{
								card.MyGameCard.ZRotOffset = 5f;
							}
						}
						else
						{
							card.MyGameCard.ZRotOffset = 0f;
						}
					}
				}
			}
		}
		else
		{
			index = 99999;
		}
		StatusText.maxVisibleCharacters = index;
		StatusText.text = WorldManager.instance.CutsceneText;
		ContinueButton.gameObject.SetActive(WorldManager.instance.ShowContinueButton);
		ButtonSpacer.gameObject.SetActive(ContinueButton.gameObject.activeInHierarchy);
		lastCutsceneText = WorldManager.instance.CutsceneText;
	}
}
