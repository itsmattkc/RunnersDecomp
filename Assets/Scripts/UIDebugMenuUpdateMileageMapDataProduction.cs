using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class UIDebugMenuUpdateMileageMapDataProduction : UIDebugMenuTask
{
	private enum TextType
	{
		EPISODE,
		CHAPTER,
		EVENT_FLAG,
		LOG,
		NUM
	}

	public class DebugData
	{
		public int m_episode;

		public int m_chapter;

		public int m_interval;

		public bool m_bossFlag;

		public DebugData(int episode, int chapter, int interval, bool bossFlag)
		{
			m_episode = episode;
			m_chapter = chapter;
			m_interval = interval;
			m_bossFlag = bossFlag;
		}

		public DebugData(string data)
		{
			string[] array = data.Split('_');
			if (array.Length == 4)
			{
				m_episode = int.Parse(array[0]);
				m_chapter = int.Parse(array[1]);
				m_interval = int.Parse(array[2]);
				m_bossFlag = ((array[3] == "1") ? true : false);
			}
		}
	}

	private const int FAR_DISTANCE = 99999999;

	[SerializeField]
	private TextAsset m_dataXml;

	private MileageDebugData m_mileageDebugData;

	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField[] m_TextFields = new UIDebugMenuTextField[4];

	private string[] DefaultTextList = new string[4]
	{
		"Story/話",
		"Chapter/章",
		"evntFlag 0 or 1",
		"log"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(100f, 150f, 150f, 50f),
		new Rect(100f, 250f, 150f, 50f),
		new Rect(300f, 150f, 150f, 50f),
		new Rect(300f, 300f, 250f, 50f)
	};

	private string[] textFieldDefault = new string[4]
	{
		"2",
		"1",
		"0",
		string.Empty
	};

	private NetDebugUpdMileageData m_updMileageData;

	private List<DebugData> m_debaguDatas = new List<DebugData>();

	private void DataRead()
	{
		string text = m_dataXml.text;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(MileageDebugData[]));
		StringReader textReader = new StringReader(text);
		MileageDebugData[] array = (MileageDebugData[])xmlSerializer.Deserialize(textReader);
		m_mileageDebugData = array[0];
		for (int i = 0; i < m_mileageDebugData.data.Length; i++)
		{
			m_debaguDatas.Add(new DebugData(m_mileageDebugData.data[i]));
		}
	}

	protected override void OnStartFromTask()
	{
		DataRead();
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 50f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(400f, 50f, 150f, 50f), "Decide", base.gameObject);
		for (int i = 0; i < 4; i++)
		{
			m_TextFields[i] = base.gameObject.AddComponent<UIDebugMenuTextField>();
			m_TextFields[i].Setup(RectList[i], DefaultTextList[i]);
			m_TextFields[i].text = textFieldDefault[i];
		}
	}

	protected override void OnTransitionTo()
	{
		if (m_backButton != null)
		{
			m_backButton.SetActive(false);
		}
		if (m_decideButton != null)
		{
			m_decideButton.SetActive(false);
		}
		for (int i = 0; i < 4; i++)
		{
			if (!(m_TextFields[i] == null))
			{
				m_TextFields[i].SetActive(false);
			}
		}
	}

	protected override void OnTransitionFrom()
	{
		if (m_backButton != null)
		{
			m_backButton.SetActive(true);
		}
		if (m_decideButton != null)
		{
			m_decideButton.SetActive(true);
		}
		for (int i = 0; i < 4; i++)
		{
			if (!(m_TextFields[i] == null))
			{
				m_TextFields[i].SetActive(true);
			}
		}
	}

	private void OnClicked(string name)
	{
		if (name == "Back")
		{
			TransitionToParent();
		}
		else
		{
			if (!(name == "Decide"))
			{
				return;
			}
			for (int i = 0; i < 4; i++)
			{
				if (i != 3)
				{
					UIDebugMenuTextField uIDebugMenuTextField = m_TextFields[i];
					int result;
					if (!(uIDebugMenuTextField == null) && !int.TryParse(uIDebugMenuTextField.text, out result))
					{
						return;
					}
				}
			}
			int episode = int.Parse(m_TextFields[0].text);
			int chapter = int.Parse(m_TextFields[1].text);
			if (CheckExistData(episode, chapter))
			{
				m_TextFields[3].text = "success";
				bool flag = int.Parse(m_TextFields[2].text) != 0;
				bool bossFlag = GetBossFlag(episode, chapter);
				int interval = GetInterval(episode, chapter);
				int num = interval * 5;
				int num2 = 5;
				if (flag && !bossFlag)
				{
					num--;
					num2--;
				}
				ServerMileageMapState serverMileageMapState = new ServerMileageMapState();
				serverMileageMapState.m_episode = episode;
				serverMileageMapState.m_chapter = chapter;
				serverMileageMapState.m_point = (flag ? num2 : 0);
				serverMileageMapState.m_stageTotalScore = (flag ? num : 0);
				ServerMileageMapState oldServerState = serverMileageMapState;
				ServerMileageMapState serverMileageMapState2 = new ServerMileageMapState();
				if (flag)
				{
					int nextDataIndex = GetNextDataIndex(episode, chapter);
					if (nextDataIndex != -1 && nextDataIndex < m_debaguDatas.Count)
					{
						serverMileageMapState2.m_episode = m_debaguDatas[nextDataIndex].m_episode;
						serverMileageMapState2.m_chapter = m_debaguDatas[nextDataIndex].m_chapter;
						serverMileageMapState2.m_point = 0;
						serverMileageMapState2.m_stageTotalScore = 0L;
						serverMileageMapState2.m_numBossAttack = 0;
						serverMileageMapState2.m_stageMaxScore = 0L;
					}
				}
				else if (bossFlag)
				{
					serverMileageMapState2.m_episode = episode;
					serverMileageMapState2.m_chapter = chapter;
					serverMileageMapState2.m_point = 5;
					serverMileageMapState2.m_stageTotalScore = num;
					serverMileageMapState2.m_numBossAttack = 0;
					serverMileageMapState2.m_stageMaxScore = 0L;
				}
				else
				{
					int nextDataIndex2 = GetNextDataIndex(episode, chapter);
					if (nextDataIndex2 != -1 && nextDataIndex2 < m_debaguDatas.Count)
					{
						serverMileageMapState2.m_episode = m_debaguDatas[nextDataIndex2].m_episode;
						serverMileageMapState2.m_chapter = m_debaguDatas[nextDataIndex2].m_chapter;
						serverMileageMapState2.m_point = 0;
						serverMileageMapState2.m_stageTotalScore = 0L;
						serverMileageMapState2.m_numBossAttack = 0;
						serverMileageMapState2.m_stageMaxScore = 0L;
					}
				}
				bool bossDestroy = false;
				if (flag && bossFlag)
				{
					bossDestroy = true;
				}
				CreateResultInfo(oldServerState, serverMileageMapState2, bossDestroy);
				m_updMileageData = new NetDebugUpdMileageData(serverMileageMapState2);
				m_updMileageData.Request();
			}
			else
			{
				m_TextFields[3].text = "error!  not data [" + episode + "-" + chapter + "]";
			}
		}
	}

	private void CreateResultInfo(ServerMileageMapState oldServerState, ServerMileageMapState newServerState, bool bossDestroy)
	{
		ResultInfo resultInfo = ResultInfo.CreateResultInfo();
		resultInfo.ResetData();
		ResultData info = resultInfo.GetInfo();
		info.m_validResult = true;
		info.m_bossStage = bossDestroy;
		info.m_bossDestroy = bossDestroy;
		info.m_oldMapState = new MileageMapState
		{
			m_episode = oldServerState.m_episode,
			m_chapter = oldServerState.m_chapter,
			m_point = oldServerState.m_point,
			m_score = oldServerState.m_stageTotalScore
		};
		info.m_newMapState = new MileageMapState
		{
			m_episode = newServerState.m_episode,
			m_chapter = newServerState.m_chapter,
			m_point = newServerState.m_point,
			m_score = newServerState.m_stageTotalScore
		};
		resultInfo.SetInfo(info);
	}

	private bool CheckExistData(int episode, int chapter)
	{
		for (int i = 0; i < m_debaguDatas.Count; i++)
		{
			if (m_debaguDatas[i].m_episode == episode && m_debaguDatas[i].m_chapter == chapter)
			{
				return true;
			}
		}
		return false;
	}

	private int GetNextDataIndex(int episode, int chapter)
	{
		int num = -1;
		for (int i = 0; i < m_debaguDatas.Count; i++)
		{
			if (m_debaguDatas[i].m_episode == episode && m_debaguDatas[i].m_chapter == chapter)
			{
				num = i;
				break;
			}
		}
		if (num != -1 && num + 1 < m_debaguDatas.Count)
		{
			num++;
		}
		return num;
	}

	private int GetInterval(int episode, int chapter)
	{
		for (int i = 0; i < m_debaguDatas.Count; i++)
		{
			if (m_debaguDatas[i].m_episode == episode && m_debaguDatas[i].m_chapter == chapter)
			{
				return m_debaguDatas[i].m_interval;
			}
		}
		return 0;
	}

	private bool GetBossFlag(int episode, int chapter)
	{
		for (int i = 0; i < m_debaguDatas.Count; i++)
		{
			if (m_debaguDatas[i].m_episode == episode && m_debaguDatas[i].m_chapter == chapter)
			{
				return m_debaguDatas[i].m_bossFlag;
			}
		}
		return false;
	}
}
