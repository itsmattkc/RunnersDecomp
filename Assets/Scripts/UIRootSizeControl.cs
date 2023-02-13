using UnityEngine;

[AddComponentMenu("NGUI/UI/RootSizeControl")]
public class UIRootSizeControl : MonoBehaviour
{
	private const float ASPECT_VALUE = 1.335f;

	private const int MIMUN_HEIGHT = 768;

	private void Start()
	{
		base.enabled = false;
		float num = (float)Screen.width / (float)Screen.height;
		if (num < 1.335f && Screen.height < 768)
		{
			UIRoot component = base.gameObject.GetComponent<UIRoot>();
			if (component != null)
			{
				component.minimumHeight = 768;
			}
		}
	}
}
