using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuUpdateMileageData : UIDebugMenuTask
{
	private enum TextType
	{
		EPISODE,
		CHAPTER,
		POINT,
		MAP_DISTANCE,
		NUM_BOSS_ATTACK,
		STAGE_DISTANCE,
		NUM
	}

	private UIDebugMenuButton m_backButton;

	private UIDebugMenuButton m_decideButton;

	private UIDebugMenuTextField[] m_TextFields = new UIDebugMenuTextField[6];

	private string[] DefaultTextList = new string[6]
	{
		"Story/話",
		"Chapter/章",
		"Point/ポイント",
		"Score On Map.",
		"Boss' Remain Life",
		"Target Score In Chapter"
	};

	private List<Rect> RectList = new List<Rect>
	{
		new Rect(200f, 200f, 250f, 50f),
		new Rect(500f, 200f, 250f, 50f),
		new Rect(200f, 300f, 250f, 50f),
		new Rect(500f, 300f, 250f, 50f),
		new Rect(200f, 400f, 250f, 50f),
		new Rect(500f, 400f, 250f, 50f)
	};

	private string[] textFieldDefault = new string[6]
	{
		"2",
		"1",
		"0",
		"0",
		"3",
		"0"
	};

	private NetDebugUpdMileageData m_updMileageData;

	protected override void OnStartFromTask()
	{
		m_backButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_backButton.Setup(new Rect(200f, 100f, 150f, 50f), "Back", base.gameObject);
		m_decideButton = base.gameObject.AddComponent<UIDebugMenuButton>();
		m_decideButton.Setup(new Rect(200f, 450f, 150f, 50f), "Decide", base.gameObject);
		for (int i = 0; i < 6; i++)
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
		for (int i = 0; i < 6; i++)
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
		for (int i = 0; i < 6; i++)
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
			for (int i = 0; i < 6; i++)
			{
				UIDebugMenuTextField uIDebugMenuTextField = m_TextFields[i];
				int result;
				if (!(uIDebugMenuTextField == null) && !int.TryParse(uIDebugMenuTextField.text, out result))
				{
					return;
				}
			}
			ServerMileageMapState serverMileageMapState = new ServerMileageMapState();
			serverMileageMapState.m_episode = int.Parse(m_TextFields[0].text);
			serverMileageMapState.m_chapter = int.Parse(m_TextFields[1].text);
			serverMileageMapState.m_point = int.Parse(m_TextFields[2].text);
			serverMileageMapState.m_stageTotalScore = int.Parse(m_TextFields[3].text);
			serverMileageMapState.m_numBossAttack = int.Parse(m_TextFields[4].text);
			serverMileageMapState.m_stageMaxScore = int.Parse(m_TextFields[5].text);
			m_updMileageData = new NetDebugUpdMileageData(serverMileageMapState);
			m_updMileageData.Request();
		}
	}
}
