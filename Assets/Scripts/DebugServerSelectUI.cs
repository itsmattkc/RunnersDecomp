using App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DebugServerSelectUI : MonoBehaviour
{
	private enum ButtonType
	{
		IDLE = -1,
		LOCAL1,
		LOCAL2,
		LOCAL3,
		LOCAL4,
		LOCAL5,
		DEVELOP1,
		DEVELOP2,
		DEVELOP3,
		RELEASE,
		TITLE
	}

	private readonly Vector2 SCREEN_RECT_SIZE = new Vector2(1600f, 900f);

	private Vector2 m_GUIScale = default(Vector2);

	private Dictionary<ButtonType, Rect> m_ButtonDict = new Dictionary<ButtonType, Rect>();

	private GUIStyle m_buttonStyle;

	private GUIStyle m_labelStyle;

	private bool m_GUIInited;

	private int m_fontSize;

	private int m_commandCount;

	private readonly int POP_BUTTON_COUNT = 10;

	private Rect m_hiddenButtonRect;

	private readonly string SAVE_FILE_NAME = "DefaultActionServerType.txt";

	private Dictionary<string, string> m_DefaultActionServerType = new Dictionary<string, string>();

	private StringBuilder m_versionLabel = new StringBuilder();

	private StringBuilder m_serverLabel = new StringBuilder();

	private ButtonType m_buttonType;

	private readonly string[] ButtonName = new string[10]
	{
		"Local1",
		"Local2",
		"Local3",
		"Local4",
		"Local5",
		"Develop1",
		"Develop2",
		"Develop3",
		"Release",
		"Title"
	};

	private void Start()
	{
		m_buttonType = ButtonType.IDLE;
		LoadFile();
		if (m_DefaultActionServerType.ContainsKey(CurrentBundleVersion.version))
		{
			string text = m_DefaultActionServerType[CurrentBundleVersion.version];
			try
			{
				Env.actionServerType = (Env.ActionServerType)(int)Enum.Parse(typeof(Env.ActionServerType), text);
			}
			catch (ArgumentException)
			{
				Debug.Log("Load ServerType Error " + text);
			}
		}
		m_versionLabel.Append("Ver : ");
		m_versionLabel.Append(CurrentBundleVersion.version);
		m_serverLabel.Append("Server : ");
		m_serverLabel.Append(Env.actionServerType);
		m_fontSize = 32;
		float num = Screen.width;
		Vector2 sCREEN_RECT_SIZE = SCREEN_RECT_SIZE;
		float x = num / sCREEN_RECT_SIZE.x;
		float num2 = Screen.height;
		Vector2 sCREEN_RECT_SIZE2 = SCREEN_RECT_SIZE;
		m_GUIScale = new Vector2(x, num2 / sCREEN_RECT_SIZE2.y);
		m_ButtonDict.Add(ButtonType.LOCAL1, MatchScreenSize(new Rect(100f, 200f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.LOCAL2, MatchScreenSize(new Rect(400f, 200f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.LOCAL3, MatchScreenSize(new Rect(700f, 200f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.LOCAL4, MatchScreenSize(new Rect(1000f, 200f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.LOCAL5, MatchScreenSize(new Rect(1300f, 200f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.DEVELOP1, MatchScreenSize(new Rect(100f, 400f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.DEVELOP2, MatchScreenSize(new Rect(400f, 400f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.DEVELOP3, MatchScreenSize(new Rect(700f, 400f, 250f, 150f)));
		m_ButtonDict.Add(ButtonType.TITLE, MatchScreenSize(new Rect(1000f, 600f, 550f, 150f)));
		m_hiddenButtonRect = MatchScreenSize(new Rect(0f, 0f, 200f, 150f));
	}

	private void OnGUI()
	{
		if (!m_GUIInited)
		{
			m_buttonStyle = new GUIStyle(GUI.skin.button);
			m_buttonStyle.fontSize = (int)((float)m_fontSize * m_GUIScale.x);
			m_labelStyle = new GUIStyle(GUI.skin.label);
			m_labelStyle.fontSize = (int)((float)m_fontSize * m_GUIScale.x);
			m_labelStyle.alignment = TextAnchor.UpperCenter;
			m_GUIInited = true;
		}
		Vector2 sCREEN_RECT_SIZE = SCREEN_RECT_SIZE;
		GUI.Label(MatchScreenSize(new Rect(0f, 60f, sCREEN_RECT_SIZE.x, 100f)), m_versionLabel.ToString(), m_labelStyle);
		Vector2 sCREEN_RECT_SIZE2 = SCREEN_RECT_SIZE;
		GUI.Label(MatchScreenSize(new Rect(0f, 120f, sCREEN_RECT_SIZE2.x, 100f)), m_serverLabel.ToString(), m_labelStyle);
		foreach (ButtonType key in m_ButtonDict.Keys)
		{
			if (key == m_buttonType)
			{
				GUI.color = Color.yellow;
				GUI.Box(m_ButtonDict[key], string.Empty);
			}
			if (GUI.Button(m_ButtonDict[key], ButtonName[(int)key], m_buttonStyle))
			{
				OnClickButton(key);
				m_serverLabel.Length = 0;
				m_serverLabel.Append("Server : ");
				m_serverLabel.Append(Env.actionServerType);
			}
			if (key == m_buttonType)
			{
				GUI.color = Color.white;
			}
		}
		if (GUI.Button(m_hiddenButtonRect, string.Empty, GUIStyle.none))
		{
			m_commandCount++;
			if (m_hiddenButtonRect.x == 0f)
			{
				m_hiddenButtonRect.x = (float)Screen.width - m_hiddenButtonRect.width;
			}
			else
			{
				m_hiddenButtonRect.x = 0f;
			}
			if (m_commandCount == POP_BUTTON_COUNT)
			{
				m_ButtonDict.Add(ButtonType.RELEASE, MatchScreenSize(new Rect(100f, 600f, 250f, 150f)));
			}
		}
	}

	private void OnClickButton(ButtonType type)
	{
		m_buttonType = type;
		switch (type)
		{
		case ButtonType.LOCAL1:
			Env.actionServerType = Env.ActionServerType.LOCAL1;
			break;
		case ButtonType.LOCAL2:
			Env.actionServerType = Env.ActionServerType.LOCAL2;
			break;
		case ButtonType.LOCAL3:
			Env.actionServerType = Env.ActionServerType.LOCAL3;
			break;
		case ButtonType.LOCAL4:
			Env.actionServerType = Env.ActionServerType.LOCAL4;
			break;
		case ButtonType.LOCAL5:
			Env.actionServerType = Env.ActionServerType.LOCAL5;
			break;
		case ButtonType.DEVELOP1:
			Env.actionServerType = Env.ActionServerType.DEVELOP;
			break;
		case ButtonType.DEVELOP2:
			Env.actionServerType = Env.ActionServerType.DEVELOP2;
			break;
		case ButtonType.DEVELOP3:
			Env.actionServerType = Env.ActionServerType.DEVELOP3;
			break;
		case ButtonType.RELEASE:
			Env.actionServerType = Env.ActionServerType.RELEASE;
			break;
		case ButtonType.TITLE:
			if (m_DefaultActionServerType.ContainsKey(CurrentBundleVersion.version))
			{
				m_DefaultActionServerType[CurrentBundleVersion.version] = Env.actionServerType.ToString();
			}
			else
			{
				m_DefaultActionServerType.Add(CurrentBundleVersion.version, Env.actionServerType.ToString());
			}
			SaveFile();
			Application.LoadLevel(TitleDefine.TitleSceneName);
			break;
		}
	}

	private Rect MatchScreenSize(Rect rect)
	{
		Rect result = new Rect(rect);
		result.x *= m_GUIScale.x;
		result.width *= m_GUIScale.x;
		result.y *= m_GUIScale.y;
		result.height *= m_GUIScale.y;
		return result;
	}

	private void SaveFile()
	{
		string path = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
		StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);
		if (streamWriter == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in m_DefaultActionServerType)
		{
			streamWriter.Write(item.Key + "," + item.Value + "\n");
		}
		streamWriter.Close();
	}

	private void LoadFile()
	{
		string path = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
		if (!File.Exists(path))
		{
			return;
		}
		StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
		if (streamReader == null)
		{
			return;
		}
		while (streamReader.Peek() >= 0)
		{
			string text = streamReader.ReadLine();
			string[] array = text.Split(',');
			if (array != null && array.Length > 1)
			{
				m_DefaultActionServerType.Add(array[0], array[1]);
			}
		}
		streamReader.Close();
	}
}
