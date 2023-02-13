public class SocialUserData
{
	private string m_id;

	private string m_name;

	private string m_url;

	private bool m_isSilhouette;

	private SocialUserCustomData m_customData;

	public string Id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}

	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	public string Url
	{
		get
		{
			return m_url;
		}
		set
		{
			m_url = value;
		}
	}

	public bool IsSilhouette
	{
		get
		{
			return m_isSilhouette;
		}
		set
		{
			m_isSilhouette = value;
		}
	}

	public SocialUserCustomData CustomData
	{
		get
		{
			return m_customData;
		}
		set
		{
			m_customData = value;
		}
	}

	public SocialUserData()
	{
		m_id = string.Empty;
		m_name = string.Empty;
		m_url = string.Empty;
		m_customData = new SocialUserCustomData();
	}
}
