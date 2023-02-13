using System.Collections.Generic;

namespace DataTable
{
	public class ChaoDataVisitorAttribute : ChaoDataVisitorBase, IChaoDataSorting
	{
		private Dictionary<CharacterAttribute, List<ChaoData>> m_chaoList;

		public ChaoDataVisitorAttribute()
		{
			m_chaoList = new Dictionary<CharacterAttribute, List<ChaoData>>();
			List<ChaoData> value = new List<ChaoData>();
			List<ChaoData> value2 = new List<ChaoData>();
			List<ChaoData> value3 = new List<ChaoData>();
			m_chaoList.Add(CharacterAttribute.SPEED, value);
			m_chaoList.Add(CharacterAttribute.FLY, value2);
			m_chaoList.Add(CharacterAttribute.POWER, value3);
		}

		public override void visit(ChaoData chaoData)
		{
			if (m_chaoList != null)
			{
				switch (chaoData.charaAtribute)
				{
				case CharacterAttribute.SPEED:
					m_chaoList[CharacterAttribute.SPEED].Add(chaoData);
					break;
				case CharacterAttribute.FLY:
					m_chaoList[CharacterAttribute.FLY].Add(chaoData);
					break;
				case CharacterAttribute.POWER:
					m_chaoList[CharacterAttribute.POWER].Add(chaoData);
					break;
				}
			}
		}

		public List<ChaoData> GetChaoList(CharacterAttribute attribute)
		{
			List<ChaoData> result = null;
			switch (attribute)
			{
			case CharacterAttribute.SPEED:
				result = m_chaoList[CharacterAttribute.SPEED];
				break;
			case CharacterAttribute.FLY:
				result = m_chaoList[CharacterAttribute.FLY];
				break;
			case CharacterAttribute.POWER:
				result = m_chaoList[CharacterAttribute.POWER];
				break;
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
			Dictionary<int, List<ChaoData>> dictionary = new Dictionary<int, List<ChaoData>>();
			dictionary.Add(0, GetChaoList(CharacterAttribute.SPEED));
			dictionary.Add(1, GetChaoList(CharacterAttribute.FLY));
			dictionary.Add(2, GetChaoList(CharacterAttribute.POWER));
			int count = dictionary.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				num = (i + offset) % count;
				if (dictionary[num] != null)
				{
					dictionary[num].Reverse();
					if (list == null)
					{
						list = dictionary[num];
					}
					else
					{
						list.AddRange(dictionary[num]);
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
