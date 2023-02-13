using UnityEngine;

public class MenuPlayerSetShortCutButton : MonoBehaviour
{
	public delegate void ButtonClickedCallback(CharaType charaType);

	private CharaType m_charaType;

	private ButtonClickedCallback m_callback;

	private bool m_isLocked;

	private GameObject m_spriteSdwObject;

	public CharaType Chara
	{
		get
		{
			return m_charaType;
		}
		private set
		{
		}
	}

	public void Setup(CharaType charaType, bool isLocked)
	{
		m_charaType = charaType;
		BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.isTrigger = false;
		}
		SetIconLock(isLocked);
		UIButtonMessage uIButtonMessage = base.gameObject.GetComponent<UIButtonMessage>();
		if (uIButtonMessage == null)
		{
			uIButtonMessage = base.gameObject.AddComponent<UIButtonMessage>();
		}
		uIButtonMessage.target = base.gameObject;
		uIButtonMessage.functionName = "ClickedCallback";
	}

	public void SetCallback(ButtonClickedCallback callback)
	{
		m_callback = callback;
	}

	public void SetIconLock(bool isLock)
	{
		m_isLocked = isLock;
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (m_isLocked)
		{
			empty = "ui_tex_player_set_unlock";
			empty2 = "ui_tex_player_set_act_unlock";
		}
		else
		{
			empty = "ui_tex_player_set_";
			empty += string.Format("{0:00}", (int)m_charaType);
			empty += "_";
			empty += CharaName.PrefixName[(int)m_charaType];
			empty2 = "ui_tex_player_set_act_";
			empty2 += string.Format("{0:00}", (int)m_charaType);
			empty2 += "_";
			empty2 += CharaName.PrefixName[(int)m_charaType];
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_pager_bg_temp");
		if (uISprite != null)
		{
			uISprite.spriteName = empty;
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_pager_act_temp");
		if (uISprite2 != null)
		{
			uISprite2.spriteName = empty2;
			m_spriteSdwObject = uISprite2.gameObject;
			m_spriteSdwObject.SetActive(false);
		}
	}

	public void SetButtonActive(bool isActive)
	{
		if (m_spriteSdwObject != null)
		{
			m_spriteSdwObject.SetActive(isActive);
		}
	}

	public bool IsVisible(UIPanel panel)
	{
		if (panel != null)
		{
			Transform transform = base.gameObject.transform;
			if (panel.IsVisible(transform.position))
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void ClickedCallback()
	{
		Debug.Log("MenuPlayerSetShortCutButton.ButtonClickedCallback");
		if (m_callback != null)
		{
			m_callback(m_charaType);
		}
	}
}
