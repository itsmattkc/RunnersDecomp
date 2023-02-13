using UnityEngine;

public class FriendSignCreateData
{
	public bool m_create;

	public Texture2D m_texture;

	public FriendSignCreateData(Texture2D texture)
	{
		m_create = false;
		m_texture = texture;
	}
}
