using Message;
using SaveData;
using System;
using Text;
using UnityEngine;

public class FirstLaunchUserSetting : MonoBehaviour
{
	private enum EventSignal
	{
		PLAY_START = 100,
		SNS_LOGIN_END
	}

	private readonly string ANCHOR_PATH = "Camera/menu_Anim/MainMenuUI4/Anchor_5_MC";

	private TinyFsmBehavior m_fsm;

	private SettingPartsSnsLogin m_snsLogin;

	private SettingPartsAcceptInvite m_acceptInvite;

	private EasySnsFeed m_feed;

	private SendApollo m_sendApollo;

	private int m_specialEggNum = -1;

	private int m_chaoRouletteNum = -1;

	private bool m_isEndPlay;

	private float m_timer;

	private bool m_addSpecialEgg;

	private bool m_getSpecialEggNum;

	private bool m_getUserResult;

	public bool IsEndPlay
	{
		get
		{
			return m_isEndPlay;
		}
		private set
		{
		}
	}

	public void PlayStart()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
		m_isEndPlay = false;
	}

	private void Start()
	{
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.initState = new TinyFsmState(StateIdle);
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
		m_snsLogin = base.gameObject.AddComponent<SettingPartsSnsLogin>();
		m_snsLogin.Setup(ANCHOR_PATH);
		m_acceptInvite = base.gameObject.AddComponent<SettingPartsAcceptInvite>();
		m_acceptInvite.Setup(ANCHOR_PATH);
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
			return TinyFsmState.End();
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateHelp));
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
			if (m_snsLogin != null)
			{
				m_snsLogin.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_snsLogin.IsEnd)
			{
				if (m_snsLogin.IsCalceled)
				{
					m_fsm.ChangeState(new TinyFsmState(StateHelp));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateAskInputInviteCode));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAskInputInviteCode(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "StateAskInputInviteCode";
			info.buttonType = GeneralWindow.ButtonType.YesNo;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_ask_accept_invite_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_ask_accept_invite_text").text;
			info.anchor_path = ANCHOR_PATH;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("StateAskInputInviteCode"))
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateInputInviteCode));
				}
				else if (GeneralWindow.IsNoButtonPressed)
				{
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateHelp));
				}
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateHelp));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInputInviteCode(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_acceptInvite != null)
			{
				m_acceptInvite.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_acceptInvite != null && m_acceptInvite.IsEndPlay())
			{
				m_fsm.ChangeState(new TinyFsmState(StateHelp));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateHelp(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "StateHelp";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_about_help_menu_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_about_help_menu_text").text;
			info.anchor_path = ANCHOR_PATH;
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("StateHelp"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateOptionButton));
				}
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateOptionButton));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateOptionButton(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.OPTION);
			m_timer = 3f;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			m_timer -= Time.deltaTime;
			if (TutorialCursor.IsTouchScreen() || m_timer < 0f)
			{
				TutorialCursor.DestroyTutorialCursor();
				m_fsm.ChangeState(new TinyFsmState(StateGetUserResult));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateGetUserResult(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_getUserResult = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerOptionUserResult(base.gameObject);
			}
			else
			{
				m_getUserResult = true;
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_getUserResult)
			{
				if (m_chaoRouletteNum == 0)
				{
					m_fsm.ChangeState(new TinyFsmState(StateGetSpecialEggNum));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateEndProcessSpEgg));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerGetOptionUserResult_Succeeded(MsgGetOptionUserResultSucceed msg)
	{
		if (msg != null && msg.m_serverOptionUserResult != null)
		{
			m_chaoRouletteNum = msg.m_serverOptionUserResult.m_numChaoRoulette;
		}
		m_getUserResult = true;
	}

	private void ServerGetOptionUserResult_Failed(MsgServerConnctFailed msg)
	{
		m_getUserResult = true;
	}

	private TinyFsmState StateGetSpecialEggNum(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_getSpecialEggNum = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetChaoWheelOptions(base.gameObject);
			}
			else
			{
				m_getSpecialEggNum = true;
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_getSpecialEggNum)
			{
				m_fsm.ChangeState(new TinyFsmState(StateEndProcessSpEgg));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerGetChaoWheelOptions_Succeeded(MsgGetChaoWheelOptionsSucceed msg)
	{
		if (msg != null && msg.m_options != null)
		{
			m_specialEggNum = msg.m_options.NumSpecialEggs;
		}
		m_getSpecialEggNum = true;
	}

	private void ServerGetChaoWheelOptions_Failed(MsgServerConnctFailed msg)
	{
		m_getSpecialEggNum = true;
	}

	private bool IsCheater()
	{
		if (m_specialEggNum == 1 && m_chaoRouletteNum == 0)
		{
			return false;
		}
		return true;
	}

	private TinyFsmState StateEndProcessSpEgg(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (IsCheater())
			{
				m_addSpecialEgg = true;
			}
			else
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					loggedInServerInterface.RequestServerAddSpecialEgg(9, base.gameObject);
					m_addSpecialEgg = false;
				}
				else
				{
					m_addSpecialEgg = true;
				}
			}
			return TinyFsmState.End();
		case -4:
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					systemdata.SetFlagStatus(SystemData.FlagStatus.TUTORIAL_END, true);
					instance.SaveSystemData();
				}
			}
			Resources.UnloadUnusedAssets();
			GC.Collect();
			return TinyFsmState.End();
		}
		case 0:
			if (m_addSpecialEgg)
			{
				m_fsm.ChangeState(new TinyFsmState(StateEndProcessApollo));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ServerAddSpecialEgg_Succeeded(MsgAddSpecialEggSucceed msg)
	{
		m_addSpecialEgg = true;
	}

	private void ServerAddSpecialEgg_Failed(MsgServerConnctFailed msg)
	{
		m_addSpecialEgg = true;
	}

	private TinyFsmState StateEndProcessApollo(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			if (m_sendApollo != null)
			{
				UnityEngine.Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
			}
			return TinyFsmState.End();
		case 0:
			m_fsm.ChangeState(new TinyFsmState(StateTutorialEnd));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateTutorialEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "StateTutorialEnd";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "end_of_tutorial_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "end_of_tutorial_text").text;
			info.anchor_path = ANCHOR_PATH;
			info.buttonType = GeneralWindow.ButtonType.TweetCancel;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("StateTutorialEnd"))
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MileageMap", "feed_highscore_caption").text;
					string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "tutorial_end_feed_text").text;
					m_feed = new EasySnsFeed(base.gameObject, ANCHOR_PATH, text, text2);
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateWaitSnsFeedEnd));
				}
				else if (GeneralWindow.IsNoButtonPressed)
				{
					SystemSaveManager instance = SystemSaveManager.Instance;
					if (instance != null)
					{
						SystemData systemdata = instance.GetSystemdata();
						if (systemdata != null)
						{
							systemdata.SetFacebookWindow(false);
							instance.SaveSystemData();
						}
					}
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateSpEggHelp));
				}
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateSpEggHelp));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitSnsFeedEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_feed != null)
			{
				EasySnsFeed.Result result = m_feed.Update();
				if (result == EasySnsFeed.Result.COMPLETED || result == EasySnsFeed.Result.FAILED)
				{
					Debug.Log("FirstLaunchUserSetting.EasySnsFeed.Result=" + result);
					m_feed = null;
					m_fsm.ChangeState(new TinyFsmState(StateSpEggHelp));
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSpEggHelp(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			ItemGetWindow itemGetWindow2 = ItemGetWindowUtil.GetItemGetWindow();
			if (itemGetWindow2 != null)
			{
				itemGetWindow2.Create(new ItemGetWindow.CInfo
				{
					caption = TextUtility.GetCommonText("MainMenu", "tutorial_sp_egg1_caption"),
					serverItemId = 220000,
					imageCount = TextUtility.GetCommonText("MainMenu", "tutorial_sp_egg1_text", "{COUNT}", 9.ToString())
				});
				SoundManager.SePlay("sys_specialegg");
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
			if (itemGetWindow != null && itemGetWindow.IsEnd)
			{
				HudMenuUtility.SendMsgUpdateSaveDataDisplay();
				itemGetWindow.Reset();
				m_fsm.ChangeState(new TinyFsmState(StateSpEggHelp2));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSpEggHelp2(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "StateSpEggHelp2";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "tutorial_sp_egg2_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "tutorial_sp_egg2_text").text;
			info.anchor_path = ANCHOR_PATH;
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("StateSpEggHelp2"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					GeneralWindow.Close();
					m_fsm.ChangeState(new TinyFsmState(StateEnd));
				}
			}
			else
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
			m_isEndPlay = true;
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}
}
