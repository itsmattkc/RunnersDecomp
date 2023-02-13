using App;
using UnityEngine;

public class DebugTraceScrollBar : MonoBehaviour
{
	public delegate void ValueChangedCallback(string name, float value);

	private ValueChangedCallback m_callback;

	public static readonly float MaxValue = 100f;

	public static readonly float MinValue = 1f;

	private string m_name;

	private bool m_isActive;

	private float m_scrollValue;

	private float m_scrollValuePrev;

	private static readonly Vector2 s_Size = new Vector2(200f, 50f);

	private Rect m_rect;

	public void SetActive(bool isActive)
	{
		m_isActive = isActive;
	}

	public void SetUp(string name, ValueChangedCallback callback, Vector2 position)
	{
		m_name = name;
		m_callback = callback;
		float x = position.x;
		float y = position.y;
		Vector2 vector = s_Size;
		float x2 = vector.x;
		Vector2 vector2 = s_Size;
		m_rect = new Rect(x, y, x2, vector2.y);
	}

	private void OnGUI()
	{
		if (!m_isActive)
		{
			return;
		}
		GUI.Label(new Rect(m_rect.left, m_rect.top - 20f, m_rect.width, m_rect.height), m_name);
		m_scrollValue = GUI.HorizontalScrollbar(m_rect, m_scrollValue, 1f, MinValue, MaxValue);
		if (!Math.NearEqual(m_scrollValue, m_scrollValuePrev))
		{
			m_scrollValuePrev = m_scrollValue;
			if (m_callback != null)
			{
				m_callback(m_name, m_scrollValue);
			}
		}
	}
}
