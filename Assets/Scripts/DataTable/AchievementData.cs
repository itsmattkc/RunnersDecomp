using System.Runtime.CompilerServices;

namespace DataTable
{
	public class AchievementData
	{
		public enum Type
		{
			ANIMAL = 0,
			DISTANCE = 1,
			PLAYER_OPEN = 2,
			PLAYER_LEVEL = 3,
			CHAO_OPEN = 4,
			CHAO_LEVEL = 5,
			COUNT = 6,
			NONE = -1
		}

		public const int ID_MIN_VALUE = 1;

		public int number
		{
			get;
			set;
		}

		public string explanation
		{
			get;
			set;
		}

		public Type type
		{
			get;
			set;
		}

		public int itemID
		{
			get;
			set;
		}

		public int value
		{
			get;
			set;
		}

		public string iosId
		{
			get;
			set;
		}

		public string androidId
		{
			get;
			set;
		}

		public AchievementData()
		{
		}

		public AchievementData(int number, string explanation, Type type, int itemID, int value, string iosId, string androidId)
		{
			this.number = number;
			this.explanation = explanation;
			this.type = type;
			this.itemID = itemID;
			this.value = value;
			this.iosId = iosId;
			this.androidId = androidId;
		}

		public string GetID()
		{
			return androidId;
		}
	}
}
