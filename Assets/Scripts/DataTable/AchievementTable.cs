using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace DataTable
{
	public class AchievementTable : MonoBehaviour
	{
		[SerializeField]
		private TextAsset m_achievementTabel;

		private static AchievementData[] m_achievementDataTable;

		private void Start()
		{
			if (m_achievementDataTable == null)
			{
				string s = AESCrypt.Decrypt(m_achievementTabel.text);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(AchievementData[]));
				StringReader textReader = new StringReader(s);
				m_achievementDataTable = (AchievementData[])xmlSerializer.Deserialize(textReader);
			}
		}

		private void OnDestroy()
		{
			m_achievementDataTable = null;
		}

		public static AchievementData[] GetDataTable()
		{
			return m_achievementDataTable;
		}

		public static int GetDataTableCount()
		{
			if (m_achievementDataTable != null)
			{
				return m_achievementDataTable.Length;
			}
			return 0;
		}

		public static AchievementData GetAchievementData(string id)
		{
			if (GetDataTable() == null)
			{
				return null;
			}
			AchievementData[] dataTable = GetDataTable();
			foreach (AchievementData achievementData in dataTable)
			{
				if (achievementData.GetID() == id)
				{
					return achievementData;
				}
			}
			return null;
		}

		public static AchievementData GetAchievementDataOfIndex(int index)
		{
			if (GetDataTable() == null)
			{
				return null;
			}
			if ((uint)index < (uint)m_achievementDataTable.Length)
			{
				return m_achievementDataTable[index];
			}
			return null;
		}
	}
}
