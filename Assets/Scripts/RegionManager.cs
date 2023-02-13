using SaveData;
using System;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
	private static RegionManager m_instance;

	private RegionInfoTable m_table;

	public static RegionManager Instance
	{
		get
		{
			return m_instance;
		}
		private set
		{
		}
	}

	public RegionInfo GetRegionInfo()
	{
		if (m_table != null)
		{
			string countryCode = SystemSaveManager.GetCountryCode();
			return m_table.GetInfo(countryCode);
		}
		return null;
	}

	public bool IsJapan()
	{
		RegionInfo regionInfo = GetRegionInfo();
		if (regionInfo != null && regionInfo.CountryCode == "JP")
		{
			return true;
		}
		return false;
	}

	public bool IsNeedIapMessage()
	{
		return false;
	}

	public bool IsNeedESRB()
	{
		RegionInfo regionInfo = GetRegionInfo();
		bool result = true;
		if (regionInfo != null && !string.IsNullOrEmpty(regionInfo.Limit))
		{
			string limit = regionInfo.Limit;
			if (limit.IndexOf("ESRB") == -1 && limit.IndexOf("esrb") == -1 && limit.IndexOf("Esrb") == -1)
			{
				result = false;
			}
		}
		return result;
	}

	public bool IsUseSNS()
	{
		bool result = true;
		if (IsNeedESRB())
		{
			result = false;
			ServerSettingState serverSettingState = null;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			int num = 1;
			if (loggedInServerInterface != null)
			{
				serverSettingState = ServerInterface.SettingState;
				if (serverSettingState != null && !string.IsNullOrEmpty(serverSettingState.m_birthday))
				{
					num = HudUtility.GetAge(DateTime.Parse(serverSettingState.m_birthday), NetUtil.GetCurrentTime());
				}
			}
			if (num >= 13)
			{
				result = true;
			}
		}
		return result;
	}

	public bool IsUseHardlightAds()
	{
		RegionInfo regionInfo = GetRegionInfo();
		if (regionInfo != null)
		{
			if (regionInfo.CountryCode == "US")
			{
				ServerSettingState serverSettingState = null;
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				int num = 1;
				if (loggedInServerInterface != null)
				{
					serverSettingState = ServerInterface.SettingState;
					if (serverSettingState != null && !string.IsNullOrEmpty(serverSettingState.m_birthday))
					{
						num = HudUtility.GetAge(DateTime.Parse(serverSettingState.m_birthday), NetUtil.GetCurrentTime());
					}
				}
				if (num >= 13)
				{
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Init();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Init()
	{
		m_table = new RegionInfoTable();
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
