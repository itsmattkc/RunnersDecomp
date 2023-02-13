using SaveData;
using Text;
using UnityEngine;

public class FirstLaunchInviteFriend : MonoBehaviour
{
	private enum EventSignal
	{
		PLAY_START = 100
	}

	private TinyFsmBehavior m_fsm;

	private float m_timer;

	private bool m_isEndPlay;

	private string m_anchorPath;

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

	public void Setup(string anchorPath)
	{
		m_anchorPath = anchorPath;
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
			m_fsm.ChangeState(new TinyFsmState(StateInviteFriendWindow));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInviteFriendWindow(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextUtility.GetCommonText("FaceBook", "ui_Lbl_facebook_invite_friend_caption");
			int num = 5;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				ServerSettingState settingState = ServerInterface.SettingState;
				if (settingState != null)
				{
					num = settingState.m_invitBaseIncentive.m_num;
				}
			}
			string text = info.message = TextUtility.GetCommonText("FaceBook", "ui_Lbl_facebook_invite_friend_text", "{RED_STAR_RING_NUM}", num.ToString());
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
				m_fsm.ChangeState(new TinyFsmState(StateDisplayTutorialCursor));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateDisplayTutorialCursor(TinyFsmEvent e)
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
			HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.INVITE_FRIEND_SEQUENCE_END);
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
