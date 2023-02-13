using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuServerAddMessage : UIDebugMenuTask
{
	private enum TextType
	{
		FROM_ID,
		TO_ID,
		NUM
	}

	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField[] m_TextFields = new UIDebugMenuTextField[2];

	private string[] DefaultTextList = new string[2]
	{
		"送信元のGameID",
		"送信先のGameID(エナジーをもらう側)"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 250f, 50f),
		new Rect(200f, 275f, 250f, 50f)
	};

	private NetDebugAddMessage m_addMsg;

	private string GetGameId()
	{
		return SystemSaveManager.GetGameID();
	}

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(200f, 450f, 150f, 50f), "Decide", base.gameObject);
		for (int i = 0; i < 2; i++)
		{
			m_TextFields[i] = base.gameObject.AddComponent<UIDebugMenuTextField>();
			if (i == 0)
			{
				m_TextFields[i].Setup(RectList[i], DefaultTextList[i], "0");
			}
			else
			{
				m_TextFields[i].Setup(RectList[i], DefaultTextList[i], GetGameId());
			}
		}
	}

	protected override void OnTransitionTo()
	{
		if (m_backButton != null)
		{
			m_backButton.SetActive(false);
		}
		if (m_decideButton != null)
		{
			m_decideButton.SetActive(false);
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(m_TextFields[i] == null))
			{
				m_TextFields[i].SetActive(false);
			}
		}
	}

	protected override void OnTransitionFrom()
	{
		if (m_backButton != null)
		{
			m_backButton.SetActive(true);
		}
		if (m_decideButton != null)
		{
			m_decideButton.SetActive(true);
		}
		for (int i = 0; i < 2; i++)
		{
			if (!(m_TextFields[i] == null))
			{
				m_TextFields[i].SetActive(true);
			}
		}
	}

	private void OnClicked(string name)
	{
		if (name == "Back")
		{
			TransitionToParent();
		}
		else
		{
			if (!(name == "Decide"))
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				UIDebugMenuTextField x = m_TextFields[i];
				if (x == null)
				{
				}
			}
			m_addMsg = new NetDebugAddMessage(m_TextFields[0].text, m_TextFields[1].text, 2);
			m_addMsg.Request();
		}
	}
}
