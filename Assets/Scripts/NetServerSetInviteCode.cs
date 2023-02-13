using LitJson;
using System.Collections.Generic;

public class NetServerSetInviteCode : NetBase
{
	public string friendId
	{
		get;
		set;
	}

	public ServerPlayerState playerState
	{
		get;
		set;
	}

	public List<ServerPresentState> incentive
	{
		get;
		set;
	}

	public NetServerSetInviteCode()
		: this(string.Empty)
	{
	}

	public NetServerSetInviteCode(string friendId)
	{
		this.friendId = friendId;
	}

	protected override void DoRequest()
	{
		SetAction("Friend/setInviteCode");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string setInviteCodeString = instance.GetSetInviteCodeString(friendId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(setInviteCodeString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_Incentive(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_FriendId()
	{
		WriteActionParamValue("friendId", friendId);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		playerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_Incentive(JsonData jdata)
	{
		incentive = new List<ServerPresentState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "incentive");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerPresentState serverPresentState = NetUtil.AnalyzePresentStateJson(jsonArray[i], string.Empty);
			if (serverPresentState != null)
			{
				incentive.Add(serverPresentState);
			}
		}
	}
}
