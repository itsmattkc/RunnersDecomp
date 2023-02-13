using Message;
using SaveData;
using System;
using Text;
using Tutorial;
using UnityEngine;

public class HudTutorial : MonoBehaviour
{
	private enum ExplanPatternType
	{
		TEXT_ONLY,
		IMAGE_ONLY,
		TEXT_AND_IMAGE,
		COUNT
	}

	[Serializable]
	private class ExplanPattern
	{
		[SerializeField]
		public GameObject m_gameObject;

		[SerializeField]
		public UILabel m_label;

		[SerializeField]
		public UITexture m_texture;
	}

	public enum Id
	{
		NONE = -1,
		MISSION_1,
		MISSION_2,
		MISSION_3,
		MISSION_4,
		MISSION_5,
		MISSION_6,
		MISSION_7,
		MISSION_8,
		MISSION_END,
		FEVERBOSS,
		MAPBOSS_1,
		MAPBOSS_2,
		MAPBOSS_3,
		EVENTBOSS_1,
		EVENTBOSS_2,
		ITEM_1,
		ITEM_2,
		ITEM_3,
		ITEM_4,
		ITEM_5,
		ITEM_6,
		ITEM_7,
		ITEM_8,
		CHARA_0,
		CHARA_1,
		CHARA_2,
		CHARA_3,
		CHARA_4,
		CHARA_5,
		CHARA_6,
		CHARA_7,
		CHARA_8,
		CHARA_9,
		CHARA_10,
		CHARA_11,
		CHARA_12,
		CHARA_13,
		CHARA_14,
		CHARA_15,
		CHARA_16,
		CHARA_17,
		CHARA_18,
		CHARA_19,
		CHARA_20,
		CHARA_21,
		CHARA_22,
		CHARA_23,
		CHARA_24,
		CHARA_25,
		CHARA_26,
		CHARA_27,
		CHARA_28,
		ACTION_1,
		ITEM_BUTTON_1,
		QUICK_1,
		NUM
	}

	public enum Kind
	{
		NONE = -1,
		MISSION,
		MISSION_END,
		FEVERBOSS,
		MAPBOSS,
		EVENTBOSS,
		ITEM,
		ITEM_BUTTON,
		CHARA,
		ACTION,
		QUICK
	}

	private enum Phase
	{
		NONE = -1,
		OPEN,
		OPEN_WAIT,
		WAIT,
		CLOSE_WAIT,
		CLOSE,
		PLAY,
		SUCCESS,
		RETRY,
		RESULT,
		END
	}

	public class TutorialData
	{
		public EventID m_eventID;

		public int m_textureCount;

		public int m_textureNumber1;

		public int m_textureNumber2;

		public TutorialData(EventID eventID, int count, int num1, int num2)
		{
			m_eventID = eventID;
			m_textureCount = count;
			m_textureNumber1 = num1;
			m_textureNumber2 = num2;
		}
	}

	public class ActionTutorialData
	{
		public SystemData.ActionTutorialFlagStatus m_flagStatus;

		public string m_textCategory;

		public string m_textCell;

		public ActionTutorialData(SystemData.ActionTutorialFlagStatus flagStatus, string textCategory, string textCell)
		{
			m_flagStatus = flagStatus;
			m_textCategory = textCategory;
			m_textCell = textCell;
		}
	}

	public class QuickModeTutorialData
	{
		public SystemData.QuickModeTutorialFlagStatus m_flagStatus;

		public QuickModeTutorialData(SystemData.QuickModeTutorialFlagStatus flagStatus)
		{
			m_flagStatus = flagStatus;
		}
	}

	[SerializeField]
	private GameObject m_captionGameObject;

	[SerializeField]
	private UILabel m_captionLabel;

	[SerializeField]
	private GameObject m_explanGameObject;

	[SerializeField]
	private ExplanPattern[] m_explanPatterns = new ExplanPattern[3];

	[SerializeField]
	private GameObject m_successGameObject;

	[SerializeField]
	private GameObject m_retryGameObject;

	[SerializeField]
	private GameObject m_anchorObj;

