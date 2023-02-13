using UnityEngine;

public class RankingSimpleScroll : MonoBehaviour
{
	private UILabel m_nameLable;

	private string m_id;

	public UIToggle m_toggle;

	private UITexture m_texture;

	private SocialUserData m_userData;

	private int m_defaultImageHash;

	private void Start()
	{
		PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
		if (playerImageManager != null)
		{
			m_defaultImageHash = playerImageManager.GetDefaultImage().GetHashCode();
		}
		m_nameLable = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_username");
		m_toggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject, "Btn_ranking_top");
		m_texture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_tex_icon_friends");
	}

	public void SetUserData(SocialUserData user)
	{
		m_userData = user;
		m_id = user.Id;
		m_nameLable.text = user.Name;
	}

	public void LoadImage()
	{
		PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
		if (!(playerImageManager != null))
		{
			return;
		}
		Texture2D playerImage = playerImageManager.GetPlayerImage(m_userData.Id, m_userData.Url, delegate(Texture2D _faceTexture)
		{
			if (_faceTexture.GetHashCode() != m_defaultImageHash && m_texture.mainTexture.GetHashCode() != _faceTexture.GetHashCode())
			{
				m_texture.mainTexture = _faceTexture;
			}
		});
		if (m_texture.mainTexture.GetHashCode() != playerImage.GetHashCode())
		{
			m_texture.mainTexture = playerImage;
		}
	}

	public void OnClickRankingScroll()
	{
		RankingFriendOptionWindow rankingFriendOptionWindow = GameObjectUtil.FindGameObjectComponent<RankingFriendOptionWindow>("RankingFriendOptionWindow");
		if (rankingFriendOptionWindow != null)
		{
			rankingFriendOptionWindow.ChooseFriend(m_userData, m_toggle);
		}
	}
}
