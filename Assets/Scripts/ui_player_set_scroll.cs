using System.Collections.Generic;
using Text;
using UnityEngine;

public class ui_player_set_scroll : MonoBehaviour
{
	private enum BTN_MODE
	{
		GET,
		ADD,
		MAX,
		LOCK_EPISODE,
		LOCK
	}

	private PlayerCharaList m_parent;

	private int m_currentDeck;

	private List<int> m_selectList;

	private BTN_MODE m_btnMode;

	private Dictionary<int, int> m_btnCost;

	private CharaType m_charaType = CharaType.UNKNOWN;

	private ServerCharacterState m_charaState;

	private static bool s_starTextDefaultInit;

	private static Color s_starTextDefault;

	public PlayerCharaList parent
	{
		get
		{
			return m_parent;
		}
	}

	public CharaType charaType
	{
		get
		{
			return m_charaType;
		}
	}

	public void Setup(PlayerCharaList parent, ServerCharacterState characterState)
	{
		m_parent = parent;
		m_charaType = characterState.charaType;
		m_charaState = characterState;
		m_currentDeck = DeckUtil.GetDeckCurrentStockIndex();
		if (m_charaType != CharaType.UNKNOWN && m_charaState != null)
		{
			SetParam();
			SetObject();
		}
	}

