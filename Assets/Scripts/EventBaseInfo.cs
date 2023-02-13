using DataTable;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventBaseInfo
{
	public enum EVENT_AGGREGATE_TARGET
	{
		SP_CRYSTAL,
		RAID_BOSS,
		ANIMAL,
		CRYSTAL,
		RING,
		DISTANCE,
		NONE
	}

	private static int s_pointSetCount;

	protected List<ChaoData> m_rewardChao;

	protected string m_eventName;

	protected bool m_init;

	protected bool m_dummyData;

	protected long m_totalPoint;

	protected EVENT_AGGREGATE_TARGET m_totalPointTarget = EVENT_AGGREGATE_TARGET.NONE;

	protected string m_caption;

	protected string m_leftTitle;

	protected string m_leftName;

	protected string m_leftText;

	protected string m_leftBg;

	protected string m_chaoTypeIcon;

	protected Texture m_leftTex;

	protected string m_rightTitle;

	protected string m_rightTitleIcon;

	protected List<EventMission> m_eventMission;

	public long totalPoint
	{
		get
		{
			return m_totalPoint;
		}
	}

	public EVENT_AGGREGATE_TARGET totalPointTarget
	{
		get
		{
			return m_totalPointTarget;
		}
	}

	public string totalPointUnitsString
	{
		get
		{
			string result = string.Empty;
			switch (m_totalPointTarget)
			{
			case EVENT_AGGREGATE_TARGET.SP_CRYSTAL:
				result = "ko";
				break;
			case EVENT_AGGREGATE_TARGET.RAID_BOSS:
				result = "tai";
				break;
			case EVENT_AGGREGATE_TARGET.ANIMAL:
				result = "hiki";
				break;
			case EVENT_AGGREGATE_TARGET.CRYSTAL:
				result = "ko";
				break;
			case EVENT_AGGREGATE_TARGET.RING:
				result = "ko";
				break;
			case EVENT_AGGREGATE_TARGET.DISTANCE:
				result = "me-toru";
				break;
			}
			return result;
		}
	}

	public string eventName
	{
		get
		{
			return m_eventName;
		}
	}

	public string caption
	{
		get
		{
			return m_caption;
		}
	}

	public string leftTitle
	{
		get
		{
			return m_leftTitle;
		}
	}

	public string leftName
	{
		get
		{
			return m_leftName;
		}
	}

	public string leftText
	{
		get
		{
			return m_leftText;
		}
	}

	public string leftBg
	{
		get
		{
			return m_leftBg;
		}
	}

	public string chaoTypeIcon
	{
		get
		{
			return m_chaoTypeIcon;
		}
	}

	public Texture leftTex
	{
		get
		{
			return m_leftTex;
		}
	}

	public string rightTitle
	{
		get
		{
			return m_rightTitle;
		}
	}

	public string rightTitleIcon
	{
		get
		{
			return m_rightTitleIcon;
		}
	}

	public List<EventMission> eventMission
	{
		get
		{
			return m_eventMission;
		}
	}

	public abstract void Init();

	public abstract void UpdateData(MonoBehaviour obj);

	protected virtual void DebugInit()
	{
	}

	public ChaoData GetRewardChao()
	{
		ChaoData result = null;
		if (m_rewardChao != null && m_rewardChao.Count > 0)
		{
			result = m_rewardChao[0];
		}
		return result;
	}

	private int GetAttainmentMissionNum(long point)
	{
		int num = 0;
		if (m_eventMission != null && m_eventMission.Count > 0)
		{
			for (int i = 0; i < m_eventMission.Count; i++)
			{
				if (m_eventMission[i] != null && m_eventMission[i].IsAttainment(point))
				{
					num++;
				}
			}
		}
		return num;
	}

	public bool SetTotalPoint(int point, out List<EventMission> mission)
	{
		bool flag = false;
		long totalPoint = this.totalPoint;
		m_totalPoint = point;
		if (totalPoint != m_totalPoint)
		{
			s_pointSetCount++;
		}
		return GetCurrentClearMission(totalPoint, out mission);
	}

	public bool SetTotalPoint(int point)
	{
		bool flag = false;
		long totalPoint = this.totalPoint;
		m_totalPoint = point;
		if (totalPoint != m_totalPoint)
		{
			s_pointSetCount++;
		}
		return GetCurrentClearMission(totalPoint);
	}

	public bool GetCurrentClearMission(long oldTotalPoint, out List<EventMission> mission, bool nextMission = false)
	{
		bool result = false;
		mission = null;
		if (totalPoint >= oldTotalPoint)
		{
			int attainmentMissionNum = GetAttainmentMissionNum(totalPoint);
			int attainmentMissionNum2 = GetAttainmentMissionNum(oldTotalPoint);
			if (attainmentMissionNum >= attainmentMissionNum2 && m_eventMission != null && m_eventMission.Count > 0)
			{
				for (int i = 0; i < m_eventMission.Count; i++)
				{
					if (m_eventMission[i] == null)
					{
						continue;
					}
					if (m_eventMission[i].IsAttainment(totalPoint))
					{
						if (!m_eventMission[i].IsAttainment(oldTotalPoint))
						{
							if (mission == null)
							{
								mission = new List<EventMission>();
								mission.Add(m_eventMission[i]);
							}
							else
							{
								mission.Add(m_eventMission[i]);
							}
							result = true;
						}
						continue;
					}
					if (nextMission)
					{
						if (mission == null)
						{
							mission = new List<EventMission>();
							mission.Add(m_eventMission[i]);
						}
						else
						{
							mission.Add(m_eventMission[i]);
						}
						result = true;
					}
					break;
				}
			}
		}
		return result;
	}

	public bool GetCurrentClearMission(long oldTotalPoint)
	{
		bool result = false;
		if (totalPoint > oldTotalPoint)
		{
			int attainmentMissionNum = GetAttainmentMissionNum(totalPoint);
			int attainmentMissionNum2 = GetAttainmentMissionNum(oldTotalPoint);
			if (attainmentMissionNum > attainmentMissionNum2)
			{
				result = true;
			}
		}
		return result;
	}
}
