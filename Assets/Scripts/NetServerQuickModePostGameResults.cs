using LitJson;
using System.Collections.Generic;

public class NetServerQuickModePostGameResults : NetBase
{
	public List<ServerItemState> m_dailyMissionIncentiveList = new List<ServerItemState>();

	public List<ServerMessageEntry> m_messageEntryList = new List<ServerMessageEntry>();

	public List<ServerOperatorMessageEntry> m_operatorMessageEntryList = new List<ServerOperatorMessageEntry>();

	public ServerQuickModeGameResults m_paramGameResults
	{
		get;
		set;
	}

	public ServerPlayerState m_resultPlayerState
	{
		get;
		private set;
	}

	public ServerCharacterState[] m_resultCharacterState
	{
		get;
		private set;
	}

	public List<ServerChaoState> m_resultChaoState
	{
		get;
		private set;
	}

	public ServerPlayCharacterState[] m_resultPlayCharacterState
	{
		get;
		private set;
	}

	public int totalMessage
	{
		get
		{
			if (m_messageEntryList != null)
			{
				return m_messageEntryList.Count;
			}
			return 0;
		}
	}

	public int totalOperatorMessage
	{
		get
		{
			if (m_operatorMessageEntryList != null)
			{
				return m_operatorMessageEntryList.Count;
			}
			return 0;
		}
	}

	public NetServerQuickModePostGameResults(ServerQuickModeGameResults gameResults)
	{
		m_paramGameResults = gameResults;
	}

	protected override void DoRequest()
	{
		SetAction("Game/quickPostGameResults");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string quickModePostGameResultString = instance.GetQuickModePostGameResultString(m_paramGameResults);
			Debug.Log("NetServerQuickModePostGameResults.json = " + quickModePostGameResultString);
			WriteJsonString(quickModePostGameResultString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
		GetResponse_ChaoState(jdata);
		GetResponse_PlayCharacterState(jdata);
		GetResponse_DailyMissionIncentives(jdata);
		GetResponse_MessageList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Suspended()
	{
		WriteActionParamValue("closed", m_paramGameResults.m_isSuspended ? 1 : 0);
	}

	private void SetParameter_Score()
	{
		WriteActionParamValue("score", m_paramGameResults.m_score);
	}

	private void SetParameter_Rings()
	{
		WriteActionParamValue("numRings", m_paramGameResults.m_numRings);
		WriteActionParamValue("numFailureRings", m_paramGameResults.m_numFailureRings);
		WriteActionParamValue("numRedStarRings", m_paramGameResults.m_numRedStarRings);
	}

	private void SetParameter_Distance()
	{
		WriteActionParamValue("distance", m_paramGameResults.m_distance);
	}

	private void SetParameter_DailyMission()
	{
		WriteActionParamValue("dailyChallengeValue", m_paramGameResults.m_dailyMissionValue);
		WriteActionParamValue("dailyChallengeComplete", m_paramGameResults.m_dailyMissionComplete ? 1 : 0);
	}

	private void SetParameter_NumAnimals()
	{
		WriteActionParamValue("numAnimals", m_paramGameResults.m_numAnimals);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		m_resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		m_resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		m_resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}

	private void GetResponse_PlayCharacterState(JsonData jdata)
	{
		m_resultPlayCharacterState = NetUtil.AnalyzePlayerState_PlayCharactersStates(jdata);
	}

	private void GetResponse_DailyMissionIncentives(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "dailyChallengeIncentive");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerItemState item = NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty);
			m_dailyMissionIncentiveList.Add(item);
		}
	}

	private void GetResponse_MessageList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "messageList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerMessageEntry item = NetUtil.AnalyzeMessageEntryJson(jdata2, string.Empty);
			m_messageEntryList.Add(item);
		}
		JsonData jsonArray2 = NetUtil.GetJsonArray(jdata, "operatorMessageList");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			JsonData jdata3 = jsonArray2[j];
			ServerOperatorMessageEntry item2 = NetUtil.AnalyzeOperatorMessageEntryJson(jdata3, string.Empty);
			m_operatorMessageEntryList.Add(item2);
		}
	}
}
