using App.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SaveData
{
	public class SystemData
	{
		public enum DeckType
		{
			CHARA_MAIN,
			CHARA_SUB,
			CHAO_MAIN,
			CHAO_SUB,
			YOBI_A,
			YOBI_B,
			NUM
		}

		public enum FlagStatus
		{
			NONE = -1,
			ROULETTE_RULE_EXPLAINED,
			TUTORIAL_BOSS_PRESENT,
			TUTORIAL_END,
			TUTORIAL_BOSS_MAP_1,
			TUTORIAL_BOSS_MAP_2,
			TUTORIAL_BOSS_MAP_3,
			TUTORIAL_ANOTHER_CHARA,
			INVITE_FRIEND_SEQUENCE_END,
			RECOMMEND_REVIEW_END,
			SUB_CHARA_ITEM_EXPLAINED,
			ANOTHER_CHARA_EXPLAINED,
			CHARA_LEVEL_UP_EXPLAINED,
			FRIEDN_ACCEPT_INVITE,
			TUTORIAL_RANKING,
			FACEBOOK_FRIEND_INIT,
			TUTORIAL_FEVER_BOSS,
			FIRST_LAUNCH_TUTORIAL_END,
			TUTORIAL_EQIP_ITEM_END
		}

		public enum ItemTutorialFlagStatus
		{
			NONE = -1,
			INVINCIBLE,
			BARRIER,
			MAGNET,
			TRAMPOLINE,
			COMBO,
			LASER,
			DRILL,
			ASTEROID
		}

		public enum CharaTutorialFlagStatus
		{
			NONE = -1,
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
			CHARA_0,
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
			CHARA_28
		}

		public enum ActionTutorialFlagStatus
		{
			NONE = -1,
			ACTION_1
		}

		public enum QuickModeTutorialFlagStatus
		{
			NONE = -1,
			QUICK_1
		}

		public enum PushNoticeFlagStatus
		{
			NONE = -1,
			EVENT_INFO,
			CHALLENGE_INFO,
			FRIEND_INFO,
			NUM
		}

		public const int MAX_STOCK_DECK = 6;

		public int chaoSortType01;

		public int chaoSortType02;

		public List<string> fbFriends = new List<string>();

		public int version
		{
			get;
			set;
		}

		public int bgmVolume
		{
			get;
			set;
		}

		public int seVolume
		{
			get;
			set;
		}

		public int achievementIncentiveCount
		{
			get;
			set;
		}

		public int iap
		{
			get;
			set;
		}

		public bool pushNotice
		{
			get;
			set;
		}

		public bool lightMode
		{
			get;
			set;
		}

		public bool highTexture
		{
			get;
			set;
		}

		public string noahId
		{
			get;
			set;
		}

		public string purchasedRecipt
		{
			get;
			set;
		}

		public string country
		{
			get;
			set;
		}

		public string deckData
		{
			get;
			set;
		}

		public string facebookTime
		{
			get;
			set;
		}

		public string gameStartTime
		{
			get;
			set;
		}

		public int playCount
		{
			get;
			set;
		}

		public string loginRankigTime
		{
			get;
			set;
		}

		public int achievementCancelCount
		{
			get;
			set;
		}

		public Bitset32 flags
		{
			get;
			set;
		}

		public Bitset32 itemTutorialFlags
		{
			get;
			set;
		}

		public Bitset32 charaTutorialFlags
		{
			get;
			set;
		}

		public Bitset32 actionTutorialFlags
		{
			get;
			set;
		}

		public Bitset32 quickModeTutorialFlags
		{
			get;
			set;
		}

		public Bitset32 pushNoticeFlags
		{
			get;
			set;
		}

		public int pictureShowEventId
		{
			get;
			set;
		}

		public int pictureShowProgress
		{
			get;
			set;
		}

		public int pictureShowEmergeRaidBossProgress
		{
			get;
			set;
		}

		public int pictureShowRaidBossFirstBattle
		{
			get;
			set;
		}

		public long currentRaidDrawIndex
		{
			get;
			set;
		}

		public bool raidEntryFlag
		{
			get;
			set;
		}

		private static string deckDefalut
		{
			get
			{
				int num = 6;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Empty + 300000 + ",");
				for (int i = 0; i < num - 1; i++)
				{
					stringBuilder.Append("-1,");
				}
				return stringBuilder.ToString();
			}
		}

		public SystemData()
		{
			Init();
		}

		public void SetFlagStatus(FlagStatus status, bool flag)
		{
			flags = flags.Set((int)status, flag);
		}

		public bool IsFlagStatus(FlagStatus status)
		{
			return flags.Test((int)status);
		}

		public void SetFlagStatus(ItemTutorialFlagStatus status, bool flag)
		{
			itemTutorialFlags = itemTutorialFlags.Set((int)status, flag);
		}

		public bool IsFlagStatus(ItemTutorialFlagStatus status)
		{
			return itemTutorialFlags.Test((int)status);
		}

		public void SetFlagStatus(CharaTutorialFlagStatus status, bool flag)
		{
			charaTutorialFlags = charaTutorialFlags.Set((int)status, flag);
		}

		public bool IsFlagStatus(CharaTutorialFlagStatus status)
		{
			return charaTutorialFlags.Test((int)status);
		}

		public void SetFlagStatus(ActionTutorialFlagStatus status, bool flag)
		{
			actionTutorialFlags = actionTutorialFlags.Set((int)status, flag);
		}

		public bool IsFlagStatus(ActionTutorialFlagStatus status)
		{
			return actionTutorialFlags.Test((int)status);
		}

		public void SetFlagStatus(QuickModeTutorialFlagStatus status, bool flag)
		{
			quickModeTutorialFlags = quickModeTutorialFlags.Set((int)status, flag);
		}

		public bool IsFlagStatus(QuickModeTutorialFlagStatus status)
		{
			return quickModeTutorialFlags.Test((int)status);
		}

		public void SetFlagStatus(PushNoticeFlagStatus status, bool flag)
		{
			pushNoticeFlags = pushNoticeFlags.Set((int)status, flag);
		}

		public bool IsFlagStatus(PushNoticeFlagStatus status)
		{
			return pushNoticeFlags.Test((int)status);
		}

		public void Init(int ver)
		{
			Init();
			version = ver;
		}

		public void Init()
		{
			iap = 0;
			version = 0;
			bgmVolume = 100;
			seVolume = 100;
			achievementIncentiveCount = 0;
			pushNotice = false;
			lightMode = false;
			highTexture = false;
			noahId = string.Empty;
			purchasedRecipt = string.Empty;
			country = string.Empty;
			deckData = DeckAllDefalut();
			facebookTime = DateTime.Now.ToString();
			gameStartTime = DateTime.Now.ToString();
			playCount = 0;
			loginRankigTime = string.Empty;
			achievementCancelCount = 0;
			flags = new Bitset32(0u);
			itemTutorialFlags = new Bitset32(0u);
			charaTutorialFlags = new Bitset32(0u);
			actionTutorialFlags = new Bitset32(0u);
			quickModeTutorialFlags = new Bitset32(0u);
			pushNoticeFlags = new Bitset32(0u);
			SetFlagStatus(PushNoticeFlagStatus.EVENT_INFO, false);
			SetFlagStatus(PushNoticeFlagStatus.CHALLENGE_INFO, false);
			SetFlagStatus(PushNoticeFlagStatus.FRIEND_INFO, false);
			pictureShowEventId = -1;
			pictureShowProgress = -1;
			pictureShowEmergeRaidBossProgress = -1;
			pictureShowRaidBossFirstBattle = -1;
			currentRaidDrawIndex = -1L;
			raidEntryFlag = false;
			chaoSortType01 = 0;
			chaoSortType02 = 0;
		}

		public static string DeckAllDefalut()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 6; i++)
			{
				stringBuilder.Append(deckDefalut);
			}
			return stringBuilder.ToString();
		}

		public bool CheckDeck()
		{
			if (string.IsNullOrEmpty(deckData))
			{
				return false;
			}
			bool result = false;
			string[] array = deckData.Split(',');
			if (array != null && array.Length > 0 && array.Length >= 36)
			{
				result = true;
			}
			return result;
		}

		public bool CheckExsitDeck()
		{
			SaveDataManager instance = SaveDataManager.Instance;
			CharaType mainChara = instance.PlayerData.MainChara;
			CharaType subChara = instance.PlayerData.SubChara;
			int mainChaoID = instance.PlayerData.MainChaoID;
			int subChaoID = instance.PlayerData.SubChaoID;
			for (int i = 0; i < 6; i++)
			{
				CharaType mainCharaType;
				CharaType subCharaType;
				int mainChaoId;
				int subChaoId;
				GetDeckData(i, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
				if (mainChara == mainCharaType && subChara == subCharaType && mainChaoID == mainChaoId && subChaoID == subChaoId)
				{
					return true;
				}
			}
			return false;
		}

		public int GetDeckCurrentStockIndex()
		{
			int result = 0;
			SaveDataManager instance = SaveDataManager.Instance;
			CharaType mainChara = instance.PlayerData.MainChara;
			CharaType subChara = instance.PlayerData.SubChara;
			int mainChaoID = instance.PlayerData.MainChaoID;
			int subChaoID = instance.PlayerData.SubChaoID;
			for (int i = 0; i < 6; i++)
			{
				CharaType mainCharaType;
				CharaType subCharaType;
				int mainChaoId;
				int subChaoId;
				GetDeckData(i, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
				if (mainChara == mainCharaType && subChara == subCharaType && mainChaoID == mainChaoId && subChaoID == subChaoId)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public void RestDeckData(int stock)
		{
			SaveDeckData(stock, CharaType.SONIC, CharaType.UNKNOWN, -1, -1);
		}

		public void SaveDeckData(int stock, CharaType currentMainCharaType, CharaType currentSubCharaType, int currentMainId, int currentSubId)
		{
			if (stock >= 0 && stock < 6)
			{
				int num = -1;
				int id = -1;
				ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(currentMainCharaType);
				ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(currentSubCharaType);
				if (serverCharacterState != null)
				{
					num = serverCharacterState.Id;
				}
				if (serverCharacterState2 != null)
				{
					id = serverCharacterState2.Id;
				}
				if (num < 0)
				{
					num = 300000;
				}
				SetDeckId(stock, DeckType.CHARA_MAIN, num);
				SetDeckId(stock, DeckType.CHARA_SUB, id);
				SetDeckId(stock, DeckType.CHAO_MAIN, currentMainId);
				SetDeckId(stock, DeckType.CHAO_SUB, currentSubId);
				SetDeckId(stock, DeckType.YOBI_A, -1);
				SetDeckId(stock, DeckType.YOBI_B, -1);
				SystemSaveManager.Save();
			}
		}

		public void SaveDeckDataChara(int stock)
		{
			if (stock >= 0 && stock < 6)
			{
				SaveDataManager instance = SaveDataManager.Instance;
				int id = -1;
				int id2 = -1;
				ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(instance.PlayerData.MainChara);
				ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(instance.PlayerData.SubChara);
				if (serverCharacterState != null)
				{
					id = serverCharacterState.Id;
				}
				if (serverCharacterState2 != null)
				{
					id2 = serverCharacterState2.Id;
				}
				SetDeckId(stock, DeckType.CHARA_MAIN, id);
				SetDeckId(stock, DeckType.CHARA_SUB, id2);
				SetDeckId(stock, DeckType.YOBI_A, -1);
				SetDeckId(stock, DeckType.YOBI_B, -1);
				SystemSaveManager.Save();
			}
		}

		public void SaveDeckDataChao(int stock)
		{
			if (stock >= 0 && stock < 6)
			{
				SaveDataManager instance = SaveDataManager.Instance;
				int mainChaoID = instance.PlayerData.MainChaoID;
				int subChaoID = instance.PlayerData.SubChaoID;
				SetDeckId(stock, DeckType.CHAO_MAIN, mainChaoID);
				SetDeckId(stock, DeckType.CHAO_SUB, subChaoID);
				SetDeckId(stock, DeckType.YOBI_A, -1);
				SetDeckId(stock, DeckType.YOBI_B, -1);
				SystemSaveManager.Save();
			}
		}

		public bool IsSaveDeckData(int stock)
		{
			if (stock < 0 || stock >= 6)
			{
				return false;
			}
			bool result = true;
			CharaType mainCharaType;
			CharaType subCharaType;
			int mainChaoId;
			int subChaoId;
			GetDeckData(stock, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
			if (mainCharaType == CharaType.SONIC && subCharaType == CharaType.UNKNOWN && mainChaoId == -1 && subChaoId == -1)
			{
				result = false;
			}
			return result;
		}

		public int GetCurrentDeckData(out CharaType mainCharaType, out CharaType subCharaType, out int mainChaoId, out int subChaoId)
		{
			int deckCurrentStockIndex = GetDeckCurrentStockIndex();
			GetDeckData(deckCurrentStockIndex, out mainCharaType, out subCharaType, out mainChaoId, out subChaoId);
			return deckCurrentStockIndex;
		}

		public int GetCurrentDeckData(out CharaType mainCharaType, out CharaType subCharaType)
		{
			int deckCurrentStockIndex = GetDeckCurrentStockIndex();
			GetDeckData(deckCurrentStockIndex, out mainCharaType, out subCharaType);
			return deckCurrentStockIndex;
		}

		public void GetDeckData(int stock, out CharaType mainCharaType, out CharaType subCharaType, out int mainChaoId, out int subChaoId)
		{
			int deckId = GetDeckId(stock, DeckType.CHARA_MAIN);
			int deckId2 = GetDeckId(stock, DeckType.CHARA_SUB);
			mainCharaType = new ServerItem((ServerItem.Id)deckId).charaType;
			subCharaType = new ServerItem((ServerItem.Id)deckId2).charaType;
			mainChaoId = GetDeckId(stock, DeckType.CHAO_MAIN);
			subChaoId = GetDeckId(stock, DeckType.CHAO_SUB);
		}

		public void GetDeckData(int stock, out CharaType mainCharaType, out CharaType subCharaType)
		{
			int deckId = GetDeckId(stock, DeckType.CHARA_MAIN);
			int deckId2 = GetDeckId(stock, DeckType.CHARA_SUB);
			mainCharaType = new ServerItem((ServerItem.Id)deckId).charaType;
			subCharaType = new ServerItem((ServerItem.Id)deckId2).charaType;
		}

		public void GetDeckData(int stock, out int mainChaoId, out int subChaoId)
		{
			mainChaoId = GetDeckId(stock, DeckType.CHAO_MAIN);
			subChaoId = GetDeckId(stock, DeckType.CHAO_SUB);
		}

		private int GetDeckId(int index, DeckType type)
		{
			if (string.IsNullOrEmpty(deckData))
			{
				deckData = DeckAllDefalut();
			}
			int num = -1;
			string[] array = deckData.Split(',');
			int num2 = 6;
			int num3 = index * num2;
			if (array.Length >= 6 * num2 && array.Length > num3 && type != DeckType.NUM)
			{
				string s = array[(int)(num3 + type)];
				num = int.Parse(s);
			}
			if (num < 0)
			{
				num = -1;
			}
			return num;
		}

		private bool SetDeckId(int index, DeckType type, int id)
		{
			if (string.IsNullOrEmpty(deckData))
			{
				deckData = DeckAllDefalut();
			}
			bool result = false;
			string[] array = deckData.Split(',');
			int num = 6;
			int num2 = index * num;
			if (array.Length >= 6 * num && array.Length > num2 && type != DeckType.NUM)
			{
				array[(int)(num2 + type)] = id.ToString();
				deckData = string.Empty;
				for (int i = 0; i < array.Length; i++)
				{
					deckData = deckData + array[i] + ",";
				}
				result = true;
			}
			return result;
		}

		private bool IsFacebookWindowOrg()
		{
			bool result = false;
			if (string.IsNullOrEmpty(facebookTime))
			{
				result = true;
			}
			else
			{
				DateTime now = DateTime.Now;
				DateTime t = Convert.ToDateTime(facebookTime, DateTimeFormatInfo.InvariantInfo);
				if (now > t)
				{
					result = true;
				}
			}
			return result;
		}

		public bool IsFacebookWindow()
		{
			return IsFacebookWindowOrg();
		}

		public void SetFacebookWindow(bool isActive, float hideTime)
		{
			if (isActive)
			{
				facebookTime = null;
			}
			else
			{
				facebookTime = DateTime.Now.AddHours(hideTime).ToString();
			}
		}

		public void SetFacebookWindow(bool isActive)
		{
			if (isActive)
			{
				facebookTime = null;
			}
			else
			{
				facebookTime = DateTime.Now.AddHours(48.0).ToString();
			}
		}

		public bool CheckLoginTime()
		{
			bool result = false;
			if (string.IsNullOrEmpty(loginRankigTime))
			{
				result = true;
			}
			else
			{
				DateTime currentTime = NetBase.GetCurrentTime();
				DateTime d = Convert.ToDateTime(loginRankigTime, DateTimeFormatInfo.InvariantInfo);
				TimeSpan timeSpan = currentTime - d;
				Debug.Log("LoginRanking Span TotalHours =" + timeSpan.TotalHours);
				if (timeSpan.TotalHours >= 24.0)
				{
					result = true;
				}
			}
			return result;
		}

		public void SetLoginTime()
		{
			loginRankigTime = NetBase.GetCurrentTime().ToString();
			Debug.Log("LoginRankingTime=" + loginRankigTime);
		}

		public bool IsNewUser()
		{
			if (gameStartTime == null)
			{
				return true;
			}
			DateTime now = DateTime.Now;
			DateTime t = Convert.ToDateTime(gameStartTime, DateTimeFormatInfo.InvariantInfo).AddHours(24.0);
			if (now > t)
			{
				return false;
			}
			return true;
		}

		public void CopyTo(SystemData dst, bool temp)
		{
			dst.version = version;
			dst.bgmVolume = bgmVolume;
			dst.seVolume = seVolume;
			dst.achievementIncentiveCount = achievementIncentiveCount;
			dst.pushNotice = pushNotice;
			dst.lightMode = lightMode;
			dst.highTexture = highTexture;
			dst.noahId = noahId;
			dst.purchasedRecipt = purchasedRecipt;
			dst.country = country;
			dst.flags = new Bitset32(flags);
			dst.itemTutorialFlags = new Bitset32(itemTutorialFlags);
			dst.charaTutorialFlags = new Bitset32(charaTutorialFlags);
			dst.actionTutorialFlags = new Bitset32(actionTutorialFlags);
			dst.quickModeTutorialFlags = new Bitset32(quickModeTutorialFlags);
			dst.pushNoticeFlags = new Bitset32(pushNoticeFlags);
			dst.deckData = deckData;
			dst.pictureShowEventId = pictureShowEventId;
			dst.pictureShowProgress = pictureShowProgress;
			dst.pictureShowEmergeRaidBossProgress = pictureShowEmergeRaidBossProgress;
			dst.pictureShowRaidBossFirstBattle = pictureShowRaidBossFirstBattle;
			dst.currentRaidDrawIndex = currentRaidDrawIndex;
			dst.raidEntryFlag = raidEntryFlag;
			dst.chaoSortType01 = chaoSortType01;
			dst.chaoSortType02 = chaoSortType02;
			dst.facebookTime = facebookTime;
			dst.gameStartTime = gameStartTime;
			dst.playCount = playCount;
			dst.loginRankigTime = loginRankigTime;
			dst.achievementCancelCount = achievementCancelCount;
		}
	}
}
