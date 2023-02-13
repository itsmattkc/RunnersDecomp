using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;

namespace SaveData
{
	public class InformationSaveManager : MonoBehaviour
	{
		public enum ErrorCode
		{
			NO_ERROR,
			FILE_NOT_EXIST,
			FILE_CANNOT_OPEN,
			DATA_INVALID
		}

		private const string INFORMATION_FILE_NAME = "ifrn";

		private const string EXTENSION = ".game";

		private const string XmlRootName = "InformationData";

		private InformationData m_informationData = new InformationData();

		private ErrorCode m_errorcode;

		private static InformationSaveManager instance;

		public static InformationSaveManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (UnityEngine.Object.FindObjectOfType(typeof(InformationSaveManager)) as InformationSaveManager);
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
			m_informationData.Init();
			if (!InformationFileCheck())
			{
				m_informationData.m_isDirty = true;
				SaveInformationData();
			}
			else
			{
				LoadInfomationData();
			}
		}

		public static InformationData GetInformationSaveData()
		{
			if (instance == null)
			{
				return null;
			}
			return instance.GetInformationData();
		}

		public InformationData GetInformationData()
		{
			return m_informationData;
		}

		private string getSavePath()
		{
			return Application.persistentDataPath;
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

		public void SaveInformationData()
		{
			if (!m_informationData.m_isDirty)
			{
				return;
			}
			string path = getSavePath() + "/ifrn.game";
			using (Stream stream = File.Open(path, FileMode.Create))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					try
					{
						XmlDocument xmlDocument = CreateXmlData();
						string text = AESCrypt.Encrypt(xmlDocument.InnerXml);
						string hashData = GetHashData(text);
						streamWriter.Write(hashData + "\n");
						streamWriter.Write(text);
						m_errorcode = ErrorCode.NO_ERROR;
					}
					catch
					{
						m_errorcode = ErrorCode.FILE_CANNOT_OPEN;
					}
				}
			}
		}

		public void LoadInfomationData()
		{
			//Discarded unreachable code: IL_0037
			Stream stream = null;
			try
			{
				stream = File.Open(getSavePath() + "/ifrn.game", FileMode.Open);
			}
			catch
			{
				m_errorcode = ErrorCode.FILE_NOT_EXIST;
				if (stream != null)
				{
					stream.Dispose();
				}
				return;
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
						string streamData = AESCrypt.Decrypt(text2);
						ParseXmlData(streamData);
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
		}

		private XmlDocument CreateXmlData()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDocument.AppendChild(newChild);
			XmlElement xmlElement = xmlDocument.CreateElement("InformationData");
			xmlDocument.AppendChild(xmlElement);
			int num = m_informationData.DataCount();
			for (int i = 0; i < num; i++)
			{
				CreateElementString(xmlDocument, xmlElement, "string", m_informationData.TextArray[i]);
			}
			return xmlDocument;
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

		private bool ParseXmlData(string streamData)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(streamData);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("InformationData");
			if (xmlNode == null)
			{
				return false;
			}
			XmlNodeList xmlNodeList = xmlNode.SelectNodes("string");
			if (xmlNodeList == null)
			{
				return false;
			}
			m_informationData.Init();
			int count = xmlNodeList.Count;
			for (int i = 0; i < count; i++)
			{
				m_informationData.TextArray.Add(xmlNodeList.Item(i).InnerText);
			}
			int num = m_informationData.DataCount();
			InformationImageManager informationImageManager = InformationImageManager.Instance;
			for (int j = 0; j < num; j++)
			{
				string data = m_informationData.GetData(j, InformationData.DataType.ID);
				if (!(data != InformationData.INVALID_PARAM) || long.Parse(data) == NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID || long.Parse(data) == NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID || long.Parse(data) == NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID)
				{
					continue;
				}
				string text = m_informationData.GetData(j, InformationData.DataType.ADD_INFO);
				if (text.Length > 11)
				{
					text = "-1";
				}
				DateTime localDateTime = NetUtil.GetLocalDateTime(long.Parse(text));
				DateTime localDateTime2 = NetUtil.GetLocalDateTime(NetUtil.GetCurrentUnixTime());
				if (localDateTime2 > localDateTime)
				{
					if (informationImageManager != null)
					{
						string data2 = m_informationData.GetData(j, InformationData.DataType.IMAGE_ID);
						informationImageManager.DeleteImageData(data2);
					}
					m_informationData.Reset(j);
				}
			}
			return true;
		}

		private string GetStringByXml(XmlNode rootNode)
		{
			XmlNode xmlNode = rootNode.SelectSingleNode("string");
			if (xmlNode != null)
			{
				return xmlNode.InnerText;
			}
			return null;
		}

		public bool InformationFileCheck()
		{
			return File.Exists(getSavePath() + "/ifrn.game");
		}

		public void DeleteInformationFile()
		{
			if (InformationFileCheck())
			{
				File.Delete(getSavePath() + "/ifrn.game");
			}
		}

		private ErrorCode GetErrorCode()
		{
			return m_errorcode;
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
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
