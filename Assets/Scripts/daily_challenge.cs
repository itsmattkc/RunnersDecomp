#define DEBUG_INFO
using AnimationOrTween;
using DataTable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Text;
using UnityEngine;

public class daily_challenge : MonoBehaviour
{
	[Serializable]
	private class InspectorUi
	{
		[SerializeField]
		public GameObject m_clearGameObject;

		[SerializeField]
		public Animation m_clearAnimation;

		[SerializeField]
		public GameObject m_dayObjectOrg;

		[SerializeField]
		public GameObject m_dayBigObjectOrg;

		[SerializeField]
		public GameObject m_dayObjectBase;

		[SerializeField]
		public List<Color> m_dayObjectColors;
	}

	public class DailyMissionInfo
	{
		public int DayIndex
		{
			get;
			set;
		}

		public int TodayMissionId
		{
			get;
			set;
		}

		public long TodayMissionClearQuota
		{
			get;
			set;
		}

		public int[] InceniveIdTable
		{
			get;
			set;
		}

		public int[] InceniveNumTable
		{
			get;
			set;
		}

		public int ClearMissionCount
		{
			get;
			set;
		}

		public bool IsClearTodayMission
		{
			get;
			set;
		}

		public long TodayMissionQuota
		{
			get;
			set;
		}

		public string TodayMissionText
		{
			get;
			set;
		}

		public int DayMax
		{
			get;
			set;
		}

		public long TodayMissionClearQuotaBefore
		{
			get;
			set;
		}

		public bool IsMissionClearNotice
		{
			get
			{
				return TodayMissionClearQuotaBefore < TodayMissionClearQuota && TodayMissionClearQuota == TodayMissionQuota;
			}
		}

		public bool IsMissionEvent
		{
			get
			{
				return !IsClearTodayMission || IsMissionClearNotice;
			}
		}
	}

	private const float BAR_SPEED = 0.004f;

	private static bool s_isUpdateEffect;

	[SerializeField]
	private InspectorUi m_inspectorUi;

	private bool m_isInitialized;

	private bool m_setUp;

	private float m_updateBarDelay;

	private float m_clearBarValue;

	private SoundManager.PlayId m_barSePlayId;

	private List<DayObject> m_days;

	private UIPlayAnimation m_windowAnime;

	private UIPlayAnimation m_windowBtnAnime;

	private UIPlayTween m_windowBtnTween;

	private DailyMissionInfo m_info;

	public static bool isUpdateEffect
	{
		get
		{
			return s_isUpdateEffect;
		}
	}

	public bool IsEndSetup
	{
		get
		{
			return m_setUp;
		}
	}

