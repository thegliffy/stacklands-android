using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Users;

public class InputController : MonoBehaviour
{
	private class UserInput
	{
		public Vector2 ScreenPosition;

		public Vector2 DeltaPosition = Vector2.zero;

		public Vector2 StartPosition;

		public float StartTime;

		public int MouseId = -1;

		public int TouchId = -1;

		public int PenId = -1;

		public bool JustStarted = true;

		public bool JustEnded;

		public bool UpdatedThisFrame;

		public override string ToString()
		{
			return MouseId + " " + TouchId + ScreenPosition.ToString() + " " + DeltaPosition.ToString() + " " + JustStarted;
		}
	}

	public static InputController instance;

	public PlayerInput PlayerInput;

	public bool DisableAllInput;

	private string inputString;

	public string ActiveScheme;

	private List<UserInput> Inputs = new List<UserInput>();

	private List<UserInput> inputsToRemove = new List<UserInput>();

	public ControllerVibrator Vibrator;

	private InputAction cancel;

	private InputAction submit;

	private InputAction time_pause;

	private InputAction pause;

	private InputAction move;

	private InputAction snap_cards;

	private InputAction time_1;

	private InputAction time_2;

	private InputAction time_3;

	private InputAction zoom;

	private InputAction panel_collapse;

	private InputAction activate_ui;

	private InputAction time_toggle;

	private InputAction sell;

	private InputAction toggle_inventory;

	private InputAction toggle_view;

	private InputAction grab;

	private InputAction snap_move;

	private bool mouseIsDragging;

	private Vector2 lastMove;

	private float lastGrab;

	private Vector2 lastSnapMove;

	private ControlScheme lastControlScheme;

	public int LastInputCount;

	private Dictionary<string, string> bindingDisplayCache = new Dictionary<string, string>();

	public ControlScheme? SchemeOverride;

	private ControlScheme? _currentScheme;

	private InputDevice lastDevice;

	public string InputString => inputString;

	public bool IsUsingMouse
	{
		get
		{
			if (CurrentSchemeIsController || CurrentSchemeIsTouch)
			{
				return false;
			}
			return Mouse.current != null;
		}
	}

	public bool MouseIsDragging => mouseIsDragging;

	private float dragDistanceThreshold => (float)Screen.height * 0.025f;

	private float dragTimeThreshold => 0.4f;

	private bool TouchesEnabled => true;

	public int InputCount => Inputs.Count;

	public ControlScheme CurrentScheme
	{
		get
		{
			if (SchemeOverride.HasValue)
			{
				return SchemeOverride.Value;
			}
			if (!_currentScheme.HasValue)
			{
				_currentScheme = GetSchemeFromName(PlayerInput.currentControlScheme);
			}
			return _currentScheme.Value;
		}
	}

	public bool CurrentSchemeIsController => CurrentScheme == ControlScheme.Controller;

	public bool CurrentSchemeIsMouseKeyboard => CurrentScheme == ControlScheme.KeyboardMouse;

	public bool CurrentSchemeIsTouch => CurrentScheme == ControlScheme.Touch;

	public event Action<ControlScheme> ControlSchemeChanged;

	private void Awake()
	{
		instance = this;
		SetupInputActions();
		EnhancedTouchSupport.Enable();
		Vibrator = new ControllerVibrator();
		InputSystem.pollingFrequency = 120f;
		InputUser.onChange += InputUser_onChange;
		InputSystem.onActionChange += delegate(object obj, InputActionChange change)
		{
			if (change == InputActionChange.ActionPerformed)
			{
				lastDevice = ((InputAction)obj).activeControl.device;
			}
		};
		if (Keyboard.current != null)
		{
			Keyboard.current.onTextInput += OnTextInput;
		}
	}

	private void OnTextInput(char c)
	{
		inputString += c;
	}

