using UnityEngine;

[CreateAssetMenu(fileName = "Debug Options", menuName = "ScriptableObjects/Debug Options")]
public class DebugOptions : ScriptableObject
{
	[Header("DLC")]
	public bool SpiritDlcEnabled;

	public bool CitiesDlcEnabled;

	public bool ModdingSupportEnabled = true;

	[Header("Gameplay")]
	public bool EndlessMoonEnabled;

	public bool NoFoodEnabled;

	public bool UnlockAllInCardopedia;

	public bool DontNeedVillagers;

	public bool NoEnergyEnabled;

	public bool DebugUnlockBoosters;

	public bool CursedFinished;

	public static DebugOptions Default => Resources.Load<DebugOptions>("DebugOptions");
}
