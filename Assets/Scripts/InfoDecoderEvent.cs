using Text;

public class InfoDecoderEvent : InfoDecoder
{
	private string m_messageInfo = string.Empty;

	private EventRankingServerInfoConverter m_converter;

	public InfoDecoderEvent(string messageInfo)
	{
		m_messageInfo = messageInfo;
		m_converter = new EventRankingServerInfoConverter(m_messageInfo);
	}

	public override string GetCaption()
	{
		string commonText = TextUtility.GetCommonText("Ranking", "ranking_result_event_caption");
		if (!string.IsNullOrEmpty(commonText))
		{
			return commonText;
		}
		return string.Empty;
	}

	public override string GetResultString()
	{
		return m_converter.Result;
	}

	public override string GetMedalSpriteName()
	{
		return "ui_ranking_world_sonicmedal_blue";
	}
}
