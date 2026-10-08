using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedConflict
{
	public string Id;

	public List<string> InvolvedCards;

	public string InitiatorCardId;

	public Vector3 StartPosition;
}
