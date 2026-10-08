using System.Collections.Generic;
using UnityEngine;

public class CreatePackLine : MonoBehaviour
{
	public float Distance;

	public float TotalWidth;

	public void CreateBoosterBoxes(List<string> boosters, BoardCurrency currency)
	{
		Object.Instantiate(PrefabManager.instance.SellBoxPrefab).transform.SetParent(base.transform, worldPositionStays: true);
		foreach (string booster in boosters)
		{
			BoosterpackData boosterData = WorldManager.instance.GetBoosterData(booster);
			if (!(boosterData == null))
			{
				BuyBoosterBox buyBoosterBox = Object.Instantiate(PrefabManager.instance.BoosterBoxPrefab);
				buyBoosterBox.BoosterId = booster;
				buyBoosterBox.Cost = boosterData.Cost;
				buyBoosterBox.BoardCurrency = currency;
				buyBoosterBox.transform.SetParent(base.transform, worldPositionStays: true);
				WorldManager.instance.AllBoosterBoxes.Add(buyBoosterBox);
			}
		}
		SetPositions();
		TotalWidth = (float)boosters.Count * Distance + 0.375f;
	}

	private void SetPositions()
	{
		int num = 0;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			if (base.transform.GetChild(i).gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		int num2 = 0;
		for (int j = 0; j < base.transform.childCount; j++)
		{
			Transform child = base.transform.GetChild(j);
			if (child.gameObject.activeInHierarchy)
			{
				float x = (float)num2 * Distance - (float)(num - 1) * Distance * 0.5f;
				child.localPosition = new Vector3(x, 0f, 0f);
				num2++;
			}
		}
	}
}
