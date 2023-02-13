using System;
using System.Collections.Generic;
using Text;

public class RankingServerInfoConverter
{
	public enum MSG_INFO
	{
		ScoreRanking,
		TotalScoreRanking,
		RivalRanking,
		TotalRivalRanking,
		SendPresent,
		StartDt,
		EndDt,
		League,
		OldLeague,
		NumLeagueMember,
		SendPresentRival,
		NUM
	}

	public enum ResultType
	{
		Up,
		Stay,
		Down,
		Error,
		NUM
	}

	private static string ms_lastServerMessageInfo;

	private string m_orgServerMessageInfo;

	private int m_scoreRanking;

	private int m_totalScoreRanking;

	private int m_rivalRanking;

	private int m_totalRivalRanking;

	private int m_sendPresent;

	private DateTime m_startDt;

	private DateTime m_endDt;

	private int m_league;

	private int m_oldLeague;

	private int m_numLeagueMember;

	private int m_sendPresentRival;

	public bool isInit
	{
		get
		{
			if (m_orgServerMessageInfo != null)
			{
				return true;
			}
			return false;
		}
	}

	public ResultType leagueResult
	{
		get
		{
			ResultType result = ResultType.Stay;
			if (!isInit)
			{
				return ResultType.Error;
			}
			if (m_league != m_oldLeague)
			{
				result = ((m_league <= m_oldLeague) ? ResultType.Down : ResultType.Up);
			}
			return result;
		}
	}

	public LeagueType currentLeague
	{
		get
		{
			return (LeagueType)m_league;
		}
	}

	public LeagueType oldLeague
	{
		get
		{
			return (LeagueType)m_oldLeague;
		}
	}

	public DateTime startDt
	{
		get
		{
			return m_startDt;
		}
	}

	public DateTime endDt
	{
		get
		{
			return m_endDt;
		}
	}

	public string rankingResultAllText
	{
		get
		{
			string text = null;
			string text2 = null;
			if (!isInit)
			{
				return null;
			}
			text2 = ((m_sendPresent > 0) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_text_3").text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_text_4").text);
			return TextUtility.Replaces(text2, new Dictionary<string, string>
			{
				{
					"{PARAM1}",
					m_scoreRanking.ToString()
				},
				{
					"{PARAM3}",
					m_totalScoreRanking.ToString()
				}
			});
		}
	}

	public string rankingResultLeagueText
	{
		get
		{
			string text = null;
			string text2 = null;
			if (!isInit)
			{
				return null;
			}
			text2 = ((m_sendPresentRival > 0) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_text_1").text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_text_2").text);
			return TextUtility.Replaces(text2, new Dictionary<string, string>
			{
				{
					"{PARAM1}",
					m_rivalRanking.ToString()
				},
				{
					"{PARAM2}",
					m_numLeagueMember.ToString()
				},
				{
					"{PARAM3}",
					m_totalRivalRanking.ToString()
				},
				{
					"{PARAM4}",
					m_numLeagueMember.ToString()
				}
			});
		}
	}

	public static string lastServerMessageInfo
	{
		get
		{
			return ms_lastServerMessageInfo;
		}
		set
		{
			ms_lastServerMessageInfo = value;
		}
	}

	public RankingServerInfoConverter(string serverMessageInfo)
	{
		Setup(serverMessageInfo);
	}

	public void Setup(string serverMessageInfo)
	{
		m_orgServerMessageInfo = serverMessageInfo;
		ms_lastServerMessageInfo = serverMessageInfo;
		string[] array = m_orgServerMessageInfo.Split(',');
		if (array != null && array.Length > 0)
		{
			Debug.Log("orgServerMessageInfo=" + m_orgServerMessageInfo);
			if (array.Length > 0)
			{
				m_scoreRanking = int.Parse(array[0]);
			}
			if (array.Length > 1)
			{
				m_totalScoreRanking = int.Parse(array[1]);
			}
			else
			{
				m_totalScoreRanking = -1;
			}
			if (array.Length > 2)
			{
				m_rivalRanking = int.Parse(array[2]);
			}
			else
			{
				m_rivalRanking = -1;
			}
			if (array.Length > 3)
			{
				m_totalRivalRanking = int.Parse(array[3]);
			}
			else
			{
				m_totalRivalRanking = -1;
			}
			if (array.Length > 4)
			{
				m_sendPresent = int.Parse(array[4]);
			}
			else
			{
				m_sendPresent = -1;
			}
			if (array.Length > 5)
			{
				m_startDt = NetUtil.GetLocalDateTime(long.Parse(array[5]));
			}
			if (array.Length > 6)
			{
				m_endDt = NetUtil.GetLocalDateTime(long.Parse(array[6]));
			}
			if (array.Length > 7)
			{
				m_league = int.Parse(array[7]);
			}
			else
			{
				m_league = -1;
			}
			if (array.Length > 8)
			{
				m_oldLeague = int.Parse(array[8]);
			}
			else
			{
				m_oldLeague = -1;
			}
			if (array.Length > 9)
			{
				m_numLeagueMember = int.Parse(array[9]);
			}
			else
			{
				m_numLeagueMember = -1;
			}
			if (array.Length > 10)
			{
				m_sendPresentRival = int.Parse(array[10]);
			}
			else
			{
				m_sendPresentRival = -1;
			}
		}
		else
		{
			m_orgServerMessageInfo = null;
		}
	}

	public static RankingServerInfoConverter CreateLastServerInfo()
	{
		RankingServerInfoConverter result = null;
		if (!string.IsNullOrEmpty(ms_lastServerMessageInfo))
		{
			result = new RankingServerInfoConverter(ms_lastServerMessageInfo);
		}
		return result;
	}
}
