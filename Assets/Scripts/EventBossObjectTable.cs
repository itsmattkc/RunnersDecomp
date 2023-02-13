using System.Xml;
using UnityEngine;

public class EventBossObjectTable : MonoBehaviour
{
	public const int ITEM_COUNT_MAX = 15;

	public const int TBL_COUNT_MAX = 1;

	[SerializeField]
	private TextAsset m_dataTabel;

	private static readonly string[] ITEM_NAMES = new string[15]
	{
		"BgmFile",
		"BgmCueName",
		"RingModel",
		"Ring10Model",
		"RingEffect",
		"Ring10Effect",
		"RingSE",
		"Ring10SE",
		"EscapeEffect",
		"Obj1_ModelName",
		"Obj1_EffectName",
		"Obj1_LoopEffectName",
		"Obj1_SetSeName",
		"Obj2_ModelName",
		"Obj2_EffectName"
	};

	private string[] m_tblInfo;

	private int m_tblCount;

	private static EventBossObjectTable s_instance = null;

	public static EventBossObjectTable Instance
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
	}

	public static string GetItemName(uint index)
	{
		if (index < 15 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public void Setup()
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new string[15];
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

	public static void CreateTable(XmlDocument doc, string[] data, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("EventBossObjectTable");
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
				for (int i = 0; i < 15; i++)
				{
					string itemName = GetItemName((uint)i);
					XmlAttribute xmlAttribute = item2.Attributes[itemName];
					string text = string.Empty;
					if (xmlAttribute != null)
					{
						text = item2.Attributes[itemName].Value;
					}
					int num2 = num * 15 + i;
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

	public string GetData(EventBossObjectTableItem item_index)
	{
		int num = 0;
		if (m_tblInfo != null && (uint)num < m_tblCount)
		{
			int num2 = (int)(num * 15 + item_index);
			if (num2 < m_tblInfo.Length)
			{
				return m_tblInfo[num2];
			}
		}
		return string.Empty;
	}

	private string GetData(int index)
	{
		return GetData((EventBossObjectTableItem)index);
	}

	public static void LoadSetup()
	{
		EventBossObjectTable instance = Instance;
		if (instance == null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "EventBossObjectTable");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			EventBossObjectTable instance2 = Instance;
			if (instance2 != null)
			{
				instance2.Setup();
			}
		}
	}

	public static string GetItemData(EventBossObjectTableItem item_index)
	{
		EventBossObjectTable instance = Instance;
		if (instance != null)
		{
			return instance.GetData(item_index);
		}
		return string.Empty;
	}
}
