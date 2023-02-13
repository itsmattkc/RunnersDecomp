public class NetNoticeItem
{
	private const int ANNOUNCE_TYPE_EVERYDAY = 0;

	private const int ANNOUNCE_TYPE_ONCE = 1;

	private const int ANNOUNCE_TYPE_FULLTIME = 2;

	private const int ANNOUNCE_TYPE_NOT_POP_UP = 3;

	public static int OPERATORINFO_START_ID = 1000000000;

	public static int OPERATORINFO_RANKINGRESULT_ID = OPERATORINFO_START_ID;

	public static int OPERATORINFO_EVENTRANKINGRESULT_ID = OPERATORINFO_START_ID + 1;

	public static int OPERATORINFO_QUICKRANKINGRESULT_ID = OPERATORINFO_START_ID + 2;

	private long m_id;

	private int m_priority;

	private long m_start;

	private long m_end;

	private int m_announceType;

	private int m_windowType;

	private string m_imageId;

	private string m_message;

	private string m_webAdress;

	private string m_saveKey;

	private string m_designatedArea;

	public long Id
	{
		get
		{
			return m_id;
		}
	}

	public int Priority
	{
		get
		{
			return m_priority;
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

	public int AnnounceType
	{
		get
		{
			return m_announceType;
		}
	}

	public int WindowType
	{
		get
		{
			return m_windowType;
		}
	}

	public string ImageId
	{
		get
		{
			return m_imageId;
		}
	}

	public string Message
	{
		get
		{
			return m_message;
		}
	}

	public string Adress
	{
		get
		{
			return m_webAdress;
		}
	}

	public string SaveKey
	{
		get
		{
			return m_saveKey;
		}
	}

	public void Init(long id, int priority, long start, long end, string param, string saveKey)
	{
		m_id = id;
		m_priority = priority;
		m_start = start;
		m_end = end;
		m_saveKey = saveKey;
		string[] array = param.Split('_');
		m_announceType = 0;
		m_imageId = "-1";
		m_message = string.Empty;
		m_windowType = 0;
		m_webAdress = string.Empty;
		m_designatedArea = string.Empty;
		if (array.Length > 0)
		{
			int.TryParse(array[0], out m_announceType);
		}
		if (array.Length > 1)
		{
			m_message = array[1];
		}
		if (array.Length > 2)
		{
			m_imageId = array[2];
		}
		if (array.Length > 3)
		{
			int.TryParse(array[3], out m_windowType);
		}
		if (array.Length == 5)
		{
			if (m_windowType == 16 || m_windowType == 17)
			{
				m_designatedArea = array[4];
			}
			else
			{
				m_webAdress = array[4];
			}
		}
		else
		{
			if (array.Length <= 5 || (m_windowType == 16 && m_windowType != 17))
			{
				return;
			}
			for (int i = 4; i < array.Length; i++)
			{
				if (i == 4)
				{
					m_webAdress = array[i];
				}
				else
				{
					m_webAdress = m_webAdress + "_" + array[i];
				}
			}
		}
	}

	public bool IsEveryDay()
	{
		return 0 == m_announceType;
	}

	public bool IsOnce()
	{
		return 1 == m_announceType;
	}

	public bool IsFullTime()
	{
		return 2 == m_announceType;
	}

	public bool IsOnlyInformationPage()
	{
		return 3 == m_announceType;
	}

	public bool IsOutsideDesignatedArea()
	{
		bool result = false;
		if (m_windowType == 16 || m_windowType == 17)
		{
			result = true;
			if (RegionManager.Instance != null)
			{
				RegionInfo regionInfo = RegionManager.Instance.GetRegionInfo();
				if (regionInfo != null && !string.IsNullOrEmpty(m_designatedArea) && !string.IsNullOrEmpty(regionInfo.CountryCode) && m_designatedArea == regionInfo.CountryCode)
				{
					result = false;
				}
			}
		}
		return result;
	}
}
