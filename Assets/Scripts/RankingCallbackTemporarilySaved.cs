using System.Collections.Generic;

public class RankingCallbackTemporarilySaved
{
	private List<RankingUtil.Ranker> m_rankerList;

	private RankingUtil.RankingScoreType m_score;

	private RankingUtil.RankingRankerType m_type = RankingUtil.RankingRankerType.RIVAL;

	private int m_page;

	private bool m_isNext;

	private bool m_isPrev;

	private bool m_isCashData;

	private RankingManager.CallbackRankingData m_callback;

	public RankingCallbackTemporarilySaved(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData, RankingManager.CallbackRankingData callback)
	{
		m_rankerList = rankerList;
		m_score = score;
		m_type = type;
		m_page = page;
		m_isNext = isNext;
		m_isPrev = isPrev;
		m_isCashData = isCashData;
		m_callback = callback;
	}

	public void SendCallback()
	{
		if (m_callback != null)
		{
			m_callback(m_rankerList, m_score, m_type, m_page, m_isNext, m_isPrev, m_isCashData);
		}
	}
}
