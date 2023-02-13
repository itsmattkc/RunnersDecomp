using System;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class EventObjectTable : MonoBehaviour
{
	public const int ITEM_COUNT_MAX = 8;

	public const int TBL_COUNT_MAX = 3;

	[SerializeField]
	private TextAsset m_dataTabel;

	private static readonly EventObjectTableItem[] CRYSTAL_ITEMS = new EventObjectTableItem[6]
	{
		EventObjectTableItem.CrystalA,
		EventObjectTableItem.CrystalB,
		EventObjectTableItem.CrystalC,
		EventObjectTableItem.Crystal10A,
		EventObjectTableItem.Crystal10B,
		EventObjectTableItem.Crystal10C
	};

	private static readonly string[] ITEM_NAMES = new string[8]
	{
		"ObjRing",
		"ObjSuperRing",
		"ObjCrystal_A",
		"ObjCrystal_B",
		"ObjCrystal_C",
		"ObjCrystal10_A",
		"ObjCrystal10_B",
		"ObjCrystal10_C"
	};

	private int[] m_tblInfo;

	private int m_tblCount;

	private static EventObjectTable s_instance = null;

	public static EventObjectTable Instance
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
			UnityEngine.Object.Destroy(base.gameObject);
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

	public static string GetItemName(uint index)
	{
		if (index < 8 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public static bool IsEventCrystalBig(EventObjectTableItem index)
	{
		switch (index)
		{
		case EventObjectTableItem.CrystalA:
		case EventObjectTableItem.CrystalB:
		case EventObjectTableItem.CrystalC:
			return false;
		default:
			return true;
		}
	}

	private void Setup()
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new int[24];
		}
		TextAsset dataTabel = m_dataTabel;
		if ((bool)dataTabel)
		{
			string xml = AESCrypt.Decrypt(dataTabel.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, m_tblInfo, out m_tblCount);
			if (m_tblCount != 0)
			{
			}
		}
	}

	public static void CreateTable(XmlDocument doc, int[] data, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("EventObjectTable");
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
					int num2 = 0;
					if (xmlAttribute != null)
					{
						num2 = int.Parse(item2.Attributes[itemName].Value, NumberStyles.AllowLeadingSign);
					}
					int num3 = num * 8 + i;
					data[num3] = num2;
				}
			}
			num++;
		}
		tbl_count = num;
	}

	public bool IsSetupEnd()
	{
		if (m_tblInfo == null)
		{
			return false;
		}
		return true;
	}

	public int GetData(int tbl_index, EventObjectTableItem item_index)
	{
		if (m_tblInfo != null && (uint)tbl_index < m_tblCount)
		{
			int num = (int)(tbl_index * 8 + item_index);
			if (num < m_tblInfo.Length)
			{
				return m_tblInfo[num];
			}
		}
		return 0;
	}

	private int GetData(int index)
	{
		if (m_tblInfo != null && index < m_tblInfo.Length)
		{
			return m_tblInfo[index];
		}
		return 0;
	}

	public static void LoadSetup()
	{
		EventObjectTable instance = Instance;
		if (instance == null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "EventObjectTable");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	public static int GetItemData(int tbl_index, EventObjectTableItem item_index)
	{
		EventObjectTable instance = Instance;
		if (instance != null)
		{
			return instance.GetData(tbl_index, item_index);
		}
		return 0;
	}

	public static EventObjectTableItem GetEventObjectTableItem(string objName)
	{
		int num = Array.IndexOf(ITEM_NAMES, objName);
		if (num >= 0)
		{
			return (EventObjectTableItem)num;
		}
		return EventObjectTableItem.NONE;
	}

	public static bool IsCyrstal(EventObjectTableItem item)
	{
		if (Array.IndexOf(CRYSTAL_ITEMS, item) >= 0)
		{
			return true;
		}
		return false;
	}
}
