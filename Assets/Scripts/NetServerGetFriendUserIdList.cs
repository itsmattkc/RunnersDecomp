using LitJson;
using System.Collections.Generic;

public class NetServerGetFriendUserIdList : NetBase
{
	public List<ServerUserTransformData> resultTransformDataList;

	public List<string> paramFriendFBIdList
	{
		get;
		set;
	}

	public NetServerGetFriendUserIdList()
		: this(null)
	{
	}

	public NetServerGetFriendUserIdList(List<string> friendFBIdList)
	{
		paramFriendFBIdList = friendFBIdList;
	}

	protected override void DoRequest()
	{
		SetAction("Friend/getFriendUserIdList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getFacebookFriendUserIdList = instance.GetGetFacebookFriendUserIdList(paramFriendFBIdList);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getFacebookFriendUserIdList);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_TransformDataList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_FriendIdList()
	{
		List<object> list = new List<object>();
		foreach (string paramFriendFBId in paramFriendFBIdList)
		{
			if (!string.IsNullOrEmpty(paramFriendFBId))
			{
				list.Add(paramFriendFBId);
			}
		}
		WriteActionParamArray("facebookIdList", list);
	}

	private void GetResponse_TransformDataList(JsonData jdata)
	{
		resultTransformDataList = NetUtil.AnalyzeUserTransformData(jdata);
	}
}
