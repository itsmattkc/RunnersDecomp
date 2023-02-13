using UnityEngine;

public class ui_presentbox_scroll : MonoBehaviour
{
	[SerializeField]
	private UIToggle m_toggle;

	[SerializeField]
	private GameObject m_imgChara;

	[SerializeField]
	private GameObject m_imgChao;

	[SerializeField]
	private GameObject m_imgItem;

	[SerializeField]
	private GameObject m_imgTex;

	[SerializeField]
	private UILabel m_infoLabel;

	[SerializeField]
	private UILabel m_itemNameLabel;

	[SerializeField]
	private UILabel m_receivedTimeLabel;

	private PresentBoxUI.PresentInfo m_persentInfo;

	private SocialInterface m_socialInterface;

	private string m_friendFBId = string.Empty;

	private bool m_check_flag;

	private bool m_se_skip_flag;

	private void Start()
	{
		base.enabled = false;
		if (m_toggle != null)
		{
			EventDelegate.Add(m_toggle.onChange, OnChangeToggle);
		}
	}

	private void OnDestroy()
	{
		ResetTextureData();
	}

	public void UpdateView(PresentBoxUI.PresentInfo info)
	{
		m_persentInfo = info;
		m_friendFBId = string.Empty;
		if (m_persentInfo == null)
		{
			return;
		}
		if (CheckTextureDisplay())
		{
			SetUITexture();
		}
		else
		{
			SetUISprite();
		}
		if (m_itemNameLabel != null)
		{
			string itemName = PresentBoxUtility.GetItemName(m_persentInfo.serverItem);
			m_itemNameLabel.text = itemName + " × " + m_persentInfo.itemNum;
		}
		if (m_infoLabel != null)
		{
			if (m_persentInfo.operatorFlag)
			{
				m_infoLabel.text = m_persentInfo.infoText;
			}
			else
			{
				m_infoLabel.text = PresentBoxUtility.GetItemInfo(m_persentInfo);
			}
		}
		if (m_receivedTimeLabel != null)
		{
			m_receivedTimeLabel.text = PresentBoxUtility.GetReceivedTime(m_persentInfo.expireTime);
		}
		SetCheckFlag(m_persentInfo.checkFlag);
	}

	public void ResetTextureData()
	{
		if (m_imgTex != null)
		{
			UITexture component = m_imgTex.GetComponent<UITexture>();
			if (component != null && component.mainTexture != null)
			{
				component.mainTexture = null;
			}
		}
	}

	public bool IsCheck()
	{
		return m_check_flag;
	}

	private void SetCheckFlag(bool check_flag)
	{
		if (m_toggle != null)
		{
			m_toggle.value = check_flag;
		}
		if (m_persentInfo != null)
		{
			m_persentInfo.checkFlag = check_flag;
		}
		m_check_flag = check_flag;
		m_se_skip_flag = true;
	}

	private void SetUITexture()
	{
		if (m_persentInfo == null)
		{
			return;
		}
		SetUISprite(m_imgChara, false, string.Empty);
		SetUISprite(m_imgItem, false, string.Empty);
		if (m_imgChao != null)
		{
			m_imgChao.SetActive(false);
		}
		if (!(m_imgTex != null))
		{
			return;
		}
		m_imgTex.SetActive(true);
		UITexture uiTex = m_imgTex.GetComponent<UITexture>();
		if (!(uiTex != null))
		{
			return;
		}
		uiTex.enabled = true;
		PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
		if (playerImageManager != null)
		{
			uiTex.mainTexture = playerImageManager.GetPlayerImage(m_friendFBId, string.Empty, delegate(Texture2D _faceTexture)
			{
				uiTex.mainTexture = _faceTexture;
			});
		}
	}

	private void SetUISprite()
	{
		if (m_imgTex != null)
		{
			m_imgTex.SetActive(false);
		}
		if (m_persentInfo == null)
		{
			return;
		}
		if (m_persentInfo.serverItem.idType == ServerItem.IdType.CHARA)
		{
			CharaType charaType = m_persentInfo.serverItem.charaType;
			int num = (int)charaType;
			string spriteName = "ui_tex_player_set_" + num.ToString("00") + "_" + CharaName.PrefixName[(int)charaType];
			SetUISprite(m_imgChara, true, spriteName);
			SetUISprite(m_imgItem, false, string.Empty);
			if (m_imgChao != null)
			{
				m_imgChao.SetActive(false);
			}
		}
		else if (m_persentInfo.serverItem.idType == ServerItem.IdType.CHAO)
		{
			SetUISprite(m_imgChara, false, string.Empty);
			SetUISprite(m_imgChao, true, "ui_tex_chao_" + m_persentInfo.serverItem.chaoId.ToString("D4"));
			SetUISprite(m_imgItem, false, string.Empty);
			if (m_imgChao != null)
			{
				m_imgChao.SetActive(true);
				UITexture component = m_imgChao.GetComponent<UITexture>();
				int chaoId = m_persentInfo.serverItem.chaoId;
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(component, null, true);
				ChaoTextureManager.Instance.GetTexture(chaoId, info);
			}
		}
		else
		{
			SetUISprite(m_imgChara, false, string.Empty);
			SetUISprite(m_imgItem, true, PresentBoxUtility.GetItemSpriteName(m_persentInfo.serverItem));
			if (m_imgChao != null)
			{
				m_imgChao.SetActive(false);
			}
		}
	}

	private void SetUISprite(GameObject obj, bool on, string spriteName = "")
	{
		if (!(obj != null))
		{
			return;
		}
		obj.SetActive(on);
		if (on)
		{
			UISprite component = obj.GetComponent<UISprite>();
			if (component != null)
			{
				component.spriteName = spriteName;
			}
		}
	}

	private void SetSocialInterface()
	{
		if (m_socialInterface == null)
		{
			m_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		}
	}

	private bool CheckTextureDisplay()
	{
		if (m_persentInfo != null && m_persentInfo.messageType == ServerMessageEntry.MessageType.SendEnergy)
		{
			SetSocialInterface();
			if (m_socialInterface != null)
			{
				SocialUserData socialUserDataFromGameId = SocialInterface.GetSocialUserDataFromGameId(m_socialInterface.FriendList, m_persentInfo.fromId);
				if (socialUserDataFromGameId != null)
				{
					m_friendFBId = socialUserDataFromGameId.Id;
					return !socialUserDataFromGameId.IsSilhouette;
				}
			}
		}
		return false;
	}

	private void OnChangeToggle()
	{
		if (m_toggle != null)
		{
			m_check_flag = m_toggle.value;
			if (m_persentInfo != null)
			{
				m_persentInfo.checkFlag = m_check_flag;
			}
		}
		if (!m_se_skip_flag)
		{
			if (m_check_flag)
			{
				SoundManager.SePlay("sys_menu_decide");
			}
			else
			{
				SoundManager.SePlay("sys_window_close");
			}
		}
		m_se_skip_flag = false;
	}
}
