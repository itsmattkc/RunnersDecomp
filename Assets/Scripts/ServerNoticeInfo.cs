using SaveData;
using System;
using System.Collections.Generic;

public class ServerNoticeInfo
{
	private DateTime m_lastUpdateInfoTime = DateTime.MinValue;

	public bool m_isGetNoticeInfo
	{
		get;
		set;
	}

	public bool m_isShowedNoticeInfo
	{
		get;
		set;
	}

	public List<NetNoticeItem> m_noticeItems
	{
		get;
		set;
	}

	public List<NetNoticeItem> m_rouletteItems
	{
		get;
		set;
	}

	public List<NetNoticeItem> m_eventItems
	{
		get;
		set;
	}

	public DateTime LastUpdateInfoTime
	{
		get
		{
			return m_lastUpdateInfoTime;
		}
		set
		{
			m_lastUpdateInfoTime = value;
		}
	}

	public ServerNoticeInfo()
	{
		m_noticeItems = new List<NetNoticeItem>();
		m_rouletteItems = new List<NetNoticeItem>();
		m_eventItems = new List<NetNoticeItem>();
		m_isGetNoticeInfo = false;
		m_isShowedNoticeInfo = false;
	}

	public bool IsNeedUpdateInfo()
	{
		DateTime currentTime = NetUtil.GetCurrentTime();
		TimeSpan t = currentTime - m_lastUpdateInfoTime;
		TimeSpan t2 = new TimeSpan(1, 0, 0);
		if (t >= t2)
		{
			return true;
		}
		return false;
	}

	public bool IsAllChecked()
	{
		if (!m_isGetNoticeInfo)
		{
			return true;
		}
		if (m_isShowedNoticeInfo)
		{
			return true;
		}
		bool flag = true;
		int count = m_noticeItems.Count;
		for (int i = 0; i < count; i++)
		{
			flag &= IsChecked(m_noticeItems[i]);
		}
		return flag;
	}

	public NetNoticeItem GetInfo(int index)
	{
		NetNoticeItem result = null;
		if (m_noticeItems.Count > index)
		{
			result = m_noticeItems[index];
		}
		return result;
	}

	public void Clear()
	{
		m_noticeItems.Clear();
		m_rouletteItems.Clear();
		m_eventItems.Clear();
		m_isGetNoticeInfo = false;
	}

	public bool IsChecked(NetNoticeItem item)
	{
		bool result = false;
		InformationSaveManager instance = InformationSaveManager.Instance;
		if (instance != null)
		{
			InformationData informationData = instance.GetInformationData();
			if (informationData != null && item != null)
			{
				string data = informationData.GetData(item.Id.ToString(), InformationData.DataType.SHOWED_TIME);
				if (data != InformationData.INVALID_PARAM)
				{
					if (item.IsEveryDay())
					{
						result = true;
						DateTime localDateTime = NetUtil.GetLocalDateTime(long.Parse(data));
						DateTime localDateTime2 = NetUtil.GetLocalDateTime(NetUtil.GetCurrentUnixTime());
						if (localDateTime.Day != localDateTime2.Day)
						{
							result = false;
						}
						if (localDateTime.Month != localDateTime2.Month)
						{
							result = false;
						}
						if (localDateTime.Year != localDateTime2.Year)
						{
							result = false;
						}
					}
					else if (item.IsOnce())
					{
						if (item.Id == NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID)
						{
							string data2 = informationData.GetData(item.Id.ToString(), InformationData.DataType.ADD_INFO);
							result = (item.SaveKey == data2);
						}
						else if (item.Id == NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID)
						{
							string eventRankingData = informationData.GetEventRankingData(item.Id.ToString(), item.SaveKey, InformationData.DataType.ADD_INFO);
							result = (item.SaveKey == eventRankingData);
						}
						else if (item.Id == NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID)
						{
							string data3 = informationData.GetData(item.Id.ToString(), InformationData.DataType.ADD_INFO);
							result = (item.SaveKey == data3);
						}
						else
						{
							result = true;
						}
					}
					else if (item.IsOnlyInformationPage())
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public bool IsCheckedForMenuIcon(NetNoticeItem item)
	{
		InformationSaveManager instance = InformationSaveManager.Instance;
		if (instance != null)
		{
			InformationData informationData = instance.GetInformationData();
			if (informationData != null && item != null)
			{
				string data = informationData.GetData(item.Id.ToString(), InformationData.DataType.SHOWED_TIME);
				if (data != InformationData.INVALID_PARAM)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsOnTime(NetNoticeItem item)
	{
		if (item != null)
		{
			long num = NetUtil.GetCurrentUnixTime();
			if (num >= item.Start && item.End > num)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateChecked(NetNoticeItem item)
	{
		if (item == null)
		{
			return;
		}
		InformationSaveManager instance = InformationSaveManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		InformationData informationData = instance.GetInformationData();
		if (informationData != null)
		{
			long num = NetUtil.GetCurrentUnixTime();
			if (item.Id == NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID)
			{
				informationData.UpdateShowedTime(item.Id.ToString(), num.ToString(), item.SaveKey, "-1");
			}
			else if (item.Id == NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID)
			{
				informationData.UpdateEventRankingShowedTime(item.Id.ToString(), num.ToString(), item.SaveKey, item.ImageId);
			}
			else if (item.Id == NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID)
			{
				informationData.UpdateShowedTime(item.Id.ToString(), num.ToString(), item.SaveKey, "-1");
			}
			else
			{
				informationData.UpdateShowedTime(item.Id.ToString(), num.ToString(), item.End.ToString(), item.ImageId);
			}
		}
	}

	public void SaveInformation()
	{
		InformationSaveManager instance = InformationSaveManager.Instance;
		if (instance != null)
		{
			instance.SaveInformationData();
		}
	}
}
