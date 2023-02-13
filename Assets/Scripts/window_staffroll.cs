using DataTable;
using Text;
using UnityEngine;

public class window_staffroll : WindowBase
{
	private const string ATTACH_ANTHOR_NAME = "UI Root (2D)/Camera/menu_Anim/OptionUI/Anchor_5_MC";

	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private UIScrollBar m_scrollBar;

	[SerializeField]
	private UILabel m_headerTextLabel;

	[SerializeField]
	private UILabel m_staffrollLabel;

	private GameObject m_parserObject;

	private string m_creditText = string.Empty;

	private string m_copyrightText = string.Empty;

	private bool m_creditFlag;

	private bool m_isEnd;

	private bool m_init;

	private UIPlayAnimation m_uiAnimation;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
	}

	private void Initialize()
	{
		if (m_init)
		{
			return;
		}
		OptionMenuUtility.TranObj(base.gameObject);
		if (m_closeBtn != null)
		{
			UIPlayAnimation component = m_closeBtn.GetComponent<UIPlayAnimation>();
			if (component != null)
			{
				EventDelegate.Add(component.onFinished, OnFinishedAnimationCallback, false);
			}
			UIButtonMessage component2 = m_closeBtn.GetComponent<UIButtonMessage>();
			if (component2 == null)
			{
				m_closeBtn.AddComponent<UIButtonMessage>();
				component2 = m_closeBtn.GetComponent<UIButtonMessage>();
			}
			if (component2 != null)
			{
				component2.enabled = true;
				component2.trigger = UIButtonMessage.Trigger.OnClick;
				component2.target = base.gameObject;
				component2.functionName = "OnClickCloseButton";
			}
		}
		if (m_scrollBar != null)
		{
			m_scrollBar.value = 0f;
		}
		if (m_staffrollLabel != null)
		{
			m_staffrollLabel.text = string.Empty;
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component3 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component3;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		m_init = true;
	}

	private void Update()
	{
		if (!(m_parserObject != null))
		{
			return;
		}
		HtmlParser component = m_parserObject.GetComponent<HtmlParser>();
		if (component != null && component.IsEndParse)
		{
			base.enabled = false;
			if (m_staffrollLabel != null)
			{
				m_staffrollLabel.text = component.ParsedString;
			}
			if (m_creditFlag)
			{
				m_creditText = component.ParsedString;
			}
			else
			{
				m_copyrightText = component.ParsedString;
			}
			Object.Destroy(m_parserObject);
			m_parserObject = null;
		}
	}

	public void SetStaffRollText()
	{
		Initialize();
		m_creditFlag = true;
		TextUtility.SetCommonText(m_headerTextLabel, "Option", "staff_credit");
		if (string.IsNullOrEmpty(m_creditText))
		{
			if (m_staffrollLabel != null)
			{
				m_staffrollLabel.text = string.Empty;
			}
			string webPageURL = NetUtil.GetWebPageURL(InformationDataTable.Type.CREDIT);
			m_parserObject = HtmlParserFactory.Create(webPageURL, HtmlParser.SyncType.TYPE_ASYNC, HtmlParser.SyncType.TYPE_ASYNC);
			base.enabled = true;
		}
		else if (m_staffrollLabel != null)
		{
			m_staffrollLabel.text = m_creditText;
		}
	}

	public void SetCopyrightText()
	{
		Initialize();
		m_creditFlag = false;
		TextUtility.SetCommonText(m_headerTextLabel, "Option", "copyright");
		if (string.IsNullOrEmpty(m_copyrightText))
		{
			if (m_staffrollLabel != null)
			{
				m_staffrollLabel.text = string.Empty;
			}
			string webPageURL = NetUtil.GetWebPageURL(InformationDataTable.Type.COPYRIGHT);
			m_parserObject = HtmlParserFactory.Create(webPageURL, HtmlParser.SyncType.TYPE_ASYNC, HtmlParser.SyncType.TYPE_ASYNC);
			base.enabled = true;
		}
		else if (m_staffrollLabel != null)
		{
			m_staffrollLabel.text = m_copyrightText;
		}
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
	}

	public void PlayOpenWindow()
	{
		m_isEnd = false;
		if (m_scrollBar != null)
		{
			m_scrollBar.value = 0f;
		}
		if (m_uiAnimation != null)
		{
			m_uiAnimation.Play(true);
		}
		SoundManager.SePlay("sys_window_open");
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
		if (component != null)
		{
			component.SendMessage("OnClick");
		}
	}
}
