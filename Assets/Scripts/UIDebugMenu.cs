using App;
using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenu : UIDebugMenuTask
{
	private enum MenuType
	{
		GAME,
		TITLE,
		AHIEVEMENT,
		EVENT,
		SERVER,
		ACTIVE_DEBUG_SERVER,
		MIGRATION,
		USER_MOVE,
		FACEBOOK,
		NOTIFICATION,
		CAMPAIGN,
		LOCAL1,
		LOCAL2,
		LOCAL3,
		DEVELOP,
		NUM
	}

	private List<string> MenuObjName = new List<string>
	{
		"Game",
		"Title",
		"Achievement",
		"Event",
		"Server",
		"for Dev./ActiveDebugServer",
		"for Dev./migration",
		"for Dev./user_move",
		"Facebook",
		"Notification",
		"Campaign",
		"LOCAL1",
		"LOCAL2",
		"LOCAL3",
		"DEVELOP"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 150f, 50f),
		new Rect(400f, 200f, 150f, 50f),
		new Rect(600f, 200f, 150f, 50f),
		new Rect(200f, 260f, 150f, 50f),
		new Rect(200f, 380f, 150f, 50f),
		new Rect(200f, 500f, 175f, 50f),
		new Rect(600f, 330f, 150f, 50f),
		new Rect(600f, 390f, 150f, 50f),
		new Rect(600f, 470f, 150f, 50f),
		new Rect(800f, 200f, 150f, 50f),
		new Rect(400f, 260f, 150f, 50f),
		new Rect(200f, 570f, 150f, 50f),
		new Rect(400f, 570f, 150f, 50f),
		new Rect(600f, 570f, 150f, 50f),
		new Rect(800f, 570f, 150f, 50f)
	};

	private UIDebugMenuButtonList m_buttonList;

	private UIDebugMenuTextField m_LoginIDField;

	private NetDebugLogin m_debugLogin;

	private UIDebugMenuTextField m_debugServerUrlField;

	private string m_clickedButtonName;

	protected override void OnStartFromTask()
	{
		m_buttonList = base.gameObject.AddComponent<UIDebugMenuButtonList>();
		for (int i = 0; i < 15; i++)
		{
			string name = MenuObjName[i];
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, name);
			if (!(gameObject == null))
			{
				string childName = MenuObjName[i];
				m_buttonList.Add(RectList, MenuObjName, base.gameObject);
				AddChild(childName, gameObject);
			}
		}
		m_LoginIDField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		string empty = string.Empty;
		string gameID = SystemSaveManager.GetGameID();
		empty = gameID;
		m_LoginIDField.Setup(new Rect(200f, 350f, 375f, 30f), "Enter Login ID.", empty);
		GameObject gameObject2 = GameObject.Find("DebugGameObject");
		if (gameObject2 != null)
		{
			gameObject2.AddComponent<LoadURLComponent>();
		}
		m_debugServerUrlField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_debugServerUrlField.Setup(new Rect(200f, 470f, 375f, 30f), "forDev:デバッグサーバーURL入力(例：http://157.109.83.27/sdl/)");
		StartCoroutine(InitCoroutine());
		TransitionFrom();
	}

	protected override void OnTransitionTo()
	{
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(false);
		}
		if (m_LoginIDField != null)
		{
			m_LoginIDField.SetActive(false);
		}
		if (m_debugServerUrlField != null)
		{
			m_debugServerUrlField.SetActive(false);
		}
	}

	protected override void OnTransitionFrom()
	{
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(true);
		}
		if (m_LoginIDField != null)
		{
			m_LoginIDField.SetActive(true);
		}
		if (m_debugServerUrlField != null)
		{
			m_debugServerUrlField.SetActive(true);
		}
	}

	private void OnClicked(string name)
	{
		m_clickedButtonName = name;
		bool flag = true;
		if (m_clickedButtonName == MenuObjName[11] || m_clickedButtonName == MenuObjName[12] || m_clickedButtonName == MenuObjName[13] || m_clickedButtonName == MenuObjName[14] || m_clickedButtonName == MenuObjName[5] || m_clickedButtonName == MenuObjName[1] || m_clickedButtonName == MenuObjName[8] || m_clickedButtonName == MenuObjName[7])
		{
			flag = false;
		}
		if (flag)
		{
			ServerSessionWatcher serverSessionWatcher = GameObjectUtil.FindGameObjectComponent<ServerSessionWatcher>("NetMonitor");
			if (serverSessionWatcher != null)
			{
				serverSessionWatcher.ValidateSession(ServerSessionWatcher.ValidateType.LOGIN_OR_RELOGIN, ValidateSessionCallback);
			}
		}
		else
		{
			ValidateSessionCallback(true);
		}
	}

	private void ValidateSessionCallback(bool isSuccess)
	{
		if (m_clickedButtonName == MenuObjName[11])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL1;
		}
		else if (m_clickedButtonName == MenuObjName[12])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL2;
		}
		else if (m_clickedButtonName == MenuObjName[13])
		{
			Env.actionServerType = Env.ActionServerType.LOCAL3;
		}
		else if (m_clickedButtonName == MenuObjName[14])
		{
			Env.actionServerType = Env.ActionServerType.DEVELOP;
		}
		if (m_clickedButtonName == MenuObjName[11] || m_clickedButtonName == MenuObjName[12] || m_clickedButtonName == MenuObjName[13] || m_clickedButtonName == MenuObjName[14])
		{
			NetBaseUtil.DebugServerUrl = null;
			string actionServerURL = NetBaseUtil.ActionServerURL;
		}
		else if (m_clickedButtonName == MenuObjName[5])
		{
			string actionServerURL = NetBaseUtil.DebugServerUrl = m_debugServerUrlField.text;
			DebugSaveServerUrl.SaveURL(actionServerURL);
		}
		else if (m_clickedButtonName == MenuObjName[1])
		{
			Application.LoadLevel(TitleDefine.TitleSceneName);
		}
		else
		{
			TransitionToChild(m_clickedButtonName);
		}
	}

	protected override void OnGuiFromTask()
	{
		GUI.Label(new Rect(400f, 510f, 300f, 60f), "現在のURL\n" + NetBaseUtil.ActionServerURL);
	}

	private IEnumerator InitCoroutine()
	{
		yield return null;
		yield return null;
		if (DebugSaveServerUrl.Url != null)
		{
			m_debugServerUrlField.text = DebugSaveServerUrl.Url;
		}
		else
		{
			m_debugServerUrlField.text = NetBaseUtil.ActionServerURL;
		}
	}
}
