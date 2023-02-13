using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIDebugMenuForceDrawRaidboss : UIDebugMenuTask
{
	private enum TextType
	{
		EVENTID,
		SCORE,
		NUM
	}

	private delegate void NetworkRequestSuccessCallback();

	private delegate void NetworkRequestFailedCallback(ServerInterface.StatusCode statusCode);

	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField[] m_TextFields = new UIDebugMenuTextField[2];

	private UIDebugMenuTextBox m_textBox;

	private string[] DefaultTextList = new string[2]
	{
		"イベントID",
		"スコア"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 250f, 50f),
		new Rect(200f, 300f, 250f, 50f)
	};

	private NetBase m_networkRequest;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(200f, 450f, 150f, 50f), "Decide", base.gameObject);
		for (int i = 0; i < 2; i++)
		{
			m_TextFields[i] = base.gameObject.AddComponent<UIDebugMenuTextField>();
			m_TextFields[i].Setup(RectList[i], DefaultTextList[i]);
		}
		m_TextFields[0].text = UIDebugMenuServerDefine.DefaultRaidbossId;
		m_textBox = base.gameObject.AddComponent<UIDebugMenuTextBox>();
		m_textBox.Setup(new Rect(500f, 100f, 400f, 500f), string.Empty);
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
		if (m_textBox != null)
		{
			m_textBox.SetActive(false);
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
		if (m_textBox != null)
		{
			m_textBox.SetActive(true);
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
				UIDebugMenuTextField uIDebugMenuTextField = m_TextFields[i];
				int result;
				if (!(uIDebugMenuTextField == null) && !int.TryParse(uIDebugMenuTextField.text, out result))
				{
					return;
				}
			}
			m_networkRequest = new NetDebugForceDrawRaidboss(int.Parse(m_TextFields[0].text), long.Parse(m_TextFields[1].text));
			StartCoroutine(NetworkRequest(m_networkRequest, ForceDrawRaidbossEndCallback, NetworkFailedCallback));
		}
	}

	private IEnumerator NetworkRequest(NetBase request, NetworkRequestSuccessCallback successCallback, NetworkRequestFailedCallback failedCallback)
	{
		request.Request();
		while (request.IsExecuting())
		{
			yield return null;
		}
		if (request.IsSucceeded())
		{
			if (successCallback != null)
			{
				successCallback();
			}
		}
		else if (failedCallback != null)
		{
			failedCallback(request.resultStCd);
		}
	}

	private void ForceDrawRaidbossEndCallback()
	{
		m_networkRequest = new NetServerGetEventUserRaidBossList(int.Parse(m_TextFields[0].text));
		StartCoroutine(NetworkRequest(m_networkRequest, GetEventUserRaidBossListEndCallback, NetworkFailedCallback));
	}

	private void GetEventUserRaidBossListEndCallback()
	{
		NetServerGetEventUserRaidBossList netServerGetEventUserRaidBossList = m_networkRequest as NetServerGetEventUserRaidBossList;
		if (netServerGetEventUserRaidBossList == null)
		{
			return;
		}
		List<ServerEventRaidBossState> userRaidBossList = netServerGetEventUserRaidBossList.UserRaidBossList;
		if (userRaidBossList == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ServerEventRaidBossState item in userRaidBossList)
		{
			if (item != null)
			{
				stringBuilder.Append("BossId: " + item.Id);
				stringBuilder.AppendLine();
				stringBuilder.Append("  Level: " + item.Level);
				stringBuilder.AppendLine();
				stringBuilder.Append("  Rarity: " + item.Rarity);
				stringBuilder.AppendLine();
				stringBuilder.Append("  HitPoint: " + item.HitPoint);
				stringBuilder.AppendLine();
				stringBuilder.Append("  MaxHitPoint: " + item.MaxHitPoint);
				stringBuilder.AppendLine();
				stringBuilder.Append("  Status: " + item.GetStatusType());
				stringBuilder.AppendLine();
				stringBuilder.Append("  EscapeAt: " + item.EscapeAt.ToString());
				stringBuilder.AppendLine();
				stringBuilder.Append("  EncounterName: " + item.EncounterName);
				stringBuilder.AppendLine();
				stringBuilder.Append("  Encounter: " + item.Encounter);
				stringBuilder.AppendLine();
				stringBuilder.Append("  Crowded: " + item.Crowded);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
		}
		m_textBox.text = stringBuilder.ToString();
	}

	private void NetworkFailedCallback(ServerInterface.StatusCode statusCode)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Error!!!!!!!!!");
		stringBuilder.AppendLine();
		stringBuilder.Append("StatusCode = " + statusCode);
		stringBuilder.AppendLine();
		m_textBox.text = stringBuilder.ToString();
	}
}
