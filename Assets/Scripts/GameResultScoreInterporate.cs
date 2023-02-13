public class GameResultScoreInterporate
{
	private UILabel m_label;

	private HudValueInterpolate m_interpolate;

	private long m_score;

	public bool IsEnd
	{
		get
		{
			return m_interpolate.IsEnd;
		}
	}

	public bool IsPause
	{
		get
		{
			return m_interpolate.IsPause;
		}
		set
		{
			m_interpolate.IsPause = value;
		}
	}

	public float CurrentTime
	{
		get
		{
			return m_interpolate.CurrentTime;
		}
	}

	public GameResultScoreInterporate()
	{
		m_label = null;
		m_interpolate = null;
	}

	public void Setup(UILabel label)
	{
		m_label = label;
		m_interpolate = new HudValueInterpolate();
		m_score = 0L;
	}

	public void AddScore(long addScore)
	{
		m_score += addScore;
		SetLabelStartValue(m_score);
	}

	public void SetLabelStartValue(long startValue)
	{
		if (!(m_label == null))
		{
			m_label.text = GameResultUtility.GetScoreFormat(startValue);
		}
	}

	public void PlayStart(long startValue, long endValue, float interpolateTime)
	{
		if (m_interpolate != null)
		{
			m_interpolate.Setup(startValue, endValue, interpolateTime);
			m_interpolate.Reset();
			if (startValue == endValue)
			{
				PlaySkip();
			}
		}
	}

	public void PlaySkip()
	{
		if (m_interpolate != null)
		{
			long val = m_interpolate.ForceEnd();
			m_label.text = GameResultUtility.GetScoreFormat(val);
		}
	}

	public long Update(float deltaTime)
	{
		if (m_interpolate == null)
		{
			return 0L;
		}
		if (m_label == null)
		{
			return 0L;
		}
		long num = m_interpolate.Update(deltaTime);
		m_label.text = GameResultUtility.GetScoreFormat(num);
		return num;
	}
}
