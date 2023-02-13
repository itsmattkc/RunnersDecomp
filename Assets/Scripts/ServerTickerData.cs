public class ServerTickerData
{
	private long m_id;

	private long m_start;

	private long m_end;

	private string m_param;

	public long Id
	{
		get
		{
			return m_id;
		}
	}

	public long Start
	{
		get
		{
			return m_start;
		}
	}

	public long End
	{
		get
		{
			return m_end;
		}
	}

	public string Param
	{
		get
		{
			return m_param;
		}
	}

	public void Init(long id, long start, long end, string param)
	{
		m_id = id;
		m_start = start;
		m_end = end;
		m_param = param;
	}

	public void CopyTo(ServerTickerData to)
	{
		to.m_id = m_id;
		to.m_start = m_start;
		to.m_end = m_end;
		to.m_param = m_param;
	}
}
