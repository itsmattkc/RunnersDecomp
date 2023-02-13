public class ServerChaoWheelOptions
{
	public enum ChaoSpinType
	{
		Normal,
		Special,
		NUM
	}

	private int m_multi;

	public int[] Rarities
	{
		get;
		set;
	}

	public int[] ItemWeight
	{
		get;
		set;
	}

	public int Cost
	{
		get;
		set;
	}

	public ChaoSpinType SpinType
	{
		get;
		set;
	}

	public int NumSpecialEggs
	{
		get;
		set;
	}

	public bool IsValid
	{
		get;
		set;
	}

	public int NumRouletteToken
	{
		get;
		set;
	}

	public bool IsTutorial
	{
		get;
		set;
	}

	public bool IsConnected
	{
		get;
		set;
	}

	public int multi
	{
		get
		{
			return m_multi;
		}
	}

	public ServerChaoWheelOptions()
	{
		Rarities = new int[8];
		ItemWeight = new int[8];
		for (int i = 0; i < 8; i++)
		{
			Rarities[i] = 0;
			ItemWeight[i] = 0;
		}
		m_multi = 1;
		Cost = 0;
		SpinType = ChaoSpinType.Normal;
		NumSpecialEggs = 0;
		IsValid = true;
		NumRouletteToken = 0;
		IsTutorial = false;
		IsConnected = false;
	}

	public bool ChangeMulti(int multi)
	{
		bool flag = false;
		flag = IsMulti(multi);
		if (flag)
		{
			m_multi = multi;
			if (m_multi < 1)
			{
				m_multi = 1;
			}
		}
		else
		{
			m_multi = 1;
		}
		return flag;
	}

	public bool IsMulti(int multi)
	{
		bool result = false;
		if (multi <= 1)
		{
			result = true;
		}
		else
		{
			int cost = Cost;
			int num = 0;
			if (NumRouletteToken > 0 && NumRouletteToken >= Cost)
			{
				num = NumRouletteToken;
				cost = Cost;
			}
			else
			{
				num = (int)SaveDataManager.Instance.ItemData.RedRingCount;
				cost = Cost;
			}
			if (num >= cost * multi)
			{
				result = true;
			}
		}
		return result;
	}

	public void Dump()
	{
	}

	public void CopyTo(ServerChaoWheelOptions to)
	{
		to.Cost = Cost;
		to.SpinType = SpinType;
		to.Rarities = (Rarities.Clone() as int[]);
		to.ItemWeight = (ItemWeight.Clone() as int[]);
		to.NumSpecialEggs = NumSpecialEggs;
		to.IsValid = IsValid;
		to.NumRouletteToken = NumRouletteToken;
		to.IsTutorial = IsTutorial;
		to.IsConnected = IsConnected;
		to.m_multi = m_multi;
	}
}
