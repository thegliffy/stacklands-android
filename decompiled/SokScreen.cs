using UnityEngine;

public class SokScreen : MonoBehaviour
{
	public RectTransform Rect => base.gameObject.GetComponent<RectTransform>();

	public virtual bool IsFrameRateUncapped { get; }
}
