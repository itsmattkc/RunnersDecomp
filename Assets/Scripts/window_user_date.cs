using Message;
using Text;
using UnityEngine;

public class window_user_date : WindowBase
{
	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private UIScrollBar m_scrollBar;

	[SerializeField]
	private UIRectItemStorage m_itemStorage;

	[SerializeField]
	private UILabel m_headerTextLabel;

	private ServerOptionUserResult m_serverOptionUserResult = new ServerOptionUserResult();

	private UIPlayAnimation m_uiAnimation;

	private bool m_isEnd;

	private bool m_init;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public ServerOptionUserResult OptionUserResult
	{
		get
		{
			return m_serverOptionUserResult;
		}
	}

	private void Start()
	{
		OptionMenuUtility.TranObj(base.gameObject);
		if (m_closeBtn != null)
		{
			UIButtonMessage component = m_closeBtn.GetComponent<UIButtonMessage>();
			if (component == null)
			{
				m_closeBtn.AddComponent<UIButtonMessage>();
				component = m_closeBtn.GetComponent<UIButtonMessage>();
			}
			if (component != null)
			{
				component.enabled = true;
				component.trigger = UIButtonMessage.Trigger.OnClick;
				component.target = base.gameObject;
				component.functionName = "OnClickCloseButton";
			}
			UIPlayAnimation component2 = m_closeBtn.GetComponent<UIPlayAnimation>();
			if (component2 != null)
			{
				EventDelegate.Add(component2.onFinished, OnFinishedAnimationCallback, false);
			}
		}
		if (m_scrollBar != null)
		{
			m_scrollBar.value = 0f;
		}
		if (m_headerTextLabel != null)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Option", "users_results");
			if (text != null)
			{
				m_headerTextLabel.text = text.text;
			}
		}
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component3 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component3;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerOptionUserResult(base.gameObject);
		}
		SoundManager.SePlay("sys_window_open");
	}

	private void Update()
	{
		if (!m_init)
		{
			base.enabled = false;
			m_init = true;
			SetItemStorage();
		}
	}

	private void SetItemStorage()
	{
		if (m_itemStorage != null)
		{
			m_itemStorage.maxItemCount = 14;
			m_itemStorage.maxRows = 14;
			m_itemStorage.Restart();
			UpdateViewItemStorage();
		}
	}

	private void UpdateViewItemStorage()
	{
		if (!(m_itemStorage != null))
		{
			return;
		}
		ui_option_window_user_date_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_option_window_user_date_scroll>(true);
		int num = componentsInChildren.Length;
		ui_option_window_user_date_scroll.ResultType resultType = ui_option_window_user_date_scroll.ResultType.HIGHT_SCORE;
		for (int i = 0; i < num; i++)
		{
			if (i < 14)
			{
				resultType = (ui_option_window_user_date_scroll.ResultType)i;
				componentsInChildren[i].UpdateView(resultType, m_serverOptionUserResult);
			}
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

	private void ServerGetOptionUserResult_Succeeded(MsgGetOptionUserResultSucceed msg)
	{
		if (msg != null && msg.m_serverOptionUserResult != null)
		{
			msg.m_serverOptionUserResult.CopyTo(m_serverOptionUserResult);
		}
		UpdateViewItemStorage();
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

	private void ServerGetOptionUserResult_Failed()
	{
	}

	public void PlayOpenWindow()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerOptionUserResult(base.gameObject);
		}
		m_isEnd = false;
		if (m_uiAnimation != null)
		{
			m_uiAnimation.Play(true);
		}
	}
}
