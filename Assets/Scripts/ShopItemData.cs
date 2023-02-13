public class ShopItemData
{
	public const int ID_NONE = -1;

	public const int ID_ORIGIN = 0;

	public int number
	{
		get;
		set;
	}

	public string name
	{
		get;
		private set;
	}

	public int rings
	{
		get;
		set;
	}

	public string details
	{
		get;
		private set;
	}

	public int id
	{
		get
		{
			return number - 1;
		}
	}

	public int index
	{
		get
		{
			return id;
		}
	}

	public bool IsValidate
	{
		get
		{
			return id != -1;
		}
	}

	public void SetName(string name)
	{
		this.name = name;
	}

	public void SetDetails(string details)
	{
		this.details = details;
	}
}
