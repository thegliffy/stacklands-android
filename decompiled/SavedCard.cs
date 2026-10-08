using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedCard
{
	public string UniqueId;

	public string ParentUniqueId;

	public string BoardId = "main";

	public string CardPrefabId = "";

	public Vector3 CardPosition;

	public bool FaceUp;

	public bool IsFoil;

	public bool IsDamaged;

	public CardDamageType DamageType;

	public List<ExtraCardData> ExtraCardData;

	public bool TimerRunning;

	public bool WithStatusBar;

	public float CurrentTimerTime;

	public float TargetTimerTime;

	public string TimerActionId;

	public string TimerBlueprintId;

	public bool SkipCitiesChecks;

	public int SubprintIndex;

	public string Status;

	public List<SavedStatusEffect> StatusEffects;

	public List<SavedCardConnector> CardConnectors;

	public string EquipmentHolderUniqueId;

	public string WorkerHolderUniqueId;

	public int WorkerIndex;
}
