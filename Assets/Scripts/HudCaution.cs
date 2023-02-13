using AnimationOrTween;
using Message;
using System;
using System.Diagnostics;
using UnityEngine;

public class HudCaution : MonoBehaviour
{
	public enum Type
	{
		GO,
		SPEEDUP,
		BOSS,
		COMBO_N,
		TRICK0,
		TRICK1,
		TRICK2,
		TRICK3,
		TRICK4,
		BONUS_N,
		COMBO_BONUS_N,
		TRICK_BONUS_N,
		COUNTDOWN,
		MAP_BOSS_CLEAR,
		MAP_BOSS_FAILED,
		STAGE_OUT,
		STAGE_IN,
		ZERO_POINT_TEST,
		GET_ITEM,
		DESTROY_ENEMY,
		NO_RING,
		WISPBOOST,
		EVENTBOSS,
		EXTREMEMODE,
		COMBOITEM_BONUS_N,
		GET_TIMER,
		COUNT
	}

	[Serializable]
	private class AnimInfo
	{
		[SerializeField]
		public Animation m_animation;

		[SerializeField]
		public string m_clipName;

		[SerializeField]
		public UILabel m_label;

		[SerializeField]
		public string m_labelStringFormat;

		[SerializeField]
		public UISlider m_slider;

		[SerializeField]
		public UISprite m_sprite;

		[SerializeField]
		public UISprite m_sprite2;

		[SerializeField]
		public bool m_finishDisable;
	}

	private class DelayWorldToScreenPoint
	{
		private Vector3 m_beforeTargetPosition;

		public Vector2 GetScreenPositon(GameObject targetGameObject, Camera targetCamera, Camera uiCamera)
		{
			Vector2 result = new Vector2(-100f, -100f);
			if (targetGameObject != null && targetCamera != null && uiCamera != null)
			{
				Vector3 beforeTargetPosition = m_beforeTargetPosition;
				m_beforeTargetPosition = targetGameObject.transform.localPosition;
				Vector3 position = targetCamera.WorldToScreenPoint(beforeTargetPosition);
				position.z = 0f;
				return uiCamera.ScreenToWorldPoint(position);
			}
			return result;
		}
	}

	private static HudCaution instance;

	[SerializeField]
	private GameObject m_playerAnchorGameObject;

	[SerializeField]
	private GameObject m_enemyAnchorGameObject;

	[SerializeField]
	private AnimInfo[] m_animInfos = new AnimInfo[26];

	[SerializeField]
	private UISprite m_bossAttenion;

	[SerializeField]
	private UISprite m_raidBossAttenion;

	private Camera m_gameMainCamera;

	private StageItemManager m_stageItemManager;

	private ObjBossEventBossParameter m_bossParameter;

	private DelayWorldToScreenPoint m_playerDelayWorldToScreenPoint;

	private GameObject m_enemyGameObject;

	private Camera m_uiCamera;

	private UISlider m_slider;

	private UISlider m_boostSlider;

	public static HudCaution Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		SetInstance();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		m_playerDelayWorldToScreenPoint = new DelayWorldToScreenPoint();
		m_uiCamera = null;
		m_slider = null;
		m_boostSlider = null;
		if (m_enemyAnchorGameObject != null)
		{
			m_enemyAnchorGameObject.SetActive(false);
		}
		if (EventManager.Instance != null && !EventManager.Instance.IsRaidBossStage() && m_playerAnchorGameObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_playerAnchorGameObject, "gp_bit_WispBoost");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (m_stageItemManager == null)
		{
			m_stageItemManager = StageItemManager.Instance;
		}
		if (m_uiCamera == null)
		{
			m_uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		if (m_gameMainCamera == null)
		{
			m_gameMainCamera = GameObjectUtil.FindGameObjectComponent<Camera>("GameMainCamera");
		}
		Vector2 screenPositon = m_playerDelayWorldToScreenPoint.GetScreenPositon(GameObject.FindWithTag("Player"), m_gameMainCamera, m_uiCamera);
		m_playerAnchorGameObject.transform.position = new Vector3(screenPositon.x, screenPositon.y, 0f);
		if (m_slider != null && m_stageItemManager != null)
		{
			float cautionItemTimeRate = m_stageItemManager.CautionItemTimeRate;
			if (cautionItemTimeRate > 0f)
			{
				m_slider.value = cautionItemTimeRate;
			}
			else
			{
				m_slider.value = 0f;
				m_slider = null;
				ActiveAnimation.Play(m_animInfos[12].m_animation, m_animInfos[12].m_clipName, Direction.Reverse);
			}
		}
		if (m_boostSlider != null && m_bossParameter != null)
		{
			float boostRatio = m_bossParameter.BoostRatio;
			if (boostRatio > 0f)
			{
				m_boostSlider.value = boostRatio;
				return;
			}
			m_boostSlider.value = 0f;
			m_boostSlider = null;
			ActiveAnimation.Play(m_animInfos[21].m_animation, m_animInfos[21].m_clipName, Direction.Reverse);
		}
	}

