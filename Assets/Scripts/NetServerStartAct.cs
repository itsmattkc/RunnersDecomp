using LitJson;
using System;
using System.Collections.Generic;

public class NetServerStartAct : NetBase
{
	public List<ItemType> paramModifiersItem
	{
		get;
		set;
	}

	public List<BoostItemType> paramModifiersBoostItem
	{
		get;
		set;
	}

	public List<string> paramFriendIdList
	{
		get;
		set;
	}

	public bool paramTutorial
	{
		get;
		set;
	}

	public int? paramEventId
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public List<ServerDistanceFriendEntry> resultDistanceFriendEntry
	{
		get;
		private set;
	}

	public NetServerStartAct()
		: this(null, null, null, false, null)
	{
	}

	public NetServerStartAct(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, List<string> distanceFriendList, bool tutorial, int? eventId)
	{
		paramModifiersItem = new List<ItemType>();
		if (modifiersItem != null)
		{
			for (int i = 0; i < modifiersItem.Count; i++)
			{
				paramModifiersItem.Add(modifiersItem[i]);
			}
		}
		paramModifiersBoostItem = new List<BoostItemType>();
		if (modifiersBoostItem != null)
		{
			for (int j = 0; j < modifiersBoostItem.Count; j++)
			{
				paramModifiersBoostItem.Add(modifiersBoostItem[j]);
			}
		}
		paramFriendIdList = distanceFriendList;
		paramTutorial = tutorial;
		paramEventId = eventId;
	}

	protected override void DoRequest()
	{
		SetAction("Game/actStart");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < paramModifiersItem.Count; i++)
			{
				ServerItem.Id id = new ServerItem(paramModifiersItem[i]).id;
				list.Add((int)id);
			}
			for (int j = 0; j < paramModifiersBoostItem.Count; j++)
			{
				ServerItem.Id id2 = new ServerItem(paramModifiersBoostItem[j]).id;
				list.Add((int)id2);
			}
			int eventId = (!paramEventId.HasValue) ? (-1) : paramEventId.Value;
			string actStartString = instance.GetActStartString(list, paramFriendIdList, paramTutorial, eventId);
			Debug.Log("NetServerPostGameResults.json = " + actStartString);
			WriteJsonString(actStartString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_MileageBonus(jdata);
		NetUtil.GetResponse_CampaignList(jdata);
		GetResponse_DistanceFriendList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		if (resultPlayerState.m_numEnergy < 1)
		{
			base.resultStCd = ServerInterface.StatusCode.NotEnoughEnergy;
			return;
		}
		resultPlayerState.m_numEnergy--;
		resultPlayerState.m_numContinuesUsed = 0;
		DateTime now = DateTime.Now;
		if (resultPlayerState.m_energyRenewsAt <= now)
		{
			resultPlayerState.m_energyRenewsAt = now + new TimeSpan(0, 0, (int)ServerInterface.SettingState.m_energyRefreshTime);
		}
	}

	private void SetParameter_Modifiers()
	{
		List<object> list = new List<object>();
		for (int i = 0; i < paramModifiersItem.Count; i++)
		{
			ServerItem.Id id = new ServerItem(paramModifiersItem[i]).id;
			list.Add((int)id);
		}
		for (int j = 0; j < paramModifiersBoostItem.Count; j++)
		{
			ServerItem.Id id2 = new ServerItem(paramModifiersBoostItem[j]).id;
			list.Add((int)id2);
		}
		WriteActionParamArray("modifire", list);
		list.Clear();
		list = null;
	}

	private void SetParameter_FriendIdList()
	{
		List<object> list = new List<object>();
		for (int i = 0; i < paramFriendIdList.Count; i++)
		{
			list.Add(paramFriendIdList[i]);
		}
		WriteActionParamArray("distanceFriendList", list);
		list.Clear();
		list = null;
	}

	private void SetParameter_Tutorial()
	{
		WriteActionParamValue("tutorial", paramTutorial ? 1 : 0);
	}

	private void SetParameter_EventId()
	{
		if (paramEventId.HasValue)
		{
			WriteActionParamValue("eventId", paramEventId);
		}
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_MileageBonus(JsonData jdata)
	{
	}

	private void GetResponse_DistanceFriendList(JsonData jdata)
	{
		resultDistanceFriendEntry = new List<ServerDistanceFriendEntry>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "distanceFriendList");
		if (jsonArray != null)
		{
			int count = jsonArray.Count;
			for (int i = 0; i < count; i++)
			{
				ServerDistanceFriendEntry serverDistanceFriendEntry = new ServerDistanceFriendEntry();
				serverDistanceFriendEntry.m_friendId = NetUtil.GetJsonString(jsonArray[i], "friendId");
				serverDistanceFriendEntry.m_distance = NetUtil.GetJsonInt(jsonArray[i], "distance");
				resultDistanceFriendEntry.Add(serverDistanceFriendEntry);
			}
		}
	}
}
