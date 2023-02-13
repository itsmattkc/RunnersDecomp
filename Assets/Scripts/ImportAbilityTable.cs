using System.Xml;
using UnityEngine;

public class ImportAbilityTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_textAsset;

	private static ImportAbilityTable m_instance;

	private AbilityUpParamTable m_table;

	private void Awake()
	{
		if (m_instance == null)
		{
			Initialize();
			m_instance = this;
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void Initialize()
	{
		if (m_textAsset == null)
		{
			return;
		}
		string xml = AESCrypt.Decrypt(m_textAsset.text);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		XmlNode documentElement = xmlDocument.DocumentElement;
		if (documentElement == null)
		{
			return;
		}
		XmlNodeList childNodes = documentElement.ChildNodes;
		if (childNodes == null)
		{
			return;
		}
		m_table = new AbilityUpParamTable();
		int num = 0;
		foreach (XmlNode item in childNodes)
		{
			if (item == null)
			{
				continue;
			}
			AbilityUpParamList abilityUpParamList = new AbilityUpParamList();
			XmlNodeList childNodes2 = item.ChildNodes;
			foreach (XmlNode item2 in childNodes2)
			{
				if (item2 != null)
				{
					string value = item2.Attributes.GetNamedItem("ring_cost").Value;
					string value2 = item2.Attributes.GetNamedItem("potential").Value;
					AbilityUpParam abilityUpParam = new AbilityUpParam();
					abilityUpParam.Cost = float.Parse(value);
					abilityUpParam.Potential = float.Parse(value2);
					abilityUpParamList.AddAbilityUpParam(abilityUpParam);
				}
			}
			if (abilityUpParamList.Count > 0)
			{
				AbilityType type = (AbilityType)num;
				m_table.AddList(type, abilityUpParamList);
				num++;
			}
		}
	}

	public float GetAbilityPotential(AbilityType type, int level)
	{
		if (m_table == null)
		{
			return 0f;
		}
		AbilityUpParamList list = m_table.GetList(type);
		if (list == null)
		{
			return 0f;
		}
		AbilityUpParam abilityUpParam = list.GetAbilityUpParam(level);
		if (abilityUpParam == null)
		{
			return 0f;
		}
		return abilityUpParam.Potential;
	}

	public float GetAbilityCost(AbilityType type, int level)
	{
		if (m_table == null)
		{
			return 0f;
		}
		AbilityUpParamList list = m_table.GetList(type);
		if (list == null)
		{
			return 0f;
		}
		AbilityUpParam abilityUpParam = list.GetAbilityUpParam(level);
		if (abilityUpParam == null)
		{
			return 0f;
		}
		return abilityUpParam.Cost;
	}

	public int GetMaxLevel(AbilityType type)
	{
		if (m_table == null)
		{
			return 0;
		}
		AbilityUpParamList list = m_table.GetList(type);
		if (list == null)
		{
			return 0;
		}
		return list.GetMaxLevel();
	}

	public static ImportAbilityTable GetInstance()
	{
		return m_instance;
	}
}
