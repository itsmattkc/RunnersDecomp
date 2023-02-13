using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class MileageMapPointDataTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private MileageMapPointData[] m_point_data;

	private static MileageMapPointDataTable instance;

	public static MileageMapPointDataTable Instance
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
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(MileageMapPointData[]));
			StringReader textReader = new StringReader(s);
			m_point_data = (MileageMapPointData[])xmlSerializer.Deserialize(textReader);
			if (m_point_data != null)
			{
				Array.Sort(m_point_data);
			}
		}
	}

	public string GetTextureName(int id)
	{
		if (m_point_data != null)
		{
			int num = m_point_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_point_data[i].id == id)
				{
					return m_point_data[i].texture_name;
				}
			}
		}
		return null;
	}
}
