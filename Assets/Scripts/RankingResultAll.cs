using Text;
using UnityEngine;

public class RankingResultAll : MonoBehaviour
{
	private enum Mode
	{
		Idle,
		Wait,
		End
	}

	private Mode m_mode;

	private RankingServerInfoConverter m_rankingData;

	public void Setup(string serverMessageInfo)
	{
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_all_caption").text;
		m_rankingData = new RankingServerInfoConverter(serverMessageInfo);
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "RankingResultAll";
		info.caption = text;
		info.message = m_rankingData.rankingResultAllText;
		info.buttonType = GeneralWindow.ButtonType.Close;
		GeneralWindow.Create(info);
		m_mode = Mode.Wait;
	}

	public bool IsEnd()
	{
		if (m_mode == Mode.Wait)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		Mode mode = m_mode;
		if (mode == Mode.Wait && GeneralWindow.IsCreated("RankingResultAll") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			m_mode = Mode.End;
		}
	}

	public static RankingResultAll Create(string serverMessageInfo)
	{
		GameObject gameObject = GameObject.Find("RankingResultAll");
		RankingResultAll rankingResultAll = null;
		if (gameObject == null)
		{
			gameObject = new GameObject("RankingResultAll");
			rankingResultAll = gameObject.AddComponent<RankingResultAll>();
		}
		else
		{
			rankingResultAll = gameObject.GetComponent<RankingResultAll>();
		}
		if (gameObject != null && rankingResultAll != null)
		{
			rankingResultAll.Setup(serverMessageInfo);
		}
		return rankingResultAll;
	}
}
