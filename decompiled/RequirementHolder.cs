using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RequirementHolder
{
	[SerializeReference]
	public List<CardRequirement> CardRequirements = new List<CardRequirement>();

	[SerializeReference]
	public List<CardRequirementResult> PositiveResults = new List<CardRequirementResult>();

	[SerializeReference]
	public List<CardRequirementResult> NegativeResults = new List<CardRequirementResult>();
}
