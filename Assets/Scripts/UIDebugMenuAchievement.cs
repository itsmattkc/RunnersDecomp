using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuAchievement : UIDebugMenuTask
{
	private enum MenuType
	{
		ALL_RESET,
		ALL_OPEN,
		OUTPUT_INFO,
		SHOW,
		NUM
	}

	private List<string> MenuObjName = new List<string>
	{
		"AllReset",
		"FlagOn:AllOpen",
		"FlagOn:DebugInfo",
		"Show"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 150f, 50f),
		new Rect(200f, 300f, 150f, 50f),
		new Rect(400f, 300f, 150f, 50f),
		new Rect(200f, 400f, 150f, 50f)
	};

	private static string STR_BACK = "Back";

	private UIDebugMenuButtonList m_buttonList;

	private UIDebugMenuButton m_backButton;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 100f, 50f), STR_BACK, base.gameObject);
		m_buttonList = base.gameObject.AddComponent<UIDebugMenuButtonList>();
		for (int i = 0; i < 4; i++)
		{
			string name = MenuObjName[i];
			GameObject x = GameObjectUtil.FindChildGameObject(base.gameObject, name);
			if (!(x == null))
			{
				m_buttonList.Add(RectList, MenuObjName, base.gameObject);
			}
		}
	}

	protected override void OnGuiFromTask()
	{
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
		if (m_backButton != null)
		{
			m_backButton.SetActive(true);
		}
		if (m_buttonList != null)
		{
			m_buttonList.SetActive(true);
		}
	}

	private void OnClicked(string name)
	{
		if (name == STR_BACK)
		{
			TransitionToParent();
		}
		else if (name == MenuObjName[1])
		{
			AchievementManager.RequestDebugAllOpen(true);
		}
		else if (name == MenuObjName[0])
		{
			AchievementManager.RequestReset();
		}
		else if (name == MenuObjName[2])
		{
			AchievementManager.RequestDebugInfo(true);
		}
		else if (name == MenuObjName[3])
		{
			AchievementManager.RequestShowAchievementsUI();
		}
	}
}
