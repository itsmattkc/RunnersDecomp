using DataTable;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NGWordCheck
{
	private static List<NGWordData> m_wordData = new List<NGWordData>();

	private static bool m_debugDrawLocal = false;

	private static bool m_errorDraw = true;

	private static ResourceSceneLoader m_sceneLoader = null;

	public static void Load()
	{
		if (!(m_sceneLoader == null))
		{
			return;
		}
		string name = "NGWordResourceSceneLoader";
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			gameObject = new GameObject(name);
		}
		if (gameObject != null)
		{
			m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
			bool onAssetBundle = true;
			if (m_sceneLoader.AddLoadAndResourceManager("NGWordTable", onAssetBundle, ResourceCategory.ETC, true, false, null))
			{
				DebugDrawLocal("Load");
			}
		}
	}

	public static bool IsLoaded()
	{
		if (m_sceneLoader != null)
		{
			return m_sceneLoader.Loaded;
		}
		return true;
	}

	public static void ResetData()
	{
		NGWordTable nGWordTable = GameObjectUtil.FindGameObjectComponent<NGWordTable>("NGWordTable");
		if (nGWordTable != null)
		{
			UnityEngine.Object.Destroy(nGWordTable.gameObject);
		}
		if (m_sceneLoader != null)
		{
			UnityEngine.Object.Destroy(m_sceneLoader.gameObject);
		}
		DebugDrawLocal("ResetData");
	}

	public static string Check(string target_word, UILabel uiLabel)
	{
		DebugDrawLocal("check target_word=" + target_word);
		if (isCheckUILabel(target_word, uiLabel))
		{
			ErrorDraw("target_word=" + target_word + " check error UILabel");
			return target_word;
		}
		if (isCheckEmoji(target_word))
		{
			ErrorDraw("target_word=" + target_word + " check error emoji");
			return target_word;
		}
		if (isCheckKisyuIzon(target_word))
		{
			ErrorDraw("target_word=" + target_word + " check error kisyu izon");
			return target_word;
		}
		SetupWordData();
		string text = convertKana(target_word);
		DebugDrawLocal("convertKana=" + text);
		int count = 0;
		string nospace_word = StrReplace(" ", string.Empty, text, ref count);
		return checkProc(text, nospace_word, count);
	}

	private static void SetupWordData()
	{
		if (m_wordData.Count == 0)
		{
			NGWordData[] dataTable = NGWordTable.GetDataTable();
			if (dataTable != null)
			{
				NGWordData[] array = dataTable;
				foreach (NGWordData item in array)
				{
					m_wordData.Add(item);
				}
			}
		}
		DebugDrawLocal("SetupWordData m_wordData.Count=" + m_wordData.Count);
	}

	private static string checkProc(string check_str, string nospace_word, int space_count)
	{
		DebugDrawLocal("checkProc check_str=" + check_str + " nospace_word=" + nospace_word);
		int num = 0;
		foreach (NGWordData wordDatum in m_wordData)
		{
			if (wordDatum.param == 0)
			{
				if (check_str.IndexOf(wordDatum.word) != -1)
				{
					ErrorDraw("0 check_str=" + check_str + " checkProc index=" + num + " row.word=" + wordDatum.word);
					return wordDatum.word;
				}
				if (nospace_word.IndexOf(wordDatum.word) != -1)
				{
					ErrorDraw("0 nospace_word=" + nospace_word + " checkProc index=" + num + " row.word=" + wordDatum.word);
					return wordDatum.word;
				}
			}
			else
			{
				if (check_str == wordDatum.word)
				{
					ErrorDraw("1 check_str=" + check_str + " checkProc index=" + num + " row.word=" + wordDatum.word);
					return wordDatum.word;
				}
				if (nospace_word == wordDatum.word)
				{
					ErrorDraw("1 nospace_word=" + nospace_word + " checkProc index=" + num + " row.word=" + wordDatum.word);
					return wordDatum.word;
				}
			}
			num++;
		}
		return null;
	}

	private static bool isCheckUILabel(string str, UILabel uiLabel)
	{
		if (str != null && uiLabel != null)
		{
			UIFont font = uiLabel.font;
			if (font != null)
			{
				BMFont bmFont = font.bmFont;
				if (bmFont != null)
				{
					for (int i = 0; i < str.Length; i++)
					{
						char c = str[i];
						if (!font.isDynamic)
						{
							BMGlyph glyph = bmFont.GetGlyph(c);
							if (glyph == null)
							{
								DebugDrawLocal("isCheckUILabel BMGlyph str=" + str + " i=" + i + " c=" + c);
								return true;
							}
						}
						else
						{
							Font dynamicFont = font.dynamicFont;
							CharacterInfo info;
							if (dynamicFont != null && !dynamicFont.GetCharacterInfo(c, out info, font.dynamicFontSize, font.dynamicFontStyle))
							{
								DebugDrawLocal("isCheckUILabel dynamicFont str=" + str + " i=" + i + " c=" + c);
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}

	private static bool isCheckEmoji(string str)
	{
		string text = "[\\u2002-\\u2005]|\\u203C|\\u2049|\\u2122|\\u2139|[\\u2194-\\u2199]|\\u21A9|\\u21AA";
		if (PregMatch(text, str))
		{
			DebugDrawLocal("PregMatch " + text);
			return true;
		}
		string text2 = "\\u231A|\\u231B|[\\u23E9-\\u23EC]|\\u23F0|\\u23F3|\\u24C2|\\u25AA|\\u25AB|\\u25B6|\\u25C0|[\\u25FB-\\u25FE]";
		if (PregMatch(text2, str))
		{
			DebugDrawLocal("PregMatch " + text2);
			return true;
		}
		string text3 = "[\\u2600-\\u27FF]|\\u2934|\\u2935|[\\u2B05-\\u2B07]|\\u2B1B|\\u2B1C|\\u2B50|\\u2B55|\\u3030|\\u303D|\\u3297|\\u3299";
		if (PregMatch(text3, str))
		{
			DebugDrawLocal("PregMatch " + text3);
			return true;
		}
		string text4 = "[\\uE000-\\uF8FF]";
		if (PregMatch(text4, str))
		{
			DebugDrawLocal("PregMatch " + text4);
			return true;
		}
		string text5 = "[\\uD800-\\uE000]";
		if (PregMatch(text5, str))
		{
			DebugDrawLocal("PregMatch " + text5);
			return true;
		}
		return false;
	}

	private static bool isCheckKisyuIzon(string str)
	{
		string text = "～∥―－①②③④⑤⑥⑦⑧⑨⑩⑪⑫⑬⑭⑮⑯⑰⑱⑲⑳ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ㍉㌔㌢㍍㌘㌧㌃㌶㍑㍗㌍㌦㌣㌫㍊㌻㎜㎝㎞㎎㎏㏄㎡㍻〝〟№㏍℡㊤㊥㊦㊧㊨㈱㈲㈹㍾㍽㍼≒≡∫∮∑√⊥∠∟⊿∵∩∪ⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩ￢￤＇＂㈱№℡∵";
		string text2 = text;
		for (int i = 0; i < text2.Length; i++)
		{
			char value = text2[i];
			if (str.IndexOf(value) != -1)
			{
				DebugDrawLocal("isCheckKisyuIzon=" + value);
				return true;
			}
		}
		return false;
	}

	private static string convertKana(string value)
	{
		string str = PregReplace("\\s+", " ", value);
		str = MbConvertKana_r(str);
		str = MbConvertKana_n(str);
		str = MbConvertKana_K(str);
		str = MbConvertKana_C(str);
		str = MbConvertKana_V(str);
		str = StrToLower(str);
		str = str2UpperKana(str);
		return replaceSpecialString(str);
	}

	private static string MbConvertKana_r(string str)
	{
		string text = "ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ";
		string text2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		char[] array = text.ToCharArray();
		char[] array2 = text2.ToCharArray();
		if (array.Length == array2.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				str = str.Replace(array[i], array2[i]);
			}
			char[] array3 = text.ToLower().ToCharArray();
			char[] array4 = text2.ToLower().ToCharArray();
			for (int j = 0; j < array3.Length; j++)
			{
				str = str.Replace(array3[j], array4[j]);
			}
		}
		return str;
	}

	private static string MbConvertKana_n(string str)
	{
		string text = "０１２３４５６７８９";
		string text2 = "0123456789";
		char[] array = text.ToCharArray();
		char[] array2 = text2.ToCharArray();
		if (array.Length == array2.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				str = str.Replace(array[i], array2[i]);
			}
		}
		return str;
	}

	private static string MbConvertKana_K(string str)
	{
		string text = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン\u309b\u309cァィゥェォャュョッ";
		string text2 = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝﾞﾟｧｨｩｪｫｬｭｮｯ";
		char[] array = text.ToCharArray();
		char[] array2 = text2.ToCharArray();
		if (array.Length == array2.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				str = str.Replace(array2[i], array[i]);
			}
		}
		return str;
	}

	private static string MbConvertKana_C(string str)
	{
		string text = "ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをん";
		string text2 = "ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲン";
		char[] array = text.ToCharArray();
		char[] array2 = text2.ToCharArray();
		if (array.Length == array2.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				str = str.Replace(array[i], array2[i]);
			}
		}
		return str;
	}

	private static string MbConvertKana_V(string str)
	{
		str = PregReplace("\u309b+", "\u309b", str);
		str = PregReplace("\u309c+", "\u309c", str);
		bool change;
		do
		{
			change = false;
			string str2 = str;
			str = MbConvertKana_V_bottom(str2, ref change);
		}
		while (change);
		return str;
	}

	private static string MbConvertKana_V_bottom(string str, ref bool change)
	{
		string text = "ウヴカガキギクグケゲコゴサザシジスズセゼソゾタダチヂツヅテデトドハバヒビフブヘベホボ";
		string text2 = "ヴヴガガギギググゲゲゴゴザザジジズズゼゼゾゾダダヂヂヅヅデデドドババビビブブベベボボ";
		char[] array = text2.ToCharArray();
		string text3 = "ハパヒピフプヘペホポ";
		string text4 = "パパピピププペペポポ";
		char[] array2 = text4.ToCharArray();
		change = false;
		if (str != null && str.Length > 0)
		{
			char[] array3 = str.ToCharArray();
			int num = str.Length - 1;
			for (int num2 = num; num2 >= 0; num2--)
			{
				char c = array3[num2];
				switch (c)
				{
				case '\u309b':
				{
					int num5 = num2 - 1;
					if (num5 >= 0)
					{
						char value2 = array3[num5];
						int num6 = text.IndexOf(value2, 0);
						if (num6 != -1 && num6 < array.Length)
						{
							string ptn2 = value2.ToString() + c;
							change = true;
							return PregReplace(ptn2, array[num6].ToString(), str);
						}
					}
					break;
				}
				case '\u309c':
				{
					int num3 = num2 - 1;
					if (num3 >= 0)
					{
						char value = array3[num3];
						int num4 = text3.IndexOf(value, 0);
						if (num4 != -1 && num4 < array2.Length)
						{
							string ptn = value.ToString() + c;
							change = true;
							return PregReplace(ptn, array2[num4].ToString(), str);
						}
					}
					break;
				}
				}
			}
		}
		return str;
	}

	private static string replaceSpecialString(string value)
	{
		string[] ptn = new string[6]
		{
			"ー",
			"×",
			"○",
			"！",
			"．",
			"ー"
		};
		string[] newStr = new string[6]
		{
			"-",
			"x",
			"0",
			"!",
			".",
			"-"
		};
		return PregReplace(ptn, newStr, value);
	}

	private static string str2UpperKana(string value)
	{
		string[] ptn = new string[12]
		{
			"ァ",
			"ィ",
			"ゥ",
			"ェ",
			"ォ",
			"ッ",
			"ャ",
			"ュ",
			"ョ",
			"ヮ",
			"ヵ",
			"ヶ"
		};
		string[] newStr = new string[12]
		{
			"ア",
			"イ",
			"ウ",
			"エ",
			"オ",
			"ツ",
			"ヤ",
			"ユ",
			"ヨ",
			"ワ",
			"カ",
			"ケ"
		};
		return PregReplace(ptn, newStr, value);
	}

	private static string StrToLower(string str)
	{
		return str.ToLower();
	}

	private static string StrReplace(string oldWord, string newWord, string str, ref int count)
	{
		string[] separator = new string[1]
		{
			oldWord
		};
		string[] array = str.Split(separator, StringSplitOptions.None);
		count = array.Length - 1;
		return PregReplace(oldWord, newWord, str);
	}

	private static string PregReplace(string ptn, string newStr, string str)
	{
		return Regex.Replace(str, ptn, newStr);
	}

	private static string PregReplace(string[] ptn, string[] newStr, string str)
	{
		if (ptn.Length == newStr.Length)
		{
			for (int i = 0; i < ptn.Length; i++)
			{
				str = PregReplace(ptn[i], newStr[i], str);
			}
		}
		return str;
	}

	private static bool PregMatch(string ptn, string str)
	{
		if (Regex.IsMatch(str, ptn))
		{
			return true;
		}
		return false;
	}

	private static void DebugDrawLocal(string str)
	{
	}

	private static void ErrorDraw(string str)
	{
	}
}
