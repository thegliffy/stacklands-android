using System;
using System.Collections;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
	private MeshRenderer myRenderer;

	private Material[] startMaterials;

	private Material[] currentMaterials;

	public void Init()
	{
		if (!(myRenderer != null))
		{
			myRenderer = GetComponent<MeshRenderer>();
			if (myRenderer == null)
			{
				Debug.Log(base.gameObject.name + " does not have a MeshRenderer");
			}
			startMaterials = myRenderer.sharedMaterials;
			currentMaterials = myRenderer.sharedMaterials;
		}
	}

	private void Awake()
	{
		Init();
	}

	public void SetMaterial(Material mat)
	{
		if (myRenderer == null)
		{
			return;
		}
		for (int i = 0; i < currentMaterials.Length; i++)
		{
			if (!(currentMaterials[i].name == "Invisible"))
			{
				currentMaterials[i] = mat;
			}
		}
		myRenderer.sharedMaterials = currentMaterials;
	}

	public void SetMaterialForTime(Material mat, float time, Action afterAction = null)
	{
		if (base.gameObject.activeInHierarchy)
		{
			SetMaterial(mat);
			StartCoroutine(ResetAfter(time, afterAction));
		}
	}

	private IEnumerator ResetAfter(float t, Action action)
	{
		yield return new WaitForSeconds(t);
		ResetMaterials();
		action?.Invoke();
	}

	public void ResetMaterials()
	{
		if (!(myRenderer == null))
		{
			myRenderer.sharedMaterials = startMaterials;
		}
	}
}
