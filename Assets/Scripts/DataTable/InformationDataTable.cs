using System.Collections;
using System.IO;
using System.Xml.Serialization;
using Text;
using UnityEngine;

namespace DataTable
{
	public class InformationDataTable : MonoBehaviour
	{
		public enum Type
		{
			COPYRIGHT,
			CREDIT,
			SHOP_LEGAL,
			HELP,
			TERMS_OF_SERVICE,
			FB_FEED_PICTURE_ANDROID,
			FB_FEED_PICTURE_IOS,
			INSTALL_PAGE_ANDROID,
			INSTALL_PAGE_IOS,
			MAINTENANCE_PAGE,
			NUM
		}

		private bool m_checkTime = true;

		private static string[] TypeName = new string[10]
		{
			"copyright",
			"credit",
			"shop_legal",
			"help",
			"terms_of_service",
			"fb_feed_picture_android",
			"fb_feed_picture_ios",
			"install_page_android",
			"install_page_ios",
			"maintenance_page"
		};

		private static InformationData[] m_infoDataTable;

		private GameObject m_returnObject;

		private static InformationDataTable s_instance = null;

		private static bool m_isError = false;

		public static InformationDataTable Instance
		{
			get
			{
				return s_instance;
			}
		}

		public bool Loaded
		{
			get
			{
				return m_infoDataTable != null;
			}
		}

		public static void Create()
		{
			if (s_instance == null)
			{
				GameObject gameObject = new GameObject("InformationDataTable");
				gameObject.AddComponent<InformationDataTable>();
			}
		}

		public void Initialize(GameObject returnObject)
		{
			m_checkTime = false;
			m_returnObject = returnObject;
			StartCoroutine(LoadURL(NetBaseUtil.InformationServerURL + "InformationDataTable.bytes"));
		}

		public bool isError()
		{
			return m_isError;
		}

		private void Awake()
		{
			if (s_instance == null)
			{
				Object.DontDestroyOnLoad(base.gameObject);
				s_instance = this;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			if (s_instance == this)
			{
				s_instance = null;
				m_infoDataTable = null;
			}
		}

		private IEnumerator LoadURL(string url)
		{
			m_isError = false;
			float oldTime = Time.realtimeSinceStartup;
			if (m_checkTime)
			{
				Debug.Log("LS:start install URL: " + url);
			}
			WWWRequest request = new WWWRequest(url);
			request.SetConnectTime(20f);
			while (!request.IsEnd())
			{
				request.Update();
				if (request.IsTimeOut())
				{
					request.Cancel();
					break;
				}
				float startTime = Time.realtimeSinceStartup;
				float spendTime2 = 0f;
				do
				{
					yield return null;
					spendTime2 = Time.realtimeSinceStartup - startTime;
				}
				while (spendTime2 <= 0.1f);
			}
			if (m_checkTime)
			{
				float loadTime = Time.realtimeSinceStartup;
				Debug.Log("LS:Load File: " + url + " Time is " + (loadTime - oldTime));
			}
			if (request.IsTimeOut())
			{
				Debug.LogError("LoadURLKeyData TimeOut. ");
				if (m_returnObject != null)
				{
					m_returnObject.SendMessage("InformationDataLoad_Failed", SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (request.GetError() != null)
			{
				Debug.LogError("LoadURLKeyData Error. " + request.GetError());
				if (m_returnObject != null)
				{
					m_returnObject.SendMessage("InformationDataLoad_Failed", SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				try
				{
					string resultText = request.GetResultString();
					if (resultText != null)
					{
						string text_data = AESCrypt.Decrypt(resultText);
						XmlSerializer serializer = new XmlSerializer(typeof(InformationData[]));
						StringReader sr = new StringReader(text_data);
						m_infoDataTable = (InformationData[])serializer.Deserialize(sr);
					}
					else
					{
						Debug.LogWarning("text load error www.text == null " + url);
						m_isError = true;
					}
				}
				catch
				{
					Debug.LogWarning("error " + url);
					m_isError = true;
				}
				if (m_returnObject != null)
				{
					m_returnObject.SendMessage("InformationDataLoad_Succeed", SendMessageOptions.DontRequireReceiver);
				}
			}
			request.Remove();
		}

		public static InformationData[] GetDataTable()
		{
			return m_infoDataTable;
		}

		public static string GetUrl(Type type)
		{
			if (m_infoDataTable != null && (uint)type < (uint)TypeName.Length)
			{
				InformationData[] infoDataTable = m_infoDataTable;
				foreach (InformationData informationData in infoDataTable)
				{
					if (informationData.tag == TypeName[(int)type] && informationData.sfx == TextUtility.GetSuffixe())
					{
						Debug.Log("GetUrl type=" + type.ToString() + " sfx=" + informationData.sfx + " tag=" + informationData.tag + " url=" + informationData.url);
						return informationData.url;
					}
				}
			}
			return string.Empty;
		}
	}
}
