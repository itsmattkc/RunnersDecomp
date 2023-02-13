using AnimationOrTween;
using DataTable;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class EventRewardWindow : EventWindowBase
{
	private enum BUTTON_ACT
	{
		CLOSE,
		ROULETTE,
		NONE
	}

	private enum Mode
	{
		Idle,
		Wait,
		End
	}

	private const int DRAW_LINE_MAX = 4;

	private const int DRAW_LINE_POS = 3;

	private const float GET_ICON_SOUND_DELAY = 0.25f;

	private const string LBL_CAPTION = "Lbl_caption";

	private const string LBL_LEFT_TITLE = "Lbl_word_left_title";

	private const string LBL_LEFT_TITLE_SH = "Lbl_word_left_title_sh";

	private const string LBL_LEFT_NAME = "Lbl_chao_name";

	private const string LBL_RIGHT_TITLE = "ui_Lbl_word_object_total_main";

	private const string LBL_RIGHT_NUM = "Lbl_object_total_main_num";

	private const string IMG_LEFT_BG = "img_chao_bg";

	private const string IMG_RIGHT_ICON = "img_object_total_main";

	private const string IMG_TYPE_ICON = "img_type_icon";

	private const string TEX_LEFT_TEXTURE = "texture_chao";

	private const string GO_LIST = "list";

	[SerializeField]
	private GameObject orgEventScroll;

	[SerializeField]
	private UIPanel mainPanel;

	private List<GameObject> m_listObject;

	private Mode m_mode;

	private int m_targetChao = -1;

	[SerializeField]
	private Animation animationData;

	private bool m_close;

	private BUTTON_ACT m_btnAct = BUTTON_ACT.NONE;

	private bool m_first = true;

	private float m_afterTime;

	private float m_rewardTime;

	private int m_attainment;

	private int m_currentAttainment;

	private bool m_isGetEffectEnd;

	private float m_initPoint;

	private List<float> m_getSoundDelay;

	private static EventRewardWindow s_instance;

	private static EventRewardWindow Instance
	{
		get
		{
			return s_instance;
		}
	}

	protected override void SetObject()
	{
		GeneralUtil.SetRouletteBtnIcon(base.gameObject);
		if (!m_isSetObject)
		{
			base.gameObject.transform.localPosition = default(Vector3);
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			List<string> list4 = new List<string>();
			list.Add("Lbl_caption");
			list.Add("Lbl_word_left_title");
			list.Add("Lbl_word_left_title_sh");
			list.Add("Lbl_chao_name");
			list.Add("ui_Lbl_word_object_total_main");
			list.Add("Lbl_object_total_main_num");
			list2.Add("img_chao_bg");
			list2.Add("img_object_total_main");
			list2.Add("img_type_icon");
			list3.Add("texture_chao");
			list4.Add("list");
			m_objectLabels = ObjectUtility.GetObjectLabel(base.gameObject, list);
			m_objectSprites = ObjectUtility.GetObjectSprite(base.gameObject, list2);
			m_objectTextures = ObjectUtility.GetObjectTexture(base.gameObject, list3);
			m_objects = ObjectUtility.GetGameObject(base.gameObject, list4);
			UIPlayAnimation uIPlayAnimation = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "blinder");
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "blinder");
			if (uIPlayAnimation != null && uIButtonMessage != null)
			{
				uIPlayAnimation.enabled = false;
				uIButtonMessage.enabled = false;
			}
			m_isSetObject = true;
		}
	}

	private void Setup(SpecialStageInfo info)
	{
		mainPanel.alpha = 1f;
		if (info != null && info.GetRewardChao() != null)
		{
			m_targetChao = info.GetRewardChao().id;
		}
		else
		{
			m_targetChao = -1;
		}
		if (info != null)
		{
			SetupObject(info);
		}
		m_mode = Mode.Wait;
	}

	private void Setup(RaidBossInfo info)
	{
		mainPanel.alpha = 1f;
		if (info != null && info.GetRewardChao() != null)
		{
			m_targetChao = info.GetRewardChao().id;
		}
		else
		{
			m_targetChao = -1;
		}
		if (info != null)
		{
			SetupObject(info);
		}
		m_mode = Mode.Wait;
	}

	private void Setup(EtcEventInfo info)
	{
		mainPanel.alpha = 1f;
		if (info != null && info.GetRewardChao() != null)
		{
			m_targetChao = info.GetRewardChao().id;
		}
		else
		{
			m_targetChao = -1;
		}
		if (info != null)
		{
			SetupObject(info);
		}
		m_mode = Mode.Wait;
	}

	private void SetupObject(EventBaseInfo info)
	{
		m_isGetEffectEnd = true;
		m_afterTime = 0.3f;
		SetObject();
		if (m_getSoundDelay != null)
		{
			m_getSoundDelay.Clear();
		}
		m_mode = Mode.Idle;
		m_close = false;
		m_btnAct = BUTTON_ACT.NONE;
		base.enabledAnchorObjects = true;
		if (animationData != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animationData, "ui_event_rewordlist_window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_open");
		}
		UIDraggablePanel uIDraggablePanel = null;
		if (m_objects != null && m_objects.ContainsKey("list"))
		{
			uIDraggablePanel = m_objects["list"].GetComponent<UIDraggablePanel>();
		}
		if (info != null)
		{
			if (m_objectLabels != null && m_objectLabels.Count > 0)
			{
				if (m_objectLabels.ContainsKey("Lbl_caption") && m_objectLabels["Lbl_caption"] != null)
				{
					m_objectLabels["Lbl_caption"].text = info.caption;
				}
				if (m_objectLabels.ContainsKey("Lbl_word_left_title") && m_objectLabels["Lbl_word_left_title"] != null)
				{
					m_objectLabels["Lbl_word_left_title"].text = info.leftTitle;
				}
				if (m_objectLabels.ContainsKey("Lbl_word_left_title_sh") && m_objectLabels["Lbl_word_left_title_sh"] != null)
				{
					m_objectLabels["Lbl_word_left_title_sh"].text = info.leftTitle;
				}
				if (m_objectLabels.ContainsKey("Lbl_chao_name") && m_objectLabels["Lbl_chao_name"] != null)
				{
					m_objectLabels["Lbl_chao_name"].text = info.leftName;
				}
				if (m_objectLabels.ContainsKey("ui_Lbl_word_object_total_main") && m_objectLabels["ui_Lbl_word_object_total_main"] != null)
				{
					m_objectLabels["ui_Lbl_word_object_total_main"].text = info.rightTitle;
				}
				if (m_objectLabels.ContainsKey("Lbl_object_total_main_num") && m_objectLabels["Lbl_object_total_main_num"] != null)
				{
					m_objectLabels["Lbl_object_total_main_num"].text = HudUtility.GetFormatNumString(info.totalPoint);
				}
			}
			if (m_objectSprites != null && m_objectSprites.Count > 0)
			{
				if (m_objectSprites.ContainsKey("img_chao_bg"))
				{
					m_objectSprites["img_chao_bg"].spriteName = info.leftBg;
				}
				if (m_objectSprites.ContainsKey("img_object_total_main"))
				{
					m_objectSprites["img_object_total_main"].spriteName = info.rightTitleIcon;
				}
				if (m_objectSprites.ContainsKey("img_type_icon") && !string.IsNullOrEmpty(info.chaoTypeIcon))
				{
					m_objectSprites["img_type_icon"].spriteName = info.chaoTypeIcon;
				}
			}
			if (m_objectTextures != null && m_objectTextures.ContainsKey("texture_chao"))
			{
				ChaoData rewardChao = info.GetRewardChao();
				ChaoTextureManager.CallbackInfo info2 = new ChaoTextureManager.CallbackInfo(m_objectTextures["texture_chao"], null, true);
				ChaoTextureManager.Instance.GetTexture(rewardChao.id, info2);
			}
			if (uIDraggablePanel != null)
			{
				List<EventMission> eventMission = info.eventMission;
				int num = 0;
				if (eventMission != null && eventMission.Count > 0)
				{
					int count = eventMission.Count;
					num = count;
					if (m_listObject == null)
					{
						m_rewardTime = 0f;
						m_attainment = 0;
						m_currentAttainment = 0;
						if (m_first)
						{
							m_isGetEffectEnd = false;
							m_afterTime = 0f;
						}
						m_listObject = new List<GameObject>();
						for (int i = 0; i < count; i++)
						{
							GameObject gameObject = Object.Instantiate(orgEventScroll, Vector3.zero, Quaternion.identity) as GameObject;
							gameObject.gameObject.transform.parent = uIDraggablePanel.gameObject.transform;
							gameObject.gameObject.transform.localPosition = new Vector3(0f, -100 * i, 0f);
							gameObject.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
							UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject.gameObject, "Lbl_itemname");
							UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject.gameObject, "Lbl_itemname_sh");
							UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject.gameObject, "Lbl_word_event_object_total");
							UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject.gameObject, "Lbl_event_object_total_num");
							GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject.gameObject, "get_icon_Anim");
							UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject.gameObject, "img_item_0");
							UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject.gameObject, "texture_chao_0");
							UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject.gameObject, "texture_chao_1");
							UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject.gameObject, "img_event_object_icon");
							if (uILabel3 != null && uILabel4 != null)
							{
								uILabel3.text = eventMission[i].name;
								uILabel4.text = HudUtility.GetFormatNumString(eventMission[i].point);
							}
							if (gameObject2 != null)
							{
								gameObject2.SetActive(false);
								if (eventMission[i].IsAttainment(info.totalPoint))
								{
									m_attainment = i + 1;
								}
							}
							if (uISprite3 != null)
							{
								uISprite3.spriteName = info.rightTitleIcon;
							}
							if (uILabel != null && uILabel2 != null && uISprite != null && uISprite2 != null && uITexture != null)
							{
								ServerItem serverItem = new ServerItem((ServerItem.Id)eventMission[i].reward);
								string text = serverItem.serverItemName;
								int index = eventMission[i].index;
								if (index > 1)
								{
									text = text + " × " + index;
								}
								uILabel.text = text;
								uILabel2.text = text;
								if (eventMission[i].reward >= 400000 && eventMission[i].reward < 500000)
								{
									uISprite2.alpha = 0f;
									uISprite.alpha = 0f;
									uITexture.alpha = 0f;
									uITexture.gameObject.SetActive(true);
									ChaoTextureManager.CallbackInfo info3 = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
									ChaoTextureManager.Instance.GetTexture(eventMission[i].reward - 400000, info3);
									uITexture.alpha = 1f;
									ChaoData chaoData = ChaoTable.GetChaoData(serverItem.chaoId);
									if (chaoData != null)
									{
										uILabel.text = chaoData.name;
										uILabel2.text = chaoData.name;
									}
								}
								else
								{
									uISprite.spriteName = PresentBoxUtility.GetItemSpriteName(serverItem);
									uISprite.alpha = 1f;
									uISprite2.alpha = 0f;
									uITexture.alpha = 0f;
									uITexture.gameObject.SetActive(false);
								}
							}
							m_listObject.Add(gameObject);
						}
						uIDraggablePanel.ResetPosition();
						if (m_attainment > 0)
						{
							for (int j = 0; j < m_attainment; j++)
							{
								Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_listObject[j], "get_icon_Anim");
								if (animation != null)
								{
									if (j < m_attainment - 5 || !m_first)
									{
										m_currentAttainment = j + 1;
										animation.enabled = false;
										animation.gameObject.SetActive(true);
									}
									else
									{
										animation.enabled = true;
									}
								}
							}
							m_initPoint = 0f;
							if (m_attainment > 3)
							{
								m_initPoint = 1f / (float)(num - 4) * (float)(m_attainment - 3);
							}
							StartCoroutine(ScrollInit(m_initPoint, uIDraggablePanel));
						}
					}
					else
					{
						uIDraggablePanel.ResetPosition();
						m_currentAttainment = m_attainment;
						m_isGetEffectEnd = true;
						m_afterTime = 0.3f;
						StartCoroutine(ScrollInit(m_initPoint, uIDraggablePanel));
					}
				}
			}
		}
		if (m_attainment <= 0 || m_currentAttainment >= m_attainment)
		{
			m_isGetEffectEnd = true;
			m_afterTime = 0.3f;
		}
		else
		{
			m_rewardTime = 0.75f;
		}
		base.enabled = true;
		m_first = false;
	}

	private void Update()
	{
		if (!m_isGetEffectEnd)
		{
			if (m_rewardTime <= 0f)
			{
				if (m_listObject != null && m_currentAttainment >= 0 && m_currentAttainment < m_listObject.Count)
				{
					GameObject gameObject = GameObjectUtil.FindChildGameObject(m_listObject[m_currentAttainment].gameObject, "get_icon_Anim");
					if (gameObject != null)
					{
						if (!gameObject.activeSelf)
						{
							if (m_getSoundDelay == null)
							{
								m_getSoundDelay = new List<float>();
							}
							m_getSoundDelay.Add(0.25f);
						}
						gameObject.SetActive(true);
					}
					m_currentAttainment++;
					m_rewardTime = 0.7f;
					if (m_attainment <= m_currentAttainment)
					{
						m_isGetEffectEnd = true;
						m_rewardTime = 0f;
					}
				}
				else
				{
					m_isGetEffectEnd = true;
					m_rewardTime = 0f;
				}
			}
			m_rewardTime -= Time.deltaTime;
		}
		else
		{
			m_afterTime += Time.deltaTime;
			m_rewardTime = 0f;
			if (m_afterTime > 3f)
			{
				base.enabled = false;
			}
		}
		if (m_getSoundDelay == null || m_getSoundDelay.Count <= 0)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = -1;
		for (int i = 0; i < m_getSoundDelay.Count; i++)
		{
			List<float> getSoundDelay;
			List<float> list = getSoundDelay = m_getSoundDelay;
			int index;
			int index2 = index = i;
			float num2 = getSoundDelay[index];
			list[index2] = num2 - deltaTime;
			if (m_getSoundDelay[i] <= 0f)
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			SoundManager.SePlay("sys_result_decide");
			m_getSoundDelay.RemoveAt(num);
		}
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		if (data != null && data.tex != null && m_objectTextures != null && m_objectTextures.ContainsKey("texture_chao"))
		{
			m_objectTextures["texture_chao"].mainTexture = data.tex;
			m_objectTextures["texture_chao"].alpha = 1f;
		}
	}

	private IEnumerator ScrollInit(float point, UIDraggablePanel list)
	{
		yield return new WaitForSeconds(0.025f);
		list.verticalScrollBar.value = point;
	}

	public bool IsEnd()
	{
		if (m_mode == Mode.Wait)
		{
			return false;
		}
		return true;
	}

	public void OnClickNoButton()
	{
		m_btnAct = BUTTON_ACT.CLOSE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		if (animationData != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animationData, "ui_event_rewordlist_window_Anim", Direction.Reverse);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	public void OnClickNoBgButton()
	{
		OnClickNoButton();
	}

	public void OnClickTarget()
	{
		if (m_targetChao < 0)
		{
			return;
		}
		SoundManager.SePlay("sys_menu_decide");
		ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
		if (!(window != null))
		{
			return;
		}
		ChaoData chaoData = ChaoTable.GetChaoData(m_targetChao);
		if (chaoData != null)
		{
			ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(chaoData);
			chaoInfo.level = ChaoTable.ChaoMaxLevel();
			chaoInfo.detail = chaoData.GetDetailsLevel(chaoInfo.level);
			if (chaoInfo.level == ChaoTable.ChaoMaxLevel())
			{
				chaoInfo.detail = chaoInfo.detail + "\n" + TextUtility.GetChaoText("Chao", "level_max");
			}
			window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
		}
	}

	public void OnClickRouletteButton()
	{
		m_btnAct = BUTTON_ACT.ROULETTE;
		m_close = true;
		SoundManager.SePlay("sys_menu_decide");
		HudMenuUtility.SendChaoRouletteButtonClicked();
		if (animationData != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animationData, "ui_event_rewordlist_window_Anim", Direction.Reverse);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			m_mode = Mode.End;
			if (m_getSoundDelay != null)
			{
				m_getSoundDelay.Clear();
			}
			switch (m_btnAct)
			{
			case BUTTON_ACT.ROULETTE:
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.CLOSE:
				base.enabledAnchorObjects = false;
				break;
			default:
				base.enabledAnchorObjects = false;
				break;
			}
		}
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (base.enabledAnchorObjects && m_btnAct == BUTTON_ACT.NONE)
		{
			if (msg != null)
			{
				msg.StaySequence();
			}
			if (m_isGetEffectEnd && m_afterTime > 0.5f)
			{
				SendMessage("OnClickNoButton");
			}
		}
	}

	public static EventRewardWindow Create(SpecialStageInfo info)
	{
		if (s_instance != null)
		{
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(info);
			return s_instance;
		}
		return null;
	}

	public static EventRewardWindow Create(RaidBossInfo info)
	{
		if (s_instance != null)
		{
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(info);
			return s_instance;
		}
		return null;
	}

	public static EventRewardWindow Create(EtcEventInfo info)
	{
		if (s_instance != null)
		{
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(info);
			return s_instance;
		}
		return null;
	}

	public void EntryBackKeyCallBack()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	public void RemoveBackKeyCallBack()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	private void Awake()
	{
		SetInstance();
		EntryBackKeyCallBack();
		base.enabledAnchorObjects = false;
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			RemoveBackKeyCallBack();
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
