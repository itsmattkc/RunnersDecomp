using UnityEngine;

public class HudMainMenuRankingButton
{
	private enum State
	{
		INIT,
		UPDATE,
		NUM
	}

	private GameObject m_mainMenuObject;

	private bool m_isQuickMode;

	private GameObject m_buttonParentObject;

	private static readonly float UPDATE_TIME = 60f;

	private float m_nextUpdateTime;

	private State m_state;

	public void Intialize(GameObject mainMenuObject, bool isQuickMode)
	{
		if (mainMenuObject == null)
		{
			return;
		}
		m_mainMenuObject = mainMenuObject;
		m_isQuickMode = isQuickMode;
		if (isQuickMode)
		{
			m_buttonParentObject = null;
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_mainMenuObject, "Anchor_5_MC");
			if (gameObject == null)
			{
				return;
			}
			m_buttonParentObject = GameObjectUtil.FindChildGameObject(gameObject, "1_Quick");
			if (m_buttonParentObject == null)
			{
				return;
			}
		}
		else
		{
			m_buttonParentObject = null;
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_mainMenuObject, "Anchor_5_MC");
			if (gameObject2 == null)
			{
				return;
			}
			m_buttonParentObject = GameObjectUtil.FindChildGameObject(gameObject2, "0_Endless");
			if (m_buttonParentObject == null)
			{
				return;
			}
		}
		m_nextUpdateTime = UPDATE_TIME;
	}

	public void Update()
	{
		if (m_buttonParentObject == null)
		{
			return;
		}
		switch (m_state)
		{
		case State.INIT:
		{
			bool flag = false;
			if ((!m_isQuickMode) ? GeneralUtil.SetEndlessRankingBtnIcon(m_buttonParentObject) : GeneralUtil.SetQuickRankingBtnIcon(m_buttonParentObject))
			{
				m_state = State.UPDATE;
			}
			break;
		}
		case State.UPDATE:
			m_nextUpdateTime -= Time.deltaTime;
			if (m_nextUpdateTime <= 0f)
			{
				if (m_isQuickMode)
				{
					GeneralUtil.SetQuickRankingTime(m_buttonParentObject);
				}
				else
				{
					GeneralUtil.SetEndlessRankingTime(m_buttonParentObject);
				}
				m_nextUpdateTime = UPDATE_TIME;
			}
			break;
		}
	}
}
