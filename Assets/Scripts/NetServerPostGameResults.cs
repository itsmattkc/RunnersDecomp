using LitJson;
using System.Collections.Generic;

public class NetServerPostGameResults : NetBase
{
	public List<ServerMessageEntry> m_messageEntryList = new List<ServerMessageEntry>();

	public List<ServerOperatorMessageEntry> m_operatorMessageEntryList = new List<ServerOperatorMessageEntry>();

	public int m_totalMessage;

	public int m_totalOperatorMessage;

	public List<ServerItemState> m_resultEventIncentiveList;

	public ServerEventState m_resultEventState;

	public ServerGameResults m_paramGameResults
	{
		get;
		set;
	}

	public ServerPlayerState m_resultPlayerState
	{
		get;
		private set;
	}

	public ServerCharacterState[] resultCharacterState
	{
		get;
		private set;
	}

	public List<ServerChaoState> resultChaoState
	{
		get;
		private set;
	}

	public ServerPlayCharacterState[] resultPlayCharacterState
	{
		get;
		private set;
	}

	public List<ServerMileageIncentive> m_resultMileageIncentive
	{
		get;
		private set;
	}

	public ServerMileageMapState m_resultMileageMapState
	{
		get;
		private set;
	}

	public List<ServerItemState> m_resultDailyMissionIncentiveList
	{
		get;
		set;
	}

	public int resultDailyMissionIncentives
	{
		get
		{
			if (m_resultDailyMissionIncentiveList != null)
			{
				return m_resultDailyMissionIncentiveList.Count;
			}
			return 0;
		}
	}

	public int resultMileageIncentives
	{
		get
		{
			if (m_resultMileageIncentive != null)
			{
				return m_resultMileageIncentive.Count;
			}
			return 0;
		}
	}

	public NetServerPostGameResults(ServerGameResults gameResults)
	{
		m_paramGameResults = gameResults;
	}

	protected override void DoRequest()
	{
		SetAction("Game/postGameResults");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string postGameResultString = instance.GetPostGameResultString(m_paramGameResults);
			Debug.Log("NetServerPostGameResults.json = " + postGameResultString);
			WriteJsonString(postGameResultString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
		GetResponse_ChaoState(jdata);
		GetResponse_PlayCharacterState(jdata);
		GetResponse_MileageMapState(jdata);
		GetResponse_DailyMissionIncentives(jdata);
		GetResponse_MileageIncentives(jdata);
		GetResponse_MessageList(jdata);
		GetResponse_Event(jdata);
		GetResponse_WheelOptions(jdata);
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
		WriteActionParamValue("stageMaxScore", m_paramGameResults.m_maxChapterScore);
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

	private void SetParameter_Mileage()
	{
		WriteActionParamValue("reachPoint", m_paramGameResults.m_reachPoint);
		WriteActionParamValue("chapterClear", m_paramGameResults.m_clearChapter ? 1 : 0);
		WriteActionParamValue("numBossAttack", m_paramGameResults.m_numBossAttack);
	}

	private void SetParameter_ChaoEggPresent()
	{
		WriteActionParamValue("getChaoEgg", m_paramGameResults.m_chaoEggPresent ? 1 : 0);
	}

	private void SetParameter_BossDestroyed()
	{
		WriteActionParamValue("bossDestroyed", m_paramGameResults.m_isBossDestroyed ? 1 : 0);
	}

	private void SetParameter_Event()
	{
		int? eventId = m_paramGameResults.m_eventId;
		if (eventId.HasValue)
		{
			WriteActionParamValue("eventId", m_paramGameResults.m_eventId);
			long? eventValue = m_paramGameResults.m_eventValue;
			if (eventValue.HasValue)
			{
				WriteActionParamValue("eventValue", m_paramGameResults.m_eventValue);
			}
		}
	}

	public ServerItemState GetResultDailyMissionIncentive(int index)
	{
		if (0 <= index && resultDailyMissionIncentives > index)
		{
			return m_resultDailyMissionIncentiveList[index];
		}
		return null;
	}

	public ServerMileageIncentive GetResultMileageIncentive(int index)
	{
		if (0 <= index && resultMileageIncentives > index)
		{
			return m_resultMileageIncentive[index];
		}
		return null;
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		m_resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_WheelOptions(JsonData jdata)
	{
		ServerWheelOptions serverWheelOptions = NetUtil.AnalyzeWheelOptionsJson(jdata, "wheelOptions");
		if (serverWheelOptions.m_numJackpotRing > 0)
		{
			RouletteManager.numJackpotRing = serverWheelOptions.m_numJackpotRing;
			Debug.Log("!!!! numJackpotRing : " + RouletteManager.numJackpotRing);
		}
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}

	private void GetResponse_PlayCharacterState(JsonData jdata)
	{
		resultPlayCharacterState = NetUtil.AnalyzePlayerState_PlayCharactersStates(jdata);
	}

	private void GetResponse_MileageMapState(JsonData jdata)
	{
		m_resultMileageMapState = NetUtil.AnalyzeMileageMapStateJson(jdata, "mileageMapState");
	}

	private void GetResponse_DailyMissionIncentives(JsonData jdata)
	{
		m_resultDailyMissionIncentiveList = new List<ServerItemState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "dailyChallengeIncentive");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerItemState item = NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty);
			m_resultDailyMissionIncentiveList.Add(item);
		}
	}

	private void GetResponse_MileageIncentives(JsonData jdata)
	{
		m_resultMileageIncentive = new List<ServerMileageIncentive>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "mileageIncentiveList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerMileageIncentive item = NetUtil.AnalyzeMileageIncentiveJson(jsonArray[i], string.Empty);
			m_resultMileageIncentive.Add(item);
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
		m_totalMessage = NetUtil.GetJsonInt(jdata, "totalMessage");
		m_totalOperatorMessage = NetUtil.GetJsonInt(jdata, "totalOperatorMessage");
	}

	private void GetResponse_Event(JsonData jdata)
	{
		m_resultEventIncentiveList = new List<ServerItemState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "eventIncentiveList");
		if (jsonArray != null)
		{
			int count = jsonArray.Count;
			for (int i = 0; i < count; i++)
			{
				ServerItemState item = NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty);
				m_resultEventIncentiveList.Add(item);
			}
		}
		m_resultEventState = NetUtil.AnalyzeEventState(jdata);
	}
}
