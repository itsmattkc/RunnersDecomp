using System;
using System.IO;
using System.Xml.Serialization;
using Text;
using UnityEngine;

public class MileageMapRouteDataTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private MileageMapRouteData[] m_route_data;

	private static MileageMapRouteDataTable instance;

	public static MileageMapRouteDataTable Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		SetData();
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (this != Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void SetData()
	{
		if (m_xml_data != null)
		{
			string s = AESCrypt.Decrypt(m_xml_data.text);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(MileageMapRouteData[]));
			StringReader textReader = new StringReader(s);
			m_route_data = (MileageMapRouteData[])xmlSerializer.Deserialize(textReader);
			if (m_route_data != null)
			{
				Array.Sort(m_route_data);
			}
		}
	}

	public MileageMapRouteData GetMileageMapRouteData(int id)
	{
		if (m_route_data != null)
		{
			int num = m_route_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_route_data[i].id == id)
				{
					return m_route_data[i];
				}
			}
		}
		return null;
	}

	public MileageBonus GetBonusType(int id)
	{
		MileageBonus result = MileageBonus.UNKNOWN;
		if (m_route_data != null)
		{
			int num = m_route_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_route_data[i].id == id)
				{
					result = m_route_data[i].ability_type;
					break;
				}
			}
		}
		return result;
	}

	public string GetBonusTypeText(int id)
	{
		string bonusTypeTextWithoutColor = GetBonusTypeTextWithoutColor(id);
		if (bonusTypeTextWithoutColor != null)
		{
			string str = "[00d2ff]";
			return str + bonusTypeTextWithoutColor;
		}
		return null;
	}

	public string GetBonusTypeTextWithoutColor(int id)
	{
		if (m_route_data != null)
		{
			MileageBonus bonusType = GetBonusType(id);
			TextObject textObject = null;
			TextManager.TextType textType = TextManager.TextType.TEXTTYPE_COMMON_TEXT;
			switch (bonusType)
			{
			case MileageBonus.SCORE:
				textObject = TextManager.GetText(textType, "Score", "score");
				break;
			case MileageBonus.ANIMAL:
				textObject = TextManager.GetText(textType, "Score", "animal");
				break;
			case MileageBonus.RING:
				textObject = TextManager.GetText(textType, "Item", "get_ring");
				break;
			case MileageBonus.DISTANCE:
				textObject = TextManager.GetText(textType, "Score", "distance");
				break;
			case MileageBonus.FINAL_SCORE:
				textObject = TextManager.GetText(textType, "Score", "final_score");
				break;
			}
			if (textObject != null)
			{
				return textObject.text;
			}
		}
		return null;
	}
}
