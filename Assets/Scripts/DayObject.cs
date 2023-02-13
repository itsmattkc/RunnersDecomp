using AnimationOrTween;
using UnityEngine;

public class DayObject
{
	public GameObject m_clearGameObject;

	private GameObject m_effect;

	private Animation m_clearAnimation;

	private UISprite m_imgCheck;

	private UISprite m_imgItem;

	private UISprite m_imgChara;

	private UITexture m_imgChao;

	private UISprite m_imgHidden;

	private UILabel m_lblCount;

	private int m_count;

	public int count
	{
		get
		{
			return m_count;
		}
		set
		{
			if (m_count != value)
			{
				m_count = value;
				if (m_lblCount != null)
				{
					m_lblCount.text = HudUtility.GetFormatNumString(m_count);
				}
			}
		}
	}

	public DayObject(GameObject obj, Color color, int day)
	{
		m_clearGameObject = obj;
		m_effect = GameObjectUtil.FindChildGameObject(m_clearGameObject, "eff_4");
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_clearGameObject, "img_day_num");
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_clearGameObject, "img_frame_color");
		m_imgItem = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_clearGameObject, "img_daily_item");
		m_imgChara = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_clearGameObject, "img_chara");
		m_imgChao = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_clearGameObject, "img_chao");
		m_lblCount = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_clearGameObject, "Lbl_count");
		m_imgHidden = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_clearGameObject, "img_hidden");
		m_imgCheck = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_clearGameObject, "img_check");
		if (m_effect != null)
		{
			m_effect.SetActive(false);
		}
		if (uISprite2 != null)
		{
			uISprite2.color = color;
		}
		if (uISprite != null)
		{
			uISprite.spriteName = "ui_daily_num_" + day;
		}
		if (m_lblCount != null)
		{
			m_lblCount.text = "0";
		}
		if (m_imgItem != null)
		{
			m_imgItem.spriteName = string.Empty;
		}
		if (m_imgChara != null)
		{
			m_imgChara.spriteName = string.Empty;
			m_imgChara.alpha = 0f;
		}
		if (m_imgChao != null)
		{
			m_imgChao.mainTexture = null;
			m_imgChao.alpha = 0f;
		}
		if (m_imgHidden != null)
		{
			m_imgHidden.enabled = false;
		}
		if (m_imgCheck != null)
		{
			m_imgCheck.enabled = false;
		}
		m_clearAnimation = obj.GetComponentInChildren<Animation>();
	}

	public void SetAlready(bool already)
	{
		if (m_imgHidden != null)
		{
			m_imgHidden.enabled = already;
		}
		if (m_imgCheck != null)
		{
			m_imgCheck.enabled = already;
		}
	}

	public void PlayGetAnimation()
	{
		if (m_imgCheck != null)
		{
			m_imgCheck.enabled = true;
		}
		if (m_clearAnimation != null)
		{
			if (m_effect != null)
			{
				m_effect.SetActive(true);
			}
			ActiveAnimation.Play(m_clearAnimation, Direction.Forward);
		}
	}

	public bool SetItem(int id)
	{
		if (m_imgItem != null && m_imgChara != null && m_imgChao != null)
		{
			if (id >= 0)
			{
				switch (Mathf.FloorToInt((float)id / 100000f))
				{
				case 3:
					m_imgItem.alpha = 0f;
					m_imgChara.alpha = 1f;
					m_imgChao.alpha = 0f;
					m_imgChara.spriteName = "ui_tex_player_" + CharaTypeUtil.GetCharaSpriteNameSuffix(new ServerItem((ServerItem.Id)id).charaType);
					break;
				case 4:
				{
					m_imgItem.alpha = 0f;
					m_imgChara.alpha = 0f;
					m_imgChao.alpha = 1f;
					ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(m_imgChao, null, true);
					ChaoTextureManager.Instance.GetTexture(id - 400000, info);
					break;
				}
				default:
					m_imgItem.alpha = 1f;
					m_imgChara.alpha = 0f;
					m_imgChao.alpha = 0f;
					m_imgItem.spriteName = "ui_cmn_icon_item_" + id;
					break;
				}
			}
			else
			{
				m_imgItem.spriteName = "ui_cmn_icon_rsring_L";
			}
			return true;
		}
		return false;
	}
}