	public bool UpdateView()
	{
		m_currentDeck = DeckUtil.GetDeckCurrentStockIndex();
		if (m_charaType != CharaType.UNKNOWN)
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			m_charaState = playerState.CharacterState(m_charaType);
			if (m_charaType != CharaType.UNKNOWN && m_charaState != null)
			{
				SetParam();
				SetObject();
			}
			return true;
		}
		return false;
	}

	private int GetSelect()
	{
		int result = 0;
		if (m_selectList != null && m_selectList.Count > 0 && m_selectList.Count > m_currentDeck)
		{
			result = m_selectList[m_currentDeck];
		}
		return result;
	}

	private void SetParam()
	{
		m_btnMode = BTN_MODE.LOCK;
		if (m_btnCost != null)
		{
			m_btnCost.Clear();
		}
		else
		{
			m_btnCost = new Dictionary<int, int>();
		}
		if (m_charaState.IsUnlocked)
		{
			m_btnMode = BTN_MODE.ADD;
			if (m_charaState.star >= m_charaState.starMax && m_charaState.starMax > 0)
			{
				m_btnMode = BTN_MODE.MAX;
			}
		}
		else
		{
			m_btnMode = BTN_MODE.GET;
			if (m_charaState.Condition == ServerCharacterState.LockCondition.MILEAGE_EPISODE)
			{
				m_btnMode = BTN_MODE.LOCK_EPISODE;
			}
		}
		if (m_btnMode != BTN_MODE.ADD || m_btnMode != 0)
		{
			if (m_charaState.priceNumRings > 0)
			{
				m_btnCost.Add(910000, m_charaState.priceNumRings);
			}
			if (m_charaState.priceNumRedRings > 0)
			{
				m_btnCost.Add(900000, m_charaState.priceNumRedRings);
			}
			if (m_charaState.IsRoulette)
			{
				m_btnCost.Add(0, 0);
			}
		}
		if (m_selectList != null)
		{
			m_selectList.Clear();
		}
		else
		{
			m_selectList = new List<int>();
		}
		List<DeckUtil.DeckSet> deckList = DeckUtil.GetDeckList();
		if (deckList == null || deckList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < deckList.Count; i++)
		{
			int item = 0;
			if (deckList[i].charaMain == m_charaType)
			{
				item = 1;
			}
			else if (deckList[i].charaSub == m_charaType)
			{
				item = 2;
			}
			m_selectList.Add(item);
		}
	}

	private void SetObject()
	{
		UILabel lv = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		UILabel name = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_name");
		UILabel star = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_star");
		UITexture chara = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_player_tex");
		UISprite genus = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_genus");
		UISprite type = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_speacies");
		UISprite select = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_word_icon");
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_2_get");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "ability");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "pattern_lock");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_1_lvUP");
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "pattern_0");
		GameObject gameObject5 = GameObjectUtil.FindChildGameObject(base.gameObject, "pattern_1");
		if (gameObject != null && gameObject2 != null && gameObject3 != null && gameObject4 != null && gameObject5 != null)
		{
			if (m_charaState.IsUnlocked)
			{
				gameObject2.SetActive(false);
				gameObject3.SetActive(true);
				gameObject4.SetActive(false);
				gameObject5.SetActive(true);
				if (!s_starTextDefaultInit)
				{
					Color color = star.color;
					float r = color.r;
					Color color2 = star.color;
					float g = color2.g;
					Color color3 = star.color;
					float b = color3.b;
					Color color4 = star.color;
					s_starTextDefault = new Color(r, g, b, color4.a);
					s_starTextDefaultInit = true;
				}
				if (m_charaState.starMax > 0 && m_charaState.star >= m_charaState.starMax)
				{
					star.color = new Color(82f / 85f, 116f / 255f, 0f);
					if (uIImageButton != null)
					{
						uIImageButton.isEnabled = false;
					}
				}
				else
				{
					star.color = s_starTextDefault;
					if (uIImageButton != null)
					{
						uIImageButton.isEnabled = true;
					}
				}
			}
			else
			{
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = true;
				}
				gameObject2.SetActive(true);
				gameObject3.SetActive(false);
				gameObject4.SetActive(true);
				gameObject5.SetActive(false);
			}
			Dictionary<BonusParam.BonusType, float> teamBonusList = m_charaState.GetTeamBonusList();
			if (teamBonusList != null && teamBonusList.Count > 0)
			{
				gameObject.SetActive(true);
				int count = teamBonusList.Count;
				GameObject gameObject6 = null;
				GameObject gameObject7 = null;
				switch (count)
				{
				case 1:
					gameObject6 = GameObjectUtil.FindChildGameObject(gameObject, "1_item");
					break;
				case 2:
					gameObject6 = GameObjectUtil.FindChildGameObject(gameObject, "2_item");
					break;
				case 3:
					gameObject6 = GameObjectUtil.FindChildGameObject(gameObject, "4_item");
					if (gameObject6 != null)
					{
						gameObject7 = GameObjectUtil.FindChildGameObject(gameObject6, "cell_4");
						if (gameObject7 != null)
						{
							gameObject7.SetActive(false);
						}
					}
					break;
				case 4:
					gameObject6 = GameObjectUtil.FindChildGameObject(gameObject, "4_item");
					break;
				default:
					Debug.Log("ui_player_set_scroll SetObject error  abilityNum:" + count + " !!!!!!!");
					break;
				}
				if (gameObject6 != null)
				{
					for (int i = 1; i <= 5; i++)
					{
						if (i != count)
						{
							GameObject gameObject8 = GameObjectUtil.FindChildGameObject(gameObject, i + "_item");
							if (gameObject8 != null)
							{
								gameObject8.SetActive(false);
							}
						}
					}
					gameObject6.SetActive(true);
					int j = 1;
					Dictionary<BonusParam.BonusType, float>.KeyCollection keys = teamBonusList.Keys;
					using (Dictionary<BonusParam.BonusType, float>.KeyCollection.Enumerator enumerator = keys.GetEnumerator())
					{
						for (; enumerator.MoveNext(); j++)
						{
							BonusParam.BonusType current = enumerator.Current;
							gameObject7 = GameObjectUtil.FindChildGameObject(gameObject6, "cell_" + j);
							if (!(gameObject7 != null))
							{
								break;
							}
							gameObject7.SetActive(true);
							UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject7, "img_ability_icon_" + j);
							UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject7, "Lbl_ability_name_" + j);
							if (!(uISprite != null) || !(uILabel != null))
							{
								continue;
							}
							float num = teamBonusList[current];
							uISprite.spriteName = BonusUtil.GetAbilityIconSpriteName(current, num);
							string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoSet", "bonus_percent").text;
							if (string.IsNullOrEmpty(text))
							{
								continue;
							}
							switch (current)
							{
							case BonusParam.BonusType.SPEED:
								uILabel.text = text.Replace("{BONUS}", (100f - num).ToString());
								continue;
							case BonusParam.BonusType.TOTAL_SCORE:
								if (Mathf.Abs(num) <= 1f)
								{
									num *= 100f;
								}
								break;
							}
							if (num >= 0f)
							{
								uILabel.text = "+" + text.Replace("{BONUS}", num.ToString());
							}
							else
							{
								uILabel.text = text.Replace("{BONUS}", num.ToString());
							}
						}
					}
				}
				else
				{
					gameObject.SetActive(false);
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
		SetCharacter(m_charaType, ref name, ref lv, ref star, ref chara, ref type, ref genus, ref select);
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_0_info", base.gameObject, "OnClickChara");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_2_get", base.gameObject, "OnClickGet");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_1_lvUP", base.gameObject, "OnClickLvUp");
	}

	private void SetCharacter(CharaType charaType, ref UILabel name, ref UILabel lv, ref UILabel star, ref UITexture chara, ref UISprite type, ref UISprite genus, ref UISprite select)
	{
		bool flag = false;
		if (charaType != CharaType.NUM && charaType != CharaType.UNKNOWN && m_charaState != null && HudCharacterPanelUtil.CheckValidChara(charaType))
		{
			chara.gameObject.SetActive(true);
			if (m_charaState.IsUnlocked)
			{
				chara.color = new Color(1f, 1f, 1f);
				TextureRequestChara request = new TextureRequestChara(charaType, chara);
				TextureAsyncLoadManager.Instance.Request(request);
			}
			else
			{
				chara.color = new Color(0f, 0f, 0f);
				TextureRequestChara request2 = new TextureRequestChara(charaType, chara);
				TextureAsyncLoadManager.Instance.Request(request2);
			}
			HudCharacterPanelUtil.SetName(charaType, name.gameObject);
			HudCharacterPanelUtil.SetLevel(charaType, lv.gameObject);
			HudCharacterPanelUtil.SetCharaType(charaType, type.gameObject);
			HudCharacterPanelUtil.SetTeamType(charaType, genus.gameObject);
			if (select != null)
			{
				switch (GetSelect())
				{
				case 1:
					select.spriteName = "ui_player_set_main";
					break;
				case 2:
					select.spriteName = "ui_player_set_sub";
					break;
				default:
					select.spriteName = string.Empty;
					break;
				}
			}
			if (star != null)
			{
				star.text = m_charaState.star.ToString();
			}
			flag = true;
		}
		if (!flag)
		{
			if (lv != null)
			{
				lv.text = string.Empty;
			}
			if (name != null)
			{
				name.text = string.Empty;
			}
			if (star != null)
			{
				star.text = string.Empty;
			}
			if (chara != null)
			{
				chara.gameObject.SetActive(false);
			}
			if (type != null)
			{
				type.spriteName = string.Empty;
			}
			if (genus != null)
			{
				genus.spriteName = string.Empty;
			}
			if (select != null)
			{
				select.spriteName = string.Empty;
			}
		}
	}

	private void OnClickChara()
	{
		if (m_parent != null && m_parent.isTutorial)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHARASELECT_MAIN);
			m_parent.SetTutorialEnd();
		}
		if (m_charaState != null && m_charaState.IsUnlocked)
		{
			PlayerSetWindowUI.Create(m_charaType, this, PlayerSetWindowUI.WINDOW_MODE.SET);
		}
	}

	private void OnClickGet()
	{
		if (m_parent != null && m_parent.isTutorial)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHARASELECT_MAIN);
			m_parent.SetTutorialEnd();
		}
		PlayerSetWindowUI.Create(m_charaType, this, PlayerSetWindowUI.WINDOW_MODE.BUY);
	}

	private void OnClickLvUp()
	{
		BackKeyManager.InvalidFlag = true;
		if (m_parent != null && m_parent.isTutorial)
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "chara_level_up_explan";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("MainMenu", "chara_level_up_explan_caption");
			info.message = TextUtility.GetCommonText("MainMenu", "chara_level_up_explan");
			info.finishedCloseDelegate = GeneralWindowCharaLevelUpCloseCallback;
			GeneralWindow.Create(info);
		}
		PlayerLvupWindow.Open(this, m_charaType);
		Debug.Log("OnClickLvUp");
	}

	private void GeneralWindowCharaLevelUpCloseCallback()
	{
		TutorialCursor.StartTutorialCursor(TutorialCursor.Type.CHARASELECT_LEVEL_UP);
	}
}
