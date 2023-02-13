using UnityEngine;

public class FriendSignData
{
	public int m_index;

	public float m_distance;

	public Texture2D m_texture;

	public bool m_appear;

	public FriendSignData(int index, float distance, Texture2D texture, bool appear)
	{
		m_index = index;
		m_distance = distance;
		m_texture = texture;
		m_appear = appear;
	}
}
