using UnityEngine;

public class DebugTraceButton : MonoBehaviour
{
	public delegate void ButtonClickedCallback(string name);

	private ButtonClickedCallback m_callback;

	private string m_name;

	private static Vector2 s_DefaultSize = new Vector2(200f, 50f);

	private Vector2 m_position;

	private Vector2 m_size;

	private Rect m_rect;

	private bool m_isActive;

	public void Setup(string name, ButtonClickedCallback callback, Vector2 position)
	{
		Setup(name, callback, position, s_DefaultSize);
	}

	public void Setup(string name, ButtonClickedCallback callback, Vector2 position, Vector2 size)
	{
		m_name = name;
		m_callback = callback;
		m_position = position;
		m_size = size;
		m_rect = new Rect(m_position.x, m_position.y, m_size.x, m_size.y);
	}

	public void SetActive(bool isActive)
	{
		m_isActive = isActive;
	}

	private void OnGUI()
	{
		if (m_isActive && m_name != null && GUI.Button(m_rect, m_name) && m_callback != null)
		{
			m_callback(m_name);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
