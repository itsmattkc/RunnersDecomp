using DataTable;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class EtcEventInfo : EventBaseInfo
{
	public override void Init()
	{
		if (m_init || EventManager.Instance == null)
		{
			return;
		}
		string cellID = "ui_Lbl_word_animl_get_event";
		string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "ui_Lbl_word_animal_get_total");
		m_totalPointTarget = EVENT_AGGREGATE_TARGET.ANIMAL;
		m_rightTitle = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_animal_get_total");
		switch (EventManager.Instance.CollectType)
		{
		case EventManager.CollectEventType.GET_RING:
			m_totalPointTarget = EVENT_AGGREGATE_TARGET.RING;
			m_rightTitle = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_ring_get_total");
			cellID = "ui_Lbl_word_ring_get_event";
			text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "ui_Lbl_word_ring_get_total");
			break;
		case EventManager.CollectEventType.RUN_DISTANCE:
			m_totalPointTarget = EVENT_AGGREGATE_TARGET.DISTANCE;
			m_rightTitle = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_run_distance_total");
			cellID = "ui_Lbl_word_run_distance_event";
			text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "ui_Lbl_word_run_distance_get_total");
			break;
		}
		m_eventName = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", cellID);
		m_totalPoint = EventManager.Instance.CollectCount;
		m_caption = TextUtility.GetCommonText("Event", "ui_Lbl_event_reward_list");
		m_rightTitleIcon = "ui_event_object_icon";
		m_eventMission = new List<EventMission>();
		List<ServerEventReward> rewardList = EventManager.Instance.RewardList;
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
		m_leftTitle = TextUtility.GetCommonText("Roulette", "ui_Lbl_word_recommended_chao");
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
		m_init = true;
	}

	public override void UpdateData(MonoBehaviour obj)
	{
		if (!m_init)
		{
			Init();
		}
		else if (m_dummyData)
		{
		}
	}

	protected override void DebugInit()
	{
		if (m_init)
		{
			return;
		}
		m_totalPoint = 123456L;
		m_totalPointTarget = EVENT_AGGREGATE_TARGET.ANIMAL;
		m_dummyData = true;
		m_eventName = "EtcEvent";
		m_eventMission = new List<EventMission>();
		List<int> list = new List<int>();
		list.Add(120100);
		list.Add(121000);
		list.Add(120101);
		list.Add(121001);
		list.Add(120102);
		list.Add(121002);
		list.Add(120103);
		list.Add(121003);
		list.Add(120104);
		list.Add(121004);
		list.Add(120105);
		list.Add(121005);
		list.Add(120106);
		list.Add(121006);
		list.Add(120107);
		list.Add(121007);
		for (int i = 0; i < 10; i++)
		{
			long point = (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1);
			m_eventMission.Add(new EventMission("獲得動物数_" + (i + 1), point, list[i % list.Count], i));
		}
		m_rewardChao = new List<ChaoData>();
		List<ChaoData> dataTable = ChaoTable.GetDataTable(ChaoData.Rarity.SRARE);
		if (dataTable != null && dataTable.Count > 0)
		{
			m_rewardChao.Add(dataTable[Random.Range(0, dataTable.Count - 1)]);
		}
		int chaoLevel = ChaoTable.ChaoMaxLevel();
		m_leftTitle = "今週の目玉";
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
		m_caption = "動物獲得イベント報酬(デバック)";
		m_rightTitle = "動物を集めろ";
		m_rightTitleIcon = "ui_event_object_icon";
		m_init = true;
	}

	public static EtcEventInfo CreateData()
	{
		EtcEventInfo etcEventInfo = new EtcEventInfo();
		etcEventInfo.Init();
		return etcEventInfo;
	}

	public static EtcEventInfo CreateDummyData()
	{
		EtcEventInfo result = null;
		Debug.LogWarning("EtcEventInfo:DummyDataCreate  not create!!!");
		return result;
	}
}
