using Message;
using SaveData;
using System.Text.RegularExpressions;
using Text;
using UnityEngine;

public class SettingTakeoverPassword : SettingBase
{
	protected enum EventSignal
	{
		PLAY_START = 100
	}

	private readonly int MaxInputLength = 10;

	private readonly int MinInputLength = 6;

	private TinyFsmBehavior m_fsm;

	private bool m_isEndConnect;

	private bool m_isEnd;

	private bool m_isCancelEnd;

	private string m_anthorPath = string.Empty;

	private bool m_requestStart;

	private bool m_calcelButtonUseFlag = true;

	private bool m_isPlayStarted;

	public bool isCancel
	{
		get
		{
			return m_isCancelEnd;
		}
	}

	public void SetCancelButtonUseFlag(bool useFlag)
	{
		m_calcelButtonUseFlag = useFlag;
	}

	private void Start()
	{
		SettingPartsTakeoverPassword settingPartsTakeoverPassword = base.gameObject.AddComponent<SettingPartsTakeoverPassword>();
		settingPartsTakeoverPassword.SetCancelButtonUseFlag(m_calcelButtonUseFlag);
		settingPartsTakeoverPassword.Setup(m_anthorPath);
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
	}

	private void OnDestroy()
	{
		SettingPartsTakeoverPassword component = base.gameObject.GetComponent<SettingPartsTakeoverPassword>();
		if (component != null)
		{
			component.SetOkButtonEnabled(true);
			Object.Destroy(component);
		}
	}

	protected override void OnSetup(string anthorPath)
	{
		m_anthorPath = anthorPath;
	}

	protected override void OnPlayStart()
	{
		if (m_fsm != null)
		{
			if (m_isPlayStarted)
			{
				m_isEnd = false;
				m_isCancelEnd = false;
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
			m_isCancelEnd = false;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateUserNameInput));
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
			SettingPartsTakeoverPassword component2 = base.gameObject.GetComponent<SettingPartsTakeoverPassword>();
			if (component2 != null)
			{
				component2.PlayStart();
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			SettingPartsTakeoverPassword component = base.gameObject.GetComponent<SettingPartsTakeoverPassword>();
			if (component != null)
			{
				if (component.IsEndPlay())
				{
					if (component.IsDecided)
					{
						m_fsm.ChangeState(new TinyFsmState(StateCheckError));
					}
					else if (component.IsCanceled)
					{
						m_isCancelEnd = true;
						m_fsm.ChangeState(new TinyFsmState(StateEnd));
					}
				}
				else
				{
					bool okButtonEnabled = CheckPassword(component.InputText);
					component.SetOkButtonEnabled(okButtonEnabled);
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
		{
			SettingPartsTakeoverPassword component = base.gameObject.GetComponent<SettingPartsTakeoverPassword>();
			string inputText = component.InputText;
			if (inputText.Length > MaxInputLength)
			{
				m_fsm.ChangeState(new TinyFsmState(StateInputErrorOverFlow));
			}
			else if (inputText.Length == 0)
			{
				m_fsm.ChangeState(new TinyFsmState(StateInputErrorNoInput));
			}
			else if (inputText.Length < MinInputLength)
			{
				m_fsm.ChangeState(new TinyFsmState(StateInputErrorOverFlow));
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateRegisterPassword));
			}
			return TinyFsmState.End();
		}
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
			info.caption = TextUtility.GetCommonText("Option", "take_over_password_input_error");
			info.message = TextUtility.GetCommonText("Option", "take_over_password_input_error_info");
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
			info.caption = TextUtility.GetCommonText("Option", "take_over_password_input_error");
			info.message = TextUtility.GetCommonText("Option", "take_over_password_input_error_info");
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

	private TinyFsmState StateRegisterPassword(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_isEndConnect = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				SettingPartsTakeoverPassword component = base.gameObject.GetComponent<SettingPartsTakeoverPassword>();
				string inputText = component.InputText;
				string userPassword = NetUtil.CalcMD5String(inputText);
				loggedInServerInterface.RequestServerGetMigrationPassword(userPassword, base.gameObject);
				SystemSaveManager.SetTakeoverPassword(inputText);
			}
			else
			{
				ServerGetMigrationPassword_Succeeded(null);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isEndConnect)
			{
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
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private bool CheckPassword(string password)
	{
		bool flag = true;
		if (password.Length > MaxInputLength)
		{
			flag = false;
		}
		else if (password.Length == 0)
		{
			flag = false;
		}
		else if (password.Length < MinInputLength)
		{
			flag = false;
		}
		else
		{
			flag = false;
			string text = "[a-zA-Z0-9]";
			for (int i = 1; i < password.Length; i++)
			{
				text += "[a-zA-Z0-9]";
			}
			if (Regex.IsMatch(password, text))
			{
				flag = true;
			}
		}
		return flag;
	}

	private void ServerGetMigrationPassword_Succeeded(MsgGetMigrationPasswordSucceed msg)
	{
		m_isEndConnect = true;
		if (msg != null)
		{
			SystemSaveManager.SetTakeoverID(msg.m_migrationPassword);
			SystemSaveManager.Instance.SaveSystemData();
		}
	}
}
