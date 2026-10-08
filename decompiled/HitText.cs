using TMPro;
using UnityEngine;

public class HitText : MonoBehaviour
{
	public TextMeshPro TextMesh;

	[HideInInspector]
	public Combatable TargetCombatable;

	public bool IsMiss;

	public SpriteRenderer BackgroundRenderer;

	private float timer;

	private Vector3 startScale;

	public float InitialScaleUp = 2f;

	public TextMeshPro VeryEffectiveText;

	public bool IsVeryEffective;

	private void Awake()
	{
		startScale = base.transform.localScale;
		base.transform.localScale *= InitialScaleUp;
	}

	public void SetVeryEffective(bool veryEffective)
	{
		IsVeryEffective = veryEffective;
		if (IsVeryEffective)
		{
			VeryEffectiveText = Object.Instantiate(PrefabManager.instance.IsVeryEffectiveText);
		}
	}

	private void Start()
	{
		SetPosition();
	}

	private void OnDestroy()
	{
		if (VeryEffectiveText != null)
		{
			Object.Destroy(VeryEffectiveText.gameObject);
		}
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (TargetCombatable.CurrentHitText != this || TargetCombatable == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (timer >= 0.8f)
		{
			float a = TextMesh.color.a;
			Color color = TextMesh.color;
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 12f);
			TextMesh.color = color;
			if (BackgroundRenderer != null)
			{
				color = BackgroundRenderer.color;
				color.a = a;
				BackgroundRenderer.color = color;
			}
			if (VeryEffectiveText != null)
			{
				color = VeryEffectiveText.color;
				color.a = a;
				VeryEffectiveText.color = color;
			}
			if (color.a < 0.01f)
			{
				Object.Destroy(base.gameObject);
			}
		}
		SetPosition();
		base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, 0f);
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, startScale, Time.deltaTime * 10f);
		if (VeryEffectiveText != null)
		{
			VeryEffectiveText.transform.localScale = Vector3.Lerp(VeryEffectiveText.transform.localScale, Vector3.one, Time.deltaTime * 10f);
		}
	}

	private void SetPosition()
	{
		GameCard myGameCard = TargetCombatable.MyGameCard;
		if (VeryEffectiveText != null)
		{
			VeryEffectiveText.transform.rotation = Quaternion.AngleAxis(-14f, Vector3.up) * myGameCard.transform.rotation;
			VeryEffectiveText.transform.position = myGameCard.transform.position;
		}
		if (!IsMiss)
		{
			base.transform.rotation = myGameCard.transform.rotation;
			base.transform.position = myGameCard.HitTextPosition.position;
		}
		else
		{
			base.transform.rotation = Camera.main.transform.rotation;
		}
	}
}
