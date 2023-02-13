using System.Collections.Generic;

namespace SaveData
{
	public class InformationData
	{
		public enum DataType
		{
			ID,
			SHOWED_TIME,
			ADD_INFO,
			IMAGE_ID,
			NUM
		}

		public const string RESET_PARAME = "-1,-1,-1,-1";

		private List<string> m_textArray = new List<string>();

		public bool m_isDirty;

		public static string INVALID_PARAM = "-1";

		public List<string> TextArray
		{
			get
			{
				return m_textArray;
			}
			set
			{
				m_textArray = value;
			}
		}

		public InformationData()
		{
			Init();
		}

		private bool CheckData(DataType dataType)
		{
			return DataType.ID <= dataType && dataType < DataType.NUM;
		}

		public string GetData(string id, DataType dataType)
		{
			if (m_textArray != null && CheckData(dataType))
			{
				for (int i = 0; i < m_textArray.Count; i++)
				{
					string[] array = m_textArray[i].Split(',');
					if (array.Length > (int)dataType && array[0] == id)
					{
						return array[(int)dataType];
					}
				}
			}
			return INVALID_PARAM;
		}

		public string GetData(int index, DataType dataType)
		{
			if (m_textArray != null && CheckData(dataType) && index < m_textArray.Count)
			{
				string[] array = m_textArray[index].Split(',');
				if (array.Length > (int)dataType)
				{
					return array[(int)dataType];
				}
			}
			return INVALID_PARAM;
		}

		public string GetEventRankingData(string id, string saveKey, DataType dataType)
		{
			if (m_textArray != null && CheckData(dataType))
			{
				for (int i = 0; i < m_textArray.Count; i++)
				{
					string[] array = m_textArray[i].Split(',');
					if (array.Length > (int)dataType && array[0] == id && array[2] == saveKey)
					{
						return array[(int)dataType];
					}
				}
			}
			return INVALID_PARAM;
		}

		public void UpdateShowedTime(string id, string showedTime, string addInfo, string imageId)
		{
			if (m_textArray == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < m_textArray.Count; i++)
			{
				string[] array = m_textArray[i].Split(',');
				if (array.Length > 0 && array[0] == id)
				{
					m_textArray[i] = id + "," + showedTime + "," + addInfo + "," + imageId;
					flag = true;
					m_isDirty = true;
					break;
				}
			}
			if (!flag)
			{
				m_textArray.Add(id + "," + showedTime + "," + addInfo + "," + imageId);
				m_isDirty = true;
			}
		}

		public void UpdateEventRankingShowedTime(string id, string showedTime, string addInfo, string imageId)
		{
			if (m_textArray == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < m_textArray.Count; i++)
			{
				string[] array = m_textArray[i].Split(',');
				if (array.Length > 2 && array[0] == id && array[2] == addInfo)
				{
					m_textArray[i] = id + "," + showedTime + "," + addInfo + "," + imageId;
					flag = true;
					m_isDirty = true;
					break;
				}
			}
			if (!flag)
			{
				m_textArray.Add(id + "," + showedTime + "," + addInfo + "," + imageId);
				m_isDirty = true;
			}
		}

		public int DataCount()
		{
			if (m_textArray != null)
			{
				return m_textArray.Count;
			}
			return 0;
		}

		public void Reset(int index)
		{
			if (m_textArray != null && index < m_textArray.Count)
			{
				m_textArray.RemoveAt(index);
				m_isDirty = true;
			}
		}

		public void Init()
		{
			if (m_textArray != null)
			{
				for (int i = 0; i < m_textArray.Count; i++)
				{
					m_textArray[i] = "-1,-1,-1,-1";
				}
			}
			m_isDirty = false;
		}

		public void CopyTo(InformationData dst)
		{
			dst.TextArray = m_textArray;
			dst.m_isDirty = m_isDirty;
		}
	}
}
