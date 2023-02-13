using AnimationOrTween;
using App.Utility;
using Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Text;
using UnityEngine;

public class HudCockpit : MonoBehaviour
{
	private enum Anchor2TC
	{
		DEFAULT,
		BOSS,
		EVENTBOSS,
		SPSTAGE,
		NUM
	}

	private enum ScoreRank
	{
		RANK_01,
		RANK_02,
		RANK_03,
		RANK_04,
		RANK_05,
		RANK_06,
		RANK_07,
		RANK_08,
		NUM
	}

	[Serializable]
	private class ScoreColor
	{
		public int red;

		public int green;

		public int blue;

		public int score;

		public float Red
		{
			get
			{
				return (float)red / 255f;
			}
		}

		public float Green
		{
			get
			{
				return (float)green / 255f;
			}
		}

		public float Blue
		{
			get
			{
				return (float)blue / 255f;
			}
		}
	}

	private const float CHARA_CHANGE_DISABLE_TIME = 5f;

	private PlayerInformation m_playerInfo;

	private LevelInformation m_levelInformation;

	private StageScoreManager m_stageScoreManager;

	private UILabel m_stockRingLabel;

	private GameObject m_addStockRing;

	private GameObject m_addStockRingEff;

	private UILabel m_ringLabel;

	private TweenColor m_ringTweenColor;

	private Color m_ringDefaultColor;

	private UISprite[] m_itemSptires = new UISprite[3];

	private UILabel m_scoreLabel;

	private UILabel m_spCrystalLabel;

	private UILabel m_animalLabel;

	private long m_realTimeScore = -1L;

	private int m_ringCount = -1;

	private int m_stockRingCount = -1;

	private int m_animalCount = -1;

	private int m_crystalCount = -1;

	private int m_distance = -1;

	private GameObject m_charaChangeBtn;

	private GameObject m_itemBtn;

	private GameObject m_quickModeObj;

	private GameObject m_endlessObj;

	private GameObject m_endlessBossObj;

	private UILabel m_distanceLabel;

	private UILabel m_timer1Label;

	private UILabel m_timer2Label;

	private TweenColor m_timer1TweenColor;

	private TweenColor m_timer2TweenColor;

	private UISlider m_distanceSlider;

	private UISlider m_speedLSlider;

	private UISlider m_speedRSlider;

	private GameObject m_bossGauge;

	private UISlider m_bossLifeSlider;

	private UILabel m_bossLifeLabel;

	private UISlider m_deathDistance;

	private TweenColor m_deathDistanceTweenColor;

	private UIImageButton m_pauseButton;

	private UIPlayAnimation m_pauseButtonAnim;

	private GameObject m_colorPowerEffect;

	private Animation m_scoreAnim;

	private List<ItemType> m_displayItems = new List<ItemType>();

	private float m_charaChaneDisableTime;

	private bool m_enablePause;

	private bool m_enableItem;

	private bool m_itemPause;

	private bool m_bBossBattleDistance;

	private bool m_bBossBattleDistanceArea;

	private bool m_bBossStart;

	private float m_bossTime;

	private bool m_quickModeFlag;

	private bool m_countDownFlag;

	private bool m_changeFlag;

	private bool m_pauseFlag;

	private int m_pauseContinueCount;

	private float m_pauseContinueTimer;

	private bool m_backTitle;

	private bool m_backMainMenuCheck;

	private bool m_createWindow;

	private bool m_itemTutorial;

	private bool m_firtsTutorial;

	private bool m_init;

	private Bitset32 m_countDownSEflags;

	private readonly string[] Anchor2TC_tbl = new string[4]
	{
		"pattern_0_default",
		"pattern_1_boss",
		"pattern_3_raid",
		"pattern_3_ev1"
	};

	private readonly string[] PauseWindowSetupLbl = new string[4]
	{
		"pause_window/pause_Anim/window/Btn_back_mainmenu/Lbl_back",
		"pause_window/pause_Anim/window/Btn_back_mainmenu/Lbl_back/Lbl_back_sh",
		"pause_window/pause_Anim/window/Btn_continue/Lbl_continue",
		"pause_window/pause_Anim/window/Btn_continue/Lbl_continue/Lbl_continue_sh"
	};

	private ScoreRank m_nextScoreRank;

	[SerializeField]
	private ScoreColor[] m_scoreColors = new ScoreColor[8];

	private Color m_scoreColor = new Color(1f, 1f, 1f, 1f);

	private void Start()
	{
		Initialize();
	}

