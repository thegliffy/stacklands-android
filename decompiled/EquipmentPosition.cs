using UnityEngine;

public class EquipmentPosition : MonoBehaviour
{
	public SpriteRenderer ShadowRenderer;

	public SpriteRenderer IconRenderer;

	private Vector3 startOffset;

	private GameCard parentCard;

	private float alpha;

	public bool IsWorkerPosition;

	private float timer;

	private void Awake()
	{
		parentCard = GetComponentInParent<GameCard>();
		startOffset = base.transform.localPosition;
		timer = Random.Range(0f, 10f);
		SpriteRenderer shadowRenderer = ShadowRenderer;
		Color color2 = (IconRenderer.color = new Color(1f, 1f, 1f, 0f));
		shadowRenderer.color = color2;
		SpriteRenderer shadowRenderer2 = ShadowRenderer;
		bool flag2 = (IconRenderer.enabled = false);
		shadowRenderer2.enabled = flag2;
	}

	private void Update()
	{
		if (!(parentCard.MyBoard == null) && parentCard.MyBoard.IsCurrent)
		{
			timer += Time.deltaTime * WorldManager.instance.TimeScale;
			alpha = Mathf.Lerp(alpha, (parentCard.ShowInventory && ((parentCard.IsWorkerInventory && IsWorkerPosition) || (!parentCard.IsWorkerInventory && !IsWorkerPosition))) ? 1f : 0f, Time.deltaTime * 20f);
			SpriteRenderer shadowRenderer = ShadowRenderer;
			Color color2 = (IconRenderer.color = new Color(1f, 1f, 1f, alpha));
			shadowRenderer.color = color2;
			SpriteRenderer shadowRenderer2 = ShadowRenderer;
			bool flag2 = (IconRenderer.enabled = alpha > 0.01f);
			shadowRenderer2.enabled = flag2;
			float x = timer * 0.5f;
			base.transform.localPosition = startOffset + new Vector3(Perlin(x, 0.2f), Perlin(x, 0.6f)) * 0.01f;
		}
	}

	private float Perlin(float x, float y)
	{
		return Mathf.PerlinNoise(x, y) * 2f - 1f;
	}
}
