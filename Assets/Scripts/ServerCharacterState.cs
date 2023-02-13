using System.Collections.Generic;
using Text;
using UnityEngine;

public class ServerCharacterState
{
	public enum CharacterStatus
	{
		Locked,
		Unlocked,
		MaxLevel
	}

	public enum LockCondition
	{
		OPEN,
		MILEAGE_EPISODE,
		RING_OR_RED_STAR_RING,
		ROULETTE
	}

	public int m_currentUnlocks;

	public int m_numTokens;

	public int m_tokenCost;

	public List<int> AbilityLevel = new List<int>();

	public List<int> OldAbiltyLevel = new List<int>();

	public List<int> AbilityNumRings = new List<int>();

	public int Id
	{
		get;
		set;
	}

	public CharacterStatus Status
	{
		get;
		set;
	}

	public CharacterStatus OldStatus
	{
		get;
		set;
	}

	public int Level
	{
		get;
		set;
	}

	public int Cost
	{
		get;
		set;
	}

	public int OldCost
	{
		get;
		set;
	}

	public int NumRedRings
	{
		get;
		set;
	}

	public LockCondition Condition
	{
		get;
		set;
	}

	public int Exp
	{
		get;
		set;
	}

	public int OldExp
	{
		get;
		set;
	}

	public int star
	{
		get;
		set;
	}

	public int starMax
	{
		get;
		set;
	}

	public int priceNumRings
	{
		get;
		set;
	}

	public int priceNumRedRings
	{
		get;
		set;
	}

	public CharaType charaType
	{
		get
		{
			CharaType result = CharaType.UNKNOWN;
			if (Id >= 0)
			{
				result = new ServerItem((ServerItem.Id)Id).charaType;
			}
			return result;
		}
	}

	public CharacterDataNameInfo.Info charaInfo
	{
		get
		{
			CharacterDataNameInfo.Info result = null;
			CharacterDataNameInfo instance = CharacterDataNameInfo.Instance;
			if (instance != null)
			{
				result = instance.GetDataByID(charaType);
			}
			return result;
		}
	}

	public bool IsBuy
	{
		get
		{
			bool result = false;
			if (starMax > 0 && star < starMax && (priceNumRings > 0 || priceNumRedRings > 0))
			{
				result = true;
			}
			return result;
		}
	}

	public bool IsRoulette
	{
		get
		{
			bool result = false;
			if (RouletteManager.Instance != null)
			{
				if (Condition == LockCondition.ROULETTE)
				{
					result = true;
				}
				else if (RouletteManager.Instance.IsPicupChara(charaType))
				{
					result = true;
				}
			}
			return result;
		}
	}

	public int UnlockCost
	{
		get
		{
			if (Status == CharacterStatus.Locked)
			{
				return Cost;
			}
			return -1;
		}
	}

	public int LevelUpCost
	{
		get
		{
			if (Status == CharacterStatus.Unlocked)
			{
				return Cost;
			}
			return -1;
		}
	}

	public bool IsUnlocked
	{
		get
		{
			return CharacterStatus.Locked != Status;
		}
	}

	public bool IsMaxLevel
	{
		get
		{
			return CharacterStatus.MaxLevel == Status;
		}
	}

	public float QuickModeTimeExtension
	{
		get
		{
			float result = 0f;
			if (starMax > 0)
			{
				StageTimeTable stageTimeTable = GameObjectUtil.FindGameObjectComponent<StageTimeTable>("StageTimeTable");
				if (stageTimeTable != null)
				{
					float num = stageTimeTable.GetTableItemData(StageTimeTableItem.OverlapBonus);
					result = (float)star * num;
				}
			}
			return result;
		}
	}

	public ServerCharacterState()
	{
		Id = -1;
		Status = CharacterStatus.Locked;
		Level = 10;
		Cost = 0;
		star = 0;
		starMax = 0;
		priceNumRings = 0;
		priceNumRedRings = 0;
	}

