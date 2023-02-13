using System.Collections.Generic;

namespace DataTable
{
	public class ChaoDataVisitorRarity : ChaoDataVisitorBase, IChaoDataSorting
	{
		private Dictionary<ChaoData.Rarity, List<ChaoData>> m_chaoRarityList;

		public ChaoDataVisitorRarity()
		{
			m_chaoRarityList = new Dictionary<ChaoData.Rarity, List<ChaoData>>();
			List<ChaoData> value = new List<ChaoData>();
			List<ChaoData> value2 = new List<ChaoData>();
			List<ChaoData> value3 = new List<ChaoData>();
			m_chaoRarityList.Add(ChaoData.Rarity.NORMAL, value);
			m_chaoRarityList.Add(ChaoData.Rarity.RARE, value2);
			m_chaoRarityList.Add(ChaoData.Rarity.SRARE, value3);
		}

		public override void visit(ChaoData chaoData)
		{
			if (m_chaoRarityList != null)
			{
				switch (chaoData.rarity)
				{
				case ChaoData.Rarity.NORMAL:
					m_chaoRarityList[ChaoData.Rarity.NORMAL].Add(chaoData);
					break;
				case ChaoData.Rarity.RARE:
					m_chaoRarityList[ChaoData.Rarity.RARE].Add(chaoData);
					break;
				case ChaoData.Rarity.SRARE:
					m_chaoRarityList[ChaoData.Rarity.SRARE].Add(chaoData);
					break;
				}
			}
		}

		public List<ChaoData> GetChaoList(ChaoData.Rarity rarity, ChaoData.Rarity raritySub = ChaoData.Rarity.NONE)
		{
			List<ChaoData> list = null;
			switch (rarity)
			{
			case ChaoData.Rarity.NORMAL:
				list = m_chaoRarityList[ChaoData.Rarity.NORMAL];
				break;
			case ChaoData.Rarity.RARE:
				list = m_chaoRarityList[ChaoData.Rarity.RARE];
				break;
			case ChaoData.Rarity.SRARE:
				list = m_chaoRarityList[ChaoData.Rarity.SRARE];
				break;
			}
			if (raritySub != ChaoData.Rarity.NONE && rarity != raritySub && list != null)
			{
				switch (raritySub)
				{
				case ChaoData.Rarity.NORMAL:
					list.AddRange(m_chaoRarityList[ChaoData.Rarity.NORMAL]);
					break;
				case ChaoData.Rarity.RARE:
					list.AddRange(m_chaoRarityList[ChaoData.Rarity.RARE]);
					break;
				case ChaoData.Rarity.SRARE:
					list.AddRange(m_chaoRarityList[ChaoData.Rarity.SRARE]);
					break;
				}
			}
			return list;
		}

		public List<ChaoData> GetChaoListAll(bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			return GetChaoListAllOffset(0, descending, exclusion);
		}

		public List<ChaoData> GetChaoListAllOffset(int offset, bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			List<ChaoData> list = null;
			List<ChaoData> chaoList = GetChaoList(ChaoData.Rarity.NORMAL);
			List<ChaoData> chaoList2 = GetChaoList(ChaoData.Rarity.RARE);
			List<ChaoData> chaoList3 = GetChaoList(ChaoData.Rarity.SRARE);
			if (chaoList != null && list == null)
			{
				list = chaoList;
			}
			if (chaoList2 != null)
			{
				if (list == null)
				{
					list = chaoList2;
				}
				else
				{
					list.AddRange(chaoList2);
				}
			}
			if (chaoList3 != null)
			{
				if (list == null)
				{
					list = chaoList3;
				}
				else
				{
					list.AddRange(chaoList3);
				}
			}
			if (descending && list != null)
			{
				list.Reverse();
			}
			if (exclusion != ChaoData.Rarity.NONE && list != null)
			{
				List<ChaoData> list2 = new List<ChaoData>();
				List<ChaoData> list3 = new List<ChaoData>();
				foreach (ChaoData item in list)
				{
					if (item.rarity != exclusion)
					{
						list2.Add(item);
					}
					else
					{
						list3.Add(item);
					}
				}
				list = list2;
				list.AddRange(list3);
			}
			return list;
		}
	}
}
