using TMPro;
using UnityEngine;

public class ScrollText : MonoBehaviour
{
	public float ScrollSpeed;

	public TextMeshProUGUI myText;

	private RectTransform myRect;

	private string lastText;

	private float startWaitTimer;

	private float endWaitTimer;

	private void Start()
	{
		myRect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		RectTransform rectTransform = (RectTransform)myRect.parent;
		Vector2 anchoredPosition = myRect.anchoredPosition;
		startWaitTimer += Time.deltaTime;
		bool flag = anchoredPosition.y >= myRect.sizeDelta.y - rectTransform.sizeDelta.y;
		if (startWaitTimer >= 0.75f && myRect.sizeDelta.y >= rectTransform.sizeDelta.y && !flag)
		{
			anchoredPosition.y += ScrollSpeed * Time.deltaTime;
		}
		if (flag)
		{
			endWaitTimer += Time.deltaTime;
			if (endWaitTimer >= 1.5f)
			{
				anchoredPosition.y = 0f;
				startWaitTimer = 0f;
				endWaitTimer = 0f;
			}
		}
		myRect.anchoredPosition = anchoredPosition;
	}

	public void ResetScroll()
	{
		Vector2 anchoredPosition = myRect.anchoredPosition;
		anchoredPosition.y = 0f;
		myRect.anchoredPosition = anchoredPosition;
		startWaitTimer = 0f;
		endWaitTimer = 0f;
	}

	private void LateUpdate()
	{
		if (myText != null && lastText != myText.text)
		{
			ResetScroll();
		}
		lastText = ((myText != null) ? myText.text : "");
	}
}
