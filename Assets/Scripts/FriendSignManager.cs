using Message;
using System.Collections.Generic;
using UnityEngine;

public class FriendSignManager : MonoBehaviour
{
	public bool m_debugDraw;

	public bool m_debugFriend;

	private Texture2D m_debugTexture;

	private static int CREATE_MAX = 10;

	private List<FriendSignData> m_data = new List<FriendSignData>();

	private static FriendSignManager instance;

	public static FriendSignManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (Object.FindObjectOfType(typeof(FriendSignManager)) as FriendSignManager);
			}
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	public void SetupFriendSignManager()
	{
		DebugDraw("SetupFriendSignManager");
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
		List<ServerDistanceFriendEntry> distanceFriendEntry = ServerInterface.DistanceFriendEntry;
		if (!(socialInterface != null) || !(playerImageManager != null) || socialInterface.FriendList == null)
		{
			return;
		}
		int num = Random.Range(0, socialInterface.FriendList.Count);
		int num2 = num;
		for (int i = 0; i < socialInterface.FriendList.Count; i++)
		{
			if (num2 >= socialInterface.FriendList.Count)
			{
				num2 = 0;
			}
			Texture2D playerImage = playerImageManager.GetPlayerImage(socialInterface.FriendList[num2].Id);
			int distance = GetDistance(distanceFriendEntry, socialInterface.FriendList[num2].CustomData);
			if (distance > 0)
			{
				FriendSignData item = new FriendSignData(m_data.Count, distance, playerImage, false);
				m_data.Add(item);
				if (m_data.Count >= CREATE_MAX)
				{
					break;
				}
			}
			num2++;
		}
	}

	public List<FriendSignData> GetFriendSignDataList()
	{
		return m_data;
	}

	public void SetAppear(int index)
	{
		if ((uint)index < (uint)m_data.Count)
		{
			m_data[index].m_appear = true;
			DebugDraw("SetAppear " + GetDebugDataString(m_data[index]));
		}
	}

	private void DebugDraw(string msg)
	{
	}

	public static string GetDebugDataString(FriendSignData data)
	{
		if (data != null)
		{
			string text = (!(data.m_texture == null)) ? "ok" : "null";
			return "Friend(idx=" + data.m_index + ", dis=" + data.m_distance + ", tex=" + text + ", app=" + data.m_appear.ToString() + ")";
		}
		return "Friend(null)";
	}

	private int GetDistance(List<ServerDistanceFriendEntry> friendEntry, SocialUserCustomData customData)
	{
		if (friendEntry != null && customData != null)
		{
			foreach (ServerDistanceFriendEntry item in friendEntry)
			{
				if (item != null)
				{
					DebugDraw("fe.m_friendId=" + item.m_friendId + " customData.GameId=" + customData.GameId + " fe.m_distance=" + item.m_distance);
					if (item.m_friendId == customData.GameId)
					{
						return item.m_distance;
					}
				}
			}
		}
		return 0;
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
