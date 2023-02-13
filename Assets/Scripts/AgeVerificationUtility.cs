using System;
using UnityEngine;

public class AgeVerificationUtility : MonoBehaviour
{
	public static bool IsValidDate(int year, int month, int day)
	{
		DateTime dateTime;
		try
		{
			dateTime = new DateTime(1900, 1, 1);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			Debug.Log(ex.ToString());
			dateTime = DateTime.MinValue;
		}
		DateTime now = DateTime.Now;
		if (year < dateTime.Year)
		{
			return false;
		}
		if (year > now.Year)
		{
			return false;
		}
		int num = DateTime.DaysInMonth(year, month);
		if (day > num)
		{
			return false;
		}
		DateTime t = new DateTime(year, month, day);
		int num2 = DateTime.Compare(t, now);
		if (num2 > 0)
		{
			return false;
		}
		return true;
	}

	public static DateTime CalcDateTime(int year, int month, int day, int deltaYear, int deltaMonth, int deltaDay, bool dummy)
	{
		DateTime dateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
		DateTime dateTime2 = dateTime;
		DateTime dateTime3 = dateTime;
		DateTime dateTime4 = dateTime;
		try
		{
			dateTime3 = new DateTime(1900, 1, 1);
			dateTime4 = DateTime.Now;
		}
		catch (ArgumentOutOfRangeException ex)
		{
			Debug.Log("AgeVerificationUtility" + ex.ToString());
			dateTime3 = DateTime.MinValue;
			dateTime4 = DateTime.MaxValue;
		}
		try
		{
			dateTime2 = dateTime.AddYears(deltaYear).AddMonths(deltaMonth).AddDays(deltaDay);
			if (dateTime2 > dateTime4)
			{
				return dateTime4;
			}
			if (dateTime2 < dateTime3)
			{
				return dateTime3;
			}
			return dateTime2;
		}
		catch (ArgumentOutOfRangeException ex2)
		{
			Debug.Log("AgeVerificationUtility" + ex2.ToString());
			return dateTime;
		}
	}
}
