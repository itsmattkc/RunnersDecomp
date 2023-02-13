using Message;
using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class SettingPartsSnsAdditional : MonoBehaviour
{
	public enum Mode
	{
		NONE,
		BACK_GROUND_LOAD,
		WAIT_TO_LOAD_END
	}

	private struct CallbackInfo
	{
		public string gameObjectName;

		public string functionName;
	}

	private bool m_isStart;

	private bool m_isEnd;

	private Mode m_mode;

	private List<CallbackInfo> m_callbackList = new List<CallbackInfo>();

	private bool m_isEndRequestMyProfile;

	private bool m_isEndRequestFriendProfile;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
	}

	public void PlayStart()
	{
		PlayStart(null, null, Mode.WAIT_TO_LOAD_END);
	}

	public void PlayStart(string gameObjectName, string functionName, Mode mode)
	{
		if (m_isEnd)
		{
			GameObject gameObject = GameObject.Find(gameObjectName);
			if (gameObject != null)
			{
				gameObject.SendMessage(functionName);
			}
			return;
		}
		if (m_isStart && !m_isEnd)
		{
			if (m_mode < mode)
			{
				m_mode = mode;
				if (m_mode == Mode.WAIT_TO_LOAD_END)
				{
					NetMonitor instance = NetMonitor.Instance;
					if (instance != null)
					{
						instance.StartMonitor(null);
					}
				}
			}
			CallbackInfo item = default(CallbackInfo);
			item.gameObjectName = gameObjectName;
			item.functionName = functionName;
			m_callbackList.Add(item);
			return;
		}
		m_isStart = true;
		m_mode = mode;
		if (m_mode == Mode.WAIT_TO_LOAD_END)
		{
			NetMonitor instance2 = NetMonitor.Instance;
			if (instance2 != null)
			{
				instance2.StartMonitor(null);
			}
		}
		CallbackInfo item2 = default(CallbackInfo);
		item2.gameObjectName = gameObjectName;
		item2.functionName = functionName;
		m_callbackList.Add(item2);
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			Debug.Log("SettingPartsSnsAdditional:PlayStart");
			socialInterface.RequestPermission(base.gameObject);
		}
		else
		{
			Debug.Log("SettingPartsSnsAdditional:NotLoggedIn");
			m_isEnd = false;
		}
	}

	private void RequestPermissionEndCallback(MsgSocialNormalResponse msg)
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			Debug.Log("SettingPartsSnsAdditional:PlayStart");
			socialInterface.RequestMyProfile(base.gameObject);
		}
	}

	private void RequestMyProfileEndCallback(MsgSocialMyProfileResponse msg)
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			socialInterface.RequestFriendList(base.gameObject);
		}
		Debug.Log("SettingPartsSnsAdditional:RequestMyProfileEndCallback");
	}

	private void RequestFriendListEndCallback(MsgSocialFriendListResponse msg)
	{
		Debug.Log("SettingPartsSnsAdditional:RequestFriendListEndCallback");
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
		if (!(socialInterface != null) || !(playerImageManager != null))
		{
			return;
		}
		List<string> list = new List<string>();
		List<SocialUserData> friendWithMeList = socialInterface.FriendWithMeList;
		if (friendWithMeList != null)
		{
			foreach (SocialUserData item in friendWithMeList)
			{
				if (item != null)
				{
					if (!item.IsSilhouette)
					{
						playerImageManager.GetPlayerImage(item.Id, item.Url, null);
						Debug.Log("sns picture add: " + item.Id + ", " + item.Url);
					}
					list.Add(item.Id);
				}
			}
			Debug.Log("SettingPartsSnsAdditional:GetPlayerImage");
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null && socialInterface != null)
		{
			loggedInServerInterface.RequestServerGetFriendUserIdList(list, base.gameObject);
		}
	}

	private void RequestGameDataEndCallback(MsgSocialCustomUserDataResponse msg)
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		SocialUserData myProfile = socialInterface.MyProfile;
		if (myProfile.Id == msg.m_userData.Id)
		{
			Debug.Log("SettingPartsSnsLogin:myProfile throwed");
			string text = string.Empty;
			if (SystemSaveManager.GetGameID() != "0")
			{
				text = SystemSaveManager.GetGameID();
			}
			bool flag = false;
			if (!msg.m_isCreated)
			{
				flag = true;
			}
			else if (text != myProfile.CustomData.GameId)
			{
				socialInterface.DeleteGameData(base.gameObject);
				flag = true;
			}
			if (flag)
			{
				Debug.Log("SettingPartsSnsLogin:Created Game Data");
				socialInterface.CreateMyGameData(text, base.gameObject);
			}
			else
			{
				CreateGameDataEndCallback(null);
			}
		}
	}

	private void CreateGameDataEndCallback(MsgSocialNormalResponse msg)
	{
		Debug.Log("SettingPartsSnsLogin:CreatedGameDataWasFinished");
		if (m_mode == Mode.WAIT_TO_LOAD_END)
		{
			NetMonitor instance = NetMonitor.Instance;
			if (instance != null)
			{
				instance.EndMonitorForward(null, null, null);
				instance.EndMonitorBackward();
			}
		}
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			socialInterface.IsEnableFriendInfo = true;
		}
		m_isEnd = true;
		foreach (CallbackInfo callback in m_callbackList)
		{
			string gameObjectName = callback.gameObjectName;
			if (string.IsNullOrEmpty(gameObjectName))
			{
				continue;
			}
			string functionName = callback.functionName;
			if (!string.IsNullOrEmpty(functionName))
			{
				GameObject gameObject = GameObject.Find(gameObjectName);
				if (gameObject != null)
				{
					gameObject.SendMessage(functionName);
				}
			}
		}
		m_callbackList.Clear();
		GameObject gameObject2 = GameObject.Find("ui_mm_ranking_page(Clone)");
		if (gameObject2 != null)
		{
			gameObject2.SendMessage("OnSettingPartsSnsAdditional");
		}
	}

	private void ServerGetFriendUserIdList_Succeeded(MsgGetFriendUserIdListSucceed msg)
	{
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (!(socialInterface != null))
		{
			return;
		}
		string id = socialInterface.MyProfile.Id;
		Debug.Log("mySnsUserId = " + id);
		bool flag = false;
		List<ServerUserTransformData> transformDataList = msg.m_transformDataList;
		if (transformDataList == null)
		{
			Debug.Log("ServerGetFriendUserIdList_Succeeded: DataList is null");
			ProcessEnd();
			return;
		}
		foreach (ServerUserTransformData item in transformDataList)
		{
			if (item == null || !(item.m_facebookId == id) || !(item.m_userId == SystemSaveManager.GetGameID()))
			{
				continue;
			}
			flag = true;
			break;
		}
		if (!flag)
		{
			Debug.Log("ServerGetFriendUserIdList_Succeeded: MyId is not Registered");
			ServerUserTransformData serverUserTransformData = new ServerUserTransformData();
			serverUserTransformData.m_facebookId = id;
			serverUserTransformData.m_userId = SystemSaveManager.GetGameID();
			transformDataList.Add(serverUserTransformData);
			id = socialInterface.MyProfile.Id;
		}
		foreach (ServerUserTransformData item2 in transformDataList)
		{
			if (item2 == null)
			{
				continue;
			}
			foreach (SocialUserData friendWithMe in socialInterface.FriendWithMeList)
			{
				if (friendWithMe != null && item2.m_facebookId == friendWithMe.Id && string.IsNullOrEmpty(friendWithMe.CustomData.GameId))
				{
					friendWithMe.CustomData.GameId = item2.m_userId;
				}
			}
		}
		if (!flag)
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null && socialInterface != null)
			{
				loggedInServerInterface.RequestServerSetFacebookScopedId(id, base.gameObject);
			}
		}
		else
		{
			Debug.Log("ServerGetFriendUserIdList_Succeeded: MyId is already Registered");
			ProcessEnd();
		}
	}

	private void ServerSetFacebookScopedId_Succeeded(MsgSetFacebookScopedIdSucceed msg)
	{
		ProcessEnd();
	}

	private void ProcessEnd()
	{
		Debug.Log("SettingPartsSnsLogin:CreatedGameDataWasFinished");
		if (m_mode == Mode.WAIT_TO_LOAD_END)
		{
			NetMonitor instance = NetMonitor.Instance;
			if (instance != null)
			{
				instance.EndMonitorForward(null, null, null);
				instance.EndMonitorBackward();
			}
		}
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			socialInterface.IsEnableFriendInfo = true;
		}
		m_isEnd = true;
		foreach (CallbackInfo callback in m_callbackList)
		{
			string gameObjectName = callback.gameObjectName;
			if (string.IsNullOrEmpty(gameObjectName))
			{
				continue;
			}
			string functionName = callback.functionName;
			if (!string.IsNullOrEmpty(functionName))
			{
				GameObject gameObject = GameObject.Find(gameObjectName);
				if (gameObject != null)
				{
					gameObject.SendMessage(functionName);
				}
			}
		}
		m_callbackList.Clear();
		GameObject gameObject2 = GameObject.Find("ui_mm_ranking_page(Clone)");
		if (gameObject2 != null)
		{
			gameObject2.SendMessage("OnSettingPartsSnsAdditional");
		}
	}
}
