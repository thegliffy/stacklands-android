using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
public class BoardOptions
{
	[Header("General")]
	public float BaseBoardSize = 2f;

	public int BaseCardCount = 20;

	public bool FoodSpoils;

	public BoardCurrency Currency;

	[Term]
	public string SellBoxTerm = "label_sell";

	[Term]
	public string SellBoxDescription = "label_sellbox_description";

	public bool IsSpiritWorld;

	public bool ResetItemDrops = true;

	public bool UsesShells;

	public SetCardBagType FallbackBag = SetCardBagType.BasicHarvestable;

	public PostProcessVolume PostProcessVolume;

	public CardPalette CardBackgroundPallete;

	public Color ConnectorColor;

	[Header("Special Pack Spawns")]
	public bool NewVillagerSpawnsFromPack;

	public bool CanSpawnCombatIntro;

	[Header("Special Events")]
	public bool CanSpawnPortals;

	public bool CanSpawnPirateBoat;

	public bool CanSpawnTravellingCart;

	public bool CanSpawnShaman;

	[Header("Travel")]
	public bool CanTravelToForest;

	public bool CanTravelToIsland;

	[Header("Audio")]
	public AudioClip Ambience;

	public float AmbienceVolume = 0.15f;

	public List<AudioClipWithVolume> Songs;
}
