using UnityEngine;

public class FirstLaunchUserName : MonoBehaviour
{
	private SettingUserName m_settingName;

	public static bool IsFirstLaunch
	{
		get
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				ServerSettingState settingState = ServerInterface.SettingState;
				if (settingState.m_userName != string.Empty)
				{
					return false;
				}
			}
			return true;
		}
		private set
		{
		}
	}

	public bool IsEndPlay
	{
		get
		{
			if (m_settingName == null)
			{
				return true;
			}
			return m_settingName.IsEndPlay();
		}
		private set
		{
		}
	}

	public void Setup(string anchorName)
	{
		if (anchorName != null && IsFirstLaunch)
		{
			m_settingName = base.gameObject.GetComponent<SettingUserName>();
			if (m_settingName == null)
			{
				m_settingName = base.gameObject.AddComponent<SettingUserName>();
			}
			m_settingName.SetCancelButtonUseFlag(false);
			m_settingName.Setup(anchorName);
		}
	}

	public void PlayStart()
	{
		if (IsFirstLaunch && !(m_settingName == null))
		{
			m_settingName.PlayStart();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
