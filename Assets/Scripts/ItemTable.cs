using System.Globalization;
using System.Xml;
using UnityEngine;

public class ItemTable
{
	public const int ITEM_COUNT_MAX = 32;

	public const int TBL_COUNT_MAX = 16;

	private int[] m_tblInfo;

	private int m_tblCount;

	public ItemType GetItemType(int tbl_index)
	{
		if (m_tblInfo != null && (uint)tbl_index < m_tblCount)
		{
			int num = tbl_index * 12;
			int num2 = num + 8;
			bool flag = false;
			flag = (!(StageModeManager.Instance != null) || !StageModeManager.Instance.IsQuickMode() || !ObjTimerUtil.IsEnableCreateTimer());
			int num3 = 0;
			for (int i = 0; i < 8; i++)
			{
				int num4 = num + i;
				num3 += m_tblInfo[num4];
			}
			for (int j = 0; j < 4; j++)
			{
				if (!flag || (j != 1 && j != 2 && j != 3))
				{
					int num5 = num2 + j;
					num3 += m_tblInfo[num5];
				}
			}
			int randomRange = ObjUtil.GetRandomRange(num3);
			int num6 = 0;
			for (int k = 0; k < 8; k++)
			{
				int num7 = num + k;
				int num8 = m_tblInfo[num7];
				if (num6 <= randomRange && randomRange < num6 + num8)
				{
					return (ItemType)k;
				}
				num6 += num8;
			}
			for (int l = 0; l < 4; l++)
			{
				if (flag && (l == 1 || l == 2 || l == 3))
				{
					continue;
				}
				int num9 = num2 + l;
				int num10 = m_tblInfo[num9];
				if (num6 <= randomRange && randomRange < num6 + num10)
				{
					switch (l)
					{
					case 0:
						return ItemType.REDSTAR_RING;
					case 1:
						return ItemType.TIMER_BRONZE;
					case 2:
						return ItemType.TIMER_SILVER;
					case 3:
						return ItemType.TIMER_GOLD;
					}
				}
				num6 += num10;
			}
		}
		return ItemType.TRAMPOLINE;
	}

	public void Setup(TerrainXmlData terrainData)
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new int[512];
		}
		if (m_tblInfo.Length < 128 || !(terrainData != null))
		{
			return;
		}
		TextAsset itemTableData = terrainData.ItemTableData;
		if ((bool)itemTableData)
		{
			string xml = AESCrypt.Decrypt(itemTableData.text);
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
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("ItemTable");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (XmlNode item in xmlNodeList)
		{
			int num2 = num * 4;
			XmlNodeList xmlNodeList2 = item.SelectNodes("Item");
			foreach (XmlNode item2 in xmlNodeList2)
			{
				for (int i = 0; i < 8; i++)
				{
					string itemTypeName = ItemTypeName.GetItemTypeName((ItemType)i);
					XmlAttribute xmlAttribute = item2.Attributes[itemTypeName];
					int num3 = 0;
					if (xmlAttribute != null)
					{
						num3 = int.Parse(item2.Attributes[itemTypeName].Value, NumberStyles.AllowLeadingSign);
					}
					int num4 = num * 8 + i + num2;
					data[num4] = num3;
				}
				for (int j = 0; j < 4; j++)
				{
					string otherItemTypeName = ItemTypeName.GetOtherItemTypeName((OtherItemType)j);
					XmlAttribute xmlAttribute2 = item2.Attributes[otherItemTypeName];
					int num5 = 0;
					if (xmlAttribute2 != null)
					{
						num5 = int.Parse(item2.Attributes[otherItemTypeName].Value, NumberStyles.AllowLeadingSign);
					}
					int num6 = num * 8 + 8 + j + num2;
					data[num6] = num5;
				}
			}
			num++;
		}
		tbl_count = num;
	}
}
