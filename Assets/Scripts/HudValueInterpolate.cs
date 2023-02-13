public class HudValueInterpolate
{
	private long m_startValue;

	private long m_endValue;

	private float m_interpolateTime;

	private float m_currentTime;

	private bool m_isEnd;

	private bool m_pauseFlag;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public bool IsPause
	{
		get
		{
			return m_pauseFlag;
		}
		set
		{
			m_pauseFlag = value;
		}
	}

	public float CurrentTime
	{
		get
		{
			return m_currentTime;
		}
	}

	public HudValueInterpolate()
	{
		m_startValue = 0L;
		m_endValue = 0L;
		m_interpolateTime = 0f;
		m_currentTime = 0f;
		m_isEnd = false;
		m_pauseFlag = false;
	}

	public void Setup(long startValue, long endValue, float interpolateTime)
	{
		m_startValue = startValue;
		m_currentTime = 0f;
		m_endValue = endValue;
		m_interpolateTime = interpolateTime;
		m_isEnd = false;
		m_pauseFlag = false;
	}

	public void Reset()
	{
		m_isEnd = false;
	}

	public long ForceEnd()
	{
		m_isEnd = true;
		m_currentTime = m_interpolateTime;
		return m_endValue;
	}

	public long Update(float deltaTime)
	{
		if (m_isEnd)
		{
			return m_endValue;
		}
		if (!m_pauseFlag)
		{
			m_currentTime += deltaTime;
		}
		if (m_currentTime >= m_interpolateTime)
		{
			m_isEnd = true;
			return m_endValue;
		}
		long num = m_endValue - m_startValue;
		float num2 = m_currentTime / m_interpolateTime;
		return m_startValue + (long)((float)num * num2);
	}
}
