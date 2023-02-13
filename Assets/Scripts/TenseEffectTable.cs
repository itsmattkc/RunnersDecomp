using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TenseEffectTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_dataTabel;

	private List<TenseParameter> m_editParameters = new List<TenseParameter>();

	private static TenseEffectTable s_instance;

	public static TenseEffectTable Instance
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

	private void Setup()
	{
		TextAsset dataTabel = m_dataTabel;
		if ((bool)dataTabel)
		{
			string xml = AESCrypt.Decrypt(dataTabel.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, out m_editParameters);
			if (m_editParameters.Count != 0)
			{
			}
		}
	}

	public static void CreateTable(XmlDocument doc, out List<TenseParameter> outdata)
	{
		outdata = new List<TenseParameter>();
		outdata.Clear();
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("TenseEffectTable");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		foreach (XmlNode item2 in xmlNodeList)
		{
			string name_ = string.Empty;
			XmlAttribute xmlAttribute = item2.Attributes["item"];
			if (xmlAttribute != null)
			{
				name_ = xmlAttribute.Value;
			}
			float r = 0f;
			XmlAttribute xmlAttribute2 = item2.Attributes["r"];
			if (xmlAttribute2 != null)
			{
				r = float.Parse(xmlAttribute2.Value);
			}
			float g = 0f;
			XmlAttribute xmlAttribute3 = item2.Attributes["g"];
			if (xmlAttribute3 != null)
			{
				g = float.Parse(xmlAttribute3.Value);
			}
			float b = 0f;
			XmlAttribute xmlAttribute4 = item2.Attributes["b"];
			if (xmlAttribute4 != null)
			{
				b = float.Parse(xmlAttribute4.Value);
			}
			float a = 0f;
			XmlAttribute xmlAttribute5 = item2.Attributes["a"];
			if (xmlAttribute5 != null)
			{
				a = float.Parse(xmlAttribute5.Value);
			}
			TenseParameter item = new TenseParameter(name_, new Color(r, g, b, a));
			outdata.Add(item);
		}
	}

	public bool IsSetupEnd()
	{
		if (m_editParameters.Count == 0)
		{
			return false;
		}
		return true;
	}

	public Color GetData(string itemName)
	{
		if (IsSetupEnd())
		{
			foreach (TenseParameter editParameter in m_editParameters)
			{
				if (editParameter.name == itemName)
				{
					return editParameter.color;
				}
			}
		}
		return Color.white;
	}

	public static Color GetItemData(string itemName)
	{
		TenseEffectTable instance = Instance;
		if (instance != null)
		{
			return instance.GetData(itemName);
		}
		return Color.white;
	}
}