	private void Initialize()
	{
		if (m_init)
		{
			return;
		}
		m_init = true;
		if (StageModeManager.Instance != null)
		{
			m_quickModeFlag = StageModeManager.Instance.IsQuickMode();
		}
		m_playerInfo = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
		m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		m_stageScoreManager = StageScoreManager.Instance;
		m_charaChangeBtn = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_change");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "HUD_btn_item");
		if (gameObject != null)
		{
			m_itemBtn = GameObjectUtil.FindChildGameObject(gameObject, "Btn_item");
			if (m_itemBtn != null)
			{
				m_itemBtn.SetActive(false);
			}
			for (int i = 0; i < m_itemSptires.Length; i++)
			{
				m_itemSptires[i] = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "item_" + i);
				if (m_itemSptires[i] != null)
				{
					m_itemSptires[i].gameObject.SetActive(false);
				}
			}
		}
		GameObject parent = GameObjectUtil.FindChildGameObject(base.gameObject, "HUD_indicator");
		m_distanceLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_distance");
		m_distanceSlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(parent, "Pgb_distance");
		m_speedLSlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(parent, "Pgb_speed_L");
		m_speedRSlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(parent, "Pgb_speed_R");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(parent, "pattern_0_default");
		if (gameObject2 != null)
		{
			m_endlessObj = GameObjectUtil.FindChildGameObject(gameObject2, "mode_endless");
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parent, "pattern_2_boss");
		if (gameObject3 != null)
		{
			m_endlessBossObj = GameObjectUtil.FindChildGameObject(gameObject3, "mode_endless");
		}
		m_quickModeObj = GameObjectUtil.FindChildGameObject(base.gameObject, "mode_quick_time");
		if (m_quickModeObj != null)
		{
			m_quickModeObj.SetActive(m_quickModeFlag);
		}
		if (m_quickModeFlag && m_quickModeObj != null)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_quickModeObj, "Lbl_time1");
			if (gameObject4 != null)
			{
				m_timer1Label = gameObject4.GetComponent<UILabel>();
				m_timer1TweenColor = gameObject4.GetComponent<TweenColor>();
			}
			GameObject gameObject5 = GameObjectUtil.FindChildGameObject(m_quickModeObj, "Lbl_time2");
			if (gameObject5 != null)
			{
				m_timer2Label = gameObject5.GetComponent<UILabel>();
				m_timer2TweenColor = gameObject5.GetComponent<TweenColor>();
			}
		}
		SetupBossParam(BossType.FEVER, 0, 0);
		GameObject parent2 = GameObjectUtil.FindChildGameObject(base.gameObject, "HUD_btn_pause");
		m_pauseButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent2, "Btn_pause");
		if (m_pauseButton != null)
		{
			m_pauseButtonAnim = m_pauseButton.GetComponent<UIPlayAnimation>();
			m_pauseButtonAnim.enabled = false;
			m_pauseButton.isEnabled = m_enablePause;
		}
		m_colorPowerEffect = GameObjectUtil.FindChildGameObject(base.gameObject, "HUD_ColorPower_effect");
		m_backTitle = false;
		ItemButton_SetEnabled(false);
		UpdateItemView();
	}

	private void SetupBossParam(BossType bossType, int hp, int hpMax)
	{
		GameObject parent = GameObjectUtil.FindChildGameObject(base.gameObject, "HUD_indicator");
		GameObject gameObject = null;
		string text = "pattern_2_boss";
		switch (bossType)
		{
		case BossType.FEVER:
			text = "pattern_2_boss";
			break;
		case BossType.MAP1:
		case BossType.MAP2:
		case BossType.MAP3:
			gameObject = GameObjectUtil.FindChildGameObject(parent, "pattern_0_default");
			text = "pattern_1_boss";
			break;
		default:
			gameObject = GameObjectUtil.FindChildGameObject(parent, "pattern_0_default");
			text = "pattern_3_raid";
			break;
		}
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		m_bossGauge = GameObjectUtil.FindChildGameObject(parent, text);
		if (m_bossGauge != null)
		{
			if (bossType != 0)
			{
				m_bossLifeSlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(m_bossGauge, "Pgb_boss_life");
				m_bossLifeLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_bossGauge, "Lbl_boss_life");
			}
			m_deathDistance = GameObjectUtil.FindChildGameObjectComponent<UISlider>(m_bossGauge, "Pgb_death_distance");
			m_deathDistanceTweenColor = GameObjectUtil.FindChildGameObjectComponent<TweenColor>(m_deathDistance.gameObject, "img_gauge_distance");
		}
	}

	private void Update()
	{
		if (m_createWindow && GeneralWindow.IsCreated("BackMainMenuCheckWindow"))
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				GameObjectUtil.SendMessageFindGameObject("pause_window", "OnBackMainMenuAnimation", null, SendMessageOptions.DontRequireReceiver);
				GameObjectUtil.SendMessageToTagObjects("GameModeStage", "OnMsgNotifyEndPauseExitStage", new MsgNotifyEndPauseExitStage(), SendMessageOptions.DontRequireReceiver);
				m_backTitle = true;
				m_createWindow = false;
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				ObjUtil.SetHudStockRingEffectOff(false);
				GeneralWindow.Close();
				m_createWindow = false;
			}
		}
		if (m_pauseContinueCount > 0)
		{
			m_pauseContinueTimer -= RealTime.deltaTime;
			if (m_pauseContinueTimer <= 0f)
			{
				m_pauseContinueCount--;
				if (m_pauseContinueCount > 0)
				{
					SoundManager.SePlay("sys_pause");
					m_pauseContinueTimer = 1f;
				}
				else
				{
					SoundManager.SePlay("sys_go");
					m_pauseContinueTimer = 0f;
				}
			}
		}
		if (m_playerInfo != null)
		{
			if (m_ringLabel != null)
			{
				int numRings = m_playerInfo.NumRings;
				if (m_ringCount != numRings)
				{
					m_ringCount = numRings;
					m_ringLabel.text = HudUtility.GetFormatNumString(m_ringCount);
				}
				if (m_ringTweenColor != null)
				{
					if (m_ringTweenColor.enabled)
					{
						if (numRings > 0)
						{
							m_ringTweenColor.enabled = false;
							m_ringLabel.color = m_ringDefaultColor;
						}
					}
					else if (numRings == 0)
					{
						m_ringTweenColor.enabled = true;
					}
				}
			}
			if (m_stockRingLabel != null && m_stageScoreManager != null)
			{
				int num = (int)m_stageScoreManager.Ring;
				if (m_stockRingCount != num)
				{
					m_stockRingCount = num;
					m_stockRingLabel.text = HudUtility.GetFormatNumString(m_stockRingCount);
				}
			}
			if (m_quickModeFlag)
			{
				UpdateTimer();
			}
			if (m_distanceLabel != null)
			{
				int num2 = Mathf.FloorToInt(m_playerInfo.TotalDistance);
				if (m_distance != num2)
				{
					m_distance = num2;
					m_distanceLabel.text = HudUtility.GetFormatNumString(m_distance);
				}
			}
			if (m_distanceSlider != null && m_levelInformation != null)
			{
				float num3 = (m_levelInformation.DistanceToBossOnStart != 0f) ? (m_levelInformation.DistanceOnStage / m_levelInformation.DistanceToBossOnStart) : 1f;
				if (num3 != m_distanceSlider.value)
				{
					m_distanceSlider.value = num3;
				}
			}
			float num4 = (1f + (float)m_playerInfo.SpeedLevel) / 3f;
			if (m_speedLSlider != null && m_speedLSlider.value != num4)
			{
				m_speedLSlider.value = num4;
			}
			if (m_speedRSlider != null && m_speedRSlider.value != num4)
			{
				m_speedRSlider.value = num4;
			}
		}
		if (m_stageScoreManager != null)
		{
			if (m_scoreLabel != null)
			{
				long realtimeScore = m_stageScoreManager.GetRealtimeScore();
				if (m_realTimeScore != realtimeScore)
				{
					m_realTimeScore = realtimeScore;
					m_scoreLabel.text = HudUtility.GetFormatNumString(m_realTimeScore);
					if (m_nextScoreRank < ScoreRank.NUM && m_scoreColors[(int)m_nextScoreRank].score < m_realTimeScore)
					{
						m_scoreColor.r = m_scoreColors[(int)m_nextScoreRank].Red;
						m_scoreColor.g = m_scoreColors[(int)m_nextScoreRank].Green;
						m_scoreColor.b = m_scoreColors[(int)m_nextScoreRank].Blue;
						m_scoreLabel.color = m_scoreColor;
						m_nextScoreRank++;
						ActiveAnimation.Play(m_scoreAnim, "ui_gp_bit_score_Anim", Direction.Forward);
					}
				}
			}
			if (m_spCrystalLabel != null)
			{
				int num5 = (int)m_stageScoreManager.GetRealtimeEventScore();
				if (m_crystalCount != num5)
				{
					m_crystalCount = num5;
					m_spCrystalLabel.text = HudUtility.GetFormatNumString(m_crystalCount);
				}
			}
			if (m_animalLabel != null)
			{
				int num6 = (int)m_stageScoreManager.GetRealtimeEventAnimal();
				if (m_animalCount != num6)
				{
					m_animalCount = num6;
					m_animalLabel.text = HudUtility.GetFormatNumString(m_animalCount);
				}
			}
		}
		if (m_deathDistance != null && m_bossGauge != null && m_bossGauge.activeSelf && m_playerInfo != null && m_levelInformation != null)
		{
			if (!m_bBossStart)
			{
				if (m_deathDistance.value != 0f)
				{
					m_deathDistance.value = 0f;
					m_bossTime = m_levelInformation.BossEndTime;
				}
			}
			else
			{
				float num7 = 0f;
				m_bossTime -= Time.deltaTime;
				if (m_bossTime < 0f)
				{
					m_bossTime = 0f;
				}
				if (m_bossTime > 0f && m_levelInformation.BossEndTime > 0f)
				{
					num7 = m_bossTime / m_levelInformation.BossEndTime;
				}
				m_deathDistance.value = 1f - num7;
			}
			if (m_deathDistance.value > 0.8f && m_deathDistanceTweenColor != null && !m_deathDistanceTweenColor.enabled)
			{
				m_deathDistanceTweenColor.enabled = true;
			}
			if (m_deathDistance.value == 1f && m_bBossBattleDistance)
			{
				MsgBossDistanceEnd value = new MsgBossDistanceEnd(true);
				GameObjectUtil.SendMessageToTagObjects("Boss", "OnMsgBossDistanceEnd", value, SendMessageOptions.DontRequireReceiver);
				m_bBossBattleDistance = false;
				GameObjectUtil.SendMessageToTagObjects("GameModeStage", "OnBossTimeUp", null, SendMessageOptions.DontRequireReceiver);
			}
			if (m_bossTime < 3f && !m_bBossBattleDistanceArea)
			{
				MsgBossDistanceEnd value2 = new MsgBossDistanceEnd(false);
				GameObjectUtil.SendMessageToTagObjects("Boss", "OnMsgBossDistanceEnd", value2, SendMessageOptions.DontRequireReceiver);
				m_bBossBattleDistanceArea = true;
			}
		}
		if (m_charaChaneDisableTime > 0f)
		{
			if (m_charaChaneDisableTime < Time.deltaTime)
			{
				m_charaChaneDisableTime = 0f;
			}
			else
			{
				m_charaChaneDisableTime -= Time.deltaTime;
			}
			if (m_charaChaneDisableTime == 0f)
			{
				OnChangeCharaButton(new MsgChangeCharaButton(true, true));
			}
		}
	}

	private void UpdateItemView()
	{
		string arg = (!m_enableItem) ? "ui_cmn_icon_item_g_" : "ui_cmn_icon_item_";
		for (int i = 0; i < m_itemSptires.Length; i++)
		{
			ItemType itemType = (i >= m_displayItems.Count) ? ItemType.UNKNOWN : m_displayItems[i];
			if (m_itemSptires[i] != null)
			{
				if (itemType == ItemType.UNKNOWN)
				{
					m_itemSptires[i].gameObject.SetActive(false);
					continue;
				}
				m_itemSptires[i].gameObject.SetActive(true);
				m_itemSptires[i].spriteName = arg + (int)itemType;
			}
		}
		if (m_itemBtn != null)
		{
			m_itemBtn.SetActive(m_displayItems.Count > 0);
		}
	}

	private void UpdateTimer()
	{
		if (!(StageTimeManager.Instance != null))
		{
			return;
		}
		float time = StageTimeManager.Instance.Time;
		int num = (int)time;
		int decimalNumber = (int)((time - (float)num) * 100f);
		if (m_timer1Label != null)
		{
			m_timer1Label.text = num.ToString("D2") + " .";
		}
		if (m_timer2Label != null)
		{
			m_timer2Label.text = decimalNumber.ToString("D2");
		}
		if (m_countDownFlag)
		{
			if (time > 10f)
			{
				m_countDownFlag = false;
				if (m_timer1TweenColor != null)
				{
					m_timer1TweenColor.enabled = false;
					if (m_timer1Label != null)
					{
						m_timer1Label.color = m_timer1TweenColor.from;
					}
				}
				if (m_timer2TweenColor != null)
				{
					m_timer2TweenColor.enabled = false;
					if (m_timer2Label != null)
					{
						m_timer2Label.color = m_timer2TweenColor.from;
					}
				}
			}
		}
		else if (time < 10f)
		{
			m_countDownFlag = true;
			m_countDownSEflags.Reset();
			if (m_timer1TweenColor != null)
			{
				m_timer1TweenColor.enabled = true;
			}
			if (m_timer2TweenColor != null)
			{
				m_timer2TweenColor.enabled = true;
			}
		}
		UpdateCountDownSE(num, decimalNumber);
	}

	private void UpdateCountDownSE(int integerNumber, int decimalNumber)
	{
		if (!m_countDownFlag)
		{
			return;
		}
		if (integerNumber == 0 && decimalNumber == 0)
		{
			if (!m_countDownSEflags.Test(0))
			{
				ObjUtil.PlaySE("sys_quickmode_count_zero");
				m_countDownSEflags.Set(0);
			}
			return;
		}
		if (m_countDownSEflags.Test(0))
		{
			m_countDownSEflags.Reset(0);
		}
		for (int i = 0; i < 10; i++)
		{
			if (i < integerNumber)
			{
				if (m_countDownSEflags.Test(i + 1))
				{
					m_countDownSEflags.Reset(i + 1);
				}
			}
			else if (i == integerNumber && !m_countDownSEflags.Test(i + 1))
			{
				m_countDownSEflags.Set(i + 1);
				ObjUtil.PlaySE("sys_quickmode_count");
				break;
			}
		}
	}

	private void OnClickStartPause()
	{
		GameObjectUtil.SendMessageToTagObjects("GameModeStage", "OnMsgNotifyStartPause", new MsgNotifyStartPause(), SendMessageOptions.DontRequireReceiver);
	}

	private void OnClickEndPause()
	{
		SoundManager.SePlay("sys_menu_decide");
		GC.Collect();
		Resources.UnloadUnusedAssets();
		GC.Collect();
		m_pauseContinueCount = 3;
		m_pauseContinueTimer = 1f;
	}

	private void OnFinishedContinueAnimation()
	{
		if (m_pauseFlag)
		{
			GameObjectUtil.SendMessageToTagObjects("GameModeStage", "OnMsgNotifyEndPause", new MsgNotifyEndPause(), SendMessageOptions.DontRequireReceiver);
			m_pauseFlag = false;
		}
	}

	private void OnClickEndPauseExitStage()
	{
		if (m_pauseContinueCount == 0 && m_pauseContinueTimer == 0f)
		{
			SoundManager.SePlay("sys_menu_decide");
			CreateBackMainMenuCheckWindow();
		}
	}

	private void OnEnablePause(MsgEnablePause msg)
	{
		m_enablePause = msg.m_enable;
		if (m_pauseButton != null)
		{
			m_pauseButton.isEnabled = m_enablePause;
		}
	}

	private void OnClickChange()
	{
		SoundManager.SePlay("act_chara_change");
		if (m_charaChaneDisableTime == 0f)
		{
			MsgChangeChara msgChangeChara = new MsgChangeChara();
			msgChangeChara.m_changeByBtn = true;
			GameObjectUtil.SendMessageToTagObjects("GameModeStage", "OnMsgChangeChara", msgChangeChara, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnChangeCharaSucceed(MsgChangeCharaSucceed msg)
	{
		m_charaChaneDisableTime = 5f;
		if (msg.m_disabled)
		{
			ChangeButton_SetActive(false);
			return;
		}
		m_changeFlag = true;
		ChangeButton_SetEnabled(false);
	}

	private void OnChangeCharaEnable(MsgChangeCharaEnable msg)
	{
		Initialize();
		ChangeButton_SetActive(msg.value);
		ChangeButton_SetEnabled(false);
	}

	private void OnChangeCharaButton(MsgChangeCharaButton msg)
	{
		if (!msg.value)
		{
			if (!msg.pause)
			{
				m_changeFlag = false;
			}
			ChangeButton_SetEnabled(false);
			return;
		}
		if (!msg.pause)
		{
			m_changeFlag = true;
		}
		if (m_charaChaneDisableTime == 0f && m_changeFlag)
		{
			ChangeButton_SetEnabled(true);
		}
	}

	private void ChangeButton_SetActive(bool isActive)
	{
		if (m_charaChangeBtn != null)
		{
			m_charaChangeBtn.SetActive(isActive);
		}
	}

	private void ChangeButton_SetEnabled(bool isEnabled)
	{
		if (m_charaChangeBtn != null)
		{
			UIImageButton component = m_charaChangeBtn.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = isEnabled;
			}
		}
	}

	private void ItemButton_SetActive(bool isActive)
	{
		if (m_itemBtn != null)
		{
			m_itemBtn.SetActive(isActive);
		}
	}

	private void ItemButton_SetEnabled(bool isEnabled)
	{
		m_enableItem = isEnabled;
		if (m_itemBtn != null)
		{
			UIImageButton component = m_itemBtn.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = isEnabled;
			}
		}
		UpdateItemView();
	}

	private void OnItemEnable(MsgItemButtonEnable msg)
	{
		if (msg.m_enable && !m_itemPause)
		{
			ItemButton_SetEnabled(true);
		}
		else
		{
			ItemButton_SetEnabled(false);
		}
	}

	private void OnStartTutorial()
	{
		m_itemTutorial = true;
		HudTutorial.StartTutorial(HudTutorial.Id.ITEM_BUTTON_1, BossType.NONE);
	}

	private void OnNextTutorial()
	{
		if (m_itemTutorial)
		{
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.STAGE_ITEM);
		}
	}

	private void OnSetEquippedItem(MsgSetEquippedItem msg)
	{
		ItemType[] itemType = msg.m_itemType;
		foreach (ItemType itemType2 in itemType)
		{
			if (itemType2 != ItemType.UNKNOWN)
			{
				m_displayItems.Add(itemType2);
			}
		}
		UpdateItemView();
	}

	private void OnSetPresentEquippedItem(MsgSetEquippedItem msg)
	{
		bool flag = m_displayItems.Count > 0;
		m_displayItems.Clear();
		ItemType[] itemType = msg.m_itemType;
		foreach (ItemType itemType2 in itemType)
		{
			if (itemType2 != ItemType.UNKNOWN)
			{
				m_displayItems.Add(itemType2);
			}
		}
		UpdateItemView();
		if (!flag && msg.m_enabled && !m_itemPause)
		{
			ItemButton_SetEnabled(true);
		}
	}

	private void OnChangeItem(MsgSetEquippedItem msg)
	{
		bool flag = m_displayItems.Count > 0;
		m_displayItems.Clear();
		ItemType[] itemType = msg.m_itemType;
		foreach (ItemType itemType2 in itemType)
		{
			if (itemType2 != ItemType.UNKNOWN)
			{
				m_displayItems.Add(itemType2);
			}
		}
		UpdateItemView();
	}

	private void OnUsedItem(MsgUsedItem msg)
	{
		m_displayItems.Remove(msg.m_itemType);
		UpdateItemView();
	}

	private void OnSetPause(MsgSetPause msg)
	{
		if (GeneralWindow.IsCreated("BackMainMenuCheckWindow") || m_backTitle)
		{
			return;
		}
		if (m_pauseFlag && msg.m_backKey)
		{
			if (m_pauseContinueCount == 0 && m_pauseContinueTimer == 0f)
			{
				GameObjectUtil.SendMessageFindGameObject("pause_window", "OnContinueAnimation", null, SendMessageOptions.DontRequireReceiver);
				OnClickEndPause();
				if (m_pauseButton != null)
				{
					NGUITools.SetActive(m_pauseButton.gameObject, true);
				}
			}
		}
		else
		{
			if (!(m_pauseButton != null) || !(m_pauseButtonAnim != null))
			{
				return;
			}
			Animation target = m_pauseButtonAnim.target;
			if (target != null)
			{
				m_pauseFlag = true;
				GameObjectUtil.SendMessageFindGameObject("pause_window", "OnMsgNotifyStartPause", null, SendMessageOptions.DontRequireReceiver);
				m_pauseButton.gameObject.SetActive(false);
				m_pauseContinueCount = 0;
				m_pauseContinueTimer = 0f;
				target.gameObject.SetActive(true);
				target.Rewind(pause_window.INANIM_NAME);
				ActiveAnimation.Play(target, pause_window.INANIM_NAME, Direction.Forward, true);
				if (msg.m_backMainMenu)
				{
					CreateBackMainMenuCheckWindow();
				}
			}
		}
	}

	private void HudBossHpGaugeOpen(MsgHudBossHpGaugeOpen msg)
	{
		SetupBossParam(msg.m_bossType, msg.m_hp, msg.m_hpMax);
		if (m_bossGauge != null)
		{
			m_bossGauge.SetActive(true);
		}
		SetBossHp(msg.m_hp, msg.m_hpMax);
		m_bBossBattleDistance = true;
		if (m_deathDistanceTweenColor != null)
		{
			m_deathDistanceTweenColor.enabled = false;
		}
	}

	private void HudBossGaugeStart(MsgHudBossGaugeStart msg)
	{
		m_bBossStart = true;
		m_bBossBattleDistanceArea = false;
	}

	private void OnBossEnd(MsgBossEnd msg)
	{
		if (m_bossGauge != null)
		{
			m_bossGauge.SetActive(false);
		}
		m_bBossStart = false;
		m_bBossBattleDistanceArea = false;
	}

	private void HudBossHpGaugeSet(MsgHudBossHpGaugeSet msg)
	{
		SetBossHp(msg.m_hp, msg.m_hpMax);
	}

	private void SetBossHp(int hp, int hpMax)
	{
		float value = 0f;
		if (hp > 0)
		{
			value = (float)hp / (float)hpMax;
		}
		if (m_bossLifeSlider != null)
		{
			m_bossLifeSlider.value = value;
		}
		if (m_bossLifeLabel != null)
		{
			m_bossLifeLabel.text = hp + "/" + hpMax;
		}
	}

	private void OnMsgPrepareContinue(MsgPrepareContinue msg)
	{
		if (msg.m_bossStage && m_deathDistance != null && m_levelInformation != null)
		{
			m_deathDistance.value = 0f;
			m_bossTime = m_levelInformation.BossEndTime;
		}
	}

	private void OnPhantomActStart(MsgPhantomActStart msg)
	{
		if (m_colorPowerEffect != null)
		{
			m_colorPowerEffect.SetActive(true);
		}
	}

	private void OnPhantomActEnd(MsgPhantomActEnd msg)
	{
		if (m_colorPowerEffect != null)
		{
			m_colorPowerEffect.SetActive(false);
		}
	}

	private void OnAddStockRing(MsgAddStockRing msg)
	{
		if (m_addStockRing != null && msg.m_numAddRings > 0)
		{
			m_addStockRing.SetActive(false);
			m_addStockRing.SetActive(true);
			SoundManager.SePlay("act_ring_collect");
		}
	}

	private void OnSetup(MsgHudCockpitSetup msg)
	{
		m_backMainMenuCheck = msg.m_backMainMenuCheck;
		m_firtsTutorial = msg.m_firstTutorial;
		GameObject[] array = new GameObject[4];
		GameObject gameObject = base.gameObject;
		if (gameObject != null)
		{
			for (int i = 0; i < 4; i++)
			{
				Transform transform = gameObject.transform.FindChild("Anchor_2_TC/" + Anchor2TC_tbl[i]);
				if (transform != null)
				{
					array[i] = transform.gameObject;
					if (array[i] != null)
					{
						array[i].SetActive(false);
					}
				}
			}
		}
		GameObject gameObject2 = GameObject.Find("UI Root (2D)/Camera/Anchor_5_MC");
		if (gameObject2 != null)
		{
			for (int j = 0; j < PauseWindowSetupLbl.Length; j++)
			{
				Transform transform2 = gameObject2.transform.FindChild(PauseWindowSetupLbl[j]);
				if (transform2 != null)
				{
					HudUtility.SetupUILabelText(transform2.gameObject);
				}
			}
		}
		if (msg.m_bossType == BossType.MAP1 || msg.m_bossType == BossType.MAP2 || msg.m_bossType == BossType.MAP3)
		{
			GameObject gameObject3 = array[1];
			if (gameObject3 != null)
			{
				gameObject3.SetActive(true);
			}
			SetupRingObj(gameObject3);
		}
		else if (msg.m_bossType != BossType.NONE && msg.m_bossType != 0)
		{
			GameObject gameObject4 = array[2];
			if (gameObject4 != null)
			{
				gameObject4.SetActive(true);
			}
			SetupRingObj(gameObject4);
		}
		else if (msg.m_spCrystal)
		{
			GameObject gameObject5 = array[3];
			if (gameObject5 != null)
			{
				gameObject5.SetActive(true);
			}
			SetupRingObj(gameObject5);
			SetupScoreObj(gameObject5);
			m_spCrystalLabel = SetupEventObj(gameObject5, "HUD_event", "Lbl_event", "img_event_object", "ui_event_object_icon");
		}
		else if (msg.m_animal)
		{
			GameObject gameObject6 = array[3];
			if (gameObject6 != null)
			{
				gameObject6.SetActive(true);
			}
			SetupRingObj(gameObject6);
			SetupScoreObj(gameObject6);
			m_animalLabel = SetupEventObj(gameObject6, "HUD_event", "Lbl_event", "img_event_object", "ui_event_object_icon");
		}
		else
		{
			GameObject gameObject7 = array[0];
			if (gameObject7 != null)
			{
				gameObject7.SetActive(true);
			}
			SetupRingObj(gameObject7);
			SetupScoreObj(gameObject7);
		}
		if (m_stageScoreManager != null)
		{
			m_stageScoreManager.SetupScoreRate();
		}
		if (m_firtsTutorial)
		{
			GameObjectUtil.SendMessageFindGameObject("pause_window", "OnSetFirstTutorial", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SetupScoreObj(GameObject patternObj)
	{
		if (patternObj != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(patternObj, "HUD_score");
			if (gameObject != null)
			{
				m_scoreLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_score");
				m_scoreAnim = GameObjectUtil.FindChildGameObjectComponent<Animation>(gameObject, "Anim_score");
			}
		}
	}

	private UILabel SetupEventObj(GameObject patternObj, string objName, string lblName, string imgName, string texName)
	{
		UILabel result = null;
		if (patternObj != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(patternObj, objName);
			if (gameObject != null)
			{
				result = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, lblName);
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, imgName);
				if (uISprite != null)
				{
					uISprite.spriteName = texName;
				}
			}
		}
		return result;
	}

	private void SetupRingObj(GameObject patternObj)
	{
		if (!(patternObj != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(patternObj, "HUD_ring");
		if (gameObject != null)
		{
			m_stockRingLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_stock_ring");
			m_ringLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_ring");
			if (m_ringLabel != null)
			{
				m_ringDefaultColor = m_ringLabel.color;
			}
			m_ringTweenColor = GameObjectUtil.FindChildGameObjectComponent<TweenColor>(gameObject, "Lbl_ring");
			m_addStockRing = GameObjectUtil.FindChildGameObject(gameObject, "add");
			if (m_addStockRing != null)
			{
				m_addStockRingEff = GameObjectUtil.FindChildGameObject(m_addStockRing, "eff_switch");
				m_addStockRing.SetActive(false);
			}
		}
	}

	private void CreateBackMainMenuCheckWindow()
	{
		string cellName = (!m_backMainMenuCheck) ? "ui_back_menu_text_option" : "ui_back_menu_text";
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "BackMainMenuCheckWindow";
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PauseWindow", "ui_back_menu_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PauseWindow", cellName).text;
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
		ObjUtil.SetHudStockRingEffectOff(true);
		m_createWindow = true;
	}

	private void OnPauseItemOnBoss()
	{
		m_itemPause = true;
		OnItemEnable(new MsgItemButtonEnable(false));
	}

	private void OnResumeItemOnBoss(bool phatomFlag)
	{
		m_itemPause = false;
		if (!phatomFlag)
		{
			OnItemEnable(new MsgItemButtonEnable(true));
		}
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
		if (m_addStockRing != null)
		{
			m_addStockRing.SetActive(false);
		}
	}

	private void OnMsgStockRingEffect(MsgHudStockRingEffect msg)
	{
		if (m_addStockRingEff != null)
		{
			if (msg.m_off)
			{
				m_addStockRingEff.transform.localPosition = new Vector3(1000f, 1000f, 0f);
			}
			else
			{
				m_addStockRingEff.transform.localPosition = Vector3.zero;
			}
		}
	}

	private void OnClickItemButton()
	{
		if (m_levelInformation != null)
		{
			m_levelInformation.RequestEqitpItem = true;
		}
		if (m_itemTutorial)
		{
			m_itemTutorial = false;
			TutorialCursor.DestroyTutorialCursor();
			MsgTutorialEnd value = new MsgTutorialEnd();
			GameObjectUtil.SendMessageToTagObjects("GameModeStage", "OnMsgTutorialItemButtonEnd", value, SendMessageOptions.DontRequireReceiver);
		}
		if (!(StageItemManager.Instance != null))
		{
			return;
		}
		for (int i = 0; i < m_displayItems.Count; i++)
		{
			if (m_displayItems[i] != ItemType.UNKNOWN)
			{
				StageItemManager.Instance.OnRequestItemUse(new MsgAskEquipItemUsed(m_displayItems[i]));
				break;
			}
		}
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}
}
