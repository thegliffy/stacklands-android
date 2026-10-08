using TMPro;
using UnityEngine;

public class FloatingStatus : Interactable
{
	public bool InAnimation;

	public GameCard ParentCard;

	public TextMeshPro Text;

	public Vector3 NoStatusOffset;

	public Vector3 StatusOffset;

	private float yOffset;

	private Vector3 endPos;

	private Vector3 positionVelocity;

	private float disappearTime;

	private string descriptionText;

	private bool closeOnHover;

	private bool RemoveTimerStarted;

	private float RemoveTimer;

	private float timer;

	private Vector3 scaleVelo;

	public void StartAnimation(GameCard parent, bool isPositive, int amount, string descriptionText, string iconTag, bool desiredBehaviour, int index = 1, float disappearTime = 1f, bool closeOnHover = false)
	{
		Transform parent2 = ((parent.WorkerHolder != null) ? parent.WorkerHolder.transform : parent.transform);
		base.transform.parent = parent2;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		base.gameObject.name = parent.CardData.Name;
		ParentCard = parent;
		yOffset = 0.12f * (float)index;
		string arg = (isPositive ? "+" : "-");
		Text.color = (desiredBehaviour ? ColorManager.instance.FloatingTextColorSuccess : ColorManager.instance.FloatingTextColorFailed);
		Text.text = $"{arg}{Mathf.Abs(amount)}{iconTag}";
		this.disappearTime = disappearTime;
		this.descriptionText = descriptionText;
		this.closeOnHover = closeOnHover;
		timer = 0f;
		Text.alpha = 1f;
		InAnimation = true;
		base.gameObject.SetActive(value: true);
	}

	public void StopAnimation()
	{
		InAnimation = false;
		if (base.transform != null)
		{
			base.transform.parent = null;
		}
		base.gameObject.SetActive(value: false);
	}

	protected override void Start()
	{
		MyBoard = WorldManager.instance.CurrentBoard;
		base.Start();
	}

	public override string GetTooltipText()
	{
		return descriptionText;
	}

	protected override void ClampPos()
	{
	}

	protected override void Update()
	{
		Vector3 vector = ((ParentCard.CardData.HasAnyStatusEffect() || ParentCard.GetCardWithStatusInStack() != null) ? StatusOffset : NoStatusOffset);
		vector.y += yOffset;
		base.transform.localPosition = FRILerp.Spring(base.transform.localPosition, vector, 12f, 12f, ref positionVelocity);
		if (InAnimation && (disappearTime > 0f || timer <= 1f) && Vector3.Distance(base.transform.localPosition, vector) < 0.01f)
		{
			timer += Time.deltaTime;
			if (timer <= 1f && disappearTime != 0f)
			{
				Text.alpha = Mathf.Lerp(1f, 0f, timer * 4f);
			}
			if (disappearTime > 0f && timer > disappearTime)
			{
				StopAnimation();
			}
		}
		Vector3 target = Vector3.one;
		if (IsHovered)
		{
			if (closeOnHover)
			{
				RemoveTimerStarted = true;
				RemoveTimer = 0f;
			}
			target = Vector3.one * 1.2f;
			Tooltip.Text = descriptionText;
		}
		RemoveTimer += Time.deltaTime * 0.5f;
		if (RemoveTimerStarted && RemoveTimer > 1f)
		{
			RemoveTimerStarted = false;
			StopAnimation();
		}
		base.transform.localScale = FRILerp.Spring(base.transform.localScale, target, 30f, 30f, ref scaleVelo);
	}
}
