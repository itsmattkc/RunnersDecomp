using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Text;
using UnityEngine;

namespace DataTable
{
	public class ChaoTable : MonoBehaviour
	{
		private const int MARKER_INTERVAL = 10;

		private const int LEVEL_MAX = 10;

		[SerializeField]
		private TextAsset m_chaoTabel;

		private static ChaoData[] s_chaoDataTable;

		private static Dictionary<int, List<int>> s_chaoDataTableMarker;

		private static bool s_setup;

		private static bool s_eventList;

		private static int s_loadingCount;

		private static List<ChaoData> s_loadingChaoList;

		private void Start()
		{
			if (s_chaoDataTable == null)
			{
				string s = AESCrypt.Decrypt(m_chaoTabel.text);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ChaoData[]));
				StringReader textReader = new StringReader(s);
				s_chaoDataTable = (ChaoData[])xmlSerializer.Deserialize(textReader);
			}
		}

		private void OnDestroy()
		{
			s_chaoDataTable = null;
			if (s_chaoDataTableMarker != null)
			{
				s_chaoDataTableMarker.Clear();
				s_chaoDataTableMarker = null;
			}
			if (s_loadingChaoList != null)
			{
				s_loadingChaoList.Clear();
				s_loadingChaoList = null;
			}
			s_setup = false;
			s_loadingCount = 0;
			s_eventList = false;
		}

		private static void Setup()
		{
			if (s_setup || s_chaoDataTable == null)
			{
				return;
			}
			Array.Sort(s_chaoDataTable, ChaoData.ChaoCompareById);
			s_chaoDataTableMarker = new Dictionary<int, List<int>>();
			int item = 0;
			int num = -1;
			int num2 = -1;
			ChaoData[] array = s_chaoDataTable;
			foreach (ChaoData chaoData in array)
			{
				num = chaoData.id / 1000 % 10;
				if (num != num2)
				{
					if (!s_chaoDataTableMarker.ContainsKey(num))
					{
						s_chaoDataTableMarker[num] = new List<int>();
						s_chaoDataTableMarker[num].Add(item);
					}
				}
				else if (chaoData.id % 10 == 0 && s_chaoDataTableMarker.ContainsKey(num))
				{
					s_chaoDataTableMarker[num].Add(item);
				}
				chaoData.index = item++;
				chaoData.name = TextUtility.GetChaoText("Chao", "name" + chaoData.id.ToString("D4"));
				chaoData.nameTwolines = TextUtility.GetChaoText("Chao", "name_for_menu_" + chaoData.id.ToString("D4"));
				chaoData.StatusUpdate();
				num2 = num;
			}
			s_setup = true;
		}

		public static int ChaoMaxLevel()
		{
			return 10;
		}

		public static bool ChangeChaoAbility(int chaoId, int abilityEventId)
		{
			bool result = false;
			if (s_chaoDataTable != null)
			{
				ChaoData[] array = s_chaoDataTable;
				foreach (ChaoData chaoData in array)
				{
					if (chaoData.id == chaoId)
					{
						result = chaoData.SetChaoAbility(abilityEventId);
						break;
					}
				}
			}
			return result;
		}

		public static bool ChangeChaoAbilityIndex(int chaoId, int abilityIndex)
		{
			bool result = false;
			if (s_chaoDataTable != null)
			{
				ChaoData[] array = s_chaoDataTable;
				foreach (ChaoData chaoData in array)
				{
					if (chaoData.id == chaoId)
					{
						result = chaoData.SetChaoAbilityIndex(abilityIndex);
						break;
					}
				}
			}
			return result;
		}

		public static bool ChangeChaoAbilityNext(int chaoId)
		{
			bool result = false;
			if (s_chaoDataTable != null)
			{
				ChaoData[] array = s_chaoDataTable;
				foreach (ChaoData chaoData in array)
				{
					if (chaoData.id == chaoId)
					{
						result = ((chaoData.abilityNum - 1 <= chaoData.currentAbility) ? chaoData.SetChaoAbilityIndex(0) : chaoData.SetChaoAbilityIndex(chaoData.currentAbility + 1));
						break;
					}
				}
			}
			return result;
		}

		public static ChaoData[] GetDataTable()
		{
			Setup();
			return s_chaoDataTable;
		}

		public static List<ChaoData> GetDataTable(ChaoData.Rarity rarity)
		{
			List<ChaoData> result = null;
			if (rarity != ChaoData.Rarity.NONE)
			{
				Setup();
				ChaoDataVisitorBase visitor = new ChaoDataVisitorRarity();
				if (s_chaoDataTable != null)
				{
					ChaoData[] array = s_chaoDataTable;
					foreach (ChaoData chaoData in array)
					{
						chaoData.accept(ref visitor);
					}
				}
				ChaoDataVisitorRarity chaoDataVisitorRarity = (ChaoDataVisitorRarity)visitor;
				result = chaoDataVisitorRarity.GetChaoList(rarity);
			}
			return result;
		}

		public static List<ChaoData> GetPossessionChaoData()
		{
			List<ChaoData> list = null;
			if (s_chaoDataTable == null)
			{
				Setup();
			}
			if (s_chaoDataTable != null && s_chaoDataTable.Length > 0)
			{
				int num = s_chaoDataTable.Length;
				for (int i = 0; i < num; i++)
				{
					ChaoData chaoData = s_chaoDataTable[i];
					if (chaoData != null && chaoData.level >= 0)
					{
						if (list == null)
						{
							list = new List<ChaoData>();
						}
						list.Add(chaoData);
					}
				}
			}
			return list;
		}

		public static List<ChaoData> GetChaoData(List<int> ids)
		{
			List<ChaoData> list = null;
			if (ids != null && ids.Count > 0)
			{
				Setup();
				int count = ids.Count;
				for (int i = 0; i < count; i++)
				{
					int id = ids[i];
					ChaoData chaoData = GetChaoData(id);
					if (chaoData != null)
					{
						if (list == null)
						{
							list = new List<ChaoData>();
						}
						list.Add(chaoData);
					}
				}
			}
			return list;
		}

		public static ChaoData GetChaoData(int id)
		{
			ChaoData result = null;
			Setup();
			if (id >= 0 && s_chaoDataTable != null && s_chaoDataTableMarker != null)
			{
				int num = s_chaoDataTable.Length;
				bool flag = false;
				int num2 = 0;
				int num3 = id / 1000 % 10;
				int num4 = 0;
				if (s_chaoDataTableMarker.ContainsKey(num3))
				{
					int num5 = id % 10;
					if (num5 >= 5)
					{
						flag = true;
					}
					int num6 = id / 10 % 100;
					if (s_chaoDataTableMarker[num3].Count > num6)
					{
						num2 = ((flag && s_chaoDataTableMarker[num3].Count - 1 == num6) ? (s_chaoDataTableMarker.ContainsKey(num3 + 1) ? (s_chaoDataTableMarker[num3 + 1][0] - 1) : (num - 1)) : ((!flag) ? s_chaoDataTableMarker[num3][num6] : (s_chaoDataTableMarker[num3][num6 + 1] - 1)));
					}
					if (num2 < 0)
					{
						num2 = 0;
					}
				}
				for (int i = 0; i < num; i++)
				{
					num4 = (flag ? ((num2 - i + num) % num) : ((num2 + i) % num));
					if (s_chaoDataTable[num4].id == id)
					{
						result = s_chaoDataTable[num4];
						break;
					}
				}
			}
			return result;
		}

		private static void ResetLoadingChao()
		{
			s_loadingChaoList = null;
			s_loadingCount = 0;
			s_eventList = false;
		}

		public static void CheckEventTime()
		{
			if (s_eventList)
			{
				if (EventManager.Instance != null && !EventManager.Instance.IsInEvent())
				{
					ResetLoadingChao();
				}
			}
			else if (EventManager.Instance != null && EventManager.Instance.IsInEvent())
			{
				ResetLoadingChao();
			}
		}

		public static List<ChaoData> GetEyeCatcherChaoData(List<ServerChaoState> serverChaoList)
		{
			if (serverChaoList != null)
			{
				List<int> list = new List<int>();
				EyeCatcherChaoData[] eyeCatcherChaoDatas = EventManager.Instance.GetEyeCatcherChaoDatas();
				if (eyeCatcherChaoDatas != null)
				{
					EyeCatcherChaoData[] array = eyeCatcherChaoDatas;
					foreach (EyeCatcherChaoData eyeCatcherChaoData in array)
					{
						foreach (ServerChaoState serverChao in serverChaoList)
						{
							int num = eyeCatcherChaoData.chao_id + 400000;
							if (num == serverChao.Id)
							{
								list.Add(eyeCatcherChaoData.chao_id);
								break;
							}
						}
					}
				}
				RewardChaoData rewardChaoData = EventManager.Instance.GetRewardChaoData();
				if (rewardChaoData != null)
				{
					foreach (ServerChaoState serverChao2 in serverChaoList)
					{
						int num2 = rewardChaoData.chao_id + 400000;
						if (num2 == serverChao2.Id)
						{
							list.Add(rewardChaoData.chao_id);
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					return GetChaoData(list);
				}
			}
			return null;
		}

		public static ChaoData GetLoadingChao()
		{
			Setup();
			if (ServerInterface.LoggedInServerInterface == null && s_chaoDataTable != null)
			{
				ChaoData[] array = s_chaoDataTable;
				foreach (ChaoData chaoData in array)
				{
					if (chaoData.id == 0)
					{
						return chaoData;
					}
				}
				return null;
			}
			CheckEventTime();
			if (s_loadingChaoList == null && s_chaoDataTable != null)
			{
				List<ChaoData> list = null;
				List<ServerChaoState> list2 = null;
				ServerPlayerState playerState = ServerInterface.PlayerState;
				if (playerState != null && playerState.ChaoStates != null)
				{
					list2 = playerState.ChaoStates;
				}
				if (EventManager.Instance != null && EventManager.Instance.IsInEvent())
				{
					ChaoDataVisitorBase visitor = new ChaoDataVisitorEvent();
					ChaoData[] array2 = s_chaoDataTable;
					foreach (ChaoData chaoData2 in array2)
					{
						if (list2 != null)
						{
							foreach (ServerChaoState item in list2)
							{
								int num = chaoData2.id + 400000;
								if (item.Id == num)
								{
									chaoData2.accept(ref visitor);
									break;
								}
							}
						}
						else
						{
							chaoData2.accept(ref visitor);
						}
					}
					ChaoDataVisitorEvent chaoDataVisitorEvent = (ChaoDataVisitorEvent)visitor;
					if (chaoDataVisitorEvent != null)
					{
						list = chaoDataVisitorEvent.GetChaoList(EventManager.GetSpecificId());
						if (list == null)
						{
							list = GetEyeCatcherChaoData(list2);
						}
					}
					s_eventList = true;
				}
				if (list == null)
				{
					ChaoDataVisitorBase visitor2 = new ChaoDataVisitorRarity();
					ChaoData[] array3 = s_chaoDataTable;
					foreach (ChaoData chaoData3 in array3)
					{
						if (list2 != null)
						{
							foreach (ServerChaoState item2 in list2)
							{
								int num2 = chaoData3.id + 400000;
								if (item2.Id == num2)
								{
									chaoData3.accept(ref visitor2);
									break;
								}
							}
						}
						else
						{
							chaoData3.accept(ref visitor2);
						}
					}
					ChaoDataVisitorRarity chaoDataVisitorRarity = (ChaoDataVisitorRarity)visitor2;
					if (chaoDataVisitorRarity != null)
					{
						list = chaoDataVisitorRarity.GetChaoList(ChaoData.Rarity.SRARE, ChaoData.Rarity.RARE);
					}
				}
				if (list != null)
				{
					s_loadingChaoList = list.OrderBy((ChaoData i) => Guid.NewGuid()).ToList();
				}
			}
			if (s_loadingChaoList == null || s_loadingChaoList.Count <= 0)
			{
				return null;
			}
			int index = s_loadingCount % s_loadingChaoList.Count;
			s_loadingCount++;
			return s_loadingChaoList[index];
		}

		public static ChaoData GetChaoDataOfIndex(int index)
		{
			ChaoData[] dataTable = GetDataTable();
			return (dataTable == null || (uint)index >= dataTable.Length) ? null : dataTable[index];
		}

		public static int GetChaoIdOfIndex(int index)
		{
			ChaoData chaoDataOfIndex = GetChaoDataOfIndex(index);
			return (chaoDataOfIndex == null) ? (-1) : chaoDataOfIndex.id;
		}

		public static int GetRandomChaoId()
		{
			ChaoData[] dataTable = GetDataTable();
			return (dataTable == null) ? (-1) : dataTable[UnityEngine.Random.Range(0, dataTable.Length)].id;
		}
	}
}
