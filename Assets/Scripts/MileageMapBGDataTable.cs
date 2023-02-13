using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class MileageMapBGDataTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private MileageMapBGData[] m_bg_data;

	private static MileageMapBGDataTable instance;

	public static MileageMapBGDataTable Instance
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
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(MileageMapBGData[]));
			StringReader textReader = new StringReader(s);
			m_bg_data = (MileageMapBGData[])xmlSerializer.Deserialize(textReader);
			if (m_bg_data != null)
			{
				Array.Sort(m_bg_data);
			}
		}
	}

	public string GetTextureName(int id)
	{
		if (m_bg_data != null)
		{
			int num = m_bg_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_bg_data[i].id == id)
				{
					return m_bg_data[i].texture_name;
				}
			}
		}
		return null;
	}

	public string GetWindowTextureName(int id)
	{
		if (m_bg_data != null)
		{
			int num = m_bg_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_bg_data[i].id == id)
				{
					return m_bg_data[i].window_texture_name;
				}
			}
		}
		return null;
	}
}
