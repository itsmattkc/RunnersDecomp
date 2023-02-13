using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerSetShortCutMenu : MonoBehaviour
{
	public delegate void ShortCutCallback(CharaType charaType);

	private List<MenuPlayerSetShortCutButton> m_buttons = new List<MenuPlayerSetShortCutButton>();

	private ShortCutCallback m_callback;

	private bool m_isEndSetup;

	private int m_prevActivePageIndex = -1;

	private float m_buttonSpace = 1f;

	private int m_displayIconCount = 1;

	public bool IsEndSetup
	{
		get
		{
			return m_isEndSetup;
		}
		private set
		{
		}
	}

	private void OnEnable()
	{
		if (!m_isEndSetup)
		{
			return;
		}
		foreach (MenuPlayerSetShortCutButton button in m_buttons)
		{
			if (!(button == null))
			{
				CharaType chara = button.Chara;
				if (SaveDataManager.Instance.CharaData.Status[(int)chara] != 0 || 1 == 0)
				{
					button.SetIconLock(false);
				}
			}
		}
	}

	public void Setup(ShortCutCallback callback)
	{
		m_callback = callback;
		StartCoroutine(OnSetupCoroutine());
	}

	private IEnumerator OnSetupCoroutine()
	{
		List<GameObject> buttonObjectList = null;
		int openedCharaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
		while (true)
		{
			buttonObjectList = GameObjectUtil.FindChildGameObjects(base.gameObject, "ui_player_set_pager_scroll(Clone)");
			if (buttonObjectList != null && buttonObjectList.Count >= openedCharaCount)
			{
				break;
			}
			yield return null;
		}
		int buttonIndex = 0;
		for (int charaIndex = 0; charaIndex < 29; charaIndex++)
		{
			if (buttonIndex >= buttonObjectList.Count)
			{
				break;
			}
			CharaType charaType = (CharaType)charaIndex;
			if (MenuPlayerSetUtil.IsOpenedCharacter(charaType))
			{
				GameObject buttonObject = buttonObjectList[buttonIndex];
				buttonIndex++;
				if (!(buttonObject == null))
				{
					MenuPlayerSetShortCutButton button = buttonObject.AddComponent<MenuPlayerSetShortCutButton>();
					bool isLocked = (SaveDataManager.Instance.CharaData.Status[charaIndex] == 0) ? true : false;
					button.Setup(charaType, isLocked);
					button.SetCallback(ButtonClickedCallback);
					m_buttons.Add(button);
				}
			}
		}
		UIRectItemStorage itemStorage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(base.gameObject, "slot");
		if (itemStorage != null)
		{
			m_buttonSpace = itemStorage.spacing_x;
			UIPanel panel = base.gameObject.GetComponent<UIPanel>();
			if (panel != null)
			{
				Vector4 clipRange = panel.clipRange;
				m_displayIconCount = (int)(clipRange.z / m_buttonSpace);
			}
		}
		GameObject scrollButtonRoot2 = null;
		GameObject parentObject = base.gameObject.transform.parent.gameObject;
		scrollButtonRoot2 = GameObjectUtil.FindChildGameObject(parentObject, "player_set_scroll_other");
		GameObject leftButtonObject = GameObjectUtil.FindChildGameObject(scrollButtonRoot2, "Btn_icon_arrow_lt");
		if (leftButtonObject != null)
		{
			UIButtonMessage buttonMessage2 = leftButtonObject.GetComponent<UIButtonMessage>();
			if (buttonMessage2 == null)
			{
				buttonMessage2 = leftButtonObject.AddComponent<UIButtonMessage>();
			}
			buttonMessage2.target = base.gameObject;
			buttonMessage2.functionName = "LeftButtonClickedCallback";
		}
		GameObject rightButtonObject = GameObjectUtil.FindChildGameObject(scrollButtonRoot2, "Btn_icon_arrow_rt");
		if (rightButtonObject != null)
		{
			UIButtonMessage buttonMessage = rightButtonObject.GetComponent<UIButtonMessage>();
			if (buttonMessage == null)
			{
				buttonMessage = rightButtonObject.AddComponent<UIButtonMessage>();
			}
			buttonMessage.target = base.gameObject;
			buttonMessage.functionName = "RightButtonClickedCallback";
		}
		m_isEndSetup = true;
	}

	public void SetActiveCharacter(CharaType charaType, bool isSetup)
	{
		StartCoroutine(OnSetActiveCharacter(charaType, isSetup));
	}

	public IEnumerator OnSetActiveCharacter(CharaType charaType, bool isSetup)
	{
		int pageIndex = MenuPlayerSetUtil.GetPageIndexFromCharaType(charaType);
		int maxPage = MenuPlayerSetUtil.GetOpenedCharaCount();
		yield return null;
		if (isSetup)
		{
			UIDraggablePanel draggablePanel2 = base.gameObject.GetComponent<UIDraggablePanel>();
			if (draggablePanel2 != null)
			{
				int moveDistance3 = -(int)m_buttonSpace * (m_displayIconCount / 2);
				draggablePanel2.MoveRelative(new Vector3(-moveDistance3, 0f, 0f));
			}
		}
		else
		{
			UIDraggablePanel draggablePanel = base.gameObject.GetComponent<UIDraggablePanel>();
			if (draggablePanel != null)
			{
				int moveDistance2 = -(int)m_buttonSpace * (m_displayIconCount / 2);
				moveDistance2 += (int)m_buttonSpace * pageIndex;
				draggablePanel.ResetPosition();
				draggablePanel.MoveRelative(new Vector3(-moveDistance2, 0f, 0f));
			}
		}
		GameObject scrollButtonRoot2 = null;
		GameObject parentObject = base.gameObject.transform.parent.gameObject;
		scrollButtonRoot2 = GameObjectUtil.FindChildGameObject(parentObject, "player_set_scroll_other");
		if (pageIndex == 0)
		{
			BoxCollider boxCollider2 = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(scrollButtonRoot2, "Btn_icon_arrow_lt");
			if (boxCollider2 != null)
			{
				boxCollider2.isTrigger = true;
			}
		}
		else if (pageIndex >= maxPage - 1)
		{
			BoxCollider boxCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(scrollButtonRoot2, "Btn_icon_arrow_rt");
			if (boxCollider != null)
			{
				boxCollider.isTrigger = true;
			}
		}
		else
		{
			BoxCollider boxColliderL = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(scrollButtonRoot2, "Btn_icon_arrow_lt");
			if (boxColliderL != null)
			{
				boxColliderL.isTrigger = false;
			}
			BoxCollider boxColliderR = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(scrollButtonRoot2, "Btn_icon_arrow_rt");
			if (boxColliderR != null)
			{
				boxColliderR.isTrigger = false;
			}
		}
		if (m_prevActivePageIndex >= 0)
		{
			m_buttons[m_prevActivePageIndex].SetButtonActive(false);
		}
		m_buttons[pageIndex].SetButtonActive(true);
		m_prevActivePageIndex = pageIndex;
	}

	public void UnclockCharacter(CharaType charaType)
	{
		int pageIndexFromCharaType = MenuPlayerSetUtil.GetPageIndexFromCharaType(charaType);
		MenuPlayerSetShortCutButton menuPlayerSetShortCutButton = m_buttons[pageIndexFromCharaType];
		if (!(menuPlayerSetShortCutButton == null))
		{
			menuPlayerSetShortCutButton.SetIconLock(false);
			menuPlayerSetShortCutButton.SetButtonActive(true);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void ButtonClickedCallback(CharaType charaType)
	{
		if (m_callback != null)
		{
			m_callback(charaType);
		}
	}

	private void LeftButtonClickedCallback()
	{
		int num = m_prevActivePageIndex - 1;
		if (num <= 0)
		{
			num = 0;
		}
		CharaType charaTypeFromPageIndex = MenuPlayerSetUtil.GetCharaTypeFromPageIndex(num);
		ButtonClickedCallback(charaTypeFromPageIndex);
	}

	private void RightButtonClickedCallback()
	{
		int num = m_prevActivePageIndex + 1;
		int openedCharaCount = MenuPlayerSetUtil.GetOpenedCharaCount();
		if (num >= openedCharaCount - 1)
		{
			num = openedCharaCount - 1;
		}
		CharaType charaTypeFromPageIndex = MenuPlayerSetUtil.GetCharaTypeFromPageIndex(num);
		ButtonClickedCallback(charaTypeFromPageIndex);
	}
}
