using Message;
using SaveData;
using UnityEngine;

public class AchievementIncentive : MonoBehaviour
{
	public enum State
	{
		Idle,
		Request,
		Succeeded,
		Failed
	}

	private State m_state;

	private void Start()
	{
	}

	public State GetState()
	{
		return m_state;
	}

	public void RequestServer()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			int achievementIncentiveCount = GetAchievementIncentiveCount();
			if (achievementIncentiveCount > 0)
			{
				loggedInServerInterface.RequestServerGetFacebookIncentive(3, achievementIncentiveCount, base.gameObject);
				m_state = State.Request;
			}
		}
	}

	private void ServerGetFacebookIncentive_Succeeded(MsgGetNormalIncentiveSucceed msg)
	{
		ResetAchievementIncentiveCount();
		if (SaveDataManager.Instance != null && SaveDataManager.Instance.ConnectData != null)
		{
			SaveDataManager.Instance.ConnectData.ReplaceMessageBox = true;
		}
		m_state = State.Succeeded;
	}

	private void ServerGetFacebookIncentive_Failed(MsgServerConnctFailed msg)
	{
		m_state = State.Failed;
	}

	private static SystemData GetSystemSaveData()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			return instance.GetSystemdata();
		}
		return null;
	}

	private static int GetAchievementIncentiveCount()
	{
		SystemData systemSaveData = GetSystemSaveData();
		if (systemSaveData != null)
		{
			return systemSaveData.achievementIncentiveCount;
		}
		return 0;
	}

	public static void AddAchievementIncentiveCount(int add)
	{
		if (add > 0)
		{
			SystemData systemSaveData = GetSystemSaveData();
			if (systemSaveData != null)
			{
				systemSaveData.achievementIncentiveCount += add;
				Save();
			}
		}
	}

	private static void ResetAchievementIncentiveCount()
	{
		SystemData systemSaveData = GetSystemSaveData();
		if (systemSaveData != null)
		{
			systemSaveData.achievementIncentiveCount = 0;
			Save();
		}
	}

	private static void Save()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			instance.SaveSystemData();
		}
	}
}
