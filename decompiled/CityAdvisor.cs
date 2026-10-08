using UnityEngine;

public class CityAdvisor : CardData
{
	public AudioClip AdvisorSound;

	public override void OnInitialCreate()
	{
		AudioManager.me.PlaySound2D(AdvisorSound, 1f, 0.1f);
		base.OnInitialCreate();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public override void UpdateCard()
	{
		if (!CutsceneScreen.instance.IsAdvisorCutscene && !MyGameCard.IsDemoCard)
		{
			MyGameCard.DestroyCard();
		}
		base.UpdateCard();
		AdvisorMovement();
	}

	public void SetAdditionalOffset()
	{
	}

	private void AdvisorMovement()
	{
		Vector3 targetPosition = GameCamera.instance.ScreenPosToWorldPos(new Vector2(Screen.width, Screen.height) * 0.5f);
		if (GameCamera.instance.TargetCardOverride != null)
		{
			targetPosition += Vector3.forward;
		}
		targetPosition += Vector3.left * 0.05f * Mathf.Cos(Time.time);
		targetPosition += Vector3.forward * 0.01f * Mathf.Cos(Time.time * 0.5f);
		MyGameCard.TargetPosition = targetPosition;
	}
}
