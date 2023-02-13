using System.Collections.Generic;
using System.Linq;

namespace DataTable
{
	public class ChaoDataVisitorUtility
	{
		public static void SortKeyDictionaryInt(ref Dictionary<int, List<ChaoData>> dictionary, bool descending = false)
		{
			Dictionary<int, List<ChaoData>> dictionary2 = new Dictionary<int, List<ChaoData>>();
			IOrderedEnumerable<KeyValuePair<int, List<ChaoData>>> orderedEnumerable = (!descending) ? dictionary.OrderBy((KeyValuePair<int, List<ChaoData>> x) => x.Key) : dictionary.OrderByDescending((KeyValuePair<int, List<ChaoData>> x) => x.Key);
			foreach (KeyValuePair<int, List<ChaoData>> item in orderedEnumerable)
			{
				dictionary2.Add(item.Key, item.Value);
			}
			dictionary = dictionary2;
		}

		public static void AddListInt(ref List<ChaoData> list, Dictionary<int, List<ChaoData>> dictionary, bool raritySort = false, bool descending = false)
		{
			if (dictionary == null || dictionary.Count <= 0)
			{
				return;
			}
			Dictionary<int, List<ChaoData>>.KeyCollection keys = dictionary.Keys;
			if (raritySort)
			{
				foreach (int item in keys)
				{
					if (dictionary[item] == null || dictionary[item].Count <= 0)
					{
						continue;
					}
					if (!descending)
					{
						dictionary[item].Sort((ChaoData chaoA, ChaoData chaoB) => chaoB.rarity - chaoA.rarity);
					}
					else
					{
						dictionary[item].Sort((ChaoData chaoA, ChaoData chaoB) => chaoA.rarity - chaoB.rarity);
					}
				}
			}
			foreach (int item2 in keys)
			{
				if (list == null)
				{
					list = dictionary[item2];
				}
				else
				{
					list.AddRange(dictionary[item2]);
				}
			}
		}
	}
}
