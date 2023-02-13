using AnimationOrTween;
using DataTable;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ui_mm_mileage_page : MonoBehaviour
{
	private enum WayPointEventType
	{
		NONE,
		SIMPLE,
		GORGEOUS,
		LAST
	}

	private abstract class BaseEvent
	{
		protected GameObject gameObject
		{
			get;
			set;
		}

		protected ui_mm_mileage_page mileage_page
		{
			get;
			set;
		}

		public bool isEnd
		{
			get;
			set;
		}

		public BaseEvent(GameObject gameObject)
		{
			this.gameObject = gameObject;
			mileage_page = gameObject.GetComponent<ui_mm_mileage_page>();
		}

		public virtual void Start()
		{
		}

		public abstract bool Update();

		public virtual void SkipMileageProcess()
		{
		}

		protected bool IsAskSnsFeed()
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					return systemdata.IsFacebookWindow();
				}
			}
			return true;
		}

		protected void SetDisableAskSnsFeed()
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					systemdata.SetFacebookWindow(false);
					instance.SaveSystemData();
				}
			}
		}
	}

	private class WaitEvent : BaseEvent
	{
		private float waitTime
		{
			get;
			set;
		}

		private float time
		{
			get;
			set;
		}

		public WaitEvent(GameObject gameObject, float waitTime)
			: base(gameObject)
		{
			this.waitTime = waitTime;
		}

		public override void Start()
		{
			time = 0f;
		}

		public override bool Update()
		{
			float time = this.time;
			this.time += Time.deltaTime;
			if (time < waitTime && this.time >= waitTime)
			{
				base.isEnd = true;
			}
			return base.isEnd;
		}
	}

	private class GeneralEvent : BaseEvent
	{
		public GeneralWindow.ButtonType buttonType
		{
			get;
			private set;
		}

		public string title
		{
			get;
			private set;
		}

		public string message
		{
			get;
			private set;
		}

		public GeneralEvent(GameObject gameObject, GeneralWindow.ButtonType buttonType, string title, string message)
			: base(gameObject)
		{
			this.buttonType = buttonType;
			this.title = title;
			this.message = message;
		}

		public override void Start()
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.buttonType = buttonType;
			info.caption = title;
			info.message = message;
			GeneralWindow.Create(info);
		}

		public override bool Update()
		{
			if (GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				base.isEnd = true;
			}
			return base.isEnd;
		}
	}

	private class SimpleEvent : BaseEvent
	{
		private enum WindowType
		{
			CHARA_GET,
			CHARA_LEVEL_UP,
			CHAO_GET,
			CHAO_GET_SRARE,
			CHAO_LEVEL_UP,
			ITEM,
			UNKNOWN
		}

		private enum StateMode
		{
			SET_WINDOW_TYPE,
			REQUEST_LOAD,
			WAIT_LOAD,
			START_WIDOW,
			WAIT_END_WIDOW,
			END
		}

		private ServerItem m_serverItem;

		private ChaoGetWindow m_charaGetWindow;

		private ChaoGetWindow m_chaoGetWindow;

		private ChaoMergeWindow m_chaoMergeWindow;

		private PlayerMergeWindow m_playerMergeWindow;

		private ButtonEventResourceLoader m_buttonEventResourceLoader;

		private WindowType m_windowType = WindowType.UNKNOWN;

		private StateMode m_stateMode;

		private RewardType rewardType
		{
			get;
			set;
		}

		private int serverItemId
		{
			get;
			set;
		}

		private int count
		{
			get;
			set;
		}

		private string title
		{
			get;
			set;
		}

		private bool disableSe
		{
			get;
			set;
		}

		public SimpleEvent(GameObject gameObject, int serverItemId, int count, string title, bool disableSe = false)
			: base(gameObject)
		{
			this.serverItemId = serverItemId;
			rewardType = new ServerItem((ServerItem.Id)serverItemId).rewardType;
			this.count = count;
			this.title = title;
			this.disableSe = disableSe;
			m_serverItem = new ServerItem(rewardType);
		}

		public override void Start()
		{
			if (rewardType != RewardType.NONE && count > 0)
			{
				MileageMapUtility.AddReward(rewardType, count);
			}
		}

		public void ResourceLoadEndCallback()
		{
			if (FontManager.Instance != null)
			{
				FontManager.Instance.ReplaceFont();
			}
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ReplaceAtlasForMenu(true);
			}
			m_stateMode = StateMode.START_WIDOW;
		}

		public override bool Update()
		{
			switch (m_stateMode)
			{
			case StateMode.SET_WINDOW_TYPE:
				if (rewardType != RewardType.NONE && count > 0)
				{
					SetWindowType();
					m_stateMode = StateMode.REQUEST_LOAD;
				}
				else
				{
					m_stateMode = StateMode.END;
				}
				break;
			case StateMode.REQUEST_LOAD:
				RequestLoadWindow();
				if (m_windowType == WindowType.UNKNOWN)
				{
					m_stateMode = StateMode.END;
				}
				else if (m_windowType == WindowType.ITEM)
				{
					m_stateMode = StateMode.START_WIDOW;
				}
				else
				{
					m_stateMode = StateMode.WAIT_LOAD;
				}
				break;
			case StateMode.WAIT_LOAD:
				if (m_buttonEventResourceLoader != null && m_buttonEventResourceLoader.IsLoaded)
				{
					m_stateMode = StateMode.START_WIDOW;
				}
				break;
			case StateMode.START_WIDOW:
				StartWindow();
				m_stateMode = StateMode.WAIT_END_WIDOW;
				break;
			case StateMode.WAIT_END_WIDOW:
				if (UpdateWindow())
				{
					m_stateMode = StateMode.END;
				}
				break;
			case StateMode.END:
				if (m_buttonEventResourceLoader != null)
				{
					UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
					m_buttonEventResourceLoader = null;
				}
				base.isEnd = true;
				break;
			}
			return base.isEnd;
		}

		public void SetWindowType()
		{
			if (m_serverItem.idType == ServerItem.IdType.CHARA)
			{
				if (m_serverItem.charaType == CharaType.UNKNOWN)
				{
					return;
				}
				ServerPlayerState playerState = ServerInterface.PlayerState;
				if (playerState != null)
				{
					ServerCharacterState serverCharacterState = playerState.CharacterState(m_serverItem.charaType);
					if (serverCharacterState != null && serverCharacterState.star > 0)
					{
						m_windowType = WindowType.CHARA_LEVEL_UP;
					}
					else
					{
						m_windowType = WindowType.CHARA_GET;
					}
				}
			}
			else if (m_serverItem.idType == ServerItem.IdType.CHAO)
			{
				DataTable.ChaoData chaoData = ChaoTable.GetChaoData(m_serverItem.chaoId);
				if (chaoData != null)
				{
					if (chaoData.level > 0)
					{
						m_windowType = WindowType.CHAO_LEVEL_UP;
					}
					else if (chaoData.rarity == DataTable.ChaoData.Rarity.NORMAL || chaoData.rarity == DataTable.ChaoData.Rarity.RARE)
					{
						m_windowType = WindowType.CHAO_GET;
					}
					else
					{
						m_windowType = WindowType.CHAO_GET_SRARE;
					}
				}
			}
			else
			{
				m_windowType = WindowType.ITEM;
			}
		}

		public void RequestLoadWindow()
		{
			string text = string.Empty;
			switch (m_windowType)
			{
			case WindowType.CHARA_GET:
			case WindowType.CHARA_LEVEL_UP:
			case WindowType.CHAO_GET:
			case WindowType.CHAO_GET_SRARE:
			case WindowType.CHAO_LEVEL_UP:
				text = "ChaoWindows";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
				m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync(text, ResourceLoadEndCallback);
			}
		}

		public void StartWindow()
		{
			if (rewardType == RewardType.NONE || count <= 0)
			{
				return;
			}
			GameObject parent = GameObject.Find("UI Root (2D)");
			switch (m_windowType)
			{
			case WindowType.CHARA_GET:
				m_charaGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(parent, "ro_PlayerGetWindowUI");
				if (m_charaGetWindow != null)
				{
					PlayerGetPartsOverlap playerGetPartsOverlap = m_charaGetWindow.gameObject.GetComponent<PlayerGetPartsOverlap>();
					if (playerGetPartsOverlap == null)
					{
						playerGetPartsOverlap = m_charaGetWindow.gameObject.AddComponent<PlayerGetPartsOverlap>();
					}
					playerGetPartsOverlap.Init((int)m_serverItem.id, 100, 0, null, PlayerGetPartsOverlap.IntroType.NO_EGG);
					ChaoGetPartsBase chaoGetParts = playerGetPartsOverlap;
					bool isTutorial = false;
					m_charaGetWindow.PlayStart(chaoGetParts, isTutorial, true);
				}
				break;
			case WindowType.CHARA_LEVEL_UP:
				m_playerMergeWindow = GameObjectUtil.FindChildGameObjectComponent<PlayerMergeWindow>(parent, "player_merge_Window");
				if (m_playerMergeWindow != null)
				{
					m_playerMergeWindow.PlayStart((int)m_serverItem.id, RouletteUtility.AchievementType.PlayerGet);
				}
				break;
			case WindowType.CHAO_GET:
			{
				DataTable.ChaoData chaoData = ChaoTable.GetChaoData(m_serverItem.chaoId);
				m_chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(parent, "chao_get_Window");
				ChaoGetPartsRare component = m_chaoGetWindow.gameObject.GetComponent<ChaoGetPartsRare>();
				ChaoGetPartsNormal chaoGetPartsNormal = m_chaoGetWindow.gameObject.GetComponent<ChaoGetPartsNormal>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				if (chaoGetPartsNormal == null)
				{
					chaoGetPartsNormal = m_chaoGetWindow.gameObject.AddComponent<ChaoGetPartsNormal>();
				}
				chaoGetPartsNormal.Init((int)m_serverItem.id, (int)chaoData.rarity);
				m_chaoGetWindow.PlayStart(chaoGetPartsNormal, false, true);
				break;
			}
			case WindowType.CHAO_GET_SRARE:
			{
				DataTable.ChaoData chaoData3 = ChaoTable.GetChaoData(m_serverItem.chaoId);
				m_chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(parent, "chao_rare_get_Window");
				ChaoGetPartsNormal component2 = m_chaoGetWindow.gameObject.GetComponent<ChaoGetPartsNormal>();
				ChaoGetPartsRare chaoGetPartsRare = m_chaoGetWindow.gameObject.GetComponent<ChaoGetPartsRare>();
				if (component2 != null)
				{
					UnityEngine.Object.Destroy(component2);
				}
				if (chaoGetPartsRare == null)
				{
					chaoGetPartsRare = m_chaoGetWindow.gameObject.AddComponent<ChaoGetPartsRare>();
				}
				chaoGetPartsRare.Init((int)m_serverItem.id, (int)chaoData3.rarity);
				m_chaoGetWindow.PlayStart(chaoGetPartsRare, false, true);
				break;
			}
			case WindowType.CHAO_LEVEL_UP:
			{
				DataTable.ChaoData chaoData2 = ChaoTable.GetChaoData(m_serverItem.chaoId);
				if (m_chaoMergeWindow == null)
				{
					m_chaoMergeWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoMergeWindow>(parent, "chao_merge_Window");
				}
				m_chaoMergeWindow.PlayStart((int)m_serverItem.id, chaoData2.level, (int)chaoData2.rarity);
				break;
			}
			case WindowType.ITEM:
			{
				ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
				if (itemGetWindow != null)
				{
					string text = MileageMapUtility.GetText("gw_item_text", new Dictionary<string, string>
					{
						{
							"{COUNT}",
							HudUtility.GetFormatNumString(count)
						}
					});
					itemGetWindow.Create(new ItemGetWindow.CInfo
					{
						caption = title,
						serverItemId = serverItemId,
						imageCount = text
					});
				}
				break;
			}
			}
			if (!disableSe)
			{
				SoundManager.SePlay("sys_roulette_itemget");
			}
		}

		public bool UpdateWindow()
		{
			bool result = false;
			switch (m_windowType)
			{
			case WindowType.CHARA_GET:
				if (m_charaGetWindow != null && m_charaGetWindow.IsPlayEnd)
				{
					result = true;
					m_charaGetWindow = null;
				}
				break;
			case WindowType.CHARA_LEVEL_UP:
				if (m_playerMergeWindow != null && m_playerMergeWindow.IsPlayEnd)
				{
					result = true;
					m_playerMergeWindow = null;
				}
				break;
			case WindowType.CHAO_GET:
				if (m_chaoGetWindow != null && m_chaoGetWindow.IsPlayEnd)
				{
					result = true;
					m_chaoGetWindow = null;
				}
				break;
			case WindowType.CHAO_GET_SRARE:
				if (m_chaoGetWindow != null && m_chaoGetWindow.IsPlayEnd)
				{
					result = true;
					m_chaoGetWindow = null;
				}
				break;
			case WindowType.CHAO_LEVEL_UP:
				if (m_chaoMergeWindow != null && m_chaoMergeWindow.IsPlayEnd)
				{
					result = true;
					m_chaoMergeWindow = null;
				}
				break;
			case WindowType.ITEM:
			{
				ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
				if (itemGetWindow != null && itemGetWindow.IsEnd)
				{
					itemGetWindow.Reset();
					result = true;
				}
				break;
			}
			default:
				result = true;
				break;
			}
			return result;
		}
	}

	private class GorgeousEvent : BaseEvent
	{
		private bool m_notAllSkip;

		public int windowId
		{
			get;
			private set;
		}

		private bool isNotPlaybackDefaultBgm
		{
			get;
			set;
		}

		public GorgeousEvent(GameObject gameObject, int windowId, bool isNotPlaybackDefaultBgm = false)
			: base(gameObject)
		{
			this.windowId = windowId;
			this.isNotPlaybackDefaultBgm = isNotPlaybackDefaultBgm;
			m_notAllSkip = false;
		}

		public GorgeousEvent(GameObject gameObject, int windowId, bool isNotPlaybackDefaultBgm, bool notAllSkip)
			: base(gameObject)
		{
			this.windowId = windowId;
			this.isNotPlaybackDefaultBgm = isNotPlaybackDefaultBgm;
			m_notAllSkip = notAllSkip;
		}

		public override void Start()
		{
			MileageMapData mileageMapData = MileageMapDataManager.Instance.GetMileageMapData();
			if (mileageMapData == null || windowId >= mileageMapData.window_data.Length)
			{
				return;
			}
			WindowEventData windowEventData = mileageMapData.window_data[windowId];
			GeneralWindow.CInfo.Event[] array = new GeneralWindow.CInfo.Event[windowEventData.body.Length];
			for (int i = 0; i < windowEventData.body.Length; i++)
			{
				WindowBodyData windowBodyData = windowEventData.body[i];
				GeneralWindow.CInfo.Event.FaceWindow[] array2 = new GeneralWindow.CInfo.Event.FaceWindow[windowBodyData.product.Length];
				for (int j = 0; j < windowBodyData.product.Length; j++)
				{
					WindowProductData windowProductData = windowBodyData.product[j];
					array2[j] = new GeneralWindow.CInfo.Event.FaceWindow
					{
						texture = MileageMapUtility.GetFaceTexture(windowProductData.face_id),
						name = ((windowProductData.name_cell_id == null) ? null : MileageMapText.GetName(windowProductData.name_cell_id)),
						effectType = windowProductData.effect,
						animType = windowProductData.anim,
						reverseType = windowProductData.reverse,
						showingType = windowProductData.showing
					};
				}
				array[i] = new GeneralWindow.CInfo.Event
				{
					faceWindows = array2,
					arrowType = windowBodyData.arrow,
					bgmCueName = windowBodyData.bgm,
					seCueName = windowBodyData.se,
					message = MileageMapText.GetText(mileageMapData.scenario.episode, windowBodyData.text_cell_id)
				};
			}
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.buttonType = GeneralWindow.ButtonType.OkNextSkipAllSkip;
			info.caption = MileageMapText.GetText(mileageMapData.scenario.episode, windowEventData.title_cell_id);
			info.events = array;
			info.isNotPlaybackDefaultBgm = isNotPlaybackDefaultBgm;
			GeneralWindow.Create(info);
		}

		public override bool Update()
		{
			if (GeneralWindow.IsButtonPressed)
			{
				if (GeneralWindow.IsAllSkipButtonPressed && !m_notAllSkip)
				{
					base.mileage_page.OnClickAllSkipBtn();
				}
				GeneralWindow.Close();
				base.isEnd = true;
			}
			return base.isEnd;
		}
	}

	private class BalloonEffectEvent : BaseEvent
	{
		private int waypointIndex
		{
			get;
			set;
		}

		private float time
		{
			get;
			set;
		}

		public BalloonEffectEvent(GameObject gameObject, int waypointIndex)
			: base(gameObject)
		{
			this.waypointIndex = waypointIndex;
		}

		public override void Start()
		{
			SetEffectActive(false);
			SetEffectActive(true);
		}

		public override bool Update()
		{
			float time = this.time;
			this.time += Time.deltaTime;
			if ((time < 0.5f && this.time >= 0.5f) || waypointIndex < 1)
			{
				SetEffectActive(false);
				base.isEnd = true;
			}
			return base.isEnd;
		}

		private void SetEffectActive(bool isActive)
		{
			if (waypointIndex >= 1)
			{
				GameObject effectGameObject = base.mileage_page.m_balloonsObjects[waypointIndex - 1].m_effectGameObject;
				if (effectGameObject != null)
				{
					effectGameObject.SetActive(isActive);
				}
			}
		}
	}

	private class BalloonEvent : BaseEvent
	{
		private int eventIndex
		{
			get;
			set;
		}

		private int newFaceId
		{
			get;
			set;
		}

		private int oldFaceId
		{
			get;
			set;
		}

		private float time
		{
			get;
			set;
		}

		public BalloonEvent(GameObject gameObject, int eventIndex, int newFaceId, int oldFaceId)
			: base(gameObject)
		{
			this.eventIndex = eventIndex;
			this.newFaceId = newFaceId;
			this.oldFaceId = oldFaceId;
		}

		public override void Start()
		{
			time = 0f;
		}

		public override bool Update()
		{
			float time = this.time;
			this.time += Time.deltaTime;
			if (time < 0.01f && this.time >= 0.01f)
			{
				SkipMileageProcess();
			}
			if (this.time >= 1f)
			{
				base.isEnd = true;
			}
			return base.isEnd;
		}

		public override void SkipMileageProcess()
		{
			if (base.mileage_page != null)
			{
				base.mileage_page.SetBalloonFaceTexture(eventIndex, newFaceId);
			}
		}
	}

	private class BgmEvent : BaseEvent
	{
		private string m_cueName;

		private float m_waitTime;

		private bool m_playBgmFlag;

		public BgmEvent(GameObject gameObject, string cueName)
			: base(gameObject)
		{
			m_cueName = cueName;
		}

		public override void Start()
		{
			if (string.IsNullOrEmpty(m_cueName))
			{
				SoundManager.BgmFadeOut(0.5f);
				base.isEnd = true;
			}
			else
			{
				SoundManager.BgmFadeOut(0.5f);
				m_playBgmFlag = true;
			}
		}

		public override bool Update()
		{
			if (m_playBgmFlag)
			{
				m_waitTime += Time.deltaTime;
				if (m_waitTime > 0.5f)
				{
					SoundManager.BgmStop();
					SoundManager.BgmPlay(m_cueName);
					base.isEnd = true;
				}
			}
			return base.isEnd;
		}
	}

	private class MapEvent : BaseEvent
	{
		public int episode
		{
			get;
			private set;
		}

		public int chapter
		{
			get;
			private set;
		}

		public bool isNext
		{
			get
			{
				return episode == -1 || chapter == -1;
			}
		}

		public MapEvent(GameObject gameObject, int episode, int chapter)
			: base(gameObject)
		{
			this.episode = episode;
			this.chapter = chapter;
		}

		public MapEvent(GameObject gameObject)
			: base(gameObject)
		{
			episode = -1;
			chapter = -1;
		}

		public override void Start()
		{
			SkipMileageProcess();
			base.isEnd = true;
		}

		public override bool Update()
		{
			return base.isEnd;
		}

		public override void SkipMileageProcess()
		{
			SoundManager.BgmChange("bgm_sys_menu");
			MileageMapDataManager.Instance.SetCurrentData(base.mileage_page.m_mapInfo.m_resultData.m_newMapState.m_episode, base.mileage_page.m_mapInfo.m_resultData.m_newMapState.m_chapter);
			base.mileage_page.m_mapInfo.m_resultData.m_oldMapState.m_episode = base.mileage_page.m_mapInfo.m_resultData.m_newMapState.m_episode;
			base.mileage_page.m_mapInfo.m_resultData.m_oldMapState.m_chapter = base.mileage_page.m_mapInfo.m_resultData.m_newMapState.m_chapter;
			ResultData resultData = base.mileage_page.m_mapInfo.m_resultData;
			base.mileage_page.m_mapInfo = new MapInfo();
			base.mileage_page.m_mapInfo.m_resultData = resultData;
			base.mileage_page.m_mapInfo.isNextMileage = true;
			base.mileage_page.m_mapInfo.ResetMileageIncentive();
			base.mileage_page.SetBG();
			base.mileage_page.SetAll();
			SaveDataManager.Instance.PlayerData.RankOffset = 0;
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		}
	}

	private class HighscoreEvent : BaseEvent
	{
		private enum Phase
		{
			Init,
			InitAskWindow,
			UpdateAskWindow,
			InitSnsFeed,
			UpdateSnsFeed,
			Term
		}

		private Phase m_phase;

		private EasySnsFeed m_easySnsFeed;

		public long highscore
		{
			get;
			private set;
		}

		public HighscoreEvent(GameObject gameObject, long highscore)
			: base(gameObject)
		{
			this.highscore = highscore;
		}

		private void NextPhase()
		{
			m_phase++;
		}

		public override bool Update()
		{
			switch (m_phase)
			{
			case Phase.Init:
				if (IsAskSnsFeed())
				{
					NextPhase();
				}
				else
				{
					base.isEnd = true;
				}
				break;
			case Phase.InitAskWindow:
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.buttonType = GeneralWindow.ButtonType.TweetCancel;
				info.caption = MileageMapUtility.GetText("gw_highscore_caption");
				info.message = MileageMapUtility.GetText("gw_highscore_text");
				GeneralWindow.Create(info);
				NextPhase();
				break;
			}
			case Phase.UpdateAskWindow:
				if (GeneralWindow.IsButtonPressed)
				{
					if (GeneralWindow.IsYesButtonPressed)
					{
						m_phase = Phase.InitSnsFeed;
					}
					else
					{
						m_phase = Phase.Term;
						SetDisableAskSnsFeed();
					}
					GeneralWindow.Close();
				}
				break;
			case Phase.InitSnsFeed:
				m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/menu_Anim/ui_mm_mileage2_page/Anchor_5_MC", MileageMapUtility.GetText("feed_highscore_caption"), MileageMapUtility.GetText("feed_highscore_text", new Dictionary<string, string>
				{
					{
						"{HIGHSCORE}",
						highscore.ToString()
					}
				}));
				NextPhase();
				break;
			case Phase.UpdateSnsFeed:
			{
				EasySnsFeed.Result result = m_easySnsFeed.Update();
				if (result == EasySnsFeed.Result.COMPLETED || result == EasySnsFeed.Result.FAILED)
				{
					NextPhase();
				}
				break;
			}
			case Phase.Term:
				base.isEnd = true;
				break;
			}
			return base.isEnd;
		}
	}

	private class RankingUPEvent : BaseEvent
	{
		private enum StateMode
		{
			WAIT_LOAD,
			START_WIDOW,
			WAIT_END_WIDOW,
			END,
			UNKNOWN
		}

		private StateMode m_stateMode = StateMode.UNKNOWN;

		private ButtonEventResourceLoader m_buttonEventResourceLoader;

		public RankingUPEvent(GameObject gameObject)
			: base(gameObject)
		{
		}

		public override void Start()
		{
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("RankingResultBitWindow", ResourceLoadEndCallback);
			m_stateMode = StateMode.WAIT_LOAD;
		}

		public void ResourceLoadEndCallback()
		{
			if (FontManager.Instance != null)
			{
				FontManager.Instance.ReplaceFont();
			}
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ReplaceAtlasForMenu(true);
			}
			m_stateMode = StateMode.START_WIDOW;
		}

		public override bool Update()
		{
			switch (m_stateMode)
			{
			case StateMode.WAIT_LOAD:
				if (m_buttonEventResourceLoader != null && m_buttonEventResourceLoader.IsLoaded)
				{
					m_stateMode = StateMode.START_WIDOW;
				}
				break;
			case StateMode.START_WIDOW:
				if (RankingUtil.ShowRankingChangeWindow(RankingUtil.RankingMode.ENDLESS))
				{
					m_stateMode = StateMode.WAIT_END_WIDOW;
				}
				else
				{
					m_stateMode = StateMode.END;
				}
				break;
			case StateMode.WAIT_END_WIDOW:
				if (RankingUtil.IsEndRankingChangeWindow())
				{
					m_stateMode = StateMode.END;
				}
				break;
			case StateMode.END:
				if (m_buttonEventResourceLoader != null)
				{
					UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
					m_buttonEventResourceLoader = null;
				}
				base.isEnd = true;
				break;
			}
			return base.isEnd;
		}
	}

	private class RankUpEvent : BaseEvent
	{
		private enum Phase
		{
			Init,
			WaitProduction,
			InitAskWindow,
			UpdateAskWindow,
			InitSnsFeed,
			UpdateSnsFeed,
			Term
		}

		private Phase m_phase;

		private EasySnsFeed m_easySnsFeed;

		private GameObject m_rankUpObj;

		private float m_waitTimer;

		private bool m_askSns;

		public int rank
		{
			get;
			private set;
		}

		public RankUpEvent(GameObject gameObject)
			: base(gameObject)
		{
			rank = (int)SaveDataManager.Instance.PlayerData.Rank;
		}

		private void NextPhase()
		{
			m_phase++;
		}

		public override bool Update()
		{
			switch (m_phase)
			{
			case Phase.Init:
				m_askSns = IsAskSnsFeed();
				m_rankUpObj = GameObjectUtil.FindChildGameObject(base.gameObject.transform.root.gameObject, "Mileage_rankup");
				if (m_rankUpObj != null)
				{
					m_rankUpObj.SetActive(true);
					GameObject gameObject = GameObjectUtil.FindChildGameObject(m_rankUpObj, "eff_set");
					if (gameObject != null)
					{
						gameObject.SetActive(true);
					}
					Animation component = m_rankUpObj.GetComponent<Animation>();
					if (component != null)
					{
						ActiveAnimation.Play(component, "ui_mileage_rankup_Anim", Direction.Forward);
					}
					SoundManager.SePlay("sys_rank_up");
				}
				m_waitTimer = 2.3f;
				NextPhase();
				break;
			case Phase.WaitProduction:
				m_waitTimer -= Time.deltaTime;
				if (m_waitTimer < 0f)
				{
					m_phase = Phase.InitAskWindow;
				}
				break;
			case Phase.InitAskWindow:
			{
				string cellName = (!m_askSns) ? "gw_rankup_text_without_post" : "gw_rankup_text";
				GeneralWindow.ButtonType buttonType = (!m_askSns) ? GeneralWindow.ButtonType.Ok : GeneralWindow.ButtonType.TweetCancel;
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.buttonType = buttonType;
				info.caption = MileageMapUtility.GetText("gw_rankup_caption");
				info.message = MileageMapUtility.GetText(cellName, new Dictionary<string, string>
				{
					{
						"{RANK}",
						(rank + 1).ToString()
					}
				});
				GeneralWindow.Create(info);
				NextPhase();
				break;
			}
			case Phase.UpdateAskWindow:
				if (!GeneralWindow.IsButtonPressed)
				{
					break;
				}
				if (GeneralWindow.IsYesButtonPressed)
				{
					m_phase = Phase.InitSnsFeed;
				}
				else
				{
					m_phase = Phase.Term;
					if (m_askSns)
					{
						SetDisableAskSnsFeed();
					}
				}
				GeneralWindow.Close();
				break;
			case Phase.InitSnsFeed:
				m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/menu_Anim/ui_mm_mileage2_page/Anchor_5_MC", MileageMapUtility.GetText("feed_rankup_caption"), MileageMapUtility.GetText("feed_rankup_text", new Dictionary<string, string>
				{
					{
						"{RANK}",
						(rank + 1).ToString()
					}
				}));
				NextPhase();
				break;
			case Phase.UpdateSnsFeed:
			{
				EasySnsFeed.Result result = m_easySnsFeed.Update();
				if (result == EasySnsFeed.Result.COMPLETED || result == EasySnsFeed.Result.FAILED)
				{
					NextPhase();
				}
				break;
			}
			case Phase.Term:
				if (m_rankUpObj != null)
				{
					UnityEngine.Object.Destroy(m_rankUpObj);
				}
				base.isEnd = true;
				break;
			}
			return base.isEnd;
		}
	}

	private class DailyMissionEvent : BaseEvent
	{
		private enum StateMode
		{
			WAIT_LOAD,
			START_WIDOW,
			WAIT_END_WIDOW,
			END,
			UNKNOWN
		}

		private StateMode m_stateMode = StateMode.UNKNOWN;

		private ButtonEventResourceLoader m_buttonEventResourceLoader;

		private DailyWindowUI m_dailyWindowUI;

		public DailyMissionEvent(GameObject gameObject)
			: base(gameObject)
		{
		}

		public override void Start()
		{
			m_buttonEventResourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
			m_buttonEventResourceLoader.LoadResourceIfNotLoadedAsync("DailyWindowUI", ResourceLoadEndCallback);
			m_stateMode = StateMode.WAIT_LOAD;
		}

		public void ResourceLoadEndCallback()
		{
			if (FontManager.Instance != null)
			{
				FontManager.Instance.ReplaceFont();
			}
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ReplaceAtlasForMenu(true);
			}
			m_stateMode = StateMode.START_WIDOW;
		}

		public override bool Update()
		{
			switch (m_stateMode)
			{
			case StateMode.WAIT_LOAD:
				if (m_buttonEventResourceLoader != null && m_buttonEventResourceLoader.IsLoaded)
				{
					m_stateMode = StateMode.START_WIDOW;
				}
				break;
			case StateMode.START_WIDOW:
			{
				GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
				if (menuAnimUIObject != null)
				{
					m_dailyWindowUI = GameObjectUtil.FindChildGameObjectComponent<DailyWindowUI>(menuAnimUIObject, "DailyWindowUI");
					if (m_dailyWindowUI != null)
					{
						m_dailyWindowUI.gameObject.SetActive(true);
						m_dailyWindowUI.PlayStart();
					}
				}
				m_stateMode = StateMode.WAIT_END_WIDOW;
				break;
			}
			case StateMode.WAIT_END_WIDOW:
				if (m_dailyWindowUI != null && m_dailyWindowUI.IsEnd)
				{
					m_dailyWindowUI.gameObject.SetActive(false);
					m_stateMode = StateMode.END;
				}
				break;
			case StateMode.END:
				if (m_buttonEventResourceLoader != null)
				{
					UnityEngine.Object.Destroy(m_buttonEventResourceLoader);
					m_buttonEventResourceLoader = null;
				}
				base.isEnd = true;
				break;
			}
			return base.isEnd;
		}
	}

	private class MapInfo
	{
		public class Route
		{
			public int routeIndex
			{
				get;
				private set;
			}

			public Route(int routeIndex)
			{
				this.routeIndex = routeIndex;
			}
		}

		public enum TutorialPhase
		{
			NONE,
			NAME_ENTRY,
			AGE_VERIFICATION,
			BEFORE_GAME,
			FIRST_EPISODE,
			FIRST_BOSS,
			FIRST_LOSE_BOSS
		}

		public ResultData m_resultData;

		private int m_waypointIndex;

		private double m_scoreDistanceRaw;

		private double m_scoreDistance;

		private double m_targetScoreDistance;

		private bool m_isBossDestroyed;

		private Route[] m_routes;

		public static int routeScoreDistance
		{
			get
			{
				return MileageMapDataManager.Instance.GetMileageMapData().map_data.event_interval;
			}
		}

		public static int stageScoreDistance
		{
			get
			{
				return routeScoreDistance * 5;
			}
		}

		public int waypointIndex
		{
			get
			{
				return m_waypointIndex;
			}
			set
			{
				m_waypointIndex = value;
			}
		}

		public double scoreDistanceRaw
		{
			get
			{
				return m_scoreDistanceRaw;
			}
			set
			{
				m_scoreDistanceRaw = value;
			}
		}

		public double scoreDistance
		{
			get
			{
				return m_scoreDistance;
			}
			set
			{
				m_scoreDistance = value;
			}
		}

		public double targetScoreDistance
		{
			get
			{
				return m_targetScoreDistance;
			}
			set
			{
				m_targetScoreDistance = value;
			}
		}

		public int nextWaypoint
		{
			get
			{
				return Mathf.Min(waypointIndex + 1, 5);
			}
		}

		public double waypointDistance
		{
			get
			{
				return waypointIndex * routeScoreDistance;
			}
		}

		public double nextWaypointDistance
		{
			get
			{
				return nextWaypoint * routeScoreDistance;
			}
		}

		public bool isNextMileage
		{
			get;
			set;
		}

		public bool isBossStage
		{
			get;
			set;
		}

		public bool isBossDestroyed
		{
			get
			{
				return m_isBossDestroyed;
			}
			set
			{
				m_isBossDestroyed = value;
			}
		}

		public Route[] routes
		{
			get
			{
				return m_routes;
			}
		}

		public long highscore
		{
			get;
			set;
		}

		public TutorialPhase tutorialPhase
		{
			get
			{
				return TutorialPhase.NONE;
			}
		}

		public MapInfo()
		{
			SetRoutesInfo();
			highscore = -1L;
		}

		public double GetRunDistance()
		{
			double result = 0.0;
			if (m_resultData != null && m_resultData.m_oldMapState != null)
			{
				result = scoreDistance - (double)(float)m_resultData.m_oldMapState.m_score;
			}
			return result;
		}

		public bool IsClearMileage()
		{
			if (m_resultData != null && m_resultData.m_oldMapState != null && m_resultData.m_newMapState != null && (m_resultData.m_oldMapState.m_episode != m_resultData.m_newMapState.m_episode || m_resultData.m_oldMapState.m_chapter != m_resultData.m_newMapState.m_chapter))
			{
				return true;
			}
			return false;
		}

		public void SetRoutesInfo()
		{
			m_routes = new Route[5];
			for (int i = 0; i < 5; i++)
			{
				m_routes[i] = new Route(i);
			}
		}

		public void ResetMileageIncentive()
		{
			if (m_resultData != null && m_resultData.m_mileageIncentiveList != null)
			{
				m_resultData.m_mileageIncentiveList.Clear();
			}
		}

		public bool CheckMileageIncentive(int point)
		{
			if (m_resultData != null && m_resultData.m_mileageIncentiveList != null)
			{
				for (int i = 0; i < m_resultData.m_mileageIncentiveList.Count; i++)
				{
					ServerMileageIncentive serverMileageIncentive = m_resultData.m_mileageIncentiveList[i];
					if (serverMileageIncentive.m_type == ServerMileageIncentive.Type.POINT && serverMileageIncentive.m_pointId == point && serverMileageIncentive.m_itemId != 0 && serverMileageIncentive.m_num > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool UpdateFrom(ResultData resultData)
		{
			if (!resultData.m_validResult)
			{
				return false;
			}
			isBossStage = resultData.m_bossStage;
			isBossDestroyed = resultData.m_bossDestroy;
			if (resultData.m_rivalHighScore)
			{
				highscore = resultData.m_highScore;
			}
			waypointIndex = resultData.m_oldMapState.m_point;
			scoreDistanceRaw = resultData.m_totalScore;
			scoreDistance = Mathf.Min(resultData.m_oldMapState.m_score, routeScoreDistance * 5);
			targetScoreDistance = Mathf.Min(resultData.m_newMapState.m_score, routeScoreDistance * 5);
			if (resultData.m_newMapState.m_episode > resultData.m_oldMapState.m_episode || (resultData.m_newMapState.m_episode == resultData.m_oldMapState.m_episode && resultData.m_newMapState.m_chapter > resultData.m_oldMapState.m_chapter))
			{
				targetScoreDistance = routeScoreDistance * 5;
			}
			m_resultData = resultData;
			return true;
		}

		public bool UpdateFrom(MileageMapState mileageMapState)
		{
			waypointIndex = mileageMapState.m_point;
			scoreDistance = Mathf.Min(mileageMapState.m_score, routeScoreDistance * 5);
			targetScoreDistance = Mathf.Min(mileageMapState.m_score, routeScoreDistance * 5);
			return true;
		}
	}

	private class PointTimeLimit
	{
		private BalloonObjects m_balloonObjs;

		private DateTime m_limitTime;

		private bool m_limitFlag;

		private bool m_failedFlag;

		private bool m_incentiveFlag;

		public bool LimitFlag
		{
			get
			{
				return m_limitFlag;
			}
		}

		public bool FailedFlag
		{
			get
			{
				return m_failedFlag;
			}
		}

		public void Reset()
		{
			m_limitFlag = false;
			m_incentiveFlag = false;
			m_failedFlag = false;
			m_balloonObjs = null;
		}

		public void SetupLimit(ServerMileageReward reward, BalloonObjects objs, bool incentiveFlag)
		{
			m_limitFlag = false;
			m_failedFlag = false;
			m_balloonObjs = objs;
			m_incentiveFlag = incentiveFlag;
			if (reward == null || reward.m_limitTime <= 0)
			{
				return;
			}
			TimeSpan value = new TimeSpan(0, 0, 0, reward.m_limitTime, 0);
			m_limitTime = reward.m_startTime;
			m_limitTime = m_limitTime.Add(value);
			m_limitFlag = true;
			if (m_balloonObjs != null)
			{
				if (m_balloonObjs.m_timerFrameObject != null)
				{
					UILabel component = m_balloonObjs.m_timerLimitObject.GetComponent<UILabel>();
					if (component != null)
					{
						component.enabled = !m_incentiveFlag;
					}
				}
				if (m_balloonObjs.m_timerWordObject != null)
				{
					UILabel component2 = m_balloonObjs.m_timerWordObject.GetComponent<UILabel>();
					if (component2 != null)
					{
						component2.enabled = !m_incentiveFlag;
					}
				}
			}
			Update();
		}

		public void Update()
		{
			if (!m_limitFlag || m_incentiveFlag || m_failedFlag || m_balloonObjs == null)
			{
				return;
			}
			TimeSpan restTime = GetRestTime(m_limitTime);
			if (restTime.Seconds < 0)
			{
				if (m_balloonObjs.m_gameObject != null)
				{
					m_balloonObjs.m_gameObject.SetActive(false);
				}
				m_failedFlag = true;
			}
			else if (m_balloonObjs.m_timerFrameObject != null)
			{
				UILabel component = m_balloonObjs.m_timerLimitObject.GetComponent<UILabel>();
				if (component != null)
				{
					component.text = GetRestTimeText(restTime);
				}
			}
		}

		private TimeSpan GetRestTime(DateTime limitTime)
		{
			return limitTime - NetBase.GetCurrentTime();
		}

		private string GetRestTimeText(TimeSpan restTime)
		{
			return string.Format("{0}:{1}:{2}", restTime.Hours.ToString("D2"), restTime.Minutes.ToString("D2"), restTime.Seconds.ToString("D2"));
		}
	}

	private enum PlayerAnimation
	{
		RUN,
		IDLE,
		COUNT
	}

	private enum SuggestedIconType
	{
		TYPE01,
		TYPE02,
		TYPE03,
		NUM
	}

	[Serializable]
	private class RouteObjects
	{
		[SerializeField]
		public UISprite m_lineSprite;

		[SerializeField]
		public GameObject m_lineEffectGameObject;

		[SerializeField]
		public GameObject m_bonusRootGameObject;

		[SerializeField]
		public UISprite m_bonusTypeSprite;

		[SerializeField]
		public UILabel m_bonusValueLabel;

		[SerializeField]
		public TweenPosition m_bonusTweenPosition;
	}

	[Serializable]
	private class BalloonObjects
	{
		[SerializeField]
		public GameObject m_gameObject;

		[SerializeField]
		public UITexture m_texture;

		[SerializeField]
		public GameObject m_effectGameObject;

		[SerializeField]
		public GameObject m_normalFrameObject;

		[SerializeField]
		public GameObject m_timerFrameObject;

		[SerializeField]
		public GameObject m_timerLimitObject;

		[SerializeField]
		public GameObject m_timerWordObject;
	}

	private enum DisplayType
	{
		RANK,
		RSRING,
		RING,
		NUM
	}

	private enum ArraveType
	{
		POINT,
		FINISH,
		POINT_FINISH,
		RUNNIG
	}

	private enum EventSignal
	{
		CLICK_NEXT = 100,
		CLICK_ALL_SKIP
	}

	private const int POINT_COUNT = 6;

	private const int ROUTE_COUNT = 5;

	private const int BALLOON_COUNT = 5;

	private const int BOSS_EVENT_INDEX = 4;

	private const float BALLOON_WAIT = 0.5f;

	private static ui_mm_mileage_page instance;

	[SerializeField]
	private bool m_disabled;

	[SerializeField]
	private float m_playerRunSpeed = 1f;

	[SerializeField]
	private float m_eventWait = 0.5f;

	[SerializeField]
	private GameObject m_playerGameObject;

	[SerializeField]
	private UISprite m_playerSprite;

	[SerializeField]
	private UISpriteAnimation[] m_playerSpriteAnimations = new UISpriteAnimation[2];

	[SerializeField]
	private UISlider m_playerSlider;

	[SerializeField]
	private GameObject m_playerEffGameObject;

	[SerializeField]
	private UISprite[] m_waypointsSprite = new UISprite[6];

	[SerializeField]
	private RouteObjects[] m_routesObjects = new RouteObjects[5];

	[SerializeField]
	private BalloonObjects[] m_balloonsObjects = new BalloonObjects[5];

	[SerializeField]
	private UILabel m_scenarioNumberLabel;

	[SerializeField]
	private UILabel m_titleLabel;

	[SerializeField]
	private UILabel m_distanceLabel;

	[SerializeField]
	private UILabel m_advanceDistanceLabel;

	[SerializeField]
	private GameObject m_advanceDistanceGameObject;

	[SerializeField]
	private GameObject m_patternNextObject;

	[SerializeField]
	private GameObject m_btnNextObject;

	[SerializeField]
	private GameObject m_btnSkipObject;

	[SerializeField]
	private GameObject m_btnPlayObject;

	[SerializeField]
	private UITexture m_stageBGTex;

	private UISlider m_distanceSlider;

	private MapInfo m_mapInfo;

	private Queue<BaseEvent> m_events = new Queue<BaseEvent>();

	private BaseEvent m_event;

	private PointTimeLimit[] m_limitDatas = new PointTimeLimit[5];

	private SoundManager.PlayId m_runSePlayId;

	private UITexture m_bannerTex;

	private GameObject m_bannerObj;

	private GameObject m_eventBannerObj;

	private long m_infoId = -1L;

	private InformationWindow m_infoWindow;

	private bool m_isInit;

	private bool m_isStart;

	private bool m_isNext;

	private bool m_isSkipMileage;

	private bool m_isProduction;

	private bool m_isReachTarget;

	private int[] m_displayOffset = new int[3];

	private TinyFsmBehavior m_fsm_behavior;

	public static ui_mm_mileage_page Instance
	{
		get
		{
			return instance;
		}
	}

	private void AddEvent(BaseEvent baseEvent, int waitType = -1)
	{
		StopRunSe();
		if (m_isSkipMileage)
		{
			baseEvent.SkipMileageProcess();
			return;
		}
		if (waitType < 0)
		{
			m_events.Enqueue(new WaitEvent(base.gameObject, m_eventWait));
		}
		m_events.Enqueue(baseEvent);
		if (waitType > 0)
		{
			m_events.Enqueue(new WaitEvent(base.gameObject, m_eventWait));
		}
	}

	private void AddEventPostWait(BaseEvent baseEvent)
	{
		AddEvent(baseEvent, 1);
	}

	private void AddEventNoWait(BaseEvent baseEvent)
	{
		AddEvent(baseEvent, 0);
	}

	private void AddWaypointEvents()
	{
		AddEventNoWait(new BalloonEffectEvent(base.gameObject, m_mapInfo.waypointIndex));
		MileageMapData mileageMapData = MileageMapDataManager.Instance.GetMileageMapData();
		if (mileageMapData == null)
		{
			return;
		}
		if (m_mapInfo.waypointIndex < 5)
		{
			EventData event_data = mileageMapData.event_data;
			PointEventData pointEventData = event_data.point[m_mapInfo.waypointIndex];
			switch ((m_mapInfo.waypointIndex != 0) ? pointEventData.event_type : 2)
			{
			case 1:
				AddEventPostWait(new SimpleEvent(base.gameObject, pointEventData.reward.serverId, pointEventData.reward.reward_count, MileageMapUtility.GetText("gw_item_caption")));
				break;
			case 2:
				if (pointEventData.window_id > -1)
				{
					AddEventPostWait(new GorgeousEvent(base.gameObject, pointEventData.window_id));
				}
				if (pointEventData.reward.reward_id > -1 || pointEventData.window_id == -1)
				{
					AddEventPostWait(new SimpleEvent(base.gameObject, pointEventData.reward.serverId, pointEventData.reward.reward_count, MileageMapUtility.GetText("gw_item_caption")));
				}
				break;
			}
			return;
		}
		EventData event_data2 = mileageMapData.event_data;
		if (!event_data2.IsBossEvent())
		{
			if (event_data2.point[m_mapInfo.waypointIndex].balloon_on_arrival_face_id != -1)
			{
				AddEventPostWait(new BalloonEvent(base.gameObject, m_mapInfo.waypointIndex - 1, event_data2.point[m_mapInfo.waypointIndex].balloon_on_arrival_face_id, event_data2.point[m_mapInfo.waypointIndex].balloon_face_id));
			}
		}
		else if (event_data2.point[m_mapInfo.waypointIndex].boss.balloon_on_arrival_face_id != -1)
		{
			AddEventPostWait(new BalloonEvent(base.gameObject, m_mapInfo.waypointIndex - 1, event_data2.point[m_mapInfo.waypointIndex].boss.balloon_on_arrival_face_id, event_data2.point[m_mapInfo.waypointIndex].boss.balloon_init_face_id));
		}
		if (m_mapInfo.tutorialPhase != MapInfo.TutorialPhase.FIRST_EPISODE || !event_data2.IsBossEvent() || m_mapInfo.isBossDestroyed)
		{
			int num = (!event_data2.IsBossEvent()) ? event_data2.point[m_mapInfo.waypointIndex].window_id : (m_mapInfo.isBossDestroyed ? event_data2.GetBossEvent().after_window_id : event_data2.GetBossEvent().before_window_id);
			if (num > -1)
			{
				AddEventNoWait(new GorgeousEvent(base.gameObject, num, IsExistsMapClearEvents()));
			}
		}
		AddMapClearEvents();
	}

	private bool IsExistsMapClearEvents()
	{
		bool flag = MileageMapDataManager.Instance.GetMileageMapData().event_data.IsBossEvent() && !m_mapInfo.isBossDestroyed;
		return !flag;
	}

	private void AddMapClearEvents()
	{
		if (!IsExistsMapClearEvents())
		{
			return;
		}
		AddEvent(new BgmEvent(base.gameObject, "jingle_sys_mapclear"));
		bool flag = true;
		RewardData[] reward = MileageMapDataManager.Instance.GetMileageMapData().map_data.reward;
		for (int i = 0; i < reward.Length; i++)
		{
			RewardData rewardData = reward[i];
			if (rewardData.reward_id != -1 && rewardData.reward_count > 0)
			{
				BaseEvent baseEvent = new SimpleEvent(base.gameObject, rewardData.serverId, rewardData.reward_count, MileageMapUtility.GetText("gw_map_bonus_caption"), true);
				if (flag)
				{
					AddEventNoWait(baseEvent);
					flag = false;
				}
				else
				{
					AddEvent(baseEvent);
				}
			}
		}
		if (MileageMapDataManager.Instance.GetMileageMapData().scenario.last_chapter_flag != 0)
		{
			RewardData[] reward2 = MileageMapDataManager.Instance.GetMileageMapData().scenario.reward;
			for (int j = 0; j < reward2.Length; j++)
			{
				RewardData rewardData2 = reward2[j];
				if (rewardData2.reward_id != -1 && rewardData2.reward_count > 0)
				{
					AddEvent(new SimpleEvent(base.gameObject, rewardData2.serverId, rewardData2.reward_count, MileageMapUtility.GetText("gw_scenario_bonus_caption"), true));
				}
			}
		}
		AddEvent(new RankUpEvent(base.gameObject));
		AddEventNoWait(new BgmEvent(base.gameObject, null));
		AddEvent(new MapEvent(base.gameObject));
		int window_id = MileageMapDataManager.Instance.GetMileageMapData().event_data.point[0].window_id;
		AddEvent(new GorgeousEvent(base.gameObject, window_id, false, true));
	}

	private double Minimum(double a, double b)
	{
		if (a < b)
		{
			return a;
		}
		return b;
	}

	private void Awake()
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
		if (m_disabled)
		{
			base.enabled = false;
			return;
		}
		m_fsm_behavior = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (m_fsm_behavior != null)
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateIdle);
			m_fsm_behavior.SetUp(description);
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Pgb_score");
		if (gameObject != null)
		{
			m_distanceSlider = gameObject.GetComponent<UISlider>();
		}
		m_playerSpriteAnimations = m_playerSpriteAnimations[0].gameObject.GetComponents<UISpriteAnimation>();
		HudMenuUtility.SetTagHudMileageMap(base.gameObject);
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			if (instance.m_fsm_behavior != null)
			{
				instance.m_fsm_behavior.ShutDown();
				instance.m_fsm_behavior = null;
			}
			instance = null;
		}
	}

	private void Update()
	{
		if (m_isInit)
		{
			for (int i = 0; i < 5; i++)
			{
				m_limitDatas[i].Update();
			}
		}
	}

	private void ChangeState(TinyFsmState nextState)
	{
		m_fsm_behavior.ChangeState(nextState);
	}

	private TinyFsmState StateIdle(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateHighScoreEvent(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (IsRankingUp())
			{
				AddEvent(new RankingUPEvent(base.gameObject));
			}
			AddEvent(new HighscoreEvent(base.gameObject, m_mapInfo.highscore));
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_event != null && m_event.Update())
			{
				m_event = null;
			}
			if (m_event == null)
			{
				if (m_events.Count > 0)
				{
					m_event = m_events.Dequeue();
					m_event.Start();
				}
				else
				{
					ChangeState(new TinyFsmState(StateEvent));
				}
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		case 101:
			m_isSkipMileage = true;
			m_events.Clear();
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEvent(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			if (m_mapInfo.isBossDestroyed)
			{
				SetBossClearEvent();
				m_isReachTarget = true;
			}
			SetSkipBtnEnable(false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_event != null && m_event.Update())
			{
				m_event = null;
			}
			if (m_event == null)
			{
				bool flag = true;
				if (m_events.Count > 0)
				{
					m_event = m_events.Dequeue();
					m_event.Start();
					flag = false;
				}
				if (flag)
				{
					if (m_isSkipMileage)
					{
						ChangeState(new TinyFsmState(StageAllSkip));
					}
					else if (m_isReachTarget)
					{
						ChangeState(new TinyFsmState(StageDailyMission));
					}
					else
					{
						ChangeState(new TinyFsmState(StageRun));
					}
				}
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		case 101:
			m_isSkipMileage = true;
			m_events.Clear();
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StageRun(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_runSePlayId = SoundManager.SePlay("sys_distance");
			SetRunEffect(true);
			SetSkipBtnEnable(true);
			return TinyFsmState.End();
		case -4:
			SetRunEffect(false);
			SetSkipBtnEnable(false);
			return TinyFsmState.End();
		case 0:
			if (m_isSkipMileage)
			{
				ChangeState(new TinyFsmState(StageAllSkip));
			}
			else
			{
				RunPlayer();
				switch (CheckRun())
				{
				case ArraveType.POINT:
					StopRunSe();
					m_mapInfo.waypointIndex++;
					SetBalloonsView(true);
					AddWaypointEvents();
					SetDistanceDsiplay();
					SetDistanceDsiplayPos();
					SetDisableBolloonView();
					SoundManager.SePlay("sys_arrive");
					ChangeState(new TinyFsmState(StateEvent));
					break;
				case ArraveType.FINISH:
					StopRunSe();
					m_mapInfo.waypointIndex = ServerInterface.MileageMapState.m_point;
					SetDistanceDsiplay();
					SetDistanceDsiplayPos();
					m_advanceDistanceGameObject.SetActive(false);
					SetDisableBolloonView();
					SoundManager.SePlay("sys_arrive");
					m_isReachTarget = true;
					m_isSkipMileage = false;
					ChangeState(new TinyFsmState(StateEvent));
					break;
				case ArraveType.POINT_FINISH:
					StopRunSe();
					m_mapInfo.waypointIndex++;
					SetBalloonsView(true);
					AddWaypointEvents();
					SetDistanceDsiplay();
					SetDistanceDsiplayPos();
					m_advanceDistanceGameObject.SetActive(false);
					SoundManager.SePlay("sys_arrive");
					SetDisableBolloonView();
					m_isReachTarget = true;
					m_isSkipMileage = false;
					ChangeState(new TinyFsmState(StateEvent));
					break;
				}
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		case 100:
			m_isNext = true;
			return TinyFsmState.End();
		case 101:
			m_isSkipMileage = true;
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StageAllSkip(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			SetAllSkip();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_event != null && m_event.Update())
			{
				m_event = null;
			}
			if (m_event == null)
			{
				bool flag = true;
				if (m_events.Count > 0)
				{
					m_event = m_events.Dequeue();
					m_event.Start();
					flag = false;
				}
				if (flag)
				{
					ChangeState(new TinyFsmState(StageDailyMission));
				}
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StageDailyMission(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			m_isSkipMileage = false;
			if (CheckClearDailyMission())
			{
				AddEvent(new DailyMissionEvent(base.gameObject));
				if (m_events.Count > 0)
				{
					m_event = m_events.Dequeue();
					m_event.Start();
				}
			}
			return TinyFsmState.End();
		case -4:
			SetRunEffect(false);
			return TinyFsmState.End();
		case 0:
			if (m_event != null && m_event.Update())
			{
				m_event = null;
			}
			if (m_event == null)
			{
				if (m_events.Count > 0)
				{
					m_event = m_events.Dequeue();
					m_event.Start();
				}
				else
				{
					ChangeState(new TinyFsmState(StateEnd));
				}
			}
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEnd(TinyFsmEvent fsm_event)
	{
		switch (fsm_event.Signal)
		{
		case -3:
			SetEndMileageProduction();
			return TinyFsmState.End();
		case -4:
			m_isProduction = false;
			return TinyFsmState.End();
		case 0:
			ChangeState(new TinyFsmState(StateIdle));
			return TinyFsmState.End();
		case 1:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void Initialize()
	{
		m_isNext = false;
		m_isSkipMileage = false;
		m_mapInfo = new MapInfo();
		for (int i = 0; i < 5; i++)
		{
			m_limitDatas[i] = new PointTimeLimit();
		}
		m_playerSpriteAnimations[0].enabled = false;
		m_playerSpriteAnimations[1].enabled = true;
		m_playerEffGameObject.SetActive(false);
		m_runSePlayId = SoundManager.PlayId.NONE;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_5_MC");
		if (gameObject != null)
		{
			m_eventBannerObj = GameObjectUtil.FindChildGameObject(gameObject, "event_banner");
			if (m_eventBannerObj != null)
			{
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_eventBannerObj, "banner_slot");
				if (gameObject2 != null)
				{
					m_bannerObj = GameObjectUtil.FindChildGameObject(gameObject2, "img_ad_tex");
					if (m_bannerObj != null)
					{
						UIButtonMessage component = m_bannerObj.GetComponent<UIButtonMessage>();
						if (component != null)
						{
							component.enabled = true;
							component.trigger = UIButtonMessage.Trigger.OnClick;
							component.target = base.gameObject;
							component.functionName = "OnEventBannerClicked";
						}
						m_bannerTex = m_bannerObj.GetComponent<UITexture>();
					}
				}
				m_eventBannerObj.SetActive(false);
			}
		}
		m_isInit = true;
	}

	private void RunPlayer()
	{
		if (m_isNext)
		{
			m_mapInfo.scoreDistance = Minimum(m_mapInfo.nextWaypointDistance, m_mapInfo.targetScoreDistance);
			m_isNext = false;
		}
		else if (m_isSkipMileage)
		{
			m_mapInfo.scoreDistance = m_mapInfo.targetScoreDistance;
		}
		else
		{
			m_mapInfo.scoreDistance = Minimum(m_mapInfo.scoreDistance + (double)(m_playerRunSpeed * Time.deltaTime), m_mapInfo.targetScoreDistance);
			m_mapInfo.scoreDistance = Minimum(m_mapInfo.scoreDistance, m_mapInfo.nextWaypointDistance);
		}
		SetPlayerPosition();
		SetDistanceDsiplay();
	}

	private ArraveType CheckRun()
	{
		if (m_mapInfo.scoreDistance == m_mapInfo.nextWaypointDistance && m_mapInfo.waypointIndex < 5)
		{
			if (m_mapInfo.scoreDistance == m_mapInfo.targetScoreDistance)
			{
				return ArraveType.POINT_FINISH;
			}
			return ArraveType.POINT;
		}
		if (m_mapInfo.scoreDistance == m_mapInfo.targetScoreDistance)
		{
			return ArraveType.FINISH;
		}
		return ArraveType.RUNNIG;
	}

	public void SetBG()
	{
		int num = 1;
		if (MileageMapDataManager.Instance != null)
		{
			num = MileageMapDataManager.Instance.MileageStageIndex;
		}
		if (m_stageBGTex != null)
		{
			Texture bGTexture = MileageMapUtility.GetBGTexture();
			if (bGTexture != null)
			{
				m_stageBGTex.mainTexture = bGTexture;
			}
		}
	}

	private void SetRunEffect(bool flag)
	{
		m_playerSpriteAnimations[0].enabled = flag;
		m_playerSpriteAnimations[1].enabled = !flag;
		m_playerEffGameObject.SetActive(flag);
	}

	private void StopRunSe()
	{
		SoundManager.SeStop("sys_distance");
		m_runSePlayId = SoundManager.PlayId.NONE;
	}

	private void SetArraivalFaceTexture()
	{
		PointEventData pointEventData = MileageMapDataManager.Instance.GetMileageMapData().event_data.point[5];
		if (pointEventData != null)
		{
			SetBalloonFaceTexture(4, pointEventData.balloon_on_arrival_face_id);
		}
	}

	private void SetBossClearEvent()
	{
		BossEvent bossEvent = MileageMapDataManager.Instance.GetMileageMapData().event_data.GetBossEvent();
		AddEvent(new BalloonEvent(base.gameObject, 4, bossEvent.balloon_clear_face_id, bossEvent.balloon_on_arrival_face_id));
		AddEvent(new GorgeousEvent(base.gameObject, bossEvent.after_window_id, IsExistsMapClearEvents()));
		AddMapClearEvents();
	}

	private bool CheckClearDailyMission()
	{
		if (m_mapInfo.m_resultData != null)
		{
			return m_mapInfo.m_resultData.m_missionComplete;
		}
		return false;
	}

	private bool IsRankingUp()
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			RankingUtil.RankChange rankingRankChange = SingletonGameObject<RankingManager>.Instance.GetRankingRankChange(RankingUtil.RankingMode.ENDLESS, RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL);
			return rankingRankChange == RankingUtil.RankChange.UP;
		}
		return false;
	}

	private void SetAll()
	{
		SetPlayerPosition();
		SetWaypoints();
		SetRoutes();
		SetTimeLimit();
		SetBalloonsView();
		SetUchanged();
		SetDistanceDsiplay();
		SetDistanceDsiplayPos();
		double a = MileageMapDataManager.Instance.GetMileageMapData().map_data.event_interval;
		double b = m_mapInfo.targetScoreDistance - m_mapInfo.scoreDistance;
		m_playerRunSpeed = (float)Minimum(a, b);
	}

	private void SetPlayerPosition()
	{
		double num = m_mapInfo.scoreDistance / (double)(MapInfo.routeScoreDistance * 5);
		if (m_playerSlider != null)
		{
			m_playerSlider.value = (float)num;
		}
	}

	private void SetWaypoints()
	{
		SetWaypoint(m_waypointsSprite[0], 2);
		EventData event_data = MileageMapDataManager.Instance.GetMileageMapData().event_data;
		if (event_data.point.Length == 6)
		{
			SetWaypoint(m_waypointsSprite[1], event_data.point[1].event_type);
			SetWaypoint(m_waypointsSprite[2], event_data.point[2].event_type);
			SetWaypoint(m_waypointsSprite[3], event_data.point[3].event_type);
			SetWaypoint(m_waypointsSprite[4], event_data.point[4].event_type);
			SetWaypoint(m_waypointsSprite[5], event_data.point[5].event_type);
		}
	}

	private void SetWaypoint(UISprite sprite, int point_id)
	{
		if (sprite != null)
		{
			sprite.spriteName = MileageMapPointDataTable.Instance.GetTextureName(point_id);
		}
	}

	private void SetRoutes()
	{
		for (int i = 0; i < 5; i++)
		{
			if (m_routesObjects[i] != null)
			{
				if (m_routesObjects[i].m_bonusRootGameObject != null)
				{
					m_routesObjects[i].m_bonusRootGameObject.SetActive(false);
				}
				if (m_routesObjects[i].m_lineSprite != null)
				{
					m_routesObjects[i].m_lineSprite.spriteName = "ui_mm_mileage_route_1";
				}
				if (m_routesObjects[i].m_lineEffectGameObject != null)
				{
					m_routesObjects[i].m_lineEffectGameObject.SetActive(false);
				}
			}
		}
	}

	private void SetBalloonsView(bool disable_on_arrival = false)
	{
		for (int i = 0; i < 5; i++)
		{
			SetBalloonView(i, disable_on_arrival);
		}
	}

	private void SetTimeLimit()
	{
		for (int i = 0; i < 5; i++)
		{
			if (m_limitDatas[i] != null)
			{
				m_limitDatas[i].Reset();
			}
		}
		MileageMapDataManager mileageMapDataManager = MileageMapDataManager.Instance;
		int episode = mileageMapDataManager.GetMileageMapData().scenario.episode;
		int chapter = mileageMapDataManager.GetMileageMapData().scenario.chapter;
		for (int j = 0; j < 5; j++)
		{
			int point = j + 1;
			ServerMileageReward mileageReward = mileageMapDataManager.GetMileageReward(episode, chapter, point);
			if (mileageReward != null && m_limitDatas[j] != null)
			{
				bool incentiveFlag = m_mapInfo.CheckMileageIncentive(point);
				m_limitDatas[j].SetupLimit(mileageReward, m_balloonsObjects[j], incentiveFlag);
			}
		}
	}

	private void SetDisableBolloonView()
	{
		for (int i = 0; i < 4; i++)
		{
			int num = (i + 1) * MapInfo.routeScoreDistance;
			if ((double)num <= m_mapInfo.scoreDistance)
			{
				m_balloonsObjects[i].m_gameObject.SetActive(false);
			}
		}
	}

	private void SetBalloonView(int eventIndex, bool disable_on_arrival)
	{
		int num = eventIndex + 1;
		EventData event_data = MileageMapDataManager.Instance.GetMileageMapData().event_data;
		int num2 = (num < 5) ? event_data.point[num].balloon_face_id : ((!event_data.IsBossEvent()) ? ((m_mapInfo.waypointIndex >= 5 && event_data.point[num].balloon_on_arrival_face_id != -1 && !disable_on_arrival) ? event_data.point[num].balloon_on_arrival_face_id : event_data.point[num].balloon_face_id) : ((m_mapInfo.waypointIndex >= 5 && !disable_on_arrival) ? event_data.GetBossEvent().balloon_on_arrival_face_id : event_data.GetBossEvent().balloon_init_face_id));
		int num3 = eventIndex + 1;
		bool flag = num2 >= 0 && m_mapInfo.scoreDistance <= (double)(num3 * MapInfo.routeScoreDistance);
		if (flag && m_limitDatas[eventIndex] != null && m_limitDatas[eventIndex].FailedFlag)
		{
			flag = false;
		}
		m_balloonsObjects[eventIndex].m_gameObject.SetActive(flag);
		if (flag)
		{
			SetBalloonFrame(eventIndex);
			SetBalloonFaceTexture(eventIndex, num2);
		}
	}

	private void SetBalloonFrame(int eventIndex)
	{
		BalloonObjects balloonObjects = m_balloonsObjects[eventIndex];
		bool limitFlag = m_limitDatas[eventIndex].LimitFlag;
		if (balloonObjects.m_normalFrameObject != null)
		{
			balloonObjects.m_normalFrameObject.SetActive(!limitFlag);
		}
		if (balloonObjects.m_timerFrameObject != null)
		{
			balloonObjects.m_timerFrameObject.SetActive(limitFlag);
		}
	}

	private void SetBalloonFaceTexture(int eventIndex, int faceId)
	{
		BalloonObjects balloonObjects = m_balloonsObjects[eventIndex];
		Texture texture = MileageMapUtility.GetFaceTexture(faceId) ?? GeneralWindow.GetDummyTexture(faceId);
		if (balloonObjects.m_texture != null && balloonObjects.m_texture.mainTexture != texture)
		{
			balloonObjects.m_texture.mainTexture = texture;
		}
	}

	private void SetUchanged()
	{
		int episode = MileageMapDataManager.Instance.GetMileageMapData().scenario.episode;
		int chapter = MileageMapDataManager.Instance.GetMileageMapData().scenario.chapter;
		m_scenarioNumberLabel.text = episode.ToString("000") + "-" + chapter;
		string title_cell_id = MileageMapDataManager.Instance.GetMileageMapData().scenario.title_cell_id;
		m_titleLabel.text = MileageMapText.GetText(episode, title_cell_id);
		bool active = m_mapInfo.m_resultData != null;
		if (m_mapInfo.isBossStage)
		{
			active = false;
		}
		else if (m_mapInfo.isNextMileage)
		{
			active = false;
		}
		else if (m_mapInfo.scoreDistanceRaw == 0.0)
		{
			active = false;
		}
		m_advanceDistanceGameObject.SetActive(active);
		m_advanceDistanceLabel.text = HudUtility.GetFormatNumString(m_mapInfo.scoreDistanceRaw);
	}

	private void SetDistanceDsiplay()
	{
		m_distanceLabel.text = HudUtility.GetFormatNumString((double)(MapInfo.routeScoreDistance * m_mapInfo.nextWaypoint) - m_mapInfo.scoreDistance);
		m_advanceDistanceLabel.text = HudUtility.GetFormatNumString(m_mapInfo.scoreDistanceRaw - m_mapInfo.GetRunDistance());
	}

	private void SetDistanceDsiplayPos()
	{
		if (m_mapInfo.waypointIndex == 5)
		{
			if (m_distanceSlider != null)
			{
				m_distanceSlider.gameObject.SetActive(false);
			}
			return;
		}
		float num = (float)(m_mapInfo.waypointIndex + 1) * 0.2f;
		if (num > 1f)
		{
			num = 1f;
		}
		if (m_distanceSlider != null)
		{
			m_distanceSlider.gameObject.SetActive(true);
			m_distanceSlider.value = num;
		}
	}

	private void SetPlayBtnImg()
	{
		if (!(m_btnPlayObject != null))
		{
			return;
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_btnPlayObject, "img_word_play");
		if (uISprite != null)
		{
			if (MileageMapUtility.IsBossStage())
			{
				uISprite.spriteName = "ui_mm_btn_word_play_boss";
			}
			else
			{
				uISprite.spriteName = "ui_mm_btn_word_play";
			}
		}
		int stageIndex = 1;
		if (MileageMapDataManager.Instance != null)
		{
			stageIndex = MileageMapDataManager.Instance.MileageStageIndex;
		}
		CharacterAttribute[] characterAttribute = MileageMapUtility.GetCharacterAttribute(stageIndex);
		if (characterAttribute != null)
		{
			for (int i = 0; i < 3; i++)
			{
				string name = "img_icon_type_" + (i + 1);
				UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_btnPlayObject, name);
				if (!(uISprite2 != null))
				{
					continue;
				}
				if (i < characterAttribute.Length)
				{
					switch (characterAttribute[i])
					{
					case CharacterAttribute.SPEED:
						uISprite2.enabled = true;
						uISprite2.spriteName = "ui_chao_set_type_icon_speed";
						break;
					case CharacterAttribute.FLY:
						uISprite2.enabled = true;
						uISprite2.spriteName = "ui_chao_set_type_icon_fly";
						break;
					case CharacterAttribute.POWER:
						uISprite2.enabled = true;
						uISprite2.spriteName = "ui_chao_set_type_icon_power";
						break;
					default:
						uISprite2.enabled = false;
						break;
					}
				}
				else
				{
					uISprite2.enabled = false;
				}
			}
		}
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_btnPlayObject, "img_next_map");
		if (uISprite3 != null)
		{
			uISprite3.spriteName = "ui_mm_map_thumb_w" + stageIndex.ToString("00") + "a";
		}
	}

	private void SetEndMileageProduction()
	{
		ResetRewindOffsetToSaveData();
		if (m_patternNextObject != null)
		{
			m_patternNextObject.SetActive(false);
		}
		HudMenuUtility.SendEnableShopButton(true);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		SetBannerCollider(true);
		if (IsChangeDataVersion() || IsTutorialEvent())
		{
			HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.EPISODE_BACK);
		}
	}

	private bool IsChangeDataVersion()
	{
		if (ServerInterface.LoginState != null && (ServerInterface.LoginState.IsChangeDataVersion || ServerInterface.LoginState.IsChangeAssetsVersion))
		{
			return true;
		}
		return false;
	}

	private bool IsTutorialEvent()
	{
		if (HudMenuUtility.IsTutorialCharaLevelUp() || HudMenuUtility.IsRouletteTutorial() || HudMenuUtility.IsRecommendReviewTutorial())
		{
			return true;
		}
		return false;
	}

	private void SetSkipBtnEnable(bool flag)
	{
		if (m_btnNextObject != null)
		{
			BoxCollider component = m_btnNextObject.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.isTrigger = !flag;
			}
		}
		if (m_btnSkipObject != null)
		{
			UIImageButton component2 = m_btnSkipObject.GetComponent<UIImageButton>();
			if (component2 != null)
			{
				component2.isEnabled = flag;
			}
		}
	}

	private void SetPlanelAlha()
	{
		UIPanel component = base.gameObject.GetComponent<UIPanel>();
		if (component != null)
		{
			component.alpha = 1f;
		}
	}

	public void StartMileageMapProduction()
	{
		StartCoroutine(DelayStart());
	}

	private void SetAllSkip()
	{
		StopRunSe();
		ResetRewindOffsetToSaveData();
		m_isSkipMileage = false;
		if (m_mapInfo.IsClearMileage())
		{
			SetMileageClearDisplayOffset_FromResultData(m_mapInfo.m_resultData);
			if (m_mapInfo.m_resultData != null && !m_mapInfo.m_resultData.m_bossDestroy)
			{
				m_mapInfo.scoreDistance = m_mapInfo.targetScoreDistance;
				m_mapInfo.waypointIndex = 5;
				SetBalloonsView(true);
				SetArraivalFaceTexture();
				SetDistanceDsiplay();
				SetDistanceDsiplayPos();
				SetPlayerPosition();
			}
			AddMapClearEvents();
		}
		else
		{
			SoundManager.BgmChange("bgm_sys_menu");
			MileageMapDataManager.Instance.SetCurrentData(ServerInterface.MileageMapState.m_episode, ServerInterface.MileageMapState.m_chapter);
			MileageMapState mileageMapState = new MileageMapState();
			mileageMapState.m_episode = ServerInterface.MileageMapState.m_episode;
			mileageMapState.m_chapter = ServerInterface.MileageMapState.m_chapter;
			mileageMapState.m_point = ServerInterface.MileageMapState.m_point;
			mileageMapState.m_score = ServerInterface.MileageMapState.m_stageTotalScore;
			m_mapInfo.UpdateFrom(mileageMapState);
			SetPlayBtnImg();
			SetBG();
			SetAll();
		}
		m_advanceDistanceGameObject.SetActive(false);
		if (!m_isReachTarget)
		{
			SoundManager.SePlay("sys_arrive");
		}
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	public IEnumerator DelayStart()
	{
		int waite_frame = 5;
		while (waite_frame > 0)
		{
			waite_frame--;
			yield return null;
		}
		if (m_mapInfo.highscore >= 0)
		{
			ChangeState(new TinyFsmState(StateHighScoreEvent));
		}
		else
		{
			ChangeState(new TinyFsmState(StateEvent));
		}
	}

	private void OnStartMileage()
	{
		m_isStart = true;
		if (m_isInit && m_isProduction)
		{
			StartMileageMapProduction();
		}
		SetEventBanner();
	}

	private void OnEndMileage()
	{
	}

	public void OnUpdateMileageMapDisplay()
	{
		if (!m_isInit)
		{
			Initialize();
			MileageMapState mileageMapState = new MileageMapState();
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				mileageMapState.m_episode = ServerInterface.MileageMapState.m_episode;
				mileageMapState.m_chapter = ServerInterface.MileageMapState.m_chapter;
				mileageMapState.m_point = ServerInterface.MileageMapState.m_point;
				mileageMapState.m_score = ServerInterface.MileageMapState.m_stageTotalScore;
			}
			else
			{
				mileageMapState.m_episode = MileageMapDataManager.Instance.GetMileageMapData().scenario.episode;
				mileageMapState.m_chapter = MileageMapDataManager.Instance.GetMileageMapData().scenario.chapter;
				mileageMapState.m_point = 0;
				mileageMapState.m_score = 0L;
			}
			m_mapInfo.UpdateFrom(mileageMapState);
			SetPlayBtnImg();
			SetBG();
			SetSkipBtnEnable(false);
			SetAll();
			SetRunEffect(false);
			SetPlanelAlha();
			SetEventBanner();
		}
	}

	public void OnPrepareMileageMapProduction(ResultData resultData)
	{
		if (m_isInit)
		{
			return;
		}
		if (resultData != null && resultData.m_quickMode)
		{
			OnUpdateMileageMapDisplay();
			return;
		}
		Initialize();
		SetDisplayOffset_FromResultData(resultData);
		m_mapInfo.UpdateFrom(resultData);
		SetPlayBtnImg();
		SetBG();
		SetAll();
		if (m_patternNextObject != null)
		{
			m_patternNextObject.SetActive(true);
		}
		SetRunEffect(false);
		SetPlanelAlha();
		HudMenuUtility.SendEnableShopButton(false);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		m_isProduction = true;
		BackKeyManager.AddMileageCallBack(base.gameObject);
		SetSkipBtnEnable(false);
		if (m_isStart)
		{
			StartMileageMapProduction();
		}
		SetEventBanner();
		SetBannerCollider(false);
	}

	private void OnClickNextBtn()
	{
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	public void OnClickAllSkipBtn()
	{
		if (m_fsm_behavior != null)
		{
			TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
			m_fsm_behavior.Dispatch(signal);
		}
	}

	private void OnClosedCharaGetWindow()
	{
		SimpleEvent simpleEvent = m_event as SimpleEvent;
		if (simpleEvent != null)
		{
			simpleEvent.isEnd = true;
		}
	}

	private void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (m_isProduction)
		{
			OnClickNextBtn();
			if (msg != null)
			{
				msg.StaySequence();
			}
		}
	}

	private void SetDisplayOffset_FromResultData(ResultData resultData)
	{
		if (m_displayOffset != null && m_displayOffset.Length == 3)
		{
			for (int i = 0; i < 3; i++)
			{
				m_displayOffset[i] = 0;
			}
			if (MileageMapUtility.IsRankUp_FromResultData(resultData))
			{
				m_displayOffset[0] = 1;
			}
			m_displayOffset[1] = MileageMapUtility.GetDisplayOffset_FromResultData(resultData, ServerItem.Id.RSRING);
			m_displayOffset[2] = MileageMapUtility.GetDisplayOffset_FromResultData(resultData, ServerItem.Id.RING);
		}
	}

	private void SetMileageClearDisplayOffset_FromResultData(ResultData resultData)
	{
		if (m_displayOffset != null && m_displayOffset.Length == 3)
		{
			for (int i = 0; i < 3; i++)
			{
				m_displayOffset[i] = 0;
			}
			if (MileageMapUtility.IsRankUp_FromResultData(resultData))
			{
				m_displayOffset[0] = 1;
			}
			m_displayOffset[1] = MileageMapUtility.GetMileageClearDisplayOffset_FromResultData(resultData, ServerItem.Id.RSRING);
			m_displayOffset[2] = MileageMapUtility.GetMileageClearDisplayOffset_FromResultData(resultData, ServerItem.Id.RING);
		}
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (!(saveDataManager == null))
		{
			PlayerData playerData = saveDataManager.PlayerData;
			if (playerData != null)
			{
				playerData.RankOffset = -m_displayOffset[0];
			}
			ItemData itemData = saveDataManager.ItemData;
			if (itemData != null)
			{
				itemData.RedRingCountOffset = -m_displayOffset[1];
				itemData.RingCountOffset = -m_displayOffset[2];
			}
		}
	}

	private void ResetRewindOffsetToSaveData()
	{
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (!(saveDataManager == null))
		{
			PlayerData playerData = saveDataManager.PlayerData;
			if (playerData != null)
			{
				playerData.RankOffset = 0;
			}
			ItemData itemData = saveDataManager.ItemData;
			if (itemData != null)
			{
				itemData.RingCountOffset = 0;
				itemData.RedRingCountOffset = 0;
			}
		}
	}

	private void SetEventBanner()
	{
		if (!m_isInit)
		{
			return;
		}
		bool flag = false;
		if (EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.BGM)
		{
			EventStageData stageData = EventManager.Instance.GetStageData();
			if (stageData != null)
			{
				flag = stageData.IsEndlessModeBGM();
			}
		}
		if (flag)
		{
			if (ServerInterface.NoticeInfo == null || ServerInterface.NoticeInfo.m_eventItems == null)
			{
				return;
			}
			foreach (NetNoticeItem eventItem in ServerInterface.NoticeInfo.m_eventItems)
			{
				if (m_infoId != eventItem.Id)
				{
					m_infoId = eventItem.Id;
					if (InformationImageManager.Instance != null)
					{
						InformationImageManager.Instance.Load(eventItem.ImageId, true, OnLoadCallback);
					}
					if (m_eventBannerObj != null)
					{
						m_eventBannerObj.SetActive(true);
					}
					break;
				}
			}
		}
		else
		{
			if (m_eventBannerObj != null)
			{
				m_eventBannerObj.SetActive(false);
			}
			if (m_bannerTex != null && m_bannerTex.mainTexture != null)
			{
				m_bannerTex.mainTexture = null;
			}
		}
	}

	public void OnLoadCallback(Texture2D texture)
	{
		if (m_bannerTex != null && texture != null)
		{
			m_bannerTex.mainTexture = texture;
		}
	}

	private void OnEventBannerClicked()
	{
		m_infoWindow = base.gameObject.GetComponent<InformationWindow>();
		if (m_infoWindow == null)
		{
			m_infoWindow = base.gameObject.AddComponent<InformationWindow>();
		}
		if (!(m_infoWindow != null) || ServerInterface.NoticeInfo == null || ServerInterface.NoticeInfo.m_eventItems == null)
		{
			return;
		}
		foreach (NetNoticeItem eventItem in ServerInterface.NoticeInfo.m_eventItems)
		{
			if (m_infoId == eventItem.Id)
			{
				InformationWindow.Information info = default(InformationWindow.Information);
				info.pattern = InformationWindow.ButtonPattern.OK;
				info.imageId = eventItem.ImageId;
				info.caption = TextUtility.GetCommonText("Informaion", "announcement");
				GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
				if (cameraUIObject != null)
				{
					GameObject newsWindowObj = GameObjectUtil.FindChildGameObject(cameraUIObject, "NewsWindow");
					m_infoWindow.Create(info, newsWindowObj);
					base.enabled = true;
					SoundManager.SePlay("sys_menu_decide");
				}
				break;
			}
		}
	}

	private void SetBannerCollider(bool on)
	{
		if (m_bannerObj != null)
		{
			BoxCollider component = m_bannerObj.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.isTrigger = !on;
			}
		}
	}
}
