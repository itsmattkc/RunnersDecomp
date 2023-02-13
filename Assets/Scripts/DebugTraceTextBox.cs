using UnityEngine;

public class DebugTraceTextBox : MonoBehaviour
{
	private string m_text;

	private Vector2 m_scrollViewVector = Vector2.zero;

	private Vector2 m_scrollScale = new Vector2(1f, 1f);

	private bool m_isActive;

	private Vector2 m_position;

	private static Vector2 s_DefaultSize = new Vector2(300f, 300f);

	private Vector2 m_size;

	private float m_sizeScale = 1f;

	private Rect m_rect;

	public void Setup(Vector2 position)
	{
		Setup(position, s_DefaultSize);
	}

	public void Setup(Vector2 position, Vector2 size)
	{
		m_position = position;
		SetSize(size);
	}

	public void SetActive(bool isActive)
	{
		m_isActive = isActive;
	}

	public void SetText(string text)
	{
		m_text = text;
	}

	public void SetSize(Vector2 size)
	{
		m_size = size;
		m_rect = new Rect(m_position.x, m_position.y, m_size.x * m_sizeScale, m_size.y * m_sizeScale);
	}

	public void SetSizeScale(float sizeScale)
	{
		m_sizeScale = sizeScale;
		m_rect = new Rect(m_position.x, m_position.y, m_size.x * m_sizeScale, m_size.y * m_sizeScale);
	}

	public void SetScrollScale(Vector2 scale)
	{
		m_scrollViewVector = Vector2.zero;
		m_scrollScale = scale;
	}

	private void OnGUI()
	{
		if (m_isActive)
		{
			Rect rect = new Rect(m_rect.left, m_rect.top, m_rect.width, m_rect.height * m_scrollScale.y);
			m_scrollViewVector = GUI.BeginScrollView(m_rect, m_scrollViewVector, rect);
			int startIndex = Mathf.Max(m_text.Length - 10000, 0);
			GUI.TextArea(rect, m_text.Substring(startIndex));
			GUI.EndScrollView();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
