using DataTable;
using Message;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ranking_help : WindowBase
{
	[SerializeField]
	private GameObject page0;

	[SerializeField]
	private GameObject page1;

	[SerializeField]
	private UITable page1Table;

	private UILabel m_labelLeague;

	private UILabel m_labelLeagueEx;

	private UILabel m_labelBody;

	private Dictionary<string, UISprite> m_imgRanks;

	private UISprite m_imgLeagueIcon;

	private UISprite m_imgLeagueStar;

	private LeagueType m_currentLeague;

	private int m_upCount;

	private int m_dnCount;

	private bool m_rewardListInit;

	private bool m_open;

	private RankingUtil.RankingMode m_rankingMode;

	private void Start()
	{
		Setup();
	}

	public void Open(bool rewardListRest)
	{
		m_open = true;
		if (rewardListRest)
		{
			m_rewardListInit = false;
		}
		m_rankingMode = RankingUtil.currentRankingMode;
	}

	private void Setup()
	{
		int leagueIndex = 0;
		RankingUtil.GetMyLeagueData(m_rankingMode, ref leagueIndex, ref m_upCount, ref m_dnCount);
		m_currentLeague = (LeagueType)leagueIndex;
		if (m_labelLeague == null)
		{
			m_labelLeague = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_league");
		}
		if (m_labelLeagueEx == null)
		{
			m_labelLeagueEx = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_league_ex");
		}
		if (m_labelBody == null)
		{
			m_labelBody = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_body");
		}
		if (m_imgRanks == null)
		{
			m_imgRanks = new Dictionary<string, UISprite>();
			List<string> list = new List<string>();
			list.Add("F");
			list.Add("E");
			list.Add("D");
			list.Add("C");
			list.Add("B");
			list.Add("A");
			list.Add("S");
			foreach (string item in list)
			{
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_rank_" + item.ToLower());
				if (uISprite != null)
				{
					m_imgRanks.Add(item, uISprite);
				}
			}
		}
		if (m_imgLeagueIcon == null)
		{
			m_imgLeagueIcon = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_icon_league");
		}
		if (m_imgLeagueStar == null)
		{
			m_imgLeagueStar = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_icon_league_sub");
		}
		m_labelLeague.text = GetRankingCurrent(m_currentLeague);
		m_labelLeagueEx.text = GetRankingHelpText(m_rankingMode, m_currentLeague);
		m_labelBody.text = GetRankingHelpPresentText();
		if (page0 != null)
		{
			UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(page0, "ScrollView");
			if (uIDraggablePanel != null)
			{
				uIDraggablePanel.ResetPosition();
			}
		}
		SetCurrentRankImage(m_currentLeague);
	}

	public void OnClose()
	{
		SoundManager.SePlay("sys_window_close");
		m_open = false;
	}

	private static string GetRankingHelpPresentText()
	{
		string result = string.Empty;
		if (SingletonGameObject<RankingManager>.Instance == null)
		{
			return result;
		}
		ServerLeagueData leagueData = SingletonGameObject<RankingManager>.Instance.GetLeagueData(RankingUtil.currentRankingMode);
		if (leagueData != null)
		{
			string itemText = RankingLeagueTable.GetItemText(leagueData.highScoreOpe);
			string itemText2 = RankingLeagueTable.GetItemText(leagueData.totalScoreOpe);
			result = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_1").text, new Dictionary<string, string>
			{
				{
					"{PARAM1}",
					itemText
				},
				{
					"{PARAM2}",
					itemText2
				}
			});
		}
		return result;
	}

	private bool SetCurrentRankImage(LeagueType leagueType)
	{
		if (m_imgRanks != null && m_imgRanks.Count > 0)
		{
			string leagueCategoryName = RankingUtil.GetLeagueCategoryName(leagueType);
			if (m_imgRanks.ContainsKey(leagueCategoryName))
			{
				Dictionary<string, UISprite>.KeyCollection keys = m_imgRanks.Keys;
				foreach (string item in keys)
				{
					m_imgRanks[item].gameObject.SetActive(false);
				}
				m_imgRanks[leagueCategoryName].gameObject.SetActive(true);
				m_imgLeagueIcon.spriteName = "ui_ranking_league_icon_" + leagueCategoryName.ToLower();
				m_imgLeagueStar.spriteName = "ui_ranking_league_icon_" + RankingUtil.GetLeagueCategoryClass(leagueType);
			}
		}
		return false;
	}

	private static string GetRankingCurrent(LeagueType leagueType)
	{
		string empty = string.Empty;
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_5").text;
		return TextUtility.Replaces(text, new Dictionary<string, string>
		{
			{
				"{PARAM_1}",
				RankingUtil.GetLeagueName(leagueType)
			}
		});
	}

	private static string GetRankingHelpText(RankingUtil.RankingMode rankingMode, LeagueType leagueType)
	{
		int num = 21;
		int num2 = 21 - (int)leagueType;
		int upCount = 0;
		int downCount = 0;
		int leagueIndex = 0;
		RankingUtil.GetMyLeagueData(rankingMode, ref leagueIndex, ref upCount, ref downCount);
		string empty = string.Empty;
		switch (num2)
		{
		case 1:
			return TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_3").text, new Dictionary<string, string>
			{
				{
					"{PARAM_1}",
					num.ToString()
				},
				{
					"{PARAM_2}",
					num2.ToString()
				},
				{
					"{PARAM_4}",
					downCount.ToString()
				}
			});
		default:
			if (downCount > 0)
			{
				return TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_2").text, new Dictionary<string, string>
				{
					{
						"{PARAM_1}",
						num.ToString()
					},
					{
						"{PARAM_2}",
						num2.ToString()
					},
					{
						"{PARAM_3}",
						upCount.ToString()
					},
					{
						"{PARAM_4}",
						downCount.ToString()
					}
				});
			}
			goto case 21;
		case 21:
			return TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_4").text, new Dictionary<string, string>
			{
				{
					"{PARAM_1}",
					num.ToString()
				},
				{
					"{PARAM_2}",
					num2.ToString()
				},
				{
					"{PARAM_3}",
					upCount.ToString()
				}
			});
		}
	}

	private void OnClickToggle()
	{
		SoundManager.SePlay("sys_menu_decide");
		if (page1.activeSelf || m_rewardListInit)
		{
			return;
		}
		page1Table.repositionNow = false;
		List<GameObject> list = GameObjectUtil.FindChildGameObjects(page1Table.gameObject, "ui_ranking_reward(Clone)");
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				GameObject parent = list[i];
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(parent, "img_icon_league");
				UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(parent, "img_icon_league_sub");
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_body");
				uILabel.text = string.Empty;
				uILabel.alpha = 0f;
				uISprite.alpha = 0f;
				uISprite2.alpha = 0f;
				string leagueCategoryName = RankingUtil.GetLeagueCategoryName((LeagueType)(list.Count - (i + 1)));
				uISprite.spriteName = "ui_ranking_league_icon_s_" + leagueCategoryName.ToLower();
				uISprite2.spriteName = "ui_ranking_league_icon_s_" + RankingUtil.GetLeagueCategoryClass((LeagueType)(list.Count - (i + 1)));
			}
		}
		if (ServerInterface.LoggedInServerInterface != null)
		{
			ServerInterface.LoggedInServerInterface.RequestServerGetLeagueOperatorData((int)RankingUtil.currentRankingMode, base.gameObject);
		}
		m_rewardListInit = true;
		page1Table.repositionNow = true;
	}

	private void ServerGetLeagueOperatorData_Succeeded(MsgGetLeagueOperatorDataSucceed msg)
	{
		page1Table.repositionNow = false;
		List<GameObject> list = GameObjectUtil.FindChildGameObjects(page1Table.gameObject, "ui_ranking_reward(Clone)");
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < msg.m_leagueOperatorData.Count; i++)
			{
				int index = msg.m_leagueOperatorData.Count - (i + 1);
				ServerLeagueOperatorData serverLeagueOperatorData = msg.m_leagueOperatorData[index];
				if (serverLeagueOperatorData != null)
				{
					GameObject gameObject = list[i];
					UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_icon_league");
					UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_icon_league_sub");
					UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_body");
					uISprite.alpha = 1f;
					uISprite2.alpha = 1f;
					uILabel.alpha = 1f;
					string leagueCategoryName = RankingUtil.GetLeagueCategoryName((LeagueType)serverLeagueOperatorData.leagueId);
					uISprite.spriteName = "ui_ranking_league_icon_s_" + leagueCategoryName.ToLower();
					uISprite2.spriteName = "ui_ranking_league_icon_s_" + RankingUtil.GetLeagueCategoryClass((LeagueType)serverLeagueOperatorData.leagueId);
					string itemText = RankingLeagueTable.GetItemText(serverLeagueOperatorData.highScoreOpe);
					string itemText2 = RankingLeagueTable.GetItemText(serverLeagueOperatorData.totalScoreOpe);
					string empty = string.Empty;
					empty = (uILabel.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_1").text, new Dictionary<string, string>
					{
						{
							"{PARAM1}",
							itemText
						},
						{
							"{PARAM2}",
							itemText2
						}
					}));
					gameObject.SendMessage("OnClickBg");
				}
			}
		}
		page1Table.repositionNow = true;
	}

	private void ServerGetLeagueOperatorData_Failed()
	{
		m_rewardListInit = false;
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (!m_open)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
