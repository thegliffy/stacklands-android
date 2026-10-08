using System.Collections.Generic;
using UnityEngine;

public class EditorSokBundle : ISokBundle
{
	private string id;

	public bool Load(string id)
	{
		this.id = id;
		return true;
	}

	public List<T> LoadAssets<T>() where T : Object
	{
		return new List<T>();
	}
}
