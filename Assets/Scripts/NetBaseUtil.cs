using App;
using SaveData;

internal class NetBaseUtil
{
	private static string m_debugServerUrl = null;

	private static string m_assetServerVersionUrl = string.Empty;

	private static string[] mActionServerUrlTable = new string[11]
	{
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/"
	};

	private static string[] mSecureActionServerUrlTable = new string[11]
	{
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/",
		"http://83.162.239.178:9001/"
	};

	private static string[] mServerTypeStringTable = new string[11]
	{
		"_L1",
		"_L2",
		"_L3",
		"_L4",
		"_L5",
		"_D1",
		"_D2",
		"_D3",
		"_S",
		string.Empty,
		"a"
	};

	private static string[] mAssetURLTable = new string[11]
	{
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/",
		"http://sanic.uk.to:9002/assets/"
	};

	private static string[] mInformationURLTable = new string[11]
	{
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/",
		"http://sanic.uk.to:9002/information/"
	};

	private static string mRedirectInstallPageUrl = "https://play.google.com/store/apps/details?id=com.sega.sonicrunners";

	public static string DebugServerUrl
	{
		get
		{
			return m_debugServerUrl;
		}
		set
		{
			m_debugServerUrl = value;
		}
	}

	public static bool IsDebugServer
	{
		get
		{
			if (m_debugServerUrl != null)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public static string ActionServerURL
	{
		get
		{
			if (IsDebugServer)
			{
				return DebugServerUrl;
			}
			return mActionServerUrlTable[(int)Env.actionServerType];
		}
	}

	public static string SecureActionServerURL
	{
		get
		{
			if (IsDebugServer)
			{
				return DebugServerUrl;
			}
			return mSecureActionServerUrlTable[(int)Env.actionServerType];
		}
	}

	public static string ServerTypeString
	{
		get
		{
			if (IsDebugServer)
			{
				return "i";
			}
			return mServerTypeStringTable[(int)Env.actionServerType];
		}
	}

	public static string AssetServerURL
	{
		get
		{
			if (m_assetServerVersionUrl != string.Empty)
			{
				return m_assetServerVersionUrl;
			}
			return mAssetURLTable[(int)Env.actionServerType];
		}
	}

	public static string InformationServerURL
	{
		get
		{
			ServerLoginState loginState = ServerInterface.LoginState;
			string text = string.Empty;
			if (loginState != null)
			{
				text = loginState.InfoVersionString;
			}
			string text2 = mInformationURLTable[(int)Env.actionServerType];
			if (text != string.Empty)
			{
				return text2 + text + "/";
			}
			return text2;
		}
	}

	public static string RedirectInstallPageUrl
	{
		get
		{
			return mRedirectInstallPageUrl;
		}
		private set
		{
		}
	}

	public static string RedirectTrmsOfServicePageUrlForTitle
	{
		get
		{
			if (Env.language == Env.Language.JAPANESE)
			{
				return "http://sonicrunners.sega-net.com/jp/rule/";
			}
			return "http://www.sega.com/legal";
		}
	}

	public static string RedirectTrmsOfServicePageUrl
	{
		get
		{
			if (RegionManager.Instance != null && RegionManager.Instance.IsJapan())
			{
				return "http://sonicrunners.sega-net.com/jp/rule/";
			}
			return "http://sonicrunners.sega-net.com/rule.html";
		}
	}

	public static void SetAssetServerURL()
	{
		ServerLoginState loginState = ServerInterface.LoginState;
		string text = string.Empty;
		if (loginState != null)
		{
			text = loginState.AssetsVersionString;
		}
		string text2 = mAssetURLTable[(int)Env.actionServerType];
		if (text != string.Empty)
		{
			m_assetServerVersionUrl = text2 + CurrentBundleVersion.version + "_" + text + "/";
		}
		else
		{
			m_assetServerVersionUrl = text2;
		}
		if (SystemSaveManager.GetSystemSaveData() != null && SystemSaveManager.GetSystemSaveData().highTexture)
		{
			m_assetServerVersionUrl += "tablet/";
		}
	}

	public static int GetVersionValue(string versionString, int scaleOffset)
	{
		string[] array = versionString.Split('.');
		int num = array.Length;
		int[] array2 = new int[num];
		bool flag = true;
		for (int i = 0; i < num; i++)
		{
			if (!int.TryParse(array[i], out array2[i]))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			int num2 = 0;
			int num3 = 1;
			for (int num4 = num - 1; num4 >= 0; num4--)
			{
				num2 += array2[num4] * num3;
				num3 *= scaleOffset;
			}
			return num2;
		}
		return -1;
	}

	public static int GetVersionValue(string versionString)
	{
		return GetVersionValue(versionString, 10);
	}
}
