using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
	public string Id = "";

	public BoxCollider WorldCollider;

	public BoxCollider TightWorldCollider;

	public Transform TopBgElements;

	public Transform BottomBgElements;

	public Transform LeftBgElements;

	public Transform RightBgElements;

	public Transform CameraIntroPosition;

	public Color CardHighlightColor;

	[HideInInspector]
	public TextAsset BoardPreset;

	[HideInInspector]
	public float WorldSizeIncrease;

	private float PreviousWorldSizeIncrease;

	public BoardBackground boardBackground;

	public List<string> BoosterIds;

	[HideInInspector]
	public float PackLineWidth;

	public BoardOptions BoardOptions;

	private Location _location;

	private bool locationSet;

	private Bounds cachedTightBounds;

	private bool hasCachedTightBounds;

	[HideInInspector]
	public Material MyMaterial;

	public Location Location
	{
		get
		{
			if (!locationSet)
			{
				locationSet = true;
				if (Id == "forest")
				{
					_location = Location.Forest;
				}
				else if (Id == "main")
				{
					_location = Location.Mainland;
				}
				else if (Id == "island")
				{
					_location = Location.Island;
				}
				else if (Id == "happiness")
				{
					_location = Location.Happiness;
				}
				else if (Id == "greed")
				{
					_location = Location.Greed;
				}
				else if (Id == "death")
				{
					_location = Location.Death;
				}
				else
				{
					if (!(Id == "cities"))
					{
						throw new ArgumentException();
					}
					_location = Location.Cities;
				}
			}
			return _location;
		}
	}

	public bool IsCurrent => WorldManager.instance.CurrentBoard == this;

	public Bounds WorldBounds
	{
		get
		{
			WorldCollider.ToWorldSpaceBox(out var center, out var halfExtents, out var _);
			return new Bounds(center, halfExtents * 2f + WorldSizeIncrease * new Vector3(1f, 0f, 0.58f) * 2f);
		}
	}

	public Bounds TightWorldBounds
	{
		get
		{
			if (!hasCachedTightBounds)
			{
				TightWorldCollider.ToWorldSpaceBox(out var center, out var halfExtents, out var _);
				cachedTightBounds = new Bounds(center, halfExtents * 2f + WorldSizeIncrease * new Vector3(1f, 0f, 0.58f) * 2f);
				hasCachedTightBounds = true;
			}
			return cachedTightBounds;
		}
	}

	public string BoardName => SokLoc.Translate("board_" + Id + "_name");

	private void Awake()
	{
		if (BoardOptions.PostProcessVolume != null)
		{
			BoardOptions.PostProcessVolume.enabled = false;
		}
		MyMaterial = GetComponent<MeshRenderer>().sharedMaterial;
	}

	private void Start()
	{
		CreatePackLine componentInChildren = GetComponentInChildren<CreatePackLine>();
		if (componentInChildren != null)
		{
			componentInChildren.CreateBoosterBoxes(BoosterIds, BoardOptions.Currency);
			PackLineWidth = componentInChildren.TotalWidth;
		}
	}

	private void Update()
	{
		if (IsCurrent)
		{
			Shader.SetGlobalFloat("_WorldSizeIncrease", WorldSizeIncrease);
		}
		float b = WorldManager.instance.DetermineTargetWorldSize(this);
		WorldSizeIncrease = Mathf.Lerp(WorldSizeIncrease, b, Time.deltaTime * 12f);
		if (WorldSizeIncrease != PreviousWorldSizeIncrease && boardBackground != null)
		{
			boardBackground.UpdateBoardBackground();
		}
		TopBgElements.localPosition = Vector3.forward * WorldSizeIncrease * 0.58f;
		BottomBgElements.localPosition = Vector3.back * WorldSizeIncrease * 0.58f;
		LeftBgElements.localPosition = Vector3.left * WorldSizeIncrease;
		RightBgElements.localPosition = Vector3.right * WorldSizeIncrease;
		PreviousWorldSizeIncrease = WorldSizeIncrease;
		if (BoardOptions.PostProcessVolume != null)
		{
			BoardOptions.PostProcessVolume.enabled = IsCurrent;
		}
		hasCachedTightBounds = false;
	}

	public Vector3 NormalizedPosToWorldPos(Vector2 pos)
	{
		Bounds worldBounds = WorldBounds;
		float x = Mathf.Lerp(worldBounds.min.x, worldBounds.max.x, pos.x);
		float z = Mathf.Lerp(worldBounds.min.z, worldBounds.max.z, pos.y);
		return new Vector3(x, 0f, z);
	}

	public Vector2 WorldPosToNormalizedPos(Vector3 pos)
	{
		Bounds worldBounds = WorldBounds;
		float x = Mathf.InverseLerp(worldBounds.min.x, worldBounds.max.x, pos.x);
		float y = Mathf.InverseLerp(worldBounds.min.z, worldBounds.max.z, pos.z);
		return new Vector2(x, y);
	}

	public Vector3 MiddleOfBoard()
	{
		return NormalizedPosToWorldPos(new Vector2(0.5f, 0.5f));
	}
}
