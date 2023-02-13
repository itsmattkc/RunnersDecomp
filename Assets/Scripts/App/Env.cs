namespace App
{
	public class Env
	{
		public enum Region
		{
			JAPAN,
			WORLDWIDE
		}

		public enum Language
		{
			JAPANESE,
			ENGLISH,
			CHINESE_ZHJ,
			CHINESE_ZH,
			KOREAN,
			FRENCH,
			GERMAN,
			SPANISH,
			PORTUGUESE,
			ITALIAN,
			RUSSIAN
		}

		public enum ActionServerType
		{
			LOCAL1,
			LOCAL2,
			LOCAL3,
			LOCAL4,
			LOCAL5,
			DEVELOP,
			DEVELOP2,
			DEVELOP3,
			STAGING,
			RELEASE,
			APPLICATION
		}

		public const bool isDebug = false;

		public const bool isDebugFont = false;

		public const bool forDevelop = false;

		private static readonly bool m_useAssetBundle = true;

		private static readonly bool m_releaseApplication;

		private static Region mRegion;

		private static Language mLanguage;

		private static ActionServerType mActionServerType = ActionServerType.RELEASE;

		public static bool useAssetBundle
		{
			get
			{
				return m_useAssetBundle;
			}
		}

		public static bool isReleaseApplication
		{
			get
			{
				return m_releaseApplication;
			}
		}

		public static Region region
		{
			get
			{
				return mRegion;
			}
			set
			{
				mRegion = value;
			}
		}

		public static Language language
		{
			get
			{
				return mLanguage;
			}
			set
			{
				mLanguage = value;
			}
		}

		public static ActionServerType actionServerType
		{
			get
			{
				return mActionServerType;
			}
			set
			{
				mActionServerType = value;
			}
		}
	}
}
