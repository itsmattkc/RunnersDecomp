using System.IO;
using System.Xml.Serialization;
using Text;
using UnityEngine;

namespace DataTable
{
	public class MissionTable : MonoBehaviour
	{
		[SerializeField]
		private TextAsset m_missionTabel;

		private static MissionData[] m_missionDataTable;

		private static MissionTable s_instance;

		public static MissionTable Instance
		{
			get
			{
				return s_instance;
			}
		}

		private void Awake()
		{
			if (s_instance == null)
			{
				Object.DontDestroyOnLoad(base.gameObject);
				s_instance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (s_instance == this)
			{
				m_missionDataTable = null;
				s_instance = null;
			}
		}

		private void Start()
		{
			if (m_missionDataTable == null)
			{
				string s = AESCrypt.Decrypt(m_missionTabel.text);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(MissionData[]));
				StringReader textReader = new StringReader(s);
				m_missionDataTable = (MissionData[])xmlSerializer.Deserialize(textReader);
			}
		}

		public static MissionData[] GetDataTable()
		{
			return m_missionDataTable;
		}

		public static MissionData GetMissionData(int id)
		{
			if (GetDataTable() == null)
			{
				return null;
			}
			MissionData[] dataTable = GetDataTable();
			foreach (MissionData missionData in dataTable)
			{
				if (missionData.id == id)
				{
					return missionData;
				}
			}
			return null;
		}

		public static MissionData GetMissionDataOfIndex(int index)
		{
			return m_missionDataTable[index];
		}

		public static void LoadSetup()
		{
			GameObject gameObject = GameObject.Find("MissionTable");
			if (!(gameObject != null))
			{
				return;
			}
			MissionData[] dataTable = GetDataTable();
			if (dataTable != null)
			{
				MissionData[] array = dataTable;
				foreach (MissionData missionData in array)
				{
					int type = (int)missionData.type;
					TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "caption" + type);
					if (text != null)
					{
						missionData.SetText(text.text);
					}
				}
			}
			if (gameObject.transform.parent != null && gameObject.transform.parent.name == "ETC")
			{
				gameObject.transform.parent = null;
			}
		}
	}
}
