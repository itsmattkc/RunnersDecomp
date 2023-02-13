using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class NetServerGetRentalState : NetBase
{
	public string[] friendIdList
	{
		get;
		set;
	}

	public int resultLastOffset
	{
		get;
		private set;
	}

	public int resultStates
	{
		get
		{
			return (resultChaoRentalStatesList != null) ? resultChaoRentalStatesList.Count : 0;
		}
	}

	protected List<ServerChaoRentalState> resultChaoRentalStatesList
	{
		get;
		set;
	}

	public NetServerGetRentalState()
		: this(null)
	{
	}

	public NetServerGetRentalState(string[] friendIdList)
	{
		this.friendIdList = friendIdList;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/getRentalState");
		SetParameter_FriendId();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_LastOffset(jdata);
		GetResponse_ChaoRentalStatesList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultChaoRentalStatesList = new List<ServerChaoRentalState>();
		int num = friendIdList.Length;
		for (int i = 0; i < num; i++)
		{
			ServerChaoRentalState serverChaoRentalState = new ServerChaoRentalState();
			serverChaoRentalState.FriendId = Random.Range(0f, 1E+11f).ToString();
			serverChaoRentalState.Name = "dummy_" + i;
			serverChaoRentalState.RentalState = Random.Range(0, 1);
			serverChaoRentalState.ChaoData = new ServerChaoData();
			serverChaoRentalState.ChaoData.Id = Random.Range(400000, 400011);
			serverChaoRentalState.ChaoData.Level = 1;
			serverChaoRentalState.ChaoData.Rarity = 0;
			resultChaoRentalStatesList.Add(serverChaoRentalState);
		}
	}

	private void SetParameter_FriendId()
	{
		List<object> list = new List<object>();
		string[] friendIdList = this.friendIdList;
		foreach (object item in friendIdList)
		{
			list.Add(item);
		}
		WriteActionParamArray("friendId", list);
	}

	public ServerChaoRentalState GetResultChaoRentalState(int index)
	{
		if (0 <= index && resultStates > index)
		{
			return resultChaoRentalStatesList[index];
		}
		return null;
	}

	private void GetResponse_LastOffset(JsonData jdata)
	{
		resultLastOffset = NetUtil.GetJsonInt(jdata, "lastOffset");
	}

	private void GetResponse_ChaoRentalStatesList(JsonData jdata)
	{
		resultChaoRentalStatesList = new List<ServerChaoRentalState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "chaoRentalState");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerChaoRentalState item = NetUtil.AnalyzeChaoRentalStateJson(jdata2, string.Empty);
			resultChaoRentalStatesList.Add(item);
		}
	}
}
