using TMPro;
using UnityEngine;

public class StatusEffectElement : Hoverable
{
	[HideInInspector]
	public StatusEffect MyStatusEffect;

	public SpriteRenderer StatusRenderer;

	public Vector3 TargetLocalPosition;

	private Vector3 startScale;

	public bool DestroyMe;

	public GameCard ParentCard;

	public TextMeshPro TextMesh;

	private MaterialPropertyBlock propBlock;

	private void Awake()
	{
		startScale = base.transform.localScale;
		propBlock = new MaterialPropertyBlock();
		StatusRenderer.GetPropertyBlock(propBlock);
	}

	public void SetStatusEffect(GameCard parentCard, StatusEffect effect)
	{
		MyStatusEffect = effect;
		ParentCard = parentCard;
		StatusRenderer.sprite = effect.Sprite;
	}

	public void Update()
	{
		Vector3 zero = startScale;
		if (DestroyMe)
		{
			zero = Vector3.zero;
			if (zero.x < 0.001f)
			{
				Object.Destroy(base.gameObject);
				ParentCard.StatusEffectElements.Remove(this);
			}
		}
		if (MyStatusEffect.FillAmount.HasValue)
		{
			propBlock.SetFloat("_FillAmount", MyStatusEffect.FillAmount.Value);
		}
		else
		{
			propBlock.SetFloat("_FillAmount", 2f);
		}
		propBlock.SetTexture("_MainTex", MyStatusEffect.Sprite.texture);
		propBlock.SetColor("_ColorA", MyStatusEffect.ColorA);
		propBlock.SetColor("_ColorB", MyStatusEffect.ColorB);
		StatusRenderer.SetPropertyBlock(propBlock);
		if (MyStatusEffect.StatusNumberColor.HasValue)
		{
			TextMesh.color = MyStatusEffect.StatusNumberColor.Value;
		}
		if (MyStatusEffect.StatusNumber.HasValue)
		{
			TextMesh.text = MyStatusEffect.StatusNumber.ToString();
		}
		else
		{
			TextMesh.text = string.Empty;
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, zero, Time.deltaTime * 12f);
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, TargetLocalPosition, Time.deltaTime * 12f);
	}

	public override string GetTitle()
	{
		return SokLoc.Translate("label_status_effect", LocParam.Create("status", MyStatusEffect.Name));
	}

	public override string GetDescription()
	{
		if (!string.IsNullOrEmpty(MyStatusEffect.Lore))
		{
			return "<i>" + MyStatusEffect.Lore + "</i>\n\n" + MyStatusEffect.Description;
		}
		return MyStatusEffect.Description;
	}
}