	public void SetBossWord(bool bossStage)
	{
		string spriteName = (!bossStage) ? "ui_gp_bit_word_chancetime" : "ui_gp_bit_word_attention";
		if (m_bossAttenion != null)
		{
			m_bossAttenion.spriteName = spriteName;
		}
		if (m_raidBossAttenion != null)
		{
			m_raidBossAttenion.spriteName = spriteName;
		}
	}

	public void SetCaution(MsgCaution msg)
	{
		if (msg.m_cautionType >= Type.COUNT)
		{
			return;
		}
		AnimInfo animInfo = m_animInfos[(int)msg.m_cautionType];
		if (!(animInfo.m_animation != null) || string.IsNullOrEmpty(animInfo.m_clipName))
		{
			return;
		}
		if (animInfo.m_label != null)
		{
			switch (msg.m_cautionType)
			{
			case Type.BONUS_N:
			case Type.COMBOITEM_BONUS_N:
				if (!string.IsNullOrEmpty(animInfo.m_labelStringFormat))
				{
					string text3 = string.Format(animInfo.m_labelStringFormat, msg.m_number);
					if (msg.m_flag)
					{
						animInfo.m_label.text = "[FF0000]" + text3;
					}
					else
					{
						animInfo.m_label.text = text3;
					}
				}
				break;
			case Type.GET_ITEM:
				if (animInfo.m_label.enabled)
				{
					animInfo.m_label.enabled = false;
				}
				break;
			case Type.GET_TIMER:
			{
				string text2 = string.Format(animInfo.m_labelStringFormat, msg.m_second);
				if (!animInfo.m_label.enabled)
				{
					animInfo.m_label.enabled = true;
				}
				animInfo.m_label.text = text2;
				break;
			}
			default:
				if (!string.IsNullOrEmpty(animInfo.m_labelStringFormat))
				{
					string text = string.Format(animInfo.m_labelStringFormat, msg.m_number);
					animInfo.m_label.text = text;
				}
				break;
			}
		}
		float num = 0f;
		switch (msg.m_cautionType)
		{
		case Type.COUNTDOWN:
			if (animInfo.m_slider != null)
			{
				num = msg.m_rate;
				if (num > 0f)
				{
					m_slider = animInfo.m_slider;
				}
				if (m_slider != null)
				{
					m_slider.value = num;
				}
			}
			break;
		case Type.WISPBOOST:
			if (!(animInfo.m_slider != null))
			{
				break;
			}
			m_bossParameter = msg.m_bossParam;
			if (!(m_bossParameter != null))
			{
				break;
			}
			if (animInfo.m_sprite != null && m_bossParameter.BoostLevel != WispBoostLevel.NONE)
			{
				animInfo.m_sprite.spriteName = "ui_event_gp_gauge_power_bg_" + (int)m_bossParameter.BoostLevel;
			}
			if (animInfo.m_sprite2 != null)
			{
				GameObject parent = GameObjectUtil.FindChildGameObject(base.gameObject, "gp_bit_WispBoost");
				RaidBossBoostGagueColor raidBossBoostGagueColor = GameObjectUtil.FindChildGameObjectComponent<RaidBossBoostGagueColor>(parent, "img_gauge");
				Color color = animInfo.m_sprite2.color;
				switch (m_bossParameter.BoostLevel)
				{
				case WispBoostLevel.LEVEL1:
					color = raidBossBoostGagueColor.Level1;
					break;
				case WispBoostLevel.LEVEL2:
					color = raidBossBoostGagueColor.Level2;
					break;
				case WispBoostLevel.LEVEL3:
					color = raidBossBoostGagueColor.Level3;
					break;
				}
				animInfo.m_sprite2.color = color;
			}
			num = m_bossParameter.BoostRatio;
			if (num > 0f)
			{
				m_boostSlider = animInfo.m_slider;
			}
			if (m_boostSlider != null)
			{
				m_boostSlider.value = num;
			}
			break;
		}
		if (!(animInfo.m_slider != null) || num != 0f)
		{
			switch (msg.m_cautionType)
			{
			case Type.NO_RING:
				animInfo.m_animation.playAutomatically = true;
				animInfo.m_animation.cullingType = AnimationCullingType.AlwaysAnimate;
				animInfo.m_animation.Rewind();
				animInfo.m_animation.Sample();
				animInfo.m_animation.Play();
				break;
			case Type.COMBO_N:
			{
				SetAnimPlay(animInfo);
				float length2 = animInfo.m_animation[animInfo.m_clipName].length;
				if (msg.m_flag)
				{
					animInfo.m_animation[animInfo.m_clipName].time = length2 * 0.05f;
					animInfo.m_animation.Sample();
					animInfo.m_animation.Stop();
				}
				else
				{
					animInfo.m_animation[animInfo.m_clipName].time = length2 * 0.01f;
				}
				break;
			}
			case Type.BONUS_N:
			case Type.COMBOITEM_BONUS_N:
			{
				SetAnimPlay(animInfo);
				float length = animInfo.m_animation[animInfo.m_clipName].length;
				animInfo.m_animation[animInfo.m_clipName].time = length * 0.01f;
				break;
			}
			default:
				SetAnimPlay(animInfo);
				break;
			}
		}
		switch (msg.m_cautionType)
		{
		case Type.GET_ITEM:
			if (animInfo.m_sprite != null)
			{
				animInfo.m_sprite.spriteName = "ui_cmn_icon_item_" + (int)msg.m_itemType;
			}
			break;
		case Type.GET_TIMER:
			if (animInfo.m_sprite != null)
			{
				animInfo.m_sprite.spriteName = "ui_cmn_icon_item_timer_" + msg.m_number;
			}
			break;
		}
	}

