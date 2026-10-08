using System;
using TMPro;
using UnityEngine;

public class NotificationElement : MonoBehaviour
{
	public CustomButton Button;

	public TextMeshProUGUI NotificationTitle;

	public TextMeshProUGUI NotificationText;

	public Action OnClicked;

	private float timer;

	private void Start()
	{
		Button.Clicked += delegate
		{
			OnClicked?.Invoke();
			UnityEngine.Object.Destroy(base.gameObject);
		};
	}

	private void Update()
	{
		timer += WorldManager.instance.TimeScale * Time.deltaTime;
		if (timer > 30f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 12f);
	}
}
