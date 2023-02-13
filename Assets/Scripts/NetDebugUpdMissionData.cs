using LitJson;
using System.Collections.Generic;

public class NetDebugUpdMissionData : NetBase
{
	private bool[] mParamMissionComplete;

	public int paramMissionSet
	{
		get;
		set;
	}

	public bool[] paramMissionComplete
	{
		get
		{
			return mParamMissionComplete;
		}
		set
		{
			mParamMissionComplete = (value.Clone() as bool[]);
		}
	}

	public NetDebugUpdMissionData()
		: this(0, default(bool), default(bool), default(bool))
	{
	}

	public NetDebugUpdMissionData(int missionSet, params bool[] missionComplete)
	{
		paramMissionSet = missionSet;
		paramMissionComplete = missionComplete;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/updMissionData");
		SetParameter_Mission();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Mission()
	{
		if (paramMissionComplete != null)
		{
			WriteActionParamValue("missionSet", paramMissionSet);
			List<object> list = new List<object>();
			for (int i = 0; i < paramMissionComplete.Length; i++)
			{
				list.Add(paramMissionComplete[i] ? 1 : 0);
			}
			WriteActionParamArray("missionsComplete", list);
			list.Clear();
			list = null;
		}
	}
}
