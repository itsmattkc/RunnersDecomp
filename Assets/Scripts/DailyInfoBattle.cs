using DataTable;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class DailyInfoBattle : MonoBehaviour
{
	private const float UPDATE_LIMIT_TIME = 0.25f;

	private const float RELOAD_TIME = 3f;

	private DailyInfo m_parent;

	private DateTime m_currentEndTime;

	private int m_currentWinFlag;

	private ServerDailyBattleDataPair m_currentData;

	private ServerDailyBattleStatus m_currentStatus;

	private UILabel m_wins;

	private UILabel m_limitTime;

	private UIButton m_reloadBtn;

	private List<UIImageButton> m_matchingBtns;

	private GameObject m_myDataObject;

	private GameObject m_rivalDataObject;

	private GameObject m_noMatchingObject;

	private GameObject m_noScoreObject;

	private GameObject m_menuObject;

	private GameObject m_myWin;

	private GameObject m_myLose;

	private GameObject m_rivalWin;

	private GameObject m_rivalLose;

	private GameObject m_vsObject;

	private UIRectItemStorage m_myDataStorage;

	private UIRectItemStorage m_rivalDataStorage;

	private ui_ranking_scroll m_myData;

	private ui_ranking_scroll m_rivalData;

	private int m_matchingIndex;

	private float m_time;

	private float m_updateLimitTime;

	private float m_reloadTime;

	private string m_matchingOldUserId = string.Empty;

	private bool m_matchingResultEnd;

	private bool m_costError;

	private bool m_isEnableRematch = true;

	private ButtonInfoTable.ButtonType m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;

	private bool isEffectVS
	{
		get
		{
			bool result = true;
			if (GeneralWindow.Created || ranking_window.isActive)
			{
				result = false;
			}
			return result;
		}
	}

	private void Update()
	{
		if (m_time > 0f && base.gameObject.activeSelf)
		{
			float deltaTime = Time.deltaTime;
			if (m_reloadTime > 0f)
			{
				m_reloadTime -= deltaTime;
				if (m_matchingResultEnd && m_reloadTime <= 2.5f)
				{
					HudMenuUtility.SetConnectAlertSimpleUI(false);
					m_matchingResultEnd = false;
				}
				if (m_reloadTime <= 0f)
				{
					if (m_reloadBtn != null && !m_reloadBtn.isEnabled)
					{
						DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
						if (instance != null && instance.IsRequestResetMatching())
						{
							m_reloadBtn.isEnabled = true;
							m_reloadTime = 0f;
							HudMenuUtility.SetConnectAlertSimpleUI(false);
						}
						else
						{
							m_reloadTime = 3f;
						}
					}
					if (m_matchingBtns != null && m_matchingBtns.Count > 0)
					{
						foreach (UIImageButton matchingBtn in m_matchingBtns)
						{
							if (!matchingBtn.isEnabled)
							{
								DailyBattleManager instance2 = SingletonGameObject<DailyBattleManager>.Instance;
								if (instance2 != null && instance2.IsRequestResetMatching())
								{
									matchingBtn.isEnabled = true;
									m_reloadTime = 0f;
									HudMenuUtility.SetConnectAlertSimpleUI(false);
								}
								else
								{
									m_reloadTime = 3f;
								}
							}
						}
					}
					m_reloadTime = 0f;
				}
			}
			m_updateLimitTime -= deltaTime;
			if (m_updateLimitTime <= 0f)
			{
				if (m_limitTime != null)
				{
					string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_limit_time");
					string timeLimitString = GeneralUtil.GetTimeLimitString(m_currentEndTime);
					m_limitTime.text = text.Replace("{TIME}", timeLimitString);
				}
				m_updateLimitTime = 0.25f;
			}
			m_time += Time.deltaTime;
		}
		if (m_vsObject != null)
		{
			m_vsObject.SetActive(isEffectVS);
		}
		if (m_matchingIndex > 0 && GeneralWindow.IsCreated("ShowMatchingInfo") && GeneralWindow.IsButtonPressed)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				Debug.Log("DailyInfoBattle.Update.Maching");
				Matching(m_matchingIndex);
			}
			else
			{
				m_matchingIndex = 0;
				m_isEnableRematch = true;
			}
		}
		if (m_costError)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				HudMenuUtility.SendMenuButtonClicked(m_costErrorType);
			}
			m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
			GeneralWindow.Close();
			m_costError = false;
			m_isEnableRematch = true;
		}
	}

	public void Setup(DailyInfo parent)
	{
		m_parent = parent;
		DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
		if (!(instance != null))
		{
			return;
		}
		m_myDataObject = GameObjectUtil.FindChildGameObject(base.gameObject, "duel_mine_set");
		m_rivalDataObject = GameObjectUtil.FindChildGameObject(base.gameObject, "duel_adversary_set");
		m_noMatchingObject = GameObjectUtil.FindChildGameObject(base.gameObject, "no_matching_set");
		m_noScoreObject = GameObjectUtil.FindChildGameObject(base.gameObject, "no_score_set");
		m_menuObject = GameObjectUtil.FindChildGameObject(base.gameObject, "time");
		m_wins = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_wins");
		m_limitTime = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_timelimit");
		m_reloadBtn = null;
		m_matchingIndex = 0;
		m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
		m_costError = false;
		if (m_matchingBtns != null)
		{
			m_matchingBtns.Clear();
		}
		m_matchingBtns = null;
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_battle_help", base.gameObject, "OnClickReward");
		if (m_wins != null)
		{
			m_wins.text = string.Empty;
		}
		if (m_limitTime != null)
		{
			m_limitTime.text = string.Empty;
		}
		if (m_menuObject != null)
		{
			m_menuObject.SetActive(false);
		}
		if (m_myDataObject != null)
		{
			m_myDataStorage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_myDataObject, "score_mine");
			if (m_myDataStorage != null)
			{
				m_myDataStorage.maxItemCount = (m_myDataStorage.maxRows = 0);
				m_myDataStorage.Restart();
			}
			m_myWin = GameObjectUtil.FindChildGameObject(m_myDataObject, "duel_win_set");
			m_myLose = GameObjectUtil.FindChildGameObject(m_myDataObject, "duel_lose_set");
			if (m_myWin != null && m_myLose != null)
			{
				m_myWin.SetActive(false);
				m_myLose.SetActive(false);
			}
			m_myDataObject.SetActive(false);
		}
		if (m_rivalDataObject != null)
		{
			m_vsObject = GameObjectUtil.FindChildGameObject(m_rivalDataObject, "deco");
			m_rivalDataStorage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_rivalDataObject, "score_adversary");
			if (m_rivalDataStorage != null)
			{
				m_rivalDataStorage.maxItemCount = (m_rivalDataStorage.maxRows = 0);
				m_rivalDataStorage.Restart();
			}
			m_rivalWin = GameObjectUtil.FindChildGameObject(m_rivalDataObject, "duel_win_set");
			m_rivalLose = GameObjectUtil.FindChildGameObject(m_rivalDataObject, "duel_lose_set");
			if (m_rivalWin != null && m_rivalLose != null)
			{
				m_rivalWin.SetActive(false);
				m_rivalLose.SetActive(false);
			}
			GeneralUtil.SetButtonFunc(base.gameObject, "Btn_matching_0", base.gameObject, "OnClickMatching1");
			GeneralUtil.SetButtonFunc(base.gameObject, "Btn_matching_1", base.gameObject, "OnClickMatching2");
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_matching_0");
			UIImageButton uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_matching_1");
			Dictionary<int, ServerConsumedCostData> resetCostList = instance.resetCostList;
			if (uIImageButton != null)
			{
				if (m_matchingBtns == null)
				{
					m_matchingBtns = new List<UIImageButton>();
				}
				m_matchingBtns.Add(uIImageButton);
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uIImageButton.gameObject, "Lbl_bet_number");
				if (uILabel != null)
				{
					if (resetCostList != null && resetCostList.ContainsKey(1))
					{
						uILabel.text = "×" + resetCostList[1].numItem;
					}
					else
					{
						uILabel.text = "---";
					}
				}
			}
			if (uIImageButton2 != null)
			{
				if (m_matchingBtns == null)
				{
					m_matchingBtns = new List<UIImageButton>();
				}
				m_matchingBtns.Add(uIImageButton2);
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uIImageButton2.gameObject, "Lbl_bet_number");
				if (uILabel2 != null)
				{
					if (resetCostList != null && resetCostList.ContainsKey(2))
					{
						uILabel2.text = "×" + resetCostList[2].numItem;
					}
					else
					{
						uILabel2.text = "---";
					}
				}
			}
			m_rivalDataObject.SetActive(false);
		}
		if (m_noMatchingObject != null)
		{
			GeneralUtil.SetButtonFunc(base.gameObject, "Btn_reload", base.gameObject, "OnClickMatching0");
			m_noMatchingObject.SetActive(false);
		}
		if (m_noScoreObject != null)
		{
			m_noScoreObject.SetActive(false);
		}
		m_time = 0f;
		m_updateLimitTime = 0f;
		if (instance.IsEndTimeOver())
		{
			instance.FirstSetup(DailyBattleManagerCallBack);
		}
		else
		{
			SetupParam();
		}
	}

	private void DailyBattleManagerCallBack()
	{
		SetupParam();
	}

	private void SetupParam()
	{
		DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
		if (instance != null)
		{
			bool flag = true;
			bool flag2 = true;
			m_currentWinFlag = instance.currentWinFlag;
			m_currentEndTime = instance.currentEndTime;
			m_currentData = instance.currentDataPair;
			m_currentStatus = instance.currentStatus;
			m_matchingResultEnd = false;
			if (m_wins != null)
			{
				if (m_currentStatus != null && m_currentStatus.goOnWin > 1)
				{
					string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_continuous_win");
					m_wins.text = text.Replace("{PARAM}", m_currentStatus.goOnWin.ToString());
				}
				else
				{
					m_wins.text = string.Empty;
				}
			}
			if (m_limitTime != null)
			{
				string text2 = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_limit_time");
				string timeLimitString = GeneralUtil.GetTimeLimitString(m_currentEndTime);
				m_limitTime.text = text2.Replace("{TIME}", timeLimitString);
			}
			if (m_currentData != null && !string.IsNullOrEmpty(m_currentData.myBattleData.userId))
			{
				flag = false;
			}
			if (m_currentData != null && !string.IsNullOrEmpty(m_currentData.rivalBattleData.userId) && !flag)
			{
				flag2 = false;
			}
			if (m_matchingBtns != null && m_matchingBtns.Count > 0 && m_reloadTime <= 0f)
			{
				foreach (UIImageButton matchingBtn in m_matchingBtns)
				{
					matchingBtn.isEnabled = true;
				}
			}
			if (!flag && flag2)
			{
				m_reloadBtn = GameObjectUtil.FindChildGameObjectComponent<UIButton>(base.gameObject, "Btn_reload");
				if (m_reloadBtn != null && m_reloadTime <= 0f)
				{
					m_reloadBtn.isEnabled = true;
				}
			}
			else
			{
				m_reloadBtn = null;
			}
			if (m_menuObject != null)
			{
				m_menuObject.SetActive(true);
			}
			if (m_noScoreObject != null)
			{
				m_noScoreObject.SetActive(flag);
			}
			if (m_noMatchingObject != null)
			{
				m_noMatchingObject.SetActive(flag2 && !flag);
			}
			if (m_myDataObject != null)
			{
				if (m_myDataStorage != null && !flag)
				{
					m_myDataStorage.maxItemCount = (m_myDataStorage.maxRows = 1);
					m_myDataStorage.Restart();
					m_myData = GameObjectUtil.FindChildGameObjectComponent<ui_ranking_scroll>(m_myDataStorage.gameObject, "ui_ranking_scroll(Clone)");
					if (m_myData != null)
					{
						RankingUtil.Ranker ranker = new RankingUtil.Ranker(m_currentData.myBattleData);
						m_myData.UpdateView(RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL, ranker, true);
						m_myData.SetMyRanker(true);
					}
					if (m_myWin != null && m_myLose != null)
					{
						if (!flag2 && m_currentWinFlag > 0)
						{
							if (m_currentWinFlag >= 2)
							{
								m_myWin.SetActive(true);
								m_myLose.SetActive(false);
							}
							else
							{
								m_myWin.SetActive(false);
								m_myLose.SetActive(true);
							}
						}
						else
						{
							m_myWin.SetActive(false);
							m_myLose.SetActive(false);
						}
					}
				}
				m_myDataObject.SetActive(!flag);
			}
			if (m_rivalDataObject != null)
			{
				if (m_rivalDataStorage != null && !flag2)
				{
					m_rivalDataStorage.maxItemCount = (m_rivalDataStorage.maxRows = 1);
					m_rivalDataStorage.Restart();
					m_rivalData = GameObjectUtil.FindChildGameObjectComponent<ui_ranking_scroll>(m_rivalDataStorage.gameObject, "ui_ranking_scroll(Clone)");
					if (m_rivalData != null)
					{
						RankingUtil.Ranker ranker2 = new RankingUtil.Ranker(m_currentData.rivalBattleData);
						if (ranker2 != null && ranker2.isFriend)
						{
							ranker2.isSentEnergy = true;
						}
						m_rivalData.UpdateView(RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL, ranker2, true);
						m_rivalData.SendButtonDisable = true;
					}
				}
				if (m_rivalWin != null && m_rivalLose != null)
				{
					if (!flag2 && m_currentWinFlag > 0)
					{
						if (m_currentWinFlag >= 2)
						{
							m_rivalWin.SetActive(false);
							m_rivalLose.SetActive(true);
						}
						else
						{
							m_rivalWin.SetActive(true);
							m_rivalLose.SetActive(false);
						}
					}
					else
					{
						m_rivalWin.SetActive(false);
						m_rivalLose.SetActive(false);
					}
				}
				m_rivalDataObject.SetActive(!flag2);
			}
		}
		m_time = 0.0001f;
		m_updateLimitTime = 0.25f;
	}

	private void UpdateRectItemStorage(UIRectItemStorage storage, RankingUtil.Ranker ranker, bool myFlag)
	{
		storage.maxItemCount = (storage.maxRows = 1);
		storage.Restart();
		ui_ranking_scroll[] componentsInChildren = storage.GetComponentsInChildren<ui_ranking_scroll>(true);
		for (int i = 0; i < storage.maxItemCount; i++)
		{
			if (ranker != null)
			{
				componentsInChildren[i].UpdateView(RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.ALL, ranker, true);
				componentsInChildren[i].SetMyRanker(myFlag);
			}
		}
	}

	private void OnClickReward()
	{
		Reward();
	}

	private void OnClickMatching0()
	{
		if (m_isEnableRematch)
		{
			m_isEnableRematch = false;
			if (m_parent != null)
			{
				m_matchingIndex = 0;
				Debug.Log("DailyInfoBattle.Update.OnClickMaching0");
				Matching(0);
			}
		}
	}

	private void OnClickMatching1()
	{
		if (m_isEnableRematch)
		{
			m_isEnableRematch = false;
			if (m_parent != null)
			{
				m_matchingIndex = 1;
				ShowMatchingInfo();
			}
		}
	}

	private void OnClickMatching2()
	{
		if (m_isEnableRematch)
		{
			m_isEnableRematch = false;
			if (m_parent != null)
			{
				m_matchingIndex = 2;
				ShowMatchingInfo();
			}
		}
	}

	private void Reward()
	{
		DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
		if (!(instance != null))
		{
			return;
		}
		List<ServerDailyBattlePrizeData> currentPrizeList = instance.currentPrizeList;
		if (currentPrizeList != null && currentPrizeList.Count > 0)
		{
			ShowReward(currentPrizeList);
		}
		else if (GeneralUtil.IsNetwork())
		{
			DailyBattleManager.CallbackGetPrize callback = CallbackGetPrizeFunc;
			if (!instance.RequestGetPrize(callback))
			{
				HudMenuUtility.SetConnectAlertSimpleUI(false);
			}
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
	}

	private void Matching(int resetIndex)
	{
		if (GeneralUtil.IsNetwork())
		{
			m_matchingIndex = 0;
			m_matchingResultEnd = false;
			m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
			m_costError = false;
			DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
			if (!(instance != null))
			{
				return;
			}
			Dictionary<int, ServerConsumedCostData> resetCostList = instance.resetCostList;
			if (resetCostList == null || !resetCostList.ContainsKey(resetIndex))
			{
				return;
			}
			ServerConsumedCostData serverConsumedCostData = resetCostList[resetIndex];
			if (serverConsumedCostData == null || serverConsumedCostData.itemId <= 0)
			{
				return;
			}
			ServerItem.Id itemId = (ServerItem.Id)serverConsumedCostData.itemId;
			long itemCount = GeneralUtil.GetItemCount(itemId);
			if (itemCount >= serverConsumedCostData.numItem || resetIndex == 0)
			{
				m_matchingOldUserId = string.Empty;
				if (m_currentData != null && m_currentData.rivalBattleData != null)
				{
					m_matchingOldUserId = m_currentData.rivalBattleData.userId;
				}
				Debug.Log("DailyInfoBattle.Maching!");
				HudMenuUtility.SetConnectAlertSimpleUI(true);
				DailyBattleManager.CallbackResetMatching callback = CallbackResetMatchingFunc;
				if (instance.RequestResetMatching(resetIndex, callback))
				{
					if (m_reloadBtn != null)
					{
						m_reloadBtn.isEnabled = false;
						m_reloadTime = 3f;
					}
					if (m_matchingBtns != null)
					{
						foreach (UIImageButton matchingBtn in m_matchingBtns)
						{
							matchingBtn.isEnabled = false;
						}
						m_reloadTime = 3f;
					}
				}
				else
				{
					HudMenuUtility.SetConnectAlertSimpleUI(false);
				}
				SoundManager.SePlay("sys_menu_decide");
				return;
			}
			switch (itemId)
			{
			case ServerItem.Id.RSRING:
			{
				bool flag = ServerInterface.IsRSREnable();
				GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
				info2.caption = TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption");
				info2.message = ((!flag) ? TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text_2") : TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text"));
				info2.buttonType = ((!flag) ? GeneralWindow.ButtonType.Ok : GeneralWindow.ButtonType.ShopCancel);
				info2.finishedCloseDelegate = GeneralWindowClosedCallback;
				info2.name = "MatchingCostErrorRSRing";
				info2.isPlayErrorSe = true;
				GeneralWindow.Create(info2);
				m_costErrorType = ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP;
				break;
			}
			case ServerItem.Id.RING:
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.caption = TextUtility.GetCommonText("ItemRoulette", "gw_cost_caption");
				info.message = TextUtility.GetCommonText("ItemRoulette", "gw_cost_text");
				info.buttonType = GeneralWindow.ButtonType.ShopCancel;
				info.name = "MatchingCostErrorRing";
				info.finishedCloseDelegate = GeneralWindowClosedCallback;
				info.isPlayErrorSe = true;
				GeneralWindow.Create(info);
				m_costErrorType = ButtonInfoTable.ButtonType.RING_TO_SHOP;
				break;
			}
			default:
				Debug.Log(string.Concat("DailyInfoBattle Matching error   itemId:", itemId, " !!!!!"));
				break;
			}
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
	}

	private void GeneralWindowClosedCallback()
	{
		m_costError = true;
		HudMenuUtility.SetConnectAlertSimpleUI(false);
	}

	private void CallbackGetPrizeFunc(List<ServerDailyBattlePrizeData> prizeList)
	{
		HudMenuUtility.SetConnectAlertSimpleUI(false);
		ShowReward(prizeList);
	}

	private void CallbackResetMatchingFunc(ServerPlayerState playerStatus, ServerDailyBattleDataPair dataPair, DateTime endTime)
	{
		HudMenuUtility.SetConnectAlertSimpleUI(false);
		m_isEnableRematch = true;
		if (playerStatus != null && dataPair != null)
		{
			m_currentData = dataPair;
			SetupParam();
			ShowMatchingResult(m_currentData);
		}
		else
		{
			Debug.Log("CallbackResetMatchingFunc error");
			SetupParam();
			ShowMatchingResult(null);
		}
	}

	private void ShowReward(List<ServerDailyBattlePrizeData> data)
	{
		if (data != null && data.Count > 0)
		{
			List<ServerRemainOperator> list = ServerDailyBattlePrizeData.ConvertRemainOperatorList(data);
			if (list != null)
			{
				int goOnWin = m_currentStatus.goOnWin;
				string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_win");
				string text2 = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_win_over");
				string itemText = RankingLeagueTable.GetItemText(list, text, text2, goOnWin - 1);
				if (!string.IsNullOrEmpty(itemText))
				{
					GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
					info.name = "ShowReward";
					info.buttonType = GeneralWindow.ButtonType.Ok;
					info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reward_caption");
					info.message = itemText;
					GeneralWindow.Create(info);
				}
			}
		}
		else
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.name = "ShowReward";
			info2.buttonType = GeneralWindow.ButtonType.Ok;
			info2.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reward_caption");
			info2.message = "Reward Failed";
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
		}
	}

	private void ShowMatchingInfo()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "ShowMatchingInfo";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reset_caption");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reset_text");
		GeneralWindow.Create(info);
	}

	private void ShowMatchingResult(ServerDailyBattleDataPair data)
	{
		bool flag = false;
		if (data != null && data.rivalBattleData != null)
		{
			flag = ((!(m_matchingOldUserId == data.rivalBattleData.userId) && !string.IsNullOrEmpty(data.rivalBattleData.userId)) ? true : false);
		}
		if (flag)
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "ShowMatchingResult";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reset_result_caption");
			info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reset_result_succeed");
			GeneralWindow.Create(info);
		}
		else
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.name = "ShowMatchingResult";
			info2.buttonType = GeneralWindow.ButtonType.Ok;
			info2.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reset_result_caption");
			info2.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_reset_result_failure");
			info2.isPlayErrorSe = true;
			GeneralWindow.Create(info2);
		}
		m_matchingIndex = 0;
		m_matchingResultEnd = true;
		m_matchingOldUserId = string.Empty;
		HudMenuUtility.SetConnectAlertSimpleUI(false);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}
}
