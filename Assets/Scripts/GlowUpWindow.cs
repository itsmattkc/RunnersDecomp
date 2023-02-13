using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;
using Text;
using UI;
using UnityEngine;

public class GlowUpWindow : MonoBehaviour
{
	public enum ExpType
	{
		RUN_STAGE,
		BOSS_SUCCESS,
		BOSS_FAILED
	}

	private enum Type
	{
		None = -1,
		Main,
		Sub,
		Num
	}

	private enum State
	{
		None = -1,
		Idle,
		Setup,
		WaitSetup,
		OnInAnim,
		Playing,
		WaitTouchButton,
		OnOutAnim,
		End,
		Num
	}

	private static readonly string[] CharaPlateName = new string[2]
	{
		"player_main",
		"player_sub"
	};

	private State m_state = State.Setup;

	private GlowUpCharacter[] m_charaPlate = new GlowUpCharacter[2];

	public bool IsPlayEnd
	{
		get
		{
			if (m_state == State.End)
			{
				return true;
			}
			return false;
		}
	}

	public void PlayStart(ExpType expType)
	{
		base.gameObject.SetActive(true);
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, false);
		}
		CharaType[] array = new CharaType[2]
		{
			SaveDataManager.Instance.PlayerData.MainChara,
			SaveDataManager.Instance.PlayerData.SubChara
		};
		bool flag = true;
		for (int i = 0; i < 2; i++)
		{
			string name = CharaPlateName[i];
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, name);
			if (gameObject == null)
			{
				continue;
			}
			if (m_charaPlate[i] == null)
			{
				m_charaPlate[i] = gameObject.AddComponent<GlowUpCharacter>();
			}
			GlowUpCharaBaseInfo glowUpCharaBaseInfo = new GlowUpCharaBaseInfo();
			CharaType charaType = array[i];
			if (ServerInterface.LoggedInServerInterface != null)
			{
				ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(charaType);
				if (serverCharacterState != null)
				{
					glowUpCharaBaseInfo.charaType = charaType;
					glowUpCharaBaseInfo.level = CalcCharacterTotalLevel(serverCharacterState.OldAbiltyLevel);
					glowUpCharaBaseInfo.levelUpCost = serverCharacterState.OldCost;
					glowUpCharaBaseInfo.currentExp = serverCharacterState.OldExp;
					bool flag2 = false;
					ServerPlayCharacterState serverPlayCharacterState = ServerInterface.PlayerState.PlayCharacterState(charaType);
					if (serverPlayCharacterState != null)
					{
						flag2 = true;
					}
					glowUpCharaBaseInfo.IsActive = flag2;
					if (flag2 && serverCharacterState.OldStatus != ServerCharacterState.CharacterStatus.MaxLevel)
					{
						flag = false;
					}
				}
			}
			m_charaPlate[i].Setup(glowUpCharaBaseInfo);
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		if (!EventManager.Instance.IsRaidBossStage())
		{
			empty3 = ((expType == ExpType.BOSS_FAILED) ? "ui_Lbl_player_exp_failed" : (flag ? "ui_Lbl_player_exp_level_max" : ((expType != ExpType.BOSS_SUCCESS) ? "ui_Lbl_player_exp" : "ui_Lbl_player_exp_success")));
		}
		else
		{
			bool flag3 = false;
			EventManager instance2 = EventManager.Instance;
			if (instance2 != null)
			{
				ServerEventRaidBossBonus raidBossBonus = instance2.RaidBossBonus;
				if (raidBossBonus != null && raidBossBonus.BeatBonus > 0)
				{
					flag3 = true;
				}
			}
			empty3 = ((!flag3) ? "ui_Lbl_player_exp_failed" : ((!flag) ? "ui_Lbl_player_exp_success_raid" : "ui_Lbl_player_exp_level_max"));
		}
		empty = empty3 + "_caption";
		empty2 = empty3 + "_text";
		Debug.Log("ExpCaption: " + empty);
		Debug.Log("ExpText: " + empty2);
		GameObject labelObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_caption");
		SetupLabel(labelObject, "Result", empty);
		GameObject labelObject2 = null;
		GameObject x = GameObjectUtil.FindChildGameObject(base.gameObject, "window_contents");
		if (x != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "body");
			if (gameObject2 != null)
			{
				labelObject2 = GameObjectUtil.FindChildGameObject(gameObject2, "Lbl_body");
			}
		}
		SetupLabel(labelObject2, "Result", empty2);
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
		if (uIImageButton != null)
		{
			uIImageButton.isEnabled = false;
		}
		m_state = State.WaitSetup;
	}

	private void Start()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		for (int i = 0; i < 2; i++)
		{
			m_charaPlate[i] = null;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_skip");
		if (gameObject != null)
		{
			UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "SkipButtonClickedCallback";
			}
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_ok");
		if (gameObject2 != null)
		{
			UIButtonMessage uIButtonMessage2 = gameObject2.GetComponent<UIButtonMessage>();
			if (uIButtonMessage2 == null)
			{
				uIButtonMessage2 = gameObject2.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage2 != null)
			{
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "ButtonClickedCallback";
			}
		}
		base.gameObject.SetActive(false);
		m_state = State.Idle;
	}

	private void OnDestroy()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.Idle:
			break;
		case State.Setup:
			break;
		case State.OnInAnim:
			break;
		case State.WaitTouchButton:
			break;
		case State.OnOutAnim:
			break;
		case State.End:
			break;
		case State.WaitSetup:
		{
			bool flag2 = true;
			for (int j = 0; j < 2; j++)
			{
				GlowUpCharacter glowUpCharacter2 = m_charaPlate[j];
				if (!(glowUpCharacter2 == null) && !glowUpCharacter2.IsEndSetup)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				Animation component = base.gameObject.GetComponent<Animation>();
				if (component != null)
				{
					ActiveAnimation activeAnimation = ActiveAnimation.Play(component, "ui_cmn_window_Anim2", Direction.Forward);
					EventDelegate.Add(activeAnimation.onFinished, InAnimationEndCallback, true);
				}
				m_state = State.OnInAnim;
			}
			break;
		}
		case State.Playing:
		{
			bool flag = true;
			GlowUpCharacter[] charaPlate = m_charaPlate;
			foreach (GlowUpCharacter glowUpCharacter in charaPlate)
			{
				if (!(glowUpCharacter == null) && !glowUpCharacter.IsEnd)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				SoundManager.SeStop("sys_gauge");
				GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_skip");
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
				UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = true;
				}
				m_state = State.WaitTouchButton;
			}
			break;
		}
		}
	}

	private void InAnimationEndCallback()
	{
		StartCoroutine(OnInAnimationEnd());
	}

	private IEnumerator OnInAnimationEnd()
	{
		yield return new WaitForSeconds(0.5f);
		GameObject blinder = GameObjectUtil.FindChildGameObject(base.gameObject, "anime_blinder");
		if (blinder != null)
		{
			blinder.SetActive(false);
		}
		GameObject skipButtonObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_skip");
		if (skipButtonObject != null)
		{
			skipButtonObject.SetActive(true);
		}
		CharaType[] charaTypeList = new CharaType[2]
		{
			SaveDataManager.Instance.PlayerData.MainChara,
			SaveDataManager.Instance.PlayerData.SubChara
		};
		for (int index = 0; index < 2; index++)
		{
			GlowUpCharacter charaPlate = m_charaPlate[index];
			if (charaPlate == null)
			{
				continue;
			}
			GlowUpCharaAfterInfo afterInfo = new GlowUpCharaAfterInfo();
			CharaType charaType = charaTypeList[index];
			if (ServerInterface.LoggedInServerInterface != null)
			{
				ServerCharacterState charaState = ServerInterface.PlayerState.CharacterState(charaType);
				if (charaState != null)
				{
					afterInfo.level = CalcCharacterTotalLevel(charaState.AbilityLevel);
					afterInfo.levelUpCost = charaState.Cost;
					afterInfo.exp = charaState.Exp;
				}
				ServerPlayCharacterState playCharaState = ServerInterface.PlayerState.PlayCharacterState(charaType);
				if (playCharaState != null)
				{
					List<AbilityType> abilityList = new List<AbilityType>();
					foreach (int ability in playCharaState.abilityLevelUp)
					{
						abilityList.Add((AbilityType)ability);
					}
					afterInfo.abilityList = abilityList;
					List<int> abilityLevelupList = new List<int>();
					foreach (int cost in playCharaState.abilityLevelUpExp)
					{
						abilityLevelupList.Add(cost);
					}
					afterInfo.abilityListExp = abilityLevelupList;
				}
			}
			charaPlate.PlayStart(afterInfo);
		}
		SoundManager.SePlay("sys_gauge");
		m_state = State.Playing;
	}

	private void OutAnimationEndCallback()
	{
		base.gameObject.SetActive(false);
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, true);
		}
		m_state = State.End;
	}

	private void SkipButtonClickedCallback()
	{
		State state = m_state;
		if (state != State.Playing)
		{
			return;
		}
		GlowUpCharacter[] charaPlate = m_charaPlate;
		foreach (GlowUpCharacter glowUpCharacter in charaPlate)
		{
			if (!(glowUpCharacter == null))
			{
				glowUpCharacter.PlaySkip();
			}
		}
	}

	private void ButtonClickedCallback()
	{
		State state = m_state;
		if (state == State.WaitTouchButton)
		{
			SoundManager.SePlay("sys_menu_decide");
			Animation component = base.gameObject.GetComponent<Animation>();
			if (component != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Reverse);
				EventDelegate.Add(activeAnimation.onFinished, OutAnimationEndCallback, true);
			}
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
			if (uIImageButton != null)
			{
				uIImageButton.isEnabled = false;
			}
			m_state = State.OnOutAnim;
		}
	}

	private int CalcCharacterTotalLevel(List<int> abilityLevelList)
	{
		int num = 0;
		if (abilityLevelList == null)
		{
			return num;
		}
		foreach (int abilityLevel in abilityLevelList)
		{
			num += abilityLevel;
		}
		return num;
	}

	private void SetupLabel(GameObject labelObject, string groupName, string cellName)
	{
		if (!(labelObject == null))
		{
			labelObject.SetActive(true);
			UILabel component = labelObject.GetComponent<UILabel>();
			if (component != null)
			{
				string text2 = component.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, groupName, cellName).text;
			}
			UILocalizeText component2 = labelObject.GetComponent<UILocalizeText>();
			if (component2 != null)
			{
				component2.enabled = false;
			}
		}
	}

	private void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		ButtonClickedCallback();
	}
}
