using UnityEngine;

public class HtmlWindow : MonoBehaviour
{
	private enum EventSignal
	{
		SIG_PLAYSTART = 100
	}

	private GameObject m_parserObject;

	private bool m_isSetup;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_isSetup)
		{
			HtmlParser component = m_parserObject.GetComponent<HtmlParser>();
			if (!(component == null) && component.IsEndParse)
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.buttonType = GeneralWindow.ButtonType.Ok;
				info.caption = "利用規約";
				info.message = component.ParsedString;
				GeneralWindow.Create(info);
				m_isSetup = false;
			}
		}
	}

	private void PlayStartSync()
	{
		string urlString = HtmlParserFactory.GetUrlString("http://puyopuyoquest.sega-net.com/rule/index.html");
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = "利用規約";
		info.message = urlString;
		GeneralWindow.Create(info);
		m_isSetup = false;
	}

	private void PlayStartASync()
	{
		m_parserObject = HtmlParserFactory.Create("http://puyopuyoquest.sega-net.com/rule/index.html", HtmlParser.SyncType.TYPE_ASYNC, HtmlParser.SyncType.TYPE_ASYNC);
		m_isSetup = true;
	}
}
