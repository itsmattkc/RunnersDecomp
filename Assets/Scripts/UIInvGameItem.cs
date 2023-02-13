using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIInvGameItem
{
	public enum Quality
	{
		Broken,
		Cursed,
		Damaged,
		Worn,
		Sturdy,
		Polished,
		Improved,
		Crafted,
		Superior,
		Enchanted,
		Epic,
		Legendary,
		_LastDoNotUse
	}

	[SerializeField]
	private int mBaseItemID;

	public Quality quality = Quality.Sturdy;

	public int itemLevel = 1;

	private UIInvBaseItem mBaseItem;

	public int baseItemID
	{
		get
		{
			return mBaseItemID;
		}
	}

	public UIInvBaseItem baseItem
	{
		get
		{
			if (mBaseItem == null)
			{
				mBaseItem = UIInvDatabase.FindByID(baseItemID);
			}
			return mBaseItem;
		}
	}

	public string name
	{
		get
		{
			if (baseItem == null)
			{
				return null;
			}
			return quality.ToString() + " " + baseItem.name;
		}
	}

	public float statMultiplier
	{
		get
		{
			float num = 0f;
			switch (quality)
			{
			case Quality.Cursed:
				num = -1f;
				break;
			case Quality.Broken:
				num = 0f;
				break;
			case Quality.Damaged:
				num = 0.25f;
				break;
			case Quality.Worn:
				num = 0.9f;
				break;
			case Quality.Sturdy:
				num = 1f;
				break;
			case Quality.Polished:
				num = 1.1f;
				break;
			case Quality.Improved:
				num = 1.25f;
				break;
			case Quality.Crafted:
				num = 1.5f;
				break;
			case Quality.Superior:
				num = 1.75f;
				break;
			case Quality.Enchanted:
				num = 2f;
				break;
			case Quality.Epic:
				num = 2.5f;
				break;
			case Quality.Legendary:
				num = 3f;
				break;
			}
			float num2 = (float)itemLevel / 50f;
			return num * Mathf.Lerp(num2, num2 * num2, 0.5f);
		}
	}

	public Color color
	{
		get
		{
			Color result = Color.white;
			switch (quality)
			{
			case Quality.Cursed:
				result = Color.red;
				break;
			case Quality.Broken:
				result = new Color(0.4f, 0.2f, 0.2f);
				break;
			case Quality.Damaged:
				result = new Color(0.4f, 0.4f, 0.4f);
				break;
			case Quality.Worn:
				result = new Color(0.7f, 0.7f, 0.7f);
				break;
			case Quality.Sturdy:
				result = new Color(1f, 1f, 1f);
				break;
			case Quality.Polished:
				result = NGUIMath.HexToColor(3774856959u);
				break;
			case Quality.Improved:
				result = NGUIMath.HexToColor(2480359935u);
				break;
			case Quality.Crafted:
				result = NGUIMath.HexToColor(1325334783u);
				break;
			case Quality.Superior:
				result = NGUIMath.HexToColor(12255231u);
				break;
			case Quality.Enchanted:
				result = NGUIMath.HexToColor(1937178111u);
				break;
			case Quality.Epic:
				result = NGUIMath.HexToColor(2516647935u);
				break;
			case Quality.Legendary:
				result = NGUIMath.HexToColor(4287627519u);
				break;
			}
			return result;
		}
	}

	public UIInvGameItem(int id)
	{
		mBaseItemID = id;
	}

	public UIInvGameItem(int id, UIInvBaseItem bi)
	{
		mBaseItemID = id;
		mBaseItem = bi;
	}

	public List<UIInvStat> CalculateStats()
	{
		List<UIInvStat> list = new List<UIInvStat>();
		if (baseItem != null)
		{
			float statMultiplier = this.statMultiplier;
			List<UIInvStat> stats = baseItem.stats;
			int i = 0;
			for (int count = stats.Count; i < count; i++)
			{
				UIInvStat uIInvStat = stats[i];
				int num = Mathf.RoundToInt(statMultiplier * (float)uIInvStat.amount);
				if (num == 0)
				{
					continue;
				}
				bool flag = false;
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					UIInvStat uIInvStat2 = list[j];
					if (uIInvStat2.id == uIInvStat.id && uIInvStat2.modifier == uIInvStat.modifier)
					{
						uIInvStat2.amount += num;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					UIInvStat uIInvStat3 = new UIInvStat();
					uIInvStat3.id = uIInvStat.id;
					uIInvStat3.amount = num;
					uIInvStat3.modifier = uIInvStat.modifier;
					list.Add(uIInvStat3);
				}
			}
			list.Sort(UIInvStat.CompareArmor);
		}
		return list;
	}
}
