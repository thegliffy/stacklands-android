using System.Collections.Generic;
using UnityEngine;

public class WorkerTransformHolder : MonoBehaviour
{
	private List<GameObject> workers = new List<GameObject>();

	public GameObject WorkerPositionPrefab;

	public GameObject WorkerPositionMiddle;

	public float WorkerAmountOffset = 0.1f;

	private void Update()
	{
	}

	public Transform GetTransformAtIndex(int index)
	{
		return workers[Mathf.Clamp(index, 0, workers.Count - 1)].transform;
	}

	public void UpdateWorkerAmount(int workerAmount)
	{
		foreach (GameObject worker in workers)
		{
			Object.Destroy(worker.gameObject);
		}
		workers.Clear();
		for (int i = 0; i < workerAmount; i++)
		{
			Vector3 position = WorkerPositionMiddle.transform.position + new Vector3((float)i * WorkerAmountOffset - (float)(workerAmount / 2) * WorkerAmountOffset + WorkerAmountOffset / 2f * ((workerAmount % 2 == 0) ? 1f : 0f), 0f, 0f);
			GameObject item = Object.Instantiate(WorkerPositionPrefab, position, WorkerPositionMiddle.transform.rotation, base.transform);
			workers.Add(item);
		}
	}
}
