using System.Globalization;
using System.Xml;
using UnityEngine;

public class EventCommonDataTable : MonoBehaviour
{
	public const int ITEM_COUNT_MAX = 8;

	public const int TBL_COUNT_MAX = 1;

	[SerializeField]
	private TextAsset m_xml_data;

	private static readonly string[] ITEM_NAMES = new string[8]
	{
		"SeFileName",
		"MenuBgmFileName",
		"Roulette_BgmName",
		"RouletteS_BgmName",
		"EventTop_BgmName",
		"RouletteDecide_SeCueName",
		"RouletteChange_SeCueName",
		"RouletteChao_Number"
	};

	private string[] m_tblInfo;

	private int m_tblCount;

	private static EventCommonDataTable s_instance = null;

	public static EventCommonDataTable Instance
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
			s_instance = null;
		}
	}

	private void Start()
	{
		Setup();
	}

	public void Setup()
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new string[8];
		}
		if ((bool)m_xml_data)
		{
			string xml = AESCrypt.Decrypt(m_xml_data.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, m_tblInfo, out m_tblCount);
			if (m_tblCount != 0)
			{
			}
		}
	}

	public static void CreateTable(XmlDocument doc, string[] data, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("EventCommonDataTable");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (XmlNode item in xmlNodeList)
		{
			XmlNodeList xmlNodeList2 = item.SelectNodes("Item");
			foreach (XmlNode item2 in xmlNodeList2)
			{
				for (int i = 0; i < 8; i++)
				{
					string itemName = GetItemName((uint)i);
					XmlAttribute xmlAttribute = item2.Attributes[itemName];
					string text = string.Empty;
					if (xmlAttribute != null)
					{
						text = item2.Attributes[itemName].Value;
					}
					int num2 = num * 8 + i;
					data[num2] = text;
				}
			}
			num++;
		}
		tbl_count = num;
	}

	public string GetData(EventCommonDataItem item_index)
	{
		int num = 0;
		if (m_tblInfo != null && (uint)num < m_tblCount)
		{
			int num2 = (int)(num * 8 + item_index);
			if (num2 < m_tblInfo.Length)
			{
				return m_tblInfo[num2];
			}
		}
		return string.Empty;
	}

	public bool IsRouletteEventChao(int chaoId)
	{
		string data = GetData(EventCommonDataItem.RouletteChao_Number);
		if (data != null && data != string.Empty)
		{
			string[] array = data.Split(',');
			string[] array2 = array;
			foreach (string s in array2)
			{
				int num = int.Parse(s, NumberStyles.AllowLeadingSign);
				if (num == chaoId)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static string GetItemName(uint index)
	{
		if (index < 8 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public static void LoadSetup()
	{
		GameObject gameObject = GameObject.Find("EventResourceCommon");
		if (!(gameObject != null))
		{
			return;
		}
		for (int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
			if (gameObject2.name == "EventCommonDataTable" && !gameObject2.activeSelf)
			{
				gameObject2.SetActive(true);
			}
		}
	}
}
