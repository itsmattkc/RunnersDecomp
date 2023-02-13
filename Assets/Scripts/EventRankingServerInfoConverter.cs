using System.Collections.Generic;
using Text;

public class EventRankingServerInfoConverter
{
	public enum MSG_INFO
	{
		EventId,
		PlayerRank,
		ParticipationNum,
		IsPresented,
		StartDt,
		EndDt,
		NUM
	}

	private string m_messageInfo;

	private int m_playerRank = -1;

	private int m_participationNum = -1;

	private int m_eventId = -1;

	private bool m_isPresented;

	public bool isInit
	{
		get
		{
			if (m_messageInfo != null)
			{
				return true;
			}
			return false;
		}
	}

	public int eventId
	{
		get
		{
			return m_eventId;
		}
	}

	public string Result
	{
		get
		{
			if (!isInit)
			{
				return null;
			}
			string text = null;
			text = ((!m_isPresented) ? TextUtility.GetCommonText("Ranking", "ranking_result_event_text_1") : TextUtility.GetCommonText("Ranking", "ranking_result_event_text_2"));
			string eventName = GetEventName();
			return TextUtility.Replaces(text, new Dictionary<string, string>
			{
				{
					"{PARAM1}",
					eventName
				},
				{
					"{PARAM2}",
					m_playerRank.ToString()
				},
				{
					"{PARAM3}",
					m_participationNum.ToString()
				}
			});
		}
	}

	public EventRankingServerInfoConverter(string serverMessageInfo)
	{
		Setup(serverMessageInfo);
	}

	public void Setup(string serverMessageInfo)
	{
		if (serverMessageInfo == null)
		{
			return;
		}
		m_messageInfo = serverMessageInfo;
		string[] array = m_messageInfo.Split(',');
		if (array != null && array.Length > 0)
		{
			if (array.Length > 0)
			{
				m_eventId = int.Parse(array[0]);
			}
			if (array.Length > 1)
			{
				m_playerRank = int.Parse(array[1]);
			}
			else
			{
				m_playerRank = 0;
			}
			if (array.Length > 2)
			{
				m_participationNum = int.Parse(array[2]);
			}
			else
			{
				m_participationNum = 0;
			}
			if (array.Length > 3)
			{
				int num = int.Parse(array[3]);
				m_isPresented = ((num != 0) ? true : false);
			}
			else
			{
				m_isPresented = false;
			}
		}
		else
		{
			m_messageInfo = null;
		}
	}

	public string GetEventName()
	{
		string result = string.Empty;
		switch (EventManager.GetType(m_eventId))
		{
		case EventManager.EventType.SPECIAL_STAGE:
			result = HudUtility.GetEventStageName(EventManager.GetSpecificId(m_eventId));
			break;
		case EventManager.EventType.RAID_BOSS:
			result = HudUtility.GetEventStageName(EventManager.GetSpecificId(m_eventId));
			break;
		case EventManager.EventType.COLLECT_OBJECT:
			switch (EventManager.GetCollectEventType(m_eventId))
			{
			case EventManager.CollectEventType.GET_ANIMALS:
				result = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_animl_get_event");
				break;
			case EventManager.CollectEventType.GET_RING:
				result = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_ring_get_event");
				break;
			case EventManager.CollectEventType.RUN_DISTANCE:
				result = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_run_distance_event");
				break;
			}
			break;
		}
		return result;
	}
}
