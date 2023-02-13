using DataTable;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class SpecialStageInfo : EventBaseInfo
{
	private string m_eventCaption;

	private string m_eventText;

	public string eventCaption
	{
		get
		{
			return m_eventCaption;
		}
	}

	public string eventText
	{
		get
		{
			return m_eventText;
		}
	}

	public override void Init()
	{
		if (m_init)
		{
			return;
		}
		m_eventName = HudUtility.GetEventStageName();
		m_totalPoint = EventManager.Instance.CollectCount;
		m_totalPointTarget = EVENT_AGGREGATE_TARGET.SP_CRYSTAL;
		m_eventCaption = m_eventName;
		m_eventText = "スペシャルステージイベント説明（デバック）\n\n\u3000あいうえお\n\u30001234567890\n\u3000ABCDEFG\n\n\u3000期間: XX/XX  XX:XX  -  XX/XX  XX:XX";
		m_eventMission = new List<EventMission>();
		string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", "ui_Lbl_word_get_total");
		List<ServerEventReward> rewardList = EventManager.Instance.RewardList;
		if (rewardList != null)
		{
			for (int i = 0; i < rewardList.Count; i++)
			{
				ServerEventReward serverEventReward = rewardList[i];
				string eventSpObjectName = HudUtility.GetEventSpObjectName();
				string name = TextUtility.Replace(text, "{PARAM_OBJ}", eventSpObjectName);
				m_eventMission.Add(new EventMission(name, serverEventReward.Param, serverEventReward.m_itemId, serverEventReward.m_num));
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
		m_rightTitle = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_get_total");
		m_rightTitleIcon = "ui_event_object_icon";
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
		m_totalPointTarget = EVENT_AGGREGATE_TARGET.SP_CRYSTAL;
		m_dummyData = true;
		m_eventName = "SpecialStage";
		m_eventCaption = "スペシャルステージイベント（デバック）";
		m_eventText = "スペシャルステージイベント説明（デバック）\n\n\u3000あいうえお\n\u30001234567890\n\u3000ABCDEFG\n\n\u3000期間: XX/XX  XX:XX  -  XX/XX  XX:XX";
		m_rewardChao = new List<ChaoData>();
		List<ChaoData> dataTable = ChaoTable.GetDataTable(ChaoData.Rarity.NORMAL);
		List<ChaoData> dataTable2 = ChaoTable.GetDataTable(ChaoData.Rarity.RARE);
		List<ChaoData> dataTable3 = ChaoTable.GetDataTable(ChaoData.Rarity.SRARE);
		bool flag = false;
		if (dataTable.Count > 0 && dataTable2.Count > 0 && dataTable3.Count > 0)
		{
			switch (Random.Range(1, 3))
			{
			case 0:
				m_rewardChao.Add(dataTable[Random.Range(0, dataTable.Count - 1)]);
				break;
			case 1:
				m_rewardChao.Add(dataTable2[Random.Range(0, dataTable2.Count - 1)]);
				break;
			case 2:
				m_rewardChao.Add(dataTable3[Random.Range(0, dataTable3.Count - 1)]);
				break;
			default:
				m_rewardChao.Add(dataTable3[Random.Range(0, dataTable3.Count - 1)]);
				break;
			}
		}
		else
		{
			flag = true;
			ChaoData chaoData = new ChaoData();
			chaoData.SetDummyData();
			m_rewardChao.Add(chaoData);
		}
		m_eventMission = new List<EventMission>();
		List<int> list = new List<int>();
		list.Add(400000 + m_rewardChao[0].id);
		list.Add(120100);
		list.Add(121000);
		list.Add(120101);
		list.Add(121001);
		list.Add(400000 + m_rewardChao[0].id);
		list.Add(120102);
		list.Add(121002);
		list.Add(120103);
		list.Add(121003);
		list.Add(400000 + m_rewardChao[0].id);
		list.Add(120104);
		list.Add(121004);
		list.Add(120105);
		list.Add(121005);
		list.Add(120106);
		list.Add(121006);
		list.Add(120107);
		list.Add(400000 + m_rewardChao[0].id);
		for (int i = 0; i < 10; i++)
		{
			long point = (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1) * (i + 1);
			m_eventMission.Add(new EventMission("累計SPクリスタル_" + (i + 1), point, list[i % list.Count], i));
		}
		int chaoLevel = ChaoTable.ChaoMaxLevel();
		m_leftTitle = "今週の目玉";
		if (m_rewardChao.Count > 0)
		{
			m_leftName = m_rewardChao[0].nameTwolines;
			m_leftText = ((!flag) ? m_rewardChao[0].GetDetailsLevel(chaoLevel) : "dummy text");
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
		m_caption = "スペシャルステージ報酬(デバック)";
		m_rightTitle = "○○を集めろ";
		m_rightTitleIcon = "ui_event_object_icon";
		m_init = true;
	}

	public static SpecialStageInfo CreateData()
	{
		SpecialStageInfo specialStageInfo = new SpecialStageInfo();
		specialStageInfo.Init();
		return specialStageInfo;
	}

	public static SpecialStageInfo CreateDummyData()
	{
		SpecialStageInfo result = null;
		Debug.LogWarning("SpecialStageInfo:DummyDataCreate  not create!!!");
		return result;
	}
}
