using UnityEngine;

public class ImageEffect : MonoBehaviour
{
	public Material PostProcessingMaterial;

	public float Weight;

	private Material realMaterial;

	private void Awake()
	{
		realMaterial = new Material(PostProcessingMaterial);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		realMaterial.SetFloat("_Weight", Weight);
		Graphics.Blit(source, destination, realMaterial);
	}
}
