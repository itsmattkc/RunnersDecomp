using System.Collections.Generic;

namespace DataTable
{
	public class ChaoDataVisitorEvent : ChaoDataVisitorBase, IChaoDataSorting
	{
		private Dictionary<int, List<ChaoData>> m_chaoList;

		public ChaoDataVisitorEvent()
		{
			m_chaoList = new Dictionary<int, List<ChaoData>>();
		}

		public override void visit(ChaoData chaoData)
		{
			int specificId = EventManager.GetSpecificId();
			if (m_chaoList == null)
			{
				return;
			}
			List<int> eventIdList = chaoData.eventIdList;
			int num = 0;
			if (eventIdList == null || eventIdList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < eventIdList.Count; i++)
			{
				if (num < eventIdList[i])
				{
					num = eventIdList[i];
				}
				if (specificId > 0)
				{
					int specificId2 = EventManager.GetSpecificId(eventIdList[i]);
					if (specificId2 == specificId)
					{
						num = specificId2;
						break;
					}
				}
			}
			if (!m_chaoList.ContainsKey(num))
			{
				m_chaoList.Add(num, new List<ChaoData>());
			}
			m_chaoList[num].Add(chaoData);
		}

		public List<ChaoData> GetChaoList(int specificId)
		{
			List<ChaoData> result = null;
			if (m_chaoList.ContainsKey(specificId))
			{
				result = m_chaoList[specificId];
			}
			return result;
		}

		public List<ChaoData> GetChaoListAll(bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			return GetChaoListAllOffset(0, descending, exclusion);
		}

		public List<ChaoData> GetChaoListAllOffset(int offset, bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			if (m_chaoList == null)
			{
				return null;
			}
			if (m_chaoList.Count < 1)
			{
				return null;
			}
			List<ChaoData> list = null;
			Dictionary<int, List<ChaoData>>.KeyCollection keys = m_chaoList.Keys;
			int id = EventManager.Instance.Id;
			int num = 0;
			num = ((id > 0) ? (id / 100000 % 10) : (-1));
			Dictionary<int, List<ChaoData>> dictionary = new Dictionary<int, List<ChaoData>>();
			Dictionary<int, List<ChaoData>> dictionary2 = new Dictionary<int, List<ChaoData>>();
			Dictionary<int, List<ChaoData>> dictionary3 = new Dictionary<int, List<ChaoData>>();
			Dictionary<int, List<ChaoData>> dictionary4 = new Dictionary<int, List<ChaoData>>();
			Dictionary<int, List<ChaoData>> dictionary5 = new Dictionary<int, List<ChaoData>>();
			foreach (int item in keys)
			{
				if (item == id && id > 0)
				{
					dictionary.Add(item, m_chaoList[item]);
					continue;
				}
				switch (item / 100000000 % 10)
				{
				case 0:
					dictionary2.Add(item, m_chaoList[item]);
					break;
				case 1:
					dictionary3.Add(item, m_chaoList[item]);
					break;
				case 2:
					dictionary4.Add(item, m_chaoList[item]);
					break;
				case 3:
					dictionary5.Add(item, m_chaoList[item]);
					break;
				default:
					dictionary5.Add(item, m_chaoList[item]);
					break;
				}
			}
			ChaoDataVisitorUtility.SortKeyDictionaryInt(ref dictionary2, true);
			ChaoDataVisitorUtility.SortKeyDictionaryInt(ref dictionary3, true);
			ChaoDataVisitorUtility.SortKeyDictionaryInt(ref dictionary4, true);
			ChaoDataVisitorUtility.SortKeyDictionaryInt(ref dictionary5, true);
			switch (num)
			{
			case 1:
				if (dictionary != null && dictionary.Count > 0)
				{
					ChaoDataVisitorUtility.AddListInt(ref list, dictionary, true);
				}
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary3, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary4, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary5, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary2, true);
				break;
			case 2:
				if (dictionary != null && dictionary.Count > 0)
				{
					ChaoDataVisitorUtility.AddListInt(ref list, dictionary, true);
				}
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary4, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary5, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary3, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary2, true);
				break;
			case 3:
				if (dictionary != null && dictionary.Count > 0)
				{
					ChaoDataVisitorUtility.AddListInt(ref list, dictionary, true);
				}
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary5, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary3, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary4, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary2, true);
				break;
			default:
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary3, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary4, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary5, true);
				ChaoDataVisitorUtility.AddListInt(ref list, dictionary2, true);
				break;
			}
			if (descending && list != null)
			{
				list.Reverse();
			}
			if (exclusion != ChaoData.Rarity.NONE && list != null)
			{
				List<ChaoData> list2 = new List<ChaoData>();
				List<ChaoData> list3 = new List<ChaoData>();
				foreach (ChaoData item2 in list)
				{
					if (item2.rarity != exclusion)
					{
						list2.Add(item2);
					}
					else
					{
						list3.Add(item2);
					}
				}
				list = list2;
				list.AddRange(list3);
			}
			return list;
		}
	}
}
