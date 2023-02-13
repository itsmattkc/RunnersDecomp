using UnityEngine;

public class HudFlagWatcher
{
	public delegate void ValueChangeCallback(float newValue, float prevValue);

	private GameObject m_watchObject;

	private float m_value;

	private ValueChangeCallback m_callback;

	public void Setup(GameObject watchObject, ValueChangeCallback callback)
	{
		m_watchObject = watchObject;
		m_callback = callback;
		if (m_watchObject != null)
		{
			m_watchObject.SetActive(true);
			Vector3 localPosition = m_watchObject.transform.localPosition;
			m_value = localPosition.x;
		}
	}

	public void Update()
	{
		if (!(m_watchObject != null))
		{
			return;
		}
		Vector3 localPosition = m_watchObject.transform.localPosition;
		float x = localPosition.x;
		if (x != m_value)
		{
			if (m_callback != null)
			{
				m_callback(x, m_value);
			}
			m_value = x;
		}
	}
}
