using App;
using Message;
using SaveData;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RankingUtil
{
	public enum UserDataType
	{
		RANKING,
		RAID_BOSS,
		DAILY_BATTLE,
		RANK_UP
	}

	public enum RankingMode
	{
		ENDLESS,
		QUICK,
		COUNT
	}

	public enum RankingScoreType
	{
		HIGH_SCORE,
		TOTAL_SCORE,
		NONE
	}

	public enum RankChange
	{
		NONE,
		STAY,
		UP,
		DOWN
	}

	public enum RankingRankerType
	{
		FRIEND,
		ALL,
		RIVAL,
		HISTORY,
		SP_ALL,
		SP_FRIEND,
		COUNT
	}

	public class Ranker
	{
		public static SocialInterface s_socialInterface;

		private bool m_isBoxCollider = true;

		public int rankIndex
		{
			get;
			set;
		}

		public int rankIndexChanged
		{
			get;
			set;
		}

		public long score
		{
			get;
			set;
		}

		public long hiScore
		{
			get;
			set;
		}

		public int mapRank
		{
			private get;
			set;
		}

		public string dispMapRank
		{
			get
			{
				return (mapRank + 1).ToString("D3");
			}
		}

		public string userName
		{
			get;
			set;
		}

		public bool isFriend
		{
			get;
			set;
		}

		public bool isSentEnergy
		{
			get;
			set;
		}

		public CharaType charaType
		{
			get;
			set;
		}

		public CharaType charaSubType
		{
			get;
			set;
		}

		public int charaLevel
		{
			get;
			set;
		}

		public int mainChaoId
		{
			get;
			set;
		}

		public int subChaoId
		{
			get;
			set;
		}

		public int mainChaoLevel
		{
			get;
			set;
		}

		public int subChaoLevel
		{
			get;
			set;
		}

		public Env.Language language
		{
			get;
			set;
		}

		public int leagueIndex
		{
			get;
			set;
		}

		public string id
		{
			get;
			set;
		}

		public DateTime loginTime
		{
			get;
			set;
		}

		public UserDataType userDataType
		{
			get;
			set;
		}

		public bool isBoxCollider
		{
			get
			{
				return m_isBoxCollider;
			}
			set
			{
				m_isBoxCollider = value;
			}
		}

		public int rankIconIndex
		{
			get
			{
				return (rankIndex >= 1) ? ((rankIndex < 10) ? 1 : ((rankIndex < 50) ? 2 : ((rankIndex >= 150) ? (-1) : 3))) : 0;
			}
		}

		public int mainChaoRarity
		{
			get
			{
				return mainChaoId / 1000;
			}
		}

		public int subChaoRarity
		{
			get
			{
				return subChaoId / 1000;
			}
		}

		public bool isMy
		{
			get
			{
				bool result = false;
				if (!string.IsNullOrEmpty(id))
				{
					string gameID = SystemSaveManager.GetGameID();
					if (!string.IsNullOrEmpty(gameID) && id == gameID)
					{
						result = true;
					}
				}
				return result;
			}
		}

		public Ranker(ServerDailyBattleData user)
		{
			id = user.userId;
			score = user.maxScore;
			hiScore = user.maxScore;
			userName = user.name;
			isSentEnergy = user.isSentEnergy;
			rankIndex = user.goOnWin;
			rankIndexChanged = user.goOnWin;
			mapRank = user.numRank;
			loginTime = NetUtil.GetLocalDateTime(user.loginTime);
			charaType = new ServerItem((ServerItem.Id)user.charaId).charaType;
			charaSubType = new ServerItem((ServerItem.Id)user.subCharaId).charaType;
			charaLevel = user.charaLevel;
			mainChaoId = new ServerItem((ServerItem.Id)user.mainChaoId).chaoId;
			subChaoId = new ServerItem((ServerItem.Id)user.subChaoId).chaoId;
			mainChaoLevel = user.mainChaoLevel;
			subChaoLevel = user.subChaoLevel;
			leagueIndex = user.league;
			language = user.language;
			userDataType = UserDataType.DAILY_BATTLE;
			if (s_socialInterface == null)
			{
				s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			}
			if (s_socialInterface != null)
			{
				isFriend = (SocialInterface.GetSocialUserDataFromGameId(s_socialInterface.FriendList, id) != null);
			}
			else
			{
				isFriend = (isSentEnergy || UnityEngine.Random.Range(0, 3) != 0);
			}
		}

		public Ranker(RaidBossUser user)
		{
			id = user.id;
			score = 0L;
			hiScore = 0L;
			userName = user.userName;
			isSentEnergy = user.isSentEnergy;
			rankIndex = user.rankIndex;
			rankIndexChanged = user.rankIndexChanged;
			mapRank = user.mapRank;
			loginTime = user.loginTime;
			charaType = user.charaType;
			charaSubType = user.charaSubType;
			charaLevel = user.charaLevel;
			mainChaoId = user.mainChaoId;
			subChaoId = user.subChaoId;
			mainChaoLevel = user.mainChaoLevel;
			subChaoLevel = user.subChaoLevel;
			leagueIndex = user.leagueIndex;
			language = user.language;
			userDataType = UserDataType.RAID_BOSS;
			if (s_socialInterface == null)
			{
				s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			}
			if (s_socialInterface != null)
			{
				isFriend = (SocialInterface.GetSocialUserDataFromGameId(s_socialInterface.FriendList, id) != null);
			}
			else
			{
				isFriend = (isSentEnergy || UnityEngine.Random.Range(0, 3) != 0);
			}
		}

		public Ranker(ServerLeaderboardEntry serverLeaderboardEntry)
		{
			id = serverLeaderboardEntry.m_hspId;
			score = serverLeaderboardEntry.m_score;
			hiScore = serverLeaderboardEntry.m_hiScore;
			userName = serverLeaderboardEntry.m_name;
			serverLeaderboardEntry.m_url = string.Empty;
			serverLeaderboardEntry.m_userData = 0;
			isSentEnergy = serverLeaderboardEntry.m_energyFlg;
			rankIndex = serverLeaderboardEntry.m_grade - 1;
			rankIndexChanged = serverLeaderboardEntry.m_gradeChanged;
			mapRank = serverLeaderboardEntry.m_numRank;
			loginTime = NetUtil.GetLocalDateTime(serverLeaderboardEntry.m_loginTime);
			charaType = new ServerItem((ServerItem.Id)serverLeaderboardEntry.m_charaId).charaType;
			charaSubType = new ServerItem((ServerItem.Id)serverLeaderboardEntry.m_subCharaId).charaType;
			charaLevel = serverLeaderboardEntry.m_charaLevel;
			mainChaoId = new ServerItem((ServerItem.Id)serverLeaderboardEntry.m_mainChaoId).chaoId;
			subChaoId = new ServerItem((ServerItem.Id)serverLeaderboardEntry.m_subChaoId).chaoId;
			mainChaoLevel = serverLeaderboardEntry.m_mainChaoLevel;
			subChaoLevel = serverLeaderboardEntry.m_subChaoLevel;
			leagueIndex = serverLeaderboardEntry.m_leagueIndex;
			language = serverLeaderboardEntry.m_language;
			userDataType = UserDataType.RANKING;
			if (s_socialInterface == null)
			{
				s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
			}
			if (s_socialInterface != null)
			{
				isFriend = (SocialInterface.GetSocialUserDataFromGameId(s_socialInterface.FriendList, id) != null);
			}
			else
			{
				isFriend = (isSentEnergy || UnityEngine.Random.Range(0, 3) != 0);
			}
		}

		public bool CheckRankerIdentity(Ranker target)
		{
			bool result = false;
			if (score == target.score && userName == target.userName && id == target.id && rankIndex == target.rankIndex)
			{
				result = true;
			}
			return result;
		}
	}

	public class RankingDataSet
	{
		private RankingMode m_rankingMode;

		private RankingScoreType m_rankingTargetRivalScoreType;

		private RankingScoreType m_rankingTargetSpScoreType = RankingScoreType.TOTAL_SCORE;

		private ServerWeeklyLeaderboardOptions m_rankingWeeklyLeaderboardOptions;

		private RankingDataContainer m_rankingDataContainer;

		private ServerLeagueData m_leagueData;

		public RankingMode rankingMode
		{
			get
			{
				return m_rankingMode;
			}
		}

		public RankingScoreType targetRivalScoreType
		{
			get
			{
				return m_rankingTargetRivalScoreType;
			}
		}

		public RankingScoreType targetSpScoreType
		{
			get
			{
				return m_rankingTargetSpScoreType;
			}
		}

		public ServerWeeklyLeaderboardOptions weeklyLeaderboardOptions
		{
			get
			{
				return m_rankingWeeklyLeaderboardOptions;
			}
		}

		public RankingDataContainer dataContainer
		{
			get
			{
				return m_rankingDataContainer;
			}
		}

		public ServerLeagueData leagueData
		{
			get
			{
				return m_leagueData;
			}
		}

		public RankingDataSet(ServerWeeklyLeaderboardOptions leaderboardOptions)
		{
			Setup(leaderboardOptions);
		}

		public void Setup(ServerWeeklyLeaderboardOptions leaderboardOptions)
		{
			int num = leaderboardOptions.mode;
			if (num < 0 || num >= 2)
			{
				num = 0;
			}
			m_rankingMode = (RankingMode)num;
			m_rankingTargetRivalScoreType = leaderboardOptions.rankingScoreType;
			m_rankingTargetSpScoreType = RankingScoreType.TOTAL_SCORE;
			m_rankingWeeklyLeaderboardOptions = new ServerWeeklyLeaderboardOptions();
			leaderboardOptions.CopyTo(m_rankingWeeklyLeaderboardOptions);
			m_rankingDataContainer = new RankingDataContainer();
			m_leagueData = null;
		}

		public void SetLeagueData(ServerLeagueData data)
		{
			m_leagueData = new ServerLeagueData();
			data.CopyTo(m_leagueData);
			Debug.Log(string.Concat("RankingDataSet SetLeagueData mode:", m_leagueData.rankinMode, " leagueId:", m_leagueData.leagueId, "  groupId:", m_leagueData.groupId, " !!!"));
		}

		public void Reset(RankingRankerType type)
		{
			if (m_rankingDataContainer != null)
			{
				m_rankingDataContainer.Reset(type);
			}
		}

		public void Reset()
		{
			if (m_rankingDataContainer != null)
			{
				m_rankingDataContainer.Reset();
			}
		}

		public void SaveRanking()
		{
			if (m_rankingDataContainer != null)
			{
				m_rankingDataContainer.SavePlayerRanking();
			}
		}

		public bool UpdateSendChallengeList(RankingRankerType type, string id)
		{
			bool result = false;
			if (m_rankingDataContainer != null)
			{
				result = m_rankingDataContainer.UpdateSendChallengeList(type, id);
			}
			return result;
		}

		public RankChange GetRankChange(RankingScoreType scoreType, RankingRankerType rankerType)
		{
			RankChange result = RankChange.NONE;
			if (m_rankingDataContainer != null)
			{
				result = m_rankingDataContainer.GetRankChange(scoreType, rankerType);
			}
			return result;
		}

		public RankChange GetRankChange(RankingScoreType scoreType, RankingRankerType rankerType, out int currentRank, out int oldRank)
		{
			RankChange result = RankChange.NONE;
			currentRank = -1;
			oldRank = -1;
			if (m_rankingDataContainer != null)
			{
				result = m_rankingDataContainer.GetRankChange(scoreType, rankerType, out currentRank, out oldRank);
			}
			return result;
		}

		public void ResetRankChange(RankingScoreType scoreType, RankingRankerType rankerType)
		{
			if (m_rankingDataContainer != null)
			{
				m_rankingDataContainer.ResetRankChange(scoreType, rankerType);
			}
		}

		public void AddRankerList(MsgGetLeaderboardEntriesSucceed msg)
		{
			if (m_rankingDataContainer != null)
			{
				m_rankingDataContainer.AddRankerList(msg);
			}
		}

		public List<Ranker> GetRankerList(RankingRankerType rankerType, RankingScoreType scoreType, int page)
		{
			List<Ranker> result = null;
			if (m_rankingDataContainer != null)
			{
				result = m_rankingDataContainer.GetRankerList(rankerType, scoreType, page);
			}
			return result;
		}
	}

	public const int RANKING_GET_LIMIT = 30000;

	private static readonly LeagueTypeParam[] LEAGUE_PARAMS = new LeagueTypeParam[21]
	{
		new LeagueTypeParam(LeagueCategory.F, "F"),
		new LeagueTypeParam(LeagueCategory.F, "F"),
		new LeagueTypeParam(LeagueCategory.F, "F"),
		new LeagueTypeParam(LeagueCategory.E, "E"),
		new LeagueTypeParam(LeagueCategory.E, "E"),
		new LeagueTypeParam(LeagueCategory.E, "E"),
		new LeagueTypeParam(LeagueCategory.D, "D"),
		new LeagueTypeParam(LeagueCategory.D, "D"),
		new LeagueTypeParam(LeagueCategory.D, "D"),
		new LeagueTypeParam(LeagueCategory.C, "C"),
		new LeagueTypeParam(LeagueCategory.C, "C"),
		new LeagueTypeParam(LeagueCategory.C, "C"),
		new LeagueTypeParam(LeagueCategory.B, "B"),
		new LeagueTypeParam(LeagueCategory.B, "B"),
		new LeagueTypeParam(LeagueCategory.B, "B"),
		new LeagueTypeParam(LeagueCategory.A, "A"),
		new LeagueTypeParam(LeagueCategory.A, "A"),
		new LeagueTypeParam(LeagueCategory.A, "A"),
		new LeagueTypeParam(LeagueCategory.S, "S"),
		new LeagueTypeParam(LeagueCategory.S, "S"),
		new LeagueTypeParam(LeagueCategory.S, "S")
	};

	public static SocialInterface s_socialInterface = null;

	private static RankingMode m_currentRankingMode = RankingMode.COUNT;

	public static RankingMode currentRankingMode
	{
		get
		{
			RankingMode result = RankingMode.ENDLESS;
			if (m_currentRankingMode != RankingMode.COUNT)
			{
				result = m_currentRankingMode;
			}
			else
			{
				Debug.Log("RankingUtil currentMode error !!!!!");
			}
			return result;
		}
	}

	public static void SetCurrentRankingMode(RankingMode mode)
	{
		m_currentRankingMode = mode;
		Debug.Log("RankingUtil SetCurrentRankingMode  currentRankingMode:" + m_currentRankingMode);
	}

	public static bool IsRankingUserFrontAndBack(RankingScoreType scoreType, RankingRankerType rankerType, int page)
	{
		bool result = false;
		if (rankerType != RankingRankerType.RIVAL && page == 0 && rankerType == RankingRankerType.SP_ALL)
		{
			result = true;
		}
		return result;
	}

	public static int GetRankingCode(RankingMode rankingMode, RankingScoreType scoreType, RankingRankerType rankerType)
	{
		int result = -1;
		if (rankingMode != RankingMode.COUNT && scoreType != RankingScoreType.NONE && rankerType != RankingRankerType.COUNT)
		{
			result = 1000 * (int)rankingMode;
			result += GetRankingType(scoreType, rankerType);
		}
		return result;
	}

	public static int GetRankingType(RankingScoreType scoreType, RankingRankerType rankerType)
	{
		if (scoreType == RankingScoreType.NONE || rankerType == RankingRankerType.COUNT)
		{
			return -1;
		}
		int num = -1;
		return (int)((int)rankerType * 2 + scoreType);
	}

	public static RankingMode GetRankerMode(int rankingType)
	{
		RankingMode result = RankingMode.ENDLESS;
		if (rankingType >= 1000)
		{
			result = (RankingMode)(rankingType / 1000);
		}
		return result;
	}

	public static RankingScoreType GetRankerScoreType(int rankingType)
	{
		rankingType %= 1000;
		RankingScoreType rankingScoreType = RankingScoreType.HIGH_SCORE;
		return (RankingScoreType)(rankingType % 2);
	}

	public static RankingRankerType GetRankerType(int rankingType)
	{
		rankingType %= 1000;
		RankingRankerType rankingRankerType = RankingRankerType.ALL;
		return (RankingRankerType)(rankingType / 2);
	}

	public static string GetLeagueName(LeagueType type)
	{
		if ((uint)type < 21u)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "league_" + (uint)type);
			if (text != null)
			{
				return text.text;
			}
		}
		return string.Empty;
	}

	public static LeagueCategory GetLeagueCategory(LeagueType type)
	{
		if ((uint)type < 21u)
		{
			return LEAGUE_PARAMS[(int)type].m_category;
		}
		return LeagueCategory.NONE;
	}

	public static string GetLeagueCategoryName(LeagueType type)
	{
		if ((uint)type < 21u)
		{
			return LEAGUE_PARAMS[(int)type].m_categoryName;
		}
		return string.Empty;
	}

	public static uint GetLeagueCategoryClass(LeagueType type)
	{
		if ((uint)type < 21u)
		{
			return (uint)type % 3u;
		}
		return 0u;
	}

	public static void GetMyLeagueData(RankingMode rankingMode, ref int leagueIndex, ref int upCount, ref int downCount)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			RankingDataSet rankingDataSet = SingletonGameObject<RankingManager>.Instance.GetRankingDataSet(rankingMode);
			if (rankingDataSet != null && rankingDataSet.leagueData != null)
			{
				leagueIndex = rankingDataSet.leagueData.leagueId;
				upCount = rankingDataSet.leagueData.numUp;
				downCount = rankingDataSet.leagueData.numDown;
			}
		}
	}

	public static void SetLeagueObject(RankingMode rankingMode, ref UISprite icon0, ref UISprite icon1, ref UILabel rankText0, ref UILabel rankText1)
	{
		int leagueIndex = 0;
		int upCount = 0;
		int downCount = 0;
		GetMyLeagueData(rankingMode, ref leagueIndex, ref upCount, ref downCount);
		icon0.spriteName = GetLeagueIconNameL((LeagueType)leagueIndex);
		icon1.spriteName = GetLeagueIconNameL2((LeagueType)leagueIndex);
		rankText0.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_league_tab_1").text, new Dictionary<string, string>
		{
			{
				"{PARAM_1}",
				GetLeagueName((LeagueType)leagueIndex)
			}
		});
		string empty = string.Empty;
		empty = ((RankingManager.EndlessRivalRankingScoreType != 0) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_total_score").text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_high_score").text);
		if (upCount == 0)
		{
			rankText1.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_league_tab_4").text, new Dictionary<string, string>
			{
				{
					"{SCORE}",
					empty
				},
				{
					"{PARAM_1}",
					downCount.ToString()
				}
			});
		}
		else if (downCount == 0)
		{
			rankText1.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_league_tab_3").text, new Dictionary<string, string>
			{
				{
					"{SCORE}",
					empty
				},
				{
					"{PARAM_1}",
					upCount.ToString()
				}
			});
		}
		else
		{
			rankText1.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_league_tab_2").text, new Dictionary<string, string>
			{
				{
					"{SCORE}",
					empty
				},
				{
					"{PARAM_1}",
					upCount.ToString()
				},
				{
					"{PARAM_2}",
					downCount.ToString()
				}
			});
		}
	}

	public static void SetLeagueObjectForMainMenu(RankingMode rankingMode, UISprite icon0, UISprite icon1, UILabel rankingText)
	{
		int leagueIndex = 0;
		int upCount = 0;
		int downCount = 0;
		GetMyLeagueData(rankingMode, ref leagueIndex, ref upCount, ref downCount);
		icon0.spriteName = GetLeagueIconName((LeagueType)leagueIndex);
		icon1.spriteName = GetLeagueIconName2((LeagueType)leagueIndex);
		int num = 0;
		RankingRankerType rankType = RankingRankerType.RIVAL;
		RankingScoreType scoreType = RankingScoreType.HIGH_SCORE;
		switch (rankingMode)
		{
		case RankingMode.ENDLESS:
			scoreType = RankingManager.EndlessRivalRankingScoreType;
			break;
		case RankingMode.QUICK:
			scoreType = RankingManager.QuickRivalRankingScoreType;
			break;
		}
		Ranker myRank = RankingManager.GetMyRank(rankingMode, rankType, scoreType);
		if (myRank != null)
		{
			num = myRank.rankIndex + 1;
		}
		rankingText.text = num.ToString("00");
	}

	public static string GetLeagueIconName(LeagueType leagueType)
	{
		return "ui_ranking_league_icon_s_" + GetLeagueCategoryName(leagueType).ToLower();
	}

	public static string GetLeagueIconName2(LeagueType leagueType)
	{
		return "ui_ranking_league_icon_s_" + GetLeagueCategoryClass(leagueType);
	}

	public static string GetLeagueIconNameL(LeagueType leagueType)
	{
		return "ui_ranking_league_icon_" + GetLeagueCategoryName(leagueType).ToLower();
	}

	public static string GetLeagueIconNameL2(LeagueType leagueType)
	{
		return "ui_ranking_league_icon_" + GetLeagueCategoryClass(leagueType);
	}

	public static Texture2D GetProfilePictureTexture(Ranker ranker, Action<Texture2D> callback)
	{
		return GetProfilePictureTexture(ranker.id, callback);
	}

	public static Texture2D GetProfilePictureTexture(string userId, Action<Texture2D> callback)
	{
		if (s_socialInterface == null)
		{
			s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		}
		Debug.Log("GetProfilePictureTexture.gameId:" + userId);
		string text = null;
		if (s_socialInterface != null)
		{
			SocialUserData socialUserDataFromGameId = SocialInterface.GetSocialUserDataFromGameId(s_socialInterface.FriendWithMeList, userId);
			if (socialUserDataFromGameId != null)
			{
				text = socialUserDataFromGameId.Id;
			}
		}
		Debug.Log("GetProfilePictureTexture.socialId:" + text);
		PlayerImageManager playerImageManager = GameObjectUtil.FindGameObjectComponent<PlayerImageManager>("PlayerImageManager");
		if (playerImageManager != null)
		{
			return playerImageManager.GetPlayerImage(text, string.Empty, callback);
		}
		return null;
	}

	public static List<Ranker> GetRankerList(MsgGetLeaderboardEntriesSucceed msg)
	{
		List<Ranker> list = new List<Ranker>();
		if (msg != null && msg.m_leaderboardEntries != null)
		{
			ServerLeaderboardEntries leaderboardEntries = msg.m_leaderboardEntries;
			if (leaderboardEntries.m_myLeaderboardEntry != null)
			{
				list.Add(new Ranker(leaderboardEntries.m_myLeaderboardEntry));
			}
			else
			{
				list.Add(null);
			}
			if (leaderboardEntries.m_leaderboardEntries != null && leaderboardEntries.m_leaderboardEntries.Count > 0)
			{
				List<ServerLeaderboardEntry> leaderboardEntries2 = leaderboardEntries.m_leaderboardEntries;
				if (leaderboardEntries2 != null)
				{
					int num = leaderboardEntries2.Count;
					if (!leaderboardEntries.IsRivalRanking() && leaderboardEntries.IsNext())
					{
						num = leaderboardEntries2.Count - 1;
					}
					for (int i = 0; i < leaderboardEntries2.Count && i < num; i++)
					{
						list.Add(new Ranker(leaderboardEntries2[i]));
					}
				}
			}
		}
		else
		{
			list.Add(null);
		}
		return list;
	}

	public static MsgGetLeaderboardEntriesSucceed InitRankingMsg(MsgGetLeaderboardEntriesSucceed msg)
	{
		int num = 4;
		MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = new MsgGetLeaderboardEntriesSucceed();
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries = new ServerLeaderboardEntries();
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_startIndex = msg.m_leaderboardEntries.m_startIndex;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first = msg.m_leaderboardEntries.m_first;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count = msg.m_leaderboardEntries.m_count;
		if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count > num)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count = num;
		}
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_rankingType = msg.m_leaderboardEntries.m_rankingType;
		if (msg.m_leaderboardEntries.m_friendIdList == null)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_friendIdList = null;
		}
		else
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_friendIdList = new string[msg.m_leaderboardEntries.m_friendIdList.Length];
		}
		if (msg.m_leaderboardEntries.m_myLeaderboardEntry == null)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry = null;
		}
		else
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry = msg.m_leaderboardEntries.m_myLeaderboardEntry.Clone();
		}
		if (msg.m_leaderboardEntries.m_leaderboardEntries == null)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries = null;
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries = 0;
		}
		else
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries = new List<ServerLeaderboardEntry>();
			int num2 = 0;
			foreach (ServerLeaderboardEntry leaderboardEntry in msg.m_leaderboardEntries.m_leaderboardEntries)
			{
				msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries.Add(leaderboardEntry.Clone());
				num2++;
				if (num <= num2)
				{
					break;
				}
			}
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries.Count;
		}
		return msgGetLeaderboardEntriesSucceed;
	}

	public static MsgGetLeaderboardEntriesSucceed CopyRankingMsg(MsgGetLeaderboardEntriesSucceed msg, MsgGetLeaderboardEntriesSucceed org = null)
	{
		MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = new MsgGetLeaderboardEntriesSucceed();
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries = new ServerLeaderboardEntries();
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_startIndex = msg.m_leaderboardEntries.m_startIndex;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first = msg.m_leaderboardEntries.m_first;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count = msg.m_leaderboardEntries.m_count;
		msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_rankingType = msg.m_leaderboardEntries.m_rankingType;
		if (msg.m_leaderboardEntries.m_friendIdList == null)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_friendIdList = null;
		}
		else
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_friendIdList = new string[msg.m_leaderboardEntries.m_friendIdList.Length];
		}
		if (msg.m_leaderboardEntries.m_myLeaderboardEntry == null)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry = null;
		}
		else
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry = msg.m_leaderboardEntries.m_myLeaderboardEntry.Clone();
		}
		if (msg.m_leaderboardEntries.m_leaderboardEntries == null)
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries = null;
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries = 0;
		}
		else
		{
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries = new List<ServerLeaderboardEntry>();
			foreach (ServerLeaderboardEntry leaderboardEntry in msg.m_leaderboardEntries.m_leaderboardEntries)
			{
				msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries.Add(leaderboardEntry.Clone());
			}
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries.Count;
		}
		if (org != null && (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first != org.m_leaderboardEntries.m_first || msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count != org.m_leaderboardEntries.m_count))
		{
			bool flag = false;
			bool flag2 = false;
			int num = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first + msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries;
			int num2 = org.m_leaderboardEntries.m_first + org.m_leaderboardEntries.m_resultTotalEntries;
			if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first <= org.m_leaderboardEntries.m_first && num >= num2)
			{
				return msgGetLeaderboardEntriesSucceed;
			}
			if (num == num2)
			{
				if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count <= msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries)
				{
					flag = true;
				}
			}
			else if (num > num2)
			{
				if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count <= msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries)
				{
					flag = true;
				}
			}
			else if (org.m_leaderboardEntries.m_count <= org.m_leaderboardEntries.m_resultTotalEntries)
			{
				flag = true;
			}
			List<ServerLeaderboardEntry> list = new List<ServerLeaderboardEntry>();
			if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first > org.m_leaderboardEntries.m_first)
			{
				flag2 = true;
				foreach (ServerLeaderboardEntry leaderboardEntry2 in org.m_leaderboardEntries.m_leaderboardEntries)
				{
					list.Add(leaderboardEntry2);
				}
			}
			else
			{
				flag2 = false;
				foreach (ServerLeaderboardEntry leaderboardEntry3 in msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries)
				{
					list.Add(leaderboardEntry3);
				}
			}
			int num3 = 0;
			int num4 = 0;
			if (flag2)
			{
				num3 = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first - 1;
				num4 = num2;
				if (num > num2)
				{
					num4 = num;
				}
				int count = list.Count;
				for (int i = num3; i < num4; i++)
				{
					if (i < count)
					{
						if (i - num3 < msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries.Count)
						{
							list[i] = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries[i - num3];
						}
					}
					else if (i - num3 < msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries.Count)
					{
						list.Add(msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries[i - num3]);
					}
				}
			}
			else
			{
				num3 = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first - org.m_leaderboardEntries.m_first;
				num4 = num - msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first;
				for (int j = num3; j < num4; j++)
				{
					if (j >= list.Count && j - num3 < org.m_leaderboardEntries.m_leaderboardEntries.Count)
					{
						list.Add(org.m_leaderboardEntries.m_leaderboardEntries[j - num3]);
					}
				}
			}
			if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first > org.m_leaderboardEntries.m_first)
			{
				msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_first = org.m_leaderboardEntries.m_first;
			}
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_leaderboardEntries = list;
			msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_resultTotalEntries = list.Count;
			if (flag)
			{
				msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count = list.Count;
			}
			else
			{
				msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count = list.Count - 1;
			}
			return msgGetLeaderboardEntriesSucceed;
		}
		return msgGetLeaderboardEntriesSucceed;
	}

	public static string[] GetFriendIdList()
	{
		string[] result = null;
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			result = SocialInterface.GetGameIdList(socialInterface.FriendList).ToArray();
		}
		return result;
	}

	public static string GetResetTime(TimeSpan span, bool isHeadText = true)
	{
		string str = string.Empty;
		if (span.Ticks <= 0)
		{
			return TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_sumup").text;
		}
		if (isHeadText)
		{
			str = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_reset").text + "\n";
		}
		return str + TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", (span.Days > 0) ? "ranking_reset_days" : ((span.Hours > 0) ? "ranking_reset_hours" : ((span.Minutes <= 0) ? "ranking_reset_seconds" : "ranking_reset_minutes"))).text, new Dictionary<string, string>
		{
			{
				"{DAYS}",
				span.Days.ToString()
			},
			{
				"{HOURS}",
				span.Hours.ToString()
			},
			{
				"{MINUTES}",
				span.Minutes.ToString()
			}
		});
	}

	public static bool ShowRankingChangeWindow(RankingMode rankingMode)
	{
		Debug.Log("ShowRankingChangeWindow");
		if (RankingResultBitWindow.Instance != null)
		{
			RankingResultBitWindow.Instance.Open(rankingMode);
			return true;
		}
		Debug.Log("ShowRankingChangeWindow error?");
		return false;
	}

	public static bool IsEndRankingChangeWindow()
	{
		if (RankingResultBitWindow.Instance != null)
		{
			return RankingResultBitWindow.Instance.IsEnd;
		}
		Debug.Log("IsEndRankingChangeWindow error?");
		return false;
	}

	public static string GetFriendIconSpriteName(Ranker ranker)
	{
		if (ranker != null && ranker.isFriend)
		{
			if (ranker.isSentEnergy)
			{
				return "ui_ranking_scroll_icon_friend_1";
			}
			return "ui_ranking_scroll_icon_friend_0";
		}
		return string.Empty;
	}

	public static string GetFriendIconSpriteName(RaidBossUser user)
	{
		if (user != null && user.isFriend)
		{
			if (user.isSentEnergy)
			{
				return "ui_ranking_scroll_icon_friend_1";
			}
			return "ui_ranking_scroll_icon_friend_0";
		}
		return string.Empty;
	}

	public static void DebugRankingChange()
	{
		Debug.Log("DebugRankingChange !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		RankingRankerType rankingRankerType = RankingRankerType.RIVAL;
		RankingScoreType endlessRivalRankingScoreType = RankingManager.EndlessRivalRankingScoreType;
		int currentRank = -1;
		int oldRank = -1;
		RankChange rankingRankChange = SingletonGameObject<RankingManager>.Instance.GetRankingRankChange(RankingMode.ENDLESS, endlessRivalRankingScoreType, rankingRankerType, out currentRank, out oldRank);
		string arg = string.Concat("set rankerType:", rankingRankerType, "  scoreType:", endlessRivalRankingScoreType);
		arg = arg + "\n  change:" + rankingRankChange;
		if (rankingRankChange != 0)
		{
			string text = arg;
			arg = text + "   " + oldRank + " → " + currentRank;
		}
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "debug_ranking_change";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = "Debug";
		info.message = arg;
		GeneralWindow.Create(info);
	}

	public static void DebugCurrentRanking(bool isEvent, long score)
	{
		long myScore;
		long myHiScore;
		int myLeague;
		bool currentRankingStatus = RankingManager.GetCurrentRankingStatus(RankingMode.ENDLESS, isEvent, out myScore, out myHiScore, out myLeague);
		long currentScore = score;
		if (isEvent)
		{
			currentScore = score + myScore;
		}
		int nextRank = 0;
		bool isHighScore;
		long nextRankScore;
		long prveRankScore;
		int currentHighScoreRank = RankingManager.GetCurrentHighScoreRank(RankingMode.ENDLESS, isEvent, ref currentScore, out isHighScore, out nextRankScore, out prveRankScore, out nextRank);
		string str = "set  isEvent:" + isEvent + "  score:" + score;
		str += "\n◆GetCurrentRankingStatus";
		str = str + "\n\u3000\u3000isStatus:" + currentRankingStatus;
		str = str + "\n\u3000\u3000myScore:" + myScore;
		str = str + "\n\u3000\u3000myLeague:" + GetLeagueName((LeagueType)myLeague);
		str = str + "\n◆GetCurrentHighScoreRank    currentScore:" + currentScore;
		str = str + "\n\u3000\u3000rank:" + currentHighScoreRank;
		str = str + "\n\u3000\u3000isHighScore:" + isHighScore;
		str = str + "\n\u3000\u3000nextRankScore:" + nextRankScore;
		str = str + "\n\u3000\u3000prveRankScore:" + prveRankScore;
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "debug_info";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = "Debug";
		info.message = str;
		GeneralWindow.Create(info);
	}
}
