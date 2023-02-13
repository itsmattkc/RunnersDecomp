using SaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlayerSetCharaPage : MonoBehaviour
{
	private static readonly string[] HideObjectName = new string[4]
	{
		"Btn_lv_up",
		"Btn_player_main",
		"_guide",
		"slot"
	};

	private GameObject m_pageRoot;

	private CharaType m_charaType;

	private MenuPlayerSetUnlockedChara m_unlocked;

	private MenuPlayerSetCharaButton m_charaButton;

	private MenuPlayerSetLevelUpButton m_levelUpButton;

	private MenuPlayerSetAbilityButton[] m_abilityButton = new MenuPlayerSetAbilityButton[10];

	private GameObject m_blinderObject;

	private bool m_isEndSetup;

	public bool IsEndSetUp
	{
		get
		{
			return m_isEndSetup;
		}
		private set
		{
		}
	}

	public void Setup(GameObject pageRoot, CharaType charaType)
	{
		base.gameObject.SetActive(true);
		m_isEndSetup = false;
		m_pageRoot = pageRoot;
		m_charaType = charaType;
		if (m_pageRoot == null)
		{
			return;
		}
		string[] hideObjectName = HideObjectName;
		foreach (string text in hideObjectName)
		{
			if (text != null)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, text);
				if (!(gameObject == null))
				{
					gameObject.SetActive(true);
				}
			}
		}
		StartCoroutine(SetupCoroutine());
	}

	private IEnumerator SetupCoroutine()
	{
		m_charaButton = base.gameObject.AddComponent<MenuPlayerSetCharaButton>();
		if (m_charaButton != null)
		{
			m_charaButton.Setup(m_charaType, m_pageRoot);
		}
		m_levelUpButton = base.gameObject.AddComponent<MenuPlayerSetLevelUpButton>();
		if (m_levelUpButton != null)
		{
			m_levelUpButton.Setup(m_charaType, m_pageRoot);
			m_levelUpButton.SetCallback(LevelUppedCallback);
		}
		GameObject buttonRoot = GameObjectUtil.FindChildGameObject(m_pageRoot, "slot");
		if (buttonRoot != null)
		{
			List<GameObject> buttonList2 = null;
			while (true)
			{
				buttonList2 = GameObjectUtil.FindChildGameObjects(buttonRoot, "ui_player_set_item_2_cell(Clone)");
				if (buttonList2 != null && buttonList2.Count >= 10)
				{
					break;
				}
				yield return null;
			}
			if (buttonList2 != null)
			{
				for (int index = 0; index < buttonList2.Count; index++)
				{
					GameObject buttonObjectRoot = buttonList2[index];
					if (!(buttonObjectRoot == null))
					{
						MenuPlayerSetAbilityButton button = buttonObjectRoot.AddComponent<MenuPlayerSetAbilityButton>();
						if (!(button == null))
						{
							AbilityType abilityType = MenuPlayerSetUtil.AbilityLevelUpOrder[index];
							button.Setup(m_charaType, abilityType);
							m_abilityButton[(int)abilityType] = button;
						}
					}
				}
			}
		}
		AbilityType nextLevelUpAbility = MenuPlayerSetUtil.GetNextLevelUpAbility(m_charaType);
		MenuPlayerSetAbilityButton nextLevelUpButton = m_abilityButton[(int)nextLevelUpAbility];
		if (nextLevelUpButton != null)
		{
			nextLevelUpButton.SetActive(true);
		}
		if (ServerInterface.LoggedInServerInterface != null)
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (playerState != null)
			{
				ServerCharacterState charaState = playerState.CharacterState(m_charaType);
				if (charaState != null && !charaState.IsUnlocked)
				{
					m_unlocked = base.gameObject.AddComponent<MenuPlayerSetUnlockedChara>();
					m_unlocked.Setup(m_charaType, m_pageRoot);
				}
			}
		}
		else
		{
			SaveDataManager save_data_manager = SaveDataManager.Instance;
			if (save_data_manager != null)
			{
				CharaData charaData = save_data_manager.CharaData;
				if (charaData.Status[(int)m_charaType] == 0)
				{
					m_unlocked = base.gameObject.AddComponent<MenuPlayerSetUnlockedChara>();
					m_unlocked.Setup(m_charaType, m_pageRoot);
				}
			}
		}
		GameObject playerSetRoot = MenuPlayerSetUtil.GetPlayerSetRoot();
		if (playerSetRoot != null)
		{
			m_blinderObject = GameObjectUtil.FindChildGameObject(playerSetRoot, "blinder");
			if (m_blinderObject != null)
			{
				UIButtonMessage buttonMessage = m_blinderObject.AddComponent<UIButtonMessage>();
				buttonMessage.target = base.gameObject;
				buttonMessage.functionName = "OnClickedSkipButton";
			}
		}
		m_isEndSetup = true;
	}

	public void OnSelected()
	{
		if (m_charaButton != null)
		{
			m_charaButton.OnSelected();
		}
	}

	public void UpdateRibbon()
	{
		if (m_charaButton != null)
		{
			m_charaButton.UpdateRibbon();
		}
	}

	private void LevelUppedCallback(AbilityType levelUppedAbility)
	{
		if (m_charaButton != null)
		{
			m_charaButton.LevelUp(LevelUpAnimationEndCallback);
		}
		if (m_blinderObject != null)
		{
			m_blinderObject.SetActive(true);
		}
		MenuPlayerSetAbilityButton menuPlayerSetAbilityButton = m_abilityButton[(int)levelUppedAbility];
		if (menuPlayerSetAbilityButton != null)
		{
			menuPlayerSetAbilityButton.LevelUp(LevelUpAnimationEndCallback);
		}
		AbilityType nextLevelUpAbility = MenuPlayerSetUtil.GetNextLevelUpAbility(m_charaType);
		MenuPlayerSetAbilityButton menuPlayerSetAbilityButton2 = m_abilityButton[(int)nextLevelUpAbility];
		if (menuPlayerSetAbilityButton2 != null)
		{
			menuPlayerSetAbilityButton2.SetActive(true);
		}
		if (m_levelUpButton != null)
		{
			m_levelUpButton.InitCostLabel();
		}
	}

	private void OnClickedSkipButton()
	{
		MenuPlayerSetAbilityButton[] abilityButton = m_abilityButton;
		foreach (MenuPlayerSetAbilityButton menuPlayerSetAbilityButton in abilityButton)
		{
			if (!(menuPlayerSetAbilityButton == null))
			{
				menuPlayerSetAbilityButton.SkipLevelUp();
			}
		}
		if (m_charaButton != null)
		{
			m_charaButton.SkipLevelUp();
		}
	}

	private void LevelUpAnimationEndCallback()
	{
		if (m_charaButton != null && !m_charaButton.AnimEnd)
		{
			return;
		}
		if (m_abilityButton != null)
		{
			MenuPlayerSetAbilityButton[] abilityButton = m_abilityButton;
			foreach (MenuPlayerSetAbilityButton menuPlayerSetAbilityButton in abilityButton)
			{
				if (!(menuPlayerSetAbilityButton == null) && !menuPlayerSetAbilityButton.AnimEnd)
				{
					return;
				}
			}
		}
		if (m_blinderObject != null)
		{
			m_blinderObject.SetActive(false);
		}
		if (m_levelUpButton != null)
		{
			m_levelUpButton.OnLevelUpEnd();
		}
	}
}
