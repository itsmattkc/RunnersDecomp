using UnityEngine;

public class RaidBossBoostGagueColor : MonoBehaviour
{
	[SerializeField]
	private float m_level1R;

	[SerializeField]
	private float m_level1G;

	[SerializeField]
	private float m_level1B;

	[SerializeField]
	private float m_level2R;

	[SerializeField]
	private float m_level2G;

	[SerializeField]
	private float m_level2B;

	[SerializeField]
	private float m_level3R;

	[SerializeField]
	private float m_level3G;

	[SerializeField]
	private float m_level3B;

	public Color Level1
	{
		get
		{
			float r = m_level1R / 255f;
			float g = m_level1G / 255f;
			float b = m_level1B / 255f;
			return new Color(r, g, b);
		}
	}

	public Color Level2
	{
		get
		{
			float r = m_level2R / 255f;
			float g = m_level2G / 255f;
			float b = m_level2B / 255f;
			return new Color(r, g, b);
		}
	}

	public Color Level3
	{
		get
		{
			float r = m_level3R / 255f;
			float g = m_level3G / 255f;
			float b = m_level3B / 255f;
			return new Color(r, g, b);
		}
	}
}
