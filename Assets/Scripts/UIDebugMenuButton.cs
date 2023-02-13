using UnityEngine;

public class UIDebugMenuButton : MonoBehaviour
{
	private Rect m_rect;

	private string m_name;

	private GameObject m_callbackObject;

	private bool m_isActive;

	public void Setup(Rect rect, string name, GameObject callbackObject)
	{
		m_rect = rect;
		m_name = name;
		m_callbackObject = callbackObject;
		m_isActive = false;
	}

	public void SetActive(bool flag)
	{
		m_isActive = flag;
	}

	private void OnGUI()
	{
		if (m_isActive && m_name != null && !(m_callbackObject == null) && GUI.Button(m_rect, m_name))
		{
			m_callbackObject.SendMessage("OnClicked", m_name);
		}
	}
}
