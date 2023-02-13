using System.Collections.Generic;

namespace Text
{
	internal class CellDataList
	{
		private Dictionary<string, CellData> m_cellDataList;

		public string m_categoryName
		{
			get;
			private set;
		}

		public CellDataList(string categoryName)
		{
			m_categoryName = categoryName;
			m_cellDataList = new Dictionary<string, CellData>();
		}

		public void Add(CellData cellData)
		{
			if (cellData != null)
			{
				if (m_cellDataList.ContainsKey(cellData.m_cellID))
				{
					Debug.LogWarning("CellDataList.Add() same cellID = " + cellData.m_cellID);
				}
				else
				{
					m_cellDataList.Add(cellData.m_cellID, cellData);
				}
			}
		}

		public CellData Get(string searchId)
		{
			if (!m_cellDataList.ContainsKey(searchId))
			{
				return null;
			}
			return m_cellDataList[searchId];
		}

		public int GetCount()
		{
			return m_cellDataList.Count;
		}
	}
}
