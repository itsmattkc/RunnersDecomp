using System;

public class ServerWheelSpinInfo
{
	public int id
	{
		get;
		set;
	}

	public DateTime start
	{
		get;
		set;
	}

	public DateTime end
	{
		get;
		set;
	}

	public string param
	{
		get;
		set;
	}

	public bool isEnabled
	{
		get
		{
			bool result = false;
			if (NetBase.GetCurrentTime() >= start && NetBase.GetCurrentTime() < end)
			{
				result = true;
			}
			return result;
		}
	}

	public ServerWheelSpinInfo()
	{
		id = 1;
		start = NetBase.GetCurrentTime();
		end = NetBase.GetCurrentTime();
		param = string.Empty;
	}

	public void Dump()
	{
	}

	public void CopyTo(ServerWheelSpinInfo to)
	{
		to.id = id;
		to.start = start;
		to.end = end;
		to.param = param;
	}
}
