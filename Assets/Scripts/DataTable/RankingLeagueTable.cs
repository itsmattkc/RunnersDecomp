using System.Collections.Generic;
using Text;
using UnityEngine;

namespace DataTable
{
	public class RankingLeagueTable : MonoBehaviour
	{
		private static RankingLeagueTable s_instance;

		public static RankingLeagueTable Instance
		{
			get
			{
				return s_instance;
			}
		}

		private void Awake()
		{
			if (s_instance == null)
			{
				Object.DontDestroyOnLoad(base.gameObject);
				s_instance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (s_instance == this)
			{
				s_instance = null;
			}
		}

		private void Setup()
		{
			base.enabled = false;
		}

		public static string GetItemText(List<ServerRemainOperator> leagueDataRemainOperator, string unitText = null, string unitTextMore = null, int head = 0, bool descendingOrder = false)
		{
			int num = 9876543;
			string text = string.Empty;
			List<RankingLeagueItem> list = new List<RankingLeagueItem>();
			List<ServerRemainOperator> list2 = new List<ServerRemainOperator>();
			Dictionary<int, List<ServerRemainOperator>> dictionary = new Dictionary<int, List<ServerRemainOperator>>();
			if (head < 0)
			{
				head = 0;
			}
			int num2 = 0;
			foreach (ServerRemainOperator item in leagueDataRemainOperator)
			{
				int num3 = 0;
				if (item.operatorData != 6)
				{
					int number = item.number;
					if (item.operatorData == 2)
					{
						item.operatorData = 4;
						item.number++;
					}
					else if (item.operatorData == 3)
					{
						item.operatorData = 5;
						item.number--;
					}
					if (item.number >= head)
					{
						list2.Add(item);
					}
					num3++;
					if (num2 < number)
					{
						num2 = number;
					}
				}
				else if (!dictionary.ContainsKey(num3))
				{
					List<ServerRemainOperator> list3 = new List<ServerRemainOperator>();
					list3.Add(item);
					dictionary.Add(num3, list3);
				}
				else
				{
					dictionary[num3].Add(item);
				}
			}
			if (list2.Count == 0)
			{
				bool flag = true;
				if (dictionary.Count > 0)
				{
					Dictionary<int, List<ServerRemainOperator>>.KeyCollection keys = dictionary.Keys;
					foreach (int item2 in keys)
					{
						List<ServerRemainOperator> list4 = dictionary[item2];
						foreach (ServerRemainOperator item3 in list4)
						{
							if (head % item3.number == 0)
							{
								flag = false;
								ServerRemainOperator serverRemainOperator = new ServerRemainOperator();
								item3.CopyTo(serverRemainOperator);
								serverRemainOperator.number = head;
								serverRemainOperator.operatorData = 0;
								list2.Add(serverRemainOperator);
								serverRemainOperator = leagueDataRemainOperator[leagueDataRemainOperator.Count - 1];
								serverRemainOperator.number = head + 1;
								if (serverRemainOperator.operatorData == 2)
								{
									serverRemainOperator.operatorData = 4;
									serverRemainOperator.number++;
								}
								list2.Add(serverRemainOperator);
								break;
							}
						}
						if (!flag)
						{
							break;
						}
					}
				}
				if (flag)
				{
					ServerRemainOperator serverRemainOperator2 = leagueDataRemainOperator[leagueDataRemainOperator.Count - 1];
					serverRemainOperator2.number = head;
					if (serverRemainOperator2.operatorData == 2)
					{
						serverRemainOperator2.operatorData = 4;
						serverRemainOperator2.number++;
					}
					list2.Add(serverRemainOperator2);
				}
			}
			if (dictionary.Count > 0)
			{
				Debug.Log("+serverRemainOpeLoop:" + dictionary.Count + "   serverRemainOpeLoop:" + list2.Count);
				Dictionary<int, List<ServerRemainOperator>>.KeyCollection keys2 = dictionary.Keys;
				foreach (int item4 in keys2)
				{
					List<ServerRemainOperator> list5 = dictionary[item4];
					ServerRemainOperator serverRemainOperator3 = null;
					List<bool> list6 = new List<bool>();
					if (leagueDataRemainOperator.Count > 0)
					{
						serverRemainOperator3 = leagueDataRemainOperator[leagueDataRemainOperator.Count - 1];
					}
					int num4 = 0;
					foreach (ServerRemainOperator item5 in list5)
					{
						if (num4 < item5.number)
						{
							num4 = item5.number;
						}
						list6.Add(false);
					}
					int num5 = head - num2;
					if (num5 < 0)
					{
						num5 = 0;
					}
					for (int i = num2 + 1 + num5; i < num4 + (num2 + 1) + head; i++)
					{
						ServerRemainOperator serverRemainOperator4 = null;
						for (int j = 0; j < list5.Count; j++)
						{
							if (i % list5[j].number == 0)
							{
								serverRemainOperator4 = new ServerRemainOperator();
								list5[j].CopyTo(serverRemainOperator4);
								list6[j] = true;
								break;
							}
						}
						if (serverRemainOperator4 != null)
						{
							serverRemainOperator4.number = i;
							serverRemainOperator4.operatorData = 0;
							list2.Add(serverRemainOperator4);
							if (serverRemainOperator3 != null && serverRemainOperator3.operatorData == 4)
							{
								ServerRemainOperator serverRemainOperator5 = new ServerRemainOperator();
								serverRemainOperator3.CopyTo(serverRemainOperator5);
								serverRemainOperator5.number = i + 1;
								list2.Add(serverRemainOperator5);
							}
						}
					}
				}
				Debug.Log("-serverRemainOpeLoop:" + dictionary.Count + "   serverRemainOpeLoop:" + list2.Count);
			}
			if (list2.Count > 0)
			{
				list2.Sort(RemainDownComparer);
				foreach (ServerRemainOperator item6 in list2)
				{
					RankingLeagueItem rankingLeagueItem = new RankingLeagueItem();
					rankingLeagueItem.ranking1 = item6.number;
					rankingLeagueItem.ranking2 = item6.number;
					rankingLeagueItem.operatorData = item6.operatorData;
					if (item6.ItemState != null && item6.ItemState.Count > 0)
					{
						Dictionary<int, ServerItemState>.KeyCollection keys3 = item6.ItemState.Keys;
						foreach (int item7 in keys3)
						{
							ServerItem serverItem = new ServerItem((ServerItem.Id)item7);
							ServerItemState serverItemState = new ServerItemState();
							serverItemState.m_itemId = (int)serverItem.id;
							serverItemState.m_num = item6.ItemState[item7].m_num;
							rankingLeagueItem.item.Add(serverItemState);
						}
					}
					list.Add(rankingLeagueItem);
				}
				switch (list[0].operatorData)
				{
				case 2:
				case 4:
					list[0].ranking2 = num;
					break;
				}
				int num6 = list[0].ranking2 + 1;
				foreach (RankingLeagueItem item8 in list)
				{
					item8.ranking2 = num6 - 1;
					int operatorData = item8.operatorData;
					if (operatorData == 2)
					{
						item8.ranking1++;
					}
					num6 = item8.ranking1;
				}
				if (!descendingOrder)
				{
					list.Reverse(0, list.Count);
				}
			}
			if (list.Count > 0)
			{
				string text2 = "\n      ";
				string text3 = (!string.IsNullOrEmpty(unitText)) ? unitText : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "rank_unit").text;
				for (int k = 0; k < list.Count; k++)
				{
					string text4 = text3;
					RankingLeagueItem rankingLeagueItem2 = list[k];
					string str = " ";
					int ranking = rankingLeagueItem2.ranking1;
					int ranking2 = rankingLeagueItem2.ranking2;
					if (ranking == ranking2)
					{
						string text5 = text4;
						text5 = TextUtility.Replaces(text5, new Dictionary<string, string>
						{
							{
								"{PARAM}",
								ranking.ToString()
							}
						});
						str += text5;
					}
					else
					{
						string text5 = ranking.ToString();
						string text6 = text4;
						if (ranking2 != num)
						{
							text6 = TextUtility.Replaces(text6, new Dictionary<string, string>
							{
								{
									"{PARAM}",
									ranking2.ToString()
								}
							});
							str = str + text5 + " - " + text6;
						}
						else if (!string.IsNullOrEmpty(unitTextMore))
						{
							text5 = TextUtility.Replaces(unitTextMore, new Dictionary<string, string>
							{
								{
									"{PARAM}",
									ranking.ToString()
								}
							});
							str += text5;
						}
						else
						{
							text6 = "   ";
							str = str + text5 + " - " + text6;
						}
					}
					str += " ";
					text4 = string.Empty;
					for (int l = 0; l < rankingLeagueItem2.item.Count; l++)
					{
						text4 += GetRankingHelpItem(new ServerItem((ServerItem.Id)rankingLeagueItem2.item[l].m_itemId), rankingLeagueItem2.item[l].m_num);
						if (l + 1 < rankingLeagueItem2.item.Count)
						{
							text4 += ",";
						}
					}
					if (!string.IsNullOrEmpty(text4))
					{
						if (k > 0)
						{
							text += "\n";
						}
						text = text + str + text4;
					}
				}
			}
			return text;
		}

