public class SettingPartsInviteFriend : SettingBase
{
	private enum State
	{
		STATE_IDLE,
		STATE_WAIT,
		STATE_LOGIN_SETUP,
		STATE_LOGIN,
		STATE_INVITE_FRIEND,
		STATE_END
	}

	private SettingPartsInviteFriendUI m_inviteFriend;

	private SettingPartsSnsLogin m_login;

	private string m_anchorPath;

	private readonly string ExcludePathName = "Camera/Anchor_5_MC";

	private State m_state;

	private void Start()
	{
	}

	protected override void OnSetup(string anthorPath)
	{
		m_anchorPath = ExcludePathName;
		if (m_inviteFriend == null)
		{
			m_inviteFriend = base.gameObject.AddComponent<SettingPartsInviteFriendUI>();
		}
		if (m_login == null)
		{
			m_login = base.gameObject.AddComponent<SettingPartsSnsLogin>();
		}
		if (m_login != null)
		{
			m_login.Setup(m_anchorPath);
		}
	}

	protected override void OnPlayStart()
	{
		m_state = State.STATE_WAIT;
	}

	protected override bool OnIsEndPlay()
	{
		if (m_inviteFriend != null && !m_inviteFriend.IsEndPlay())
		{
			return false;
		}
		return true;
	}

	protected override void OnUpdate()
	{
		switch (m_state)
		{
		case State.STATE_IDLE:
			break;
		case State.STATE_END:
			break;
		case State.STATE_WAIT:
			m_state = State.STATE_LOGIN_SETUP;
			break;
		case State.STATE_LOGIN_SETUP:
			if (m_login != null)
			{
				m_login.PlayStart();
			}
			m_state = State.STATE_LOGIN;
			break;
		case State.STATE_LOGIN:
			if (m_login != null && m_login.IsEnd)
			{
				m_inviteFriend.Setup(m_anchorPath);
				m_inviteFriend.PlayStart();
				m_state = State.STATE_INVITE_FRIEND;
			}
			break;
		case State.STATE_INVITE_FRIEND:
			if (m_inviteFriend.IsEndPlay())
			{
				m_state = State.STATE_END;
			}
			break;
		}
	}
}
