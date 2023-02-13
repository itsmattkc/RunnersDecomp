using UnityEngine;

public class HudDailyBattleButton
{
	private GameObject m_mainMenuObject;

	private GameObject m_quickModeObject;

	private static readonly float UPDATE_TIME = 1f;

	private float m_nextUpdateTime;

	public void Initialize(GameObject mainMenuObject)
	{
		if (mainMenuObject == null)
		{
			return;
		}
		m_mainMenuObject = mainMenuObject;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_mainMenuObject, "Anchor_5_MC");
		if (!(gameObject == null))
		{
			m_quickModeObject = GameObjectUtil.FindChildGameObject(gameObject, "1_Quick");
			if (!(m_quickModeObject == null))
			{
				GeneralUtil.SetDailyBattleBtnIcon(m_quickModeObject);
			}
		}
	}

	public void Update()
	{
		m_nextUpdateTime -= Time.deltaTime;
		if (m_nextUpdateTime <= 0f)
		{
			GeneralUtil.SetDailyBattleTime(m_quickModeObject);
			m_nextUpdateTime = UPDATE_TIME;
		}
	}

	public void UpdateView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_mainMenuObject, "Anchor_5_MC");
		if (!(gameObject == null))
		{
			m_quickModeObject = GameObjectUtil.FindChildGameObject(gameObject, "1_Quick");
			if (!(m_quickModeObject == null))
			{
				GeneralUtil.SetDailyBattleBtnIcon(m_quickModeObject);
			}
		}
	}
}
