using UnityEngine;

public class UIDebugMenuTextBox : MonoBehaviour
{
	private Rect m_rect;

	private string m_text;

	private bool m_isActive;

	private Vector2 m_scrollViewVector = Vector2.zero;

	private Vector2 m_scrollScale = new Vector2(1f, 2f);

	public string Text
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

	public Vector2 ScrollScale
	{
		get
		{
			return m_scrollScale;
		}
		set
		{
			m_scrollScale = value;
		}
	}

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

	public void Setup(Rect rect, string text)
	{
		m_rect = rect;
		m_text = text;
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
			Rect rect = new Rect(m_rect.left, m_rect.top, m_rect.width * m_scrollScale.x, m_rect.height * m_scrollScale.y);
			m_scrollViewVector = GUI.BeginScrollView(m_rect, m_scrollViewVector, rect);
			GUI.TextArea(rect, m_text);
			GUI.EndScrollView();
		}
	}
}
