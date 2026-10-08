using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoosterAddition
{
	[SerializeReference]
	public VariableFilter Filter;

	public List<CardBag> CardBags;
}
