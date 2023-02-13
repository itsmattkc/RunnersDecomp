using System.Collections.Generic;
using UnityEngine;

public class SocialInterface : MonoBehaviour
{
	public enum Permission
	{
		PUBLIC_PROFILE,
		USER_FRIENDS,
		NUM
	}

	private SocialPlatform m_platform;

	private bool m_isInitialized;

	private bool m_isLoggedIn;

	private SocialUserData m_myProfile = new SocialUserData();

	private bool m_isEnableFriendInfo;

	private List<SocialUserData> m_allFriendList = new List<SocialUserData>();

	private List<SocialUserData> m_friendList;

	private List<SocialUserData> m_notInstalledFriendList = new List<SocialUserData>();

	private List<SocialUserData> m_invitedFriendList;

	private bool[] m_isGrantedPermission = new bool[2];

	private static SocialInterface m_instance;

	public static SocialInterface Instance
	{
		get
		{
			return m_instance;
		}
	}

	public bool IsInitialized
	{
		get
		{
			return m_isInitialized;
		}
		set
		{
			m_isInitialized = value;
		}
	}

	public bool IsLoggedIn
	{
		get
		{
			return m_isLoggedIn;
		}
		set
		{
			m_isLoggedIn = value;
		}
	}

	public SocialUserData MyProfile
	{
		get
		{
			return m_myProfile;
		}
		set
		{
			m_myProfile = value;
		}
	}

	public bool IsEnableFriendInfo
	{
		get
		{
			return m_isEnableFriendInfo;
		}
		set
		{
			m_isEnableFriendInfo = value;
		}
	}

	public List<SocialUserData> FriendList
	{
		get
		{
			return m_friendList;
		}
		set
		{
			m_friendList = value;
		}
	}

	public List<SocialUserData> AllFriendList
	{
		get
		{
			return m_allFriendList;
		}
		set
		{
			m_allFriendList = value;
		}
	}

	public List<SocialUserData> NotInstalledFriendList
	{
		get
		{
			return m_notInstalledFriendList;
		}
		set
		{
			m_notInstalledFriendList = value;
		}
	}

	public List<SocialUserData> InvitedFriendList
	{
		get
		{
			return m_invitedFriendList;
		}
		set
		{
			m_invitedFriendList = value;
		}
	}

	public bool[] IsGrantedPermission
	{
		get
		{
			return m_isGrantedPermission;
		}
		set
		{
			m_isGrantedPermission = value;
		}
	}

	public List<SocialUserData> FriendWithMeList
	{
		get
		{
			List<SocialUserData> list = new List<SocialUserData>();
			if (m_friendList != null)
			{
				foreach (SocialUserData friend in m_friendList)
				{
					list.Add(friend);
				}
			}
			if (m_myProfile != null)
			{
				list.Add(m_myProfile);
			}
			return list;
		}
	}

	public static List<string> GetGameIdList(List<SocialUserData> socialUserDataList)
	{
		List<string> list = new List<string>();
		if (socialUserDataList == null)
		{
			return list;
		}
		foreach (SocialUserData socialUserData in socialUserDataList)
		{
			string gameId = socialUserData.CustomData.GameId;
			if (!string.IsNullOrEmpty(gameId))
			{
				list.Add(gameId);
			}
		}
		return list;
	}

	public static SocialUserData GetSocialUserDataFromGameId(List<SocialUserData> socialUserDataList, string gameId)
	{
		if (socialUserDataList != null)
		{
			foreach (SocialUserData socialUserData in socialUserDataList)
			{
				if (socialUserData.CustomData.GameId == gameId)
				{
					return socialUserData;
				}
			}
		}
		return null;
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			//m_platform = base.gameObject.AddComponent<SocialPlatformFacebook>();
			Object.DontDestroyOnLoad(base.gameObject);
			m_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
	}

	public void Initialize(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.Initialize(callbackObject);
		}
	}

	public void Login(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.Login(callbackObject);
		}
	}

	public void Logout()
	{
		if (!(m_platform == null))
		{
			m_platform.Logout();
		}
	}

	public void RequestMyProfile(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.RequestMyProfile(callbackObject);
		}
	}

	public void RequestFriendList(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.RequestFriendList(callbackObject);
		}
	}

	public void SetScore(SocialDefine.ScoreType type, int score, GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.SetScore(type, score, callbackObject);
		}
	}

	public void CreateMyGameData(string gameId, GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.CreateMyGameData(gameId, callbackObject);
		}
	}

	public void RequestGameData(string userId, GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.RequestGameData(userId, callbackObject);
		}
	}

	public void DeleteGameData(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.DeleteGameData(callbackObject);
		}
	}

	public void InviteFriend(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.InviteFriend(callbackObject);
		}
	}

	public void SendEnergy(SocialUserData userData, GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.SendEnergy(userData, callbackObject);
		}
	}

	public void Feed(string feedCaption, string feedText, GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.Feed(feedCaption, feedText, callbackObject);
		}
	}

	public void RequestInvitedFriend(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.RequestInvitedFriend(callbackObject);
		}
	}

	public void RequestPermission(GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.RequestPermission(callbackObject);
		}
	}

	public void AddPermission(List<Permission> permissions, GameObject callbackObject)
	{
		if (!(m_platform == null))
		{
			m_platform.AddPermission(permissions, callbackObject);
		}
	}

	public void RequestFriendRankingInfoSet(string gameObjectName, string functionName, SettingPartsSnsAdditional.Mode mode)
	{
		Debug.Log("RequestFriendRankingInfoSet");
		SettingPartsSnsAdditional settingPartsSnsAdditional = base.gameObject.GetComponent<SettingPartsSnsAdditional>();
		if (settingPartsSnsAdditional == null)
		{
			settingPartsSnsAdditional = base.gameObject.AddComponent<SettingPartsSnsAdditional>();
		}
		settingPartsSnsAdditional.PlayStart(gameObjectName, functionName, mode);
	}
}
