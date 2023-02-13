using System.Collections;
using UnityEngine;

public class HtmlParser : MonoBehaviour
{
	public enum SyncType
	{
		TYPE_SYNC,
		TYPE_ASYNC
	}

	private HtmlLoader m_loader;

	private SyncType m_loadSyncType;

	private SyncType m_parseSyncType;

	private TinyFsmBehavior m_fsm;

	private bool m_isEndParse;

	private string m_parsedString;

	private string m_url;

	public bool IsEndParse
	{
		get
		{
			return m_isEndParse;
		}
	}

	public string ParsedString
	{
		get
		{
			return m_parsedString;
		}
	}

	public void Setup(string url, SyncType loadSyncType, SyncType parseSyncType)
	{
		m_loadSyncType = loadSyncType;
		m_parseSyncType = parseSyncType;
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm == null)
		{
			return;
		}
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.ignoreDeltaTime = true;
		m_isEndParse = false;
		switch (m_loadSyncType)
		{
		case SyncType.TYPE_SYNC:
			switch (m_parseSyncType)
			{
			case SyncType.TYPE_SYNC:
				m_loader = base.gameObject.AddComponent<HtmlLoaderSync>();
				m_loader.Setup(url);
				ParseSync(m_loader.GetUrlContentsText());
				description.initState = new TinyFsmState(StateIdle);
				break;
			case SyncType.TYPE_ASYNC:
				m_loader = base.gameObject.AddComponent<HtmlLoaderSync>();
				m_loader.Setup(url);
				description.initState = new TinyFsmState(StateParseHtml);
				break;
			}
			break;
		case SyncType.TYPE_ASYNC:
			switch (m_parseSyncType)
			{
			case SyncType.TYPE_SYNC:
				m_loader = base.gameObject.AddComponent<HtmlLoaderASync>();
				m_loader.Setup(url);
				description.initState = new TinyFsmState(StateLoadHtml);
				break;
			case SyncType.TYPE_ASYNC:
				m_loader = base.gameObject.AddComponent<HtmlLoaderASync>();
				m_loader.Setup(url);
				description.initState = new TinyFsmState(StateLoadHtml);
				break;
			}
			break;
		}
		m_loader.Setup(url);
		m_fsm.SetUp(description);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private TinyFsmState StateLoadHtml(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_loader != null && m_loader.IsEndLoad)
			{
				m_fsm.ChangeState(new TinyFsmState(StateParseHtml));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateParseHtml(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			switch (m_parseSyncType)
			{
			case SyncType.TYPE_SYNC:
				ParseSync(m_loader.GetUrlContentsText());
				break;
			case SyncType.TYPE_ASYNC:
				StartCoroutine(ParseASync(m_loader.GetUrlContentsText()));
				break;
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isEndParse)
			{
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void ParseSync(string htmlString)
	{
		BeginParse(htmlString);
		while (!Parse())
		{
		}
		EndParse();
	}

	private IEnumerator ParseASync(string htmlString)
	{
		BeginParse(htmlString);
		while (!Parse())
		{
			yield return null;
		}
		EndParse();
	}

	private void BeginParse(string htmlString)
	{
		if (htmlString != null)
		{
			string text = "<body>";
			int num = htmlString.IndexOf(text);
			m_parsedString = htmlString.Remove(0, num + text.Length);
		}
	}

	private bool Parse()
	{
		int num = m_parsedString.IndexOf("<");
		int num2 = m_parsedString.IndexOf(">");
		if (num < 0 || num2 < 0)
		{
			return true;
		}
		int count = num2 + 1 - num;
		m_parsedString = m_parsedString.Remove(num, count);
		return false;
	}

	private void EndParse()
	{
		m_isEndParse = true;
	}
}
