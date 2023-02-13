using Message;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class GeneralUtil
{
	public const float OUT_AREA = 300000f;

	public static void RemoveCharaTexture(List<CharaType> exclusionCharaList = null)
	{
		Debug.Log("GeneralUtil RemoveCharaTexture !");
		List<CharaType> list = null;
		list = ((exclusionCharaList == null || exclusionCharaList.Count <= 0) ? new List<CharaType>() : new List<CharaType>(exclusionCharaList));
		int deckCurrentStockIndex = DeckUtil.GetDeckCurrentStockIndex();
		CharaType currentMainCharaType = CharaType.UNKNOWN;
		CharaType currentSubCharaType = CharaType.UNKNOWN;
		DeckUtil.GetDeckData(deckCurrentStockIndex, ref currentMainCharaType, ref currentSubCharaType);
		if (currentMainCharaType != CharaType.UNKNOWN)
		{
			list.Add(currentMainCharaType);
		}
		if (currentSubCharaType != CharaType.UNKNOWN)
		{
			list.Add(currentSubCharaType);
		}
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState == null)
		{
			return;
		}
		List<CharaType> characterTypeList = playerState.GetCharacterTypeList();
		if (characterTypeList == null || characterTypeList.Count <= 0)
		{
			return;
		}
		foreach (CharaType item in characterTypeList)
		{
			if (list == null || !list.Contains(item))
			{
				TextureRequestChara request = new TextureRequestChara(item, null);
				TextureAsyncLoadManager.Instance.Remove(request);
				Debug.Log("GeneralUtil RemoveCharaTexture type:" + item);
			}
		}
	}

	public static int gcd(int m, int n)
	{
		if (m == 0 || n == 0)
		{
			return 0;
		}
		while (m != n)
		{
			if (m > n)
			{
				m -= n;
			}
			else
			{
				n -= m;
			}
		}
		return m;
	}

	public static int lcm(int m, int n)
	{
		if (m == 0 || n == 0)
		{
			return 0;
		}
		return m / gcd(m, n) * n;
	}

	public static string GetDateStringHour(DateTime targetTime, int addTimeHour = 0)
	{
		return GetDateString(targetTime, addTimeHour * 60 * 60 * 1000);
	}

	public static string GetDateStringMinute(DateTime targetTime, int addTimeMinute = 0)
	{
		return GetDateString(targetTime, addTimeMinute * 60 * 1000);
	}

	public static string GetDateStringSecond(DateTime targetTime, int addTimeSecond = 0)
	{
		return GetDateString(targetTime, addTimeSecond * 1000);
	}

	public static string GetDateString(DateTime targetTime, int addTimeMillisecond = 0)
	{
		string format = "{0:D2}/{1:D2}";
		string text = "--/--";
		DateTime dateTime = targetTime.AddMilliseconds(addTimeMillisecond);
		return string.Format(format, dateTime.Month, dateTime.Day);
	}

	public static string GetTimeLimitString(DateTime targetTime, bool slightlyChangeColor = false)
	{
		string text = "--:--:--";
		DateTime currentTime = NetBase.GetCurrentTime();
		TimeSpan timeSpan = targetTime - currentTime;
		if (timeSpan.Ticks > 0)
		{
			if (timeSpan.TotalHours >= 100.0)
			{
				return "99:59:59";
			}
			if (timeSpan.TotalSeconds > 60.0 || !slightlyChangeColor)
			{
				return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			return string.Format("[ff0000]{0:D2}:{1:D2}:{2:D2}[-]", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		if (slightlyChangeColor)
		{
			return "[ff0000]00:00:00[-]";
		}
		return "00:00:00";
	}

	public static string GetTimeLimitString(TimeSpan limitTimeSpan, bool slightlyChangeColor = false)
	{
		string text = "--:--:--";
		TimeSpan timeSpan = limitTimeSpan;
		if (timeSpan.Ticks > 0)
		{
			if (timeSpan.TotalHours >= 100.0)
			{
				return "99:59:59";
			}
			if (timeSpan.TotalSeconds > 60.0 || !slightlyChangeColor)
			{
				return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			return string.Format("[ff0000]{0:D2}:{1:D2}:{2:D2}[-]", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		if (slightlyChangeColor)
		{
			return "[ff0000]00:00:00[-]";
		}
		return "00:00:00";
	}

	public static bool SetButtonFunc(GameObject parent, string buttonObjectName, GameObject target, string functionName)
	{
		bool result = false;
		if (parent == null)
		{
			parent = GameObject.Find("UI Root (2D)");
		}
		if (parent != null)
		{
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(parent, buttonObjectName);
			if (uIButtonMessage == null)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(parent, buttonObjectName);
				if (gameObject != null)
				{
					uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
				}
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.target = target;
				uIButtonMessage.functionName = functionName;
				result = true;
			}
		}
		return result;
	}

	public static bool SetButtonAnimFinished(GameObject parent, string buttonObjectName, EventDelegate.Callback func)
	{
		bool result = false;
		if (parent == null)
		{
			parent = GameObject.Find("UI Root (2D)");
		}
		if (parent != null)
		{
			List<UIPlayAnimation> list = GameObjectUtil.FindChildGameObjectsComponents<UIPlayAnimation>(parent, buttonObjectName);
			{
				foreach (UIPlayAnimation item in list)
				{
					if (item != null && item.onFinished.Count <= 0)
					{
						EventDelegate.Add(item.onFinished, func, true);
						result = true;
					}
				}
				return result;
			}
		}
		return result;
	}

	public static bool IsOverTimeHour(DateTime baseTime, int addTimeHour = 0)
	{
		return IsOverTime(baseTime, addTimeHour * 60 * 60 * 1000);
	}

	public static bool IsOverTimeMinute(DateTime baseTime, int addTimeMinute = 0)
	{
		return IsOverTime(baseTime, addTimeMinute * 60 * 1000);
	}

	public static bool IsOverTimeSecond(DateTime baseTime, int addTimeSecond = 0)
	{
		return IsOverTime(baseTime, addTimeSecond * 1000);
	}

	public static bool IsOverTime(DateTime baseTime, int addTimeMillisecond = 0)
	{
		bool result = false;
		DateTime currentTime = NetBase.GetCurrentTime();
		if ((currentTime - baseTime).TotalMilliseconds > (double)addTimeMillisecond)
		{
			result = true;
		}
		return result;
	}

	public static bool IsInTime(DateTime startTime, DateTime endTime, int addTimeMillisecond = 0)
	{
		bool result = false;
		DateTime dateTime = NetBase.GetCurrentTime().AddMilliseconds(addTimeMillisecond);
		if (dateTime.Ticks >= startTime.Ticks && dateTime.Ticks < endTime.Ticks)
		{
			result = true;
		}
		return result;
	}

	public static void RandomList<T>(ref List<T> listData)
	{
		int num = listData.Count;
		while (num > 1)
		{
			num--;
			int index = UnityEngine.Random.Range(0, listData.Count);
			T value = listData[index];
			listData[index] = listData[num];
			listData[num] = value;
		}
	}

	public static void CleanAllCache()
	{
		Caching.CleanCache();
		if (InformationImageManager.Instance != null)
		{
			InformationImageManager.Instance.DeleteImageFiles();
		}
	}

	public static bool IsNetwork()
	{
		return true;
	}

	public static void ShowNoCommunication(string windowName = "ShowNoCommunication")
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = windowName;
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_cutoff");
		info.isPlayErrorSe = true;
		GeneralWindow.Create(info);
	}

	public static void ShowEventEnd(string windowName = "ShowEventEnd")
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = windowName;
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "event_finished_game_result_caption");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "event_finished_guidance");
		info.isPlayErrorSe = true;
		GeneralWindow.Create(info);
	}

	public static bool CheckChaoTexture(UITexture target, int chaoId)
	{
		bool result = false;
		if (target != null && target.mainTexture != null && chaoId >= 0)
		{
			string name = target.mainTexture.name;
			if (name.IndexOf("default") == -1)
			{
				string[] array = name.Split('_');
				if (array != null && array.Length > 1 && !string.IsNullOrEmpty(array[array.Length - 1]))
				{
					int num = int.Parse(array[array.Length - 1]);
					if (num == chaoId)
					{
						result = true;
					}
					else if (num % 10000 == chaoId % 10000)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static bool CheckChaoTexture(Texture targetTexture, int chaoId)
	{
		bool result = false;
		if (targetTexture != null && chaoId >= 0)
		{
			string name = targetTexture.name;
			if (name.IndexOf("default") == -1)
			{
				string[] array = name.Split('_');
				if (array != null && array.Length > 1 && !string.IsNullOrEmpty(array[array.Length - 1]))
				{
					int num = int.Parse(array[array.Length - 1]);
					if (num == chaoId)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	public static GameObject SetToggleObject(GameObject btnTargetObject, GameObject parent, List<string> btnFuncName, List<string> toggleObjectName, int currentSelectIndex, bool enabled = true)
	{
		GameObject gameObject = null;
		if (parent != null && toggleObjectName != null && toggleObjectName.Count > 0 && btnFuncName != null && btnFuncName.Count > 0 && btnFuncName.Count == toggleObjectName.Count)
		{
			gameObject = GameObjectUtil.FindChildGameObject(parent, "tab_mask");
			if (gameObject != null)
			{
				gameObject.SetActive(!enabled);
			}
			UIToggle uIToggle = null;
			for (int i = 0; i < toggleObjectName.Count; i++)
			{
				string name = toggleObjectName[i];
				string functionName = btnFuncName[i];
				UIToggle uIToggle2 = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(parent, name);
				UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(parent, name);
				if (uIToggle2 != null)
				{
					if (uIToggle == null)
					{
						uIToggle = uIToggle2;
					}
					if (i == currentSelectIndex)
					{
						uIToggle2.startsActive = true;
						uIToggle = uIToggle2;
					}
					else
					{
						uIToggle2.startsActive = false;
					}
				}
				if (uIButtonMessage != null)
				{
					uIButtonMessage.target = btnTargetObject;
					uIButtonMessage.functionName = functionName;
					uIButtonMessage.gameObject.SetActive(true);
				}
			}
			if (uIToggle != null)
			{
				uIToggle.gameObject.SendMessage("Start");
				MonoBehaviour component = btnTargetObject.GetComponent<MonoBehaviour>();
				if (component != null)
				{
					component.StartCoroutine(InitCoroutine(uIToggle.gameObject));
				}
			}
		}
		return gameObject;
	}

	private static IEnumerator InitCoroutine(GameObject target)
	{
		yield return null;
		target.SetActive(false);
		target.SetActive(true);
	}

	public static void SetGameObjectOutMoveEnabled(GameObject target, bool enabled)
	{
		Vector3 localPosition = target.transform.localPosition;
		float num = localPosition.x;
		if (enabled)
		{
			if (num > 225000f)
			{
				num -= 300000f;
			}
		}
		else if (num < 225000f)
		{
			num += 300000f;
		}
		target.gameObject.transform.localPosition = new Vector3(num, localPosition.y, localPosition.z);
	}

	public static bool IsGameObjectOutMoveEnabled(GameObject target)
	{
		bool result = true;
		Vector3 localPosition = target.transform.localPosition;
		float x = localPosition.x;
		if (x > 225000f)
		{
			result = false;
		}
		return result;
	}

	public static bool SetEventBanner(GameObject parentObject = null, string targetObjectName = "event_banner")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					parentObject = gameObject2;
				}
			}
		}
		if (parentObject != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject3 != null)
			{
				EventManager.EventType eventType = EventManager.EventType.UNKNOWN;
				if (EventManager.Instance != null)
				{
					eventType = EventManager.Instance.Type;
				}
				GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject3, "badge_alert");
				GameObject gameObject5 = GameObjectUtil.FindChildGameObject(gameObject3, "banner_slot");
				GameObject gameObject6 = GameObjectUtil.FindChildGameObject(gameObject3, "CollectAnimalOption");
				if (gameObject5 != null)
				{
					gameObject5.SetActive(true);
				}
				if (gameObject6 != null)
				{
					bool flag = eventType != EventManager.EventType.UNKNOWN && eventType != EventManager.EventType.GACHA && eventType != EventManager.EventType.ADVERT;
					gameObject6.SetActive(flag);
					if (flag)
					{
						UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject6, "Lbl_num_event_obj");
						UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject6, "ui_Lbl_word_object_total_main");
						if (uILabel != null && uILabel2 != null)
						{
							long num = 0L;
							switch (eventType)
							{
							case EventManager.EventType.RAID_BOSS:
							{
								RaidBossInfo raidBossInfo = EventManager.Instance.RaidBossInfo;
								if (raidBossInfo != null)
								{
									num = raidBossInfo.totalDestroyCount;
									if (num >= 10000000)
									{
										num = 9999999L;
										uILabel.text = HudUtility.GetFormatNumString(num) + "+";
									}
									else
									{
										uILabel.text = HudUtility.GetFormatNumString(num);
									}
									uILabel2.text = raidBossInfo.rightTitle;
								}
								else
								{
									uILabel.text = "0";
									uILabel2.text = string.Empty;
								}
								break;
							}
							case EventManager.EventType.COLLECT_OBJECT:
							{
								EtcEventInfo etcEventInfo = EventManager.Instance.EtcEventInfo;
								if (etcEventInfo != null)
								{
									num = etcEventInfo.totalPoint;
									if (num >= 10000000)
									{
										num = 9999999L;
										uILabel.text = HudUtility.GetFormatNumString(num) + "+";
									}
									else
									{
										uILabel.text = HudUtility.GetFormatNumString(num);
									}
									uILabel2.text = etcEventInfo.rightTitle;
								}
								else
								{
									uILabel.text = "0";
									uILabel2.text = string.Empty;
								}
								break;
							}
							case EventManager.EventType.SPECIAL_STAGE:
							{
								SpecialStageInfo specialStageInfo = EventManager.Instance.SpecialStageInfo;
								if (specialStageInfo != null)
								{
									num = specialStageInfo.totalPoint;
									if (num >= 10000000)
									{
										num = 9999999L;
										uILabel.text = HudUtility.GetFormatNumString(num) + "+";
									}
									else
									{
										uILabel.text = HudUtility.GetFormatNumString(num);
									}
									uILabel2.text = specialStageInfo.rightTitle;
								}
								else
								{
									uILabel.text = "0";
									uILabel2.text = string.Empty;
								}
								break;
							}
							}
						}
					}
				}
				if (gameObject4 != null)
				{
					bool active = false;
					switch (eventType)
					{
					case EventManager.EventType.RAID_BOSS:
					{
						RaidBossInfo raidBossInfo2 = EventManager.Instance.RaidBossInfo;
						if (raidBossInfo2 != null && raidBossInfo2.IsAttention())
						{
							active = true;
						}
						break;
					}
					case EventManager.EventType.SPECIAL_STAGE:
					case EventManager.EventType.COLLECT_OBJECT:
					case EventManager.EventType.GACHA:
						active = false;
						break;
					}
					gameObject4.SetActive(active);
				}
				return true;
			}
		}
		return false;
	}

	public static bool SetRouletteBannerBtn(GameObject parentObject, string targetObjectName, GameObject functionTarget, string functionName, RouletteCategory category, bool enabled)
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
			}
		}
		if (parentObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject2 != null)
			{
				if (enabled)
				{
					gameObject2.SetActive(true);
					UITexture componentInChildren = gameObject2.GetComponentInChildren<UITexture>();
					UIButtonMessage componentInChildren2 = gameObject2.GetComponentInChildren<UIButtonMessage>();
					if (componentInChildren != null)
					{
						RouletteInformationManager.InfoBannerRequest bannerRequest = new RouletteInformationManager.InfoBannerRequest(componentInChildren);
						if (category == RouletteCategory.SPECIAL)
						{
							category = RouletteCategory.PREMIUM;
						}
						RouletteInformationManager.Instance.LoadInfoBaner(bannerRequest, category);
					}
					if (componentInChildren2 != null && functionTarget != null)
					{
						componentInChildren2.functionName = functionName;
						componentInChildren2.target = functionTarget;
					}
				}
				else
				{
					gameObject2.SetActive(false);
				}
				return true;
			}
		}
		return false;
	}

	public static bool SetEndlessRankingBtnIcon(GameObject parentObject = null, string targetObjectName = "Btn_1_ranking")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "0_Endless");
					if (gameObject3 != null)
					{
						parentObject = gameObject3;
					}
				}
			}
		}
		GameObject targetObject = null;
		if (parentObject != null)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject4 != null)
			{
				targetObject = gameObject4;
			}
		}
		return SetRankingBtnIcon(targetObject, RankingUtil.RankingMode.ENDLESS);
	}

	public static bool SetEndlessRankingTime(GameObject parentObject = null, string targetObjectName = "Btn_1_ranking")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "0_Endless");
					if (gameObject3 != null)
					{
						parentObject = gameObject3;
					}
				}
			}
		}
		GameObject targetObject = null;
		if (parentObject != null)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject4 != null)
			{
				targetObject = gameObject4;
			}
		}
		return SetRankingTime(targetObject, RankingUtil.RankingMode.ENDLESS);
	}

	public static bool SetQuickRankingBtnIcon(GameObject parentObject = null, string targetObjectName = "Btn_1_ranking")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "1_Quick");
					if (gameObject3 != null)
					{
						parentObject = gameObject3;
					}
				}
			}
		}
		GameObject targetObject = null;
		if (parentObject != null)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject4 != null)
			{
				targetObject = gameObject4;
			}
		}
		return SetRankingBtnIcon(targetObject, RankingUtil.RankingMode.QUICK);
	}

	public static bool SetQuickRankingTime(GameObject parentObject = null, string targetObjectName = "Btn_1_ranking")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "1_Quick");
					if (gameObject3 != null)
					{
						parentObject = gameObject3;
					}
				}
			}
		}
		GameObject targetObject = null;
		if (parentObject != null)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject4 != null)
			{
				targetObject = gameObject4;
			}
		}
		return SetRankingTime(targetObject, RankingUtil.RankingMode.QUICK);
	}

	private static bool SetRankingBtnIcon(GameObject targetObject, RankingUtil.RankingMode mode)
	{
		bool result = false;
		TimeSpan timeSpan = default(TimeSpan);
		if (targetObject != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(targetObject, "img_icon_league");
			UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(targetObject, "img_icon_league_sub");
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(targetObject, "Lbl_league_rank");
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(targetObject, "Lbl_limit");
			if (SingletonGameObject<RankingManager>.Instance != null && ServerInterface.PlayerState != null)
			{
				int leagueType = ServerInterface.PlayerState.m_leagueIndex;
				RankingUtil.RankingScoreType scoreType = RankingManager.EndlessRivalRankingScoreType;
				RankingUtil.RankingRankerType rankerType = RankingUtil.RankingRankerType.RIVAL;
				if (mode == RankingUtil.RankingMode.QUICK)
				{
					scoreType = RankingManager.QuickRivalRankingScoreType;
					leagueType = ServerInterface.PlayerState.m_leagueIndexQuick;
				}
				if (uISprite != null && uISprite2 != null)
				{
					uISprite.spriteName = RankingUtil.GetLeagueIconName((LeagueType)leagueType);
					uISprite2.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)leagueType);
				}
				List<RankingUtil.Ranker> cacheRankingList = SingletonGameObject<RankingManager>.Instance.GetCacheRankingList(mode, scoreType, rankerType);
				bool flag = false;
				if (cacheRankingList != null && cacheRankingList.Count > 0)
				{
					RankingUtil.Ranker ranker = cacheRankingList[0];
					result = true;
					if (ranker != null)
					{
						flag = true;
						if (uILabel != null)
						{
							uILabel.text = string.Empty + (ranker.rankIndex + 1);
						}
						timeSpan = SingletonGameObject<RankingManager>.Instance.GetRankigResetTimeSpan(mode, scoreType, rankerType);
						if (uILabel2 != null)
						{
							uILabel2.text = RankingUtil.GetResetTime(timeSpan, false);
						}
					}
					Debug.Log(string.Concat("SetRankingBtnIcon mode:", mode, "  rankingList:", cacheRankingList.Count));
				}
				if (!flag && uILabel != null)
				{
					uILabel.text = string.Empty;
				}
			}
		}
		return result;
	}

	private static bool SetRankingTime(GameObject targetObject, RankingUtil.RankingMode mode)
	{
		bool result = false;
		TimeSpan timeSpan = default(TimeSpan);
		if (targetObject != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(targetObject, "Lbl_limit");
			if (SingletonGameObject<RankingManager>.Instance != null && ServerInterface.PlayerState != null)
			{
				RankingUtil.RankingScoreType scoreType = RankingManager.EndlessRivalRankingScoreType;
				RankingUtil.RankingRankerType rankerType = RankingUtil.RankingRankerType.RIVAL;
				if (mode == RankingUtil.RankingMode.QUICK)
				{
					scoreType = RankingManager.QuickRivalRankingScoreType;
				}
				timeSpan = SingletonGameObject<RankingManager>.Instance.GetRankigResetTimeSpan(mode, scoreType, rankerType);
				result = true;
				if (uILabel != null)
				{
					uILabel.text = RankingUtil.GetResetTime(timeSpan, false);
				}
			}
		}
		return result;
	}

	public static bool SetDailyBattleBtnIcon(GameObject parentObject = null, string targetObjectName = "Btn_2_battle")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					parentObject = gameObject2;
				}
			}
		}
		bool result = false;
		TimeSpan timeSpan = default(TimeSpan);
		if (parentObject != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			UIButtonMessage x = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(parentObject, targetObjectName);
			if (x != null)
			{
			}
			if (gameObject3 != null)
			{
				GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject3, "duel_mine_set");
				GameObject gameObject5 = GameObjectUtil.FindChildGameObject(gameObject3, "duel_adversary_set");
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_limit");
				if (SingletonGameObject<DailyBattleManager>.Instance != null)
				{
					bool flag = false;
					if (SingletonGameObject<DailyBattleManager>.Instance.currentWinFlag >= 3)
					{
						flag = true;
					}
					else if (SingletonGameObject<DailyBattleManager>.Instance.currentWinFlag >= 2)
					{
						flag = true;
					}
					ServerDailyBattleDataPair currentDataPair = SingletonGameObject<DailyBattleManager>.Instance.currentDataPair;
					bool flag2 = false;
					if (gameObject4 != null)
					{
						if (currentDataPair != null && currentDataPair.myBattleData != null)
						{
							gameObject4.SetActive(true);
							GameObject gameObject6 = GameObjectUtil.FindChildGameObject(gameObject4, "duel_win_set");
							GameObject gameObject7 = GameObjectUtil.FindChildGameObject(gameObject4, "duel_lose_set");
							UITexture texUser = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject4, "img_icon_friends");
							GameObject imgUser = GameObjectUtil.FindChildGameObject(gameObject4, "img_icon_friends_default");
							if (string.IsNullOrEmpty(currentDataPair.myBattleData.userId))
							{
								flag = false;
							}
							if (gameObject6 != null)
							{
								gameObject6.SetActive(flag);
							}
							if (gameObject7 != null)
							{
								gameObject7.SetActive(!flag);
							}
							if (texUser != null && imgUser != null)
							{
								string userId = currentDataPair.myBattleData.userId;
								if (string.IsNullOrEmpty(userId))
								{
									userId = ServerInterface.SettingState.m_userId;
								}
								imgUser.gameObject.SetActive(true);
								texUser.alpha = 0f;
								texUser.mainTexture = RankingUtil.GetProfilePictureTexture(userId, delegate(Texture2D _faceTexture)
								{
									texUser.gameObject.SetActive(true);
									texUser.mainTexture = _faceTexture;
									texUser.alpha = 1f;
									imgUser.gameObject.SetActive(false);
								});
							}
							flag2 = true;
						}
						else
						{
							gameObject4.SetActive(false);
						}
					}
					if (gameObject5 != null)
					{
						if (currentDataPair != null && currentDataPair.rivalBattleData != null && !string.IsNullOrEmpty(currentDataPair.rivalBattleData.userId))
						{
							gameObject5.SetActive(true);
							GameObject gameObject8 = GameObjectUtil.FindChildGameObject(gameObject5, "duel_win_set");
							GameObject gameObject9 = GameObjectUtil.FindChildGameObject(gameObject5, "duel_lose_set");
							UITexture texUser2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject5, "img_icon_friends");
							GameObject imgUser2 = GameObjectUtil.FindChildGameObject(gameObject5, "img_icon_friends_default");
							if (gameObject8 != null)
							{
								gameObject8.SetActive(!flag);
							}
							if (gameObject9 != null)
							{
								gameObject9.SetActive(flag);
							}
							if (texUser2 != null && imgUser2 != null)
							{
								if (currentDataPair.rivalBattleData.CheckFriend())
								{
									string userId2 = currentDataPair.rivalBattleData.userId;
									imgUser2.gameObject.SetActive(true);
									texUser2.alpha = 0f;
									texUser2.mainTexture = RankingUtil.GetProfilePictureTexture(userId2, delegate(Texture2D _faceTexture)
									{
										texUser2.gameObject.SetActive(true);
										texUser2.mainTexture = _faceTexture;
										texUser2.alpha = 1f;
										imgUser2.gameObject.SetActive(false);
									});
								}
								else
								{
									texUser2.gameObject.SetActive(false);
									imgUser2.gameObject.SetActive(true);
								}
							}
							flag2 = true;
						}
						else
						{
							gameObject5.SetActive(false);
						}
					}
					if (!flag2)
					{
						Debug.Log("DailyBattle not start !!!!!!!");
					}
					else
					{
						timeSpan = SingletonGameObject<DailyBattleManager>.Instance.GetLimitTimeSpan();
						result = true;
						if (uILabel != null)
						{
							uILabel.text = GetTimeLimitString(timeSpan, true);
						}
					}
				}
			}
		}
		return result;
	}

	public static bool SetDailyBattleTime(GameObject parentObject = null, string targetObjectName = "Btn_2_battle")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					parentObject = gameObject2;
				}
			}
		}
		bool result = false;
		TimeSpan timeSpan = default(TimeSpan);
		if (parentObject != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			UIButtonMessage x = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(parentObject, targetObjectName);
			if (x != null)
			{
			}
			if (gameObject3 != null)
			{
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_limit");
				if (SingletonGameObject<DailyBattleManager>.Instance != null)
				{
					timeSpan = SingletonGameObject<DailyBattleManager>.Instance.GetLimitTimeSpan();
					result = true;
					if (uILabel != null)
					{
						uILabel.text = GetTimeLimitString(timeSpan, true);
					}
				}
			}
		}
		return result;
	}

	public static bool SetRouletteBtnIcon(GameObject parentObject = null, string targetObjectName = "Btn_roulette")
	{
		if (parentObject == null)
		{
			GameObject gameObject = GameObject.Find("UI Root (2D)");
			if (gameObject != null)
			{
				parentObject = gameObject;
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "MainMenuUI4");
				if (gameObject2 != null)
				{
					parentObject = gameObject2;
				}
			}
		}
		if (parentObject != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject3 != null)
			{
				GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject3, "badge_spin");
				GameObject gameObject5 = GameObjectUtil.FindChildGameObject(gameObject3, "badge_egg");
				GameObject gameObject6 = GameObjectUtil.FindChildGameObject(gameObject3, "badge_alert");
				GameObject gameObject7 = GameObjectUtil.FindChildGameObject(gameObject3, "event_icon");
				if (gameObject4 != null)
				{
					bool active = false;
					GameObject gameObject8 = gameObject4.transform.FindChild("Lbl_roulette_volume").gameObject;
					if (gameObject8 != null)
					{
						UILabel component = gameObject8.GetComponent<UILabel>();
						if (component != null)
						{
							int num = 0;
							ServerWheelOptions wheelOptions = ServerInterface.WheelOptions;
							if (wheelOptions != null)
							{
								num = wheelOptions.m_numRemaining;
								if (num >= 1)
								{
									active = true;
								}
							}
							component.text = num.ToString();
						}
					}
					gameObject4.SetActive(active);
				}
				if (gameObject5 != null)
				{
					bool active2 = false;
					if (RouletteManager.Instance != null && RouletteManager.Instance.specialEgg >= 10)
					{
						active2 = true;
					}
					gameObject5.SetActive(active2);
				}
				if (gameObject6 != null)
				{
					bool active3 = HudMenuUtility.IsSale(Constants.Campaign.emType.ChaoRouletteCost);
					gameObject6.SetActive(active3);
				}
				if (gameObject7 != null)
				{
					bool active4 = false;
					EventManager instance = EventManager.Instance;
					if (instance != null)
					{
						active4 = instance.IsInEvent();
					}
					gameObject7.SetActive(active4);
				}
				return true;
			}
		}
		return false;
	}

	public static bool SetCharasetBtnIcon(GameObject parentObject = null, string targetObjectName = "Btn_charaset")
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			PlayerData playerData = instance.PlayerData;
			if (playerData != null)
			{
				CharaType mainChara = playerData.MainChara;
				CharaType subChara = playerData.SubChara;
				int mainChaoID = playerData.MainChaoID;
				int subChaoID = playerData.SubChaoID;
				return SetCharasetBtnIcon(mainChara, subChara, mainChaoID, subChaoID, parentObject, targetObjectName);
			}
		}
		return false;
	}

	public static bool SetCharasetBtnIcon(CharaType mainChara, CharaType subChara, int mainChaoId, int subChaoId, GameObject parentObject = null, string targetObjectName = "Btn_charaset")
	{
		if (parentObject == null)
		{
			parentObject = GameObject.Find("UI Root (2D)");
		}
		if (parentObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(parentObject, targetObjectName);
			if (gameObject != null)
			{
				UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject, "img_chao_main");
				UITexture uITexture2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject, "img_chao_sub");
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_player_main");
				UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_player_sub");
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_player_main_lv");
				UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_decknum");
				UITexture[] componentsInChildren = gameObject.GetComponentsInChildren<UITexture>();
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					UITexture[] array = componentsInChildren;
					foreach (UITexture uITexture3 in array)
					{
						uITexture3.gameObject.SetActive(false);
					}
				}
				if (uITexture != null)
				{
					if (mainChaoId >= 0)
					{
						ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
						ChaoTextureManager.Instance.GetTexture(mainChaoId, info);
						uITexture.gameObject.SetActive(true);
					}
					else
					{
						uITexture.gameObject.SetActive(false);
					}
				}
				if (uITexture2 != null)
				{
					if (subChaoId >= 0)
					{
						ChaoTextureManager.CallbackInfo info2 = new ChaoTextureManager.CallbackInfo(uITexture2, null, true);
						ChaoTextureManager.Instance.GetTexture(subChaoId, info2);
						uITexture2.gameObject.SetActive(true);
					}
					else
					{
						uITexture2.gameObject.SetActive(false);
					}
				}
				if (uISprite != null)
				{
					uISprite.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(mainChara);
					uISprite.gameObject.SetActive(true);
				}
				if (uISprite2 != null)
				{
					uISprite2.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(subChara);
					uISprite2.gameObject.SetActive(true);
				}
				if (uILabel != null)
				{
					string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
					uILabel.text = text.Replace("{PARAM}", SaveDataUtil.GetCharaLevel(mainChara).ToString());
				}
				int deckCurrentStockIndex = DeckUtil.GetDeckCurrentStockIndex();
				if (uISprite3 != null)
				{
					uISprite3.spriteName = "ui_chao_set_deck_tab_" + (deckCurrentStockIndex + 1);
				}
				return true;
			}
		}
		return false;
	}

	public static void SetPresentItemCount(MsgUpdateMesseageSucceed msg)
	{
		if (msg == null || msg.m_presentStateList == null || msg.m_presentStateList.Count <= 0)
		{
			return;
		}
		List<ServerPresentState> presentStateList = msg.m_presentStateList;
		foreach (ServerPresentState item in presentStateList)
		{
			ServerItem serverItem = new ServerItem((ServerItem.Id)item.m_itemId);
			if (serverItem.idType == ServerItem.IdType.EGG_ITEM || serverItem.idType == ServerItem.IdType.RAIDRING || serverItem.idType == ServerItem.IdType.ITEM_ROULLETE_TICKET || serverItem.idType == ServerItem.IdType.PREMIUM_ROULLETE_TICKET)
			{
				AddItemCount(serverItem.id, item.m_numItem);
			}
		}
	}

	private static void AddItemCount(ServerItem.Id itemId, long count)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && instance.ItemData != null)
		{
			instance.ItemData.AddEtcItemCount(itemId, count);
		}
	}

	public static void SetItemCount(ServerItem.Id itemId, long count)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance != null) || instance.ItemData == null)
		{
			return;
		}
		if (itemId != ServerItem.Id.RING && itemId != ServerItem.Id.RSRING)
		{
			instance.ItemData.SetEtcItemCount(itemId, count);
			return;
		}
		switch (itemId)
		{
		case ServerItem.Id.RING:
			instance.ItemData.RingCount = (uint)count;
			instance.ItemData.RingCountOffset = 0;
			break;
		case ServerItem.Id.RSRING:
			instance.ItemData.RedRingCount = (uint)count;
			instance.ItemData.RedRingCountOffset = 0;
			break;
		}
	}

	public static bool SetItemCountOffset(ServerItem.Id itemId, long offset)
	{
		bool result = false;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && instance.ItemData != null)
		{
			if (itemId != ServerItem.Id.RING && itemId != ServerItem.Id.RSRING)
			{
				result = instance.ItemData.SetEtcItemCountOffset(itemId, offset);
			}
			else
			{
				switch (itemId)
				{
				case ServerItem.Id.RING:
					instance.ItemData.RingCountOffset = (int)offset;
					break;
				case ServerItem.Id.RSRING:
					instance.ItemData.RedRingCountOffset = (int)offset;
					break;
				}
				result = true;
			}
		}
		return result;
	}

	public static long GetItemCount(ServerItem.Id itemId)
	{
		long result = 0L;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && instance.ItemData != null)
		{
			if (itemId != ServerItem.Id.RING && itemId != ServerItem.Id.RSRING)
			{
				result = instance.ItemData.GetEtcItemCount(itemId);
			}
			else
			{
				switch (itemId)
				{
				case ServerItem.Id.RING:
					result = instance.ItemData.DisplayRingCount;
					break;
				case ServerItem.Id.RSRING:
					result = instance.ItemData.DisplayRedRingCount;
					break;
				}
			}
		}
		return result;
	}

	public static long GetItemCountOffset(ServerItem.Id itemId)
	{
		long result = 0L;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && instance.ItemData != null)
		{
			if (itemId != ServerItem.Id.RING && itemId != ServerItem.Id.RSRING)
			{
				result = instance.ItemData.GetEtcItemCountOffset(itemId);
			}
			else
			{
				switch (itemId)
				{
				case ServerItem.Id.RING:
					result = instance.ItemData.RingCountOffset;
					break;
				case ServerItem.Id.RSRING:
					result = instance.ItemData.RedRingCountOffset;
					break;
				}
			}
		}
		return result;
	}

	public bool IsItemCount(ServerItem.Id itemId)
	{
		bool result = false;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && instance.ItemData != null)
		{
			result = (itemId == ServerItem.Id.RING || itemId == ServerItem.Id.RSRING || instance.ItemData.IsEtcItemCount(itemId));
		}
		return result;
	}
}
