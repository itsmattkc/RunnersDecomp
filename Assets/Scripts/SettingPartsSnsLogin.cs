using Message;
using SaveData;
using Text;
using UnityEngine;

public class SettingPartsSnsLogin : MonoBehaviour
{
	private enum EventSignal
	{
		PLAY_START = 100,
		SNS_LOGIN_END,
		SNS_LOGIN_FAILED,
		INCENTIVE_CONNECT_END
	}

	private TinyFsmBehavior m_fsm;

	private string m_anchorPath;

	private bool m_isEndPlay;

	private bool m_isCalceled;

	private IncentiveWindowQueue m_windowQueue;

	private SettingPartsSnsAdditional m_snsAdditional;

	private bool m_cancelWindowUseFlag = true;

	private bool m_isPlayStart;

	public bool IsEnd
	{
		get
		{
			return m_isEndPlay;
		}
		private set
		{
		}
	}

	public bool IsCalceled
	{
		get
		{
			return m_isCalceled;
		}
	}

	public void Setup(string anchorPath)
	{
		m_anchorPath = anchorPath;
	}

	public void PlayStart()
	{
		m_isEndPlay = false;
		m_isCalceled = false;
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.SetActiveEffect(HudMenuUtility.EffectPriority.UniqueWindow, false);
		}
		m_isPlayStart = true;
		HudMenuUtility.SetConnectAlertSimpleUI(true);
	}

	public bool IsEnableCreateCustomData()
	{
		string gameID = SystemSaveManager.GetGameID();
		if (gameID != null && gameID != "0")
		{
			return true;
		}
		return false;
	}

	public void SetCancelWindowUseFlag(bool flag)
	{
		m_cancelWindowUseFlag = flag;
	}

	private void Start()
	{
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.initState = new TinyFsmState(StateIdle);
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
	}

	private void Update()
	{
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isPlayStart)
			{
				SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
				if (socialInterface != null)
				{
					if (socialInterface.IsLoggedIn)
					{
						Debug.Log("SettingPartsSnsLoging: Logging in");
						m_fsm.ChangeState(new TinyFsmState(StateEnd));
					}
					else
					{
						Debug.Log("SettingPartsSnsLoging: Not Logging in");
						m_fsm.ChangeState(new TinyFsmState(StateAskSnsLogin));
					}
				}
				return TinyFsmState.End();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAskSnsLogin(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			Debug.Log("SettingPartsSnsLoging:StateAskSnsLogin");
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_facebook_login").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_facebook_login_info").text;
			info.anchor_path = m_anchorPath;
			info.buttonType = GeneralWindow.ButtonType.YesNo;
			info.name = "FacebookLogin";
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsYesButtonPressed)
			{
				Debug.Log("SettingPartsSnsLoging:AskSnsLogin.YesButton");
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateSnsLogin));
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				Debug.Log("SettingPartsSnsLoging:AskSnsLogin.NoButton");
				GeneralWindow.Close();
				if (m_cancelWindowUseFlag)
				{
					m_fsm.ChangeState(new TinyFsmState(StateSnsLoginCanceled));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateEnd));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSnsLoginCanceled(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			Debug.Log("SettingPartsSnsLoging:StateSnsLoginCanceled");
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_facebook_login_method").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_facebook_login_method_info").text;
			info.anchor_path = m_anchorPath;
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				m_isCalceled = true;
				m_fsm.ChangeState(new TinyFsmState(StateEnd));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSnsLogin(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			Debug.Log("SettingPartsSnsLoging:StateSnsLogin");
			SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface != null)
			{
				socialInterface.Login(base.gameObject);
			}
			NetMonitor instance2 = NetMonitor.Instance;
			if (instance2 != null)
			{
				instance2.StartMonitor(null);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 101:
		{
			NetMonitor instance3 = NetMonitor.Instance;
			if (instance3 != null)
			{
				instance3.EndMonitorForward(null, null, null);
				instance3.EndMonitorBackward();
			}
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_windowQueue = base.gameObject.AddComponent<IncentiveWindowQueue>();
				loggedInServerInterface.RequestServerGetFacebookIncentive(0, 0, base.gameObject);
				m_fsm.ChangeState(new TinyFsmState(StateIncentiveConnectWait));
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateGetSNSInformations));
			}
			return TinyFsmState.End();
		}
		case 102:
		{
			NetMonitor instance = NetMonitor.Instance;
			if (instance != null)
			{
				instance.EndMonitorForward(null, null, null);
				instance.EndMonitorBackward();
			}
			m_fsm.ChangeState(new TinyFsmState(StateSnsLoginFailed));
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIncentiveConnectWait(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Debug.Log("SettingPartsSnsLoging:StateIncentiveConnectWait");
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 103:
			Debug.Log("SettingPartsSnsLoging:StateIncentiveConnectWait INCENTIVE_CONNECT_END");
			m_fsm.ChangeState(new TinyFsmState(StateSetupIncentiveQueue));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSetupIncentiveQueue(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Debug.Log("SettingPartsSnsLoging:StateSetupIncentiveQueue");
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if ((bool)m_windowQueue && m_windowQueue.SetUpped)
			{
				m_fsm.ChangeState(new TinyFsmState(StateIncentiveDisplaying));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIncentiveDisplaying(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			Debug.Log("SettingPartsSnsLoging:StateIncentiveDisplaying");
			if (m_windowQueue != null)
			{
				Debug.Log("SettingPartsSnsLoging:StateIncentiveDisplaying  m_windowQueue.PlayStart()");
				m_windowQueue.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			Debug.Log("SettingPartsSnsLoging:StateIncentiveDisplaying  UPDATE(1)");
			if (m_windowQueue != null)
			{
				Debug.Log("SettingPartsSnsLoging:StateIncentiveDisplaying  UPDATE(2)");
				if (m_windowQueue.IsEmpty())
				{
					m_fsm.ChangeState(new TinyFsmState(StateGetSNSInformations));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSnsLoginFailed(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			Debug.Log("SettingPartsSnsLoging:StateGetSNSInformations");
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_network_error").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_network_error_info").text;
			info.anchor_path = m_anchorPath;
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateAskSnsLogin));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetSNSInformations(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			Debug.Log("SettingPartsSnsLoging:StateGetSNSInformations");
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			SocialInterface socialInterface2 = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface2 != null && socialInterface2.IsLoggedIn)
			{
				socialInterface2.RequestFriendRankingInfoSet(null, null, SettingPartsSnsAdditional.Mode.BACK_GROUND_LOAD);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			if (socialInterface != null && socialInterface.IsLoggedIn)
			{
				if (socialInterface.IsEnableFriendInfo)
				{
					m_fsm.ChangeState(new TinyFsmState(StateEnd));
				}
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateEnd));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			UIEffectManager instance = UIEffectManager.Instance;
			if (instance != null)
			{
				instance.SetActiveEffect(HudMenuUtility.EffectPriority.UniqueWindow, true);
			}
			Debug.Log("SettingPartsSnsLoging:StateEnd");
			m_isPlayStart = false;
			m_isEndPlay = true;
			HudMenuUtility.SetConnectAlertSimpleUI(false);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateIdle));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerGetFacebookIncentive_Succeeded(MsgGetNormalIncentiveSucceed msg)
	{
		Debug.Log("SettingPartsSnsLoging:ServerGetFacebookIncentive_Succeeded ");
		foreach (ServerPresentState item in msg.m_incentive)
		{
			Debug.Log("SettingPartsSnsLoging:ServerGetFacebookIncentive_Succeeded m_incentive");
			IncentiveWindow window = new IncentiveWindow(item.m_itemId, item.m_numItem, m_anchorPath);
			m_windowQueue.AddWindow(window);
		}
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(103);
		if ((bool)m_fsm)
		{
			Debug.Log("SettingPartsSnsLoging:ServerGetFacebookIncentive_Succeeded INCENTIVE_CONNECT_END");
			m_fsm.Dispatch(signal);
		}
	}

	private void LoginEndCallback(MsgSocialNormalResponse msg)
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (!(socialInterface == null))
		{
			TinyFsmEvent signal;
			if (socialInterface.IsLoggedIn)
			{
				HudMenuUtility.SetUpdateRankingFlag();
				signal = TinyFsmEvent.CreateUserEvent(101);
			}
			else
			{
				signal = TinyFsmEvent.CreateUserEvent(102);
			}
			if (m_fsm != null)
			{
				m_fsm.Dispatch(signal);
			}
		}
	}
}
