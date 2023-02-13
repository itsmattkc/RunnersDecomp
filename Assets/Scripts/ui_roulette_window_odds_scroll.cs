using UnityEngine;

public class ui_roulette_window_odds_scroll : MonoBehaviour
{
	[SerializeField]
	private UILabel m_prizeName;

	[SerializeField]
	private UILabel m_oddsValue;

	public void UpdateView(string prizeNmae, string oddsValue)
	{
		m_prizeName.text = prizeNmae;
		m_oddsValue.text = oddsValue;
	}
}
