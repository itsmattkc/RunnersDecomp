using Message;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuGetMigrationPassword : UIDebugMenuTask
{
	private enum TextType
	{
		PASSWORD,
		NUM
	}

	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField[] m_TextFields = new UIDebugMenuTextField[1];

	private string[] DefaultTextList = new string[1]
	{
		"Decideを押すと、ここにパスワードが入ります。"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 450f, 50f)
	};

	private NetDebugUpdPointData m_upPoint;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(200f, 450f, 150f, 50f), "Decide", base.gameObject);
		for (int i = 0; i < 1; i++)
		{
			m_TextFields[i] = base.gameObject.AddComponent<UIDebugMenuTextField>();
			m_TextFields[i].Setup(RectList[i], DefaultTextList[i]);
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
		for (int i = 0; i < 1; i++)
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
		for (int i = 0; i < 1; i++)
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
		else if (name == "Decide")
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetMigrationPassword(null, base.gameObject);
			}
		}
	}

	private void ServerGetMigrationPassword_Succeeded(MsgGetMigrationPasswordSucceed msg)
	{
		UIDebugMenuTextField uIDebugMenuTextField = m_TextFields[0];
		uIDebugMenuTextField.text = msg.m_migrationPassword;
	}
}
