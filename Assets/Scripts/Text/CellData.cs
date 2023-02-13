namespace Text
{
	internal class CellData
	{
		public string m_cellID
		{
			get;
			private set;
		}

		public string m_cellString
		{
			get;
			private set;
		}

		public CellData(string cellID, string cellString)
		{
			m_cellID = cellID;
			m_cellString = cellString;
		}
	}
}
