using LitJson;
using System.Collections.Generic;

public class NetServerGetMileageData : NetBase
{
	private string[] m_distanceFriendList;

	public ServerMileageMapState resultMileageMapState
	{
		get;
		private set;
	}

	public List<ServerMileageFriendEntry> m_resultMileageFriendList
	{
		get;
		private set;
	}

	public NetServerGetMileageData(string[] distanceFriendList)
	{
		m_distanceFriendList = distanceFriendList;
	}

	protected override void DoRequest()
	{
		SetAction("Game/getMileageData");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_MileageState(jdata);
		GetResponse_MileageFriendList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_DistanceFriendList()
	{
		if (m_distanceFriendList != null && m_distanceFriendList.Length != 0)
		{
			WriteActionParamArray("distanceFriendList", new List<object>(m_distanceFriendList));
		}
	}

	private void GetResponse_MileageState(JsonData jdata)
	{
		resultMileageMapState = NetUtil.AnalyzeMileageMapStateJson(jdata, "mileageMapState");
	}

	private void GetResponse_MileageFriendList(JsonData jdata)
	{
		m_resultMileageFriendList = NetUtil.AnalyzeMileageFriendListJson(jdata, "mileageFriendList");
	}
}
