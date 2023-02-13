using DataTable;
using Message;
using Mission;
using System.Collections.Generic;
using UnityEngine;

public class StageMissionManager : MonoBehaviour
{
	private static readonly MissionTypeParam[] MISSION_TYPE_PARAM_TBL = new MissionTypeParam[6]
	{
		new MissionTypeParam(EventID.ENEMYDEAD, MissionCategory.COUNT),
		new MissionTypeParam(EventID.GOLDENENEMYDEAD, MissionCategory.COUNT),
		new MissionTypeParam(EventID.TOTALDISTANCE, MissionCategory.COUNT),
		new MissionTypeParam(EventID.GET_ANIMALS, MissionCategory.COUNT),
		new MissionTypeParam(EventID.GET_SCORE, MissionCategory.COUNT),
		new MissionTypeParam(EventID.GET_RING, MissionCategory.COUNT)
	};

	private List<MissionCheck> m_missionChecks;

	private static StageMissionManager instance;

	public bool Completed
	{
		get;
		private set;
	}

	public static StageMissionManager Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	private void Update()
	{
		if (m_missionChecks == null)
		{
			return;
		}
		foreach (MissionCheck missionCheck in m_missionChecks)
		{
			missionCheck.Update(Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
		DeleteAllMissionCheck();
		if (this == instance)
		{
			instance = null;
		}
	}

	public void SetupMissions()
	{
		if (m_missionChecks == null)
		{
			m_missionChecks = new List<MissionCheck>();
		}
		if (m_missionChecks == null || !SaveDataManager.Instance)
		{
			return;
		}
		DailyMissionData dailyMission = SaveDataManager.Instance.PlayerData.DailyMission;
		int id = dailyMission.id;
		MissionData missionData = MissionTable.GetMissionData(id);
		if (missionData != null)
		{
			bool flag = true;
			if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage() && missionData.type == MissionData.Type.RING)
			{
				flag = false;
			}
			if (dailyMission.missions_complete)
			{
				flag = false;
			}
			if (flag)
			{
				bool isSetInitialValue = missionData.save;
				long progress = dailyMission.progress;
				CreateMissionCheck(missionData, isSetInitialValue, progress);
			}
		}
	}

	public void SaveMissions()
	{
		if (m_missionChecks == null)
		{
			return;
		}
		foreach (MissionCheck missionCheck in m_missionChecks)
		{
			MissionData data = missionCheck.GetData();
			if (data == null || !(SaveDataManager.Instance != null))
			{
				continue;
			}
			DailyMissionData dailyMission = SaveDataManager.Instance.PlayerData.DailyMission;
			if (data.save && dailyMission.id == data.id)
			{
				dailyMission.progress = missionCheck.GetValue();
				if (missionCheck.IsCompleted())
				{
					dailyMission.missions_complete = true;
					Completed = true;
				}
			}
		}
	}

	private void CreateMissionCheck(MissionData data, bool isSetInitialValue = false, long initialValue = 0)
	{
		MissionCheck missionCheck = null;
		if ((uint)data.type < MISSION_TYPE_PARAM_TBL.Length && MISSION_TYPE_PARAM_TBL[(int)data.type].m_category == MissionCategory.COUNT)
		{
			MissionCheckCount missionCheckCount = new MissionCheckCount(MISSION_TYPE_PARAM_TBL[(int)data.type].m_eventID);
			missionCheck = missionCheckCount;
		}
		if (missionCheck != null)
		{
			if (isSetInitialValue)
			{
				missionCheck.SetInitialValue(initialValue);
			}
			missionCheck.SetData(data);
			m_missionChecks.Add(missionCheck);
		}
	}

	private void OnMissionEvent(MsgMissionEvent msg)
	{
		if (m_missionChecks == null)
		{
			return;
		}
		foreach (MsgMissionEvent.Data mission in msg.m_missions)
		{
			MissionEvent missionEvent = new MissionEvent(mission.eventid, mission.num);
			ProcEvent(missionEvent);
		}
	}

	private void ProcEvent(MissionEvent missionEvent)
	{
		if (m_missionChecks == null)
		{
			return;
		}
		foreach (MissionCheck missionCheck in m_missionChecks)
		{
			missionCheck.ProcEvent(missionEvent);
		}
	}

	private void DeleteAllMissionCheck()
	{
		m_missionChecks = null;
	}

	private bool GetMissionIsCompletedAndValue(int index, ref bool? isCompleted, ref int? value)
	{
		return false;
	}

	private bool IsCompletedMission(int index)
	{
		bool? isCompleted = false;
		int? value = null;
		GetMissionIsCompletedAndValue(index, ref isCompleted, ref value);
		return isCompleted.Value;
	}

	private int GetMissionValue(int index)
	{
		bool? isCompleted = null;
		int? value = 0;
		if (GetMissionIsCompletedAndValue(index, ref isCompleted, ref value))
		{
			return value.Value;
		}
		return 0;
	}

	private void DebugComplete(int missionNo)
	{
		if (m_missionChecks == null)
		{
			return;
		}
		foreach (MissionCheck missionCheck in m_missionChecks)
		{
			if (missionCheck.GetIndex() == missionNo)
			{
				missionCheck.DebugComplete(missionNo);
				break;
			}
		}
	}

	private void DebugComplete()
	{
		if (m_missionChecks == null)
		{
			return;
		}
		foreach (MissionCheck missionCheck in m_missionChecks)
		{
			missionCheck.DebugComplete(missionCheck.GetIndex());
		}
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}
}
