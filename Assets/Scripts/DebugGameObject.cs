using DataTable;
using Message;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Text;
using UnityEngine;

public class DebugGameObject : SingletonGameObject<DebugGameObject>
{
	public enum GUI_RECT_ANCHOR
	{
		CENTER,
		CENTER_LEFT,
		CENTER_RIGHT,
		CENTER_TOP,
		CENTER_BOTTOM,
		LEFT_TOP,
		LEFT_BOTTOM,
		RIGHT_TOP,
		RIGHT_BOTTOM
	}

	public enum LOADING_SUFFIXE
	{
		DEBUG_JA,
		DEBUG_DE,
		DEBUG_EN,
		DEBUG_ES,
		DEBUG_FR,
		DEBUG_IT,
		DEBUG_KO,
		DEBUG_PT,
		DEBUG_RU,
		DEBUG_ZH,
		DEBUG_ZHJ,
		NONE
	}

	public enum MOUSE_R_CLICK
	{
		PAUSED,
		ATLAS,
		HI_SPEED,
		LOW_SPEED,
		NONE
	}

	public enum DEBUG_CHECK_TYPE
	{
		DRAW_CALL,
		LOAD_ATLAS,
		NONE
	}

	private enum DEBUG_PLAY_TYPE
	{
		ITEM,
		COLOR,
		BOSS_DESTORY,
		NUM,
		NONE
	}

	private enum DEBUG_MENU_TYPE
	{
		ITEM,
		MILEAGE,
		RANKING,
		CHAO_TEX_RELEASE,
		DAILY_BATTLE,
		DEBUG_GUI_OFF,
		NUM,
		NONE
	}

	private enum DEBUG_MENU_RANKING_CATEGORY
	{
		CACHE,
		CHANGE_TEST,
		NUM
	}

	private enum DEBUG_MENU_ITEM_CATEGORY
	{
		ITEM,
		OTOMO,
		CHARACTER,
		NUM
	}

	private delegate void NetworkRequestSuccessCallback();

	private delegate void NetworkRequestFailedCallback(ServerInterface.StatusCode statusCode);

	private const float DEBUG_DISP_SIZE_W = 800f;

	private const float DEBUG_DISP_SIZE_H = 450f;

	private const float DEBUG_POP_TIME = 10f;

	private const float DEBUG_POP_MOVE_RATE = 0.05f;

	private const int DEBUG_MILEAGE_MAX = 50;

	public const float UPDATE_PERIOD_TIME = 2f;

	private const RankingUtil.RankingScoreType DEFAULT_RAIVAL_SCORE_TYPE = RankingUtil.RankingScoreType.HIGH_SCORE;

	private const RankingUtil.RankingScoreType DEFAULT_SP_SCORE_TYPE = RankingUtil.RankingScoreType.TOTAL_SCORE;

	[SerializeField]
	[Header("デバック用のオブジェクトです。不要な場合は要削除")]
	public bool m_debugActive = true;

	[SerializeField]
	public bool m_debugNetworkActive = true;

	[SerializeField]
	public bool m_debugTestBtn = true;

	[SerializeField]
	public DEBUG_CHECK_TYPE m_debugCheckType = DEBUG_CHECK_TYPE.NONE;

	[SerializeField]
	public MOUSE_R_CLICK m_mouseRightClick = MOUSE_R_CLICK.NONE;

	[SerializeField]
	public bool m_mouseWheelUseSpeed;

	[SerializeField]
	public ItemType m_mouseWheelUseItem = ItemType.UNKNOWN;

	[SerializeField]
	public float m_currentTimeScale = 1f;

	[SerializeField]
	private bool m_titleFirstLogin;

	[SerializeField]
	private LOADING_SUFFIXE m_titleLoadingSuffixe = LOADING_SUFFIXE.NONE;

	[SerializeField]
	private string m_suffixeBaseText = "title_load_index_{LANG}.html";

	[SerializeField]
	[Header("ランキング関連")]
	private bool m_rankingDebug;

	[SerializeField]
	private RankingUtil.RankingRankerType m_targetRankingRankerType = RankingUtil.RankingRankerType.RIVAL;

	[SerializeField]
	private RankingUtil.RankingScoreType m_rivalRankingScoreType;

	[SerializeField]
	private RankingUtil.RankingScoreType m_spRankingScoreType = RankingUtil.RankingScoreType.TOTAL_SCORE;

	[SerializeField]
	private bool m_rankingLog;

	[SerializeField]
	[Header("現在の順位情報取得")]
	private int m_currentScore = -1;

	[SerializeField]
	private int m_currentScoreEvent = -1;

	[SerializeField]
	[Header("通信関連")]
	private int m_msgMax;

	[Header("暗号化フラグ")]
	[SerializeField]
	private bool m_crypt = true;

	[Header("更新頻度(数値表示確認用)")]
	[SerializeField]
	private int m_updateCost;

	[SerializeField]
	private List<string> m_updateCostList;

	[Header("ルーレット関連")]
	[SerializeField]
	private RouletteCategory m_rouletteDummyCategory;

	[SerializeField]
	private bool m_rouletteTutorial;

	[SerializeField]
	private float m_rouletteConectTime = 1f;

	[SerializeField]
	private List<ServerItem.Id> m_rouletteDataPremium;

	[SerializeField]
	private List<ServerItem.Id> m_rouletteDataSpecial;

	[SerializeField]
	private List<ServerItem.Id> m_rouletteDataItem;

	[SerializeField]
	private List<ServerItem.Id> m_rouletteDataRaid;

	[SerializeField]
	private List<ServerItem.Id> m_rouletteDataDefault;

	[Header("ダミー通信障害発生率(%)")]
	[SerializeField]
	private int m_rouletteDummyError;

	[SerializeField]
	[Header("デバック用キャンペーン設定")]
	private List<Constants.Campaign.emType> m_debugCampaign;

	private List<string> m_debugDummy;

	private float m_mouseRightClickDelayTime;

	private Dictionary<string, int> m_updCost;

	private GameObject m_rouletteCallback;

	private MsgGetWheelOptionsGeneralSucceed m_rouletteGetMsg;

	private MsgCommitWheelSpinGeneralSucceed m_rouletteCommitMsg;

	private float m_debugRouletteTime;

	private float m_debugGetRouletteTime;

	private bool m_debugRouletteConectError = true;

	private List<string> m_keys;

	private Dictionary<string, int> m_currentUpdCost;

	private float m_time;

	private float m_wheelInputDelay;

	private Camera m_camera;

	private float m_cameraSizeRate = 1f;

	private bool m_debugPlay;

	private DEBUG_PLAY_TYPE m_debugPlayType = DEBUG_PLAY_TYPE.NONE;

	private bool m_debugScore;

	private float m_debugScoreDelay;

	private string m_debugScoreText = string.Empty;

	private bool m_debugMenu;

	private DEBUG_MENU_TYPE m_debugMenuType = DEBUG_MENU_TYPE.NONE;

	private DEBUG_MENU_RANKING_CATEGORY m_debugMenuRankingCateg;

	private RankingUtil.RankingRankerType m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;

	private DEBUG_MENU_ITEM_CATEGORY m_debugMenuItemSelect;

	private Dictionary<DEBUG_MENU_ITEM_CATEGORY, List<int>> m_debugMenuItemList;

	private int m_debugMenuItemNum = 1;

	private int m_debugMenuItemPage;

	private List<DataTable.ChaoData> m_debugMenuOtomoList;

	private List<CharacterDataNameInfo.Info> m_debugMenuCharaList = new List<CharacterDataNameInfo.Info>();

	private int m_debugMenuMileageEpi = 2;

	private int m_debugMenuMileageCha = 1;

	private int m_debugMenuRankingCurrentRank;

	private int m_debugMenuRankingCurrentDummyRank;

	private int m_debugMenuRankingCurrentLegMax;

	private bool m_debugCheckFlag;

	private Dictionary<string, Dictionary<string, List<UIDrawCall>>> m_debugDrawCallList;

	private string m_debugDrawCallPanelCurrent = string.Empty;

	private string m_debugDrawCallMatCurrent = string.Empty;

	private List<UIAtlas> m_debugAtlasList;

	private List<UIAtlas> m_debugAtlasLangList;

	private string m_debugAtlasLangCode = "---";

	private Dictionary<long, string> m_debugPop;

	private Dictionary<long, Rect> m_debugPopRect;

	private Dictionary<long, float> m_debugPopTime;

	private long m_debugPopCount;

	private bool m_debugDeck;

	private int m_debugDeckCount;

	private int m_debugDeckCurrentIndex;

	private List<string> m_debugDeckList;

	private bool m_debugCharaData;

	private int m_debugCharaDataCount;

	private ServerPlayerState.CHARA_SORT m_debugCharaDataSort;

	private List<string> m_debugCharaDataList;

	private Dictionary<CharaType, ServerCharacterState> m_debugCharaList;

	private string m_debugCharaDataInfo = string.Empty;

	private ServerCharacterState m_debugCharaDataState;

	private Dictionary<ServerItem.Id, int> m_debugCharaDataBuyCost;

	private bool m_debugCharaDataBuy;

	private ResourceSceneLoader m_debugSceneLoader;

	private int m_debugGiftItemId;

	private GameObject m_debugGiftCallback;

	public bool firstLogin
	{
		get
		{
			return m_titleFirstLogin;
		}
	}

	public LOADING_SUFFIXE loadingSuffixe
	{
		get
		{
			return m_titleLoadingSuffixe;
		}
	}

	public string suffixeBaseText
	{
		get
		{
			return m_suffixeBaseText;
		}
	}

	public bool debugActive
	{
		get
		{
			return m_debugActive;
		}
	}

	public bool rankingDebug
	{
		get
		{
			return m_rankingDebug;
		}
	}

	public RankingUtil.RankingRankerType targetRankingRankerType
	{
		get
		{
			if (!rankingDebug)
			{
				return RankingUtil.RankingRankerType.RIVAL;
			}
			return m_targetRankingRankerType;
		}
	}

	public RankingUtil.RankingScoreType rivalRankingScoreType
	{
		get
		{
			if (!rankingDebug)
			{
				return RankingUtil.RankingScoreType.HIGH_SCORE;
			}
			return m_rivalRankingScoreType;
		}
	}

	public RankingUtil.RankingScoreType spRankingScoreType
	{
		get
		{
			if (!rankingDebug)
			{
				return RankingUtil.RankingScoreType.TOTAL_SCORE;
			}
			return m_spRankingScoreType;
		}
	}

	public RouletteCategory rouletteDummyCategory
	{
		get
		{
			return m_rouletteDummyCategory;
		}
	}

	public bool rouletteTutorial
	{
		get
		{
			return m_rouletteTutorial;
		}
	}

	public bool crypt
	{
		get
		{
			return m_crypt;
		}
	}

	public bool rankingLog
	{
		get
		{
			return m_rankingLog;
		}
	}

	public List<Constants.Campaign.emType> debugCampaign
	{
		get
		{
			return m_debugCampaign;
		}
	}

	private ResourceSceneLoader debugSceneLoader
	{
		get
		{
			if (m_debugSceneLoader == null)
			{
				GameObject gameObject = new GameObject("DebugTextLoader");
				if (gameObject != null)
				{
					m_debugSceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
				}
			}
			return m_debugSceneLoader;
		}
	}

	public void PopLog(string log, float xRate, float yRate, GUI_RECT_ANCHOR anchor = GUI_RECT_ANCHOR.CENTER)
	{
	}

	public bool CheckMsgText(string msg)
	{
		if (!string.IsNullOrEmpty(msg) && m_msgMax < msg.Length)
		{
			m_msgMax = msg.Length;
			return true;
		}
		return false;
	}

	public void CheckUpdate(string name = "")
	{
		if (m_currentUpdCost == null)
		{
			m_currentUpdCost = new Dictionary<string, int>();
			m_currentUpdCost.Add("TOTAL_COST", 1);
			m_keys = new List<string>();
			m_keys.Add("TOTAL_COST");
			if (!string.IsNullOrEmpty(name) && !m_currentUpdCost.ContainsKey(name))
			{
				m_currentUpdCost.Add(name, 1);
				m_keys.Add(name);
			}
		}
		else
		{
			if (m_currentUpdCost.ContainsKey("TOTAL_COST"))
			{
				Dictionary<string, int> currentUpdCost;
				Dictionary<string, int> dictionary = currentUpdCost = m_currentUpdCost;
				string key;
				string key2 = key = "TOTAL_COST";
				int num = currentUpdCost[key];
				dictionary[key2] = num + 1;
			}
			if (!string.IsNullOrEmpty(name))
			{
				if (!m_currentUpdCost.ContainsKey(name))
				{
					m_currentUpdCost.Add(name, 1);
					m_keys.Add(name);
				}
				else
				{
					Dictionary<string, int> currentUpdCost2;
					Dictionary<string, int> dictionary2 = currentUpdCost2 = m_currentUpdCost;
					string key;
					string key3 = key = name;
					int num = currentUpdCost2[key];
					dictionary2[key3] = num + 1;
				}
			}
		}
		if (m_updCost == null)
		{
			m_updCost = new Dictionary<string, int>();
			m_updCost.Add("TOTAL_COST", 0);
			if (!string.IsNullOrEmpty(name) && !m_updCost.ContainsKey(name))
			{
				m_updCost.Add(name, 0);
			}
		}
		else if (!string.IsNullOrEmpty(name) && !m_updCost.ContainsKey(name))
		{
			m_updCost.Add(name, 0);
		}
	}

	private void OnGUI()
	{
		base.enabled = false;
	}

