public class HudInterpolateConstant
{
	private int m_startValue;

	private int m_endValue;

	private float m_addValuePerSec;

	private float m_currentValue;

	private float m_prevValue;

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

	public int CurrentValue
	{
		get
		{
			return (int)m_currentValue;
		}
	}

	public int PrevValue
	{
		get
		{
			return (int)m_prevValue;
		}
	}

	public HudInterpolateConstant()
	{
		m_startValue = 0;
		m_endValue = 0;
		m_addValuePerSec = 0f;
		m_currentValue = 0f;
		m_isEnd = false;
		m_pauseFlag = false;
	}

	public void Setup(int startValue, int endValue, float addValuePerSec)
	{
		m_startValue = startValue;
		m_currentValue = m_startValue;
		m_prevValue = m_currentValue;
		m_endValue = endValue;
		m_addValuePerSec = addValuePerSec;
		m_isEnd = false;
		m_pauseFlag = false;
	}

	public void Reset()
	{
		m_isEnd = false;
	}

	public int ForceEnd()
	{
		m_isEnd = true;
		m_currentValue = m_endValue;
		return m_endValue;
	}

	public int SetForceValue(int value)
	{
		m_prevValue = m_currentValue;
		if (value < m_endValue)
		{
			m_currentValue = value;
		}
		else
		{
			m_currentValue = m_endValue;
			m_isEnd = true;
		}
		return (int)m_currentValue;
	}

	public int Update(float deltaTime)
	{
		if (m_isEnd)
		{
			return m_endValue;
		}
		if (!m_pauseFlag)
		{
			m_prevValue = m_currentValue;
			float num = m_addValuePerSec * deltaTime;
			m_currentValue += num;
		}
		if (m_currentValue >= (float)m_endValue)
		{
			m_currentValue = m_endValue;
			m_isEnd = true;
			return m_endValue;
		}
		return (int)m_currentValue;
	}
}
