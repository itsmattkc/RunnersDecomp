using UnityEngine;

public class OptionFriendInvitation : MonoBehaviour
{
	private const string ATTACH_ANTHOR_NAME = "Camera/menu_Anim/OptionUI/Anchor_5_MC";

	private SettingPartsInviteFriend m_inviteFriend;

	private EasySnsFeed m_easySnsFeed;

	private bool m_loginFlag;

	private ui_option_scroll m_ui_option_scroll;

	public void Setup(ui_option_scroll scroll)
	{
		if (scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		m_loginFlag = false;
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			m_loginFlag = socialInterface.IsLoggedIn;
		}
		if (m_loginFlag)
		{
			PlayInvite();
		}
		else
		{
			m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/Anchor_5_MC");
		}
		base.enabled = true;
	}

	private void SetInvite()
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			m_loginFlag = socialInterface.IsLoggedIn;
		}
		if (!m_loginFlag)
		{
			return;
		}
		if (m_inviteFriend != null)
		{
			m_inviteFriend.PlayStart();
			return;
		}
		m_inviteFriend = base.gameObject.AddComponent<SettingPartsInviteFriend>();
		if (m_inviteFriend != null)
		{
			m_inviteFriend.Setup("Camera/menu_Anim/OptionUI/Anchor_5_MC");
			m_inviteFriend.PlayStart();
		}
	}

	private void PlayInvite()
	{
		if (m_inviteFriend != null)
		{
			m_inviteFriend.PlayStart();
			return;
		}
		m_inviteFriend = base.gameObject.AddComponent<SettingPartsInviteFriend>();
		if (m_inviteFriend != null)
		{
			m_inviteFriend.Setup("Camera/menu_Anim/OptionUI/Anchor_5_MC");
			m_inviteFriend.PlayStart();
		}
	}

	public void Update()
	{
		if (m_loginFlag)
		{
			if (m_inviteFriend != null && m_inviteFriend.IsEndPlay())
			{
				if (m_ui_option_scroll != null)
				{
					m_ui_option_scroll.OnEndChildPage();
				}
				base.enabled = false;
			}
		}
		else
		{
			if (m_easySnsFeed == null)
			{
				return;
			}
			switch (m_easySnsFeed.Update())
			{
			case EasySnsFeed.Result.COMPLETED:
				SetInvite();
				m_easySnsFeed = null;
				if (!m_loginFlag)
				{
					if (m_ui_option_scroll != null)
					{
						m_ui_option_scroll.OnEndChildPage();
					}
					base.enabled = false;
				}
				break;
			case EasySnsFeed.Result.FAILED:
				m_easySnsFeed = null;
				if (m_ui_option_scroll != null)
				{
					m_ui_option_scroll.OnEndChildPage();
				}
				base.enabled = false;
				break;
			}
		}
	}
}
