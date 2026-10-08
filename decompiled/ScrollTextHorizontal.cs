using TMPro;
using UnityEngine;

public class ScrollTextHorizontal : MonoBehaviour
{
	public float ScrollSpeed;

	private TextMeshProUGUI myText;

	private RectTransform myRect;

	private string lastText;

	private float startWaitTimer;

	private float endWaitTimer;

	private void Start()
	{
		myRect = GetComponent<RectTransform>();
		myText = GetComponent<TextMeshProUGUI>();
	}

	private void Update()
	{
		RectTransform rectTransform = (RectTransform)myRect.parent;
		Vector2 anchoredPosition = myRect.anchoredPosition;
		startWaitTimer += Time.deltaTime;
		bool flag = anchoredPosition.x <= 0f - (myRect.sizeDelta.x - rectTransform.sizeDelta.x);
		if (startWaitTimer >= 0.75f && myRect.sizeDelta.x >= rectTransform.sizeDelta.x && !flag)
		{
			anchoredPosition.x -= ScrollSpeed * Time.deltaTime;
		}
		if (flag)
		{
			endWaitTimer += Time.deltaTime;
			if (endWaitTimer >= 1.5f)
			{
				anchoredPosition.x = 0f;
				startWaitTimer = 0f;
				endWaitTimer = 0f;
			}
		}
		myRect.anchoredPosition = anchoredPosition;
	}

	private void LateUpdate()
	{
		if (lastText != myText.text)
		{
			Vector2 anchoredPosition = myRect.anchoredPosition;
			anchoredPosition.x = 0f;
			myRect.anchoredPosition = anchoredPosition;
			startWaitTimer = 0f;
			endWaitTimer = 0f;
		}
		lastText = myText.text;
	}
}
