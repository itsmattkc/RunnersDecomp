using Message;
using Text;
using UnityEngine;

public class ui_option_tutorial_scroll : MonoBehaviour
{
	[SerializeField]
	private UILabel m_textLabel;

	[SerializeField]
	private UILabel m_shadowTextLabel;

	[SerializeField]
	private GameObject m_btnObj;

	private window_tutorial_other_character m_window;

	private window_tutorial.ScrollInfo m_scrollInfo;

	private bool m_openWindow;

	public bool OpenWindow
	{
		get
		{
			return m_openWindow;
		}
	}

	private void Start()
	{
		base.enabled = false;
		if (m_btnObj != null)
		{
			UIButtonMessage component = m_btnObj.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.enabled = true;
				component.trigger = UIButtonMessage.Trigger.OnClick;
				component.target = base.gameObject;
				component.functionName = "OnClickOptionTutorialScroll";
			}
		}
	}

	public void Update()
	{
		if (m_window != null && m_window.IsEnd)
		{
			m_openWindow = false;
			m_window.gameObject.SetActive(false);
			base.enabled = false;
			if (m_scrollInfo != null && m_scrollInfo.Parent != null)
			{
				m_scrollInfo.Parent.SetCloseBtnColliderTrigger(false);
			}
		}
	}

	public void UpdateView(window_tutorial.ScrollInfo info)
	{
		m_scrollInfo = info;
		SetText();
	}

	public void SetText()
	{
		if (m_scrollInfo == null)
		{
			return;
		}
		string text = null;
		switch (m_scrollInfo.DispType)
		{
		case window_tutorial.DisplayType.TUTORIAL:
			text = TextUtility.GetCommonText("Option", "tutorial");
			break;
		case window_tutorial.DisplayType.QUICK:
			text = TextUtility.GetCommonText("Tutorial", "caption_quickmode_tutorial");
			break;
		case window_tutorial.DisplayType.CHARA:
			text = TextUtility.GetCommonText("CharaName", CharaName.Name[(int)m_scrollInfo.Chara]);
			break;
		case window_tutorial.DisplayType.BOSS_MAP_1:
			text = BossTypeUtil.GetTextCommonBossName(BossType.MAP1);
			break;
		case window_tutorial.DisplayType.BOSS_MAP_2:
			text = BossTypeUtil.GetTextCommonBossName(BossType.MAP2);
			break;
		case window_tutorial.DisplayType.BOSS_MAP_3:
			text = BossTypeUtil.GetTextCommonBossName(BossType.MAP3);
			break;
		}
		if (text != null)
		{
			if (m_textLabel != null)
			{
				m_textLabel.text = text;
			}
			if (m_shadowTextLabel != null)
			{
				m_shadowTextLabel.text = text;
			}
		}
	}

	private void OnClickOptionTutorialScroll()
	{
		if (m_scrollInfo == null)
		{
			return;
		}
		if (m_scrollInfo.DispType == window_tutorial.DisplayType.TUTORIAL)
		{
			HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.STAGE);
			return;
		}
		if (m_window == null)
		{
			GameObject loadMenuChildObject = HudMenuUtility.GetLoadMenuChildObject("window_tutorial_other_character", true);
			if (loadMenuChildObject != null)
			{
				m_window = loadMenuChildObject.GetComponent<window_tutorial_other_character>();
			}
		}
		if (m_window != null)
		{
			m_window.SetScrollInfo(m_scrollInfo);
			m_window.PlayOpenWindow();
			m_openWindow = true;
			base.enabled = true;
			if (m_scrollInfo.Parent != null)
			{
				m_scrollInfo.Parent.SetCloseBtnColliderTrigger(true);
			}
		}
	}
}