		private static string GetRankingHelpItem(ServerItem serverItem, int num)
		{
			switch (serverItem.idType)
			{
			case ServerItem.IdType.RSRING:
			{
				TextObject text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_rs_ring");
				text3.ReplaceTag("{PARAM}", HudUtility.GetFormatNumString(num));
				return text3.text;
			}
			case ServerItem.IdType.RING:
			{
				TextObject text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_help_ring");
				text2.ReplaceTag("{PARAM}", HudUtility.GetFormatNumString(num));
				return text2.text;
			}
			case ServerItem.IdType.CHARA:
			{
				int idIndex2 = serverItem.idIndex;
				if ((uint)idIndex2 < CharaName.Name.Length)
				{
					return TextUtility.GetCommonText("CharaName", CharaName.Name[idIndex2]);
				}
				break;
			}
			case ServerItem.IdType.CHAO:
			{
				int idIndex = serverItem.idIndex;
				ChaoData chaoData = ChaoTable.GetChaoData(idIndex);
				if (chaoData != null)
				{
					return chaoData.name;
				}
				break;
			}
			default:
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "tutorial_sp_egg1_text").text;
				text = text.Replace("{COUNT}", HudUtility.GetFormatNumString(num));
				return serverItem.serverItemName + " " + text;
			}
			}
			return string.Empty;
		}

		private static int RankUpComparer(RankingLeagueItem itemA, RankingLeagueItem itemB)
		{
			if (itemA != null && itemB != null)
			{
				if (itemA.ranking1 > itemB.ranking1)
				{
					return 1;
				}
				if (itemA.ranking1 < itemB.ranking1)
				{
					return -1;
				}
			}
			return 0;
		}

		private static int RemainDownComparer(ServerRemainOperator itemA, ServerRemainOperator itemB)
		{
			if (itemA != null && itemB != null)
			{
				if (itemA.number > itemB.number)
				{
					return -1;
				}
				if (itemA.number < itemB.number)
				{
					return 1;
				}
			}
			return 0;
		}

		private static int RemainUpComparer(ServerRemainOperator itemA, ServerRemainOperator itemB)
		{
			if (itemA != null && itemB != null)
			{
				if (itemA.number > itemB.number)
				{
					return 1;
				}
				if (itemA.number < itemB.number)
				{
					return -1;
				}
			}
			return 0;
		}

		public static void SetupRankingLeagueTable()
		{
			RankingLeagueTable instance = Instance;
			if (instance == null)
			{
				GameObject gameObject = new GameObject("RankingLeagueTable");
				gameObject.AddComponent<RankingLeagueTable>();
				instance = Instance;
				if (instance != null)
				{
					instance.Setup();
				}
			}
			else
			{
				instance.Setup();
			}
		}

		public static RankingLeagueTable GetRankingLeagueTable()
		{
			RankingLeagueTable instance = Instance;
			if (instance == null)
			{
				SetupRankingLeagueTable();
				instance = Instance;
			}
			return instance;
		}
	}
}
