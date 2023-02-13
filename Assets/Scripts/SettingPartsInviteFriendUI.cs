using Message;
using UnityEngine;

public class SettingPartsInviteFriendUI : SettingBase
{
	private bool m_isEnd;

	protected override void OnSetup(string anthorPath)
	{
	}

	protected override void OnPlayStart()
	{
		GameObject gameObject = GameObject.Find("SocialInterface");
		if (!(gameObject == null))
		{
			SocialInterface component = gameObject.GetComponent<SocialInterface>();
			if (!(component == null))
			{
				component.InviteFriend(base.gameObject);
			}
		}
	}

	protected override bool OnIsEndPlay()
	{
		return m_isEnd;
	}

	protected override void OnUpdate()
	{
	}

	private void InviteFriendEndCallback(MsgSocialNormalResponse msg)
	{
		m_isEnd = true;
	}
}