	private void GuiDummyObject()
	{
		if (Application.loadedLevelName.IndexOf("title") != -1 || !m_debugTestBtn)
		{
			return;
		}
		int num = 0;
		if (m_debugDummy != null && m_debugDummy.Count > 0)
		{
			num = m_debugDummy.Count;
		}
		Rect position = CreateGuiRect(new Rect(0f, 90f, 70f, 25f), GUI_RECT_ANCHOR.RIGHT_TOP);
		if (GUI.Button(position, "Stress Test\n" + num))
		{
			if (m_debugDummy == null)
			{
				m_debugDummy = new List<string>();
			}
			for (int i = 0; i < 500; i++)
			{
				m_debugDummy.Add(string.Empty + m_debugDummy.Count);
			}
		}
		if (m_debugDummy == null || m_debugDummy.Count <= 0)
		{
			return;
		}
		int num2 = 0;
		foreach (string item in m_debugDummy)
		{
			Rect position2 = CreateGuiRect(new Rect(3f * (float)(num2 % 250), 4f * (float)(num2 / 250), 2f, 3f));
			GUI.Box(position2, item);
			num2++;
		}
	}

	private void GuiPop()
	{
		if (m_debugPop == null || m_debugPop.Count <= 0 || !(m_camera != null))
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		long[] array = new long[m_debugPop.Count];
		long num = -1L;
		float num2 = m_camera.pixelRect.yMax * 0.05f;
		m_debugPop.Keys.CopyTo(array, 0);
		if (array.Length > 0)
		{
			long[] array2 = array;
			foreach (long num3 in array2)
			{
				string text = m_debugPop[num3] + "\n count:" + num3;
				Rect position = m_debugPopRect[num3];
				float num4 = (10f - m_debugPopTime[num3]) / 10f;
				num4 = 1f - (num4 - 1f) * (num4 - 1f);
				position.y -= num2 * num4;
				GUI.Box(position, text);
				Dictionary<long, float> debugPopTime;
				Dictionary<long, float> dictionary = debugPopTime = m_debugPopTime;
				long key;
				long key2 = key = num3;
				float num5 = debugPopTime[key];
				dictionary[key2] = num5 - deltaTime;
				if (m_debugPopTime[num3] <= 0f)
				{
					num = num3;
				}
			}
		}
		if (num >= 0)
		{
			m_debugPop.Remove(num);
			m_debugPopRect.Remove(num);
			m_debugPopTime.Remove(num);
		}
	}

