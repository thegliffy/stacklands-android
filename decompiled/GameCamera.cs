using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameCamera : MonoBehaviour
{
	public static GameCamera instance;

	public float MoveSpeed = 1f;

	public float ZoomSpeed = 1f;

	public float MinZoom = 3f;

	public float MaxZoom = 11f;

	public Vector3 cameraStartPosition;

	public Vector3 cameraTargetPosition;

	public float CameraMoveSpeed = 12f;

	private Vector2 startInputPosition;

	private Vector3 startPosition;

	private float startTime;

	private bool isTouchDraggingCard;

	private Vector3 touchCameraStartPosition;

	private Vector3 zoomStartPosition;

	private float startZoom;

	private float startDist;

	private float prevInputCount;

	private bool isDraggingCamera;

	private Vector3? _targetPositionOverride;

	private IGameCardOrCardData _targetCardOverride;

	public float? CameraPositionDistanceOverride;

	public PostProcessVolume PauseVolume;

	public PostProcessVolume FocusVolume;

	public PostProcessVolume SpiritVolume;

	public PostProcessVolume EnergyVolume;

	public ImageEffect SpiritImageEffect;

	public ImageEffect SpiritTransitionEffect;

	public Material SpiritBackgroundMaterial;

	[HideInInspector]
	public Material TempSpiritBackgroundMaterial;

	public Vector3 GameStartCameraPosition;

	public Camera MyCam;

	public float Screenshake;

	private Draggable currentlySelectedDraggable;

	private float spiritEffectStrength;

	public bool IsDragging => isDraggingCamera;

	public Vector3? TargetPositionOverride
	{
		get
		{
			return _targetPositionOverride;
		}
		set
		{
			_targetCardOverride = null;
			_targetPositionOverride = value;
		}
	}

	public IGameCardOrCardData TargetCardOverride
	{
		get
		{
			if (_targetCardOverride is CardData cardData && cardData.MyGameCard == null)
			{
				_targetCardOverride = null;
				return null;
			}
			return _targetCardOverride;
		}
		set
		{
			_targetPositionOverride = null;
			_targetCardOverride = value;
		}
	}

	private bool canControlCamera
	{
		get
		{
			if (WorldManager.instance.IsPlaying && !TargetPositionOverride.HasValue && WorldManager.instance.IntroPack == null)
			{
				return !GameScreen.instance.ControllerIsInUI;
			}
			return false;
		}
	}

	public void CenterOnBoard(GameBoard board)
	{
		Vector3 vector2 = (base.transform.position = board.MiddleOfBoard() + GameStartCameraPosition);
		cameraTargetPosition = vector2;
	}

	public void KeepCameraAtCurrentPos()
	{
		cameraTargetPosition = base.transform.position;
	}

	private void Awake()
	{
		instance = this;
		TempSpiritBackgroundMaterial = new Material(SpiritBackgroundMaterial);
	}

	private Transform GetIntroCameraTransform()
	{
		GameBoard gameBoard = (WorldManager.instance.IsCitiesDlcActive() ? WorldManager.instance.GetBoardWithId("cities") : ((!WorldManager.instance.IsSpiritDlcActive()) ? WorldManager.instance.GetBoardWithId("main") : WorldManager.instance.GetBoardWithId("death")));
		return gameBoard.CameraIntroPosition;
	}

	private void Start()
	{
		base.transform.position = GetIntroCameraTransform().position;
		cameraStartPosition = base.transform.position;
		cameraTargetPosition = base.transform.position;
	}

	public Vector3 ScreenPosToWorldPos(Vector3 p)
	{
		Ray ray = MyCam.ScreenPointToRay(p);
		new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var enter);
		return ray.origin + ray.direction * enter;
	}

	public Vector3 ScreenPosToWorldPos(Vector2 pos, Vector3 camPos)
	{
		Vector3 position = MyCam.transform.position;
		MyCam.transform.position = camPos;
		Vector3 result = ScreenPosToWorldPos(pos);
		MyCam.transform.position = position;
		return result;
	}

	public void StartDragging()
	{
		if (canControlCamera && InputController.instance.InputCount != 2)
		{
			startInputPosition = InputController.instance.GetInputPosition(0);
			startPosition = base.transform.position;
			startTime = Time.time;
			isDraggingCamera = true;
		}
	}

	private Draggable FindNextDraggableInDirection(Vector3 curPos, Vector3 wantedDir)
	{
		float num = float.MinValue;
		Draggable result = null;
		foreach (Draggable allDraggable in WorldManager.instance.AllDraggables)
		{
			if (!allDraggable.gameObject.activeInHierarchy || allDraggable == currentlySelectedDraggable || allDraggable == WorldManager.instance.DraggingDraggable)
			{
				continue;
			}
			if (allDraggable.MyBoard == null)
			{
				Debug.Log(allDraggable?.ToString() + " does not have a board");
			}
			else
			{
				if (!allDraggable.MyBoard.IsCurrent || !allDraggable.CanBeAutoMovedTo)
				{
					continue;
				}
				Vector3 rhs = allDraggable.AutoMoveSnapPosition - curPos;
				float num2 = Vector3.Dot(wantedDir, rhs);
				if (!(num2 <= 0f))
				{
					float num3 = num2 / rhs.sqrMagnitude;
					if (num3 > num)
					{
						num = num3;
						result = allDraggable;
					}
				}
			}
		}
		return result;
	}

	private Draggable FindNextDraggable(Vector2 snapMoveInput)
	{
		return FindNextDraggableInDirection(WorldManager.instance.mouseWorldPosition, new Vector3(snapMoveInput.x, 0f, snapMoveInput.y));
	}

	private void Update()
	{
		Screenshake -= Time.deltaTime;
		Vector3 vector;
		if (Screenshake > 0f && AccessibilityScreen.ScreenshakeEnabled)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			vector = Screenshake * (base.transform.right * insideUnitCircle.x + base.transform.up * insideUnitCircle.y);
		}
		else
		{
			vector = Vector3.zero;
		}
		if (isDraggingCamera && !canControlCamera)
		{
			isDraggingCamera = false;
		}
		if (isDraggingCamera && InputController.instance.InputCount == 2)
		{
			isDraggingCamera = false;
		}
		Vector2 move = InputController.instance.GetMove();
		Vector3 vector2 = new Vector3(move.x, 0f, move.y);
		Vector2 snapMoveInput = InputController.instance.GetSnapMovePressed();
		if (!canControlCamera)
		{
			vector2 = Vector3.zero;
			snapMoveInput = Vector2.zero;
		}
		if (snapMoveInput.magnitude > 0f)
		{
			if (WorldManager.instance.DraggingCard != null)
			{
				WorldManager.instance.grabOffset = WorldManager.instance.DraggingCard.CardNameText.transform.position - WorldManager.instance.DraggingCard.transform.position;
			}
			Draggable draggable = FindNextDraggable(snapMoveInput);
			if (draggable != null)
			{
				currentlySelectedDraggable = draggable;
				cameraTargetPosition = draggable.AutoMoveSnapPosition - GetCurrentGroundOffset();
			}
		}
		if (vector2.magnitude > 0.01f)
		{
			currentlySelectedDraggable = null;
		}
		cameraTargetPosition += vector2 * Time.deltaTime * (MoveSpeed + Mathf.Clamp(cameraTargetPosition.y / 2f - 4f, 0f, 10f));
		bool flag = canControlCamera;
		if (InputController.instance.CurrentSchemeIsMouseKeyboard && GameCanvas.instance.MousePositionIsOverUI())
		{
			flag = false;
		}
		if (WorldManager.instance.InAnimation && !WorldManager.instance.CutsceneBoardView)
		{
			flag = false;
		}
		if (InputController.instance.InputCount == 2)
		{
			Vector2 inputPosition = InputController.instance.GetInputPosition(0);
			Vector2 inputPosition2 = InputController.instance.GetInputPosition(1);
			float num = Vector2.Distance(inputPosition, inputPosition2);
			if (prevInputCount != 2f && flag)
			{
				Debug.Log("Started zooming");
				touchCameraStartPosition = base.transform.position;
				zoomStartPosition = Vector2.Lerp(inputPosition, inputPosition2, 0.5f);
				startZoom = base.transform.position.y;
				startDist = num;
			}
			if (flag && num > float.Epsilon)
			{
				Vector2 vector3 = Vector2.Lerp(inputPosition, inputPosition2, 0.5f);
				Vector3 vector4 = ScreenPosToWorldPos(vector3) - ScreenPosToWorldPos(zoomStartPosition);
				cameraTargetPosition = touchCameraStartPosition - vector4;
				Vector3 vector5 = ScreenPosToWorldPos(vector3, cameraTargetPosition);
				float max = MaxZoom + WorldManager.instance.CurrentBoard.WorldSizeIncrease * 2.7f;
				cameraTargetPosition.y = Mathf.Clamp(startZoom * (startDist / num), MinZoom, max);
				Vector3 vector6 = ScreenPosToWorldPos(vector3, cameraTargetPosition);
				Vector3 vector7 = vector5 - vector6;
				vector7.y = 0f;
				cameraTargetPosition += vector7;
				base.transform.position = cameraTargetPosition;
			}
		}
		if (flag && !InputController.instance.CurrentSchemeIsTouch)
		{
			float num2 = InputController.instance.GetZoom() * 0.2f;
			Vector2 pos = InputController.instance.ClampedMousePosition();
			if (InputController.instance.CurrentSchemeIsController)
			{
				pos = new Vector2(Screen.width, Screen.height) * 0.5f;
			}
			Vector3 vector8 = ScreenPosToWorldPos(pos, cameraTargetPosition);
			vector8.y = cameraTargetPosition.y;
			float y = cameraTargetPosition.y;
			cameraTargetPosition.y += ZoomSpeed * num2;
			float max2 = MaxZoom + WorldManager.instance.CurrentBoard.WorldSizeIncrease * 2.7f;
			cameraTargetPosition.y = Mathf.Clamp(cameraTargetPosition.y, MinZoom, max2);
			if (y != cameraTargetPosition.y && Mathf.Abs(num2) > 0.0001f)
			{
				Vector3 vector9 = ScreenPosToWorldPos(pos, cameraTargetPosition);
				Vector3 vector10 = vector8 - vector9;
				vector10.y = 0f;
				cameraTargetPosition += vector10;
			}
		}
		if ((bool)WorldManager.instance.DraggingCard)
		{
			if (InputController.instance.CurrentSchemeIsTouch)
			{
				isTouchDraggingCard = true;
			}
		}
		else
		{
			isTouchDraggingCard = false;
		}
		if (isTouchDraggingCard)
		{
			float num3 = 0.5f * cameraTargetPosition.y;
			if (InputController.instance.GetInputPosition(0).y >= (float)Screen.height * 0.8f)
			{
				float num4 = Mathf.InverseLerp((float)Screen.height * 0.8f, Screen.height, InputController.instance.GetInputPosition(0).y) * num3;
				cameraTargetPosition += Vector3.forward * Time.deltaTime * num4;
			}
			else if (InputController.instance.GetInputPosition(0).y <= (float)Screen.height * 0.2f)
			{
				float num5 = Mathf.InverseLerp((float)Screen.height * 0.2f, 0f, InputController.instance.GetInputPosition(0).y) * num3;
				cameraTargetPosition += Vector3.back * Time.deltaTime * num5;
			}
			if (InputController.instance.GetInputPosition(0).x >= (float)Screen.width * 0.8f)
			{
				float num6 = Mathf.InverseLerp((float)Screen.width * 0.8f, Screen.width, InputController.instance.GetInputPosition(0).x) * num3;
				cameraTargetPosition += Vector3.right * Time.deltaTime * num6;
			}
			else if (InputController.instance.GetInputPosition(0).x <= (float)Screen.width * 0.2f)
			{
				float num7 = Mathf.InverseLerp((float)Screen.width * 0.2f, 0f, InputController.instance.GetInputPosition(0).x) * num3;
				cameraTargetPosition += Vector3.left * Time.deltaTime * num7;
			}
		}
		if (isDraggingCamera && InputController.instance.GetInput(0))
		{
			Vector3 vector11 = ScreenPosToWorldPos(InputController.instance.GetInputPosition(0)) - ScreenPosToWorldPos(startInputPosition);
			Vector3 vector13 = (base.transform.position = startPosition - vector11);
			cameraTargetPosition = vector13;
		}
		if (isDraggingCamera && InputController.instance.GetInputEnded(0))
		{
			Vector2 vector14 = startInputPosition - InputController.instance.GetInputPosition(0);
			if (Time.time - startTime < 0.2f && vector14.magnitude <= 5f)
			{
				Clicked();
			}
			isDraggingCamera = false;
		}
		Vector3 vector15 = cameraTargetPosition;
		bool flag2 = true;
		bool flag3 = TargetPositionOverride.HasValue || TargetCardOverride != null;
		Vector3? vector16 = TargetPositionOverride;
		if (TargetCardOverride != null)
		{
			vector16 = TargetCardOverride.Position;
		}
		if (WorldManager.instance.IntroPack != null)
		{
			vector16 = WorldManager.instance.IntroPack.transform.position;
			cameraTargetPosition = base.transform.position;
			flag3 = true;
		}
		if (vector16.HasValue)
		{
			Vector3 value = vector16.Value;
			value.y = 0.01f;
			float num8 = (CameraPositionDistanceOverride.HasValue ? CameraPositionDistanceOverride.Value : 7f);
			vector15 = value - base.transform.forward * num8;
			flag2 = false;
		}
		if (WorldManager.instance.CurrentGameState == WorldManager.GameState.InMenu)
		{
			vector15 = (cameraTargetPosition = GetIntroCameraTransform().position);
			flag2 = false;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, vector15 + vector * 0.5f, Time.deltaTime * CameraMoveSpeed);
		if (flag2)
		{
			base.transform.position = ClampPos(base.transform.position);
			cameraTargetPosition = ClampPos(cameraTargetPosition);
		}
		PauseVolume.enabled = WorldManager.instance.SpeedUp == 0f && !WorldManager.instance.InAnimation;
		PauseVolume.gameObject.SetActive(WorldManager.instance.SpeedUp == 0f && !WorldManager.instance.InAnimation);
		if (WorldManager.instance.currentAnimation != null || WorldManager.instance.currentAnimationRoutine != null)
		{
			flag3 = true;
		}
		if (WorldManager.instance.CurrentGameState != 0)
		{
			flag3 = true;
		}
		FocusVolume.weight = Mathf.Lerp(FocusVolume.weight, flag3 ? 1f : 0f, Time.deltaTime * 16f);
		if (Screenshotter.instance != null && Screenshotter.instance.IsScreenshotting)
		{
			FocusVolume.weight = 0f;
		}
		if (WorldManager.instance.CurrentBoard != null)
		{
			MyCam.backgroundColor = WorldManager.instance.CurrentBoard.MyMaterial.GetColor("_Color");
		}
		UpdateSpiritEffect();
		prevInputCount = InputController.instance.InputCount;
	}

	public void OnRestartGame()
	{
		base.transform.position = (cameraTargetPosition = GetIntroCameraTransform().position);
	}

	private void UpdateSpiritEffect()
	{
		bool flag = false;
		bool inAnimation = WorldManager.instance.InAnimation;
		if (WorldManager.instance.GetCard<Spirit>() != null)
		{
			flag = true;
		}
		spiritEffectStrength = Mathf.Lerp(spiritEffectStrength, (flag && inAnimation) ? 1f : 0f, Time.deltaTime * 4f);
		SpiritVolume.weight = spiritEffectStrength;
		SpiritImageEffect.Weight = spiritEffectStrength;
		Color color = TempSpiritBackgroundMaterial.color;
		color.a = spiritEffectStrength * 0.5f;
		TempSpiritBackgroundMaterial.color = color;
		if (TransitionScreen.instance.CurrentTransitionType.Id == "spirit")
		{
			SpiritTransitionEffect.Weight = TransitionScreen.instance.TransitionAmount;
		}
		else
		{
			SpiritTransitionEffect.Weight = 0f;
		}
	}

	public void Clicked()
	{
		WorldManager.instance.CloseOpenInventories();
	}

	private Vector3 GetCurrentGroundOffset()
	{
		Ray ray;
		return WorldManager.instance.ScreenPosToWorldPos(new Vector2(Screen.width, Screen.height) * 0.5f, out ray) - base.transform.position;
	}

	private Vector3 ClampPos(Vector3 p)
	{
		Bounds worldBounds = WorldManager.instance.CurrentBoard.WorldBounds;
		float num = Vector3.Dot(GetCurrentGroundOffset(), Vector3.forward);
		float num2 = 0.2f;
		p.x = Mathf.Clamp(p.x, worldBounds.min.x - num2, worldBounds.max.x + num2);
		p.z = Mathf.Clamp(p.z, worldBounds.min.z - num - num2, worldBounds.max.z - num + num2);
		return p;
	}
}
