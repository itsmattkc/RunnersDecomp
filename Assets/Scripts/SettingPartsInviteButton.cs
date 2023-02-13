using Message;
using UnityEngine;

public class SettingPartsInviteButton : MonoBehaviour
{
	public delegate void ButtonPressedCallback(SocialUserData friendData);

	private SocialUserData m_friendData;

	private ButtonPressedCallback m_callback;

	public void Setup(SocialUserData friendData, ButtonPressedCallback callback)
	{
		if (friendData == null)
		{
			return;
		}
		m_friendData = friendData;
		m_callback = callback;
		UITexture texture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_icon_friends");
		if (texture != null)
		{
			PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
			texture.mainTexture = playerImageManager.GetPlayerImage(m_friendData.Id, string.Empty, delegate(Texture2D _faceTexture)
			{
				texture.mainTexture = _faceTexture;
			});
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_friend_name");
		if (uILabel != null)
		{
			uILabel.text = m_friendData.Name;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_invite");
		if (gameObject != null)
		{
			UIButtonMessage uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickButton";
		}
	}

	private void OnClickButton()
	{
		if (m_callback != null)
		{
			m_callback(m_friendData);
		}
	}

	private void InviteFriendEndCallback(MsgSocialNormalResponse msg)
	{
	}
}
