using Message;
using System;
using UnityEngine;

public class ServerDayCrossWatcher : MonoBehaviour
{
	public class MsgDayCross
	{
		public bool ServerConnect
		{
			get;
			set;
		}
	}

	public delegate void UpdateInfoCallback(MsgDayCross msg);

	private static ServerDayCrossWatcher m_instance;

	private static readonly int DayCrossHour = 15;

	private static readonly int DayCrossMinute;

	private DateTime m_nextGetInfoTime;

	private DateTime m_nextGetLoginBonusTime;

	private UpdateInfoCallback m_callbackDayCross;

	private UpdateInfoCallback m_callbackDailyMission;

	private UpdateInfoCallback m_callbackDailyMissionForOneDay;

	private UpdateInfoCallback m_callbackLoginBonus;

	public static ServerDayCrossWatcher Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool IsDayCross()
	{
		DateTime currentTime = NetBase.GetCurrentTime();
		if (DateTime.Compare(currentTime, m_nextGetInfoTime) >= 0)
		{
			return true;
		}
		return false;
	}

	public bool IsDaylyMissionEnd()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			DateTime currentTime = NetBase.GetCurrentTime();
			DateTime endDailyMissionDate = ServerInterface.PlayerState.m_endDailyMissionDate;
			if (DateTime.Compare(currentTime, endDailyMissionDate) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsDaylyMissionChallengeEnd()
	{
		DateTime currentTime = NetBase.GetCurrentTime();
		DateTime t = currentTime.AddDays(1.0);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			t = ServerInterface.DailyChallengeState.m_chalEndTime;
		}
		if (DateTime.Compare(currentTime, t) >= 0)
		{
			return true;
		}
		return false;
	}

	public bool IsLoginBonusDayCross()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			DateTime currentTime = NetBase.GetCurrentTime();
			if (DateTime.Compare(currentTime, m_nextGetLoginBonusTime) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateClientInfosByDayCross(UpdateInfoCallback callback)
	{
		if (callback == null)
		{
			return;
		}
		m_callbackDayCross = callback;
		if (!IsDayCross())
		{
			if (m_callbackDayCross != null)
			{
				m_callbackDayCross(new MsgDayCross());
				m_callbackDayCross = null;
			}
			return;
		}
		CalcNextGetInfoTime();
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetWheelOptions(base.gameObject);
		}
	}

	public void UpdateDailyMissionForOneDay(UpdateInfoCallback callback)
	{
		if (callback == null)
		{
			return;
		}
		m_callbackDailyMissionForOneDay = callback;
		if (!IsDaylyMissionEnd())
		{
			if (m_callbackDailyMissionForOneDay != null)
			{
				m_callbackDailyMissionForOneDay(new MsgDayCross());
				m_callbackDailyMissionForOneDay = null;
			}
		}
		else
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerRetrievePlayerState(base.gameObject);
			}
		}
	}

	public void UpdateDailyMissionInfoByChallengeEnd(UpdateInfoCallback callback)
	{
		if (callback == null)
		{
			return;
		}
		m_callbackDailyMission = callback;
		if (!IsDaylyMissionChallengeEnd())
		{
			if (m_callbackDailyMission != null)
			{
				m_callbackDailyMission(new MsgDayCross());
				m_callbackDailyMission = null;
			}
		}
		else
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetDailyMissionData(base.gameObject);
			}
		}
	}

	public void UpdateLoginBonusEnd(UpdateInfoCallback callback)
	{
		if (callback == null)
		{
			return;
		}
		m_callbackLoginBonus = callback;
		if (!IsLoginBonusDayCross())
		{
			if (m_callbackLoginBonus != null)
			{
				m_callbackLoginBonus(new MsgDayCross());
				m_callbackLoginBonus = null;
			}
			return;
		}
		CalcNextGetLoginBonusTime();
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerLoginBonus(base.gameObject);
		}
	}

	private void CalcNextGetInfoTime()
	{
		DateTime dateTime = DateTime.Today.AddHours(DayCrossHour).AddMinutes(DayCrossMinute);
		DateTime currentTime = NetBase.GetCurrentTime();
		if (DateTime.Compare(currentTime, dateTime) < 0)
		{
			m_nextGetInfoTime = dateTime;
		}
		else
		{
			DateTime dateTime2 = m_nextGetInfoTime = dateTime.AddDays(1.0);
		}
	}

	private void CalcNextGetLoginBonusTime()
	{
		DateTime dateTime = DateTime.Today.AddHours(DayCrossHour).AddMinutes(DayCrossMinute);
		DateTime currentTime = NetBase.GetCurrentTime();
		if (DateTime.Compare(currentTime, dateTime) < 0)
		{
			dateTime.AddMinutes(UnityEngine.Random.Range(1, 30));
			m_nextGetInfoTime = dateTime;
		}
		else
		{
			DateTime nextGetInfoTime = dateTime.AddDays(1.0);
			nextGetInfoTime.AddMinutes(UnityEngine.Random.Range(1, 30));
			m_nextGetInfoTime = nextGetInfoTime;
		}
	}

	private void Start()
	{
		if (m_instance == null)
		{
			m_instance = this;
			m_nextGetInfoTime = NetUtil.GetLocalDateTime(0L);
			m_nextGetLoginBonusTime = NetUtil.GetLocalDateTime(0L);
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void Update()
	{
	}

	private void ServerGetWheelOptions_Succeeded(MsgGetWheelOptionsSucceed msg)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetCampaignList(base.gameObject);
		}
	}

	private void GetCampaignList_Succeeded(MsgGetCampaignListSucceed msg)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventList(base.gameObject);
		}
	}

	private void ServerGetEventList_Succeeded(MsgGetEventListSucceed msg)
	{
		if (m_callbackDayCross != null)
		{
			MsgDayCross msgDayCross = new MsgDayCross();
			msgDayCross.ServerConnect = true;
			m_callbackDayCross(msgDayCross);
			m_callbackDayCross = null;
		}
	}

	private void ServerGetDailyMissionData_Succeeded(MsgGetDailyMissionDataSucceed msg)
	{
		if (m_callbackDailyMission != null)
		{
			MsgDayCross msgDayCross = new MsgDayCross();
			msgDayCross.ServerConnect = true;
			m_callbackDailyMission(msgDayCross);
			m_callbackDailyMission = null;
		}
	}

	private void ServerRetrievePlayerState_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		if (m_callbackDailyMissionForOneDay != null)
		{
			MsgDayCross msgDayCross = new MsgDayCross();
			msgDayCross.ServerConnect = true;
			m_callbackDailyMissionForOneDay(msgDayCross);
			m_callbackDailyMissionForOneDay = null;
		}
	}

	private void ServerLoginBonus_Succeeded(MsgLoginBonusSucceed msg)
	{
		if (m_callbackLoginBonus != null)
		{
			MsgDayCross msgDayCross = new MsgDayCross();
			msgDayCross.ServerConnect = true;
			m_callbackLoginBonus(msgDayCross);
			m_callbackLoginBonus = null;
		}
	}
}
