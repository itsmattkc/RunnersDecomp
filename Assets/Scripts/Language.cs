using App;
using UnityEngine;

public class Language
{
	public static string GetLocalLanguage()
	{
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			//Discarded unreachable code: IL_0036, IL_0048
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
				{
					return androidJavaObject.Call<string>("getLanguage", new object[0]);
				}
			}
		}
#endif
		return "en"; // fallback
	}

	private static string GetLocale()
	{
#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			//Discarded unreachable code: IL_0034, IL_0046
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]))
				{
					return androidJavaObject.Call<string>("toString", new object[0]);
				}
			}
		}
#endif
		return "en_US"; // fallback
	}

	private static void SetEditorLanguage()
	{
		Env.language = Env.Language.ENGLISH;
	}

	private static void SetIPhoneLanguage()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Japanese:
			Env.language = Env.Language.JAPANESE;
			break;
		case SystemLanguage.English:
			Env.language = Env.Language.ENGLISH;
			break;
		case SystemLanguage.German:
			Env.language = Env.Language.GERMAN;
			break;
		case SystemLanguage.Spanish:
			Env.language = Env.Language.SPANISH;
			break;
		case SystemLanguage.French:
			Env.language = Env.Language.FRENCH;
			break;
		case SystemLanguage.Italian:
			Env.language = Env.Language.ITALIAN;
			break;
		case SystemLanguage.Korean:
			Env.language = Env.Language.KOREAN;
			break;
		case SystemLanguage.Portuguese:
			Env.language = Env.Language.PORTUGUESE;
			break;
		case SystemLanguage.Russian:
			Env.language = Env.Language.RUSSIAN;
			break;
		case SystemLanguage.Chinese:
		{
			string localLanguage = GetLocalLanguage();
			Debug.Log("Language.InitAppEnvLanguage() GetLocalLanguage = " + localLanguage);
			if (localLanguage.Contains("zh-Hans"))
			{
				Env.language = Env.Language.CHINESE_ZHJ;
			}
			else
			{
				Env.language = Env.Language.CHINESE_ZH;
			}
			break;
		}
		default:
			Env.language = Env.Language.ENGLISH;
			break;
		}
	}

	private static void SetAndroidLanguage()
	{
		switch (GetLocalLanguage())
		{
		case "ja":
			Env.language = Env.Language.JAPANESE;
			break;
		case "en":
			Env.language = Env.Language.ENGLISH;
			break;
		case "de":
			Env.language = Env.Language.GERMAN;
			break;
		case "es":
			Env.language = Env.Language.SPANISH;
			break;
		case "fr":
			Env.language = Env.Language.FRENCH;
			break;
		case "it":
			Env.language = Env.Language.ITALIAN;
			break;
		case "ko":
			Env.language = Env.Language.KOREAN;
			break;
		case "pt":
			Env.language = Env.Language.PORTUGUESE;
			break;
		case "ru":
			Env.language = Env.Language.RUSSIAN;
			break;
		case "zh":
		{
			Env.language = Env.Language.CHINESE_ZHJ;
			string locale = GetLocale();
			if (locale != null && locale == "zh_TW")
			{
				Env.language = Env.Language.CHINESE_ZH;
			}
			break;
		}
		default:
			Env.language = Env.Language.ENGLISH;
			break;
		}
	}

	public static void InitAppEnvLanguage()
	{
#if UNITY_EDITOR
		SetEditorLanguage();
#elif UNITY_ANDROID
		SetAndroidLanguage();
#else
		SetIPhoneLanguage(); // for iPhone and all other platforms
#endif
	}
}
