using System.Collections.Generic;

namespace DataTable
{
	public class ChaoDataVisitorAbility : ChaoDataVisitorBase, IChaoDataSorting
	{
		private Dictionary<ChaoAbility, List<ChaoData>> m_chaoList;

		public ChaoDataVisitorAbility()
		{
			m_chaoList = new Dictionary<ChaoAbility, List<ChaoData>>();
			int num = 94;
			for (int i = 0; i < num; i++)
			{
				m_chaoList.Add((ChaoAbility)i, new List<ChaoData>());
			}
		}

		public override void visit(ChaoData chaoData)
		{
			if (m_chaoList != null && chaoData.chaoAbility >= ChaoAbility.ALL_BONUS_COUNT && chaoData.chaoAbility < ChaoAbility.NUM)
			{
				m_chaoList[chaoData.chaoAbility].Add(chaoData);
			}
		}

		public List<ChaoData> GetChaoList(ChaoAbility ability)
		{
			List<ChaoData> result = null;
			if (ability >= ChaoAbility.ALL_BONUS_COUNT && ability < ChaoAbility.NUM)
			{
				result = m_chaoList[ability];
			}
			return result;
		}

		public List<ChaoData> GetChaoListAll(bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			return GetChaoListAllOffset(0, descending, exclusion);
		}

		public List<ChaoData> GetChaoListAllOffset(int offset, bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			List<ChaoData> list = null;
			ChaoAbility chaoAbility = ChaoAbility.ALL_BONUS_COUNT;
			int num = 94;
			for (int i = 0; i <= num; i++)
			{
				chaoAbility = (ChaoAbility)((i + offset) % num);
				if (m_chaoList[chaoAbility].Count > 0)
				{
					m_chaoList[chaoAbility].Reverse();
					if (list == null)
					{
						list = m_chaoList[chaoAbility];
					}
					else
					{
						list.AddRange(m_chaoList[chaoAbility]);
					}
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
