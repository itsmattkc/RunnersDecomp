using LitJson;
using System.Collections.Generic;

public class NetServerEventUpdateGameResults : NetBase
{
	private ServerEventGameResults m_paramEventGameResults;

	private ServerPlayerState m_playerState;

	private ServerPlayCharacterState[] m_playCharacterState;

	private ServerWheelOptions m_wheelOptions;

	private List<ServerItemState> m_dailyMissionIncentiveList = new List<ServerItemState>();

	private List<ServerMessageEntry> m_messageEntryList = new List<ServerMessageEntry>();

	private List<ServerOperatorMessageEntry> m_operatorMessageEntryList = new List<ServerOperatorMessageEntry>();

	private int m_totalMessage;

	private int m_totalOperatorMessage;

	private List<ServerItemState> m_eventIncentiveList = new List<ServerItemState>();

	private ServerEventState m_eventState;

	private ServerEventRaidBossBonus m_raidBossBonus;

	public ServerPlayerState PlayerState
	{
		get
		{
			return m_playerState;
		}
	}

	public ServerPlayCharacterState[] PlayerCharacterState
	{
		get
		{
			return m_playCharacterState;
		}
	}

	public ServerWheelOptions WheelOptions
	{
		get
		{
			return m_wheelOptions;
		}
	}

	public List<ServerItemState> DailyMissionIncentiveList
	{
		get
		{
			return m_dailyMissionIncentiveList;
		}
	}

	public List<ServerMessageEntry> MessageEntryList
	{
		get
		{
			return m_messageEntryList;
		}
	}

	public int TotalMessage
	{
		get
		{
			return m_totalMessage;
		}
	}

	public List<ServerOperatorMessageEntry> OperatorMessageEntryList
	{
		get
		{
			return m_operatorMessageEntryList;
		}
	}

	public int TotalOperatorMessage
	{
		get
		{
			return m_totalOperatorMessage;
		}
	}

	public List<ServerItemState> EventIncentiveList
	{
		get
		{
			return m_eventIncentiveList;
		}
	}

	public ServerEventState EventState
	{
		get
		{
			return m_eventState;
		}
	}

	public ServerEventRaidBossBonus RaidBossBonus
	{
		get
		{
			return m_raidBossBonus;
		}
	}

	public NetServerEventUpdateGameResults(ServerEventGameResults eventGameResults)
	{
		m_paramEventGameResults = eventGameResults;
	}

	protected override void DoRequest()
	{
		SetAction("Event/eventUpdateGameResults");
		SetParameter_Rings();
		SetParameter_Suspended();
		SetParameter_DailyMission();
		SetParameter_EventRaidBoss();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_PlayCharacterState(jdata);
		GetResponse_WheelOptions(jdata);
		GetResponse_DailyMissionIncentives(jdata);
		GetResponse_MessageList(jdata);
		GetResponse_EventRaidBoss(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Suspended()
	{
		WriteActionParamValue("closed", m_paramEventGameResults.m_isSuspended ? 1 : 0);
	}

	private void SetParameter_Rings()
	{
		WriteActionParamValue("numRings", m_paramEventGameResults.m_numRings);
		WriteActionParamValue("numRedStarRings", m_paramEventGameResults.m_numRedStarRings);
		WriteActionParamValue("numFailureRings", m_paramEventGameResults.m_numFailureRings);
	}

	private void SetParameter_DailyMission()
	{
		WriteActionParamValue("dailyChallengeValue", m_paramEventGameResults.m_dailyMissionValue);
		WriteActionParamValue("dailyChallengeComplete", m_paramEventGameResults.m_dailyMissionComplete ? 1 : 0);
	}

	private void SetParameter_EventRaidBoss()
	{
		WriteActionParamValue("eventId", m_paramEventGameResults.m_eventId);
		WriteActionParamValue("eventValue", m_paramEventGameResults.m_eventValue);
		WriteActionParamValue("raidbossId", m_paramEventGameResults.m_raidBossId);
		WriteActionParamValue("raidbossDamage", m_paramEventGameResults.m_raidBossDamage);
		WriteActionParamValue("raidbossBeatFlg", m_paramEventGameResults.m_isRaidBossBeat ? 1 : 0);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		m_playerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_PlayCharacterState(JsonData jdata)
	{
		m_playCharacterState = NetUtil.AnalyzePlayerState_PlayCharactersStates(jdata);
	}

	private void GetResponse_WheelOptions(JsonData jdata)
	{
		m_wheelOptions = NetUtil.AnalyzeWheelOptionsJson(jdata, "wheelOptions");
		if (m_wheelOptions.m_numJackpotRing > 0)
		{
			RouletteManager.numJackpotRing = m_wheelOptions.m_numJackpotRing;
			Debug.Log("numJackpotRing : " + RouletteManager.numJackpotRing);
		}
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
		m_totalMessage = NetUtil.GetJsonInt(jdata, "totalMessage");
		m_totalOperatorMessage = NetUtil.GetJsonInt(jdata, "totalOperatorMessage");
	}

	private void GetResponse_EventRaidBoss(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "eventIncentiveList");
		if (jsonArray != null)
		{
			int count = jsonArray.Count;
			for (int i = 0; i < count; i++)
			{
				ServerItemState item = NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty);
				m_eventIncentiveList.Add(item);
			}
		}
		m_eventState = NetUtil.AnalyzeEventState(jdata);
		m_raidBossBonus = NetUtil.AnalyzeEventRaidBossBonus(jdata);
	}
}
