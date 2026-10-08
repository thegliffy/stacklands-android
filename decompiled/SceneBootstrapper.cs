using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneBootstrapper : MonoBehaviour
{
	public List<GameBoard> Boards;

	public List<GameObject> ObjectsToInstantiate;

	private void Awake()
	{
		GameObject gameObject = new GameObject("Boards");
		foreach (GameBoard board in Boards)
		{
			GameBoard gameBoard = UnityEngine.Object.Instantiate(board);
			gameBoard.transform.SetParent(gameObject.transform, worldPositionStays: true);
			gameBoard.gameObject.name = board.gameObject.name;
		}
		GameObject gameObject2 = new GameObject("Managers");
		foreach (GameObject item in ObjectsToInstantiate)
		{
			GameObject gameObject3;
			try
			{
				gameObject3 = UnityEngine.Object.Instantiate(item);
			}
			catch (Exception exception)
			{
				Debug.LogError("Exception during scene bootstrapping:");
				Debug.LogException(exception);
				continue;
			}
			if (item.name.Contains("Manager") || item.name.Contains("Controller"))
			{
				gameObject3.transform.SetParent(gameObject2.transform, worldPositionStays: true);
			}
			gameObject3.name = item.name;
		}
		if (PlatformHelper.HasModdingSupport)
		{
			ModManager.instance.ReadyUpMods();
		}
	}
}