	public Dictionary<ServerItem.Id, int> GetBuyCostItemList()
	{
		Dictionary<ServerItem.Id, int> dictionary = null;
		if (IsBuy)
		{
			if (priceNumRings > 0)
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<ServerItem.Id, int>();
				}
				dictionary.Add(ServerItem.Id.RING, priceNumRings);
			}
			if (priceNumRedRings > 0)
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<ServerItem.Id, int>();
				}
				dictionary.Add(ServerItem.Id.RSRING, priceNumRedRings);
			}
		}
		return dictionary;
	}

	public Dictionary<BonusParam.BonusType, float> GetStarBonusList()
	{
		Dictionary<BonusParam.BonusType, float> result = null;
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ETC, "OverlapBonusTable");
		if (gameObject != null)
		{
			OverlapBonusTable component = gameObject.GetComponent<OverlapBonusTable>();
			if (component != null)
			{
				result = component.GetStarBonusList(charaType);
			}
		}
		return result;
	}

	public Dictionary<BonusParam.BonusType, float> GetTeamBonusList()
	{
		Dictionary<BonusParam.BonusType, float> bonusParam = null;
		BonusUtil.GetTeamBonus(charaType, out bonusParam);
		return bonusParam;
	}

	public string GetCharaAttributeText()
	{
		string empty = string.Empty;
		string cellName = "chara_attribute_" + CharaName.Name[(int)charaType];
		if (!IsUnlocked)
		{
			cellName = "recommend_chara_unlock_text_" + CharaName.Name[(int)charaType];
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", cellName).text;
			if (string.IsNullOrEmpty(text))
			{
				List<string> list = new List<string>();
				Dictionary<ServerItem.Id, int> buyCostItemList = GetBuyCostItemList();
				if (IsRoulette)
				{
					list.Add(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "GeneralWindow", "ui_Lbl_roulette").text);
				}
				if (buyCostItemList != null && buyCostItemList.Count > 0)
				{
					Dictionary<ServerItem.Id, int>.KeyCollection keys = buyCostItemList.Keys;
					foreach (ServerItem.Id item in keys)
					{
						list.Add(new ServerItem(item).serverItemName);
					}
				}
				Debug.Log("nameList.Count:" + list.Count);
				if (list.Count > 0)
				{
					int num = list.Count;
					if (num > 3)
					{
						num = 3;
					}
					text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "recommend_chara_unlock_text_type_" + num).text;
					empty = text;
					for (int i = 0; i < num; i++)
					{
						empty = empty.Replace("{PARAM" + (i + 1) + "}", list[i]);
					}
					empty = empty.Replace("{BONUS}", GetTeamBonusText());
				}
				else
				{
					empty = string.Empty;
				}
			}
			else
			{
				empty = text;
				empty = empty.Replace("{BONUS}", GetTeamBonusText());
			}
		}
		else
		{
			empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", cellName).text;
			empty = (string.IsNullOrEmpty(empty) ? "Unknown" : empty.Replace("{BONUS}", GetTeamBonusText()));
		}
		if (starMax > 0 && star > 0)
		{
			string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_overlap_text_2").text;
			float quickModeTimeExtension = QuickModeTimeExtension;
			float num2 = 0f;
			Dictionary<BonusParam.BonusType, float> starBonusList = GetStarBonusList();
			if (starBonusList != null && starBonusList.Count > 0)
			{
				Dictionary<BonusParam.BonusType, float>.KeyCollection keys2 = starBonusList.Keys;
				using (Dictionary<BonusParam.BonusType, float>.KeyCollection.Enumerator enumerator2 = keys2.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						BonusParam.BonusType current2 = enumerator2.Current;
						float num3 = starBonusList[current2];
						num2 = num3;
					}
				}
			}
			text2 = text2.Replace("{TIME}", quickModeTimeExtension.ToString());
			text2 = text2.Replace("{PARAM}", num2.ToString());
			empty = empty + "\n\n" + text2;
		}
		return empty;
	}

	private string GetTeamBonusText()
	{
		string text = string.Empty;
		Dictionary<BonusParam.BonusType, float> teamBonusList = GetTeamBonusList();
		Dictionary<BonusParam.BonusType, float>.KeyCollection keys = teamBonusList.Keys;
		string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "bonus_percent").text;
		foreach (BonusParam.BonusType item in keys)
		{
			Debug.Log("GetTeamBonusText key:" + item);
			float num = teamBonusList[item];
			string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "ui_Lbl_bonusname_" + (int)item).text;
			if (item == BonusParam.BonusType.SPEED)
			{
				text3 = text3 + " " + text2.Replace("{BONUS}", (100f - num).ToString());
			}
			else if (item != BonusParam.BonusType.TOTAL_SCORE || !(Mathf.Abs(num) <= 1f))
			{
				text3 = ((!(num >= 0f)) ? (text3 + " " + text2.Replace("{BONUS}", num.ToString())) : (text3 + " +" + text2.Replace("{BONUS}", num.ToString())));
			}
			else
			{
				num *= 100f;
				text3 = ((!(num >= 0f)) ? (text3 + " " + text2.Replace("{BONUS}", num.ToString())) : (text3 + " +" + text2.Replace("{BONUS}", num.ToString())));
			}
			text = ((!string.IsNullOrEmpty(text)) ? (text + "\n" + text3) : text3);
		}
		return text;
	}

	public bool IsExpGot()
	{
		if (OldExp < Exp)
		{
			return true;
		}
		if (OldAbiltyLevel.Count == AbilityLevel.Count)
		{
			int count = AbilityLevel.Count;
			for (int i = 0; i < count; i++)
			{
				int num = OldAbiltyLevel[i];
				int num2 = AbilityLevel[i];
				if (num < num2)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void Dump()
	{
		Debug.Log(string.Concat("Id=", Id, ", Status=", Status, ", Level=", Level, ", Cost=", Cost, ", UnlockCost=", UnlockCost, ", LevelUpCost=", LevelUpCost));
	}
}
