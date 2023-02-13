using UnityEngine;

public class QuotaInfo
{
	private enum State
	{
		IDLE,
		IN,
		BONUS,
		WAIT,
		END
	}

	private string m_caption;

	private string m_quotaString;

	private int m_serverRewardId;

	private string m_reward;

	private bool m_isCleared;

	private GameObject m_quotaPlate;

	private Animation m_animation;

	private string m_animClipName;

	private bool m_isPlayEnd;

	private static readonly float WAIT_TIME = 0.5f;

	private float m_timer;

	private State m_state;

	public GameObject QuotaPlate
	{
		get
		{
			return m_quotaPlate;
		}
	}

	public string AnimClipName
	{
		get
		{
			return m_animClipName;
		}
	}

	public QuotaInfo(string caption, string quotaString, int serverRewardId, string reward, bool isCleared)
	{
		m_caption = caption;
		m_quotaString = quotaString;
		m_serverRewardId = serverRewardId;
		m_reward = reward;
		m_isCleared = isCleared;
	}

	public void Setup(GameObject quotaPlate, Animation animation, string animClipName)
	{
		m_quotaPlate = quotaPlate;
		m_animation = animation;
		m_animClipName = animClipName;
	}

	public void SetupDisplay()
	{
		ServerItem serverItem = new ServerItem((ServerItem.Id)m_serverRewardId);
		bool flag = false;
		if (serverItem.idType == ServerItem.IdType.CHAO)
		{
			flag = true;
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_quotaPlate, "img_item_0");
		if (uISprite != null)
		{
			uISprite.gameObject.SetActive(!flag);
			uISprite.spriteName = PresentBoxUtility.GetItemSpriteName(serverItem);
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_quotaPlate, "texture_chao_0");
		if (uISprite2 != null)
		{
			uISprite2.gameObject.SetActive(false);
		}
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_quotaPlate, "texture_chao_1");
		if (uITexture != null)
		{
			uITexture.gameObject.SetActive(flag);
			if (flag && ChaoTextureManager.Instance != null)
			{
				uITexture.alpha = 1f;
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
				ChaoTextureManager.Instance.GetTexture(serverItem.chaoId, info);
			}
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_quotaPlate, "Lbl_event_object_total_num");
		if (uILabel != null)
		{
			uILabel.text = m_quotaString;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_quotaPlate, "Lbl_itemname");
		if (uILabel2 != null)
		{
			uILabel2.text = m_reward;
			UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_quotaPlate, "Lbl_itemname_sh");
			if (uILabel3 != null)
			{
				uILabel3.text = m_reward;
			}
		}
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_quotaPlate, "Lbl_word_event_object_total");
		if (uILabel4 != null)
		{
			uILabel4.text = m_caption;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_quotaPlate, "get_icon_Anim");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}

	public void PlayStart()
	{
		m_state = State.IN;
		m_isPlayEnd = false;
		m_timer = 0f;
		SetupDisplay();
		if (!string.IsNullOrEmpty(m_animClipName) && !(m_animation == null))
		{
			ActiveAnimation component = m_animation.gameObject.GetComponent<ActiveAnimation>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			m_animation.enabled = true;
			m_animation.Rewind();
			m_animation.Play(m_animClipName);
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_quotaPlate, "get_icon_Anim");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	public bool IsPlayEnd()
	{
		if (m_isPlayEnd)
		{
			return true;
		}
		return false;
	}

	public virtual void Update()
	{
		switch (m_state)
		{
		case State.IDLE:
			break;
		case State.IN:
			if (!(m_animation != null) || m_animation.IsPlaying(m_animClipName))
			{
				break;
			}
			if (m_isCleared)
			{
				Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_quotaPlate, "get_icon_Anim");
				if (animation != null)
				{
					animation.gameObject.SetActive(true);
					animation.Rewind();
					animation.Play("ui_Event_mission_getin_Anim");
					SoundManager.SePlay("sys_result_decide");
				}
				m_state = State.BONUS;
			}
			else
			{
				m_state = State.WAIT;
			}
			break;
		case State.BONUS:
		{
			Animation animation2 = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_quotaPlate, "get_icon_Anim");
			if (animation2 != null && !animation2.IsPlaying("ui_Event_mission_getin_Anim"))
			{
				m_state = State.WAIT;
			}
			break;
		}
		case State.WAIT:
			m_timer += Time.deltaTime;
			if (m_timer >= WAIT_TIME)
			{
				m_state = State.END;
			}
			break;
		case State.END:
			m_isPlayEnd = true;
			m_state = State.IDLE;
			break;
		}
	}
}
