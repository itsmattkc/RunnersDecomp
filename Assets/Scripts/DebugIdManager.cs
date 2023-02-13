// Comment this out to disable id manager
//#define SHOW_ID_MANAGER

using App;
using SaveData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DebugIdManager : MonoBehaviour
{
	private class UserData
	{
		public string id;

		public string pass;

		public string date;

		public UserData(string id, string pass, string date)
		{
			this.id = id;
			this.pass = pass;
			this.date = date;
		}
	}

	private readonly string SAVE_FILE_NAME = "DebugId.txt";

	private Dictionary<string, UserData> m_userDataDict = new Dictionary<string, UserData>();

	private string actionServerType;

	private string m_saveLabel = "ID Save";

	private string m_loadLabel;

	private void Awake()
	{
#if !SHOW_ID_MANAGER
		UnityEngine.Object.Destroy(base.gameObject);
#endif
	}

	private void Start()
	{
		actionServerType = Env.actionServerType.ToString();
		LoadFile();
		SetLoadLabel();
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			BoxCollider boxCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(gameObject, "Btn_mainmenu");
			if (boxCollider != null)
			{
				boxCollider.center -= new Vector3(300f, 0f, 0f);
			}
		}
	}

	private void SetLoadLabel()
	{
		if (m_userDataDict.ContainsKey(actionServerType))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ID Load\n\n");
			stringBuilder.Append(m_userDataDict[actionServerType].id);
			stringBuilder.Append("\n");
			stringBuilder.Append(actionServerType);
			stringBuilder.Append("\n");
			stringBuilder.Append(m_userDataDict[actionServerType].date);
			m_loadLabel = stringBuilder.ToString();
		}
		else
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("ID Load\n\n");
			stringBuilder2.Append("----------\n");
			stringBuilder2.Append(actionServerType);
			stringBuilder2.Append("\n--/-- --:--:--");
			m_loadLabel = stringBuilder2.ToString();
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width - 150, Screen.height / 2 - 80, 140f, 60f), m_saveLabel) && SystemSaveManager.Instance != null)
		{
			if (SystemSaveManager.GetGameID() == "0")
			{
				Debug.LogWarning("Game ID has not been set!");
				return;
			}
			if (m_userDataDict.ContainsKey(actionServerType))
			{
				m_userDataDict[actionServerType].id = SystemSaveManager.GetGameID();
				m_userDataDict[actionServerType].pass = SystemSaveManager.GetGamePassword();
				m_userDataDict[actionServerType].date = DateTime.Now.ToString("MM/dd HH:mm:ss");
			}
			else
			{
				m_userDataDict.Add(actionServerType, new UserData(SystemSaveManager.GetGameID(), SystemSaveManager.GetGamePassword(), DateTime.Now.ToString("MM/dd HH:mm:ss")));
			}
			SaveFile();
			SetLoadLabel();
		}
		if (GUI.Button(new Rect(Screen.width - 150, Screen.height / 2, 140f, 90f), m_loadLabel) && SystemSaveManager.Instance != null && m_userDataDict.ContainsKey(actionServerType))
		{
			SystemSaveManager.SetGameID(m_userDataDict[actionServerType].id);
			SystemSaveManager.SetGamePassword(m_userDataDict[actionServerType].pass);
			SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
			if (systemdata != null)
			{
				SystemSaveManager.Instance.SaveSystemData();
			}
			HudMenuUtility.GoToTitleScene();
		}
	}

	private void SaveFile()
	{
		string path = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
		StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);
		if (streamWriter == null)
		{
			return;
		}
		foreach (string key in m_userDataDict.Keys)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(key);
			stringBuilder.Append(",");
			stringBuilder.Append(m_userDataDict[key].id);
			stringBuilder.Append(",");
			stringBuilder.Append(m_userDataDict[key].pass);
			stringBuilder.Append(",");
			stringBuilder.Append(m_userDataDict[key].date);
			stringBuilder.Append("\n");
			streamWriter.Write(stringBuilder.ToString());
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
			if (array != null && array.Length > 3)
			{
				m_userDataDict.Add(array[0], new UserData(array[1], array[2], array[3]));
			}
		}
		streamReader.Close();
	}
}
