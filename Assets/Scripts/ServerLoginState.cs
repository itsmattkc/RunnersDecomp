public class ServerLoginState
{
	public string m_lineId;

	public string m_altLineId;

	public string m_lineAuthToken;

	public string m_segaAuthToken;

	private string m_sessionId;

	private ulong m_seqNum;

	public int sessionTimeLimit;

	private bool m_isChangeDataVersion;

	private int m_dataVersion;

	private bool m_isChangeAssetsVersion;

	private int m_assetsVersion;

	private string m_assetsVersionString;

	private string m_infoVersionString;

	public bool IsLoggedIn
	{
		get
		{
			return sessionId != null && string.Empty != sessionId;
		}
	}

	public string sessionId
	{
		get
		{
			return m_sessionId;
		}
		set
		{
			m_sessionId = value;
		}
	}

	public ulong seqNum
	{
		get
		{
			return m_seqNum;
		}
		set
		{
			m_seqNum = value;
		}
	}

	public string serverVersion
	{
		get;
		set;
	}

	public int serverVersionValue
	{
		get;
		set;
	}

	public long serverLastTime
	{
		get;
		set;
	}

	public bool IsChangeDataVersion
	{
		get
		{
			return m_isChangeDataVersion;
		}
		set
		{
			m_isChangeDataVersion = value;
		}
	}

	public int DataVersion
	{
		get
		{
			return m_dataVersion;
		}
		set
		{
			if (value != m_dataVersion)
			{
				m_dataVersion = value;
				m_isChangeDataVersion = true;
			}
		}
	}

	public bool IsChangeAssetsVersion
	{
		get
		{
			return m_isChangeAssetsVersion;
		}
		set
		{
			m_isChangeAssetsVersion = value;
		}
	}

	public int AssetsVersion
	{
		get
		{
			return m_assetsVersion;
		}
		set
		{
			if (value != m_assetsVersion)
			{
				m_assetsVersion = value;
				m_isChangeAssetsVersion = true;
			}
		}
	}

	public string AssetsVersionString
	{
		get
		{
			return m_assetsVersionString;
		}
		set
		{
			if (value != m_assetsVersionString)
			{
				m_assetsVersionString = value;
				m_isChangeAssetsVersion = true;
			}
		}
	}

	public string InfoVersionString
	{
		get
		{
			return m_infoVersionString;
		}
		set
		{
			m_infoVersionString = value;
		}
	}

	public ServerLoginState()
	{
		m_lineId = null;
		m_altLineId = null;
		m_lineAuthToken = null;
		m_segaAuthToken = null;
		sessionId = null;
		seqNum = 0uL;
		m_isChangeDataVersion = false;
		m_dataVersion = -1;
		m_isChangeAssetsVersion = false;
		m_assetsVersion = -1;
	}
}
