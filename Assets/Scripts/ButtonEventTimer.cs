using UnityEngine;

public class ButtonEventTimer : MonoBehaviour
{
	private float m_waitButtonTime;

	public void SetWaitTime(float waitTime)
	{
		m_waitButtonTime = waitTime;
	}

	public void SetWaitTimeDefault()
	{
		SetWaitTime(0.4f);
	}

	public bool IsWaiting()
	{
		if (m_waitButtonTime <= 0f)
		{
			return false;
		}
		return true;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_waitButtonTime > 0f)
		{
			m_waitButtonTime -= RealTime.deltaTime;
			if (m_waitButtonTime <= 0f)
			{
				m_waitButtonTime = 0f;
			}
		}
	}
}
