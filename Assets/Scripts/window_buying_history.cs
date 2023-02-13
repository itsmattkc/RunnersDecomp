using Text;
using UnityEngine;

public class window_buying_history : WindowBase
{
	[SerializeField]
	private GameObject m_closeBtn;

	[SerializeField]
	private UILabel m_headerTextLabel;

	[SerializeField]
	private UILabel m_redRingText;

	[SerializeField]
	private UILabel m_redRingGetScoreText;

	[SerializeField]
	private UILabel m_redRingBuyScoreText;

	[SerializeField]
	private UILabel m_redRingGetText;

	[SerializeField]
	private UILabel m_redRingBuyText;

	[SerializeField]
	private UILabel m_ringText;

	[SerializeField]
	private UILabel m_ringGetScoreText;

	[SerializeField]
	private UILabel m_ringBuyScoreText;

	[SerializeField]
	private UILabel m_ringGetText;

	[SerializeField]
	private UILabel m_ringBuyText;

	[SerializeField]
	private UILabel m_energyText;

	[SerializeField]
	private UILabel m_energyGetScoreText;

	[SerializeField]
	private UILabel m_energyBuyScoreText;

	[SerializeField]
	private UILabel m_energyGetText;

	[SerializeField]
	private UILabel m_energyBuyText;

	private bool m_isEnd;

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
		OptionMenuUtility.TranObj(base.gameObject);
		base.enabled = false;
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
		m_uiAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		if (m_uiAnimation != null)
		{
			Animation component3 = base.gameObject.GetComponent<Animation>();
			m_uiAnimation.target = component3;
			m_uiAnimation.clipName = "ui_menu_option_window_Anim";
		}
		UpdateView();
		SoundManager.SePlay("sys_window_open");
	}

	private void OnDestroy()
	{
		Debug.LogWarning("window_buying_history::OnDestroy()");
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
	}

	private void OnFinishedAnimationCallback()
	{
		m_isEnd = true;
	}

	private void UpdateView()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int value = 0;
		int value2 = 0;
		int value3 = 0;
		if (ServerInterface.PlayerState != null)
		{
			num = ServerInterface.PlayerState.m_numBuyRedRings;
			num2 = ServerInterface.PlayerState.m_numBuyRings;
			num3 = ServerInterface.PlayerState.m_numBuyEnergy;
			value = ServerInterface.PlayerState.m_numRedRings - num;
			value2 = ServerInterface.PlayerState.m_numRings - num2;
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				value3 = (int)instance.PlayerData.ChallengeCount - num3;
			}
		}
		num = Mathf.Clamp(num, 0, 99999999);
		num2 = Mathf.Clamp(num2, 0, 99999999);
		num3 = Mathf.Clamp(num3, 0, 99999999);
		value = Mathf.Clamp(value, 0, 99999999);
		value2 = Mathf.Clamp(value2, 0, 99999999);
		value3 = Mathf.Clamp(value3, 0, 99999999);
		TextUtility.SetCommonText(m_headerTextLabel, "Option", "buying_info");
		TextUtility.SetCommonText(m_redRingText, "Item", "red_star_ring");
		TextUtility.SetCommonText(m_ringText, "Item", "ring");
		TextUtility.SetCommonText(m_energyText, "Item", "energy");
		TextUtility.SetCommonText(m_redRingGetText, "Option", "take");
		TextUtility.SetCommonText(m_ringGetText, "Option", "take");
		TextUtility.SetCommonText(m_energyGetText, "Option", "take");
		TextUtility.SetCommonText(m_redRingBuyText, "Option", "buy");
		TextUtility.SetCommonText(m_ringBuyText, "Option", "buy");
		TextUtility.SetCommonText(m_energyBuyText, "Option", "buy");
		TextUtility.SetCommonText(m_redRingGetScoreText, "Score", "number_of_pieces", "{NUM}", HudUtility.GetFormatNumString(value));
		TextUtility.SetCommonText(m_ringGetScoreText, "Score", "number_of_pieces", "{NUM}", HudUtility.GetFormatNumString(value2));
		TextUtility.SetCommonText(m_energyGetScoreText, "Score", "number_of_pieces", "{NUM}", HudUtility.GetFormatNumString(value3));
		TextUtility.SetCommonText(m_redRingBuyScoreText, "Score", "number_of_pieces", "{NUM}", HudUtility.GetFormatNumString(num));
		TextUtility.SetCommonText(m_ringBuyScoreText, "Score", "number_of_pieces", "{NUM}", HudUtility.GetFormatNumString(num2));
		TextUtility.SetCommonText(m_energyBuyScoreText, "Score", "number_of_pieces", "{NUM}", HudUtility.GetFormatNumString(num3));
	}

	public void PlayOpenWindow()
	{
		m_isEnd = false;
		if (m_uiAnimation != null)
		{
			UpdateView();
			m_uiAnimation.Play(true);
		}
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
