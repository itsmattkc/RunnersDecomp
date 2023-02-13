using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuButtonList : MonoBehaviour
{
	private List<UIDebugMenuButton> m_buttons;

	public UIDebugMenuButtonList()
	{
		m_buttons = new List<UIDebugMenuButton>();
	}

	public void Add(List<Rect> rect, List<string> name, GameObject callbackObject)
	{
		if (rect.Count == name.Count)
		{
			int count = rect.Count;
			for (int i = 0; i < count; i++)
			{
				UIDebugMenuButton uIDebugMenuButton = base.gameObject.AddComponent<UIDebugMenuButton>();
				uIDebugMenuButton.Setup(rect[i], name[i], callbackObject);
				m_buttons.Add(uIDebugMenuButton);
			}
		}
	}

	public void SetActive(bool flag)
	{
		foreach (UIDebugMenuButton button in m_buttons)
		{
			if (!(button == null))
			{
				button.SetActive(flag);
			}
		}
	}
}
