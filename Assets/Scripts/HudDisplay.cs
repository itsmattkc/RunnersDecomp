using Message;
using System.Collections.Generic;
using UnityEngine;

public class HudDisplay
{
	public enum ObjType
	{
		Chao,
		NewItem,
		Main,
		Player,
		Option,
		Infomation,
		Roulette,
		Shop,
		PresentBox,
		DailyChallenge,
		DailyBattle,
		Ranking,
		Event,
		Mileage,
		NUM,
		NONE
	}

	private List<GameObject>[] m_obj_list = new List<GameObject>[14];

	public HudDisplay()
	{
		for (int i = 0; i < 14; i++)
		{
			m_obj_list[i] = new List<GameObject>();
		}
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			GameObject @object = GetObject(menuAnimUIObject, "ChaoSetUIPage");
			m_obj_list[0].Add(@object);
			m_obj_list[0].Add(GetObject(@object, "ChaoSetUI"));
			m_obj_list[6].Add(GetObject(menuAnimUIObject, "RouletteTopUI"));
			m_obj_list[2].Add(GetObject(menuAnimUIObject, "MainMenuUI4"));
			m_obj_list[3].Add(GetObject(menuAnimUIObject, "PlayerSet_3_UI"));
			GameObject object2 = GetObject(menuAnimUIObject, "ShopPage");
			m_obj_list[7].Add(object2);
			m_obj_list[7].Add(GetObject(object2, "ShopUI2"));
			m_obj_list[4].Add(GetObject(menuAnimUIObject, "OptionUI"));
			m_obj_list[5].Add(GetObject(menuAnimUIObject, "InformationUI"));
			m_obj_list[8].Add(GetObject(menuAnimUIObject, "PresentBoxUI"));
			m_obj_list[9].Add(GetObject(menuAnimUIObject, "DailyWindowUI"));
			m_obj_list[10].Add(GetObject(menuAnimUIObject, "DailyInfoUI"));
			m_obj_list[1].Add(GetObject(menuAnimUIObject, "ItemSet_3_UI"));
			m_obj_list[11].Add(GetObject(menuAnimUIObject, "ui_mm_ranking_page"));
			m_obj_list[13].Add(GetObject(menuAnimUIObject, "ui_mm_mileage2_page"));
		}
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject object3 = GetObject(cameraUIObject, "SpecialStageWindowUI");
			if (object3 != null)
			{
				m_obj_list[12].Add(object3);
			}
			GameObject object4 = GetObject(cameraUIObject, "RaidBossWindowUI");
			if (object4 != null)
			{
				m_obj_list[12].Add(object4);
			}
		}
	}

	public void SetAllDisableDisplay()
	{
		for (int i = 0; i < 14; i++)
		{
			if (m_obj_list[i] != null)
			{
				SetActiveListObj(m_obj_list[i], false);
			}
		}
	}

	public void SetDisplayHudObject(ObjType obj_type)
	{
		for (int i = 0; i < 14; i++)
		{
			if (m_obj_list[i] != null)
			{
				bool active_flag = i == (int)obj_type;
				SetActiveListObj(m_obj_list[i], active_flag);
			}
		}
	}

	private GameObject GetObject(GameObject menu_anim_obj, string obj_name)
	{
		if (menu_anim_obj != null && obj_name != null)
		{
			Transform transform = menu_anim_obj.transform.Find(obj_name);
			if (transform != null)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private void SetActiveListObj(List<GameObject> obj_list, bool active_flag)
	{
		if (obj_list == null)
		{
			return;
		}
		foreach (GameObject item in obj_list)
		{
			if (item != null)
			{
				item.SetActive(active_flag);
				ButtonEvent.DebugInfoDraw("SetActive " + item.name + " " + active_flag);
			}
		}
	}

	public static ObjType CalcObjTypeFromSequenceType(MsgMenuSequence.SequeneceType seqType)
	{
		ObjType result = ObjType.Main;
		switch (seqType)
		{
		case MsgMenuSequence.SequeneceType.PRESENT_BOX:
			result = ObjType.PresentBox;
			break;
		case MsgMenuSequence.SequeneceType.DAILY_CHALLENGE:
			result = ObjType.DailyChallenge;
			break;
		case MsgMenuSequence.SequeneceType.DAILY_BATTLE:
			result = ObjType.DailyBattle;
			break;
		case MsgMenuSequence.SequeneceType.CHARA_MAIN:
			result = ObjType.Player;
			break;
		case MsgMenuSequence.SequeneceType.CHAO:
			result = ObjType.Chao;
			break;
		case MsgMenuSequence.SequeneceType.PLAY_ITEM:
		case MsgMenuSequence.SequeneceType.EPISODE_PLAY:
		case MsgMenuSequence.SequeneceType.QUICK:
		case MsgMenuSequence.SequeneceType.PLAY_AT_EPISODE_PAGE:
		case MsgMenuSequence.SequeneceType.MAIN_PLAY_BUTTON:
			result = ObjType.NewItem;
			break;
		case MsgMenuSequence.SequeneceType.OPTION:
			result = ObjType.Option;
			break;
		case MsgMenuSequence.SequeneceType.INFOMATION:
			result = ObjType.Infomation;
			break;
		case MsgMenuSequence.SequeneceType.ROULETTE:
		case MsgMenuSequence.SequeneceType.CHAO_ROULETTE:
		case MsgMenuSequence.SequeneceType.ITEM_ROULETTE:
			result = ObjType.Roulette;
			break;
		case MsgMenuSequence.SequeneceType.SHOP:
			result = ObjType.Shop;
			break;
		case MsgMenuSequence.SequeneceType.MAIN:
			result = ObjType.Main;
			break;
		case MsgMenuSequence.SequeneceType.STAGE:
			result = ObjType.NONE;
			break;
		case MsgMenuSequence.SequeneceType.EVENT_TOP:
		case MsgMenuSequence.SequeneceType.EVENT_SPECIAL:
		case MsgMenuSequence.SequeneceType.EVENT_RAID:
		case MsgMenuSequence.SequeneceType.EVENT_COLLECT:
			result = ObjType.Event;
			break;
		case MsgMenuSequence.SequeneceType.EPISODE_RANKING:
		case MsgMenuSequence.SequeneceType.QUICK_RANKING:
			result = ObjType.Ranking;
			break;
		case MsgMenuSequence.SequeneceType.EPISODE:
			result = ObjType.Mileage;
			break;
		}
		return result;
	}
}
