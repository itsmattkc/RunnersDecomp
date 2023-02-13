namespace DataTable
{
	public class MissionData
	{
		public enum Type
		{
			ENEMY = 0,
			G_ENEMY = 1,
			DISTANCE = 2,
			ANIMAL = 3,
			SCORE = 4,
			RING = 5,
			COUNT = 6,
			NONE = -1
		}

		public const int ID_MIN_VALUE = 1;

		public int id
		{
			get;
			set;
		}

		public Type type
		{
			get;
			set;
		}

		public string text
		{
			get;
			set;
		}

		public int quota
		{
			get;
			set;
		}

		public bool save
		{
			get;
			set;
		}

		public MissionData()
		{
		}

		public MissionData(int id, Type type, string text, int quota, bool save)
		{
			this.id = id;
			this.type = type;
			this.text = text;
			this.quota = quota;
			this.save = save;
		}

		public void SetText(string text)
		{
			this.text = text;
		}
	}
}
