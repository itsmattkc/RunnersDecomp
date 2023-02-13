using Message;
using SaveData;
using System;
using UnityEngine;

public class ServerSessionWatcher : MonoBehaviour
{
	public enum ValidateType
	{
		NONE = -1,
		PRELOGIN,
		LOGIN,
		LOGIN_OR_RELOGIN,
		COUNT
	}

	private enum SessionState
	{
		NONE = -1,
		NEED_LOGIN,
		VALID,
		COUNT
	}

	public delegate void ValidateSessionEndCallback(bool isSuccess);

	private ValidateType m_validateType;

	private ValidateSessionEndCallback m_callback;

	private string m_loginKey = string.Empty;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ValidateSession(ValidateType type, ValidateSessionEndCallback callback, string loginKey)
	{
		m_loginKey = loginKey;
		ValidateSession(type, callback);
	}

	public void ValidateSession(ValidateType type, ValidateSessionEndCallback callback)
	{
		m_validateType = type;
		m_callback = callback;
		switch (CheckSessionState())
		{
		case SessionState.NEED_LOGIN:
			switch (m_validateType)
			{
			case ValidateType.PRELOGIN:
				Login();
				break;
			case ValidateType.LOGIN:
				Login();
				break;
			case ValidateType.LOGIN_OR_RELOGIN:
				Login();
				break;
			default:
				Fail();
				break;
			}
			break;
		case SessionState.VALID:
			Success();
			break;
		default:
			Fail();
			break;
		}
	}

	public void InvalidateSession()
	{
		ServerLoginState loginState = ServerInterface.LoginState;
		if (loginState != null)
		{
			loginState.sessionTimeLimit = 0;
			loginState.seqNum = 0uL;
		}
	}

	private SessionState CheckSessionState()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface == null)
		{
			return SessionState.NEED_LOGIN;
		}
		ServerLoginState loginState = ServerInterface.LoginState;
		DateTime localDateTime = NetUtil.GetLocalDateTime(loginState.sessionTimeLimit);
		DateTime currentTime = NetUtil.GetCurrentTime();
		if (NetUtil.IsAlreadySessionTimeOut(localDateTime, currentTime))
		{
			return SessionState.NEED_LOGIN;
		}
		return SessionState.VALID;
	}

	private void Success()
	{
		Debug.Log("ServerSessionWatcher:Succeeded");
		m_callback(true);
	}

	private void Fail()
	{
		Debug.Log("ServerSessionWatcher:Failed");
		m_callback(false);
	}

	private void Login()
	{
		Debug.Log("ServerSessionWatcher:Login");
		if (!string.IsNullOrEmpty(m_loginKey))
		{
			string systemSaveDataGameId = TitleUtil.GetSystemSaveDataGameId();
			string systemSaveDataPassword = TitleUtil.GetSystemSaveDataPassword();
			string password = NetUtil.CalcMD5String(m_loginKey + ":dho5v5yy7n2uswa5iblb:" + systemSaveDataGameId + ":" + systemSaveDataPassword);
			ServerInterface serverInterface = GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
			if (serverInterface != null)
			{
				serverInterface.RequestServerLogin(systemSaveDataGameId, password, base.gameObject);
			}
		}
		else
		{
			string systemSaveDataGameId2 = TitleUtil.GetSystemSaveDataGameId();
			ServerInterface serverInterface2 = GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
			if (serverInterface2 != null)
			{
				serverInterface2.RequestServerLogin(systemSaveDataGameId2, string.Empty, base.gameObject);
			}
		}
	}

	private void Relogin()
	{
		Debug.Log("ServerSessionWatcher:ReLogin");
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerReLogin(base.gameObject);
		}
	}

	private void ServerLogin_Succeeded(MsgLoginSucceed msg)
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		bool flag = TitleUtil.SetSystemSaveDataGameId(msg.m_userId);
		bool flag2 = TitleUtil.SetSystemSaveDataPassword(msg.m_password);
		if ((flag || flag2) && instance != null)
		{
			instance.SaveSystemData();
		}
		m_loginKey = string.Empty;
		Success();
	}

	private void ServerLogin_Failed(MessageBase msg)
	{
		if (msg == null)
		{
			return;
		}
		MsgServerPasswordError msgServerPasswordError = msg as MsgServerPasswordError;
		if (msgServerPasswordError == null)
		{
			return;
		}
		m_loginKey = msgServerPasswordError.m_key;
		bool flag = TitleUtil.SetSystemSaveDataGameId(msgServerPasswordError.m_userId);
		bool flag2 = TitleUtil.SetSystemSaveDataPassword(msgServerPasswordError.m_password);
		if (flag || flag2)
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				instance.SaveSystemData();
			}
		}
		if (flag)
		{
		}
		if (m_validateType == ValidateType.PRELOGIN)
		{
			m_callback(true);
			return;
		}
		Debug.Log("GameModeTitle.UserId: " + msgServerPasswordError.m_userId);
		Debug.Log("GameModeTitle.Password: " + msgServerPasswordError.m_password);
		Login();
	}

	public void ServerReLogin_Succeeded()
	{
		Success();
	}
}
