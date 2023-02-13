using System.Globalization;
using System.Xml;
using UnityEngine;

public class ObjectPartTable
{
	private static readonly string[] ITEM_NAMES = new string[19]
	{
		"BrokenBonusRatio",
		"BrokenBonusSuperRing",
		"BrokenBonusRedStarRing",
		"BrokenBonusCrystal10",
		"ComboBonusCombo1",
		"ComboBonusCombo2",
		"ComboBonusCombo3",
		"ComboBonusCombo4",
		"ComboBonusCombo5",
		"ComboBonusCombo6",
		"ComboBonusCombo7",
		"ComboBonusBonus1",
		"ComboBonusBonus2",
		"ComboBonusBonus3",
		"ComboBonusBonus4",
		"ComboBonusBonus5",
		"ComboBonusBonus6",
		"ComboBonusBonus7",
		"ComboBonusBonus8"
	};

	private int[] m_tblInfo;

	private void Start()
	{
	}

	public static string GetItemName(uint index)
	{
		if (index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public BrokenBonusType GetBrokenBonusType()
	{
		int data = GetData(ObjectPartType.BROKENBONUS_RATIO);
		int randomRange = ObjUtil.GetRandomRange100();
		if (randomRange < data)
		{
			bool flag = false;
			if (StageModeManager.Instance != null)
			{
				flag = StageModeManager.Instance.FirstTutorial;
			}
			int randomRange2 = ObjUtil.GetRandomRange100();
			int num = 0;
			for (int i = 1; i < 4; i++)
			{
				int num2 = GetData((ObjectPartType)i);
				if (flag && i == 2)
				{
					num2 = 0;
				}
				if (num <= randomRange2 && randomRange2 < num + num2)
				{
					switch (i)
					{
					case 1:
						return BrokenBonusType.SUPER_RING;
					case 2:
						return BrokenBonusType.REDSTAR_RING;
					case 3:
						return BrokenBonusType.CRYSTAL10;
					}
				}
				num += num2;
			}
		}
		return BrokenBonusType.NONE;
	}

	public BrokenBonusType GetBrokenBonusTypeForChaoAbility()
	{
		int data = GetData(ObjectPartType.BROKENBONUS_RATIO);
		int randomRange = ObjUtil.GetRandomRange100();
		if (randomRange < data)
		{
			int randomRange2 = ObjUtil.GetRandomRange100();
			int num = 0;
			if (StageAbilityManager.Instance != null)
			{
				num = (int)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ChaoAbility.COMBO_STEP_DESTROY_GET_10_RING);
			}
			if (randomRange2 <= 1)
			{
				return BrokenBonusType.REDSTAR_RING;
			}
			if (randomRange2 <= num)
			{
				return BrokenBonusType.SUPER_RING;
			}
			return BrokenBonusType.CRYSTAL10;
		}
		return BrokenBonusType.NONE;
	}

	public int GetComboBonusComboNum(int index)
	{
		return GetData((ObjectPartType)(4 + index));
	}

	public int GetComboBonusBonusNum(int index)
	{
		return GetData((ObjectPartType)(11 + index));
	}

	public void Setup(TerrainXmlData terrainData)
	{
		if (m_tblInfo == null)
		{
			m_tblInfo = new int[19];
		}
		if (terrainData != null)
		{
			TextAsset objectPartTableData = terrainData.ObjectPartTableData;
			if ((bool)objectPartTableData)
			{
				string xml = AESCrypt.Decrypt(objectPartTableData.text);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xml);
				CreateTable(xmlDocument, m_tblInfo);
			}
		}
	}

	public static void CreateTable(XmlDocument doc, int[] data)
	{
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("ObjectPartTable");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 19; i++)
		{
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				string itemName = GetItemName((uint)i);
				XmlNodeList xmlNodeList2 = item.SelectNodes(itemName);
				foreach (XmlNode item2 in xmlNodeList2)
				{
					if (item2.InnerText != null)
					{
						num = int.Parse(item2.InnerText, NumberStyles.AllowLeadingSign);
					}
				}
			}
			data[i] = num;
		}
	}

	private int GetData(ObjectPartType item_index)
	{
		if (m_tblInfo != null && (int)item_index < m_tblInfo.Length)
		{
			return m_tblInfo[(int)item_index];
		}
		return 0;
	}
}
