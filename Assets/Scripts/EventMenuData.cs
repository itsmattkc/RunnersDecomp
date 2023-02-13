using System;

public class EventMenuData : IComparable
{
	public WindowEventData[] window_data
	{
		get;
		set;
	}

	public EventStageData stage_data
	{
		get;
		set;
	}

	public EventChaoData chao_data
	{
		get;
		set;
	}

	public EventProductionData puduction_data
	{
		get;
		set;
	}

	public EventRaidProductionData raid_data
	{
		get;
		set;
	}

	public EventAvertData advert_data
	{
		get;
		set;
	}

	public EventMenuData()
	{
	}

	public EventMenuData(WindowEventData[] window_data, EventStageData stage_data, EventChaoData chao_data, EventProductionData puduction_data, EventRaidProductionData raid_data, EventAvertData advert_data)
	{
		this.stage_data = stage_data;
		this.window_data = window_data;
		this.chao_data = chao_data;
		this.puduction_data = puduction_data;
		this.raid_data = raid_data;
		this.advert_data = advert_data;
	}

	public int CompareTo(object obj)
	{
		if (this == (EventMenuData)obj)
		{
			return 0;
		}
		return -1;
	}
}
