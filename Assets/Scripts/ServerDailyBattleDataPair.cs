using System;

public class ServerDailyBattleDataPair
{
	public bool isDummyData;

	public DateTime starTime;

	public DateTime endTime;

	public ServerDailyBattleData myBattleData;

	public ServerDailyBattleData rivalBattleData;

	public string starDateString
	{
		get
		{
			return GeneralUtil.GetDateString(starTime);
		}
	}

	public string endDateString
	{
		get
		{
			return GeneralUtil.GetDateString(endTime);
		}
	}

	public bool isToday
	{
		get
		{
			bool result = false;
			if (starTime.Ticks != endTime.Ticks && !isDummyData)
			{
				DateTime currentTime = NetBase.GetCurrentTime();
				if (endTime.Ticks >= currentTime.Ticks)
				{
					result = true;
				}
			}
			return result;
		}
	}

	public int goOnWin
	{
		get
		{
			int result = 0;
			if (myBattleData != null && !string.IsNullOrEmpty(myBattleData.userId))
			{
				result = myBattleData.goOnWin;
			}
			return result;
		}
	}

	public int winFlag
	{
		get
		{
			int result = 0;
			if (myBattleData != null && !string.IsNullOrEmpty(myBattleData.userId))
			{
				result = ((rivalBattleData == null || string.IsNullOrEmpty(rivalBattleData.userId)) ? 4 : ((myBattleData.maxScore > rivalBattleData.maxScore) ? 3 : ((myBattleData.maxScore != rivalBattleData.maxScore) ? 1 : 2)));
			}
			return result;
		}
	}

	public ServerDailyBattleDataPair()
	{
		isDummyData = false;
		starTime = NetBase.GetCurrentTime();
		endTime = NetBase.GetCurrentTime();
		myBattleData = new ServerDailyBattleData();
		rivalBattleData = new ServerDailyBattleData();
	}

	public ServerDailyBattleDataPair(ServerDailyBattleDataPair data)
	{
		isDummyData = data.isDummyData;
		starTime = data.starTime;
		endTime = data.endTime;
		myBattleData = new ServerDailyBattleData();
		rivalBattleData = new ServerDailyBattleData();
		data.myBattleData.CopyTo(myBattleData);
		data.rivalBattleData.CopyTo(rivalBattleData);
	}

	public ServerDailyBattleDataPair(DateTime start, DateTime end)
	{
		isDummyData = true;
		starTime = start;
		endTime = end;
		myBattleData = new ServerDailyBattleData();
		rivalBattleData = new ServerDailyBattleData();
	}

	public void Dump()
	{
		myBattleData.Dump();
		rivalBattleData.Dump();
	}

	public void CopyTo(ServerDailyBattleDataPair dest)
	{
		dest.isDummyData = isDummyData;
		dest.starTime = starTime;
		dest.endTime = endTime;
		myBattleData.CopyTo(dest.myBattleData);
		rivalBattleData.CopyTo(dest.rivalBattleData);
	}
}
