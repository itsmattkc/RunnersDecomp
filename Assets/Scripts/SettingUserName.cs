using Message;
using Text;
using UnityEngine;

public class SettingUserName : SettingBase
{
	protected enum EventSignal
	{
		PLAY_START = 100
	}

	private readonly int MaxInputLength = 8;

	private TinyFsmBehavior m_fsm;

	private bool m_isEndConnect;

	private bool m_isEnd;

	private string m_anthorPath = string.Empty;

	private bool m_requestStart;

	private bool m_calcelButtonUseFlag = true;

	private bool m_isPlayStarted;

	private bool m_sendApolloFlag;

	private SendApollo m_sendApollo;

	public void SetCancelButtonUseFlag(bool useFlag)
	{
		m_calcelButtonUseFlag = useFlag;
	}

	private void Start()
	{
		SettingPartsUserName settingPartsUserName = base.gameObject.AddComponent<SettingPartsUserName>();
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateWaitStart);
			description.onFixedUpdate = true;
			m_fsm.SetUp(description);
			if (m_requestStart)
			{
				TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
				m_fsm.Dispatch(signal);
				m_requestStart = false;
			}
		}
		m_sendApolloFlag = FirstLaunchUserName.IsFirstLaunch;
	}

	private void OnDestroy()
	{
		SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		NGWordCheck.ResetData();
	}

	protected override void OnSetup(string anthorPath)
	{
		m_anthorPath = anthorPath;
		SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
		if (component != null)
		{
			component.SetCancelButtonUseFlag(m_calcelButtonUseFlag);
			component.Setup(m_anthorPath);
		}
	}

	protected override void OnPlayStart()
	{
		base.gameObject.SetActive(true);
		if (m_fsm != null)
		{
			if (m_isPlayStarted)
			{
				m_isEnd = false;
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			else
			{
				TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
				m_fsm.Dispatch(signal);
				m_isPlayStarted = true;
			}
		}
		else
		{
			m_isPlayStarted = true;
			m_requestStart = true;
		}
	}

	protected override bool OnIsEndPlay()
	{
		return m_isEnd;
	}

	protected override void OnUpdate()
	{
	}

	private TinyFsmState StateWaitStart(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_isEnd = false;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
			if (m_sendApolloFlag)
			{
				m_fsm.ChangeState(new TinyFsmState(StateSendApolloStart));
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSendApolloStart(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP2, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value);
			return TinyFsmState.End();
		}
		case -4:
			if (m_sendApollo != null)
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		case 0:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateUserNameInput(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SettingPartsUserName component2 = base.gameObject.GetComponent<SettingPartsUserName>();
			if (component2 != null)
			{
				component2.PlayStart();
				NGWordCheck.Load();
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
			if (component != null && component.IsEndPlay())
			{
				if (component.IsDecided)
				{
					m_fsm.ChangeState(new TinyFsmState(StateCheckError));
				}
				else if (component.IsCanceled)
				{
					m_fsm.ChangeState(new TinyFsmState(StateEnd));
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCheckError(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (NGWordCheck.IsLoaded())
			{
				SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
				string inputText = component.InputText;
				if (inputText.Length > MaxInputLength)
				{
					m_fsm.ChangeState(new TinyFsmState(StateInputErrorOverFlow));
				}
				else if (inputText.Length == 0)
				{
					m_fsm.ChangeState(new TinyFsmState(StateInputErrorNoInput));
				}
				else if (NGWordCheck.Check(inputText, component.TextLabel) != null)
				{
					m_fsm.ChangeState(new TinyFsmState(StateInputErrorNGWord));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateAskToDecideName));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInputErrorOverFlow(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("UserName", "input_error");
			info.message = TextUtility.GetCommonText("UserName", "input_error_info_1");
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
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInputErrorNoInput(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("UserName", "input_error");
			info.message = TextUtility.GetCommonText("UserName", "input_error_info_2");
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
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInputErrorNGWord(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("UserName", "input_error");
			info.message = TextUtility.GetCommonText("UserName", "input_error_info_ng_word");
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
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAskToDecideName(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
			string inputText = component.InputText;
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("UserName", "entry_user");
			string tag = "{NAME}";
			info.message = TextUtility.GetCommonText("UserName", "entry_user_info", tag, inputText);
			info.buttonType = GeneralWindow.ButtonType.YesNo;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateRegisterUser));
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRegisterUser(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_isEndConnect = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
				string inputText = component.InputText;
				if (ServerInterface.SettingState.m_userName != inputText)
				{
					loggedInServerInterface.RequestServerSetUserName(inputText, base.gameObject);
				}
				else
				{
					ServerSetUserName_Succeeded(null);
				}
			}
			else
			{
				ServerSetUserName_Succeeded(null);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isEndConnect)
			{
				if (m_sendApolloFlag)
				{
					m_fsm.ChangeState(new TinyFsmState(StateSendApolloEnd));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateFinishedRegisterUser));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSendApolloEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP2, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
			return TinyFsmState.End();
		}
		case -4:
			if (m_sendApollo != null)
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		case 0:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				m_fsm.ChangeState(new TinyFsmState(StateFinishedRegisterUser));
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFinishedRegisterUser(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			SettingPartsUserName component = base.gameObject.GetComponent<SettingPartsUserName>();
			string inputText = component.InputText;
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("UserName", "end_entry_user");
			string tag = "{NAME}";
			info.message = TextUtility.GetCommonText("UserName", "end_entry_user_info", tag, inputText);
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				ServerInterface.SettingState.m_userName = inputText;
			}
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateEnd));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_isEnd = true;
			base.gameObject.SetActive(false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerSetUserName_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		if (msg != null)
		{
			HudMenuUtility.SetUpdateRankingFlag();
		}
		m_isEndConnect = true;
	}
}
