using LitJson;
using System.Collections.Generic;

public class NetDebugUpdMileageData : NetBase
{
	public ServerMileageMapState mileageMapState
	{
		get;
		set;
	}

	public NetDebugUpdMileageData()
		: this(null)
	{
	}

	public NetDebugUpdMileageData(ServerMileageMapState mileageMapState)
	{
		this.mileageMapState = mileageMapState;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/updMileageData");
		SetParameter_MileageMapState();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_MileageMapState()
	{
		long num = NetUtil.GetCurrentUnixTime();
		int num2 = 0;
		int num3 = 0;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("episode", mileageMapState.m_episode);
		dictionary.Add("chapter", mileageMapState.m_chapter);
		dictionary.Add("point", mileageMapState.m_point);
		dictionary.Add("mapDistance", num2);
		dictionary.Add("numBossAttack", mileageMapState.m_numBossAttack);
		dictionary.Add("stageDistance", num3);
		dictionary.Add("chapterStartTime", num);
		dictionary.Add("stageTotalScore", mileageMapState.m_stageTotalScore);
		dictionary.Add("stageMaxScore", mileageMapState.m_stageMaxScore);
		WriteActionParamObject("mileageMapState", dictionary);
	}
}
