using System.Collections.Generic;
using UnityEngine;

namespace Text
{
	public class TextManager : MonoBehaviour
	{
		public enum TextType
		{
			TEXTTYPE_NONE = -1,
			TEXTTYPE_COMMON_TEXT,
			TEXTTYPE_MILEAGE_MAP_COMMON,
			TEXTTYPE_MILEAGE_MAP_EPISODE,
			TEXTTYPE_MILEAGE_MAP_PRE_EPISODE,
			TEXTTYPE_FIXATION_TEXT,
			TEXTTYPE_EVENT_COMMON_TEXT,
			TEXTTYPE_EVENT_SPECIFIC,
			TEXTTYPE_CHAO_TEXT,
			TEXTTYPE_END
		}

		private static TextManager m_instance;

		private Dictionary<TextType, TextLoadImpl> m_textDictionary;

		private string m_languageName;

		private string m_suffixeName;

		public static void SetLanguageName()
		{
			TextManager instance = GetInstance();
			instance.m_languageName = TextUtility.GetXmlLanguageDataType();
		}

		public static void SetSuffixeName()
		{
			TextManager instance = GetInstance();
			instance.m_suffixeName = TextUtility.GetSuffixe();
		}

		private void Awake()
		{
			if (m_instance == null)
			{
				m_instance = this;
				Object.DontDestroyOnLoad(base.gameObject);
				Language.InitAppEnvLanguage();
				m_languageName = TextUtility.GetXmlLanguageDataType();
				m_suffixeName = TextUtility.GetSuffixe();
				m_textDictionary = new Dictionary<TextType, TextLoadImpl>();
				SetupFixationText();
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private void OnDestroy()
		{
			if (m_instance == this)
			{
				m_textDictionary.Clear();
				m_instance = null;
			}
		}

		private void SetupFixationText()
		{
			TextType key = TextType.TEXTTYPE_FIXATION_TEXT;
			Dictionary<TextType, TextLoadImpl> textDictionary = m_textDictionary;
			if (!textDictionary.ContainsKey(key))
			{
				textDictionary.Add(key, new TextLoadImpl());
				string fileName = "TextData/text_fixation_text_" + m_suffixeName;
				if (!textDictionary[key].IsSetup())
				{
					textDictionary[key].LoadResourcesSetup(fileName, m_languageName);
				}
			}
		}

		public static void NotLoadSetupCommonText()
		{
			NotLoadSetupText(TextType.TEXTTYPE_COMMON_TEXT, "text_common_text");
			SetupCommonText();
		}

		public static void NotLoadSetupChaoText()
		{
			NotLoadSetupText(TextType.TEXTTYPE_CHAO_TEXT, "text_chao_text");
			SetupChaoText();
		}

		public static void NotLoadSetupEventText()
		{
			NotLoadSetupText(TextType.TEXTTYPE_EVENT_COMMON_TEXT, "text_event_common_text");
			SetupEventText();
		}

		private static void NotLoadSetupText(TextType textType, string baseName)
		{
			TextManager instance = GetInstance();
			if (instance == null)
			{
				return;
			}
			Dictionary<TextType, TextLoadImpl> textDictionary = instance.m_textDictionary;
			if (textDictionary.ContainsKey(textType))
			{
				GameObject gameObject = GameObject.Find(baseName + "_" + instance.m_suffixeName);
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
			else
			{
				textDictionary.Add(textType, new TextLoadImpl());
			}
		}

		public static void LoadCommonText(ResourceSceneLoader sceneLoader)
		{
			Load(sceneLoader, TextType.TEXTTYPE_COMMON_TEXT, "text_common_text");
		}

		public static void LoadChaoText(ResourceSceneLoader sceneLoader)
		{
			Load(sceneLoader, TextType.TEXTTYPE_CHAO_TEXT, "text_chao_text");
		}

		public static void LoadEventText(ResourceSceneLoader sceneLoader)
		{
			Load(sceneLoader, TextType.TEXTTYPE_EVENT_COMMON_TEXT, "text_event_common_text");
		}

		private static string GetEventProductionTextName(int specificId)
		{
			if (specificId > 0)
			{
				return "text_event_" + specificId + "_text";
			}
			return null;
		}

		public static void LoadEventProductionText(ResourceSceneLoader sceneLoader)
		{
			if (!(EventManager.Instance != null) || EventUtility.IsExistSpecificEventText(EventManager.Instance.Id))
			{
				int specificId = EventManager.GetSpecificId();
				string eventProductionTextName = GetEventProductionTextName(specificId);
				if (!string.IsNullOrEmpty(eventProductionTextName))
				{
					Load(sceneLoader, TextType.TEXTTYPE_EVENT_SPECIFIC, eventProductionTextName);
				}
			}
		}

		public static void SetupCommonText()
		{
			Setup(TextType.TEXTTYPE_COMMON_TEXT, "text_common_text");
		}

		public static void SetupChaoText()
		{
			Setup(TextType.TEXTTYPE_CHAO_TEXT, "text_chao_text");
		}

		public static void SetupEventText()
		{
			Setup(TextType.TEXTTYPE_EVENT_COMMON_TEXT, "text_event_common_text");
		}

		public static void SetupEventProductionText()
		{
			if (!(EventManager.Instance != null) || EventUtility.IsExistSpecificEventText(EventManager.Instance.Id))
			{
				int specificId = EventManager.GetSpecificId();
				string eventProductionTextName = GetEventProductionTextName(specificId);
				if (!string.IsNullOrEmpty(eventProductionTextName))
				{
					Setup(TextType.TEXTTYPE_EVENT_SPECIFIC, eventProductionTextName);
				}
			}
		}

		public static void Load(ResourceSceneLoader sceneLoader, TextType textType, string fileName)
		{
			TextManager instance = GetInstance();
			if (!(instance == null))
			{
				Dictionary<TextType, TextLoadImpl> textDictionary = instance.m_textDictionary;
				if (!textDictionary.ContainsKey(textType))
				{
					textDictionary.Add(textType, new TextLoadImpl(sceneLoader, fileName, instance.m_suffixeName));
				}
			}
		}

		public static void Setup(TextType textType, string fileName)
		{
			TextManager instance = GetInstance();
			if (!(instance == null))
			{
				Dictionary<TextType, TextLoadImpl> textDictionary = instance.m_textDictionary;
				if (textDictionary.ContainsKey(textType) && !textDictionary[textType].IsSetup())
				{
					textDictionary[textType].LoadSceneSetup(fileName, instance.m_languageName, instance.m_suffixeName);
				}
			}
		}

		public static void UnLoad(TextType textType)
		{
			TextManager instance = GetInstance();
			if (!(instance == null))
			{
				Dictionary<TextType, TextLoadImpl> textDictionary = instance.m_textDictionary;
				if (textDictionary.ContainsKey(textType))
				{
					textDictionary.Remove(textType);
				}
			}
		}

		public static TextObject GetText(TextType textType, string categoryName, string cellName)
		{
			TextManager instance = GetInstance();
			if (instance == null)
			{
				return new TextObject(string.Empty);
			}
			Dictionary<TextType, TextLoadImpl> textDictionary = instance.m_textDictionary;
			if (!textDictionary.ContainsKey(textType))
			{
				return new TextObject(string.Empty);
			}
			return new TextObject(textDictionary[textType].GetText(categoryName, cellName));
		}

		public static int GetCategoryCellCount(TextType textType, string categoryName)
		{
			TextManager instance = GetInstance();
			if (instance == null)
			{
				return -1;
			}
			Dictionary<TextType, TextLoadImpl> textDictionary = instance.m_textDictionary;
			if (!textDictionary.ContainsKey(textType))
			{
				return -1;
			}
			return textDictionary[textType].GetCellCount(categoryName);
		}

		private static TextManager GetInstance()
		{
			if (m_instance == null)
			{
				GameObject gameObject = new GameObject("TextManager");
				gameObject.AddComponent<TextManager>();
			}
			return m_instance;
		}
	}
}
