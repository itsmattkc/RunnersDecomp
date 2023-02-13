using App.Utility;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;

namespace SaveData
{
	public class SystemSaveManager : MonoBehaviour
	{
		public enum ErrorCode
		{
			NO_ERROR,
			FILE_NOT_EXIST,
			FILE_CANNOT_OPEN,
			DATA_INVALID,
			INVALID_DEVICE_ID
		}

		private const string SYSTEM_FILE_NAME = "sfrn";

		private const string EXTENSION = ".game";

		private const string XmlRootName = "SystemData";

		private bool m_gameDataDirty;

		private bool m_errorOnStart;

		private SystemData m_SystemData = new SystemData();

		private GameIDData m_GameIDData = new GameIDData();

		private int m_SaveDataVersion = 1;

		private ErrorCode m_errorcode;

		private static SystemSaveManager instance;

		public static SystemSaveManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (UnityEngine.Object.FindObjectOfType(typeof(SystemSaveManager)) as SystemSaveManager);
				}
				return instance;
			}
		}

		protected void Awake()
		{
			CheckInstance();
		}

		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			Init();
		}

		public void Init()
		{
			m_SystemData.Init(m_SaveDataVersion);
			m_GameIDData.Init();
			if (!SystemFileCheck())
			{
				string @string = PlayerPrefs.GetString("aa7329ab4330306fbdd6dbe9b85c96be");
				if (!string.IsNullOrEmpty(@string) && !@string.Equals("0") && GetGameIDDataFromPlayerPrefs())
				{
					m_errorOnStart = true;
				}
				m_GameIDData.device = SystemInfo.deviceUniqueIdentifier;
				CheckLightMode();
				SaveSystemData();
				return;
			}
			LoadSystemData();
			if (m_errorcode == ErrorCode.INVALID_DEVICE_ID)
			{
				m_SystemData.Init(m_SaveDataVersion);
				m_GameIDData.Init();
				m_GameIDData.device = SystemInfo.deviceUniqueIdentifier;
				CheckLightMode();
				SaveSystemData();
			}
			else if (m_errorcode != 0)
			{
				GetGameIDDataFromPlayerPrefs();
				m_errorOnStart = true;
			}
		}

		public static SystemData GetSystemSaveData()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.GetSystemdata();
		}

		public SystemData GetSystemdata()
		{
			return m_SystemData;
		}

		public static string GetGameID()
		{
			if (instance == null)
			{
				return "0";
			}
			if (instance.m_GameIDData == null)
			{
				return "0";
			}
			return instance.m_GameIDData.id;
		}

		public static bool SetGameID(string id)
		{
			if (instance == null)
			{
				return false;
			}
			instance.m_GameIDData.id = id;
			instance.m_gameDataDirty = true;
			return true;
		}

		public static string GetGamePassword()
		{
			if (instance == null)
			{
				return string.Empty;
			}
			if (instance.m_GameIDData == null)
			{
				return string.Empty;
			}
			return instance.m_GameIDData.password;
		}

		public static bool SetGamePassword(string password)
		{
			if (instance == null)
			{
				return false;
			}
			instance.m_GameIDData.password = password;
			instance.m_gameDataDirty = true;
			return true;
		}

		public static bool IsUserIDValid()
		{
			if (instance == null)
			{
				return false;
			}
			string gameID = GetGameID();
			if (string.IsNullOrEmpty(gameID))
			{
				return false;
			}
			if (gameID == "0")
			{
				return false;
			}
			return true;
		}

		public static string GetTakeoverID()
		{
			if (instance == null)
			{
				return string.Empty;
			}
			if (instance.m_GameIDData == null)
			{
				return string.Empty;
			}
			return instance.m_GameIDData.takeoverId;
		}

		public static bool SetTakeoverID(string id)
		{
			if (instance == null)
			{
				return false;
			}
			instance.m_GameIDData.takeoverId = id;
			instance.m_gameDataDirty = true;
			return true;
		}

		public static string GetTakeoverPassword()
		{
			if (instance == null)
			{
				return string.Empty;
			}
			if (instance.m_GameIDData == null)
			{
				return string.Empty;
			}
			return instance.m_GameIDData.takeoverPassword;
		}

		public static bool SetTakeoverPassword(string pass)
		{
			if (instance == null)
			{
				return false;
			}
			instance.m_GameIDData.takeoverPassword = pass;
			instance.m_gameDataDirty = true;
			return true;
		}

		public static string GetCountryCode()
		{
			if (instance == null)
			{
				return string.Empty;
			}
			return instance.m_SystemData.country;
		}

		public static bool SetCountryCode(string countrycode)
		{
			if (instance == null)
			{
				return false;
			}
			instance.m_SystemData.country = countrycode;
			return true;
		}

		public static bool IsIAPMessage()
		{
			if (instance == null)
			{
				return false;
			}
			bool result = false;
			if (instance.m_SystemData != null && !string.IsNullOrEmpty(instance.m_SystemData.country) && instance.m_SystemData.iap == 0)
			{
				result = true;
			}
			return result;
		}

		public static void CheckIAPMessage()
		{
			if (!(instance == null) && instance.m_SystemData != null && !string.IsNullOrEmpty(instance.m_SystemData.country))
			{
				if (RegionManager.Instance.IsNeedIapMessage())
				{
					instance.m_SystemData.iap = 0;
				}
				else
				{
					instance.m_SystemData.iap = 2;
				}
				Save();
			}
		}

		public static void SetIAPMessageAlreadyRead()
		{
			if (!(instance == null) && instance.m_SystemData != null && !string.IsNullOrEmpty(instance.m_SystemData.country))
			{
				instance.m_SystemData.iap = 1;
				Save();
			}
		}

		public static void Save()
		{
			if (!(instance == null))
			{
				instance.SaveSystemData();
			}
		}

		public static bool Load()
		{
			if (instance == null)
			{
				return false;
			}
			return instance.LoadSystemData();
		}

		private string getSavePath()
		{
			return Application.persistentDataPath;
		}

		private string getDevelopmentPath()
		{
			return getSavePath() + "/../../SonicRunnerss";
		}

		private string GetHashData(string textdata)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(textdata);
			SHA256 sHA = new SHA256CryptoServiceProvider();
			byte[] array = sHA.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.AppendFormat("{0:X2}", array[i]);
			}
			sHA.Clear();
			return stringBuilder.ToString();
		}

		private bool WriteFile(string path, string cnvText)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			using (Stream stream = File.Open(path + "/sfrn.game", FileMode.Create))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					try
					{
						string hashData = GetHashData(cnvText);
						streamWriter.Write(hashData + "\n");
						streamWriter.Write(cnvText);
						m_errorcode = ErrorCode.NO_ERROR;
					}
					catch (Exception ex)
					{
						Debug.Log("WriteFile Error:" + ex.Message);
						m_errorcode = ErrorCode.FILE_CANNOT_OPEN;
					}
				}
			}
			return m_errorcode == ErrorCode.NO_ERROR;
		}

		public void SaveSystemData()
		{
			if (m_gameDataDirty)
			{
				SaveGameIDDataToPlayerPrefs();
				m_gameDataDirty = false;
			}
			XmlDocument xmlDocument = CreateXmlData();
			string cnvText = AESCrypt.Encrypt(xmlDocument.InnerXml);
			WriteFile(getSavePath(), cnvText);
		}

		public bool LoadSystemData()
		{
			//Discarded unreachable code: IL_003a
			Stream stream = null;
			try
			{
				stream = File.Open(getSavePath() + "/sfrn.game", FileMode.Open);
			}
			catch
			{
				m_errorcode = ErrorCode.FILE_NOT_EXIST;
				if (stream != null)
				{
					stream.Dispose();
				}
				return false;
			}
			if (stream != null)
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					string text = streamReader.ReadLine();
					string text2 = streamReader.ReadToEnd();
					string hashData = GetHashData(text2);
					if (!text.Equals(hashData))
					{
						Debug.Log("Data is Invalid.");
						m_errorcode = ErrorCode.DATA_INVALID;
					}
					else
					{
						string data = AESCrypt.Decrypt(text2);
						ParseXmlData(data);
						m_errorcode = ErrorCode.NO_ERROR;
					}
				}
				stream.Close();
				stream = null;
			}
			else
			{
				m_errorcode = ErrorCode.FILE_NOT_EXIST;
			}
			return m_errorcode == ErrorCode.NO_ERROR;
		}

		public bool ErrorOnStart()
		{
			return m_errorOnStart;
		}

		public bool SaveForStartingError()
		{
			m_gameDataDirty = false;
			SaveSystemData();
			m_errorOnStart = false;
			return m_errorcode == ErrorCode.NO_ERROR;
		}

		public void CheckLightMode()
		{
			int width = Screen.width;
			int height = Screen.height;
			int num = width * height;
			if (SystemInfo.systemMemorySize > 512)
			{
				m_SystemData.lightMode = false;
				if (num > 2304000)
				{
					m_SystemData.highTexture = true;
				}
				else
				{
					m_SystemData.highTexture = false;
				}
			}
			else
			{
				m_SystemData.highTexture = false;
				m_SystemData.lightMode = true;
			}
		}

		private XmlDocument CreateXmlData()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDocument.AppendChild(newChild);
			XmlElement xmlElement = xmlDocument.CreateElement("SystemData");
			xmlDocument.AppendChild(xmlElement);
			CleateElementInt(xmlDocument, xmlElement, "version", m_SystemData.version);
			CleateElementInt(xmlDocument, xmlElement, "bgmVolume", m_SystemData.bgmVolume);
			CleateElementInt(xmlDocument, xmlElement, "seVolume", m_SystemData.seVolume);
			CleateElementInt(xmlDocument, xmlElement, "achievementIncentiveCount", m_SystemData.achievementIncentiveCount);
			CleateElementInt(xmlDocument, xmlElement, "iap", m_SystemData.iap);
			CraeteElementBool(xmlDocument, xmlElement, "pushNotice", m_SystemData.pushNotice);
			CraeteElementBool(xmlDocument, xmlElement, "lightMode", m_SystemData.lightMode);
			CraeteElementBool(xmlDocument, xmlElement, "highTexture", m_SystemData.highTexture);
			CreateElementString(xmlDocument, xmlElement, "gameId", m_GameIDData.id);
			CreateElementString(xmlDocument, xmlElement, "password", m_GameIDData.password);
			CreateElementString(xmlDocument, xmlElement, "device", m_GameIDData.device);
			CreateElementString(xmlDocument, xmlElement, "takeoverId", m_GameIDData.takeoverId);
			CreateElementString(xmlDocument, xmlElement, "takeoverPassword", m_GameIDData.takeoverPassword);
			CreateElementString(xmlDocument, xmlElement, "noahId", m_SystemData.noahId);
			CreateElementString(xmlDocument, xmlElement, "purchasedRecipt", m_SystemData.purchasedRecipt);
			CreateElementString(xmlDocument, xmlElement, "country", m_SystemData.country);
			CreateElementString(xmlDocument, xmlElement, "facebookTime", m_SystemData.facebookTime);
			CreateElementString(xmlDocument, xmlElement, "gameStartTime", m_SystemData.gameStartTime);
			CreateElementUint(xmlDocument, xmlElement, "flags", m_SystemData.flags.to_ulong());
			CreateElementUint(xmlDocument, xmlElement, "itemTutorialFlags", m_SystemData.itemTutorialFlags.to_ulong());
			CreateElementUint(xmlDocument, xmlElement, "charaTutorialFlags", m_SystemData.charaTutorialFlags.to_ulong());
			CreateElementUint(xmlDocument, xmlElement, "actionTutorialFlags", m_SystemData.actionTutorialFlags.to_ulong());
			CreateElementUint(xmlDocument, xmlElement, "quickModeTutorialFlags", m_SystemData.quickModeTutorialFlags.to_ulong());
			CreateElementUint(xmlDocument, xmlElement, "pushNoticeFlags", m_SystemData.pushNoticeFlags.to_ulong());
			CreateElementString(xmlDocument, xmlElement, "deckData", m_SystemData.deckData);
			CleateElementInt(xmlDocument, xmlElement, "pictureShowEventId", m_SystemData.pictureShowEventId);
			CleateElementInt(xmlDocument, xmlElement, "pictureShowProgress", m_SystemData.pictureShowProgress);
			CleateElementInt(xmlDocument, xmlElement, "pictureShowEmergeRaidBossProgress", m_SystemData.pictureShowEmergeRaidBossProgress);
			CleateElementInt(xmlDocument, xmlElement, "pictureShowRaidBossFirstBattle", m_SystemData.pictureShowRaidBossFirstBattle);
			CreateElementLong(xmlDocument, xmlElement, "currentRaidDrawIndex", m_SystemData.currentRaidDrawIndex);
			CraeteElementBool(xmlDocument, xmlElement, "raidEntryFlag", m_SystemData.raidEntryFlag);
			CleateElementInt(xmlDocument, xmlElement, "chaoSortType01", m_SystemData.chaoSortType01);
			CleateElementInt(xmlDocument, xmlElement, "chaoSortType02", m_SystemData.chaoSortType02);
			CleateElementInt(xmlDocument, xmlElement, "playCount", m_SystemData.playCount);
			CreateElementString(xmlDocument, xmlElement, "loginRankigTime", m_SystemData.loginRankigTime);
			CleateElementInt(xmlDocument, xmlElement, "playGamesCancelCount", m_SystemData.achievementCancelCount);
			XmlElement xmlElement2 = StartArray(xmlDocument, "fbFriends");
			for (int i = 0; i < m_SystemData.fbFriends.Count; i++)
			{
				string text = m_SystemData.fbFriends[i];
				if (text != null)
				{
					string name = "friend" + (i + 1);
					CreateElementString(xmlDocument, xmlElement2, name, text);
				}
			}
			EndArray(xmlElement, xmlElement2);
			return xmlDocument;
		}

		private void CleateElementInt(XmlDocument doc, XmlElement rootElement, string name, int value)
		{
			XmlElement xmlElement = doc.CreateElement(name);
			XmlText newChild = doc.CreateTextNode(value.ToString());
			xmlElement.AppendChild(newChild);
			rootElement.AppendChild(xmlElement);
		}

		private void CreateElementUint(XmlDocument doc, XmlElement rootElement, string name, uint value)
		{
			XmlElement xmlElement = doc.CreateElement(name);
			XmlText newChild = doc.CreateTextNode(value.ToString());
			xmlElement.AppendChild(newChild);
			rootElement.AppendChild(xmlElement);
		}

		private void CreateElementLong(XmlDocument doc, XmlElement rootElement, string name, long value)
		{
			XmlElement xmlElement = doc.CreateElement(name);
			XmlText newChild = doc.CreateTextNode(value.ToString());
			xmlElement.AppendChild(newChild);
			rootElement.AppendChild(xmlElement);
		}

		private void CreateElementString(XmlDocument doc, XmlElement rootElement, string name, string value)
		{
			XmlElement xmlElement = doc.CreateElement(name);
			string text = value;
			if (value == null || value.Length == 0)
			{
				text = string.Empty;
			}
			XmlText newChild = doc.CreateTextNode(text);
			xmlElement.AppendChild(newChild);
			rootElement.AppendChild(xmlElement);
		}

		private void CraeteElementBool(XmlDocument doc, XmlElement rootElement, string name, bool value)
		{
			XmlElement xmlElement = doc.CreateElement(name);
			XmlText newChild = doc.CreateTextNode((!value) ? "false" : "true");
			xmlElement.AppendChild(newChild);
			rootElement.AppendChild(xmlElement);
		}

		private XmlElement StartArray(XmlDocument doc, string name)
		{
			return doc.CreateElement(name);
		}

		private void EndArray(XmlElement parentElement, XmlElement arrayRoot)
		{
			parentElement.AppendChild(arrayRoot);
		}

		private bool ParseXmlData(string data)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(data);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("SystemData");
			if (xmlNode == null)
			{
				return false;
			}
			m_SystemData.Init();
			m_GameIDData.Init();
			m_SystemData.version = GetIntByXml(xmlNode, "version");
			m_SystemData.bgmVolume = GetIntByXml(xmlNode, "bgmVolume");
			m_SystemData.seVolume = GetIntByXml(xmlNode, "seVolume");
			m_SystemData.achievementIncentiveCount = GetIntByXml(xmlNode, "achievementIncentiveCount");
			m_SystemData.iap = GetIntByXml(xmlNode, "iap");
			m_SystemData.pushNotice = GetBoolByXml(xmlNode, "pushNotice");
			m_SystemData.lightMode = GetBoolByXml(xmlNode, "lightMode");
			m_SystemData.highTexture = GetBoolByXml(xmlNode, "highTexture");
			m_GameIDData.id = GetStringByXml(xmlNode, "gameId");
			m_GameIDData.password = GetStringByXml(xmlNode, "password");
			m_GameIDData.device = GetStringByXml(xmlNode, "device");
			m_GameIDData.takeoverId = GetStringByXml(xmlNode, "takeoverId");
			m_GameIDData.takeoverPassword = GetStringByXml(xmlNode, "takeoverPassword");
			m_SystemData.noahId = GetStringByXml(xmlNode, "noahId");
			m_SystemData.purchasedRecipt = GetStringByXml(xmlNode, "purchasedRecipt");
			m_SystemData.country = GetStringByXml(xmlNode, "country");
			m_SystemData.facebookTime = GetStringByXml(xmlNode, "facebookTime");
			m_SystemData.gameStartTime = GetStringByXml(xmlNode, "gameStartTime");
			m_SystemData.flags = new Bitset32(GetUintByXml(xmlNode, "flags"));
			m_SystemData.itemTutorialFlags = new Bitset32(GetUintByXml(xmlNode, "itemTutorialFlags"));
			m_SystemData.charaTutorialFlags = new Bitset32(GetUintByXml(xmlNode, "charaTutorialFlags"));
			m_SystemData.actionTutorialFlags = new Bitset32(GetUintByXml(xmlNode, "actionTutorialFlags"));
			m_SystemData.quickModeTutorialFlags = new Bitset32(GetUintByXml(xmlNode, "quickModeTutorialFlags"));
			m_SystemData.pushNoticeFlags = new Bitset32(GetUintByXml(xmlNode, "pushNoticeFlags"));
			m_SystemData.deckData = GetStringByXml(xmlNode, "deckData");
			m_SystemData.pictureShowEventId = GetIntByXml(xmlNode, "pictureShowEventId");
			m_SystemData.pictureShowProgress = GetIntByXml(xmlNode, "pictureShowProgress");
			m_SystemData.pictureShowEmergeRaidBossProgress = GetIntByXml(xmlNode, "pictureShowEmergeRaidBossProgress");
			m_SystemData.pictureShowRaidBossFirstBattle = GetIntByXml(xmlNode, "pictureShowRaidBossFirstBattle");
			m_SystemData.currentRaidDrawIndex = GetIntByXml(xmlNode, "currentRaidDrawIndex");
			m_SystemData.raidEntryFlag = GetBoolByXml(xmlNode, "raidEntryFlag");
			m_SystemData.chaoSortType01 = GetIntByXml(xmlNode, "chaoSortType01");
			m_SystemData.chaoSortType02 = GetIntByXml(xmlNode, "chaoSortType02");
			m_SystemData.playCount = GetIntByXml(xmlNode, "playCount");
			m_SystemData.loginRankigTime = GetStringByXml(xmlNode, "loginRankigTime");
			m_SystemData.achievementCancelCount = GetIntByXml(xmlNode, "playGamesCancelCount");
			XmlNode xmlNode2 = xmlNode.SelectSingleNode("fbFriends");
			if (xmlNode2 != null)
			{
				int num = 0;
				while (true)
				{
					string name = "friend" + (num + 1);
					string stringByXml = GetStringByXml(xmlNode2, name);
					if (stringByXml == null)
					{
						break;
					}
					m_SystemData.fbFriends.Add(stringByXml);
					num++;
				}
			}
			if (!m_SystemData.CheckDeck())
			{
				m_SystemData.deckData = SystemData.DeckAllDefalut();
			}
			return true;
		}

		private int GetIntByXml(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = rootNode.SelectSingleNode(name);
			if (xmlNode != null)
			{
				string innerText = xmlNode.InnerText;
				if (innerText != null)
				{
					return int.Parse(innerText);
				}
			}
			return 0;
		}

		private uint GetUintByXml(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = rootNode.SelectSingleNode(name);
			if (xmlNode != null)
			{
				string innerText = xmlNode.InnerText;
				if (innerText != null)
				{
					return uint.Parse(innerText);
				}
			}
			return 0u;
		}

		private string GetStringByXml(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = rootNode.SelectSingleNode(name);
			if (xmlNode != null)
			{
				return xmlNode.InnerText;
			}
			return null;
		}

		private bool GetBoolByXml(XmlNode rootNode, string name)
		{
			XmlNode xmlNode = rootNode.SelectSingleNode(name);
			if (xmlNode != null)
			{
				string innerText = xmlNode.InnerText;
				if (innerText != null)
				{
					return innerText.Equals("true") ? true : false;
				}
			}
			return false;
		}

		public bool SystemFileCheck()
		{
			return File.Exists(getSavePath() + "/sfrn.game");
		}

		public void DeleteSystemFile()
		{
			if (SystemFileCheck())
			{
				File.Delete(getSavePath() + "/sfrn.game");
			}
			m_SystemData.Init();
			m_GameIDData.Init();
			SaveGameIDDataToPlayerPrefs();
		}

		public ErrorCode GetErrorCode()
		{
			return m_errorcode;
		}

		public void SaveGameIDDataToPlayerPrefs()
		{
			if (m_GameIDData != null)
			{
				if (!string.IsNullOrEmpty(m_GameIDData.id) && !m_GameIDData.id.Equals("0"))
				{
					string value = AESCrypt.Encrypt(m_GameIDData.id);
					PlayerPrefs.SetString("aa7329ab4330306fbdd6dbe9b85c96be", value);
				}
				else
				{
					PlayerPrefs.DeleteKey("aa7329ab4330306fbdd6dbe9b85c96be");
				}
				if (!string.IsNullOrEmpty(m_GameIDData.password))
				{
					string value2 = AESCrypt.Encrypt(m_GameIDData.password);
					PlayerPrefs.SetString("48521cd1266052bfc25718720e91fa83", value2);
				}
				else
				{
					PlayerPrefs.DeleteKey("48521cd1266052bfc25718720e91fa83");
				}
				PlayerPrefs.Save();
			}
		}

		public bool GetGameIDDataFromPlayerPrefs()
		{
			if (m_GameIDData == null)
			{
				return false;
			}
			string @string = PlayerPrefs.GetString("aa7329ab4330306fbdd6dbe9b85c96be");
			if (!string.IsNullOrEmpty(@string))
			{
				m_GameIDData.id = AESCrypt.Decrypt(@string);
			}
			else
			{
				m_GameIDData.id = "0";
			}
			string string2 = PlayerPrefs.GetString("48521cd1266052bfc25718720e91fa83");
			if (!string.IsNullOrEmpty(string2))
			{
				m_GameIDData.password = AESCrypt.Decrypt(string2);
			}
			else
			{
				m_GameIDData.password = string.Empty;
			}
			return true;
		}

		public bool CheckCmSkipCount()
		{
			bool result = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				ServerSettingState settingState = ServerInterface.SettingState;
				if (settingState != null && m_SystemData.playCount >= settingState.m_cmSkipCount)
				{
					result = true;
				}
			}
			return result;
		}

		public void AddPlayCount()
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (!(loggedInServerInterface != null))
			{
				return;
			}
			ServerSettingState settingState = ServerInterface.SettingState;
			if (settingState != null)
			{
				int cmSkipCount = settingState.m_cmSkipCount;
				int playCount = m_SystemData.playCount;
				if (playCount < cmSkipCount)
				{
					playCount++;
					m_SystemData.playCount = playCount;
					Debug.Log("SystemSaveManager:AddPlayCount >>> " + playCount + "/" + cmSkipCount);
					Save();
				}
			}
		}

		private bool CheckInstance()
		{
			if (instance == null)
			{
				instance = this;
				return true;
			}
			if (this == Instance)
			{
				return true;
			}
			UnityEngine.Object.Destroy(base.gameObject);
			return false;
		}
	}
}
