public class RegionInfo
{
	private int m_countryId;

	private string m_countryCode;

	private string m_area;

	private string m_limit;

	public int CountryId
	{
		get
		{
			return m_countryId;
		}
		private set
		{
		}
	}

	public string CountryCode
	{
		get
		{
			return m_countryCode;
		}
		private set
		{
		}
	}

	public string Area
	{
		get
		{
			return m_area;
		}
		private set
		{
		}
	}

	public string Limit
	{
		get
		{
			return m_limit;
		}
		private set
		{
		}
	}

	public RegionInfo(int countryId, string countryCode, string area, string limit)
	{
		m_countryId = countryId;
		m_countryCode = countryCode;
		m_area = area;
		m_limit = limit;
	}
}
