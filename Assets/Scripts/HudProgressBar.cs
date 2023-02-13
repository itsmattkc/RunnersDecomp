using UnityEngine;

public class HudProgressBar : MonoBehaviour
{
	[SerializeField]
	private UISlider m_slider;

	[SerializeField]
	private UILabel m_parcentLabel;

	private float m_stateNum;

	private float m_state = -1f;

	public void SetUp(int stateNum)
	{
		m_stateNum = stateNum;
		m_state = -1f;
	}

	public void SetState(int state)
	{
		m_state = state;
		if (m_state >= 0f)
		{
			base.gameObject.SetActive(true);
			if (m_slider != null)
			{
				m_slider.value = (m_state + 1f) / m_stateNum;
				if (m_parcentLabel != null)
				{
					int num = (int)(m_slider.value * 100f);
					m_parcentLabel.text = num + "%";
				}
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}
}
