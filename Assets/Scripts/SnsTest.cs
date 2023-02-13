using Message;
using System.Collections.Generic;
using UnityEngine;

public class SnsTest : MonoBehaviour
{
	private SocialInterface m_socialInterface;

	private MsgSocialMyProfileResponse m_profileMsg;

	private MsgSocialFriendListResponse m_friendListMsg;

	private int m_score;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("SocialInterface");
		if (gameObject != null)
		{
			m_socialInterface = gameObject.GetComponent<SocialInterface>();
			if (m_socialInterface != null)
			{
				m_socialInterface.Initialize(base.gameObject);
			}
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(100f, 100f, 300f, 150f), "Login") && m_socialInterface != null)
		{
			m_socialInterface.Login(base.gameObject);
		}
		if (GUI.Button(new Rect(100f, 300f, 300f, 150f), "Logout") && m_socialInterface != null)
		{
			m_socialInterface.Logout();
		}
		if (GUI.Button(new Rect(450f, 100f, 300f, 150f), "Feed") && m_socialInterface != null)
		{
			m_socialInterface.Feed("マイレージ達成!", "マイレージマップ10を終えて、11に進んでます。", base.gameObject);
		}
		if (GUI.Button(new Rect(450f, 300f, 300f, 150f), "GetFriendList") && m_socialInterface != null)
		{
			m_socialInterface.RequestFriendList(base.gameObject);
		}
		if (m_profileMsg != null)
		{
			GUI.Label(new Rect(750f, 200f, 300f, 150f), m_profileMsg.m_profile.Name);
			GUI.Label(new Rect(750f, 300f, 300f, 150f), m_profileMsg.m_profile.Url);
		}
		if (m_friendListMsg != null)
		{
			List<SocialUserData> friends = m_friendListMsg.m_friends;
			int num = 0;
			foreach (SocialUserData item in friends)
			{
				if (item != null)
				{
					GUI.Label(new Rect(750f, 430 + 100 * num, 300f, 150f), item.Name);
					GUI.Label(new Rect(750f, 480 + 100 * num, 300f, 150f), item.Id);
					num++;
				}
			}
			if (!GUI.Button(new Rect(100f, 500f, 300f, 150f), "InviteFriend") || m_socialInterface != null)
			{
			}
			if (GUI.Button(new Rect(450f, 500f, 300f, 150f), "GetFriendScore") && m_socialInterface != null)
			{
				m_socialInterface.RequestGameData(friends[0].Id, base.gameObject);
			}
		}
		if (m_score != 0)
		{
			GUI.Label(new Rect(850f, 400f, 300f, 150f), m_score.ToString());
		}
	}

	private void RequestMyProfileEndCallback(MsgSocialMyProfileResponse msg)
	{
		Debug.Log("RequestMyProfileEndCallback");
		if (msg != null)
		{
			m_profileMsg = msg;
		}
	}

	private void RequestFriendListEndCallback(MsgSocialFriendListResponse msg)
	{
		Debug.Log("RequestFriendListEndCallback");
		if (msg != null)
		{
			m_friendListMsg = msg;
		}
	}

	private void RequestGameDataEndCallback(MsgSocialCustomUserDataResponse msg)
	{
	}
}
