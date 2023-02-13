using System.Xml;
using UnityEngine;

public class EventBossParamTable : MonoBehaviour
{
	public struct DropRingParam
	{
		public int rate;

		public int normal;

		public int rare;

		public int srare;
	}

	public class BossParam
	{
		public DropRingParam[] m_rings = new DropRingParam[10];
	}

	public const int ITEM_COUNT_MAX = 12;

	public const int RING_LEVEL_COUNT_MAX = 10;

	public const int TBL_COUNT_MAX = 1;

	[SerializeField]
	private TextAsset m_dataTabel;

	private static readonly string[] ITEM_NAMES = new string[12]
	{
		"Level1",
		"Level2",
		"Level3",
		"Level4",
		"WispRatio",
		"WispRatioDown",
		"BoostAttack1",
		"BoostAttack2",
		"BoostAttack3",
		"BoostSpeed1",
		"BoostSpeed2",
		"BoostSpeed3"
	};

	private static readonly string[] RING_DROP_LEVEL_NAMES = new string[10]
	{
		"RingDropLevel1",
		"RingDropLevel2",
		"RingDropLevel3",
		"RingDropLevel4",
		"RingDropLevel5",
		"RingDropLevel6",
		"RingDropLevel7",
		"RingDropLevel8",
		"RingDropLevel9",
		"RingDropLevel10"
	};

	private float[] m_tblInfo;

	private BossParam m_newTblInfo;

	private int m_tblCount;

	private static EventBossParamTable s_instance = null;

	public static EventBossParamTable Instance
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

	public static string GetItemName(uint index)
	{
		if (index < 12 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public static string GetRingDropName(uint index)
	{
		if (index < 10 && index < RING_DROP_LEVEL_NAMES.Length)
		{
			return RING_DROP_LEVEL_NAMES[index];
		}
		return string.Empty;
	}

	private void Setup()
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new float[12];
		}
		if (m_newTblInfo == null)
		{
			m_newTblInfo = new BossParam();
		}
		TextAsset dataTabel = m_dataTabel;
		if ((bool)dataTabel)
		{
			string xml = AESCrypt.Decrypt(dataTabel.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, m_tblInfo, m_newTblInfo, out m_tblCount);
			if (m_tblCount != 0)
			{
			}
		}
	}

	public static void CreateTable(XmlDocument doc, float[] data, BossParam param, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("EventBossParamTable");
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
				for (int i = 0; i < 12; i++)
				{
					string itemName = GetItemName((uint)i);
					XmlAttribute xmlAttribute = item2.Attributes[itemName];
					float num2 = 0f;
					if (xmlAttribute != null)
					{
						num2 = float.Parse(item2.Attributes[itemName].Value);
					}
					int num3 = num * 12 + i;
					data[num3] = num2;
				}
			}
			XmlNodeList xmlNodeList3 = item.SelectNodes("Ring");
			int num4 = 0;
			foreach (XmlNode item3 in xmlNodeList3)
			{
				if (num4 < 10)
				{
					int result = 0;
					if (int.TryParse(item3.Attributes["SurperRingRate"].Value, out result))
					{
						param.m_rings[num4].rate = result;
					}
					int result2 = 0;
					if (int.TryParse(item3.Attributes["Normal"].Value, out result2))
					{
						param.m_rings[num4].normal = result2;
					}
					int result3 = 0;
					if (int.TryParse(item3.Attributes["Rare"].Value, out result3))
					{
						param.m_rings[num4].rare = result3;
					}
					int result4 = 0;
					if (int.TryParse(item3.Attributes["SRare"].Value, out result4))
					{
						param.m_rings[num4].srare = result4;
					}
					num4++;
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

	public float GetData(EventBossParamTableItem item_index)
	{
		int num = 0;
		if (m_tblInfo != null && (uint)num < m_tblCount)
		{
			int num2 = (int)(num * 12 + item_index);
			if (num2 < m_tblInfo.Length)
			{
				return m_tblInfo[num2];
			}
		}
		return 0f;
	}

	public int GetData(BossType bossType, int playerAggressivity)
	{
		if (m_newTblInfo != null)
		{
			for (int i = 0; i < 10; i++)
			{
				int num = -1;
				switch (bossType)
				{
				case BossType.EVENT1:
					num = m_newTblInfo.m_rings[i].normal;
					break;
				case BossType.EVENT2:
					num = m_newTblInfo.m_rings[i].rare;
					break;
				case BossType.EVENT3:
					num = m_newTblInfo.m_rings[i].srare;
					break;
				}
				if (num > 0 && playerAggressivity <= num)
				{
					return m_newTblInfo.m_rings[i].rate;
				}
			}
		}
		return 0;
	}

	private float GetData(int index)
	{
		return GetData((EventBossParamTableItem)index);
	}

	public static void LoadSetup()
	{
		EventBossParamTable instance = Instance;
		if (instance == null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "EventBossParamTable");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
		}
	}

	public static float GetItemData(EventBossParamTableItem item_index)
	{
		EventBossParamTable instance = Instance;
		if (instance != null)
		{
			return instance.GetData(item_index);
		}
		return 0f;
	}

	public static int GetSuperRingDropData(BossType bossType, int playerAggressivity)
	{
		if (Instance != null)
		{
			return Instance.GetData(bossType, playerAggressivity);
		}
		return 0;
	}
}
