using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableCutscene", menuName = "ScriptableObjects/ScriptableCutscene", order = 1)]
public class ScriptableCutscene : ScriptableObject
{
	public string CutsceneId;

	public bool IsAdvisorCutscene;

	public bool AdvisorWarning;

	[SerializeReference]
	public List<CutsceneStep> CutsceneSteps = new List<CutsceneStep>();

	public bool IsCitiesCutscene => CutsceneId.StartsWith("cities");
}
