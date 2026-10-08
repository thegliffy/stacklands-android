using System.Collections.Generic;
using UnityEngine;

public interface ISokBundle
{
	bool Load(string id);

	List<T> LoadAssets<T>() where T : Object;
}