	public static DailyMissionInfo GetInfoFromSaveData(long todayMissionClearQuotaBefore)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return null;
		}
		int id = instance.PlayerData.DailyMission.id;
		MissionData missionData = MissionTable.GetMissionData(id);
		if (missionData == null)
		{
			return null;
		}
		DailyMissionInfo dailyMissionInfo = new DailyMissionInfo();
		dailyMissionInfo.DayIndex = instance.PlayerData.DailyMission.date;
		dailyMissionInfo.DayMax = instance.PlayerData.DailyMission.max;
		dailyMissionInfo.TodayMissionId = instance.PlayerData.DailyMission.id;
		dailyMissionInfo.TodayMissionClearQuota = instance.PlayerData.DailyMission.progress;
		dailyMissionInfo.InceniveIdTable = new int[instance.PlayerData.DailyMission.reward_max];
		dailyMissionInfo.InceniveNumTable = new int[instance.PlayerData.DailyMission.reward_max];
		dailyMissionInfo.ClearMissionCount = instance.PlayerData.DailyMission.clear_count;
		dailyMissionInfo.IsClearTodayMission = instance.PlayerData.DailyMission.missions_complete;
		dailyMissionInfo.TodayMissionQuota = missionData.quota;
		dailyMissionInfo.TodayMissionText = missionData.text;
		DailyMissionInfo dailyMissionInfo2 = dailyMissionInfo;
		int reward_max = instance.PlayerData.DailyMission.reward_max;
		for (int i = 0; i < reward_max; i++)
		{
			if (i < instance.PlayerData.DailyMission.reward_id.Length)
			{
				dailyMissionInfo2.InceniveIdTable[i] = instance.PlayerData.DailyMission.reward_id[i];
			}
			if (i < instance.PlayerData.DailyMission.reward_count.Length)
			{
				dailyMissionInfo2.InceniveNumTable[i] = instance.PlayerData.DailyMission.reward_count[i];
			}
		}
		dailyMissionInfo2.TodayMissionClearQuota = ((dailyMissionInfo2.TodayMissionClearQuota >= dailyMissionInfo2.TodayMissionQuota) ? dailyMissionInfo2.TodayMissionQuota : dailyMissionInfo2.TodayMissionClearQuota);
		dailyMissionInfo2.TodayMissionClearQuotaBefore = ((todayMissionClearQuotaBefore == -1) ? dailyMissionInfo2.TodayMissionClearQuota : todayMissionClearQuotaBefore);
		if (todayMissionClearQuotaBefore != -1 && dailyMissionInfo2.IsMissionClearNotice)
		{
			dailyMissionInfo2.ClearMissionCount--;
		}
		return dailyMissionInfo2;
	}

	public void Initialize(long todayMissionClearQuotaBefore)
	{
		m_info = GetInfoFromSaveData(todayMissionClearQuotaBefore);
		m_updateBarDelay = 1f;
		m_barSePlayId = SoundManager.PlayId.NONE;
		GameObject gameObject = null;
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "DailyWindowUI");
		}
		if (gameObject != null)
		{
			m_windowAnime = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(gameObject, "blinder");
			if (m_windowAnime != null)
			{
				m_windowAnime.enabled = false;
			}
			m_windowBtnAnime = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(gameObject, "Btn_next");
			m_windowBtnTween = GameObjectUtil.FindChildGameObjectComponent<UIPlayTween>(gameObject, "Btn_next");
			if (m_windowBtnAnime != null)
			{
				m_windowBtnAnime.enabled = false;
			}
			if (m_windowBtnTween != null)
			{
				m_windowBtnTween.enabled = false;
			}
		}
		if (m_info != null)
		{
			m_clearBarValue = (float)m_info.TodayMissionClearQuotaBefore / (float)m_info.TodayMissionQuota;
			DebugLog("UpdateBar " + m_info.TodayMissionClearQuotaBefore + "→" + m_info.TodayMissionClearQuota + "/" + m_info.TodayMissionQuota);
			InitItem();
			m_isInitialized = true;
			s_isUpdateEffect = true;
			UpdateView();
		}
		if (m_inspectorUi.m_clearGameObject != null)
		{
			m_inspectorUi.m_clearGameObject.SetActive(false);
		}
		m_setUp = true;
	}

	private void Update()
	{
		if (m_isInitialized)
		{
			UpdateBar(0.004f);
		}
	}

	private void UpdateBar(float speed)
	{
		m_updateBarDelay = ((!(m_updateBarDelay > Time.deltaTime)) ? 0f : (m_updateBarDelay - Time.deltaTime));
		if (m_updateBarDelay > 0f)
		{
			return;
		}
		float num = (float)m_info.TodayMissionClearQuota / (float)m_info.TodayMissionQuota;
		if (m_clearBarValue < num)
		{
			if (m_barSePlayId == SoundManager.PlayId.NONE)
			{
				m_barSePlayId = SoundManager.SePlay("sys_gauge");
			}
			m_clearBarValue = Mathf.Min(m_clearBarValue + speed, num);
			if (m_info.IsMissionClearNotice && m_clearBarValue == 1f)
			{
				m_info.ClearMissionCount++;
				StopBarSe();
				if (m_inspectorUi.m_clearGameObject != null)
				{
					m_inspectorUi.m_clearGameObject.SetActive(true);
				}
				if (m_inspectorUi.m_clearAnimation != null)
				{
					ActiveAnimation.Play(m_inspectorUi.m_clearAnimation, Direction.Forward);
				}
				if (m_days.Count > m_info.ClearMissionCount - 1)
				{
					DayObject dayObject = m_days[m_info.ClearMissionCount - 1];
					dayObject.PlayGetAnimation();
				}
				SoundManager.SePlay("sys_dailychallenge");
			}
			if (m_clearBarValue == num)
			{
				s_isUpdateEffect = false;
				m_info.TodayMissionClearQuotaBefore = m_info.TodayMissionClearQuota;
				StopBarSe();
			}
			else
			{
				s_isUpdateEffect = true;
			}
			UpdateView();
		}
		else
		{
			s_isUpdateEffect = false;
		}
	}

	private void UpdateView()
	{
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_days");
		if (uILabel != null)
		{
			uILabel.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "day").text, new Dictionary<string, string>
			{
				{
					"{DAY}",
					(m_info.DayMax - m_info.DayIndex).ToString()
				}
			});
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_daily_challenge");
		if (uILabel2 != null)
		{
			uILabel2.text = TextUtility.Replaces(m_info.TodayMissionText, new Dictionary<string, string>
			{
				{
					"{QUOTA}",
					m_info.TodayMissionQuota.ToString()
				}
			});
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_attainment_fg");
		if (gameObject != null)
		{
			gameObject.SetActive(m_clearBarValue > 0f);
		}
		UISlider uISlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_attainment");
		if (uISlider != null)
		{
			uISlider.value = m_clearBarValue;
		}
		bool flag = m_info.IsClearTodayMission && m_clearBarValue == 1f;
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_percent_clear");
		if (uILabel3 != null)
		{
			if (flag)
			{
				uILabel3.text = string.Empty;
			}
			else
			{
				float num = m_clearBarValue * 100f;
				if (num > 100f)
				{
					num = 100f;
				}
				uILabel3.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "clear_percent").text, new Dictionary<string, string>
				{
					{
						"{PARAM}",
						((int)num).ToString()
					}
				});
			}
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "img_clear");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(flag);
		}
		for (int i = 0; i < m_info.InceniveIdTable.Length; i++)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_day" + (i + 1));
			if (!(gameObject3 == null))
			{
				GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject3, "img_check");
				if (gameObject4 != null)
				{
					gameObject4.SetActive(i < m_info.ClearMissionCount);
				}
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_daily_item");
				if (uISprite != null)
				{
					uISprite.spriteName = ((i >= m_info.InceniveIdTable.Length - 1) ? "ui_cmn_icon_rsring_L" : ("ui_cmn_icon_item_" + m_info.InceniveIdTable[i]));
				}
				UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3.gameObject, "Lbl_count");
				if (uILabel4 != null)
				{
					uILabel4.text = m_info.InceniveNumTable[i].ToString();
				}
			}
		}
	}

	private void InitItem()
	{
		if (m_inspectorUi != null && m_inspectorUi.m_dayObjectBase != null && m_inspectorUi.m_dayObjectOrg != null && m_inspectorUi.m_dayBigObjectOrg != null)
		{
			GameObject dayObjectBase = m_inspectorUi.m_dayObjectBase;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_inspectorUi.m_dayObjectOrg, "img_frame");
			UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_inspectorUi.m_dayBigObjectOrg, "img_frame");
			if (uISprite != null)
			{
				num = uISprite.width;
			}
			if (uISprite2 != null)
			{
				num2 = uISprite2.width;
			}
			num3 = (float)(m_info.InceniveIdTable.Length - 1) * -0.5f * num - (num2 - num) * 0.5f;
			if (m_days != null && m_days.Count > 0)
			{
				for (int i = 0; i < m_days.Count; i++)
				{
					UnityEngine.Object.Destroy(m_days[i].m_clearGameObject);
				}
			}
			m_days = new List<DayObject>();
			for (int j = 0; j < m_info.InceniveIdTable.Length; j++)
			{
				GameObject gameObject = null;
				Color color = new Color(1f, 1f, 1f, 1f);
				if (j < m_info.InceniveIdTable.Length - 1)
				{
					gameObject = (UnityEngine.Object.Instantiate(m_inspectorUi.m_dayObjectOrg) as GameObject);
					if (m_inspectorUi.m_dayObjectColors != null && m_inspectorUi.m_dayObjectColors.Count > j)
					{
						color = m_inspectorUi.m_dayObjectColors[j];
					}
				}
				else
				{
					gameObject = (UnityEngine.Object.Instantiate(m_inspectorUi.m_dayBigObjectOrg) as GameObject);
				}
				if (!(gameObject != null))
				{
					continue;
				}
				gameObject.transform.parent = dayObjectBase.transform;
				float x = 0f;
				if (m_info.InceniveIdTable.Length > 1)
				{
					x = num3 + (float)j * num;
					if (j >= m_info.InceniveIdTable.Length - 1)
					{
						x = num3 + (float)j * num + (num2 - num) * 0.45f;
					}
				}
				gameObject.transform.localPosition = new Vector3(x, 0f, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				DayObject dayObject = new DayObject(gameObject, color, j + 1);
				TweenPosition tweenPosition = GameObjectUtil.FindChildGameObjectComponent<TweenPosition>(dayObject.m_clearGameObject, "img_daily_item");
				if (tweenPosition != null)
				{
					tweenPosition.ignoreTimeScale = false;
					tweenPosition.Reset();
				}
				TweenScale tweenScale = GameObjectUtil.FindChildGameObjectComponent<TweenScale>(dayObject.m_clearGameObject, "img_daily_item");
				if (tweenScale != null)
				{
					tweenScale.ignoreTimeScale = false;
					tweenScale.Reset();
				}
				dayObject.SetItem(m_info.InceniveIdTable[j]);
				dayObject.count = m_info.InceniveNumTable[j];
				int num4 = m_info.InceniveIdTable.Length;
				if (num4 > 0)
				{
					if (m_info.ClearMissionCount < num4)
					{
						dayObject.SetAlready(j < m_info.ClearMissionCount);
					}
					else if (m_info.InceniveIdTable.Length - 1 > j)
					{
						dayObject.SetAlready(true);
					}
					else
					{
						dayObject.SetAlready(m_info.IsClearTodayMission);
					}
				}
				else
				{
					dayObject.SetAlready(false);
				}
				m_days.Add(dayObject);
			}
		}
		else
		{
			if (m_inspectorUi == null || !(m_inspectorUi.m_dayObjectBase != null))
			{
				return;
			}
			List<GameObject> list = new List<GameObject>();
			for (int k = 1; k <= 7; k++)
			{
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "day" + k);
				if (gameObject2 != null)
				{
					list.Add(gameObject2);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			int num5 = 0;
			foreach (GameObject item in list)
			{
				UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_bg");
				UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_check");
				UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_daily_item");
				UISprite uISprite6 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_chara");
				UISprite uISprite7 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_chao");
				UISprite uISprite8 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_day_num");
				UISprite uISprite9 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(item, "img_hidden");
				if (uISprite3 != null)
				{
					uISprite3.enabled = (num5 == m_info.InceniveIdTable.Length - 1);
				}
				if (num5 < m_info.InceniveIdTable.Length)
				{
					if (uISprite4 != null)
					{
						uISprite4.enabled = (num5 < m_info.ClearMissionCount);
					}
					int num6 = m_info.InceniveIdTable[num5];
					int num7 = Mathf.FloorToInt((float)num6 / 100000f);
					if (uISprite6 != null)
					{
						uISprite6.alpha = 0f;
						uISprite6.enabled = true;
					}
					if (uISprite7 != null)
					{
						uISprite7.alpha = 0f;
						uISprite7.enabled = true;
					}
					if (uISprite5 != null)
					{
						uISprite5.alpha = 0f;
						uISprite5.enabled = true;
					}
					switch (num7)
					{
					case 3:
						if (uISprite6 != null)
						{
							uISprite6.alpha = 1f;
							uISprite6.spriteName = "ui_tex_player_" + CharaTypeUtil.GetCharaSpriteNameSuffix(new ServerItem((ServerItem.Id)num6).charaType);
						}
						break;
					case 4:
						if (uISprite7 != null)
						{
							uISprite7.alpha = 1f;
							uISprite7.spriteName = string.Format("ui_tex_chao_{0:D4}", num6 - 400000);
						}
						break;
					default:
						if (uISprite5 != null)
						{
							uISprite5.alpha = 1f;
							if (num6 >= 0)
							{
								uISprite5.spriteName = "ui_cmn_icon_item_" + num6;
							}
							else
							{
								uISprite5.spriteName = "ui_cmn_icon_item_9";
							}
						}
						break;
					}
					if (uISprite8 != null)
					{
						uISprite8.enabled = true;
						if (num5 == m_info.InceniveIdTable.Length - 1)
						{
							uISprite8.color = new Color(1f, 64f / 85f, 0f, 1f);
						}
						else
						{
							uISprite8.color = new Color(128f / 255f, 1f, 1f, 1f);
						}
					}
					if (uISprite9 != null)
					{
						uISprite9.enabled = (num5 < m_info.ClearMissionCount);
					}
				}
				else
				{
					if (uISprite4 != null)
					{
						uISprite4.enabled = false;
					}
					if (uISprite5 != null)
					{
						uISprite5.enabled = false;
					}
					if (uISprite8 != null)
					{
						uISprite8.enabled = false;
					}
					if (uISprite9 != null)
					{
						uISprite9.enabled = true;
					}
				}
				num5++;
			}
		}
	}

	private void OnStartDailyMissionInMileageMap(long todayMissionClearQuotaBefore)
	{
		m_setUp = false;
		Initialize(todayMissionClearQuotaBefore);
	}

	private void OnStartDailyMissionInScreen()
	{
		Initialize(-1L);
		HudMenuUtility.SendStartInformaionDlsplay();
	}

	private void OnClickNextButton(GameObject dailyWindowGameObject)
	{
		float num = (float)m_info.TodayMissionClearQuota / (float)m_info.TodayMissionQuota;
		if (m_clearBarValue >= num)
		{
			if (m_windowBtnAnime != null)
			{
				m_windowBtnAnime.enabled = true;
			}
			if (m_windowBtnTween != null)
			{
				m_windowBtnTween.enabled = true;
			}
			if (m_windowAnime != null)
			{
				m_windowAnime.enabled = true;
				m_windowAnime.Play(false);
			}
		}
		else
		{
			UpdateBar(1f);
		}
		StopBarSe();
	}

	private void StopBarSe()
	{
		if (m_barSePlayId != 0)
		{
			SoundManager.SeStop(m_barSePlayId);
		}
		SoundManager.SeStop("sys_gauge");
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
