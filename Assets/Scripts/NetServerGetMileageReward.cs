using LitJson;
using System.Collections.Generic;

public class NetServerGetMileageReward : NetBase
{
	private int m_episode;

	private int m_chapter;

	public List<ServerMileageReward> m_rewardList
	{
		get;
		private set;
	}

	public NetServerGetMileageReward(int episode, int chapter)
	{
		m_episode = episode;
		m_chapter = chapter;
		m_rewardList = new List<ServerMileageReward>();
	}

	protected override void DoRequest()
	{
		SetAction("Game/getMileageReward");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getMileageRewardString = instance.GetGetMileageRewardString(m_episode, m_chapter);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getMileageRewardString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_MileageReward(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_EpisodeChapter()
	{
		WriteActionParamValue("episode", m_episode);
		WriteActionParamValue("chapter", m_chapter);
	}

	private void GetResponse_MileageReward(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "mileageMapRewardList");
		if (jsonArray != null)
		{
			for (int i = 0; i < jsonArray.Count; i++)
			{
				ServerMileageReward serverMileageReward = AnalyzeMileageRewardJson(jsonArray[i]);
				serverMileageReward.m_startTime = ServerInterface.MileageMapState.m_chapterStartTime;
				m_rewardList.Add(serverMileageReward);
			}
		}
	}

	private ServerMileageReward AnalyzeMileageRewardJson(JsonData jdata)
	{
		if (jdata == null)
		{
			return null;
		}
		ServerMileageReward serverMileageReward = new ServerMileageReward();
		if (serverMileageReward != null)
		{
			serverMileageReward.m_episode = m_episode;
			serverMileageReward.m_chapter = m_chapter;
			serverMileageReward.m_type = NetUtil.GetJsonInt(jdata, "type");
			serverMileageReward.m_point = NetUtil.GetJsonInt(jdata, "point");
			serverMileageReward.m_itemId = NetUtil.GetJsonInt(jdata, "itemId");
			serverMileageReward.m_numItem = NetUtil.GetJsonInt(jdata, "numItem");
			serverMileageReward.m_limitTime = NetUtil.GetJsonInt(jdata, "limitTime");
		}
		return serverMileageReward;
	}
}
