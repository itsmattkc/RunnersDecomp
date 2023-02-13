using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuServerAddOpeMessage : UIDebugMenuTask
{
	private enum TextType
	{
		MESSAGE_KIND,
		INFO_ID,
		ITEM_ID,
		NUM_ITEM,
		ADDITIONAL_INFO1,
		ADDITIONAL_INFO2,
		MSG_CONTENT,
		NUM
	}

	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField[] m_TextFields = new UIDebugMenuTextField[7];

	private string[] DefaultTextList = new string[7]
	{
		"メッセージ種別( 0:全体 1:個別)",
		"お知らせID(全体へのプレゼント時の時に、入力可。個別時は0とする)",
		"運営配布のアイテムID",
		"運営配布のアイテム数",
		"アイテム追加情報_1(チャオであればLV)",
		"アイテム追加情報_2(チャオであればLVMAX時の取得スペシャルエッグ数)",
		"個別メッセージの内容"
	};

	private string[] DefaultParamTextList = new string[7]
	{
		"1",
		"0",
		"120000",
		"1",
		"0",
		"0",
		string.Empty
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(100f, 100f, 200f, 50f),
		new Rect(310f, 100f, 500f, 50f),
		new Rect(100f, 175f, 250f, 50f),
		new Rect(360f, 175f, 250f, 50f),
		new Rect(100f, 250f, 500f, 50f),
		new Rect(100f, 325f, 500f, 50f),
		new Rect(100f, 475f, 500f, 50f)
	};

	private NetDebugAddOpeMessage m_addOpeMsg;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(100f, 20f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(250f, 20f, 150f, 50f), "Decide", base.gameObject);
		for (int i = 0; i < 7; i++)
		{
			m_TextFields[i] = base.gameObject.AddComponent<UIDebugMenuTextField>();
			m_TextFields[i].Setup(RectList[i], DefaultTextList[i], DefaultParamTextList[i]);
		}
	}

	private string GetGameId()
	{
		return SystemSaveManager.GetGameID();
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
		for (int i = 0; i < 7; i++)
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
		for (int i = 0; i < 7; i++)
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
			NetDebugAddOpeMessage.OpeMsgInfo opeMsgInfo = new NetDebugAddOpeMessage.OpeMsgInfo();
			opeMsgInfo.userID = GetGameId();
			opeMsgInfo.messageKind = int.Parse(m_TextFields[0].text);
			opeMsgInfo.infoId = int.Parse(m_TextFields[1].text);
			opeMsgInfo.itemId = int.Parse(m_TextFields[2].text);
			opeMsgInfo.numItem = int.Parse(m_TextFields[3].text);
			opeMsgInfo.additionalInfo1 = int.Parse(m_TextFields[4].text);
			opeMsgInfo.additionalInfo2 = int.Parse(m_TextFields[5].text);
			opeMsgInfo.msgTitle = string.Empty;
			opeMsgInfo.msgContent = m_TextFields[6].text;
			opeMsgInfo.msgImageId = "0";
			m_addOpeMsg = new NetDebugAddOpeMessage(opeMsgInfo);
			m_addOpeMsg.Request();
		}
	}
}
