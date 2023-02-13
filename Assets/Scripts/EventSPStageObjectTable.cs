using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EventSPStageObjectTable : MonoBehaviour
{
	public const int ITEM_COUNT_MAX = 8;

	public const int TBL_COUNT_MAX = 1;

	[SerializeField]
	private TextAsset m_dataTabel;

	private static readonly string[] ITEM_NAMES = new string[8]
	{
		"SPCrystalModelA",
		"SPCrystalModelB",
		"SPCrystalModelC",
		"SPCrystal10Model",
		"SPCrystalEffect",
		"SPCrystal10Effect",
		"SPCrystalSE",
		"SPCrystal10SE"
	};

	private static readonly EventSPStageObjectTableItem[] SPCRYSTAL_MODEL_TBL = new EventSPStageObjectTableItem[3]
	{
		EventSPStageObjectTableItem.SPCrystalModelA,
		EventSPStageObjectTableItem.SPCrystalModelB,
		EventSPStageObjectTableItem.SPCrystalModelC
	};

	private List<string> m_spCrystalModelList = new List<string>();

	private string[] m_tblInfo;

	private int m_tblCount;

	private static EventSPStageObjectTable s_instance = null;

	public static EventSPStageObjectTable Instance
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
	}

	public static string GetItemName(uint index)
	{
		if (index < 8 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	private void Setup()
	{
		if (IsSetupEnd())
		{
			return;
		}
		if (m_tblInfo == null)
		{
			m_tblInfo = new string[8];
		}
		TextAsset dataTabel = m_dataTabel;
		if (!dataTabel)
		{
			return;
		}
		string xml = AESCrypt.Decrypt(dataTabel.text);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		CreateTable(xmlDocument, m_tblInfo, out m_tblCount);
		if (m_tblCount == 0)
		{
			return;
		}
		for (int i = 0; i < SPCRYSTAL_MODEL_TBL.Length; i++)
		{
			string itemData = GetItemData(SPCRYSTAL_MODEL_TBL[i]);
			if (itemData != null && itemData != string.Empty)
			{
				m_spCrystalModelList.Add(itemData);
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
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("EventSPStageObjectTable");
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

	public bool IsSetupEnd()
	{
		if (m_tblInfo == null)
		{
			return false;
		}
		return true;
	}

	public string GetData(EventSPStageObjectTableItem item_index)
	{
		if (!IsSetupEnd())
		{
			Setup();
		}
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

	private string GetData(int index)
	{
		return GetData((EventSPStageObjectTableItem)index);
	}

	public List<string> GetSPCrystalModelList()
	{
		if (!IsSetupEnd())
		{
			Setup();
		}
		return m_spCrystalModelList;
	}

	public static void LoadSetup()
	{
		EventSPStageObjectTable instance = Instance;
		if (instance == null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "EventSPStageObjectTable");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	public static string GetItemData(EventSPStageObjectTableItem item_index)
	{
		EventSPStageObjectTable instance = Instance;
		if (instance != null)
		{
			return instance.GetData(item_index);
		}
		return string.Empty;
	}

	public static string GetSPCrystalModel()
	{
		EventSPStageObjectTable instance = Instance;
		if (instance != null)
		{
			List<string> sPCrystalModelList = instance.GetSPCrystalModelList();
			if (sPCrystalModelList != null && sPCrystalModelList.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, sPCrystalModelList.Count);
				return sPCrystalModelList[index];
			}
		}
		return string.Empty;
	}

	public static EventSPStageObjectTableItem GetEventSPStageObjectTableItem(string objName)
	{
		int num = Array.IndexOf(ITEM_NAMES, objName);
		if (num >= 0)
		{
			return (EventSPStageObjectTableItem)num;
		}
		return EventSPStageObjectTableItem.NONE;
	}
}
