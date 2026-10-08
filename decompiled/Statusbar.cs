using UnityEngine;

public class Statusbar : MonoBehaviour
{
	public float FillAmount;

	public float StatusTime;

	public bool DestroyMe;

	public GameCard ParentCard;

	public Vector3 Offset;

	public Vector3 ExtraOffset;

	public MeshRenderer Renderer;

	private MaterialPropertyBlock propBlock;

	private Vector3 startScale;

	private Vector3 hiddenScale;

	public bool Paused;

	private float blinkTimer;

	private bool blink;

	private void Start()
	{
		startScale = base.transform.localScale;
		hiddenScale = startScale;
		hiddenScale.y = 0f;
		base.transform.localScale = hiddenScale;
		propBlock = new MaterialPropertyBlock();
		Renderer.GetPropertyBlock(propBlock);
	}

	private void Update()
	{
		float fillAmount = Mathf.Clamp01(ParentCard.CurrentTimerTime / ParentCard.TargetTimerTime);
		if (Paused)
		{
			blinkTimer += Time.deltaTime;
			if (blinkTimer >= 0.3f)
			{
				blink = !blink;
				blinkTimer = 0f;
			}
		}
		if (ParentCard != null && ParentCard.CurrentStatusbar != this)
		{
			DestroyMe = true;
		}
		FillAmount = fillAmount;
		propBlock.SetFloat("_FillAmount", FillAmount);
		propBlock.SetFloat("_Pause", (Paused && blink) ? 1f : 0f);
		Renderer.SetPropertyBlock(propBlock);
		Vector3 b;
		if (DestroyMe)
		{
			b = hiddenScale;
			if (base.transform.localScale.y < 0.01f)
			{
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			b = startScale;
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, b, Time.deltaTime * 22f);
		if (ParentCard != null)
		{
			Vector3 zero = Vector3.zero;
			GameCard rootCard = ParentCard.GetRootCard();
			if (rootCard.StatusEffectElements.Count > 0)
			{
				zero += ExtraOffset;
			}
			base.transform.position = rootCard.transform.position + Offset + zero;
		}
		else
		{
			DestroyMe = true;
		}
	}
}
