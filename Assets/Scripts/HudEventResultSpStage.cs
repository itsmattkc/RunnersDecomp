using AnimationOrTween;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class HudEventResultSpStage : HudEventResultParts
{
	private enum EventKind
	{
		SP_EVENT,
		ANIMAL_EVENT,
		RING_EVENT,
		DISTANCE_EVENT,
		NUM
	}

	public class SpResultReplaceText
	{
		public string CountText;

		public string TotalText;

		public SpResultReplaceText(string s1, string s2)
		{
			CountText = s1;
			TotalText = s2;
		}
	}

	private GameObject m_resultRootObject;

	private HudEventResult.AnimationEndCallback m_callback;

	private Animation m_animation;

	private HudEventResult.AnimType m_currentAnimType;

	private SpecialStageInfo m_info;

	private HudEventQuota m_hudEventQuota;

	private GameResultScoreInterporate m_score;

	private GameResultScoreInterporate m_totalScore;

	private float m_timer;

	private float m_waitTime;

	private long m_beforeTotalPoint;

	private static readonly float WAIT_TIME = 1f;

	private static readonly float WAIT_TIME_WITH_BONUS = 0.5f;

	private SpResultReplaceText[] SpResultReplaceStringTable = new SpResultReplaceText[4]
	{
		new SpResultReplaceText("ui_Lbl_word_get", "ui_Lbl_word_get_total"),
		new SpResultReplaceText("ui_Lbl_word_animal_get", "ui_Lbl_word_animal_get_total"),
		new SpResultReplaceText("ui_Lbl_word_ring_get", "ui_Lbl_word_ring_get_total"),
		new SpResultReplaceText("ui_Lbl_word_run_distance_get", "ui_Lbl_word_run_distance_get_total")
	};

	public override void Init(GameObject resultRootObject, long beforeTotalPoint, HudEventResult.AnimationEndCallback callback)
	{
		m_resultRootObject = resultRootObject;
		m_beforeTotalPoint = beforeTotalPoint;
		m_callback = callback;
		m_timer = 0f;
		m_info = EventManager.Instance.SpecialStageInfo;
		if (m_info == null)
		{
			return;
		}
		m_animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "EventResult_Anim");
		if (m_animation == null)
		{
			return;
		}
		m_hudEventQuota = m_resultRootObject.GetComponent<HudEventQuota>();
		if (m_hudEventQuota == null)
		{
			m_hudEventQuota = m_resultRootObject.AddComponent<HudEventQuota>();
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_resultRootObject, "Lbl_object");
		if (uILabel != null)
		{
			StageScoreManager instance = StageScoreManager.Instance;
			if (instance != null)
			{
				m_score = new GameResultScoreInterporate();
				m_score.Setup(uILabel);
				m_score.SetLabelStartValue(instance.SpecialCrystal);
			}
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_resultRootObject, "Lbl_total_object");
		if (uILabel2 != null)
		{
			m_totalScore = new GameResultScoreInterporate();
			m_totalScore.Setup(uILabel2);
			long labelStartValue = 0L;
			StageScoreManager instance2 = StageScoreManager.Instance;
			if (instance2 != null)
			{
				labelStartValue = m_info.totalPoint - instance2.FinalCountData.sp_crystal;
			}
			m_totalScore.SetLabelStartValue(labelStartValue);
		}
		EventKind eventKind = EventKind.SP_EVENT;
		EventManager instance3 = EventManager.Instance;
		if (instance3.Type == EventManager.EventType.COLLECT_OBJECT)
		{
			switch (instance3.CollectType)
			{
			case EventManager.CollectEventType.GET_ANIMALS:
				eventKind = EventKind.ANIMAL_EVENT;
				break;
			case EventManager.CollectEventType.GET_RING:
				eventKind = EventKind.RING_EVENT;
				break;
			case EventManager.CollectEventType.RUN_DISTANCE:
				eventKind = EventKind.DISTANCE_EVENT;
				break;
			}
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_resultRootObject, "ui_Lbl_word_object_get");
		if (uILabel3 != null)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", SpResultReplaceStringTable[(int)eventKind].CountText);
			if (eventKind == EventKind.SP_EVENT)
			{
				string eventSpObjectName = HudUtility.GetEventSpObjectName();
				text.ReplaceTag("{PARAM_OBJ}", eventSpObjectName);
			}
			uILabel3.text = text.text;
		}
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_resultRootObject, "ui_Lbl_word_total_object_get");
		if (uILabel4 != null)
		{
			TextObject text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Result", SpResultReplaceStringTable[(int)eventKind].TotalText);
			if (eventKind == EventKind.SP_EVENT)
			{
				string eventSpObjectName2 = HudUtility.GetEventSpObjectName();
				text2.ReplaceTag("{PARAM_OBJ}", eventSpObjectName2);
			}
			uILabel4.text = text2.text;
		}
		List<EventMission> mission = null;
		if (m_info.GetCurrentClearMission(m_beforeTotalPoint, out mission, true) && mission != null)
		{
			int count = mission.Count;
			for (int i = 0; i < count; i++)
			{
				string name = mission[i].name;
				string formatNumString = HudUtility.GetFormatNumString(mission[i].point);
				int reward = mission[i].reward;
				string empty = string.Empty;
				empty = new ServerItem((ServerItem.Id)mission[i].reward).serverItemName;
				int index = mission[i].index;
				if (index > 1)
				{
					empty = empty + " × " + index;
				}
				bool isCleared = mission[i].IsAttainment(m_info.totalPoint);
				QuotaInfo info = new QuotaInfo(name, formatNumString, reward, empty, isCleared);
				m_hudEventQuota.AddQuota(info);
			}
		}
		m_hudEventQuota.Setup(m_resultRootObject, m_animation, "ui_EventResult_spstage_mission_inout1_Anim", "ui_EventResult_spstage_mission_inout2_Anim");
	}

	public override void PlayAnimation(HudEventResult.AnimType animType)
	{
		m_currentAnimType = animType;
		switch (animType)
		{
		case HudEventResult.AnimType.IDLE:
			break;
		case HudEventResult.AnimType.OUT_WAIT:
			break;
		case HudEventResult.AnimType.IN:
		{
			ActiveAnimation activeAnimation2 = ActiveAnimation.Play(m_animation, "ui_EventResult_spstage_intro_Anim", Direction.Forward, true);
			EventDelegate.Add(activeAnimation2.onFinished, AnimationFinishCallback, true);
			break;
		}
		case HudEventResult.AnimType.IN_BONUS:
		{
			bool flag = false;
			if (true)
			{
				Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_resultRootObject, "otomo_bonus_Anim");
				if (animation != null)
				{
					float specialCrystalRate = StageAbilityManager.Instance.SpecialCrystalRate;
					specialCrystalRate *= 100f;
					if (specialCrystalRate > 0f)
					{
						animation.gameObject.SetActive(true);
						ActiveAnimation activeAnimation3 = ActiveAnimation.Play(animation, "ui_result_bonus_ev_Anim", Direction.Forward, true);
						EventDelegate.Add(activeAnimation3.onFinished, AnimationFinishCallback, true);
						UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(animation.gameObject, "Lbl_mileage_bonus_score");
						if (uILabel != null)
						{
							uILabel.text = specialCrystalRate.ToString("F1") + "%";
						}
						if (m_score != null)
						{
							long startValue2 = 0L;
							long endValue = 0L;
							StageScoreManager instance2 = StageScoreManager.Instance;
							if (instance2 != null)
							{
								startValue2 = instance2.SpecialCrystal;
								endValue = instance2.FinalCountData.sp_crystal;
							}
							m_score.PlayStart(startValue2, endValue, 0.5f);
							SoundManager.SePlay("sys_result_count");
						}
						flag = true;
					}
				}
			}
			if (!flag)
			{
				AnimationFinishCallback();
				m_waitTime = WAIT_TIME;
			}
			else
			{
				m_waitTime = WAIT_TIME_WITH_BONUS;
			}
			break;
		}
		case HudEventResult.AnimType.WAIT_ADD_COLLECT_OBJECT:
			m_timer = 0f;
			break;
		case HudEventResult.AnimType.ADD_COLLECT_OBJECT:
			if (m_totalScore != null)
			{
				long startValue = 0L;
				long totalPoint = m_info.totalPoint;
				StageScoreManager instance = StageScoreManager.Instance;
				if (instance != null)
				{
					startValue = m_info.totalPoint - instance.FinalCountData.sp_crystal;
				}
				m_totalScore.PlayStart(startValue, totalPoint, 0.5f);
				SoundManager.SePlay("sys_result_count");
			}
			break;
		case HudEventResult.AnimType.SHOW_QUOTA_LIST:
			if (m_hudEventQuota != null)
			{
				m_hudEventQuota.PlayStart(QuotaPlayEndCallback);
			}
			break;
		case HudEventResult.AnimType.OUT:
		{
			if (m_hudEventQuota != null)
			{
				m_hudEventQuota.PlayStop();
			}
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_EventResult_spstage_outro_Anim", Direction.Forward, true);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
			}
			break;
		}
		}
	}

	private void Update()
	{
		switch (m_currentAnimType)
		{
		case HudEventResult.AnimType.IN_BONUS:
			if (m_score != null)
			{
				m_score.Update(Time.deltaTime);
				if (m_score.IsEnd)
				{
					SoundManager.SeStop("sys_result_count");
				}
			}
			break;
		case HudEventResult.AnimType.WAIT_ADD_COLLECT_OBJECT:
			m_timer += Time.deltaTime;
			if (m_timer >= m_waitTime)
			{
				m_callback(m_currentAnimType);
			}
			break;
		case HudEventResult.AnimType.ADD_COLLECT_OBJECT:
			if (m_totalScore != null)
			{
				m_totalScore.Update(Time.deltaTime);
				if (m_totalScore.IsEnd)
				{
					SoundManager.SeStop("sys_result_count");
					m_callback(m_currentAnimType);
				}
			}
			break;
		}
	}

	private void AnimationFinishCallback()
	{
		if (m_callback != null)
		{
			m_callback(m_currentAnimType);
		}
	}

	private void QuotaPlayEndCallback()
	{
		if (m_callback != null)
		{
			m_callback(m_currentAnimType);
		}
	}
}