	private void SetupInputActions()
	{
		cancel = PlayerInput.actions["cancel"];
		submit = PlayerInput.actions["submit"];
		time_pause = PlayerInput.actions["time_pause"];
		pause = PlayerInput.actions["pause"];
		move = PlayerInput.actions["move"];
		snap_cards = PlayerInput.actions["snap_cards"];
		time_1 = PlayerInput.actions["time_1"];
		time_2 = PlayerInput.actions["time_2"];
		time_3 = PlayerInput.actions["time_3"];
		zoom = PlayerInput.actions["zoom"];
		panel_collapse = PlayerInput.actions["panel_collapse"];
		activate_ui = PlayerInput.actions["activate_ui"];
		time_toggle = PlayerInput.actions["time_toggle"];
		sell = PlayerInput.actions["sell"];
		toggle_inventory = PlayerInput.actions["toggle_inventory"];
		toggle_view = PlayerInput.actions["toggle_view"];
		grab = PlayerInput.actions["grab"];
		snap_move = PlayerInput.actions["snap_move"];
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			if (Keyboard.current != null)
			{
				InputSystem.ResetDevice(Keyboard.current);
			}
			ClearInputs();
			if (Vibrator != null)
			{
				Vibrator.StopVibrate();
			}
		}
	}

	private void OnDestroy()
	{
		if (Vibrator != null)
		{
			Vibrator.StopVibrate();
		}
		InputUser.onChange -= InputUser_onChange;
	}

	public void ClearInputs()
	{
		Inputs.Clear();
	}

	public void LogCurrentState()
	{
		string text = "Input controller state log\n";
		foreach (UserInput input in Inputs)
		{
			text = text + input.ToString() + "\n";
		}
		text += $"Active touches report: {Touch.activeTouches.Count} touches!";
		Debug.Log(text);
	}

	private ButtonControl GetMouseButton(int buttonId)
	{
		return buttonId switch
		{
			0 => Mouse.current.leftButton, 
			1 => Mouse.current.rightButton, 
			_ => Mouse.current.middleButton, 
		};
	}

	public Vector2 ClampedMousePosition()
	{
		if (Mouse.current == null)
		{
			return new Vector2(Screen.width, Screen.height) * 0.5f;
		}
		Vector2 result = Mouse.current.position.ReadValue();
		result.x = Mathf.Clamp(result.x, 0f, Screen.width);
		result.y = Mathf.Clamp(result.y, 0f, Screen.height);
		return result;
	}

	private bool MousePositionIsInScreen()
	{
		Vector2 vector = Mouse.current.position.ReadValue();
		if (vector.x > 0f && vector.x < (float)Screen.width && vector.y > 0f)
		{
			return vector.y < (float)Screen.height;
		}
		return false;
	}

	public Vector2 GetSafeTouchPosition(int i)
	{
		if (!GetInput(i))
		{
			return new Vector2(Screen.width, Screen.height) * 0.5f;
		}
		return GetInputPosition(i);
	}

	private void Update()
	{
		InputSystem.Update();
		Vibrator.UpdateVibrate(Time.unscaledDeltaTime);
		foreach (UserInput item in inputsToRemove)
		{
			Inputs.Remove(item);
		}
		inputsToRemove.Clear();
		foreach (UserInput input in Inputs)
		{
			input.JustStarted = false;
			input.UpdatedThisFrame = false;
		}
		if (Mouse.current != null)
		{
			int num = 0;
			if (AccessibilityScreen.ClickToDragEnabled)
			{
				num = 1;
			}
			int mouseId;
			for (mouseId = 0; mouseId <= num; mouseId++)
			{
				ButtonControl mouseButton = GetMouseButton(mouseId);
				if (mouseButton.wasPressedThisFrame)
				{
					if (MousePositionIsInScreen() && Inputs.Count <= 0)
					{
						mouseIsDragging = false;
						Inputs.Add(new UserInput
						{
							ScreenPosition = ClampedMousePosition(),
							MouseId = mouseId,
							JustStarted = true,
							StartPosition = ClampedMousePosition(),
							StartTime = Time.time,
							UpdatedThisFrame = true
						});
					}
					continue;
				}
				if (mouseButton.isPressed)
				{
					UserInput userInput = Inputs.FirstOrDefault((UserInput x) => x.MouseId == mouseId);
					if (userInput != null)
					{
						userInput.DeltaPosition = ClampedMousePosition() - userInput.ScreenPosition;
						userInput.ScreenPosition = ClampedMousePosition();
						userInput.UpdatedThisFrame = true;
						if (!mouseIsDragging && ((userInput.StartPosition - userInput.ScreenPosition).magnitude > dragDistanceThreshold || Time.time - userInput.StartTime >= dragTimeThreshold))
						{
							mouseIsDragging = true;
						}
					}
					continue;
				}
				for (int i = 0; i < Inputs.Count; i++)
				{
					if (Inputs[i].MouseId == mouseId)
					{
						Inputs[i].JustEnded = true;
						Inputs[i].UpdatedThisFrame = true;
						inputsToRemove.Add(Inputs[i]);
					}
				}
			}
		}
		else
		{
			mouseIsDragging = false;
		}
		if (Inputs.Count == 0)
		{
			mouseIsDragging = false;
		}
		if (TouchesEnabled)
		{
			for (int j = 0; j < Touch.activeTouches.Count; j++)
			{
				Touch touch = Touch.activeTouches[j];
				if (!touch.valid)
				{
					Debug.Log("Invalid touch in active touches!");
					continue;
				}
				if (touch.phase == TouchPhase.Began)
				{
					Inputs.Add(new UserInput
					{
						ScreenPosition = touch.screenPosition,
						TouchId = touch.touchId,
						JustStarted = true,
						StartPosition = touch.screenPosition,
						StartTime = Time.time,
						UpdatedThisFrame = true
					});
					continue;
				}
				if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
				{
					UserInput userInput2 = Inputs.FirstOrDefault((UserInput x) => x.TouchId == touch.touchId);
					if (userInput2 != null)
					{
						userInput2.DeltaPosition = touch.delta;
						userInput2.ScreenPosition = touch.screenPosition;
						userInput2.UpdatedThisFrame = true;
					}
					continue;
				}
				for (int k = 0; k < Inputs.Count; k++)
				{
					if (Inputs[k].TouchId == touch.touchId)
					{
						Inputs[k].JustEnded = true;
						Inputs[k].UpdatedThisFrame = true;
						inputsToRemove.Add(Inputs[k]);
					}
				}
			}
		}
		UpdatePen();
		for (int num2 = Inputs.Count - 1; num2 >= 0; num2--)
		{
			if (!Inputs[num2].UpdatedThisFrame)
			{
				Debug.Log("Removed a non-updated input!");
				Inputs.RemoveAt(num2);
			}
		}
		Cursor.visible = !CurrentSchemeIsController;
		if (lastControlScheme != CurrentScheme)
		{
			ClearBindingDisplayCache();
		}
		ActiveScheme = PlayerInput.currentControlScheme;
		lastControlScheme = CurrentScheme;
	}

	private void UpdatePen()
	{
		if (Pen.current == null)
		{
			return;
		}
		ButtonControl tip = Pen.current.tip;
		Vector2Control position = Pen.current.position;
		Vector2 vector = new Vector2(position.x.ReadValue(), position.y.ReadValue());
		if (tip.wasPressedThisFrame)
		{
			Inputs.Add(new UserInput
			{
				ScreenPosition = vector,
				PenId = Pen.current.deviceId,
				JustStarted = true,
				StartPosition = vector,
				StartTime = Time.time,
				UpdatedThisFrame = true
			});
			return;
		}
		if (tip.isPressed)
		{
			UserInput userInput = Inputs.FirstOrDefault((UserInput x) => x.PenId == Pen.current.deviceId);
			if (userInput != null)
			{
				userInput.DeltaPosition = vector - userInput.ScreenPosition;
				userInput.ScreenPosition = vector;
				userInput.UpdatedThisFrame = true;
			}
			return;
		}
		for (int i = 0; i < Inputs.Count; i++)
		{
			if (Inputs[i].PenId == Pen.current.deviceId)
			{
				Inputs[i].JustEnded = true;
				Inputs[i].UpdatedThisFrame = true;
				inputsToRemove.Add(Inputs[i]);
			}
		}
	}

	private void LateUpdate()
	{
		lastGrab = GetGrab();
		lastMove = GetMove();
		lastSnapMove = GetSnapMove();
		inputString = "";
		LastInputCount = InputCount;
		_currentScheme = null;
	}

	public Vector2 AllDeltaPos()
	{
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		if (InputCount == 0)
		{
			return Vector2.zero;
		}
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < InputCount; i++)
		{
			zero += GetDeltaPosition(i);
		}
		return zero / InputCount;
	}

	public bool GetInputBegan(int i)
	{
		if (GetInput(i))
		{
			return Inputs[i].JustStarted;
		}
		return false;
	}

	public bool GetRightMouseBegan()
	{
		if (GetInput(0) && Inputs[0].JustStarted)
		{
			return Inputs[0].MouseId == 1;
		}
		return false;
	}

	public bool GetLeftMouseBegan()
	{
		if (GetInput(0) && Inputs[0].JustStarted)
		{
			return Inputs[0].MouseId == 0;
		}
		return false;
	}

	public bool GetRightMouseEnded()
	{
		if (GetInput(0) && Inputs[0].JustEnded)
		{
			return Inputs[0].MouseId == 1;
		}
		return false;
	}

	public bool GetLeftMouseEnded()
	{
		if (GetInput(0) && Inputs[0].JustEnded)
		{
			return Inputs[0].MouseId == 0;
		}
		return false;
	}

	public bool GetInputTapped(int i)
	{
		UserInput userInput = Inputs[i];
		if (userInput.JustEnded && Time.time - userInput.StartTime <= 0.15f)
		{
			return (userInput.ScreenPosition - userInput.StartPosition).magnitude < (float)Screen.width * 0.05f;
		}
		return false;
	}

	public bool IsNotRightClick(int i)
	{
		return Inputs[i].MouseId != 1;
	}

	public bool GetInputEnded(int i)
	{
		if (GetInput(i))
		{
			return Inputs[i].JustEnded;
		}
		return false;
	}

	public bool GetInput(int i)
	{
		return Inputs.Count > i;
	}

	public bool GetInputMoving(int i)
	{
		if (Inputs.Count > i)
		{
			return Inputs[i].DeltaPosition != Vector2.zero;
		}
		return false;
	}

	public Vector2 GetDeltaPosition(int i)
	{
		return Inputs[i].DeltaPosition;
	}

	public Vector2 GetDeltaPositionSinceStart(int i)
	{
		return Inputs[i].ScreenPosition - Inputs[i].StartPosition;
	}

	public Vector2 GetInputPosition(int i)
	{
		return Inputs[i].ScreenPosition;
	}

	public Vector2 GetStartPosition(int i)
	{
		return Inputs[i].StartPosition;
	}

	public bool GetStickHorizontal()
	{
		if (!((double)move.ReadValue<Vector2>().x > 0.3))
		{
			return (double)move.ReadValue<Vector2>().x < -0.3;
		}
		return true;
	}

	public bool CancelTriggered()
	{
		return cancel.triggered;
	}

	public bool SubmitTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return submit.triggered;
	}

	public bool TimePauseTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_pause.triggered;
	}

	public bool PauseTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return pause.triggered;
	}

	public bool SnapCardsTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return snap_cards.triggered;
	}

	public bool Time1_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_1.triggered;
	}

	public bool Time2_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_2.triggered;
	}

	public bool Time3_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_3.triggered;
	}

	public float GetZoom()
	{
		if (DisableAllInput)
		{
			return 0f;
		}
		return zoom.ReadValue<float>();
	}

	public bool PanelCollapse_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return panel_collapse.triggered;
	}

	public bool ActivateUI_Triggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return activate_ui.triggered;
	}

	public bool TimeToggleTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return time_toggle.triggered;
	}

	public bool SellTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return sell.triggered;
	}

	public bool ToggleInventoryTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return toggle_inventory.triggered;
	}

	public bool ToggleViewTriggered()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return toggle_view.triggered;
	}

	public Vector2 GetMove()
	{
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		return move.ReadValue<Vector2>();
	}

	public Vector2 GetSnapMovePressed()
	{
		Vector2 snapMove = GetSnapMove();
		if (snapMove.magnitude == 0f)
		{
			return Vector2.zero;
		}
		return (snapMove - lastSnapMove).normalized;
	}

	public Vector2 GetSnapMove()
	{
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		return snap_move.ReadValue<Vector2>();
	}

	public float GetGrab()
	{
		if (DisableAllInput)
		{
			return 0f;
		}
		return grab.ReadValue<float>();
	}

	public Vector2 GetDeltaMove()
	{
		if (DisableAllInput)
		{
			return Vector2.zero;
		}
		return GetMove() - lastMove;
	}

	public bool StartedGrabbing()
	{
		if (DisableAllInput)
		{
			return false;
		}
		return grab.triggered;
	}

	public bool StoppedGrabbing()
	{
		if (DisableAllInput)
		{
			return false;
		}
		if (GetGrab() < 0.5f)
		{
			return lastGrab > 0.5f;
		}
		return false;
	}

	public string GetActionDisplayString(string name)
	{
		if (!bindingDisplayCache.ContainsKey(name))
		{
			bindingDisplayCache[name] = "[" + PlayerInput.actions[name].GetBindingDisplayString() + "]";
		}
		return bindingDisplayCache[name];
	}

	public void ClearBindingDisplayCache()
	{
		bindingDisplayCache.Clear();
	}

	public bool GetKeyDown(Key key)
	{
		if (Keyboard.current == null)
		{
			return false;
		}
		return Keyboard.current[key].wasPressedThisFrame;
	}

	public bool GetKey(Key key)
	{
		if (Keyboard.current == null)
		{
			return false;
		}
		return Keyboard.current[key].isPressed;
	}

	public bool AnyInputDone()
	{
		if (InputCount > 0 && GetInputTapped(0))
		{
			return true;
		}
		if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
		{
			return true;
		}
		if (SubmitTriggered())
		{
			return true;
		}
		return false;
	}

	private ControlScheme GetSchemeFromName(string scheme)
	{
		if (scheme == "Keyboard&Mouse")
		{
			return ControlScheme.KeyboardMouse;
		}
		if (scheme == "Gamepad")
		{
			return ControlScheme.Controller;
		}
		return ControlScheme.Touch;
	}

	private void InputUser_onChange(InputUser user, InputUserChange change, InputDevice device)
	{
		if (change == InputUserChange.ControlSchemeChanged)
		{
			ControlScheme schemeFromName = GetSchemeFromName(user.controlScheme.Value.name);
			this.ControlSchemeChanged?.Invoke(schemeFromName);
		}
	}

	public void LogDevices()
	{
		for (int i = 0; i < InputSystem.devices.Count; i++)
		{
			InputDevice inputDevice = InputSystem.devices[i];
			Debug.Log($"Device {i}\n" + "Display name: " + inputDevice.displayName + "\nInterface name: " + inputDevice.description.interfaceName + "\nDevice class: " + inputDevice.description.deviceClass + "\nProduct: " + inputDevice.description.product + "\n");
		}
	}
}
