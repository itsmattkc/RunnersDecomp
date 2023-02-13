using SaveData;
using System;
using System.Diagnostics;
using Text;
using UnityEngine;

public class PushNoticeManager : MonoBehaviour
{
	private static PushNoticeManager instance;

	public static PushNoticeManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(PushNoticeManager)) as PushNoticeManager);
			}
			return instance;
		}
	}

	private void Awake()
	{
		CheckInstance();
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		LocalNotification.Initialize();
	}

	private bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
			LocalNotification.OnActive();
		}
		else
		{
			PushNotice(GetSecondsToFullChallenge());
		}
	}

	private void OnApplicationQuit()
	{
		PushNotice(GetSecondsToFullChallenge());
	}

	private void PushNotice(int secondsToFullChallenge)
	{
		if (SystemSaveManager.Instance != null && SystemSaveManager.Instance.GetSystemdata().pushNoticeFlags.Test(1) && secondsToFullChallenge > 0)
		{
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "PushNotice", "challenge_notification").text;
			LocalNotification.RegisterNotification(secondsToFullChallenge, text);
		}
	}

	private static int GetSecondsToFullChallenge()
	{
		int result = -1;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerSettingState settingState = ServerInterface.SettingState;
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (settingState != null && playerState != null)
			{
				int energyRecoveryMax = settingState.m_energyRecoveryMax;
				result = 0;
				if (NetUtil.GetUnixTime(playerState.m_energyRenewsAt) != 0L && playerState.m_numEnergy < energyRecoveryMax)
				{
					DateTime d = playerState.m_energyRenewsAt.AddSeconds(settingState.m_energyRefreshTime * (energyRecoveryMax - playerState.m_numEnergy - 1));
					result = (int)(d - NetUtil.GetCurrentTime()).TotalSeconds;
				}
			}
		}
		return result;
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}
}
