using AnimationOrTween;
using DataTable;
using Text;
using UnityEngine;

public class ChaoSetWindowUI : MonoBehaviour
{
	public struct ChaoInfo
	{
		public int id;

		public int level;

		public ChaoData.Rarity rarity;

		public CharacterAttribute charaAttribute;

		public string name;

		public string detail;

		public bool onMask;

		public ChaoInfo(ChaoData chaoData)
		{
			id = chaoData.id;
			level = chaoData.level;
			rarity = chaoData.rarity;
			charaAttribute = chaoData.charaAtribute;
			name = chaoData.name;
			detail = chaoData.GetDetailLevelPlusSP(level);
			onMask = false;
		}
	}

	public enum WindowType
	{
		WITH_BUTTON,
		WINDOW_ONLY
	}

	private static bool m_isActive;

	private ChaoInfo m_chaoInfo;

	[SerializeField]
	private UISprite m_chaoSprite;

	[SerializeField]
	private UITexture m_chaoTexture;

	[SerializeField]
	private UISprite m_chaoRankSprite;

	[SerializeField]
	private UILabel m_chaoNameLabel;

	[SerializeField]
	private UILabel m_chaoLevelLabel;

	[SerializeField]
	private UISprite m_chaoTypeSprite;

	[SerializeField]
	private UILabel m_chaoTypeLabel;

	[SerializeField]
	private UISprite m_bonusTypeSprite;

	[SerializeField]
	private UILabel m_bonusLabel;

	private UIDraggablePanel m_draggablePanel;

	private bool m_tutorial;

	public static bool isActive
	{
		get
		{
			return m_isActive;
		}
	}

	public static ChaoSetWindowUI GetWindow()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			return GameObjectUtil.FindChildGameObjectComponent<ChaoSetWindowUI>(gameObject, "ChaoSetWindowUI");
		}
		return null;
	}

	public void OpenWindow(ChaoInfo chaoInfo, WindowType windowType)
	{
		m_isActive = true;
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(true);
		}
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "chao_set_window");
		if (animation != null)
		{
			animation.gameObject.SetActive(true);
			ActiveAnimation.Play(animation, Direction.Forward);
		}
		SoundManager.SePlay("sys_window_open");
		OnSetChao(chaoInfo);
		string text = string.Empty;
		switch (windowType)
		{
		case WindowType.WITH_BUTTON:
			OnSetActiveButton(true);
			text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "ui_Lbl_caption").text;
			break;
		case WindowType.WINDOW_ONLY:
			OnSetActiveButton(false);
			text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "ui_Lbl_caption_detail").text;
			break;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption");
		if (uILabel != null)
		{
			uILabel.text = text;
		}
		m_tutorial = ChaoSetUI.IsChaoTutorial();
		if (m_tutorial)
		{
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.CHAOSELECT_MAIN);
		}
	}

	private void Start()
	{
		SetAnimationCallBack("Btn_set_main");
		SetAnimationCallBack("Btn_set_sub");
		SetAnimationCallBack("Btn_window_close");
	}

	private void SetAnimationCallBack(string objName)
	{
		UIPlayAnimation uIPlayAnimation = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, objName);
		if (uIPlayAnimation != null)
		{
			EventDelegate.Add(uIPlayAnimation.onFinished, OnFinishedAnimationCallback, false);
		}
	}

	private void OnDestroy()
	{
		m_isActive = false;
		if (m_chaoTexture != null && m_chaoTexture.mainTexture != null)
		{
			m_chaoTexture.mainTexture = null;
		}
	}

	private void OnSetChao(ChaoInfo chaoInfo)
	{
		m_chaoInfo = chaoInfo;
		ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(m_chaoTexture, null, true);
		ChaoTextureManager.Instance.GetTexture(m_chaoInfo.id, info);
		m_chaoTexture.enabled = true;
		if (m_chaoRankSprite != null)
		{
			m_chaoRankSprite.spriteName = "ui_chao_set_bg_ll_" + (int)m_chaoInfo.rarity;
		}
		if (m_chaoNameLabel != null)
		{
			m_chaoNameLabel.text = m_chaoInfo.name;
		}
		if (m_chaoLevelLabel != null)
		{
			m_chaoLevelLabel.text = TextUtility.GetTextLevel(m_chaoInfo.level.ToString());
		}
		string text = m_chaoInfo.charaAttribute.ToString().ToLower();
		if (m_chaoTypeSprite != null)
		{
			m_chaoTypeSprite.spriteName = "ui_chao_set_type_icon_" + text;
		}
		if (m_chaoTypeLabel != null)
		{
			m_chaoTypeLabel.text = TextUtility.GetCommonText("CharaAtribute", text);
		}
		if (m_chaoTexture != null)
		{
			if (m_chaoInfo.onMask)
			{
				m_chaoTexture.color = Color.black;
			}
			else
			{
				m_chaoTexture.color = Color.white;
			}
		}
		if (m_bonusLabel != null)
		{
			m_bonusLabel.text = m_chaoInfo.detail;
		}
		if (m_draggablePanel == null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "ScrollView");
			if (gameObject != null)
			{
				m_draggablePanel = gameObject.GetComponent<UIDraggablePanel>();
			}
		}
		if (m_draggablePanel != null)
		{
			m_draggablePanel.ResetPosition();
		}
	}

	private void OnSetActiveButton(bool isActive)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_set_main");
		if (gameObject != null)
		{
			gameObject.SetActive(isActive);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_set_sub");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(isActive);
		}
	}

	private void OnClickMain()
	{
		SoundManager.SePlay("sys_menu_decide");
		ChaoSetUI chaoSetUI = GameObjectUtil.FindGameObjectComponent<ChaoSetUI>("ChaoSetUI");
		if (chaoSetUI != null)
		{
			chaoSetUI.RegistChao(0, m_chaoInfo.id);
		}
		if (m_tutorial)
		{
			CreateTutorialWindow();
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHAOSELECT_MAIN);
		}
	}

	private void OnClickSub()
	{
		SoundManager.SePlay("sys_menu_decide");
		ChaoSetUI chaoSetUI = GameObjectUtil.FindGameObjectComponent<ChaoSetUI>("ChaoSetUI");
		if (chaoSetUI != null)
		{
			chaoSetUI.RegistChao(1, m_chaoInfo.id);
		}
	}

	private void OnClickClose()
	{
		SoundManager.SePlay("sys_window_close");
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		if (m_chaoInfo.id == data.chao_id && m_chaoTexture != null)
		{
			m_chaoTexture.mainTexture = data.tex;
			m_chaoTexture.enabled = true;
		}
	}

	private void OnFinishedAnimationCallback()
	{
		if (m_chaoTexture != null && m_chaoTexture.mainTexture != null)
		{
			m_chaoTexture.mainTexture = null;
			m_chaoTexture.enabled = false;
		}
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
		m_isActive = false;
		m_chaoInfo = default(ChaoInfo);
	}

	private void CreateTutorialWindow()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "ChaoTutorial";
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "tutorial_ready_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "tutorial_ready_message").text;
		info.buttonType = GeneralWindow.ButtonType.Ok;
		GeneralWindow.Create(info);
	}
}
