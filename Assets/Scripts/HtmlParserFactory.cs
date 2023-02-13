using UnityEngine;

public class HtmlParserFactory
{
	public static GameObject Create(string url, HtmlParser.SyncType loadSyncType, HtmlParser.SyncType parseSyncType)
	{
		GameObject gameObject = new GameObject("HtmlParser");
		HtmlParser htmlParser = gameObject.AddComponent<HtmlParser>();
		if (htmlParser != null)
		{
			htmlParser.Setup(url, loadSyncType, parseSyncType);
		}
		return gameObject;
	}

	public static string GetUrlString(string url)
	{
		GameObject gameObject = Create(url, HtmlParser.SyncType.TYPE_SYNC, HtmlParser.SyncType.TYPE_SYNC);
		if (gameObject == null)
		{
			return null;
		}
		HtmlParser component = gameObject.GetComponent<HtmlParser>();
		if (component == null)
		{
			return null;
		}
		return component.ParsedString;
	}
}