	private static readonly TutorialData[] TUTORIAL_DATA_TBL = new TutorialData[55]
	{
		new TutorialData(EventID.JUMP, 1, 0, 0),
		new TutorialData(EventID.DOUBLE_JUMP, 1, 1, 0),
		new TutorialData(EventID.RING_BONUS, 2, 2, 8),
		new TutorialData(EventID.ENEMY, 1, 3, 0),
		new TutorialData(EventID.DAMAGE, 2, 4, 9),
		new TutorialData(EventID.MISS, 1, 5, 0),
		new TutorialData(EventID.PARA_LOOP, 1, 6, 0),
		new TutorialData(EventID.FEVER_BOSS, 1, 7, 0),
		new TutorialData(EventID.COMPLETE, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 1),
		new TutorialData(EventID.NUM, 2, 0, 1),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 1, 0),
		new TutorialData(EventID.NUM, 1, 2, 0),
		new TutorialData(EventID.NUM, 1, 3, 0),
		new TutorialData(EventID.NUM, 1, 4, 0),
		new TutorialData(EventID.NUM, 1, 5, 0),
		new TutorialData(EventID.NUM, 1, 6, 0),
		new TutorialData(EventID.NUM, 1, 7, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0),
		new TutorialData(EventID.NUM, 0, 0, 0),
		new TutorialData(EventID.NUM, 1, 0, 0)
	};

	private static readonly ActionTutorialData[] ACTION_TUTORIAL_DATA_TBL = new ActionTutorialData[1]
	{
		new ActionTutorialData(SystemData.ActionTutorialFlagStatus.ACTION_1, "Item", "crystal_2")
	};

	private static readonly QuickModeTutorialData[] QUICK_MODE_TUTORIAL_DATA_TBL = new QuickModeTutorialData[1]
	{
		new QuickModeTutorialData(SystemData.QuickModeTutorialFlagStatus.QUICK_1)
	};

	private static string MISSION_SCENENAME = "ui_tex_tutorial_mission";

	private static string FEVERBOSS_SCENENAME = "ui_tex_tutorial_feverboss";

	private static string MAPBOSS_SCENENAME = "ui_tex_tutorial_mapboss_";

	private static string EVENTBOSS_SCENENAME = "ui_tex_tutorial_eventboss_";

	private static string ITEM_SCENENAME = "ui_tex_tutorial_item";

	private static string CHARA_SCENENAME = "ui_tex_tutorial_chara_";

	private static string ACTION_SCENENAME = "ui_tex_tutorial_action_";

	private static string QUICK_SCENENAME = "ui_tex_tutorial_quick_";

	private ExplanPattern m_explanPattern;

	private Id m_id;

	private Phase m_phase;

	private int m_textureCount;

	private int m_currentTextureIndex;

	private float m_timer;

	private BossType m_bossType;

	private Kind kind
	{
		get
		{
			return GetKind(m_id);
		}
	}

	public static Kind GetKind(Id id)
	{
		if (id == Id.NONE)
		{
			return Kind.NONE;
		}
		if (id < Id.MISSION_END)
		{
			return Kind.MISSION;
		}
		if (id == Id.MISSION_END)
		{
			return Kind.MISSION_END;
		}
		if (id < Id.MAPBOSS_1)
		{
			return Kind.FEVERBOSS;
		}
		if (id < Id.EVENTBOSS_1)
		{
			return Kind.MAPBOSS;
		}
		if (id < Id.ITEM_1)
		{
			return Kind.EVENTBOSS;
		}
		if (id < Id.CHARA_0)
		{
			return Kind.ITEM;
		}
		if (id < Id.ACTION_1)
		{
			return Kind.CHARA;
		}
		if (id < Id.ITEM_BUTTON_1)
		{
			return Kind.ACTION;
		}
		if (id < Id.QUICK_1)
		{
			return Kind.ITEM_BUTTON;
		}
		return Kind.QUICK;
	}

	private void Start()
	{
		Initialize();
	}

	private void SetTexture(string sceneName, int index)
	{
		m_explanPattern.m_texture.mainTexture = null;
		GameObject gameObject = GameObject.Find(sceneName);
		if (gameObject == null)
		{
			GameObject gameObject2 = GameObject.Find("EventResourceCommon");
			if (gameObject2 != null)
			{
				for (int i = 0; i < gameObject2.transform.childCount; i++)
				{
					GameObject gameObject3 = gameObject2.transform.GetChild(i).gameObject;
					if (gameObject3.name == sceneName)
					{
						gameObject = gameObject3;
						break;
					}
				}
			}
		}
		if (gameObject != null)
		{
			HudTutorialTexture component = gameObject.GetComponent<HudTutorialTexture>();
			if (component != null && (uint)index < (uint)component.m_texList.Length)
			{
				m_explanPattern.m_texture.mainTexture = component.m_texList[index];
			}
		}
		m_explanPattern.m_texture.enabled = true;
	}

	private void SetTexture(int index)
	{
		TutorialData tutorialData = GetTutorialData(m_id);
		if (tutorialData != null)
		{
			int num = 0;
			switch (index)
			{
			default:
				return;
			case 0:
				num = tutorialData.m_textureNumber1;
				break;
			case 1:
				num = tutorialData.m_textureNumber2;
				break;
			}
			switch (kind)
			{
			case Kind.MISSION_END:
			case Kind.ITEM_BUTTON:
				break;
			case Kind.MISSION:
				SetTexture(MISSION_SCENENAME, num);
				break;
			case Kind.FEVERBOSS:
				SetTexture(FEVERBOSS_SCENENAME, num);
				break;
			case Kind.MAPBOSS:
				SetTexture(MAPBOSS_SCENENAME + (int)(m_id - 10), num);
				break;
			case Kind.EVENTBOSS:
				SetTexture(EVENTBOSS_SCENENAME + (int)(m_id - 13), num);
				break;
			case Kind.ITEM:
				SetTexture(ITEM_SCENENAME, num);
				break;
			case Kind.QUICK:
				SetTexture(QUICK_SCENENAME + (int)(m_id - 54 + 1), num);
				break;
			case Kind.CHARA:
				SetTexture(CHARA_SCENENAME + (int)(m_id - 23), num);
				break;
			case Kind.ACTION:
				SetTexture(ACTION_SCENENAME + (int)(m_id - 52 + 1), num);
				break;
			}
		}
	}

	private void Update()
	{
		if (m_id == Id.NONE)
		{
			return;
		}
		switch (m_phase)
		{
		case Phase.WAIT:
			break;
		case Phase.PLAY:
			break;
		case Phase.RESULT:
			break;
		case Phase.NONE:
			switch (kind)
			{
			case Kind.MISSION:
			case Kind.FEVERBOSS:
			case Kind.MAPBOSS:
			case Kind.ITEM:
			case Kind.CHARA:
			case Kind.ACTION:
			case Kind.QUICK:
				m_captionGameObject.SetActive(true);
				m_captionLabel.text = GetCaptionText(m_id);
				m_explanGameObject.SetActive(true);
				m_explanPattern = m_explanPatterns[2];
				m_explanPattern.m_gameObject.SetActive(true);
				m_explanPattern.m_label.text = GetExplainText(m_id);
				SetTexture(0);
				break;
			case Kind.MISSION_END:
				m_explanGameObject.SetActive(true);
				m_explanPattern = m_explanPatterns[0];
				m_explanPattern.m_gameObject.SetActive(true);
				m_explanPattern.m_label.text = GetExplainText(m_id);
				break;
			case Kind.EVENTBOSS:
				m_captionGameObject.SetActive(true);
				m_captionLabel.text = GetEventBossCaptionText(m_id, m_bossType);
				m_explanGameObject.SetActive(true);
				m_explanPattern = m_explanPatterns[2];
				m_explanPattern.m_gameObject.SetActive(true);
				m_explanPattern.m_label.text = GetEventBossExplainText(m_id, m_bossType);
				SetTexture(0);
				break;
			}
			if (kind == Kind.ITEM_BUTTON)
			{
				m_phase = Phase.CLOSE;
				break;
			}
			m_textureCount = GetTexuterPageCount(m_id);
			m_currentTextureIndex = 0;
			m_timer = 1f;
			m_phase = Phase.OPEN;
			break;
		case Phase.OPEN:
			if (m_anchorObj != null)
			{
				m_anchorObj.SetActive(true);
			}
			m_phase = Phase.OPEN_WAIT;
			break;
		case Phase.OPEN_WAIT:
			m_timer -= RealTime.deltaTime;
			if (m_timer <= 0f)
			{
				m_phase = Phase.WAIT;
			}
			break;
		case Phase.CLOSE_WAIT:
			m_timer -= RealTime.deltaTime;
			if (m_timer <= 0f)
			{
				m_phase = Phase.CLOSE;
			}
			break;
		case Phase.CLOSE:
		{
			m_phase = ((kind != 0) ? Phase.END : Phase.PLAY);
			if (kind == Kind.ITEM_BUTTON)
			{
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnNextTutorial", null, SendMessageOptions.DontRequireReceiver);
				break;
			}
			TutorialData tutorialData = GetTutorialData(m_id);
			if (tutorialData != null)
			{
				MsgTutorialPlayAction value = new MsgTutorialPlayAction(tutorialData.m_eventID);
				GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialPlayAction", value, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case Phase.SUCCESS:
			if (m_id != Id.MISSION_8)
			{
				SoundManager.SePlay("sys_clear");
				m_successGameObject.SetActive(true);
			}
			m_phase = Phase.RESULT;
			break;
		case Phase.RETRY:
			if (m_id != Id.MISSION_8)
			{
				SoundManager.SePlay("sys_retry");
				m_retryGameObject.SetActive(true);
			}
			m_phase = Phase.RESULT;
			break;
		case Phase.END:
			if (m_anchorObj != null)
			{
				m_anchorObj.SetActive(false);
			}
			m_phase = Phase.NONE;
			m_id = Id.NONE;
			break;
		}
	}

	private void Initialize()
	{
		m_id = Id.NONE;
		m_phase = Phase.NONE;
		m_captionGameObject.SetActive(false);
		m_explanGameObject.SetActive(false);
		ExplanPattern[] explanPatterns = m_explanPatterns;
		foreach (ExplanPattern explanPattern in explanPatterns)
		{
			explanPattern.m_gameObject.SetActive(false);
			if (explanPattern.m_label != null)
			{
				explanPattern.m_label.text = null;
			}
			if (explanPattern.m_texture != null)
			{
				explanPattern.m_texture.enabled = false;
			}
		}
		m_successGameObject.SetActive(false);
		m_retryGameObject.SetActive(false);
		if (m_anchorObj != null)
		{
			m_anchorObj.SetActive(false);
		}
	}

	private void OnClickScreen()
	{
		if (m_phase != Phase.WAIT)
		{
			return;
		}
		bool flag = false;
		if (m_textureCount > 1 && m_currentTextureIndex < m_textureCount - 1)
		{
			flag = true;
		}
		if (flag)
		{
			m_currentTextureIndex++;
			switch (kind)
			{
			case Kind.MISSION:
				m_explanPattern.m_label.text = GetExplainText(m_id, m_currentTextureIndex);
				break;
			case Kind.MAPBOSS:
				m_explanPattern.m_label.text = GetExplainText(m_id, m_currentTextureIndex);
				break;
			case Kind.EVENTBOSS:
				m_explanPattern.m_label.text = GetEventBossExplainText(m_id, m_bossType, m_currentTextureIndex);
				break;
			}
			SetTexture(m_currentTextureIndex);
			m_timer = 0.5f;
			m_phase = Phase.OPEN_WAIT;
		}
		else
		{
			OnClose();
			if (kind == Kind.ITEM || kind == Kind.CHARA || kind == Kind.ACTION || kind == Kind.ITEM_BUTTON || kind == Kind.QUICK)
			{
				m_timer = 0.3f;
			}
			else
			{
				m_timer = 0f;
			}
			m_phase = Phase.CLOSE_WAIT;
		}
	}

	private void OnClose()
	{
		switch (kind)
		{
		case Kind.MAPBOSS:
		case Kind.EVENTBOSS:
		case Kind.ITEM:
		case Kind.ITEM_BUTTON:
		case Kind.CHARA:
		case Kind.ACTION:
		case Kind.QUICK:
			m_captionGameObject.SetActive(false);
			break;
		case Kind.MISSION:
			if (m_id == Id.MISSION_8)
			{
				m_captionGameObject.SetActive(false);
			}
			break;
		}
		m_explanGameObject.SetActive(false);
		m_explanPattern.m_gameObject.SetActive(false);
	}

	private void OnStartTutorial(MsgTutorialHudStart msg)
	{
		Initialize();
		if (m_phase == Phase.NONE)
		{
			m_id = msg.m_id;
			m_bossType = msg.m_bossType;
		}
	}

	private void OnSuccessTutorial()
	{
		if (m_phase == Phase.PLAY)
		{
			m_phase = Phase.SUCCESS;
		}
	}

	private void OnRetryTutorial()
	{
		if (m_phase == Phase.PLAY)
		{
			m_phase = Phase.RETRY;
		}
	}

	private void OnEndTutorial()
	{
		Initialize();
	}

	private void OnPushBackKey()
	{
		OnClickScreen();
	}

	private static void SetUIEffect(bool flag)
	{
		if (UIEffectManager.Instance != null)
		{
			UIEffectManager.Instance.SetActiveEffect(HudMenuUtility.EffectPriority.Menu, flag);
		}
	}

	public static void StartTutorial(Id id, BossType bossType)
	{
		SetUIEffect(false);
		GameObjectUtil.SendMessageFindGameObject("HudTutorial", "OnStartTutorial", new MsgTutorialHudStart(id, bossType), SendMessageOptions.DontRequireReceiver);
	}

	public static void SuccessTutorial()
	{
		GameObjectUtil.SendMessageFindGameObject("HudTutorial", "OnSuccessTutorial", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void RetryTutorial()
	{
		GameObjectUtil.SendMessageFindGameObject("HudTutorial", "OnRetryTutorial", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void EndTutorial()
	{
		SetUIEffect(true);
		GameObjectUtil.SendMessageFindGameObject("HudTutorial", "OnEndTutorial", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void PushBackKey()
	{
		GameObjectUtil.SendMessageFindGameObject("HudTutorial", "OnPushBackKey", null, SendMessageOptions.DontRequireReceiver);
	}

	public static void Load(ResourceSceneLoader loaderComponent, bool turorial, bool bossStage, BossType tutorialBossType, CharaType mainChara, CharaType subChara)
	{
		bool onAssetBundle = true;
		if (!(loaderComponent != null))
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (turorial)
		{
			flag = true;
			flag3 = true;
			loaderComponent.AddLoad(FEVERBOSS_SCENENAME, onAssetBundle, false);
		}
		else if (tutorialBossType == BossType.MAP1 || tutorialBossType == BossType.MAP2 || tutorialBossType == BossType.MAP3)
		{
			flag2 = true;
			flag4 = true;
		}
		else if (bossStage)
		{
			flag4 = true;
		}
		else
		{
			flag3 = true;
			flag4 = true;
			if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode())
			{
				for (int i = 0; i < QUICK_MODE_TUTORIAL_DATA_TBL.Length; i++)
				{
					if (IsQuickModeTutorial((Id)(54 + i)))
					{
						loaderComponent.AddLoad(QUICK_SCENENAME + (i + 1), onAssetBundle, false);
					}
				}
			}
			for (int j = 0; j < ACTION_TUTORIAL_DATA_TBL.Length; j++)
			{
				if (IsActionTutorial((Id)(52 + j)))
				{
					loaderComponent.AddLoad(ACTION_SCENENAME + (j + 1), onAssetBundle, false);
				}
			}
			if (tutorialBossType == BossType.FEVER)
			{
				loaderComponent.AddLoad(FEVERBOSS_SCENENAME, onAssetBundle, false);
			}
		}
		if (flag)
		{
			loaderComponent.AddLoad(MISSION_SCENENAME, onAssetBundle, false);
		}
		if (flag2)
		{
			int num = (int)(tutorialBossType - 1);
			loaderComponent.AddLoad(MAPBOSS_SCENENAME + num, onAssetBundle, false);
		}
		if (flag3 && IsItemTutorial())
		{
			loaderComponent.AddLoad(ITEM_SCENENAME, onAssetBundle, false);
		}
		if (!flag4)
		{
			return;
		}
		if (IsCharaTutorial(mainChara))
		{
			int charaTutorialIndex = GetCharaTutorialIndex(mainChara);
			if (charaTutorialIndex >= 0)
			{
				loaderComponent.AddLoad(CHARA_SCENENAME + charaTutorialIndex, onAssetBundle, false);
			}
		}
		if (IsCharaTutorial(subChara))
		{
			int charaTutorialIndex2 = GetCharaTutorialIndex(subChara);
			if (charaTutorialIndex2 >= 0)
			{
				loaderComponent.AddLoad(CHARA_SCENENAME + charaTutorialIndex2, onAssetBundle, false);
			}
		}
	}

	public static TutorialData GetTutorialData(Id id)
	{
		if ((uint)id < TUTORIAL_DATA_TBL.Length)
		{
			return TUTORIAL_DATA_TBL[(int)id];
		}
		return null;
	}

	public static string GetLoadSceneName(BossType bossType)
	{
		if (bossType != BossType.NONE)
		{
			int num = (int)(bossType - 1);
			return MAPBOSS_SCENENAME + num;
		}
		return string.Empty;
	}

	public static string GetLoadSceneName(CharaType charaType)
	{
		if (charaType != CharaType.UNKNOWN)
		{
			int charaTutorialIndex = GetCharaTutorialIndex(charaType);
			return CHARA_SCENENAME + charaTutorialIndex;
		}
		return string.Empty;
	}

	public static string GetLoadQuickModeSceneName(Id quickID)
	{
		int num = (int)(quickID - 54);
		if (0 <= num && num < QUICK_MODE_TUTORIAL_DATA_TBL.Length)
		{
			return QUICK_SCENENAME + (num + 1);
		}
		return string.Empty;
	}

	public static int GetTexuterPageCount(Id id)
	{
		TutorialData tutorialData = GetTutorialData(id);
		if (tutorialData != null)
		{
			return tutorialData.m_textureCount;
		}
		return 0;
	}

	public static string GetCaptionText(Id id, int page = 0)
	{
		string result = string.Empty;
		switch (GetKind(id))
		{
		case Kind.MISSION:
			result = TextUtility.GetCommonText("Tutorial", "caption" + (int)(id + 1));
			break;
		case Kind.FEVERBOSS:
		{
			TextObject text7 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", "caption_explan2_2");
			string commonText2 = TextUtility.GetCommonText("BossName", "ferver");
			text7.ReplaceTag("{PARAM_NAME}", commonText2);
			result = text7.text;
			break;
		}
		case Kind.MAPBOSS:
		{
			BossType type = BossType.MAP1;
			switch (id)
			{
			case Id.MAPBOSS_1:
				type = BossType.MAP1;
				break;
			case Id.MAPBOSS_2:
				type = BossType.MAP2;
				break;
			case Id.MAPBOSS_3:
				type = BossType.MAP3;
				break;
			}
			TextObject text6 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", "caption_boss");
			text6.ReplaceTag("{PARAM_NAME}", BossTypeUtil.GetTextCommonBossName(type));
			result = text6.text;
			break;
		}
		case Kind.ITEM:
		{
			string itemTutorialCaptionText = GetItemTutorialCaptionText(id);
			TextObject text5 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", itemTutorialCaptionText);
			text5.ReplaceTag("{PARAM_NAME}", GetItemTutorialText(id));
			result = text5.text;
			break;
		}
		case Kind.QUICK:
		{
			string commonText = TextUtility.GetCommonText("Tutorial", "caption_quickmode_tutorial");
			result = TextUtility.GetCommonText("Tutorial", "caption_explan2", "{PARAM_NAME}", commonText);
			break;
		}
		case Kind.ITEM_BUTTON:
		{
			TextObject text4 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", "caption_explan3");
			result = text4.text;
			break;
		}
		case Kind.CHARA:
		{
			CharaType commonTextCharaName = GetCommonTextCharaName(id);
			string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", CharaName.Name[(int)commonTextCharaName]).text;
			TextObject text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", "caption_explan1");
			text3.ReplaceTag("{PARAM_NAME}", text2);
			result = text3.text;
			break;
		}
		case Kind.ACTION:
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", "caption_explan2_2");
			text.ReplaceTag("{PARAM_NAME}", GetActionTutorialText(id));
			result = text.text;
			break;
		}
		}
		return result;
	}

	public static string GetEventBossCaptionText(Id id, BossType bossType, int page = 0)
	{
		string result = string.Empty;
		Kind kind = GetKind(id);
		if (kind == Kind.EVENTBOSS)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tutorial", "caption_boss");
			text.ReplaceTag("{PARAM_NAME}", BossTypeUtil.GetTextCommonBossCharaName(bossType));
			result = text.text;
		}
		return result;
	}

	public static string GetExplainText(Id id, int page = 0)
	{
		string result = string.Empty;
		switch (GetKind(id))
		{
		case Kind.MISSION:
		{
			if (page == 0)
			{
				result = TextUtility.GetCommonText("Tutorial", "explan" + (int)(id + 1));
				break;
			}
			string arg2 = "_" + page;
			result = TextUtility.GetCommonText("Tutorial", "explan" + (int)(id + 1) + arg2);
			break;
		}
		case Kind.MISSION_END:
			result = TextUtility.GetCommonText("Tutorial", "end");
			break;
		case Kind.FEVERBOSS:
			result = TextUtility.GetCommonText("Tutorial", "fever_boss");
			break;
		case Kind.MAPBOSS:
		{
			if (page == 0)
			{
				result = TextUtility.GetCommonText("Tutorial", "boss" + (int)(id - 10 + 1));
				break;
			}
			string arg = "_" + page;
			result = TextUtility.GetCommonText("Tutorial", "boss" + (int)(id - 10 + 1) + arg);
			break;
		}
		case Kind.ITEM:
			result = TextUtility.GetCommonText("Tutorial", "item_" + (int)(id - 15 + 1));
			break;
		case Kind.QUICK:
			result = TextUtility.GetCommonText("Tutorial", "quick" + (int)(id - 54 + 1));
			break;
		case Kind.CHARA:
			result = TextUtility.GetCommonText("Tutorial", "chara" + (int)(id - 24 + 1));
			break;
		case Kind.ACTION:
			result = TextUtility.GetCommonText("Tutorial", "action" + (int)(id - 52 + 1));
			break;
		case Kind.ITEM_BUTTON:
			result = TextUtility.GetCommonText("Tutorial", "item_btn");
			break;
		}
		return result;
	}

	public static string GetEventBossExplainText(Id id, BossType bossType, int page = 0)
	{
		string result = string.Empty;
		Kind kind = GetKind(id);
		if (kind == Kind.EVENTBOSS)
		{
			if (page == 0)
			{
				result = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_SPECIFIC, "Tutorial", "boss_" + (int)(id - 13 + 1)).text;
			}
			else
			{
				string text = "_" + page;
				result = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_SPECIFIC, "Tutorial", "boss_" + (int)(id - 13 + 1) + "_" + text).text;
			}
		}
		return result;
	}

	private static bool IsItemTutorial()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				for (int i = 0; i < 8; i++)
				{
					SystemData.ItemTutorialFlagStatus itemTutorialStatus = ItemTypeName.GetItemTutorialStatus((ItemType)i);
					if (itemTutorialStatus != SystemData.ItemTutorialFlagStatus.NONE && !systemdata.IsFlagStatus(itemTutorialStatus))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool IsItemTutorial(ItemType type)
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SystemData.ItemTutorialFlagStatus itemTutorialStatus = ItemTypeName.GetItemTutorialStatus(type);
				if (itemTutorialStatus != SystemData.ItemTutorialFlagStatus.NONE && !systemdata.IsFlagStatus(itemTutorialStatus))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool IsCharaTutorial(CharaType type)
	{
		if (type == CharaType.SONIC)
		{
			ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
			if (mileageMapState != null && mileageMapState.m_episode > 1)
			{
				return false;
			}
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SystemData.CharaTutorialFlagStatus characterSaveDataFlagStatus = CharaTypeUtil.GetCharacterSaveDataFlagStatus(type);
				if (characterSaveDataFlagStatus != SystemData.CharaTutorialFlagStatus.NONE && !systemdata.IsFlagStatus(characterSaveDataFlagStatus))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static int GetCharaTutorialIndex(CharaType type)
	{
		Id characterTutorialID = CharaTypeUtil.GetCharacterTutorialID(type);
		if (characterTutorialID != Id.NONE)
		{
			return (int)(characterTutorialID - 23);
		}
		return -1;
	}

	public static CharaType GetCommonTextCharaName(Id in_id)
	{
		for (int i = 0; i < 29; i++)
		{
			Id characterTutorialID = CharaTypeUtil.GetCharacterTutorialID((CharaType)i);
			if (characterTutorialID == in_id)
			{
				return (CharaType)i;
			}
		}
		return CharaType.UNKNOWN;
	}

	private static bool IsActionTutorial(Id actionID)
	{
		if (GetKind(actionID) == Kind.ACTION)
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					SystemData.ActionTutorialFlagStatus actionTutorialSaveFlag = GetActionTutorialSaveFlag(actionID);
					if (actionTutorialSaveFlag != SystemData.ActionTutorialFlagStatus.NONE && !systemdata.IsFlagStatus(actionTutorialSaveFlag))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool IsQuickModeTutorial(Id actionID)
	{
		if (GetKind(actionID) == Kind.QUICK)
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					SystemData.QuickModeTutorialFlagStatus quickModeTutorialSaveFlag = GetQuickModeTutorialSaveFlag(actionID);
					if (quickModeTutorialSaveFlag != SystemData.QuickModeTutorialFlagStatus.NONE && !systemdata.IsFlagStatus(quickModeTutorialSaveFlag))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static void SendItemTutorial(ItemType itemType)
	{
		if (IsItemTutorial(itemType))
		{
			Id itemTutorialID = ItemTypeName.GetItemTutorialID(itemType);
			if (itemTutorialID != Id.NONE)
			{
				GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialItem", new MsgTutorialItem(itemTutorialID), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static void SendActionTutorial(Id actionID)
	{
		if (GetKind(actionID) == Kind.ACTION && IsActionTutorial(actionID))
		{
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialAction", new MsgTutorialAction(actionID), SendMessageOptions.DontRequireReceiver);
		}
	}

	public static SystemData.ActionTutorialFlagStatus GetActionTutorialSaveFlag(Id actionID)
	{
		if (GetKind(actionID) == Kind.ACTION)
		{
			int num = (int)(actionID - 52);
			if ((uint)num < ACTION_TUTORIAL_DATA_TBL.Length)
			{
				return ACTION_TUTORIAL_DATA_TBL[num].m_flagStatus;
			}
		}
		return SystemData.ActionTutorialFlagStatus.NONE;
	}

	public static SystemData.QuickModeTutorialFlagStatus GetQuickModeTutorialSaveFlag(Id quickID)
	{
		if (GetKind(quickID) == Kind.QUICK)
		{
			int num = (int)(quickID - 54);
			if ((uint)num < QUICK_MODE_TUTORIAL_DATA_TBL.Length)
			{
				return QUICK_MODE_TUTORIAL_DATA_TBL[num].m_flagStatus;
			}
		}
		return SystemData.QuickModeTutorialFlagStatus.NONE;
	}

	public static string GetActionTutorialText(Id actionID)
	{
		if (GetKind(actionID) == Kind.ACTION)
		{
			int num = (int)(actionID - 52);
			if ((uint)num < ACTION_TUTORIAL_DATA_TBL.Length)
			{
				return TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, ACTION_TUTORIAL_DATA_TBL[num].m_textCategory, ACTION_TUTORIAL_DATA_TBL[num].m_textCell).text;
			}
		}
		return string.Empty;
	}

	public static string GetItemTutorialText(Id itemID)
	{
		if (GetKind(itemID) == Kind.ITEM)
		{
			string text = "name" + (int)(itemID - 15 + 1);
			if (itemID == Id.ITEM_6)
			{
				text += "_2";
			}
			return TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", text).text;
		}
		return string.Empty;
	}

	public static string GetItemTutorialCaptionText(Id itemID)
	{
		if (GetKind(itemID) == Kind.ITEM)
		{
			if (itemID == Id.ITEM_6)
			{
				return "caption_explan2_2";
			}
			return "caption_explan2";
		}
		return string.Empty;
	}
}