	private void GuiCheck()
	{
		if (Application.loadedLevelName.IndexOf("title") != -1 || m_debugCheckType == DEBUG_CHECK_TYPE.NONE)
		{
			return;
		}
		if (m_debugCheckFlag)
		{
			float num = 350f;
			float num2 = 700f;
			Rect rect = CreateGuiRect(new Rect(0f, 0f, num2 + 10f, num + 10f), GUI_RECT_ANCHOR.CENTER);
			GUI.Box(rect, string.Empty);
			if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 200f, 20f), GUI_RECT_ANCHOR.CENTER_TOP), "close"))
			{
				switch (m_debugCheckType)
				{
				case DEBUG_CHECK_TYPE.DRAW_CALL:
					if (string.IsNullOrEmpty(m_debugDrawCallPanelCurrent) && string.IsNullOrEmpty(m_debugDrawCallMatCurrent))
					{
						m_debugCheckFlag = !m_debugCheckFlag;
						m_debugDrawCallPanelCurrent = string.Empty;
						m_debugDrawCallMatCurrent = string.Empty;
						if (m_debugAtlasList != null)
						{
							m_debugAtlasList.Clear();
						}
						if (m_debugAtlasLangList != null)
						{
							m_debugAtlasLangList.Clear();
						}
						HudMenuUtility.SetConnectAlertSimpleUI(false);
					}
					else if (!string.IsNullOrEmpty(m_debugDrawCallMatCurrent))
					{
						m_debugDrawCallMatCurrent = string.Empty;
					}
					else if (!string.IsNullOrEmpty(m_debugDrawCallPanelCurrent))
					{
						m_debugDrawCallPanelCurrent = string.Empty;
						m_debugDrawCallMatCurrent = string.Empty;
					}
					else
					{
						m_debugCheckFlag = !m_debugCheckFlag;
						m_debugDrawCallPanelCurrent = string.Empty;
						m_debugDrawCallMatCurrent = string.Empty;
						HudMenuUtility.SetConnectAlertSimpleUI(false);
					}
					break;
				case DEBUG_CHECK_TYPE.LOAD_ATLAS:
					m_debugCheckFlag = !m_debugCheckFlag;
					m_debugDrawCallPanelCurrent = string.Empty;
					m_debugDrawCallMatCurrent = string.Empty;
					if (m_debugAtlasList != null)
					{
						m_debugAtlasList.Clear();
					}
					HudMenuUtility.SetConnectAlertSimpleUI(false);
					break;
				}
			}
			switch (m_debugCheckType)
			{
			case DEBUG_CHECK_TYPE.DRAW_CALL:
				GuiDrawCall2D(rect, num2, num);
				break;
			case DEBUG_CHECK_TYPE.LOAD_ATLAS:
				GuiLoadAtlas(rect, num2, num);
				break;
			}
		}
		else if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 200f, 20f), GUI_RECT_ANCHOR.CENTER_TOP), string.Empty + m_debugCheckType))
		{
			m_debugCheckFlag = !m_debugCheckFlag;
			m_debugDrawCallPanelCurrent = string.Empty;
			m_debugDrawCallMatCurrent = string.Empty;
			switch (m_debugCheckType)
			{
			case DEBUG_CHECK_TYPE.DRAW_CALL:
				CheckDrawCall2D();
				break;
			case DEBUG_CHECK_TYPE.LOAD_ATLAS:
				CheckLoadAtlas();
				break;
			}
			HudMenuUtility.SetConnectAlertSimpleUI(true);
		}
	}

	private void GuiDeckData()
	{
		if (Application.loadedLevelName.IndexOf("title") != -1 || Application.loadedLevelName.IndexOf("playingstage") != -1 || Application.loadedLevelName.IndexOf("MainMenu") == -1 || DeckViewWindow.isActive)
		{
			return;
		}
		if (m_debugDeck)
		{
			float height = 100f;
			float width = 550f;
			Rect rect = CreateGuiRect(new Rect(0f, 40f, width, height), GUI_RECT_ANCHOR.CENTER_TOP);
			GUI.Box(rect, string.Empty);
			if (m_debugDeckList != null && m_debugDeckList.Count > 0)
			{
				float height2 = 0.7f;
				float num = 19f / 140f;
				float num2 = 0.142857149f;
				float height3 = 0.2f;
				float width2 = num;
				float num3 = (num2 - num) * 0.5f;
				for (int i = 0; i < m_debugDeckList.Count; i++)
				{
					GUI.Box(CreateGuiRectInRate(rect, new Rect(num3 + num2 * (float)i, 0.03f, num, height2)), m_debugDeckList[i]);
					if (GUI.Button(CreateGuiRectInRate(rect, new Rect(num3 + num2 * (float)i, -0.03f, width2, height3), GUI_RECT_ANCHOR.LEFT_BOTTOM), "reset"))
					{
						DeckUtil.DeckReset(i);
						DebugCreateDeckList();
					}
				}
				if (GUI.Button(CreateGuiRectInRate(rect, new Rect(0f - num3, 0.03f, num, 0.94f), GUI_RECT_ANCHOR.RIGHT_TOP), "reload\n\n current:" + m_debugDeckCurrentIndex))
				{
					DebugCreateDeckList();
				}
			}
			if (GUI.Button(CreateGuiRect(new Rect(0f, 10f, 60f, 25f), GUI_RECT_ANCHOR.CENTER_TOP), "close"))
			{
				m_debugDeck = !m_debugDeck;
			}
			if (m_debugDeckCount > 300)
			{
				DebugCreateDeckList();
			}
			m_debugDeckCount++;
		}
		else if (m_debugCharaData)
		{
			float height4 = 350f;
			float width3 = 600f;
			int num4 = 6;
			Rect rect2 = CreateGuiRect(new Rect(0f, 40f, width3, height4), GUI_RECT_ANCHOR.CENTER_TOP);
			GUI.Box(rect2, string.Empty);
			if (m_debugCharaDataList != null && m_debugCharaDataList.Count > 0)
			{
				float height5 = 0.19f;
				float num5 = 1f / (float)num4 * 0.95f;
				float num6 = 1f / (float)num4;
				float num7 = (num6 - num5) * 0.5f;
				for (int j = 0; j < m_debugCharaDataList.Count; j++)
				{
					int num8 = j % num4;
					int num9 = j / num4;
					if (GUI.Button(CreateGuiRectInRate(rect2, new Rect(num7 + num6 * (float)num8, 0.02f + 0.2f * (float)num9, num5, height5)), m_debugCharaDataList[j]) && !m_debugCharaDataBuy)
					{
						DebugCreateCharaInfo(j);
					}
				}
				if (GUI.Button(CreateGuiRectInRate(rect2, new Rect(0.01f, -0.12f, 0.175f, 0.1f), GUI_RECT_ANCHOR.LEFT_BOTTOM), "sort\n" + m_debugCharaDataSort))
				{
					int num10 = 3;
					int debugCharaDataSort = (int)m_debugCharaDataSort;
					debugCharaDataSort = (debugCharaDataSort + 1) % num10;
					DebugCreateCharaList((ServerPlayerState.CHARA_SORT)debugCharaDataSort);
				}
				if (GUI.Button(CreateGuiRectInRate(rect2, new Rect(0.01f, -0.01f, 0.175f, 0.1f), GUI_RECT_ANCHOR.LEFT_BOTTOM), "offset\n" + m_debugCharaDataCount))
				{
					DebugCreateCharaList(m_debugCharaDataSort);
				}
				GUI.Box(CreateGuiRectInRate(rect2, new Rect(-0.16f, -0.01f, 0.65f, 0.21f), GUI_RECT_ANCHOR.RIGHT_BOTTOM), m_debugCharaDataInfo);
				if (m_debugCharaDataBuyCost != null && m_debugCharaDataBuyCost.Count > 0 && !m_debugCharaDataBuy)
				{
					int count = m_debugCharaDataBuyCost.Count;
					Dictionary<ServerItem.Id, int>.KeyCollection keys = m_debugCharaDataBuyCost.Keys;
					float num11 = 0.2f / (float)count;
					float num12 = 0.02f / (float)count;
					float num13 = (num11 + num12) * -1f;
					int num14 = 0;
					foreach (ServerItem.Id item in keys)
					{
						if (GUI.Button(CreateGuiRectInRate(rect2, new Rect(-0.005f, 0f - num12 + num13 * (float)num14, 0.145f, num11), GUI_RECT_ANCHOR.RIGHT_BOTTOM), string.Concat(string.Empty, item, "\n", m_debugCharaDataBuyCost[item])))
						{
							DebugBuyChara(item, m_debugCharaDataBuyCost[item]);
						}
						num14++;
					}
				}
			}
			if (GUI.Button(CreateGuiRect(new Rect(0f, 10f, 60f, 25f), GUI_RECT_ANCHOR.CENTER_TOP), "close"))
			{
				m_debugCharaData = !m_debugCharaData;
			}
			if (GUI.Button(CreateGuiRect(new Rect(200f, 10f, 100f, 25f), GUI_RECT_ANCHOR.CENTER_TOP), "roulette\n chara picup") && RouletteManager.Instance != null)
			{
				RouletteManager.Instance.RequestPicupCharaList();
			}
		}
		else if (GUI.Button(CreateGuiRect(new Rect(-100f, 5f, 60f, 25f), GUI_RECT_ANCHOR.CENTER_TOP), "deck data"))
		{
			m_debugDeck = !m_debugDeck;
			DebugCreateDeckList();
		}
	}

	private void DebugCreateDeckList()
	{
		m_debugDeckCount = 0;
		if (m_debugDeckList != null)
		{
			m_debugDeckList.Clear();
		}
		m_debugDeckList = new List<string>();
		m_debugDeckCurrentIndex = DeckUtil.GetDeckCurrentStockIndex();
		for (int i = 0; i < 6; i++)
		{
			CharaType currentMainCharaType = CharaType.UNKNOWN;
			CharaType currentSubCharaType = CharaType.UNKNOWN;
			int currentMainId = -1;
			int currentSubId = -1;
			DeckUtil.DeckSetLoad(i, ref currentMainCharaType, ref currentSubCharaType, ref currentMainId, ref currentSubId);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Empty + currentMainCharaType);
			stringBuilder.Append("\n" + currentSubCharaType);
			stringBuilder.Append("\n" + currentMainId);
			stringBuilder.Append("\n" + currentSubId);
			m_debugDeckList.Add(stringBuilder.ToString());
		}
	}

	private void DebugBuyChara(ServerItem.Id itemId, int cost)
	{
		if (m_debugCharaDataState == null)
		{
			return;
		}
		long itemCount = GeneralUtil.GetItemCount(itemId);
		if (itemCount >= cost)
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerUnlockedCharacter(item: new ServerItem(itemId), charaType: m_debugCharaDataState.charaType, callbackObject: base.gameObject);
				m_debugCharaDataBuy = true;
			}
		}
		else
		{
			Debug.Log(string.Concat("DebugBuyChara error  ", itemId, ":", itemCount));
		}
	}

	private void ServerUnlockedCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		if (m_debugCharaDataList != null)
		{
			m_debugCharaDataList.Clear();
		}
		m_debugCharaDataList = new List<string>();
		CharaType charaType = CharaType.UNKNOWN;
		if (m_debugCharaDataState != null)
		{
			charaType = m_debugCharaDataState.charaType;
		}
		ServerPlayerState playerState = ServerInterface.PlayerState;
		m_debugCharaList = playerState.GetCharacterStateList(m_debugCharaDataSort, false, m_debugCharaDataCount);
		int idx = 0;
		int num = 0;
		Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = m_debugCharaList.Keys;
		foreach (CharaType item in keys)
		{
			if (charaType == item)
			{
				idx = num;
			}
			ServerCharacterState serverCharacterState = m_debugCharaList[item];
			CharacterDataNameInfo.Info charaInfo = m_debugCharaList[item].charaInfo;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Empty + charaInfo.m_name);
			stringBuilder.Append("\n Lv:" + serverCharacterState.Level);
			stringBuilder.Append(" ☆" + serverCharacterState.star);
			stringBuilder.Append("\n" + charaInfo.m_attribute);
			stringBuilder.Append("\n" + charaInfo.m_teamAttribute);
			stringBuilder.Append("\n IsUnlock:" + serverCharacterState.IsUnlocked);
			m_debugCharaDataList.Add(stringBuilder.ToString());
			num++;
		}
		DebugCreateCharaInfo(idx);
		m_debugCharaDataBuy = false;
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void DebugCreateCharaInfo(int idx)
	{
		ServerPlayerState playerState = ServerInterface.PlayerState;
		Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = m_debugCharaList.Keys;
		int num = 0;
		foreach (CharaType item in keys)
		{
			if (idx == num)
			{
				ServerCharacterState serverCharacterState = m_debugCharaList[item];
				CharacterDataNameInfo.Info charaInfo = m_debugCharaList[item].charaInfo;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Empty + charaInfo.m_name);
				stringBuilder.Append(" Lv:" + serverCharacterState.Level);
				stringBuilder.Append(" ☆" + serverCharacterState.star);
				stringBuilder.Append("  " + charaInfo.m_attribute);
				stringBuilder.Append("  " + charaInfo.m_teamAttribute);
				stringBuilder.Append("  IsUnlock:" + serverCharacterState.IsUnlocked);
				stringBuilder.Append("\n Condition:" + serverCharacterState.Condition);
				stringBuilder.Append(" Status:" + serverCharacterState.Status);
				stringBuilder.Append(" OldStatus:" + serverCharacterState.OldStatus);
				stringBuilder.Append("\n Exp:" + serverCharacterState.Exp);
				stringBuilder.Append(" OldExp:" + serverCharacterState.OldExp);
				stringBuilder.Append(" priceRing:" + serverCharacterState.priceNumRings);
				stringBuilder.Append(" priceRSR:" + serverCharacterState.priceNumRedRings);
				stringBuilder.Append("\n teamAttribute:" + charaInfo.m_teamAttribute);
				stringBuilder.Append(" teamAttributeCategory:" + charaInfo.m_teamAttributeCategory);
				stringBuilder.Append(string.Concat("\n mainBonus:", charaInfo.m_mainAttributeBonus, " [", charaInfo.GetTeamAttributeValue(charaInfo.m_mainAttributeBonus), "]"));
				stringBuilder.Append(string.Concat(" subBonus:", charaInfo.m_subAttributeBonus, " [", charaInfo.GetTeamAttributeValue(charaInfo.m_subAttributeBonus), "]"));
				stringBuilder.Append("\n IsRoulette:" + serverCharacterState.IsRoulette);
				m_debugCharaDataInfo = stringBuilder.ToString();
				m_debugCharaDataState = serverCharacterState;
				m_debugCharaDataBuyCost = m_debugCharaDataState.GetBuyCostItemList();
				break;
			}
			num++;
		}
	}

	private void DebugCreateCharaList(ServerPlayerState.CHARA_SORT sort)
	{
		if (m_debugCharaDataList != null)
		{
			m_debugCharaDataList.Clear();
		}
		m_debugCharaDataList = new List<string>();
		if (sort != m_debugCharaDataSort)
		{
			m_debugCharaDataCount = 0;
		}
		else
		{
			m_debugCharaDataCount++;
		}
		m_debugCharaDataInfo = string.Empty;
		m_debugCharaDataSort = sort;
		ServerPlayerState playerState = ServerInterface.PlayerState;
		m_debugCharaList = playerState.GetCharacterStateList(m_debugCharaDataSort, false, m_debugCharaDataCount);
		Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = m_debugCharaList.Keys;
		foreach (CharaType item in keys)
		{
			ServerCharacterState serverCharacterState = m_debugCharaList[item];
			CharacterDataNameInfo.Info charaInfo = m_debugCharaList[item].charaInfo;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Empty + charaInfo.m_name);
			stringBuilder.Append("\n Lv:" + serverCharacterState.Level);
			stringBuilder.Append(" ☆" + serverCharacterState.star);
			stringBuilder.Append("\n" + charaInfo.m_attribute);
			stringBuilder.Append("\n" + charaInfo.m_teamAttribute);
			stringBuilder.Append("\n IsUnlock:" + serverCharacterState.IsUnlocked);
			m_debugCharaDataList.Add(stringBuilder.ToString());
		}
	}

	private void GuiEtc()
	{
		if (Application.loadedLevelName.IndexOf("title") != -1 || !(m_camera != null))
		{
			return;
		}
		if (Time.timeScale < 5f)
		{
			if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 65f, 20f), GUI_RECT_ANCHOR.RIGHT_BOTTOM), "hi speed"))
			{
				Time.timeScale = 5f;
			}
		}
		else if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 65f, 20f), GUI_RECT_ANCHOR.RIGHT_BOTTOM), "reset"))
		{
			Time.timeScale = 1f;
		}
	}

	private void GuiMainMenu()
	{
		if (Application.loadedLevelName.IndexOf("title") == -1 && Application.loadedLevelName.IndexOf("playingstage") == -1 && Application.loadedLevelName.IndexOf("MainMenu") != -1 && RouletteManager.IsRouletteEnabled())
		{
			m_debugMenu = false;
			m_debugMenuType = DEBUG_MENU_TYPE.NONE;
			m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
			m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
			m_debugMenuItemNum = 1;
			m_debugMenuItemPage = 0;
			m_debugMenuMileageEpi = 2;
			m_debugMenuMileageCha = 1;
		}
	}

	private void DebugPlayingCurrentScoreCheck()
	{
		m_debugScoreText = string.Empty;
		bool flag = false;
		if (EventManager.Instance != null && EventManager.Instance.IsSpecialStage())
		{
			flag = true;
		}
		StageScoreManager instance = StageScoreManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		long num = 0L;
		num = ((!flag) ? instance.GetRealtimeScore() : instance.SpecialCrystal);
		if (num > 0)
		{
			RankingManager instance2 = SingletonGameObject<RankingManager>.Instance;
			if (instance2 != null)
			{
				bool isHighScore = false;
				long nextRankScore = 0L;
				long prveRankScore = 0L;
				int nextRank = 0;
				int currentHighScoreRank = RankingManager.GetCurrentHighScoreRank(RankingUtil.RankingMode.ENDLESS, flag, ref num, out isHighScore, out nextRankScore, out prveRankScore, out nextRank);
				m_debugScoreText = "isSpStage:" + flag + " score:" + num;
				string debugScoreText = m_debugScoreText;
				m_debugScoreText = debugScoreText + "\n rank:" + currentHighScoreRank + " nextRank:" + nextRank + " isHighScore:" + isHighScore;
				debugScoreText = m_debugScoreText;
				m_debugScoreText = debugScoreText + "\n nextScore:" + nextRankScore + " prveScore:" + prveRankScore;
			}
		}
	}

	private void GuiPlayingStageBtn()
	{
		if (m_debugScore)
		{
			float num = 60f;
			float num2 = 220f;
			Rect rect = CreateGuiRect(new Rect(0f, -20f, num2 + 10f, num + 10f), GUI_RECT_ANCHOR.CENTER_BOTTOM);
			if (m_debugScoreDelay <= 0f)
			{
				DebugPlayingCurrentScoreCheck();
				m_debugScoreDelay = 0.2f;
			}
			else
			{
				m_debugScoreDelay -= Time.deltaTime;
			}
			GUI.Box(rect, m_debugScoreText);
			if (GUI.Button(CreateGuiRectInRate(rect, new Rect(0f, -0.01f, 0.95f, 0.32f), GUI_RECT_ANCHOR.CENTER_BOTTOM), "close"))
			{
				m_debugScore = !m_debugScore;
				m_debugScoreDelay = 0f;
			}
		}
		else if (GUI.Button(CreateGuiRect(new Rect(0f, -80f, 50f, 40f), GUI_RECT_ANCHOR.RIGHT_BOTTOM), "debug gui\n   off"))
		{
			AllocationStatus.hide = true;
			SingletonGameObject<DebugGameObject>.Remove();
		}
		if (m_debugPlay)
		{
			float num3 = 300f;
			float width = 120f;
			Rect rect2 = CreateGuiRect(new Rect(0f, 0f, width, num3 + 10f), GUI_RECT_ANCHOR.LEFT_BOTTOM);
			GUI.Box(rect2, string.Empty);
			if (m_debugPlayType == DEBUG_PLAY_TYPE.NONE)
			{
				int num4 = 3;
				float num5 = (num3 - 40f) / num3 / (float)num4;
				float width2 = 0.95f;
				for (int i = 0; i < num4; i++)
				{
					if (GUI.Button(CreateGuiRectInRate(rect2, new Rect(0f, 0.02f + num5 * (float)i, width2, num5 * 0.9f), GUI_RECT_ANCHOR.CENTER_TOP), string.Empty + (DEBUG_PLAY_TYPE)i))
					{
						m_debugPlayType = (DEBUG_PLAY_TYPE)i;
						if (m_debugPlayType == DEBUG_PLAY_TYPE.BOSS_DESTORY)
						{
							m_debugPlayType = DEBUG_PLAY_TYPE.NONE;
							MsgBossEnd value = new MsgBossEnd(true);
							GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnBossEnd", value, SendMessageOptions.DontRequireReceiver);
							MsgUseEquipItem value2 = new MsgUseEquipItem();
							GameObjectUtil.SendMessageFindGameObject("StageItemManager", "OnUseEquipItem", value2, SendMessageOptions.DontRequireReceiver);
						}
					}
				}
				if (GUI.Button(CreateGuiRectInRate(rect2, new Rect(0f, -0.02f, 0.833f, 0.1f), GUI_RECT_ANCHOR.CENTER_BOTTOM), "close"))
				{
					m_debugPlay = false;
					m_debugPlayType = DEBUG_PLAY_TYPE.NONE;
				}
			}
			else
			{
				switch (m_debugPlayType)
				{
				case DEBUG_PLAY_TYPE.ITEM:
					GuiPlayingStageBtnItem(rect2);
					break;
				case DEBUG_PLAY_TYPE.COLOR:
					GuiPlayingStageBtnColor(rect2);
					break;
				}
			}
		}
		else if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 50f, 40f), GUI_RECT_ANCHOR.LEFT_BOTTOM), "debug\n menu"))
		{
			m_debugPlay = !m_debugPlay;
			m_debugPlayType = DEBUG_PLAY_TYPE.NONE;
		}
	}

	private void DebugUseItem(ItemType useItem)
	{
		switch (useItem)
		{
		case ItemType.INVINCIBLE:
		case ItemType.BARRIER:
		case ItemType.MAGNET:
		case ItemType.TRAMPOLINE:
		case ItemType.COMBO:
		case ItemType.LASER:
		case ItemType.DRILL:
		case ItemType.ASTEROID:
			Debug.Log("debug use:" + useItem);
			break;
		default:
		{
			int min = 0;
			int num = 7;
			int num2 = UnityEngine.Random.Range(min, num + 1);
			useItem = (ItemType)num2;
			Debug.Log("debug use:" + useItem);
			break;
		}
		}
		if (useItem != ItemType.UNKNOWN)
		{
			StageItemManager x = UnityEngine.Object.FindObjectOfType<StageItemManager>();
			if (x != null)
			{
				GameObjectUtil.SendMessageFindGameObject("StageItemManager", "OnAddItem", new MsgAddItemToManager(useItem), SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void GuiPlayingStageBtnItem(Rect target)
	{
		int num = 0;
		int num2 = 4;
		int num3 = num2 - num + 1;
		float height = 0.85f / (float)num3 * 0.95f;
		float num4 = 0.85f / (float)num3;
		int num5 = 0;
		for (int i = num; i <= num2; i++)
		{
			ItemType itemType = (ItemType)i;
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(0f, 0.02f + (float)num5 * num4, 0.95f, height), GUI_RECT_ANCHOR.CENTER_TOP), string.Empty + itemType))
			{
				DebugUseItem(itemType);
			}
			num5++;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(0f, -0.02f, 0.9f, 0.1f), GUI_RECT_ANCHOR.CENTER_BOTTOM), "close"))
		{
			m_debugPlayType = DEBUG_PLAY_TYPE.NONE;
		}
	}

	private void GuiPlayingStageBtnColor(Rect target)
	{
		int num = 5;
		int num2 = 7;
		int num3 = num2 - num + 1;
		float height = 0.85f / (float)num3 * 0.95f;
		float num4 = 0.85f / (float)num3;
		int num5 = 0;
		for (int i = num; i <= num2; i++)
		{
			ItemType itemType = (ItemType)i;
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(0f, 0.02f + (float)num5 * num4, 0.95f, height), GUI_RECT_ANCHOR.CENTER_TOP), string.Empty + itemType))
			{
				DebugUseItem(itemType);
			}
			num5++;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(0f, -0.02f, 0.9f, 0.1f), GUI_RECT_ANCHOR.CENTER_BOTTOM), "close"))
		{
			m_debugPlayType = DEBUG_PLAY_TYPE.NONE;
		}
	}

	private void GuiMainMenuBtn()
	{
		if (m_debugMenu)
		{
			float num = 250f;
			float num2 = 250f;
			Rect rect = CreateGuiRect(new Rect(-100f, 0f, num2, num), GUI_RECT_ANCHOR.CENTER_RIGHT);
			GUI.Box(rect, string.Empty);
			if (m_debugMenuType == DEBUG_MENU_TYPE.NONE)
			{
				int num3 = 6;
				float num4 = num / (float)num3;
				float num5 = num2 - 10f;
				float num6 = num4 - 10f;
				float num7 = num5;
				float num8 = num * -0.5f - num4 * 0.5f;
				m_debugMenuRankingCurrentRank = 0;
				for (int i = 0; i < num3; i++)
				{
					DEBUG_MENU_TYPE dEBUG_MENU_TYPE = (DEBUG_MENU_TYPE)i;
					if (dEBUG_MENU_TYPE == DEBUG_MENU_TYPE.DEBUG_GUI_OFF)
					{
						num8 += num4 * 0.125f;
						num6 *= 0.75f;
						num7 *= 0.75f;
					}
					if (!GUI.Button(CreateGuiRectInRate(rect, new Rect(0f, (num8 + num4 * (float)(i + 1)) / num, num7 / num2, num6 / num), GUI_RECT_ANCHOR.CENTER), string.Empty + dEBUG_MENU_TYPE))
					{
						continue;
					}
					m_debugMenuType = dEBUG_MENU_TYPE;
					m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
					m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
					m_debugMenuItemNum = 1;
					m_debugMenuItemPage = 0;
					m_debugMenuMileageEpi = 2;
					m_debugMenuMileageCha = 1;
					if (m_debugMenuType == DEBUG_MENU_TYPE.ITEM)
					{
						if (m_debugMenuItemList != null)
						{
							m_debugMenuItemList.Clear();
						}
						m_debugMenuCharaList = new List<CharacterDataNameInfo.Info>();
						m_debugMenuItemList = new Dictionary<DEBUG_MENU_ITEM_CATEGORY, List<int>>();
						List<int> list = new List<int>();
						List<int> list2 = new List<int>();
						List<int> list3 = new List<int>();
						list.Add(120000);
						list.Add(120001);
						list.Add(120002);
						list.Add(120003);
						list.Add(120004);
						list.Add(120005);
						list.Add(120006);
						list.Add(120007);
						list.Add(220000);
						list.Add(900000);
						list.Add(910000);
						list.Add(920000);
						list.Add(240000);
						list.Add(230000);
						ServerPlayerState playerState = ServerInterface.PlayerState;
						int num9 = 0;
						DataTable.ChaoData[] dataTable = ChaoTable.GetDataTable();
						m_debugMenuOtomoList = new List<DataTable.ChaoData>();
						List<int> list4 = new List<int>();
						List<ServerChaoState> list5 = null;
						if (playerState != null && playerState.ChaoStates != null)
						{
							list5 = playerState.ChaoStates;
						}
						if (dataTable != null && dataTable.Length > 0)
						{
							DataTable.ChaoData[] array = dataTable;
							foreach (DataTable.ChaoData chaoData in array)
							{
								if (chaoData.rarity != DataTable.ChaoData.Rarity.NONE && list5 != null && list5.Count > 0)
								{
									m_debugMenuOtomoList.Add(chaoData);
								}
							}
						}
						if (m_debugMenuOtomoList != null && m_debugMenuOtomoList.Count > 0)
						{
							num9 = m_debugMenuOtomoList.Count;
							for (int k = 0; k < num9; k++)
							{
								int item = m_debugMenuOtomoList[k].id + 400000;
								if (!list4.Contains(item))
								{
									list2.Add(item);
								}
							}
						}
						if (list4.Count > 0)
						{
							foreach (int item2 in list4)
							{
								list2.Add(item2 + 100000);
							}
						}
						if (playerState != null)
						{
							num9 = 29;
							for (int l = 0; l < num9; l++)
							{
								CharaType charaType = (CharaType)l;
								ServerCharacterState serverCharacterState = playerState.CharacterState(charaType);
								if (serverCharacterState != null)
								{
									CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(charaType);
									if (dataByID != null)
									{
										list3.Add(dataByID.m_serverID);
										m_debugMenuCharaList.Add(dataByID);
									}
								}
							}
						}
						m_debugMenuItemList.Add(DEBUG_MENU_ITEM_CATEGORY.ITEM, list);
						m_debugMenuItemList.Add(DEBUG_MENU_ITEM_CATEGORY.OTOMO, list2);
						m_debugMenuItemList.Add(DEBUG_MENU_ITEM_CATEGORY.CHARACTER, list3);
					}
					else if (m_debugMenuType == DEBUG_MENU_TYPE.RANKING)
					{
						if (SingletonGameObject<RankingManager>.Instance != null)
						{
							RankingUtil.Ranker myRank = RankingManager.GetMyRank(RankingUtil.RankingMode.ENDLESS, RankingUtil.RankingRankerType.RIVAL, RankingManager.EndlessRivalRankingScoreType);
							if (myRank != null)
							{
								m_debugMenuRankingCurrentRank = myRank.rankIndex + 1;
								m_debugMenuRankingCurrentDummyRank = m_debugMenuRankingCurrentRank;
								m_debugMenuRankingCurrentLegMax = RankingManager.GetCurrentMyLeagueMax(RankingUtil.RankingMode.ENDLESS);
							}
						}
					}
					else if (m_debugMenuType == DEBUG_MENU_TYPE.DEBUG_GUI_OFF)
					{
						m_debugMenu = !m_debugMenu;
						m_debugMenuType = DEBUG_MENU_TYPE.NONE;
						m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
						m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
						AllocationStatus.hide = true;
						SingletonGameObject<DebugGameObject>.Remove();
					}
					else if (m_debugMenuType == DEBUG_MENU_TYPE.CHAO_TEX_RELEASE)
					{
						m_debugMenu = !m_debugMenu;
						m_debugMenuType = DEBUG_MENU_TYPE.NONE;
						m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
						m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
						SingletonGameObject<RankingManager>.Instance.ResetChaoTexture();
						ChaoTextureManager.Instance.RemoveChaoTextureForMainMenuEnd();
					}
					break;
				}
			}
			else
			{
				switch (m_debugMenuType)
				{
				case DEBUG_MENU_TYPE.ITEM:
					GuiMainMenuBtnItem(rect);
					break;
				case DEBUG_MENU_TYPE.MILEAGE:
					GuiMainMenuBtnMile(rect);
					break;
				case DEBUG_MENU_TYPE.RANKING:
					GuiMainMenuBtnRanking(rect);
					break;
				case DEBUG_MENU_TYPE.DAILY_BATTLE:
					if (!(SingletonGameObject<DailyBattleManager>.Instance != null))
					{
					}
					break;
				}
			}
			if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 50f, 40f), GUI_RECT_ANCHOR.CENTER_RIGHT), "close"))
			{
				if (m_debugMenuType == DEBUG_MENU_TYPE.NONE)
				{
					m_debugMenu = !m_debugMenu;
				}
				else if (m_debugMenuRankingCateg == DEBUG_MENU_RANKING_CATEGORY.CACHE && m_debugMenuRankingType != RankingUtil.RankingRankerType.COUNT)
				{
					m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
					m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
					m_debugMenuMileageEpi = 2;
					m_debugMenuMileageCha = 1;
				}
				else
				{
					m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
					m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
					m_debugMenuType = DEBUG_MENU_TYPE.NONE;
					m_debugMenuMileageEpi = 2;
					m_debugMenuMileageCha = 1;
				}
			}
		}
		else if (GUI.Button(CreateGuiRect(new Rect(0f, 0f, 50f, 40f), GUI_RECT_ANCHOR.CENTER_RIGHT), "debug\n menu"))
		{
			m_debugMenu = !m_debugMenu;
			m_debugMenuType = DEBUG_MENU_TYPE.NONE;
			m_debugMenuRankingCateg = DEBUG_MENU_RANKING_CATEGORY.CACHE;
			m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
		}
	}

	private void ChangeLoadLangeAtlas()
	{
		if (m_debugAtlasLangList != null && m_debugAtlasLangList.Count > 0)
		{
			foreach (UIAtlas debugAtlasLang in m_debugAtlasLangList)
			{
				string text = ChangeAtlasName(debugAtlasLang);
				Debug.Log("! " + text);
				GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.UI, text);
				if (gameObject != null)
				{
					UIAtlas component = gameObject.GetComponent<UIAtlas>();
					if (debugAtlasLang != null && component != null)
					{
						debugAtlasLang.replacement = component;
						debugAtlasLang.name = text;
					}
					Debug.Log("!!! " + debugAtlasLang.name);
				}
			}
			Resources.UnloadUnusedAssets();
		}
		CheckLoadAtlas();
	}

	private void CheckLoadAtlas()
	{
		if (m_debugAtlasList != null)
		{
			m_debugAtlasList.Clear();
		}
		if (m_debugAtlasLangList != null)
		{
			m_debugAtlasLangList.Clear();
		}
		m_debugAtlasList = new List<UIAtlas>();
		m_debugAtlasLangList = new List<UIAtlas>();
		UIAtlas[] array = Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[];
		if (array == null || array.Length <= 0)
		{
			return;
		}
		UIAtlas[] array2 = array;
		foreach (UIAtlas uIAtlas in array2)
		{
			if (IsLangAtlas(uIAtlas))
			{
				m_debugAtlasLangList.Add(uIAtlas);
			}
			else
			{
				m_debugAtlasList.Add(uIAtlas);
			}
		}
	}

	private bool IsLangAtlas(UIAtlas atlas)
	{
		bool result = false;
		if (atlas != null)
		{
			string[] array = atlas.name.Split('_');
			if (array != null && array.Length > 1)
			{
				string text = array[array.Length - 1];
				if (!string.IsNullOrEmpty(text) && TextUtility.IsSuffix(text))
				{
					result = true;
				}
			}
		}
		return result;
	}

	private string ChangeAtlasName(UIAtlas atlas)
	{
		string result = null;
		if (atlas != null)
		{
			string[] array = atlas.name.Split('_');
			if (array != null && array.Length > 1)
			{
				string text = array[array.Length - 1];
				if (!string.IsNullOrEmpty(text) && TextUtility.IsSuffix(text))
				{
					result = string.Empty;
					for (int i = 0; i < array.Length - 1; i++)
					{
						result = result + array[i] + "_";
					}
					result += m_debugAtlasLangCode;
				}
			}
		}
		return result;
	}

	private void CheckDrawCall2D()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			List<UIPanel> list = new List<UIPanel>(gameObject.GetComponentsInChildren<UIPanel>());
			if (list.Count > 0)
			{
				m_debugDrawCallList = new Dictionary<string, Dictionary<string, List<UIDrawCall>>>();
				for (int i = 0; i < list.Count; i++)
				{
					UIPanel uIPanel = list[i];
					if (!(uIPanel != null))
					{
						continue;
					}
					int num = 0;
					Dictionary<string, List<UIDrawCall>> dictionary = null;
					Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
					for (int j = 0; j < UIDrawCall.list.size; j++)
					{
						UIDrawCall uIDrawCall = UIDrawCall.list[j];
						if (uIDrawCall.panel != uIPanel)
						{
							continue;
						}
						if (dictionary == null)
						{
							dictionary = new Dictionary<string, List<UIDrawCall>>();
						}
						if (!dictionary2.ContainsKey(uIDrawCall.material.name))
						{
							dictionary2.Add(uIDrawCall.material.name, 1);
						}
						else
						{
							Dictionary<string, int> dictionary3;
							Dictionary<string, int> dictionary4 = dictionary3 = dictionary2;
							string name;
							string key = name = uIDrawCall.material.name;
							int num2 = dictionary3[name];
							dictionary4[key] = num2 + 1;
						}
						if (dictionary2.ContainsKey(uIDrawCall.material.name))
						{
							string key2 = uIDrawCall.material.name + " " + dictionary2[uIDrawCall.material.name];
							if (!dictionary.ContainsKey(key2))
							{
								List<UIDrawCall> list2 = new List<UIDrawCall>();
								list2.Add(uIDrawCall);
								dictionary.Add(key2, list2);
							}
							else
							{
								dictionary[key2].Add(uIDrawCall);
							}
						}
						num++;
					}
					if (dictionary != null)
					{
						m_debugDrawCallList.Add(uIPanel.name + "  atlas:" + num, dictionary);
					}
				}
			}
			else
			{
				m_debugDrawCallList = null;
			}
		}
		else
		{
			m_debugDrawCallList = null;
		}
	}

	private void GuiLoadAtlas(Rect target, float sizeW, float sizeH)
	{
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.007f, 0.5f, 0.056f), GUI_RECT_ANCHOR.CENTER_TOP);
		if (GUI.Button(position, string.Empty + m_debugCheckType))
		{
			CheckLoadAtlas();
		}
		if (m_debugAtlasList == null && m_debugAtlasLangList == null)
		{
			return;
		}
		int num = 0;
		float num2 = -0.375f;
		float width = 0.24f;
		float num3 = 0.25f;
		float height = 0.06f;
		float num4 = 0.064f;
		foreach (UIAtlas debugAtlas in m_debugAtlasList)
		{
			Rect position2 = CreateGuiRectInRate(target, new Rect(num2 + num3 * (float)(num % 4), 0.065f + num4 * (float)(num / 4), width, height), GUI_RECT_ANCHOR.CENTER_TOP);
			string text = debugAtlas.name + " [" + debugAtlas.texture.width + "]";
			if (GUI.Button(position2, string.Empty + text))
			{
				int num5 = debugAtlas.texture.width * debugAtlas.texture.height;
				int num6 = 0;
				int num7 = 0;
				if (debugAtlas.spriteList != null)
				{
					num7 = debugAtlas.spriteList.Count;
				}
				Debug.Log("===================== " + text + " spriteNum:" + num7 + " ========================");
				if (num7 > 0)
				{
					int num8 = 0;
					string text2 = string.Empty;
					foreach (UISpriteData sprite in debugAtlas.spriteList)
					{
						if (string.IsNullOrEmpty(text2))
						{
							text2 = string.Empty + num8 + "  " + sprite.name + " [" + sprite.width + "×" + sprite.height + "]";
						}
						else
						{
							string text3 = text2;
							text2 = text3 + "\n" + num8 + "  " + sprite.name + " [" + sprite.width + "×" + sprite.height + "]";
						}
						num6 += sprite.width * sprite.height;
						num8++;
					}
					Debug.Log(text2);
				}
				Debug.Log("===================================== useArea: " + (float)(int)((float)num6 / (float)num5 * 1000f) / 10f + "% =============================================");
			}
			num++;
		}
		foreach (UIAtlas debugAtlasLang in m_debugAtlasLangList)
		{
			Rect position3 = CreateGuiRectInRate(target, new Rect(num2 + num3 * (float)(num % 4), 0.065f + num4 * (float)(num / 4), width, height), GUI_RECT_ANCHOR.CENTER_TOP);
			string text4 = debugAtlasLang.name + " [" + debugAtlasLang.texture.width + "]";
			if (GUI.Button(position3, string.Empty + text4))
			{
				int num9 = debugAtlasLang.texture.width * debugAtlasLang.texture.height;
				int num10 = 0;
				int num11 = 0;
				if (debugAtlasLang.spriteList != null)
				{
					num11 = debugAtlasLang.spriteList.Count;
				}
				Debug.Log("===================== " + text4 + " spriteNum:" + num11 + " ========================");
				if (num11 > 0)
				{
					string text5 = string.Empty;
					int num12 = 0;
					foreach (UISpriteData sprite2 in debugAtlasLang.spriteList)
					{
						if (string.IsNullOrEmpty(text5))
						{
							text5 = string.Empty + num12 + "  " + sprite2.name + " [" + sprite2.width + "×" + sprite2.height + "]";
						}
						else
						{
							string text3 = text5;
							text5 = text3 + "\n" + num12 + "  " + sprite2.name + " [" + sprite2.width + "×" + sprite2.height + "]";
						}
						num10 += sprite2.width * sprite2.height;
						num12++;
					}
					Debug.Log(text5);
				}
				Debug.Log("===================================== useArea: " + (float)(int)((float)num10 / (float)num9 * 1000f) / 10f + "% =============================================");
			}
			num++;
		}
	}

	private void GuiDrawCall2D(Rect target, float sizeW, float sizeH)
	{
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.007f, 0.5f, 0.056f), GUI_RECT_ANCHOR.CENTER_TOP);
		if (m_debugDrawCallList == null || m_debugDrawCallList.Count <= 0)
		{
			return;
		}
		if (string.IsNullOrEmpty(m_debugDrawCallPanelCurrent))
		{
			if (GUI.Button(position, string.Empty + m_debugCheckType))
			{
				CheckDrawCall2D();
				m_debugDrawCallPanelCurrent = string.Empty;
				m_debugDrawCallMatCurrent = string.Empty;
			}
			Dictionary<string, Dictionary<string, List<UIDrawCall>>>.KeyCollection keys = m_debugDrawCallList.Keys;
			int num = 0;
			foreach (string item in keys)
			{
				Rect position2 = CreateGuiRectInRate(target, new Rect(0f, 0.065f + 0.07f * (float)num, 0.96f, 0.06f), GUI_RECT_ANCHOR.CENTER_TOP);
				if (GUI.Button(position2, string.Empty + item))
				{
					m_debugDrawCallPanelCurrent = item;
				}
				num++;
			}
		}
		else
		{
			if (!m_debugDrawCallList.ContainsKey(m_debugDrawCallPanelCurrent))
			{
				return;
			}
			float num2 = -0.25f;
			float num3 = 0.065f;
			if (string.IsNullOrEmpty(m_debugDrawCallMatCurrent))
			{
				GUI.Box(position, string.Empty + m_debugDrawCallPanelCurrent);
				Dictionary<string, List<UIDrawCall>> dictionary = m_debugDrawCallList[m_debugDrawCallPanelCurrent];
				Dictionary<string, List<UIDrawCall>>.KeyCollection keys2 = dictionary.Keys;
				int num4 = 0;
				foreach (string item2 in keys2)
				{
					int num5 = num4 % 2;
					int num6 = 0;
					if (num4 >= 2)
					{
						num6 = num4 / 2;
					}
					Rect position3 = CreateGuiRectInRate(target, new Rect(num2 + (float)num5 * 0.5f, num3 + 0.06f * (float)num6, 0.48f, 0.057f), GUI_RECT_ANCHOR.CENTER_TOP);
					GUI.Box(position3, string.Empty + item2);
					num4++;
				}
				return;
			}
			GUI.Box(position, string.Empty + m_debugDrawCallMatCurrent);
			List<UIDrawCall> list = m_debugDrawCallList[m_debugDrawCallPanelCurrent][m_debugDrawCallMatCurrent];
			int num7 = 0;
			foreach (UIDrawCall item3 in list)
			{
				int num8 = num7 % 2;
				int num9 = 0;
				if (num7 >= 2)
				{
					num9 = num7 / 2;
				}
				Rect position4 = CreateGuiRectInRate(target, new Rect(num2 + (float)num8 * 0.5f, num3 + 0.06f * (float)num9, 0.48f, 0.057f), GUI_RECT_ANCHOR.CENTER_TOP);
				if (GUI.Button(position4, string.Empty + item3.name + " " + item3.gameObject.activeSelf))
				{
					item3.gameObject.SetActive(!item3.gameObject.activeSelf);
				}
				num7++;
			}
		}
	}

	private void GuiMainMenuBtnMile(Rect target)
	{
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.04f, 0.44f, 0.08f), GUI_RECT_ANCHOR.CENTER_TOP);
		GUI.Box(position, "Mileage");
		Rect position2 = CreateGuiRectInRate(target, new Rect(0f, 0.18f, 0.32f, 0.16f), GUI_RECT_ANCHOR.CENTER_TOP);
		if (GUI.Button(position2, string.Empty + m_debugMenuMileageEpi))
		{
			m_debugMenuMileageEpi = 2;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.02f, 0.18f, 0.28f, 0.16f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
		{
			m_debugMenuMileageEpi++;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.02f, 0.18f, 0.28f, 0.16f)), "<"))
		{
			m_debugMenuMileageEpi--;
			if (m_debugMenuMileageEpi < 2)
			{
				m_debugMenuMileageEpi = 2;
			}
		}
		Rect position3 = CreateGuiRectInRate(target, new Rect(0f, 0.4f, 0.32f, 0.16f), GUI_RECT_ANCHOR.CENTER_TOP);
		if (GUI.Button(position3, string.Empty + m_debugMenuMileageCha))
		{
			m_debugMenuMileageCha = 1;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.02f, 0.4f, 0.28f, 0.16f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
		{
			m_debugMenuMileageCha++;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.02f, 0.4f, 0.28f, 0.16f)), "<"))
		{
			m_debugMenuMileageCha--;
			if (m_debugMenuMileageCha < 1)
			{
				m_debugMenuMileageCha = 1;
			}
		}
		Rect position4 = CreateGuiRectInRate(target, new Rect(0f, 0.6f, 0.96f, 0.16f), GUI_RECT_ANCHOR.CENTER_TOP);
		if (GUI.Button(position4, "ok"))
		{
			GuiMainMenuBtnMileSetting();
		}
		Rect position5 = CreateGuiRectInRate(target, new Rect(0f, 0.78f, 0.96f, 0.08f), GUI_RECT_ANCHOR.CENTER_TOP);
		GUI.Box(position5, "※ back to title!");
	}

	private void GuiMainMenuBtnMileSetting()
	{
		ServerMileageMapState serverMileageMapState = new ServerMileageMapState();
		serverMileageMapState.m_episode = m_debugMenuMileageEpi;
		serverMileageMapState.m_chapter = m_debugMenuMileageCha;
		serverMileageMapState.m_point = 0;
		serverMileageMapState.m_stageTotalScore = 0L;
		serverMileageMapState.m_numBossAttack = 0;
		serverMileageMapState.m_stageMaxScore = 0L;
		NetDebugUpdMileageData request = new NetDebugUpdMileageData(serverMileageMapState);
		StartCoroutine(NetworkRequest(request, AddOpeMessageMileCallback, NetworkFailedMileCallback));
	}

	private void AddOpeMessageMileCallback()
	{
		m_debugMenuType = DEBUG_MENU_TYPE.NONE;
		HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.TITLE);
	}

	private void GuiMainMenuBtnRankingChange(Rect target)
	{
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.16f, 0.6f, 0.08f), GUI_RECT_ANCHOR.CENTER_TOP);
		if (m_debugMenuRankingCurrentRank > 0 && m_debugMenuRankingCurrentLegMax > 1)
		{
			GUI.Box(position, "current rank:" + m_debugMenuRankingCurrentRank);
			Rect position2 = CreateGuiRectInRate(target, new Rect(0f, 0.28f, 0.28f, 0.12f), GUI_RECT_ANCHOR.CENTER_TOP);
			if (GUI.Button(position2, string.Empty + m_debugMenuRankingCurrentDummyRank))
			{
				m_debugMenuRankingCurrentDummyRank = m_debugMenuRankingCurrentRank;
			}
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.02f, 0.28f, 0.24f, 0.12f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
			{
				m_debugMenuRankingCurrentDummyRank++;
				if (m_debugMenuRankingCurrentDummyRank > m_debugMenuRankingCurrentLegMax)
				{
					m_debugMenuRankingCurrentDummyRank = m_debugMenuRankingCurrentLegMax;
				}
			}
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.02f, 0.28f, 0.24f, 0.12f)), "<"))
			{
				m_debugMenuRankingCurrentDummyRank--;
				if (m_debugMenuRankingCurrentDummyRank < 1)
				{
					m_debugMenuRankingCurrentDummyRank = 1;
				}
			}
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(0f, 0.48f, 0.76f, 0.16f), GUI_RECT_ANCHOR.CENTER_TOP), "dummy old rank save"))
			{
				SingletonGameObject<RankingManager>.Instance.SavePlayerRankingDataDummy(RankingUtil.RankingMode.ENDLESS, RankingUtil.RankingRankerType.RIVAL, RankingManager.EndlessRivalRankingScoreType, m_debugMenuRankingCurrentDummyRank);
				RankingUI.DebugInitRankingChange();
			}
		}
		else
		{
			GUI.Box(position, "no ranking data");
		}
	}

	private void GuiMainMenuBtnRankingCache(Rect target)
	{
		if (!(SingletonGameObject<RankingManager>.Instance != null))
		{
			return;
		}
		RankingUtil.RankingMode rankingMode = RankingUtil.RankingMode.ENDLESS;
		if (m_debugMenuRankingType == RankingUtil.RankingRankerType.COUNT)
		{
			int num = 6;
			int num2 = 0;
			float num3 = 0.8f / (float)num;
			float width = 0.92f;
			for (int i = 0; i < num; i++)
			{
				RankingUtil.RankingRankerType rankingRankerType = (RankingUtil.RankingRankerType)i;
				if (SingletonGameObject<RankingManager>.Instance.IsRankingTop(rankingMode, RankingUtil.RankingScoreType.HIGH_SCORE, rankingRankerType) || SingletonGameObject<RankingManager>.Instance.IsRankingTop(rankingMode, RankingUtil.RankingScoreType.TOTAL_SCORE, rankingRankerType))
				{
					if (GUI.Button(CreateGuiRectInRate(target, new Rect(0f, 0.16f + (float)num2 * num3, width, num3 - 0.01f), GUI_RECT_ANCHOR.CENTER_TOP), string.Empty + rankingRankerType))
					{
						m_debugMenuRankingType = rankingRankerType;
					}
					num2++;
				}
			}
			return;
		}
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.16f, 0.92f, 0.08f), GUI_RECT_ANCHOR.CENTER_TOP);
		Rect rect = CreateGuiRectInRate(target, new Rect(0f, 0.26f, 0.92f, 0.32f), GUI_RECT_ANCHOR.CENTER_TOP);
		Rect rect2 = CreateGuiRectInRate(target, new Rect(0f, 0.588f, 0.92f, 0.32f), GUI_RECT_ANCHOR.CENTER_TOP);
		Rect position2 = CreateGuiRectInRate(target, new Rect(0f, 0.92f, 0.92f, 0.1f), GUI_RECT_ANCHOR.CENTER_TOP);
		GUI.Box(position, string.Empty + m_debugMenuRankingType);
		if (GUI.Button(position2, "back"))
		{
			m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
		}
		if (SingletonGameObject<RankingManager>.Instance.IsRankingTop(rankingMode, RankingUtil.RankingScoreType.HIGH_SCORE, m_debugMenuRankingType))
		{
			GuiMainMenuBtnRankingCacheInfo(rect, RankingUtil.RankingScoreType.HIGH_SCORE);
		}
		else
		{
			GUI.Box(rect, "not found");
		}
		if (SingletonGameObject<RankingManager>.Instance.IsRankingTop(rankingMode, RankingUtil.RankingScoreType.TOTAL_SCORE, m_debugMenuRankingType))
		{
			GuiMainMenuBtnRankingCacheInfo(rect2, RankingUtil.RankingScoreType.TOTAL_SCORE);
		}
		else
		{
			GUI.Box(rect2, "not found");
		}
	}

	private void GuiMainMenuBtnRankingCacheInfo(Rect rect, RankingUtil.RankingScoreType scoreType)
	{
		string text = string.Empty + scoreType;
		List<RankingUtil.Ranker> cacheRankingList = SingletonGameObject<RankingManager>.Instance.GetCacheRankingList(RankingUtil.RankingMode.ENDLESS, scoreType, m_debugMenuRankingType);
		if (cacheRankingList != null && cacheRankingList.Count > 1)
		{
			RankingUtil.Ranker ranker = cacheRankingList[0];
			int num = -1;
			int num2 = -1;
			int num3 = 0;
			text = ((ranker == null) ? (text + "  myRank:---") : (text + "  myRank:" + (ranker.rankIndex + 1)));
			for (int i = 1; i < cacheRankingList.Count; i++)
			{
				if (num == -1)
				{
					num = cacheRankingList[i].rankIndex + 1;
					num2 = -1;
					num3 = 0;
				}
				else if (num + num3 + 1 != cacheRankingList[i].rankIndex + 1)
				{
					num2 = cacheRankingList[i - 1].rankIndex + 1;
				}
				else
				{
					num3++;
				}
				if (num != -1 && num2 != -1)
				{
					string text2 = text;
					text = text2 + "\n" + num + " ～ " + num2;
					num = cacheRankingList[i].rankIndex + 1;
					num2 = -1;
					num3 = 0;
				}
			}
			if (num != -1 && num2 == -1)
			{
				string text2 = text;
				text = text2 + "\n" + num + " ～ " + (cacheRankingList[cacheRankingList.Count - 1].rankIndex + 1);
			}
		}
		GUI.Box(rect, text);
	}

	private void GuiMainMenuBtnRanking(Rect target)
	{
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.04f, 0.44f, 0.08f), GUI_RECT_ANCHOR.CENTER_TOP);
		GUI.Box(position, string.Empty + m_debugMenuRankingCateg);
		int num = (int)m_debugMenuRankingCateg;
		int num2 = 2;
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.01f, 0.02f, 0.24f, 0.12f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
		{
			num = (int)(m_debugMenuRankingCateg = (DEBUG_MENU_RANKING_CATEGORY)((num + 1 + num2) % num2));
			m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.01f, 0.02f, 0.24f, 0.12f)), "<"))
		{
			num = (int)(m_debugMenuRankingCateg = (DEBUG_MENU_RANKING_CATEGORY)((num - 1 + num2) % num2));
			m_debugMenuRankingType = RankingUtil.RankingRankerType.COUNT;
		}
		switch (m_debugMenuRankingCateg)
		{
		case DEBUG_MENU_RANKING_CATEGORY.CACHE:
			GuiMainMenuBtnRankingCache(target);
			break;
		case DEBUG_MENU_RANKING_CATEGORY.CHANGE_TEST:
			GuiMainMenuBtnRankingChange(target);
			break;
		}
	}

	private void GuiMainMenuBtnItem(Rect target)
	{
		Rect position = CreateGuiRectInRate(target, new Rect(0f, 0.04f, 0.44f, 0.08f), GUI_RECT_ANCHOR.CENTER_TOP);
		GUI.Box(position, string.Empty + m_debugMenuItemSelect);
		int num = (int)m_debugMenuItemSelect;
		int num2 = 3;
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.01f, 0.02f, 0.24f, 0.12f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
		{
			num = (num + 1 + num2) % num2;
			m_debugMenuItemNum = 1;
			m_debugMenuItemPage = 0;
			m_debugMenuItemSelect = (DEBUG_MENU_ITEM_CATEGORY)num;
		}
		if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.01f, 0.02f, 0.24f, 0.12f)), "<"))
		{
			num = (num - 1 + num2) % num2;
			m_debugMenuItemNum = 1;
			m_debugMenuItemPage = 0;
			m_debugMenuItemSelect = (DEBUG_MENU_ITEM_CATEGORY)num;
		}
		if ((m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.ITEM || m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.OTOMO) && m_debugMenuItemList.ContainsKey(m_debugMenuItemSelect) && m_debugMenuItemList[m_debugMenuItemSelect].Count > 0)
		{
			Rect position2 = CreateGuiRectInRate(target, new Rect(0f, 0.15f, 0.24f, 0.12f), GUI_RECT_ANCHOR.CENTER_TOP);
			if (GUI.Button(position2, string.Empty + m_debugMenuItemNum))
			{
				m_debugMenuItemNum = 1;
			}
			if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.ITEM)
			{
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.01f, 0.15f, 0.144f, 0.12f), GUI_RECT_ANCHOR.RIGHT_TOP), ">>"))
				{
					m_debugMenuItemNum += 10;
				}
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.18f, 0.15f, 0.16f, 0.12f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
				{
					m_debugMenuItemNum++;
				}
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.01f, 0.15f, 0.144f, 0.12f)), "<<"))
				{
					m_debugMenuItemNum -= 10;
					if (m_debugMenuItemNum < 1)
					{
						m_debugMenuItemNum = 1;
					}
				}
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.18f, 0.15f, 0.16f, 0.12f)), "<"))
				{
					m_debugMenuItemNum--;
					if (m_debugMenuItemNum < 1)
					{
						m_debugMenuItemNum = 1;
					}
				}
			}
			else if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.OTOMO)
			{
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.01f, 0.15f, 0.36f, 0.12f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
				{
					m_debugMenuItemNum++;
					if (m_debugMenuItemNum > 6)
					{
						m_debugMenuItemNum = 6;
					}
				}
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.01f, 0.15f, 0.36f, 0.12f)), "<"))
				{
					m_debugMenuItemNum--;
					if (m_debugMenuItemNum < 1)
					{
						m_debugMenuItemNum = 1;
					}
				}
			}
		}
		if (!m_debugMenuItemList.ContainsKey(m_debugMenuItemSelect) || m_debugMenuItemList[m_debugMenuItemSelect].Count <= 0)
		{
			return;
		}
		List<int> list = m_debugMenuItemList[m_debugMenuItemSelect];
		int num3 = 5;
		float num4 = 0.155f;
		if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.ITEM || m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.OTOMO)
		{
			num3 = 4;
			num4 = 0.275f;
			if (list.Count > num3)
			{
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.01f, num4, 0.136f, 0.72f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
				{
					m_debugMenuItemPage++;
					if (m_debugMenuItemPage * num3 >= list.Count)
					{
						m_debugMenuItemPage--;
					}
				}
				if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.01f, num4, 0.136f, 0.72f)), "<"))
				{
					m_debugMenuItemPage--;
					if (m_debugMenuItemPage < 0)
					{
						m_debugMenuItemPage = 0;
					}
				}
			}
		}
		else if (list.Count > num3)
		{
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(-0.01f, num4, 0.136f, 0.84f), GUI_RECT_ANCHOR.RIGHT_TOP), ">"))
			{
				m_debugMenuItemPage++;
				if (m_debugMenuItemPage * num3 >= list.Count)
				{
					m_debugMenuItemPage--;
				}
			}
			if (GUI.Button(CreateGuiRectInRate(target, new Rect(0.01f, num4, 0.136f, 0.84f)), "<"))
			{
				m_debugMenuItemPage--;
				if (m_debugMenuItemPage < 0)
				{
					m_debugMenuItemPage = 0;
				}
			}
		}
		for (int i = 0; i < num3; i++)
		{
			int num5 = i + m_debugMenuItemPage * num3;
			if (num5 >= list.Count)
			{
				break;
			}
			int num6 = list[num5];
			int num7 = i;
			bool flag = true;
			string text = num6.ToString();
			if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.ITEM)
			{
				text = string.Empty + (ServerItem.Id)num6;
			}
			else if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.OTOMO)
			{
				if (m_debugMenuOtomoList != null && m_debugMenuOtomoList.Count > num5)
				{
					text = m_debugMenuOtomoList[num5].name;
					if (m_debugMenuOtomoList[num5].id + 400000 != num6)
					{
						flag = false;
						text += "\n Unimplemented";
					}
					else
					{
						int level = m_debugMenuOtomoList[num5].level;
						text = ((level < 0) ? (text + "\n not have") : (text + "\n Lv:" + level));
					}
				}
			}
			else if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.CHARACTER)
			{
				text = "[" + num6 + "]";
				if (m_debugMenuCharaList != null && m_debugMenuCharaList.Count > num5)
				{
					text = m_debugMenuCharaList[num5].m_name;
				}
			}
			if (flag)
			{
				if (!GUI.Button(CreateGuiRectInRate(target, new Rect(0f, num4 + (float)num7 * 0.173f, 0.62f, 0.16f), GUI_RECT_ANCHOR.CENTER_TOP), text))
				{
					continue;
				}
				if (m_debugMenuItemSelect == DEBUG_MENU_ITEM_CATEGORY.OTOMO && m_debugMenuItemNum > 1)
				{
					for (int j = 0; j < m_debugMenuItemNum - 1; j++)
					{
						DebugRequestGiftItem(num6, 1, null, false);
					}
					DebugRequestGiftItem(num6, 1, null);
				}
				else
				{
					DebugRequestGiftItem(num6, m_debugMenuItemNum, null);
				}
			}
			else
			{
				GUI.Box(CreateGuiRectInRate(target, new Rect(0f, num4 + (float)num7 * 0.173f, 0.62f, 0.16f), GUI_RECT_ANCHOR.CENTER_TOP), text);
			}
		}
	}

	private void GuiRoulette()
	{
		if (Application.loadedLevelName.IndexOf("title") == -1 && Application.loadedLevelName.IndexOf("playingstage") == -1 && Application.loadedLevelName.IndexOf("MainMenu") != -1)
		{
		}
	}

	private void GuiPlayerCharaList()
	{
		if (Application.loadedLevelName.IndexOf("title") == -1 && Application.loadedLevelName.IndexOf("playingstage") == -1 && Application.loadedLevelName.IndexOf("MainMenu") != -1)
		{
		}
	}

	private void GuiRaid()
	{
		if (Application.loadedLevelName.IndexOf("title") == -1 && Application.loadedLevelName.IndexOf("playingstage") == -1 && Application.loadedLevelName.IndexOf("MainMenu") != -1)
		{
		}
	}

	private void GuiRanking()
	{
		if (Application.loadedLevelName.IndexOf("title") == -1 && Application.loadedLevelName.IndexOf("playingstage") == -1 && Application.loadedLevelName.IndexOf("MainMenu") != -1)
		{
		}
	}

	private void GuiCurrentScoreTest()
	{
		if (Application.loadedLevelName.IndexOf("title") != -1 || Application.loadedLevelName.IndexOf("playingstage") != -1 || Application.loadedLevelName.IndexOf("MainMenu") == -1 || (m_currentScore < 0 && m_currentScoreEvent < 0))
		{
			return;
		}
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null)
		{
			if (m_currentScore >= 0 && GUI.Button(new Rect(30f, 110f, 150f, 40f), string.Concat("Current ", targetRankingRankerType, "\n", m_currentScore)))
			{
				RankingUtil.DebugCurrentRanking(false, m_currentScore);
			}
			if (m_currentScoreEvent >= 0 && GUI.Button(new Rect(30f, 165f, 150f, 40f), "Current Rank Event\n" + m_currentScoreEvent))
			{
				RankingUtil.DebugCurrentRanking(true, m_currentScoreEvent);
			}
		}
	}

	private void Update()
	{
		m_currentTimeScale = Time.timeScale;
		if (m_debugRouletteTime > 0f)
		{
			m_debugRouletteTime -= Time.deltaTime;
			if (m_debugRouletteTime <= 0f)
			{
				if (m_rouletteCommitMsg != null && !m_debugRouletteConectError)
				{
					m_rouletteCallback.SendMessage("ServerCommitWheelSpinGeneral_Succeeded", m_rouletteCommitMsg);
				}
				else
				{
					MsgServerConnctFailed value = new MsgServerConnctFailed(ServerInterface.StatusCode.AlreadyEndEvent);
					m_rouletteCallback.SendMessage("ServerCommitWheelSpinGeneral_Failed", value);
					m_debugRouletteConectError = false;
				}
				m_debugRouletteTime = 0f;
			}
		}
		if (m_debugGetRouletteTime > 0f)
		{
			m_debugGetRouletteTime -= Time.deltaTime;
			if (m_debugGetRouletteTime <= 0f)
			{
				if (m_rouletteGetMsg != null && !m_debugRouletteConectError)
				{
					m_rouletteCallback.SendMessage("ServerGetWheelOptionsGeneral_Succeeded", m_rouletteGetMsg);
				}
				else
				{
					MsgServerConnctFailed value2 = new MsgServerConnctFailed(ServerInterface.StatusCode.AlreadyEndEvent);
					m_rouletteCallback.SendMessage("ServerWheelOptionsGeneral_Failed", value2);
					m_debugRouletteConectError = false;
				}
				m_debugGetRouletteTime = 0f;
			}
		}
		base.enabled = false;
	}

	private void ShowUpdCost()
	{
		if (m_currentUpdCost == null || m_keys == null)
		{
			return;
		}
		if (m_updateCostList == null)
		{
			m_updateCostList = new List<string>();
		}
		int num = m_keys.Count - m_updateCostList.Count;
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				m_updateCostList.Add("---");
			}
		}
		int num2 = 0;
		foreach (string key in m_keys)
		{
			if (num2 < m_updateCostList.Count)
			{
				m_updateCostList[num2] = key + " : " + m_updCost[key];
				num2++;
				continue;
			}
			break;
		}
	}

	private bool SetUpdCost(string name, float rate)
	{
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		bool result = false;
		if (m_currentUpdCost != null && m_updCost != null && m_currentUpdCost.ContainsKey(name) && m_updCost.ContainsKey(name))
		{
			int num = 0;
			if (m_currentUpdCost[name] > 0)
			{
				num = Mathf.FloorToInt((float)m_currentUpdCost[name] / 2f * rate);
			}
			if (name == "TOTAL_COST")
			{
				m_updateCost = num;
			}
			m_updCost[name] = num;
			m_currentUpdCost[name] = 0;
		}
		return result;
	}

	private bool CheckDummyRequest()
	{
		bool result = true;
		if (UnityEngine.Random.Range(0, 100) < m_rouletteDummyError)
		{
			result = false;
		}
		return result;
	}

	private void DummyRequestWheelOptionsItem(RouletteCategory cate, int rank, ref ServerWheelOptionsGeneral wheel, int max = 8)
	{
		if (wheel == null)
		{
			return;
		}
		List<ServerItem.Id> list = null;
		switch (cate)
		{
		case RouletteCategory.ITEM:
			list = m_rouletteDataItem;
			break;
		case RouletteCategory.PREMIUM:
			list = m_rouletteDataPremium;
			break;
		case RouletteCategory.SPECIAL:
			list = m_rouletteDataSpecial;
			break;
		case RouletteCategory.RAID:
			list = m_rouletteDataRaid;
			break;
		default:
			list = m_rouletteDataDefault;
			break;
		}
		for (int i = 0; i < max; i++)
		{
			int num = 0;
			int itemId = 120000;
			int num2 = 1;
			int weight = 1;
			if (list != null && list.Count > 0)
			{
				num = (i + max * rank) % list.Count;
				if (num < 0)
				{
					num = 0;
				}
				itemId = (int)list[num];
				if (list[num] == ServerItem.Id.RING)
				{
					num2 = 1000 * (rank + 1);
				}
				else if (list[num] == ServerItem.Id.RSRING)
				{
					num2 = 10 * (rank + 1);
				}
				else if (list[num] == ServerItem.Id.ENERGY)
				{
					num2 = 2 * (rank + 1);
				}
				else if (list[num] == ServerItem.Id.INVINCIBLE || list[num] == ServerItem.Id.COMBO || list[num] == ServerItem.Id.BARRIER || list[num] == ServerItem.Id.MAGNET || list[num] == ServerItem.Id.TRAMPOLINE || list[num] == ServerItem.Id.DRILL)
				{
					num2 = 1 * (rank + 1);
				}
				if (num2 < 1)
				{
					num2 = 1;
				}
			}
			wheel.SetupItem(i, itemId, weight, num2);
		}
	}

	private ServerWheelOptionsGeneral DummyRequestWheelOptionsGeneral(int rouletteId, int rank, int costItemId, int costItemNum, int costItemStock)
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = new List<int>();
		list.Add(costItemId);
		list2.Add(costItemNum);
		list3.Add(costItemStock);
		return DummyRequestWheelOptionsGeneral(rouletteId, rank, list, list2, list3);
	}

	private ServerWheelOptionsGeneral DummyRequestWheelOptionsGeneral(int rouletteId, int rank, List<int> costItemId, List<int> costItemNum, List<int> costItemStock)
	{
		ServerWheelOptionsGeneral wheel = new ServerWheelOptionsGeneral();
		int remaining = 0;
		RouletteCategory rouletteCategory = (RouletteCategory)rouletteId;
		DateTime nextFree = DateTime.Now.AddDays(999.0);
		if (rank > 0)
		{
			remaining = 1;
		}
		if (rouletteCategory == RouletteCategory.ITEM)
		{
			remaining = 5;
		}
		else if (rank > 0)
		{
			remaining = 1;
		}
		if (rouletteCategory == RouletteCategory.PREMIUM || rouletteCategory == RouletteCategory.SPECIAL)
		{
			rank = 0;
		}
		if (rouletteCategory == RouletteCategory.PREMIUM && RouletteManager.Instance != null && RouletteManager.Instance.specialEgg >= 10)
		{
			rouletteCategory = RouletteCategory.SPECIAL;
			rouletteId = 8;
		}
		DummyRequestWheelOptionsItem(rouletteCategory, rank, ref wheel);
		wheel.SetupParam(rouletteId, remaining, 9999999, rank, 5, nextFree);
		wheel.ResetupCostItem();
		for (int i = 0; i < costItemId.Count; i++)
		{
			int num = costItemId[i];
			int oneCost = costItemNum[i];
			int itemNum = costItemStock[i];
			if (num > 0)
			{
				wheel.AddCostItem(num, itemNum, oneCost);
			}
		}
		return wheel;
	}

	public void DummyRequestGetRouletteGeneral(int eventId, int rouletteId, GameObject callbackObject)
	{
		if (callbackObject != null)
		{
			m_rouletteGetMsg = new MsgGetWheelOptionsGeneralSucceed();
			int num = 0;
			switch (rouletteId)
			{
			case 2:
				num = 910000;
				break;
			case 1:
			case 8:
				num = 900000;
				break;
			default:
				num = 960000;
				break;
			}
			m_rouletteCallback = callbackObject;
			m_rouletteGetMsg.m_wheelOptionsGeneral = DummyRequestWheelOptionsGeneral(rouletteId, 0, num, 5, 100);
			m_debugGetRouletteTime = m_rouletteConectTime;
			m_debugRouletteConectError = !CheckDummyRequest();
			if (m_debugRouletteConectError)
			{
				m_debugGetRouletteTime = m_rouletteConectTime * 3f;
			}
		}
	}

	public void DummyRequestCommitRouletteGeneral(ServerWheelOptionsGeneral org, int eventId, int rouletteId, int spinCostItemId, int spinNum, GameObject callbackObject)
	{
		if (m_debugRouletteTime > 0f)
		{
			m_rouletteCommitMsg = null;
		}
		else
		{
			if (!(callbackObject != null))
			{
				return;
			}
			bool flag = false;
			MsgCommitWheelSpinGeneralSucceed msgCommitWheelSpinGeneralSucceed = new MsgCommitWheelSpinGeneralSucceed();
			ServerSpinResultGeneral serverSpinResultGeneral = new ServerSpinResultGeneral();
			List<ServerItemState> list = new List<ServerItemState>();
			List<ServerChaoData> list2 = new List<ServerChaoData>();
			int num = 0;
			int rank = 0;
			int num2 = 0;
			List<int> list3 = new List<int>();
			if (spinNum <= 1)
			{
				for (int i = 0; i < org.itemLenght; i++)
				{
					list3.Add(i);
				}
				num = list3[UnityEngine.Random.Range(0, list3.Count)];
				int itemId;
				int itemNum;
				float itemRate;
				org.GetCell(num, out itemId, out itemNum, out itemRate);
				if (itemId == 200000 || itemId == 200001)
				{
					rank = (int)(org.rank + 1);
				}
				else
				{
					ServerPlayerState playerState = ServerInterface.PlayerState;
					if (itemId >= 400000 && itemId < 500000)
					{
						ServerChaoData serverChaoData = new ServerChaoData();
						serverChaoData.Id = itemId;
						serverChaoData.Level = 0;
						serverChaoData.Rarity = 100;
						bool flag2 = false;
						ServerChaoState serverChaoState = playerState.ChaoStateByItemID(itemId);
						if (serverChaoState != null)
						{
							switch (serverChaoState.Status)
							{
							case ServerChaoState.ChaoStatus.NotOwned:
								serverChaoData.Level = 0;
								break;
							case ServerChaoState.ChaoStatus.Owned:
								serverChaoData.Level = serverChaoState.Level + 1;
								break;
							case ServerChaoState.ChaoStatus.MaxLevel:
								serverChaoData.Level = 5;
								flag2 = true;
								break;
							}
							if (flag2)
							{
								ServerItemState serverItemState = new ServerItemState();
								serverItemState.m_itemId = 220000;
								serverItemState.m_num = 4;
								num2 += 4;
								list.Add(serverItemState);
							}
						}
						serverChaoData.Rarity = itemId / 1000 % 10;
						list2.Add(serverChaoData);
					}
					else if (itemId >= 300000 && itemId < 400000)
					{
						ServerChaoData serverChaoData2 = new ServerChaoData();
						serverChaoData2.Id = itemId;
						serverChaoData2.Level = 0;
						serverChaoData2.Rarity = 100;
						list2.Add(serverChaoData2);
						ServerCharacterState serverCharacterState = playerState.CharacterStateByItemID(itemId);
						if (serverCharacterState != null && serverCharacterState.Id >= 0 && serverCharacterState.IsUnlocked)
						{
							ServerItemState serverItemState2 = new ServerItemState();
							serverItemState2.m_itemId = 900000;
							serverItemState2.m_num = 99;
							list.Add(serverItemState2);
							serverItemState2 = new ServerItemState();
							serverItemState2.m_itemId = 910000;
							serverItemState2.m_num = 1234;
							list.Add(serverItemState2);
							serverItemState2 = new ServerItemState();
							serverItemState2.m_itemId = 220000;
							serverItemState2.m_num = 5;
							num2 += 5;
							list.Add(serverItemState2);
						}
					}
					else
					{
						ServerItemState serverItemState3 = new ServerItemState();
						serverItemState3.m_itemId = itemId;
						serverItemState3.m_num = itemNum;
						list.Add(serverItemState3);
					}
				}
			}
			else if (org.rank == RouletteUtility.WheelRank.Normal)
			{
				num = -1;
				List<int> list4 = new List<int>();
				List<int> list5 = new List<int>();
				list4.Add(910000);
				list4.Add(120000);
				list4.Add(120001);
				list4.Add(120003);
				list4.Add(220000);
				list4.Add(120004);
				list4.Add(120005);
				list4.Add(120006);
				list4.Add(120007);
				list4.Add(220000);
				list4.Add(900000);
				list5.Add(400000);
				list5.Add(400001);
				list5.Add(400002);
				list5.Add(300000);
				list5.Add(400003);
				list5.Add(400019);
				list5.Add(300004);
				list5.Add(401000);
				list5.Add(401001);
				list5.Add(401002);
				list5.Add(300001);
				list5.Add(401003);
				list5.Add(401004);
				list5.Add(300005);
				int num3 = UnityEngine.Random.Range(0, list4.Count);
				for (int j = 0; j < spinNum; j++)
				{
					ServerItemState serverItemState4 = new ServerItemState();
					serverItemState4.m_itemId = list4[(j + num3) % list4.Count];
					serverItemState4.m_num = 1;
					if (serverItemState4.m_itemId == 220000)
					{
						num2++;
					}
					list.Add(serverItemState4);
				}
				num3 = UnityEngine.Random.Range(0, list5.Count);
				for (int k = 0; k < spinNum; k++)
				{
					ServerChaoData serverChaoData3 = new ServerChaoData();
					serverChaoData3.Id = list5[(k + num3) % list5.Count];
					serverChaoData3.Level = 5;
					if (serverChaoData3.Id >= 300000 && serverChaoData3.Id < 400000)
					{
						serverChaoData3.Level = 0;
						serverChaoData3.Rarity = 100;
						list2.Add(serverChaoData3);
						continue;
					}
					serverChaoData3.Rarity = serverChaoData3.Id / 1000 % 10;
					list2.Add(serverChaoData3);
					list2.Add(serverChaoData3);
					list2.Add(serverChaoData3);
					list2.Add(serverChaoData3);
					list2.Add(serverChaoData3);
					list2.Add(serverChaoData3);
				}
			}
			else
			{
				flag = true;
			}
			for (int l = 0; l < list.Count; l++)
			{
				serverSpinResultGeneral.AddItemState(list[l]);
			}
			for (int m = 0; m < list2.Count; m++)
			{
				serverSpinResultGeneral.AddChaoState(list2[m]);
			}
			serverSpinResultGeneral.ItemWon = num;
			m_rouletteCallback = callbackObject;
			RouletteCategory rouletteId2 = (RouletteCategory)org.rouletteId;
			int num4 = 0;
			switch (rouletteId2)
			{
			case RouletteCategory.ITEM:
				num4 = 910000;
				break;
			case RouletteCategory.PREMIUM:
			case RouletteCategory.SPECIAL:
				num4 = 900000;
				break;
			default:
				num4 = 960000;
				break;
			}
			ServerWheelOptionsGeneral serverWheelOptionsGeneral = DummyRequestWheelOptionsGeneral(org.rouletteId, rank, num4, 5, 100);
			serverWheelOptionsGeneral.spEgg = RouletteManager.Instance.specialEgg + num2;
			if (!flag)
			{
				msgCommitWheelSpinGeneralSucceed.m_playerState = ServerInterface.PlayerState;
				msgCommitWheelSpinGeneralSucceed.m_wheelOptionsGeneral = serverWheelOptionsGeneral;
				msgCommitWheelSpinGeneralSucceed.m_resultSpinResultGeneral = serverSpinResultGeneral;
				m_rouletteCommitMsg = msgCommitWheelSpinGeneralSucceed;
				m_debugRouletteTime = m_rouletteConectTime;
				m_debugRouletteConectError = !CheckDummyRequest();
				if (m_debugRouletteConectError)
				{
					m_debugRouletteTime = 3f;
				}
			}
			else
			{
				m_rouletteCommitMsg = null;
				m_debugRouletteTime = 0.1f;
				m_debugRouletteConectError = true;
			}
		}
	}

	public ServerSpinResultGeneral DummyRouletteGeneralResult(int spinNum)
	{
		ServerSpinResultGeneral serverSpinResultGeneral = new ServerSpinResultGeneral();
		List<ServerItemState> list = new List<ServerItemState>();
		List<ServerChaoData> list2 = new List<ServerChaoData>();
		int num = 0;
		if (spinNum <= 1)
		{
			num = 1;
			ServerItemState serverItemState = new ServerItemState();
			serverItemState.m_itemId = 120001;
			serverItemState.m_num = 1;
			list.Add(serverItemState);
		}
		else
		{
			num = -1;
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			list3.Add(910000);
			list3.Add(120000);
			list3.Add(120001);
			list3.Add(120003);
			list3.Add(220000);
			list3.Add(120004);
			list3.Add(120005);
			list3.Add(120006);
			list3.Add(120007);
			list3.Add(220000);
			list3.Add(900000);
			list4.Add(400000);
			list4.Add(400001);
			list4.Add(400002);
			list4.Add(300000);
			list4.Add(400003);
			list4.Add(400019);
			list4.Add(300004);
			list4.Add(401000);
			list4.Add(401001);
			list4.Add(401002);
			list4.Add(300001);
			list4.Add(401003);
			list4.Add(401004);
			list4.Add(300005);
			int num2 = UnityEngine.Random.Range(0, list3.Count);
			for (int i = 0; i < spinNum; i++)
			{
				ServerItemState serverItemState2 = new ServerItemState();
				serverItemState2.m_itemId = list3[(i + num2) % list3.Count];
				serverItemState2.m_num = 1;
				list.Add(serverItemState2);
			}
			num2 = UnityEngine.Random.Range(0, list4.Count);
			for (int j = 0; j < spinNum; j++)
			{
				ServerChaoData serverChaoData = new ServerChaoData();
				serverChaoData.Id = list4[(j + num2) % list4.Count];
				serverChaoData.Level = 5;
				if (serverChaoData.Id >= 300000 && serverChaoData.Id < 400000)
				{
					serverChaoData.Level = 0;
					serverChaoData.Rarity = 100;
					list2.Add(serverChaoData);
					continue;
				}
				serverChaoData.Rarity = serverChaoData.Id / 1000 % 10;
				list2.Add(serverChaoData);
				list2.Add(serverChaoData);
				list2.Add(serverChaoData);
				list2.Add(serverChaoData);
				list2.Add(serverChaoData);
				list2.Add(serverChaoData);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			serverSpinResultGeneral.AddItemState(list[k]);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			serverSpinResultGeneral.AddChaoState(list2[l]);
		}
		serverSpinResultGeneral.ItemWon = num;
		return serverSpinResultGeneral;
	}

	public Rect CreateGuiRectIn(Rect target, Rect rect, GUI_RECT_ANCHOR anchor = GUI_RECT_ANCHOR.LEFT_TOP)
	{
		return CreateGuiRectInRate(rect: new Rect(rect.x / target.width, rect.y / target.height, rect.width / target.width, rect.height / target.height), target: target, anchor: anchor);
	}

	public Rect CreateGuiRectInRate(Rect target, Rect rect, GUI_RECT_ANCHOR anchor = GUI_RECT_ANCHOR.LEFT_TOP)
	{
		rect.x *= target.width;
		rect.y *= target.height;
		rect.width *= target.width;
		rect.height *= target.height;
		Rect result = new Rect(rect.x, rect.y, rect.width, rect.height);
		float num = 0f;
		float num2 = 0f;
		switch (anchor)
		{
		case GUI_RECT_ANCHOR.CENTER:
			num = target.width * 0.5f - rect.width * 0.5f;
			num2 = target.height * 0.5f - rect.height * 0.5f;
			break;
		case GUI_RECT_ANCHOR.CENTER_LEFT:
			num = 0f;
			num2 = target.height * 0.5f - rect.height * 0.5f;
			break;
		case GUI_RECT_ANCHOR.CENTER_RIGHT:
			num = target.width - rect.width;
			num2 = target.height * 0.5f - rect.height * 0.5f;
			break;
		case GUI_RECT_ANCHOR.CENTER_TOP:
			num = target.width * 0.5f - rect.width * 0.5f;
			num2 = 0f;
			break;
		case GUI_RECT_ANCHOR.CENTER_BOTTOM:
			num = target.width * 0.5f - rect.width * 0.5f;
			num2 = target.height - rect.height;
			break;
		case GUI_RECT_ANCHOR.LEFT_TOP:
			num = 0f;
			num2 = 0f;
			break;
		case GUI_RECT_ANCHOR.LEFT_BOTTOM:
			num = 0f;
			num2 = target.height - rect.height;
			break;
		case GUI_RECT_ANCHOR.RIGHT_TOP:
			num = target.width - rect.width;
			num2 = 0f;
			break;
		case GUI_RECT_ANCHOR.RIGHT_BOTTOM:
			num = target.width - rect.width;
			num2 = target.height - rect.height;
			break;
		}
		num += target.x;
		num2 += target.y;
		result.x = num + rect.x;
		result.y = num2 + rect.y;
		result.width = rect.width;
		result.height = rect.height;
		return result;
	}

	public Rect CreateGuiRect(Rect rect, GUI_RECT_ANCHOR anchor = GUI_RECT_ANCHOR.LEFT_TOP)
	{
		Rect rect2 = new Rect(rect.x / 800f, rect.y / 450f, rect.width / 800f, rect.height / 450f);
		return CreateGuiRectRate(rect2, anchor);
	}

	public Rect CreateGuiRectRate(Rect rect, GUI_RECT_ANCHOR anchor = GUI_RECT_ANCHOR.LEFT_TOP)
	{
		if (m_camera == null)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, rect.height);
			rect2.x = rect.x;
			rect2.y = rect.y;
			rect2.width = rect.width;
			rect2.height = rect.height;
			return rect;
		}
		rect.x *= m_camera.pixelRect.xMax;
		rect.y *= m_camera.pixelRect.yMax;
		rect.width *= m_camera.pixelRect.xMax;
		rect.height *= m_camera.pixelRect.yMax;
		Rect result = new Rect(rect.x, rect.y, rect.width, rect.height);
		if (m_camera != null)
		{
			float num = 0f;
			float num2 = 0f;
			switch (anchor)
			{
			case GUI_RECT_ANCHOR.CENTER:
				num = m_camera.pixelRect.xMax * 0.5f - rect.width * 0.5f;
				num2 = m_camera.pixelRect.yMax * 0.5f - rect.height * 0.5f;
				break;
			case GUI_RECT_ANCHOR.CENTER_LEFT:
				num = 0f;
				num2 = m_camera.pixelRect.yMax * 0.5f - rect.height * 0.5f;
				break;
			case GUI_RECT_ANCHOR.CENTER_RIGHT:
				num = m_camera.pixelRect.xMax - rect.width;
				num2 = m_camera.pixelRect.yMax * 0.5f - rect.height * 0.5f;
				break;
			case GUI_RECT_ANCHOR.CENTER_TOP:
				num = m_camera.pixelRect.xMax * 0.5f - rect.width * 0.5f;
				num2 = 0f;
				break;
			case GUI_RECT_ANCHOR.CENTER_BOTTOM:
				num = m_camera.pixelRect.xMax * 0.5f - rect.width * 0.5f;
				num2 = m_camera.pixelRect.yMax - rect.height;
				break;
			case GUI_RECT_ANCHOR.LEFT_TOP:
				num = 0f;
				num2 = 0f;
				break;
			case GUI_RECT_ANCHOR.LEFT_BOTTOM:
				num = 0f;
				num2 = m_camera.pixelRect.yMax - rect.height;
				break;
			case GUI_RECT_ANCHOR.RIGHT_TOP:
				num = m_camera.pixelRect.xMax - rect.width;
				num2 = 0f;
				break;
			case GUI_RECT_ANCHOR.RIGHT_BOTTOM:
				num = m_camera.pixelRect.xMax - rect.width;
				num2 = m_camera.pixelRect.yMax - rect.height;
				break;
			}
			result.x = num + rect.x;
			result.y = num2 + rect.y;
			result.width = rect.width;
			result.height = rect.height;
		}
		return result;
	}

	public bool IsDebugGift()
	{
		return m_debugGiftItemId == 0;
	}

	public bool DebugRequestUpPoint(int rsring, int ring = 0, int energy = 0)
	{
		NetDebugUpdPointData netDebugUpdPointData = new NetDebugUpdPointData(energy, 0, ring, 0, rsring, 0);
		netDebugUpdPointData.Request();
		return true;
	}

	public bool DebugRequestGiftItem(int itemId, int num, GameObject callbackObject, bool response = true)
	{
		PopLog("item:" + itemId + "×" + num + " response:" + response, 0f, 0f);
		if (response)
		{
			if (!IsDebugGift())
			{
				return false;
			}
			m_debugGiftItemId = itemId;
			m_debugGiftCallback = callbackObject;
		}
		NetDebugAddOpeMessage.OpeMsgInfo opeMsgInfo = new NetDebugAddOpeMessage.OpeMsgInfo();
		opeMsgInfo.userID = SystemSaveManager.GetGameID();
		opeMsgInfo.messageKind = 1;
		opeMsgInfo.infoId = 0;
		opeMsgInfo.itemId = itemId;
		opeMsgInfo.numItem = num;
		opeMsgInfo.additionalInfo1 = 0;
		opeMsgInfo.additionalInfo2 = 1;
		opeMsgInfo.msgTitle = "debug";
		opeMsgInfo.msgContent = "Debug Gift Item " + itemId;
		opeMsgInfo.msgImageId = string.Empty + opeMsgInfo.itemId;
		NetDebugAddOpeMessage netDebugAddOpeMessage = new NetDebugAddOpeMessage(opeMsgInfo);
		if (response)
		{
			StartCoroutine(NetworkRequest(netDebugAddOpeMessage, AddOpeMessageCallback, NetworkFailedCallback));
		}
		else
		{
			netDebugAddOpeMessage.Request();
		}
		return true;
	}

	private IEnumerator NetworkRequest(NetBase request, NetworkRequestSuccessCallback successCallback, NetworkRequestFailedCallback failedCallback)
	{
		request.Request();
		while (request.IsExecuting())
		{
			yield return null;
		}
		if (request.IsSucceeded())
		{
			if (successCallback != null)
			{
				successCallback();
			}
		}
		else if (failedCallback != null)
		{
			failedCallback(request.resultStCd);
		}
	}

	private void AddOpeMessageCallback()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetMessageList(base.gameObject);
		}
	}

	private void ServerGetMessageList_Succeeded(MsgGetMessageListSucceed msg)
	{
		if (m_debugGiftCallback == null)
		{
			m_debugGiftItemId = 0;
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			return;
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (!(loggedInServerInterface != null))
		{
			return;
		}
		List<ServerOperatorMessageEntry> operatorMessageList = ServerInterface.OperatorMessageList;
		if (operatorMessageList == null || operatorMessageList.Count <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		foreach (ServerOperatorMessageEntry item in operatorMessageList)
		{
			if (item.m_presentState != null && item.m_presentState.m_itemId == m_debugGiftItemId && item.m_messageId > 0)
			{
				list2.Add(item.m_messageId);
			}
		}
		if (list.Count > 0 || list2.Count > 0)
		{
			loggedInServerInterface.RequestServerUpdateMessage(list, list2, base.gameObject);
			return;
		}
		m_debugGiftCallback.SendMessage("DebugRequestGiftItem_Failed", SendMessageOptions.DontRequireReceiver);
		m_debugGiftCallback = null;
		m_debugGiftItemId = 0;
	}

	private void ServerUpdateMessage_Succeeded(MsgUpdateMesseageSucceed msg)
	{
		if (m_debugGiftCallback != null)
		{
			m_debugGiftCallback.SendMessage("DebugRequestGiftItem_Succeeded", SendMessageOptions.DontRequireReceiver);
		}
		m_debugGiftCallback = null;
		m_debugGiftItemId = 0;
	}

	private void NetworkFailedCallback(ServerInterface.StatusCode statusCode)
	{
		if (m_debugGiftCallback != null)
		{
			m_debugGiftCallback.SendMessage("DebugRequestGiftItem_Failed", SendMessageOptions.DontRequireReceiver);
		}
		m_debugGiftCallback = null;
		m_debugGiftItemId = 0;
	}

	private void NetworkFailedMileCallback(ServerInterface.StatusCode statusCode)
	{
		if (m_debugGiftCallback != null)
		{
			m_debugGiftCallback.SendMessage("DebugUpdMileageData_Failed", SendMessageOptions.DontRequireReceiver);
		}
		m_debugGiftCallback = null;
		m_debugGiftItemId = 0;
	}

	public bool DebugStoryWindow()
	{
		int num = 5;
		GeneralWindow.CInfo.Event[] array = new GeneralWindow.CInfo.Event[num];
		int episode = 1;
		int pre_episode = 1;
		if (debugSceneLoader != null)
		{
			MileageMapText.Load(debugSceneLoader, episode, pre_episode);
		}
		for (int i = 0; i < num; i++)
		{
			string empty = string.Empty;
			MileageMapText.GetText(episode, empty);
			if (string.IsNullOrEmpty(empty))
			{
				empty = "NoText";
			}
			array[i] = new GeneralWindow.CInfo.Event
			{
				bgmCueName = string.Empty,
				seCueName = string.Empty
			};
		}
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "DebugStoryWindow";
		info.buttonType = GeneralWindow.ButtonType.OkNextSkip;
		info.caption = "DebugStory";
		info.events = array;
		info.isNotPlaybackDefaultBgm = true;
		info.isSpecialEvent = true;
		GeneralWindow.Create(info);
		return true;
	}
}
