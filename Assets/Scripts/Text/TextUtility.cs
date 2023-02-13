using App;
using System.Collections.Generic;

namespace Text
{
	public class TextUtility
	{
		public static string Replaces(string text, Dictionary<string, string> replaceDic)
		{
			if (text == null)
			{
				Debug.LogWarning("TextUtility.Replaces() first argument is null");
				return string.Empty;
			}
			foreach (string key in replaceDic.Keys)
			{
				text = text.Replace(key, replaceDic[key]);
			}
			return text;
		}

		public static string Replace(string text, string srcText, string dstText)
		{
			return Replaces(text, new Dictionary<string, string>
			{
				{
					srcText,
					dstText
				}
			});
		}

		public static void SetText(UILabel label, TextManager.TextType type, string groupID, string cellID)
		{
			if (label != null)
			{
				TextObject text = TextManager.GetText(type, groupID, cellID);
				if (text != null && text.text != null)
				{
					label.text = text.text;
				}
			}
		}

		public static void SetText(UILabel label, TextManager.TextType type, string groupID, string cellID, string tag, string replace)
		{
			if (label != null)
			{
				TextObject text = TextManager.GetText(type, groupID, cellID);
				if (text != null && text.text != null)
				{
					text.ReplaceTag(tag, replace);
					label.text = text.text;
				}
			}
		}

		public static void SetCommonText(UILabel label, string groupID, string cellID)
		{
			SetText(label, TextManager.TextType.TEXTTYPE_COMMON_TEXT, groupID, cellID);
		}

		public static void SetCommonText(UILabel label, string groupID, string cellID, string tag, string replace)
		{
			SetText(label, TextManager.TextType.TEXTTYPE_COMMON_TEXT, groupID, cellID, tag, replace);
		}

		public static string GetText(TextManager.TextType type, string groupID, string cellID)
		{
			TextObject text = TextManager.GetText(type, groupID, cellID);
			if (text != null && text.text != null)
			{
				return text.text;
			}
			return null;
		}

		public static string GetText(TextManager.TextType type, string groupID, string cellID, string tag, string replace)
		{
			TextObject text = TextManager.GetText(type, groupID, cellID);
			if (text != null && text.text != null)
			{
				text.ReplaceTag(tag, replace);
				return text.text;
			}
			return null;
		}

		public static string GetCommonText(string groupID, string cellID)
		{
			return GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, groupID, cellID);
		}

		public static string GetCommonText(string groupID, string cellID, string tag, string replace)
		{
			return GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, groupID, cellID, tag, replace);
		}

		public static string GetChaoText(string groupID, string cellID)
		{
			return GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, groupID, cellID);
		}

		public static string GetChaoText(string groupID, string cellID, string tag, string replace)
		{
			return GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, groupID, cellID, tag, replace);
		}

		public static string GetXmlLanguageDataType()
		{
			switch (Env.language)
			{
			case Env.Language.JAPANESE:
				return "Japanese";
			case Env.Language.ENGLISH:
				return "English";
			case Env.Language.GERMAN:
				return "German";
			case Env.Language.SPANISH:
				return "Spanish";
			case Env.Language.FRENCH:
				return "French";
			case Env.Language.ITALIAN:
				return "Italian";
			case Env.Language.KOREAN:
				return "Korean";
			case Env.Language.PORTUGUESE:
				return "Portuguese";
			case Env.Language.RUSSIAN:
				return "Russian";
			case Env.Language.CHINESE_ZHJ:
				return "SimplifiedChinese";
			case Env.Language.CHINESE_ZH:
				return "TraditionalChinese";
			default:
				return "English";
			}
		}

		public static string GetSuffixe()
		{
			switch (Env.language)
			{
			case Env.Language.JAPANESE:
				return "ja";
			case Env.Language.ENGLISH:
				return "en";
			case Env.Language.GERMAN:
				return "de";
			case Env.Language.SPANISH:
				return "es";
			case Env.Language.FRENCH:
				return "fr";
			case Env.Language.ITALIAN:
				return "it";
			case Env.Language.KOREAN:
				return "ko";
			case Env.Language.PORTUGUESE:
				return "pt";
			case Env.Language.RUSSIAN:
				return "ru";
			case Env.Language.CHINESE_ZHJ:
				return "zhj";
			case Env.Language.CHINESE_ZH:
				return "zh";
			default:
				return "en";
			}
		}

		public static string GetSuffix(Env.Language language)
		{
			switch (language)
			{
			case Env.Language.JAPANESE:
				return "ja";
			case Env.Language.ENGLISH:
				return "en";
			case Env.Language.GERMAN:
				return "de";
			case Env.Language.SPANISH:
				return "es";
			case Env.Language.FRENCH:
				return "fr";
			case Env.Language.ITALIAN:
				return "it";
			case Env.Language.KOREAN:
				return "ko";
			case Env.Language.PORTUGUESE:
				return "pt";
			case Env.Language.RUSSIAN:
				return "ru";
			case Env.Language.CHINESE_ZHJ:
				return "zhj";
			case Env.Language.CHINESE_ZH:
				return "zh";
			default:
				return "en";
			}
		}

		public static bool IsSuffix(string languageCode)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(languageCode))
			{
				switch (languageCode)
				{
				case "ja":
					result = true;
					break;
				case "en":
					result = true;
					break;
				case "de":
					result = true;
					break;
				case "es":
					result = true;
					break;
				case "fr":
					result = true;
					break;
				case "it":
					result = true;
					break;
				case "ko":
					result = true;
					break;
				case "pt":
					result = true;
					break;
				case "ru":
					result = true;
					break;
				case "zhj":
					result = true;
					break;
				case "zh":
					result = true;
					break;
				case "JA":
					result = true;
					break;
				case "EN":
					result = true;
					break;
				case "DE":
					result = true;
					break;
				case "ES":
					result = true;
					break;
				case "FR":
					result = true;
					break;
				case "IT":
					result = true;
					break;
				case "KO":
					result = true;
					break;
				case "PT":
					result = true;
					break;
				case "RU":
					result = true;
					break;
				case "ZHJ":
					result = true;
					break;
				case "ZH":
					result = true;
					break;
				}
			}
			return result;
		}

		public static string GetTextLevel(string levelNumber)
		{
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
			return Replace(text, "{PARAM}", levelNumber);
		}
	}
}
