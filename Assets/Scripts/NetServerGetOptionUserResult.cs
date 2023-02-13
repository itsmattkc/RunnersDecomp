using LitJson;

public class NetServerGetOptionUserResult : NetBase
{
	private ServerOptionUserResult m_userResult = new ServerOptionUserResult();

	public ServerOptionUserResult UserResult
	{
		get
		{
			return m_userResult;
		}
	}

	protected override void DoRequest()
	{
		SetAction("Option/userResult");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_OptionUserResult(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_OptionUserResult(JsonData jdata)
	{
		JsonData jsonObject = NetUtil.GetJsonObject(jdata, "optionUserResult");
		if (jsonObject != null)
		{
			m_userResult.m_totalSumHightScore = NetUtil.GetJsonLong(jsonObject, "totalSumHightScore");
			m_userResult.m_quickTotalSumHightScore = NetUtil.GetJsonLong(jsonObject, "quickTotalSumHightScore");
			m_userResult.m_numTakeAllRings = NetUtil.GetJsonLong(jsonObject, "numTakeAllRings");
			m_userResult.m_numTakeAllRedRings = NetUtil.GetJsonLong(jsonObject, "numTakeAllRedRings");
			m_userResult.m_numChaoRoulette = NetUtil.GetJsonInt(jsonObject, "numChaoRoulette");
			m_userResult.m_numItemRoulette = NetUtil.GetJsonInt(jsonObject, "numItemRoulette");
			m_userResult.m_numJackPot = NetUtil.GetJsonInt(jsonObject, "numJackPot");
			m_userResult.m_numMaximumJackPotRings = NetUtil.GetJsonInt(jsonObject, "numMaximumJackPotRings");
			m_userResult.m_numSupport = NetUtil.GetJsonInt(jsonObject, "numSupport");
		}
	}
}
