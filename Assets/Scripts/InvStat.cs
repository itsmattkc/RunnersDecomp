using System;

[Serializable]
public class InvStat
{
	public enum Identifier
	{
		Strength,
		Constitution,
		Agility,
		Intelligence,
		Damage,
		Crit,
		Armor,
		Health,
		Mana,
		Other
	}

	public enum Modifier
	{
		Added,
		Percent
	}

	public Identifier id;

	public Modifier modifier;

	public int amount;

	public static string GetName(Identifier i)
	{
		return i.ToString();
	}

	public static string GetDescription(Identifier i)
	{
		switch (i)
		{
		case Identifier.Strength:
			return "Strength increases melee damage";
		case Identifier.Constitution:
			return "Constitution increases health";
		case Identifier.Agility:
			return "Agility increases armor";
		case Identifier.Intelligence:
			return "Intelligence increases mana";
		case Identifier.Damage:
			return "Damage adds to the amount of damage done in combat";
		case Identifier.Crit:
			return "Crit increases the chance of landing a critical strike";
		case Identifier.Armor:
			return "Armor protects from damage";
		case Identifier.Health:
			return "Health prolongs life";
		case Identifier.Mana:
			return "Mana increases the number of spells that can be cast";
		default:
			return null;
		}
	}

	public static int CompareArmor(InvStat a, InvStat b)
	{
		int num = (int)a.id;
		int num2 = (int)b.id;
		if (a.id == Identifier.Armor)
		{
			num -= 10000;
		}
		else if (a.id == Identifier.Damage)
		{
			num -= 5000;
		}
		if (b.id == Identifier.Armor)
		{
			num2 -= 10000;
		}
		else if (b.id == Identifier.Damage)
		{
			num2 -= 5000;
		}
		if (a.amount < 0)
		{
			num += 1000;
		}
		if (b.amount < 0)
		{
			num2 += 1000;
		}
		if (a.modifier == Modifier.Percent)
		{
			num += 100;
		}
		if (b.modifier == Modifier.Percent)
		{
			num2 += 100;
		}
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		return 0;
	}

	public static int CompareWeapon(InvStat a, InvStat b)
	{
		int num = (int)a.id;
		int num2 = (int)b.id;
		if (a.id == Identifier.Damage)
		{
			num -= 10000;
		}
		else if (a.id == Identifier.Armor)
		{
			num -= 5000;
		}
		if (b.id == Identifier.Damage)
		{
			num2 -= 10000;
		}
		else if (b.id == Identifier.Armor)
		{
			num2 -= 5000;
		}
		if (a.amount < 0)
		{
			num += 1000;
		}
		if (b.amount < 0)
		{
			num2 += 1000;
		}
		if (a.modifier == Modifier.Percent)
		{
			num += 100;
		}
		if (b.modifier == Modifier.Percent)
		{
			num2 += 100;
		}
		if (num < num2)
		{
			return -1;
		}
		if (num > num2)
		{
			return 1;
		}
		return 0;
	}
}
