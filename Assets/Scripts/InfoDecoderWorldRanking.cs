using Text;

public class InfoDecoderWorldRanking : InfoDecoder
{
	private string m_messageInfo = string.Empty;

	private RankingServerInfoConverter m_converter;

	public InfoDecoderWorldRanking(string messageInfo)
	{
		m_messageInfo = messageInfo;
		m_converter = new RankingServerInfoConverter(m_messageInfo);
	}

	public override string GetCaption()
	{
		string commonText = TextUtility.GetCommonText("Ranking", "ranking_result_all_caption");
		if (!string.IsNullOrEmpty(commonText))
		{
			return commonText;
		}
		return string.Empty;
	}

	public override string GetResultString()
	{
		return m_converter.rankingResultAllText;
	}

	public override string GetMedalSpriteName()
	{
		return "ui_ranking_world_sonicmedal_blue";
	}
}
