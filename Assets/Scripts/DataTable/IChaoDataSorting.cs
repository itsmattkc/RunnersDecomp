using System.Collections.Generic;

namespace DataTable
{
	internal interface IChaoDataSorting
	{
		List<ChaoData> GetChaoListAll(bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE);

		List<ChaoData> GetChaoListAllOffset(int offset, bool descending = false, ChaoData.Rarity exclusion = ChaoData.Rarity.NONE);
	}
}
