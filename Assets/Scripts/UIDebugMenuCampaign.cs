using System;
using UnityEngine;

public class UIDebugMenuCampaign : UIDebugMenuTask
{
	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField m_CampaignField;

	private UIDebugMenuTextField m_IdField;

	private UIDebugMenuTextField m_StartTimeField;

	private UIDebugMenuTextField m_EndTimeField;

	private UIDebugMenuTextField m_ContentField;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(400f, 100f, 150f, 50f), "Decide", base.gameObject);
		m_CampaignField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_CampaignField.Setup(new Rect(200f, 200f, 350f, 50f), "キャンペーンタイプ(IF定義のキャンペーン番号)");
		m_IdField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_IdField.Setup(new Rect(200f, 280f, 350f, 50f), "ID(必要な場合のみ)");
		m_IdField.text = "0";
		m_StartTimeField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_StartTimeField.Setup(new Rect(200f, 360f, 350f, 50f), "何分前に始まった？");
		m_StartTimeField.text = "1";
		m_EndTimeField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_EndTimeField.Setup(new Rect(200f, 440f, 350f, 50f), "何分後に終わる？");
		m_EndTimeField.text = "10";
		m_ContentField = base.gameObject.AddComponent<UIDebugMenuTextField>();
		m_ContentField.Setup(new Rect(200f, 520f, 350f, 50f), "キャンペーン値");
		m_ContentField.text = "10";
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
		if (m_CampaignField != null)
		{
			m_CampaignField.SetActive(false);
		}
		if (m_IdField != null)
		{
			m_IdField.SetActive(false);
		}
		if (m_StartTimeField != null)
		{
			m_StartTimeField.SetActive(false);
		}
		if (m_EndTimeField != null)
		{
			m_EndTimeField.SetActive(false);
		}
		if (m_ContentField != null)
		{
			m_ContentField.SetActive(false);
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
		if (m_CampaignField != null)
		{
			m_CampaignField.SetActive(true);
		}
		if (m_IdField != null)
		{
			m_IdField.SetActive(true);
		}
		if (m_StartTimeField != null)
		{
			m_StartTimeField.SetActive(true);
		}
		if (m_EndTimeField != null)
		{
			m_EndTimeField.SetActive(true);
		}
		if (m_ContentField != null)
		{
			m_ContentField.SetActive(true);
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
			ServerCampaignState campaignState = ServerInterface.CampaignState;
			if (campaignState != null)
			{
				ServerCampaignData serverCampaignData = new ServerCampaignData();
				serverCampaignData.campaignType = (Constants.Campaign.emType)int.Parse(m_CampaignField.text);
				serverCampaignData.id = int.Parse(m_IdField.text);
				serverCampaignData.beginDate = 0L;
				serverCampaignData.endDate = 0L;
				DateTime currentTime = NetBase.GetCurrentTime();
				DateTime dateTime = currentTime.AddMinutes(0.0 - double.Parse(m_StartTimeField.text));
				DateTime dateTime2 = currentTime.AddMinutes(double.Parse(m_EndTimeField.text));
				serverCampaignData.beginDate = NetUtil.GetUnixTime(dateTime);
				serverCampaignData.endDate = NetUtil.GetUnixTime(dateTime2);
				serverCampaignData.iContent = int.Parse(m_ContentField.text);
				serverCampaignData.iSubContent = 0;
				campaignState.RegistCampaign(serverCampaignData);
			}
		}
	}
}
