using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuServer : UIDebugMenuTask
{
	private enum MenuType
	{
		UPGRADE_CHAO,
		UPGRADE_CHARACTER,
		UP_POINT,
		GET_MIGRATION_PASSWORD,
		UPDATE_DAILY_MISSION,
		ADD_MESSAGE,
		ADD_OPE_MESSAGE,
		UP_MILEAGE_MAP_DATA,
		UP_MILEAGEMAPPRODUCTION,
		RING_EXCHANGE,
		FORCE_DRAW_RAIDBOSS,
		UPDATE_USER_DATA,
		NUM
	}

	private List<string> MenuObjName = new List<string>
	{
		"upgradeChao",
		"upGradeCharacter",
		"upPoint",
		"getMigrationPassword",
		"updateDailyMission",
		"addMessage",
		"addOpeMessage",
		"updateMileageMapData",
		"updateMileageMapDataProduction",
		"ringExchange",
		"forceDrawRaidboss",
		"updateUserData"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(300f, 200f, 150f, 50f),
		new Rect(100f, 300f, 150f, 50f),
		new Rect(300f, 300f, 150f, 50f),
		new Rect(100f, 400f, 150f, 50f),
		new Rect(300f, 400f, 150f, 50f),
		new Rect(500f, 200f, 150f, 50f),
		new Rect(500f, 300f, 150f, 50f),
		new Rect(500f, 400f, 150f, 50f),
		new Rect(700f, 400f, 220f, 50f),
		new Rect(900f, 500f, 150f, 50f),
		new Rect(100f, 500f, 150f, 50f),
		new Rect(500f, 500f, 150f, 50f)
	};

	private UIDebugMenuButtonList m_buttonList;

	private UIDebugMenuButton m_backButton;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 150f, 50f), "Back", base.gameObject);
		m_buttonList = base.gameObject.AddComponent<UIDebugMenuButtonList>();
		for (int i = 0; i < 12; i++)
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
	}

	protected override void OnTransitionTo()
	{
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(false);
		}
		if (m_backButton != null)
		{
			m_backButton.SetActive(false);
		}
	}

	protected override void OnTransitionFrom()
	{
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(true);
		}
		if (m_backButton != null)
		{
			m_backButton.SetActive(true);
		}
	}

	private void OnClicked(string name)
	{
		if (name == "Back")
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerRetrievePlayerState(base.gameObject);
			}
			TransitionToParent();
		}
		else if (name == "getMigrationPassword")
		{
			ServerInterface loggedInServerInterface2 = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface2 != null)
			{
				loggedInServerInterface2.RequestServerGetMigrationPassword(null, base.gameObject);
			}
		}
		else
		{
			TransitionToChild(name);
		}
	}
}
