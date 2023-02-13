using UnityEngine;

public class UIDebugMenuTextField : MonoBehaviour
{
	private Rect m_rect;

	private string m_title;

	private string m_text;

	private bool m_isActive;

	public string text
	{
		get
		{
			return m_text;
		}
		set
		{
			m_text = value;
		}
	}

	public void Setup(Rect rect, string titleText)
	{
		Setup(rect, titleText, "0");
	}

	public void Setup(Rect rect, string titleText, string fieldText)
	{
		m_rect = rect;
		m_title = titleText;
		m_text = fieldText;
		m_isActive = false;
	}

	public void SetActive(bool flag)
	{
		m_isActive = flag;
	}

	private void OnGUI()
	{
		if (m_isActive)
		{
			GUI.TextArea(new Rect(m_rect.xMin, m_rect.yMin - 20f, m_rect.width, 20f), m_title);
			m_text = GUI.TextField(m_rect, m_text);
		}
	}
}
