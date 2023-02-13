using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class HudLoadingCharaInfo : MonoBehaviour
{
	private struct CharaInfo
	{
		public bool isReady;

		public string type;

		public string name;

		public Texture2D picture;

		public string explain;

		public string explainCaption;
	}

	private const int WebParamCount = 5;

	private List<CharaInfo> m_charaInfoList;

	private Dictionary<string, Texture2D> m_pictureStack;

	private int m_currentCharaIndex;

	private int m_loopCount;

	private bool m_isStartCoroutine;

	private bool m_isErrorRestartCoroutine;

	private void Awake()
	{
		m_charaInfoList = new List<CharaInfo>();
		m_pictureStack = new Dictionary<string, Texture2D>();
		m_loopCount = 0;
		m_currentCharaIndex = 0;
		m_isStartCoroutine = false;
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		m_charaInfoList.Clear();
		m_pictureStack.Clear();
		m_loopCount = 0;
		m_currentCharaIndex = 0;
		m_isStartCoroutine = false;
	}

	private void Update()
	{
		if (!m_isStartCoroutine)
		{
			StartCoroutine(LoadWWW());
			m_isStartCoroutine = true;
			m_isErrorRestartCoroutine = false;
			return;
		}
		if (NetworkErrorWindow.IsCreated("NetworkErrorReload") || NetworkErrorWindow.IsCreated("NetworkErrorRetry"))
		{
			m_isErrorRestartCoroutine = true;
			return;
		}
		if (m_isErrorRestartCoroutine)
		{
			m_isStartCoroutine = false;
		}
		m_isErrorRestartCoroutine = false;
	}

	private IEnumerator LoadWWW()
	{
		m_loopCount = 0;
		if (TitleUtil.initUser)
		{
			m_currentCharaIndex = 0;
		}
		else
		{
			m_currentCharaIndex = 1;
		}
		string url = string.Empty;
		string serverUrl = NetBaseUtil.InformationServerURL;
		url = serverUrl + "title_load/title_load_index_" + TextUtility.GetSuffixe() + ".html";
		Debug.Log("HudLoadingCharaInfo LoadWWW url:" + url + " !!!!");
		GameObject gameObjectParser = HtmlParserFactory.Create(url, HtmlParser.SyncType.TYPE_ASYNC, HtmlParser.SyncType.TYPE_ASYNC);
		if (!(gameObjectParser != null))
		{
			yield break;
		}
		HtmlParser parser = gameObjectParser.GetComponent<HtmlParser>();
		if (!(parser != null))
		{
			yield break;
		}
		while (!parser.IsEndParse)
		{
			yield return null;
		}
		string result = parser.ParsedString;
		string[] contents = result.Split(']');
		for (int index3 = 0; index3 < contents.Length; index3++)
		{
			contents[index3] = contents[index3].Remove(0, 2);
		}
		int paramCountMax = 5;
		if (contents.Length < 5)
		{
			paramCountMax = 4;
		}
		else
		{
			bool typeKeyFound = false;
			for (int index2 = 2; index2 < 5; index2++)
			{
				if (contents[index2] == "howtoplay" || contents[index2] == "Howtoplay")
				{
					typeKeyFound = true;
				}
				else if (contents[index2] == "player" || contents[index2] == "Player")
				{
					typeKeyFound = true;
				}
				else if (contents[index2] == "chara" || contents[index2] == "Chara")
				{
					typeKeyFound = true;
				}
				else if (contents[index2] == "object" || contents[index2] == "Object")
				{
					typeKeyFound = true;
				}
			}
			if (!typeKeyFound)
			{
				paramCountMax = 4;
			}
		}
		int contentsLength = contents.Length / paramCountMax;
		for (int index = 0; index < contentsLength; index++)
		{
			CharaInfo charaInfo = new CharaInfo
			{
				isReady = false
			};
			int contentsIndex = paramCountMax * index;
			charaInfo.name = contents[contentsIndex];
			string pictureUrl = serverUrl + "pictures/title/" + contents[contentsIndex + 1];
			bool loadingFlg = true;
			if (m_pictureStack.Count > 0 && m_pictureStack.ContainsKey(pictureUrl))
			{
				loadingFlg = false;
			}
			if (loadingFlg)
			{
				WWW wwwPicture = new WWW(pictureUrl);
				yield return wwwPicture;
				charaInfo.picture = ((!(wwwPicture.texture != null)) ? null : wwwPicture.texture);
				m_pictureStack.Add(pictureUrl, wwwPicture.texture);
				wwwPicture.Dispose();
			}
			else
			{
				yield return null;
				charaInfo.picture = m_pictureStack[pictureUrl];
			}
			charaInfo.explain = contents[contentsIndex + 2];
			charaInfo.explainCaption = contents[contentsIndex + 3];
			if (paramCountMax == 5)
			{
				charaInfo.type = contents[contentsIndex + 4];
			}
			else
			{
				charaInfo.type = "player";
			}
			charaInfo.isReady = true;
			m_charaInfoList.Add(charaInfo);
		}
	}

	public int GetCharaCount()
	{
		return m_charaInfoList.Count;
	}

	public bool IsReady()
	{
		if (m_currentCharaIndex >= m_charaInfoList.Count)
		{
			return false;
		}
		CharaInfo charaInfo = m_charaInfoList[m_currentCharaIndex];
		return charaInfo.isReady;
	}

	public void GoNext()
	{
		m_currentCharaIndex++;
		if (m_currentCharaIndex >= m_charaInfoList.Count && m_charaInfoList.Count > 0)
		{
			m_currentCharaIndex = 0;
			m_loopCount++;
		}
		if (m_loopCount <= 0)
		{
			return;
		}
		for (int i = 0; i < m_charaInfoList.Count; i++)
		{
			CharaInfo charaInfo = m_charaInfoList[(m_currentCharaIndex + i) % m_charaInfoList.Count];
			if (charaInfo.type != "howtoplay" && charaInfo.type != "Howtoplay")
			{
				m_currentCharaIndex = (m_currentCharaIndex + i) % m_charaInfoList.Count;
				break;
			}
		}
	}

	public string GetCharaName()
	{
		if (m_currentCharaIndex >= m_charaInfoList.Count)
		{
			return string.Empty;
		}
		CharaInfo charaInfo = m_charaInfoList[m_currentCharaIndex];
		return charaInfo.name;
	}

	public string GetTypeName()
	{
		if (m_currentCharaIndex >= m_charaInfoList.Count)
		{
			return string.Empty;
		}
		CharaInfo charaInfo = m_charaInfoList[m_currentCharaIndex];
		return charaInfo.type;
	}

	public Texture2D GetCharaPicture()
	{
		if (m_currentCharaIndex >= m_charaInfoList.Count)
		{
			return null;
		}
		CharaInfo charaInfo = m_charaInfoList[m_currentCharaIndex];
		return charaInfo.picture;
	}

	public string GetCharaExplain()
	{
		if (m_currentCharaIndex >= m_charaInfoList.Count)
		{
			return string.Empty;
		}
		CharaInfo charaInfo = m_charaInfoList[m_currentCharaIndex];
		return charaInfo.explain;
	}

	public string GetCharaExplainCaption()
	{
		if (m_currentCharaIndex >= m_charaInfoList.Count)
		{
			return string.Empty;
		}
		CharaInfo charaInfo = m_charaInfoList[m_currentCharaIndex];
		return charaInfo.explainCaption;
	}
}
