using System.Collections.Generic;

namespace DataTable
{
	public class ChaoDataSorting
	{
		private ChaoDataVisitorBase m_visitor;

		private IChaoDataSorting m_chaoSorting;

		public ChaoDataVisitorBase visitor
		{
			get
			{
				return m_visitor;
			}
		}

		public ChaoDataSorting(ChaoSort sort)
		{
			switch (sort)
			{
			case ChaoSort.RARE:
				m_visitor = new ChaoDataVisitorRarity();
				break;
			case ChaoSort.LEVEL:
				m_visitor = new ChaoDataVisitorLevel();
				break;
			case ChaoSort.ATTRIBUTE:
				m_visitor = new ChaoDataVisitorAttribute();
				break;
			case ChaoSort.ABILITY:
				m_visitor = new ChaoDataVisitorAbility();
				break;
			case ChaoSort.EVENT:
				m_visitor = new ChaoDataVisitorEvent();
				break;
			}
			if (m_visitor != null)
			{
				m_chaoSorting = (IChaoDataSorting)m_visitor;
			}
		}

		public List<ChaoData> GetChaoList(bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			List<ChaoData> result = null;
			if (m_chaoSorting != null)
			{
				List<ChaoData> chaoListAll = m_chaoSorting.GetChaoListAll(descending, exclusion);
				if (chaoListAll != null)
				{
					result = new List<ChaoData>();
					{
						foreach (ChaoData item in chaoListAll)
						{
							if (item.level >= 0)
							{
								result.Add(item);
							}
						}
						return result;
					}
				}
			}
			return result;
		}

		public List<ChaoData> GetChaoListAll(bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			List<ChaoData> result = null;
			if (m_chaoSorting != null)
			{
				result = m_chaoSorting.GetChaoListAll(descending, exclusion);
			}
			return result;
		}

		public List<ChaoData> GetChaoListAllOffset(int offset, bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE)
		{
			List<ChaoData> result = null;
			if (m_chaoSorting != null)
			{
				result = m_chaoSorting.GetChaoListAllOffset(offset, descending, exclusion);
			}
			return result;
		}
	}
}
