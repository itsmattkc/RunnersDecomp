using App.Utility;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CharacterDataNameInfo : MonoBehaviour
{
	public class Info
	{
		public enum Option
		{
			Big,
			HighSpeed,
			ThirdJump
		}

		public string m_name;

		public CharaType m_ID = CharaType.UNKNOWN;

		public string m_hud_suffix;

		public CharacterAttribute m_attribute = CharacterAttribute.UNKNOWN;

		public TeamAttribute m_teamAttribute = TeamAttribute.UNKNOWN;

		public TeamAttributeCategory m_teamAttributeCategory = TeamAttributeCategory.NONE;

		public TeamAttributeBonusType m_mainAttributeBonus = TeamAttributeBonusType.NONE;

		public TeamAttributeBonusType m_subAttributeBonus = TeamAttributeBonusType.NONE;

		private float m_teamAttributeValue;

		private float m_teamAttributeSubValue;

		public int m_serverID;

		public Bitset32 m_flag;

		public float TeamAttributeValue
		{
			get
			{
				return m_teamAttributeValue;
			}
			set
			{
				m_teamAttributeValue = value;
			}
		}

		public float TeamAttributeSubValue
		{
			get
			{
				return m_teamAttributeSubValue;
			}
			set
			{
				m_teamAttributeSubValue = value;
			}
		}

		public bool BigSize
		{
			get
			{
				return m_flag.Test(0);
			}
			set
			{
				m_flag.Set(0, value);
			}
		}

		public bool HighSpeedEffect
		{
			get
			{
				return m_flag.Test(1);
			}
			set
			{
				m_flag.Set(1, value);
			}
		}

		public bool ThirdJump
		{
			get
			{
				return m_flag.Test(2);
			}
			set
			{
				m_flag.Set(2, value);
			}
		}

		public string characterSpriteName
		{
			get
			{
				string result = null;
				int iD = (int)m_ID;
				if (iD >= 0)
				{
					result = string.Format("ui_tex_player_{0:00}_{1}", iD, m_prefixNameList[iD]);
				}
				return result;
			}
		}

		public float GetTeamAttributeValue(TeamAttributeBonusType type)
		{
			if (m_mainAttributeBonus == type)
			{
				return m_teamAttributeValue;
			}
			if (m_subAttributeBonus == type)
			{
				return m_teamAttributeSubValue;
			}
			return 0f;
		}
	}

	private List<Info> m_list;

	private static string[] m_prefixNameList;

	private static string[] m_charaNameList;

	private static string[] m_charaNameLowerList;

	[SerializeField]
	private TextAsset m_text;

	private static CharacterDataNameInfo instance;

	public static string[] PrefixNameList
	{
		get
		{
			return m_prefixNameList;
		}
	}

	public static string[] CharaNameList
	{
		get
		{
			return m_charaNameList;
		}
	}

	public static string[] CharaNameLowerList
	{
		get
		{
			return m_charaNameLowerList;
		}
	}

	public static CharacterDataNameInfo Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObjectUtil.FindGameObjectComponent<CharacterDataNameInfo>("CharacterDataNameInfo");
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
			Setup();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void Start()
	{
	}

	private void Setup()
	{
		if (m_list == null)
		{
			m_list = new List<Info>();
			string xml = AESCrypt.Decrypt(m_text.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			m_list = new List<Info>();
			CreateTable(xmlDocument, m_list);
			SetNameList();
		}
	}

	public Info GetDataByIndex(int index)
	{
		if (index < 0 || index >= m_list.Count)
		{
			return null;
		}
		return m_list[index];
	}

	public Info GetDataByName(string name)
	{
		foreach (Info item in m_list)
		{
			if (item.m_name == name)
			{
				return item;
			}
		}
		return null;
	}

	public Info GetDataByID(CharaType id)
	{
		foreach (Info item in m_list)
		{
			if (item.m_ID == id)
			{
				return item;
			}
		}
		return null;
	}

	public string GetNameByID(CharaType id)
	{
		foreach (Info item in m_list)
		{
			if (item.m_ID == id)
			{
				return item.m_name;
			}
		}
		return null;
	}

	public Info GetDataByServerID(int serverID)
	{
		foreach (Info item in m_list)
		{
			if (item.m_serverID == serverID)
			{
				return item;
			}
		}
		return null;
	}

	public string GetNameByServerID(int serverID)
	{
		foreach (Info item in m_list)
		{
			if (item.m_serverID == serverID)
			{
				return item.m_name;
			}
		}
		return null;
	}

	public static void CreateTable(XmlDocument doc, List<Info> list)
	{
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.SelectNodes("SonicRunnersCharacterInfo");
		if (xmlNodeList == null || xmlNodeList.Count == 0 || xmlNodeList[0] == null)
		{
			return;
		}
		XmlNodeList xmlNodeList2 = xmlNodeList[0].SelectNodes("Character");
		foreach (XmlNode item in xmlNodeList2)
		{
			Info info = new Info();
			info.m_name = item.Attributes["Name"].Value;
			string value = item.Attributes["ID"].Value;
			if (Enum.IsDefined(typeof(CharaType), value))
			{
				info.m_ID = (CharaType)(int)Enum.Parse(typeof(CharaType), value, true);
			}
			info.m_hud_suffix = item.Attributes["Suffix"].Value;
			string value2 = item.Attributes["Attribute"].Value;
			if (Enum.IsDefined(typeof(CharacterAttribute), value2))
			{
				info.m_attribute = (CharacterAttribute)(int)Enum.Parse(typeof(CharacterAttribute), value2, true);
			}
			string value3 = item.Attributes["Team"].Value;
			if (Enum.IsDefined(typeof(TeamAttribute), value3))
			{
				info.m_teamAttribute = (TeamAttribute)(int)Enum.Parse(typeof(TeamAttribute), value3, true);
			}
			string value4 = item.Attributes["MainBonus"].Value;
			if (Enum.IsDefined(typeof(TeamAttributeBonusType), value4))
			{
				info.m_mainAttributeBonus = (TeamAttributeBonusType)(int)Enum.Parse(typeof(TeamAttributeBonusType), value4, true);
			}
			string value5 = item.Attributes["SubBonus"].Value;
			if (Enum.IsDefined(typeof(TeamAttributeBonusType), value5))
			{
				info.m_subAttributeBonus = (TeamAttributeBonusType)(int)Enum.Parse(typeof(TeamAttributeBonusType), value5, true);
			}
			float result = 0f;
			if (float.TryParse(item.Attributes["TeamAttributeValue"].Value, out result))
			{
				info.TeamAttributeValue = result;
			}
			float result2 = 0f;
			if (float.TryParse(item.Attributes["TeamAttributeSubValue"].Value, out result2))
			{
				info.TeamAttributeSubValue = result2;
			}
			string value6 = item.Attributes["Category"].Value;
			if (Enum.IsDefined(typeof(TeamAttributeCategory), value6))
			{
				info.m_teamAttributeCategory = (TeamAttributeCategory)(int)Enum.Parse(typeof(TeamAttributeCategory), value6, true);
			}
			else
			{
				info.m_teamAttributeCategory = TeamAttributeCategory.NUM;
			}
			int result3;
			if (int.TryParse(item.Attributes["ServerID"].Value, out result3))
			{
				info.m_serverID = result3;
			}
			if (item.Attributes["OptBig"] != null)
			{
				string value7 = item.Attributes["OptBig"].Value;
				if (value7.Equals("true"))
				{
					info.m_flag.Set(0, true);
				}
			}
			if (item.Attributes["OptHighSpeed"] != null)
			{
				string value8 = item.Attributes["OptHighSpeed"].Value;
				if (value8.Equals("true"))
				{
					info.m_flag.Set(1, true);
				}
			}
			if (item.Attributes["OpThirdJump"] != null)
			{
				string value9 = item.Attributes["OpThirdJump"].Value;
				if (value9.Equals("true"))
				{
					info.m_flag.Set(2, true);
				}
			}
			if (info.m_ID != CharaType.UNKNOWN && info.m_attribute != CharacterAttribute.UNKNOWN && info.m_teamAttribute != TeamAttribute.UNKNOWN)
			{
				list.Add(info);
			}
		}
	}

	private void SetNameList()
	{
		if (m_list == null)
		{
			return;
		}
		m_prefixNameList = new string[m_list.Count];
		m_charaNameList = new string[m_list.Count];
		m_charaNameLowerList = new string[m_list.Count];
		foreach (Info item in m_list)
		{
			if ((int)item.m_ID < m_list.Count)
			{
				m_prefixNameList[(int)item.m_ID] = item.m_hud_suffix;
				m_charaNameList[(int)item.m_ID] = item.m_name;
				m_charaNameLowerList[(int)item.m_ID] = item.m_name.ToLower();
			}
		}
	}

	public static void LoadSetup()
	{
		GameObject gameObject = GameObject.Find("CharacterDataNameInfo");
		if (gameObject != null && gameObject.transform.parent != null && gameObject.transform.parent.name == "ETC")
		{
			gameObject.transform.parent = null;
		}
	}
}
