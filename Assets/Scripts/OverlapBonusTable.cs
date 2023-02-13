using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

public class OverlapBonusTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_overlapBonusTabel;

	private static Dictionary<CharaType, List<int[]>> m_OverlapBonus = new Dictionary<CharaType, List<int[]>>();

	private static readonly string[] ITEM_NAMES = new string[5]
	{
		"Score",
		"Ring",
		"Animal",
		"Distance",
		"Enemy"
	};

	private int m_tblCount;

	private void Start()
	{
	}

	public void Setup()
	{
		if ((bool)m_overlapBonusTabel)
		{
			string xml = AESCrypt.Decrypt(m_overlapBonusTabel.text);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			CreateTable(xmlDocument, out m_tblCount);
			if (m_tblCount != 0)
			{
			}
		}
	}

	public static string GetItemName(uint index)
	{
		if (index < 5 && index < ITEM_NAMES.Length)
		{
			return ITEM_NAMES[index];
		}
		return string.Empty;
	}

	public static void CreateTable(XmlDocument doc, out int tbl_count)
	{
		tbl_count = 0;
		if (doc == null || doc.DocumentElement == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = doc.DocumentElement.SelectNodes("BonusData");
		if (xmlNodeList == null || xmlNodeList.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (XmlNode item in xmlNodeList)
		{
			XmlAttribute xmlAttribute = item.Attributes["charaID"];
			int serverID = 0;
			if (xmlAttribute != null)
			{
				serverID = int.Parse(item.Attributes["charaID"].Value, NumberStyles.AllowLeadingSign);
			}
			CharacterDataNameInfo instance = CharacterDataNameInfo.Instance;
			if (!(instance != null))
			{
				continue;
			}
			CharacterDataNameInfo.Info dataByServerID = instance.GetDataByServerID(serverID);
			if (dataByServerID == null)
			{
				continue;
			}
			CharaType iD = dataByServerID.m_ID;
			XmlNodeList xmlNodeList2 = item.SelectNodes("Param");
			int num2 = 0;
			foreach (XmlNode item2 in xmlNodeList2)
			{
				int[] array = new int[5];
				for (int i = 0; i < 5; i++)
				{
					string itemName = GetItemName((uint)i);
					XmlAttribute xmlAttribute2 = item2.Attributes[itemName];
					int num3 = 0;
					if (xmlAttribute2 != null)
					{
						num3 = int.Parse(item2.Attributes[itemName].Value, NumberStyles.AllowLeadingSign);
					}
					array[i] = num3;
				}
				if (!m_OverlapBonus.ContainsKey(iD))
				{
					m_OverlapBonus.Add(iD, new List<int[]>());
				}
				m_OverlapBonus[iD].Add(array);
				num2++;
			}
			num++;
		}
		tbl_count = num;
	}

	public bool IsSetupEnd()
	{
		if (m_tblCount <= 0)
		{
			return false;
		}
		return true;
	}

	public float GetStarBonusList(CharaType charaType, int star, OverlapBonusType bonusType)
	{
		if (IsSetupEnd())
		{
			CharaType key = charaType;
			if (!m_OverlapBonus.ContainsKey(key))
			{
				key = CharaType.SONIC;
			}
			if (star < m_OverlapBonus[key].Count && OverlapBonusType.SCORE <= bonusType && bonusType < OverlapBonusType.NUM)
			{
				return m_OverlapBonus[key][star][(int)bonusType];
			}
		}
		return 0f;
	}

	public Dictionary<BonusParam.BonusType, float> GetStarBonusList(CharaType charaType, int star)
	{
		Dictionary<BonusParam.BonusType, float> dictionary = null;
		dictionary = new Dictionary<BonusParam.BonusType, float>();
		if (IsSetupEnd())
		{
			CharaType key = charaType;
			if (!m_OverlapBonus.ContainsKey(key))
			{
				key = CharaType.SONIC;
			}
			if (star < m_OverlapBonus[key].Count)
			{
				dictionary.Add(BonusParam.BonusType.SCORE, m_OverlapBonus[key][star][0]);
				dictionary.Add(BonusParam.BonusType.RING, m_OverlapBonus[key][star][1]);
				dictionary.Add(BonusParam.BonusType.ANIMAL, m_OverlapBonus[key][star][2]);
				dictionary.Add(BonusParam.BonusType.DISTANCE, m_OverlapBonus[key][star][3]);
				dictionary.Add(BonusParam.BonusType.ENEMY_OBJBREAK, m_OverlapBonus[key][star][4]);
			}
		}
		return dictionary;
	}

	public Dictionary<BonusParam.BonusType, float> GetStarBonusList(CharaType charaType)
	{
		Dictionary<BonusParam.BonusType, float> result = null;
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null)
		{
			ServerCharacterState serverCharacterState = playerState.CharacterState(charaType);
			if (serverCharacterState != null)
			{
				return GetStarBonusList(charaType, serverCharacterState.star);
			}
		}
		return result;
	}
}
