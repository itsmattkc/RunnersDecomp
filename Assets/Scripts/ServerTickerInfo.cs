using System.Collections.Generic;

public class ServerTickerInfo
{
	private bool m_existNewData;

	private int m_tickerIndex;

	private List<ServerTickerData> m_data = new List<ServerTickerData>();

	public bool ExistNewData
	{
		get
		{
			return m_existNewData;
		}
		set
		{
			m_existNewData = value;
		}
	}

	public int TickerIndex
	{
		get
		{
			return m_tickerIndex;
		}
		set
		{
			if (m_tickerIndex != value)
			{
				m_existNewData = true;
				m_tickerIndex = value;
			}
			else
			{
				m_existNewData = false;
			}
		}
	}

	public List<ServerTickerData> Data
	{
		get
		{
			return m_data;
		}
		private set
		{
		}
	}

	public void Init(int tickerIndex)
	{
		m_existNewData = true;
		m_tickerIndex = tickerIndex;
	}
}
