using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class StageSuggestedDataTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private StageSuggestedData[] m_data;

	private static StageSuggestedDataTable instance;

	public static StageSuggestedDataTable Instance
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
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(StageSuggestedData[]));
			StringReader textReader = new StringReader(s);
			m_data = (StageSuggestedData[])xmlSerializer.Deserialize(textReader);
			if (m_data != null)
			{
				Array.Sort(m_data);
			}
		}
	}

	public CharacterAttribute[] GetStageSuggestedData(int stageIndex)
	{
		if (m_data != null)
		{
			int num = m_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_data[i].id == stageIndex)
				{
					return m_data[i].charaAttribute;
				}
			}
		}
		return null;
	}
}
