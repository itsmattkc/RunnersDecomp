using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InvBaseItem
{
	public enum Slot
	{
		None,
		Weapon,
		Shield,
		Body,
		Shoulders,
		Bracers,
		Boots,
		Trinket,
		_LastDoNotUse
	}

	public int id16;

	public string name;

	public string description;

	public Slot slot;

	public int minItemLevel = 1;

	public int maxItemLevel = 50;

	public List<InvStat> stats = new List<InvStat>();

	public GameObject attachment;

	public Color color = Color.white;

	public UIAtlas iconAtlas;

	public string iconName = string.Empty;
}
