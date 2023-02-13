using LitJson;
using System.Collections.Generic;

public class NetServerGetFacebookIncentive : NetBase
{
	public int incentiveType
	{
		get;
		set;
	}

	public int achievementCount
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

	public NetServerGetFacebookIncentive()
		: this(0, 0)
	{
	}

	public NetServerGetFacebookIncentive(int incentiveType, int achievementCount)
	{
		this.incentiveType = incentiveType;
		this.achievementCount = achievementCount;
	}

	protected override void DoRequest()
	{
		SetAction("Friend/getFacebookIncentive");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getFacebookIncentiveString = instance.GetGetFacebookIncentiveString(incentiveType, achievementCount);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getFacebookIncentiveString);
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

	private void SetParameter_Data()
	{
		WriteActionParamValue("type", incentiveType);
		WriteActionParamValue("achievementCount", achievementCount);
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
			ServerPresentState item = NetUtil.AnalyzePresentStateJson(jsonArray[i], string.Empty);
			incentive.Add(item);
		}
	}
}
