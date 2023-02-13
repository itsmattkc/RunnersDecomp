using System.Collections.Generic;
using Text;
using UnityEngine;

public class DailyInfoHistory : MonoBehaviour
{
	private const int MAX_HISTORY_DATA = 30;

	private DailyInfo m_parent;

	private UILabel m_wins;

	private UILabel m_winStreak;

	private GameObject m_scrollObject;

	private UIRectItemStorage m_scrollStorage;

	private List<ui_daily_battle_scroll> m_historyList;

	public void Setup(DailyInfo parent)
	{
		m_parent = parent;
		Debug.Log("DailyInfoHistory Setup parent:" + (m_parent != null));
		DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
		if (!(instance != null))
		{
			return;
		}
		m_wins = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_wins");
		m_winStreak = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_win_streak");
		if (m_historyList != null)
		{
			m_historyList.Clear();
		}
		m_scrollObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Scrollsystem");
		if (m_scrollObject != null)
		{
			m_scrollStorage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_scrollObject, "slot");
			if (m_scrollStorage != null)
			{
				m_scrollStorage.maxItemCount = (m_scrollStorage.maxRows = 0);
				m_scrollStorage.Restart();
			}
		}
		if (m_wins != null)
		{
			m_wins.text = string.Empty;
		}
		if (m_winStreak != null)
		{
			m_winStreak.text = string.Empty;
		}
		bool flag = true;
		Debug.Log("currentDataPairList:" + ((instance.currentDataPairList == null) ? "null" : instance.currentDataPairList.Count.ToString()));
		if (instance.currentDataPairList != null)
		{
			if (instance.currentDataPairList.Count > 0)
			{
				if (!instance.IsReload(DailyBattleManager.REQ_TYPE.GET_DATA_HISTORY, 60.0))
				{
					flag = false;
				}
			}
			else if (instance.IsDataInit(DailyBattleManager.REQ_TYPE.GET_DATA_HISTORY) && !instance.IsReload(DailyBattleManager.REQ_TYPE.GET_DATA_HISTORY, 5.0))
			{
				flag = false;
			}
		}
		if (instance.IsEndTimeOver() || flag)
		{
			if (GeneralUtil.IsNetwork())
			{
				DailyBattleManager.CallbackGetDataHistory callback = CallbackHistory;
				if (instance.RequestGetDataHistory(30, callback))
				{
					HudMenuUtility.SetConnectAlertSimpleUI(true);
				}
				else
				{
					SetupParam();
				}
			}
			else
			{
				GeneralUtil.ShowNoCommunication("ShowNoCommunicationDailyInfoHistory");
			}
		}
		else
		{
			SetupParam();
		}
	}

	private void CallbackHistory(List<ServerDailyBattleDataPair> list)
	{
		SetupParam();
	}

	private void SetupParam()
	{
		DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
		if (instance != null)
		{
			List<ServerDailyBattleDataPair> currentDataPairList = instance.currentDataPairList;
			if (currentDataPairList != null && currentDataPairList.Count > 0)
			{
				m_scrollStorage.maxItemCount = (m_scrollStorage.maxRows = currentDataPairList.Count);
				m_scrollStorage.Restart();
				m_historyList = GameObjectUtil.FindChildGameObjectsComponents<ui_daily_battle_scroll>(m_scrollStorage.gameObject, "ui_daily_battle_scroll(Clone)");
				if (m_historyList != null && m_historyList.Count > 0)
				{
					for (int i = 0; i < currentDataPairList.Count; i++)
					{
						if (currentDataPairList[i] != null && m_historyList.Count > i)
						{
							m_historyList[i].UpdateView(currentDataPairList[i]);
						}
					}
				}
			}
			if (instance.currentStatus != null)
			{
				if (m_wins != null)
				{
					string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_result");
					m_wins.text = TextUtility.Replaces(text, new Dictionary<string, string>
					{
						{
							"{WIN}",
							instance.currentStatus.numWin.ToString()
						},
						{
							"{LOSE}",
							instance.currentStatus.numLose.ToString()
						},
						{
							"{FAILURE}",
							instance.currentStatus.numLoseByDefault.ToString()
						}
					});
				}
				if (m_winStreak != null)
				{
					if (instance.currentStatus.goOnWin > 1)
					{
						string text2 = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_continuous_win");
						m_winStreak.text = text2.Replace("{PARAM}", instance.currentStatus.goOnWin.ToString());
					}
					else
					{
						m_winStreak.text = string.Empty;
					}
				}
			}
		}
		HudMenuUtility.SetConnectAlertSimpleUI(false);
	}
}
