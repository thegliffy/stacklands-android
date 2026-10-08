using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Selectable, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
	public Image Image;

	public bool EnableUnderline = true;

	public bool ButtonEnabled = true;

	private RectTransform rectTransform;

	private bool isDown;

	public Func<bool> IsSelectableAction;

	public SokScreen parentScreen;

	public bool SetColor = true;

	public AudioClip CustomSound;

	private Color startColor;

	public string TooltipText;

	private ScrollRect parentScrollRect;

	private bool? _isSelected;

	private bool triedFindingTmPro;

	private TextMeshProUGUI tmPro;

	private PointerEventData lastEventData;

	private float HorizontalStickTimer;

	public bool IsHovered;

	public bool ScrollToInRect = true;

	private bool tryFindParentScrollRect = true;

	public RectTransform RectTransform => (RectTransform)base.transform;

	public bool SelectableWithController
	{
		get
		{
			if (!ButtonEnabled)
			{
				return false;
			}
			if (parentScreen != null && (GameCanvas.instance.ModalIsOpen || (TransitionScreen.InTransition && !TransitionScreen.instance.IsLeaving)))
			{
				return false;
			}
			if (IsSelectableAction == null)
			{
				return true;
			}
			return IsSelectableAction();
		}
	}

	public bool IsSelected
	{
		get
		{
			if (!_isSelected.HasValue)
			{
				if (InputController.instance.CurrentSchemeIsController && base.currentSelectionState == SelectionState.Selected)
				{
					_isSelected = true;
				}
				else
				{
					_isSelected = false;
				}
			}
			return _isSelected.Value;
		}
	}

	public TextMeshProUGUI TextMeshPro
	{
		get
		{
			if (!triedFindingTmPro)
			{
				tmPro = GetComponent<TextMeshProUGUI>();
				if (tmPro == null)
				{
					tmPro = GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
				}
				triedFindingTmPro = true;
			}
			return tmPro;
		}
	}

	private bool canBeClicked => !(parentScreen != null) || !GameCanvas.instance.ModalIsOpen;

	private Camera cam => null;

	public bool WasRightClick
	{
		get
		{
			if (lastEventData != null)
			{
				return lastEventData.button == PointerEventData.InputButton.Right;
			}
			return false;
		}
	}

	public bool IsClicked
	{
		get
		{
			bool result = false;
			if (isDown && InputController.instance.InputCount > 0 && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, InputController.instance.GetInputPosition(0), cam))
			{
				result = true;
			}
			if (TransitionScreen.InTransition || !ButtonEnabled || !GameCanvas.instance.ScreenIsInteractable(parentScreen) || (InputController.instance.IsUsingMouse && InputController.instance.MouseIsDragging))
			{
				result = false;
			}
			return result;
		}
	}

	[HideInInspector]
	public event Action Clicked;

	[HideInInspector]
	public event Action<Vector2> StartDragging;

	public event Func<CustomButton, Navigation, Navigation> ExplicitNavigationChanged;

	protected override void Awake()
	{
		if (Application.isPlaying)
		{
			Image = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();
			startColor = Image.color;
			base.Awake();
		}
	}

	protected override void Start()
	{
		if (Application.isPlaying)
		{
			parentScreen = GameCanvas.instance.GetParentScreen(rectTransform);
			if (parentScreen == null && Application.isEditor)
			{
				Debug.LogWarning("No parent screen found for " + base.name);
			}
			base.Start();
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (canBeClicked)
		{
			isDown = true;
		}
		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (isDown)
		{
			lastEventData = eventData;
			if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, cam) && canBeClicked)
			{
				SubmitClick();
			}
			isDown = false;
		}
		base.OnPointerUp(eventData);
	}

	private void HorizontalStick()
	{
		Slider componentInChildren = GetComponentInChildren<Slider>();
		if (!(componentInChildren == null))
		{
			if (HorizontalStickTimer == 0f)
			{
				componentInChildren.value += ((InputController.instance.GetMove().x > 0f) ? 0.05f : (-0.05f));
			}
			HorizontalStickTimer += Time.deltaTime;
			if (HorizontalStickTimer > 1.15f - Mathf.Abs(InputController.instance.GetMove().x))
			{
				HorizontalStickTimer = 0f;
			}
		}
	}

	private void SubmitClick()
	{
		if (parentScreen == null)
		{
			parentScreen = GameCanvas.instance.GetParentScreen(rectTransform);
		}
		bool flag = parentScreen == null || GameCanvas.instance.ScreenIsInteractable(parentScreen);
		bool flag2 = InputController.instance.IsUsingMouse && InputController.instance.MouseIsDragging;
		if (this.Clicked != null && !TransitionScreen.InTransition && flag && !flag2 && ButtonEnabled)
		{
			this.Clicked();
			if (CustomSound == null)
			{
				AudioManager.me.PlaySound2D(AudioManager.me.Click, 1f, 0.1f);
				return;
			}
			AudioManager.me.PlaySound2D(new List<AudioClip> { CustomSound }, 1f, 0.1f);
		}
	}

	protected override void OnDisable()
	{
		isDown = false;
		base.OnDisable();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (tryFindParentScrollRect)
		{
			tryFindParentScrollRect = false;
			parentScrollRect = GetComponentInParent<ScrollRect>();
		}
		if (ButtonEnabled && IsSelected && InputController.instance != null && InputController.instance.CurrentSchemeIsController && SelectableWithController)
		{
			if (InputController.instance.SubmitTriggered())
			{
				SubmitClick();
			}
			if (InputController.instance.GetStickHorizontal())
			{
				HorizontalStick();
			}
			else
			{
				HorizontalStickTimer = 0f;
			}
			if (parentScrollRect != null && ScrollToInRect)
			{
				ScrollToMe();
			}
		}
		if (IsHovered)
		{
			Tooltip.Text = TooltipText;
		}
		Color color = (SetColor ? ColorManager.instance.ButtonColor : startColor);
		if ((IsHovered || IsSelected) && ButtonEnabled && SetColor)
		{
			color = ColorManager.instance.HoverButtonColor;
		}
		if (TextMeshPro != null)
		{
			if (!ButtonEnabled)
			{
				TextMeshPro.color = ColorManager.instance.DisabledButtonTextColor;
			}
			else
			{
				TextMeshPro.color = ColorManager.instance.ButtonTextColor;
			}
			FontStyles fontStyle = TextMeshPro.fontStyle;
			fontStyle = (((!IsHovered && !IsSelected) || !ButtonEnabled || !EnableUnderline) ? (fontStyle & ~FontStyles.Underline) : (fontStyle | FontStyles.Underline));
			TextMeshPro.fontStyle = fontStyle;
		}
		if (SetColor && Image != null)
		{
			Image.color = color;
		}
		base.interactable = ButtonEnabled;
		if (!InputController.instance.CurrentSchemeIsController)
		{
			return;
		}
		if (!SelectableWithController)
		{
			Navigation navigation = base.navigation;
			navigation.mode = Navigation.Mode.None;
			base.navigation = navigation;
			return;
		}
		Navigation arg = base.navigation;
		if (this.ExplicitNavigationChanged != null && IsSelected)
		{
			arg.mode = Navigation.Mode.Explicit;
			arg.selectOnLeft = FindSelectable(Vector3.left);
			arg.selectOnRight = FindSelectable(Vector3.right);
			arg.selectOnUp = FindSelectable(Vector3.up);
			arg.selectOnDown = FindSelectable(Vector3.down);
			arg = this.ExplicitNavigationChanged(this, arg);
		}
		else
		{
			arg.mode = Navigation.Mode.Automatic;
		}
		base.navigation = arg;
	}

	public void ScrollToMe()
	{
		GameCanvas.SetScrollRectPosition(parentScrollRect, rectTransform, centerInView: true);
	}

	private void LateUpdate()
	{
		if (Application.isPlaying)
		{
			if (IsSelected && (!SelectableWithController || !ButtonEnabled) && InputController.instance.CurrentSchemeIsController)
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
			_isSelected = null;
		}
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (this.StartDragging != null)
		{
			this.StartDragging(eventData.position);
			return;
		}
		isDown = false;
		GetComponentInParent<ScrollRect>()?.OnBeginDrag(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		GetComponentInParent<ScrollRect>()?.OnDrag(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		GetComponentInParent<ScrollRect>()?.OnEndDrag(eventData);
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
	}

	public void HardSetText(string text)
	{
		GetComponentInChildren<TextMeshProUGUI>().text = text;
	}
}
