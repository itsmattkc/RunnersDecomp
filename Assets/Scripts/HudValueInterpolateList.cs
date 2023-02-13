using System.Collections.Generic;

public class HudValueInterpolateList
{
	private List<HudValueInterpolate> m_interpolateList;

	private bool m_isEnd;

	private long m_lastValue;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public HudValueInterpolateList()
	{
		m_interpolateList = new List<HudValueInterpolate>();
		m_lastValue = 0L;
	}

	public void Add(int startValue, int endValue, float interpolateTime)
	{
		HudValueInterpolate hudValueInterpolate = new HudValueInterpolate();
		hudValueInterpolate.Setup(startValue, endValue, interpolateTime);
		m_interpolateList.Add(hudValueInterpolate);
		m_isEnd = false;
	}

	public void Reset()
	{
		m_isEnd = false;
		if (m_interpolateList == null)
		{
			return;
		}
		foreach (HudValueInterpolate interpolate in m_interpolateList)
		{
			if (interpolate != null)
			{
				interpolate.Reset();
			}
		}
	}

	public void Clear()
	{
		if (m_interpolateList != null)
		{
			m_interpolateList.Clear();
		}
	}

	public void ForceEnd()
	{
		if (m_interpolateList != null)
		{
			int count = m_interpolateList.Count;
			if (count != 0)
			{
				HudValueInterpolate hudValueInterpolate = m_interpolateList[count - 1];
				m_lastValue = hudValueInterpolate.ForceEnd();
				m_interpolateList.Clear();
			}
		}
	}

	public long Update(float deltaTime)
	{
		if (m_isEnd)
		{
			return m_lastValue;
		}
		HudValueInterpolate hudValueInterpolate = m_interpolateList[0];
		long lastValue = hudValueInterpolate.Update(deltaTime);
		if (hudValueInterpolate.IsEnd)
		{
			m_interpolateList.Remove(hudValueInterpolate);
			if (m_interpolateList.Count == 0)
			{
				m_isEnd = true;
			}
		}
		m_lastValue = lastValue;
		return m_lastValue;
	}
}
