using System.Globalization;
using System.Xml;
using UnityEngine;

public class RareEnemyTable
{
	public const int TBL_COUNT_MAX = 8;

	private int[] m_tblInfo;

	private int m_tblCount;

	public bool IsRareEnemy(uint tbl_index)
	{
		if (m_tblInfo != null && tbl_index < m_tblCount && tbl_index < m_tblInfo.Length)
		{
			int randomRange = ObjUtil.GetRandomRange100();
			int src_value = m_tblInfo[tbl_index];
			src_value = ObjUtil.GetChaoAbliltyValue(ChaoAbility.RARE_ENEMY_UP, src_value);
			if (src_value > randomRange)
			{
				return true;
			}
		}
		return false;
	}

	public void Setup(TerrainXmlData terrainData)
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new int[8];
		}
		if (!(terrainData != null))
		{
			return;
		}
		TextAsset rareEnemyTableData = terrainData.RareEnemyTableData;
		if ((bool)rareEnemyTableData)
		{
			string xml = AESCrypt.Decrypt(rareEnemyTableData.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, m_tblInfo, out m_tblCount);
			if (m_tblCount != 0)
			{
			}
		}
	}

	public static void CreateTable(XmlDocument doc, int[] data, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("RareEnemyTable");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (XmlNode item in xmlNodeList)
		{
			XmlNodeList xmlNodeList2 = item.SelectNodes("Table");
			foreach (XmlNode item2 in xmlNodeList2)
			{
				int num2 = data[num] = int.Parse(item2.Attributes["Param"].Value, NumberStyles.AllowLeadingSign);
			}
			num++;
		}
		tbl_count = num;
	}
}
