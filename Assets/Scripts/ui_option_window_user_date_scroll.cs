using Text;
using UnityEngine;

public class ui_option_window_user_date_scroll : MonoBehaviour
{
	public enum ResultType
	{
		QUICK_HIGHT_SCORE,
		QUICK_TOTAL_SCORE,
		HIGHT_SCORE,
		TOTAL_SCORE,
		DISTANCE,
		CUMULATIVE_DISTANCE,
		PLAYING_NUM,
		RING,
		RED_RING,
		ANIMAL,
		CHAO_ROULETTE,
		ITEM_ROULETTE,
		JACK_POT_NUM,
		JACK_POT_RING,
		NUM
	}

	public class textInfo
	{
		public string grop;

		public string cell;
	}

	private readonly textInfo[] m_textInfos = new textInfo[14]
	{
		new textInfo
		{
			grop = "Option",
			cell = "quick_high_score"
		},
		new textInfo
		{
			grop = "Option",
			cell = "quick_total_sum_high_score"
		},
		new textInfo
		{
			grop = "Option",
			cell = "endless_high_score"
		},
		new textInfo
		{
			grop = "Option",
			cell = "endless_total_sum_high_score"
		},
		new textInfo
		{
			grop = "Option",
			cell = "maximum_distance"
		},
		new textInfo
		{
			grop = "Option",
			cell = "cumulative_distance"
		},
		new textInfo
		{
			grop = "Option",
			cell = "playing_num"
		},
		new textInfo
		{
			grop = "Option",
			cell = "take_all_rings"
		},
		new textInfo
		{
			grop = "Option",
			cell = "take_all_red_rings"
		},
		new textInfo
		{
			grop = "Option",
			cell = "take_all_red_animals"
		},
		new textInfo
		{
			grop = "Option",
			cell = "chao_roulette_num"
		},
		new textInfo
		{
			grop = "Option",
			cell = "item_roulette_num"
		},
		new textInfo
		{
			grop = "Option",
			cell = "jack_pot_num"
		},
		new textInfo
		{
			grop = "Option",
			cell = "jack_pot_ring"
		}
	};

	[SerializeField]
	private UILabel m_textLabel;

	[SerializeField]
	private UILabel m_socoreLabel;

	private ResultType m_rusultType = ResultType.HIGHT_SCORE;

	private void Start()
	{
		base.enabled = false;
	}

	public void UpdateView(ResultType type, ServerOptionUserResult optionUserResult)
	{
		m_rusultType = type;
		TextInit(optionUserResult);
	}

	public void TextInit(ServerOptionUserResult optionUserResult)
	{
		if (m_rusultType < ResultType.NUM)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, m_textInfos[(int)m_rusultType].grop, m_textInfos[(int)m_rusultType].cell);
			if (m_textLabel != null && text != null)
			{
				m_textLabel.text = text.text;
			}
		}
		if (m_socoreLabel != null)
		{
			m_socoreLabel.text = GetScore(m_rusultType, optionUserResult);
		}
	}

	private static string GetScore(ResultType type, ServerOptionUserResult optionUserResult)
	{
		long num = 0L;
		if (ServerInterface.PlayerState != null && optionUserResult != null)
		{
			switch (type)
			{
			case ResultType.QUICK_HIGHT_SCORE:
				num = ServerInterface.PlayerState.m_totalHighScoreQuick;
				break;
			case ResultType.QUICK_TOTAL_SCORE:
				num = optionUserResult.m_quickTotalSumHightScore;
				break;
			case ResultType.HIGHT_SCORE:
				num = ServerInterface.PlayerState.m_totalHighScore;
				break;
			case ResultType.TOTAL_SCORE:
				num = optionUserResult.m_totalSumHightScore;
				break;
			case ResultType.DISTANCE:
				num = ServerInterface.PlayerState.m_maxDistance;
				break;
			case ResultType.CUMULATIVE_DISTANCE:
				num = ServerInterface.PlayerState.m_totalDistance;
				break;
			case ResultType.PLAYING_NUM:
				num = ServerInterface.PlayerState.m_numPlaying;
				break;
			case ResultType.RING:
				num = optionUserResult.m_numTakeAllRings;
				break;
			case ResultType.RED_RING:
				num = optionUserResult.m_numTakeAllRedRings;
				break;
			case ResultType.ANIMAL:
				num = ServerInterface.PlayerState.m_numAnimals;
				break;
			case ResultType.CHAO_ROULETTE:
				num = optionUserResult.m_numChaoRoulette;
				break;
			case ResultType.ITEM_ROULETTE:
				num = optionUserResult.m_numItemRoulette;
				break;
			case ResultType.JACK_POT_NUM:
				num = optionUserResult.m_numJackPot;
				break;
			case ResultType.JACK_POT_RING:
				num = optionUserResult.m_numMaximumJackPotRings;
				break;
			}
		}
		if (num < 0)
		{
			num = 0L;
		}
		else if (num > 999999999999L)
		{
			num = 999999999999L;
		}
		return HudUtility.GetFormatNumString(num);
	}
}