	private void SetAnimPlay(AnimInfo animInfo)
	{
		if (animInfo != null)
		{
			if (animInfo.m_animation != null && animInfo.m_animation.gameObject != null && !animInfo.m_animation.gameObject.activeSelf)
			{
				animInfo.m_animation.gameObject.SetActive(true);
			}
			animInfo.m_animation.Rewind(animInfo.m_clipName);
			DisableCondition disableCondition = animInfo.m_finishDisable ? DisableCondition.DisableAfterForward : DisableCondition.DoNotDisable;
			ActiveAnimation.Play(animInfo.m_animation, animInfo.m_clipName, Direction.Forward, EnableCondition.DoNothing, disableCondition, false);
		}
	}

	public void SetInvisibleCaution(MsgCaution msg)
	{
		if ((uint)msg.m_cautionType >= 26u)
		{
			return;
		}
		AnimInfo animInfo = m_animInfos[(int)msg.m_cautionType];
		if (animInfo.m_animation != null && !string.IsNullOrEmpty(animInfo.m_clipName))
		{
			Type cautionType = msg.m_cautionType;
			if (cautionType == Type.NO_RING)
			{
				animInfo.m_animation.playAutomatically = false;
				animInfo.m_animation.cullingType = AnimationCullingType.BasedOnRenderers;
				animInfo.m_animation.Rewind();
				animInfo.m_animation.Sample();
				animInfo.m_animation.Stop();
			}
			else
			{
				animInfo.m_animation.Play();
				float length = animInfo.m_animation[animInfo.m_clipName].length;
				animInfo.m_animation[animInfo.m_clipName].time = length;
				animInfo.m_animation.Sample();
				animInfo.m_animation.Stop();
			}
		}
	}

	public void SetMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
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
