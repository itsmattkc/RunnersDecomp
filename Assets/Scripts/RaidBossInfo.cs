using DataTable;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RaidBossInfo : EventBaseInfo
{
	public delegate void CallbackRaidBossInfoUpdate(RaidBossInfo info);

	private static RaidBossData m_currentRaidData;

	private string m_bossName;

	private long m_raidRing;

	private long m_raidRingOffset;

	private List<RaidBossData> m_raidData;

	private CallbackRaidBossInfoUpdate m_callback;

	public static RaidBossData currentRaidData
	{
		get
		{
			return m_currentRaidData;
		}
		set
		{
			m_currentRaidData = value;
		}
	}

	public string bossName
	{
		get
		{
			return m_bossName;
		}
	}

	public long totalDestroyCount
	{
		get
		{
			return m_totalPoint;
		}
		set
		{
			m_totalPoint = value;
		}
	}

	public long raidRing
	{
		get
		{
			return m_raidRing + m_raidRingOffset;
		}
		set
		{
			m_raidRing = value;
			m_raidRingOffset = 0L;
		}
	}

	public long raidRingOffset
	{
		get
		{
			return m_raidRingOffset;
		}
		set
		{
			m_raidRingOffset = value;
		}
	}

	public List<RaidBossData> raidData
	{
		get
		{
			return m_raidData;
		}
	}

	public CallbackRaidBossInfoUpdate callback
	{
		set
		{
			m_callback = value;
		}
	}

	public int raidNumActive
	{
		get
		{
			if (m_raidData == null)
			{
				return 0;
			}
			int num = 0;
			foreach (RaidBossData raidDatum in m_raidData)
			{
				if (raidDatum != null && (!raidDatum.end || raidDatum.IsDiscoverer() || !raidDatum.participation))
				{
					num++;
				}
			}
			return num;
		}
	}

	public int raidNumLost
	{
		get
		{
			if (m_raidData == null)
			{
				return 0;
			}
			int num = 0;
			foreach (RaidBossData raidDatum in m_raidData)
			{
				if (raidDatum != null && raidDatum.end && !raidDatum.IsDiscoverer() && raidDatum.participation)
				{
					num++;
				}
			}
			return num;
		}
	}

	public override void Init()
	{
		if (m_init)
		{
			return;
		}
		m_eventName = "RaidBoss(正式なテキストを追加してください)";
		string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "ui_Lbl_word_boss_destroy");
		List<ServerEventReward> rewardList = EventManager.Instance.RewardList;
		m_eventMission = new List<EventMission>();
		if (rewardList != null)
		{
			for (int i = 0; i < rewardList.Count; i++)
			{
				ServerEventReward serverEventReward = rewardList[i];
				m_eventMission.Add(new EventMission(text, serverEventReward.Param, serverEventReward.m_itemId, serverEventReward.m_num));
			}
		}
		m_rewardChao = new List<ChaoData>();
		RewardChaoData rewardChaoData = EventManager.Instance.GetRewardChaoData();
		if (rewardChaoData != null)
		{
			ChaoData chaoData = ChaoTable.GetChaoData(rewardChaoData.chao_id);
			if (chaoData != null)
			{
				m_rewardChao.Add(chaoData);
			}
		}
		EyeCatcherChaoData[] eyeCatcherChaoDatas = EventManager.Instance.GetEyeCatcherChaoDatas();
		if (eyeCatcherChaoDatas != null)
		{
			EyeCatcherChaoData[] array = eyeCatcherChaoDatas;
			foreach (EyeCatcherChaoData eyeCatcherChaoData in array)
			{
				ChaoData chaoData2 = ChaoTable.GetChaoData(eyeCatcherChaoData.chao_id);
				if (chaoData2 != null)
				{
					m_rewardChao.Add(chaoData2);
				}
			}
		}
		int chaoLevel = ChaoTable.ChaoMaxLevel();
		m_leftTitle = TextUtility.GetCommonText("Roulette", "ui_Lbl_word_best_chao");
		if (m_rewardChao.Count > 0)
		{
			m_leftName = m_rewardChao[0].nameTwolines;
			m_leftText = m_rewardChao[0].GetDetailsLevel(chaoLevel);
			switch (m_rewardChao[0].rarity)
			{
			case ChaoData.Rarity.NORMAL:
				m_leftBg = "ui_tex_chao_bg_0";
				break;
			case ChaoData.Rarity.RARE:
				m_leftBg = "ui_tex_chao_bg_1";
				break;
			case ChaoData.Rarity.SRARE:
				m_leftBg = "ui_tex_chao_bg_2";
				break;
			}
			switch (m_rewardChao[0].charaAtribute)
			{
			case CharacterAttribute.SPEED:
				m_chaoTypeIcon = "ui_chao_set_type_icon_speed";
				break;
			case CharacterAttribute.FLY:
				m_chaoTypeIcon = "ui_chao_set_type_icon_fly";
				break;
			case CharacterAttribute.POWER:
				m_chaoTypeIcon = "ui_chao_set_type_icon_power";
				break;
			}
		}
		m_caption = TextUtility.GetCommonText("Event", "ui_Lbl_event_reward_list");
		m_rightTitle = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_destroy_total");
		m_rightTitleIcon = "ui_event_object_icon";
		m_init = true;
	}

	public override void UpdateData(MonoBehaviour obj)
	{
		if (!m_init)
		{
			Init();
		}
		else
		{
			m_callback(this);
		}
	}

	public bool IsAttention()
	{
		bool result = false;
		if (m_raidData != null && m_raidData.Count > 0)
		{
			foreach (RaidBossData raidDatum in m_raidData)
			{
				if (!raidDatum.end && raidDatum.id != 0L)
				{
					return true;
				}
				if (raidDatum.participation)
				{
					return true;
				}
			}
			return result;
		}
		return result;
	}

	public static RaidBossInfo CreateData(List<RaidBossData> raidBossDatas)
	{
		RaidBossInfo raidBossInfo = new RaidBossInfo();
		raidBossInfo.Init();
		raidBossInfo.m_raidData = new List<RaidBossData>();
		if (raidBossDatas != null)
		{
			foreach (RaidBossData raidBossData in raidBossDatas)
			{
				raidBossInfo.m_raidData.Add(raidBossData);
			}
			return raidBossInfo;
		}
		return raidBossInfo;
	}

	public static RaidBossInfo CreateDataForDebugData(List<RaidBossData> raidBossDatas)
	{
		RaidBossInfo result = null;
		Debug.LogWarning("RaidBossInfo:DummyDataCreate  not create!!!");
		return result;
	}
}
