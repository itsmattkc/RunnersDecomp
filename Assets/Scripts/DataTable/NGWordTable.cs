using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace DataTable
{
	public class NGWordTable : MonoBehaviour
	{
		[SerializeField]
		private TextAsset m_ngWordTabel;

		private static NGWordData[] m_ngWordDataTable;

		private void Start()
		{
			if (m_ngWordDataTable == null)
			{
				TimeProfiler.StartCountTime("NGWordTable::Start");
				TimeProfiler.StartCountTime("NGWordTable::AESCrypt.Decrypt");
				string s = AESCrypt.Decrypt(m_ngWordTabel.text);
				TimeProfiler.EndCountTime("NGWordTable::AESCrypt.Decrypt");
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(NGWordData[]));
				StringReader textReader = new StringReader(s);
				m_ngWordDataTable = (NGWordData[])xmlSerializer.Deserialize(textReader);
				TimeProfiler.EndCountTime("NGWordTable::Start");
			}
		}

		private void OnDestroy()
		{
			m_ngWordDataTable = null;
		}

		public static NGWordData[] GetDataTable()
		{
			return m_ngWordDataTable;
		}

		public static int GetDataTableCount()
		{
			if (m_ngWordDataTable != null)
			{
				return m_ngWordDataTable.Length;
			}
			return 0;
		}
	}
}
