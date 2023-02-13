using System.Collections.Generic;
using UnityEngine;

public class ServerInterface : MonoBehaviour
{
	public enum StatusCode
	{
		Ok = 0,
		ServerSecurityError = -19001,
		VersionDifference = -19002,
		DecryptionFailure = -19003,
		ParamHashDifference = -19004,
		ServerNextVersion = -19990,
		ServerMaintenance = -19997,
		ServerBusyError = -19998,
		ServerSystemError = -19999,
		RequestParamError = -10100,
		NotAvailablePlayer = -10101,
		MissingPlayer = -10102,
		ExpirationSession = -10103,
		PassWordError = -10104,
		InvalidSerialCode = -10105,
		UsedSerialCode = -10106,
		HspWebApiError = -10110,
		ApolloWebApiError = -10115,
		DataMismatch = -30120,
		MasterDataMismatch = -10121,
		NotEnoughRedStarRings = -20130,
		NotEnoughRings = -20131,
		NotEnoughEnergy = -20132,
		RouletteUseLimit = -30401,
		RouletteBoardReset = -30411,
		CharacterLevelLimit = -20601,
		AllChaoLevelLimit = -20602,
		AlreadyInvitedFriend = -30801,
		AlreadyRequestedEnergy = -30901,
		AlreadySentEnergy = -30902,
		ReceiveFailureMessage = -30910,
		AlreadyExistedPrePurchase = -11001,
		AlreadyRemovedPrePurchase = -11002,
		InvalidReceiptData = -11003,
		AlreadyProcessedReceipt = -11004,
		EnergyLimitPurchaseTrigger = -21010,
		NotStartEvent = -10201,
		AlreadyEndEvent = -10202,
		VersionForApplication = -999002,
		TimeOut = -7,
		OtherError = -8,
		NotReachability = -10,
		InvalidResponse = -20,
		CliendError = -400,
		InternalServerError = -500,
		HspPurchaseError = -600,
		ServerBusy = -700
	}

	public ServerLeaderboardEntry m_myLeaderboardEntry;

	private static ServerSettingState s_settingState;

	private static ServerLoginState s_loginState;

	private static ServerNextVersionState s_nextState;

	private static ServerPlayerState s_playerState;

	private static ServerFreeItemState s_freeItemState;

	private static ServerLoginBonusData s_loginBonusData;

	private static ServerNoticeInfo s_noticeInfo;

	private static ServerTickerInfo s_tickerInfo;

	private static ServerWheelOptions s_wheelOptions;

	private static ServerChaoWheelOptions s_chaoWheelOptions;

	private static List<ServerRingExchangeList> s_ringExchangeList;

	private static List<ServerConsumedCostData> s_consumedCostList;

	private static List<ServerConsumedCostData> s_costList;

	private static ServerMileageMapState s_mileageMapState;

	private static List<ServerMileageReward> s_mileageRewardList;

	private static List<ServerMileageFriendEntry> s_mileageFriendList;

	private static List<ServerDistanceFriendEntry> s_distanceFriendEntry;

	private static ServerCampaignState s_campaignState;

	private static List<ServerMessageEntry> s_messageList;

	private static List<ServerOperatorMessageEntry> s_operatorMessageList;

	private static ServerLeaderboardEntries s_leaderboardEntries;

	private static ServerLeaderboardEntries s_leaderboardEntriesRivalHighScore;

	private static ServerLeaderboardEntry s_leaderboardEntryRivalHighScoreTop;

	private static ServerPrizeState s_premiumRoulettePrizeList;

	private static ServerPrizeState s_specialRoulettePrizeList;

	private static ServerPrizeState s_raidRoulettePrizeList;

	private static ServerEventState s_eventState;

	private static List<ServerEventEntry> s_eventEntryList;

	private static List<ServerEventReward> s_eventRewardList;

	private static List<ServerRedStarItemState> s_redStartItemState;

	private static List<ServerRedStarItemState> s_redStartExchangeRingItemState;

	private static List<ServerRedStarItemState> s_redStartExchangeEnergyItemState;

	private static List<ServerRedStarItemState> s_redStartExchangeRaidbossEnergyItemState;

	private static ServerDailyChallengeState s_dailyChallengeState;

	private static List<ServerUserTransformData> s_userTransformDataList;

	private static string s_migrationPassword;

	private static ServerLeagueData s_leagueData;

	private static bool s_isCreated;

	public static ServerSettingState SettingState
	{
		get
		{
			return s_settingState;
		}
	}

	public static ServerLoginState LoginState
	{
		get
		{
			return s_loginState;
		}
	}

	public static ServerNextVersionState NextVersionState
	{
		get
		{
			return s_nextState;
		}
	}

	public static ServerFreeItemState FreeItemState
	{
		get
		{
			return s_freeItemState;
		}
	}

	public static ServerLoginBonusData LoginBonusData
	{
		get
		{
			return s_loginBonusData;
		}
	}

	public static ServerNoticeInfo NoticeInfo
	{
		get
		{
			return s_noticeInfo;
		}
	}

	public static ServerTickerInfo TickerInfo
	{
		get
		{
			return s_tickerInfo;
		}
	}

	public static ServerPlayerState PlayerState
	{
		get
		{
			return s_playerState;
		}
	}

	public static ServerWheelOptions WheelOptions
	{
		get
		{
			return s_wheelOptions;
		}
	}

	public static ServerChaoWheelOptions ChaoWheelOptions
	{
		get
		{
			return s_chaoWheelOptions;
		}
	}

	public static List<ServerRingExchangeList> RingExchangeList
	{
		get
		{
			return s_ringExchangeList;
		}
	}

	public static List<ServerConsumedCostData> ConsumedCostList
	{
		get
		{
			return s_consumedCostList;
		}
	}

	public static List<ServerConsumedCostData> CostList
	{
		get
		{
			return s_costList;
		}
	}

	public static ServerMileageMapState MileageMapState
	{
		get
		{
			return s_mileageMapState;
		}
	}

	public static List<ServerMileageReward> MileageRewardList
	{
		get
		{
			return s_mileageRewardList;
		}
	}

	public static List<ServerMileageFriendEntry> MileageFriendList
	{
		get
		{
			return s_mileageFriendList;
		}
	}

	public static ServerCampaignState CampaignState
	{
		get
		{
			return s_campaignState;
		}
	}

	public static List<ServerDistanceFriendEntry> DistanceFriendEntry
	{
		get
		{
			return s_distanceFriendEntry;
		}
	}

	public static ServerLeaderboardEntries LeaderboardEntries
	{
		get
		{
			return s_leaderboardEntries;
		}
	}

	public static ServerLeaderboardEntries LeaderboardEntriesRivalHighScore
	{
		get
		{
			return s_leaderboardEntriesRivalHighScore;
		}
	}

	public static ServerLeaderboardEntry LeaderboardEntryRivalHighScoreTop
	{
		get
		{
			return s_leaderboardEntryRivalHighScoreTop;
		}
	}

	public static ServerPrizeState PremiumRoulettePrizeList
	{
		get
		{
			return s_premiumRoulettePrizeList;
		}
	}

	public static ServerPrizeState SpecialRoulettePrizeList
	{
		get
		{
			return s_specialRoulettePrizeList;
		}
	}

	public static ServerPrizeState RaidRoulettePrizeList
	{
		get
		{
			return s_raidRoulettePrizeList;
		}
	}

	public static List<ServerMessageEntry> MessageList
	{
		get
		{
			return s_messageList;
		}
	}

	public static List<ServerOperatorMessageEntry> OperatorMessageList
	{
		get
		{
			return s_operatorMessageList;
		}
	}

	public static ServerEventState EventState
	{
		get
		{
			return s_eventState;
		}
	}

	public static List<ServerEventEntry> EventEntryList
	{
		get
		{
			return s_eventEntryList;
		}
	}

	public static List<ServerEventReward> EventRewardList
	{
		get
		{
			return s_eventRewardList;
		}
	}

	public static List<ServerRedStarItemState> RedStarItemList
	{
		get
		{
			return s_redStartItemState;
		}
	}

	public static List<ServerRedStarItemState> RedStarExchangeRingItemList
	{
		get
		{
			return s_redStartExchangeRingItemState;
		}
	}

	public static List<ServerRedStarItemState> RedStarExchangeEnergyItemList
	{
		get
		{
			return s_redStartExchangeEnergyItemState;
		}
	}

	public static List<ServerRedStarItemState> RedStarExchangeRaidbossEnergyItemList
	{
		get
		{
			return s_redStartExchangeRaidbossEnergyItemState;
		}
	}

	public static ServerDailyChallengeState DailyChallengeState
	{
		get
		{
			return s_dailyChallengeState;
		}
	}

	public static List<ServerUserTransformData> UserTransformDataList
	{
		get
		{
			return s_userTransformDataList;
		}
	}

	public static string MigrationPassword
	{
		get
		{
			return s_migrationPassword;
		}
		set
		{
			s_migrationPassword = value;
		}
	}

	public static ServerInterface LoggedInServerInterface
	{
		get
		{
			return (LoginState == null || !LoginState.IsLoggedIn) ? null : GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
		}
	}

	private static void Init()
	{
		if (!s_isCreated)
		{
			s_settingState = new ServerSettingState();
			s_loginState = new ServerLoginState();
			s_nextState = new ServerNextVersionState();
			s_playerState = new ServerPlayerState();
			s_freeItemState = new ServerFreeItemState();
			s_loginBonusData = new ServerLoginBonusData();
			s_noticeInfo = new ServerNoticeInfo();
			s_tickerInfo = new ServerTickerInfo();
			s_wheelOptions = new ServerWheelOptions();
			s_chaoWheelOptions = new ServerChaoWheelOptions();
			s_ringExchangeList = new List<ServerRingExchangeList>();
			s_consumedCostList = new List<ServerConsumedCostData>();
			s_costList = new List<ServerConsumedCostData>();
			s_mileageMapState = new ServerMileageMapState();
			s_mileageRewardList = new List<ServerMileageReward>();
			s_mileageFriendList = new List<ServerMileageFriendEntry>();
			s_distanceFriendEntry = new List<ServerDistanceFriendEntry>();
			s_campaignState = new ServerCampaignState();
			s_leaderboardEntries = new ServerLeaderboardEntries();
			s_leaderboardEntriesRivalHighScore = new ServerLeaderboardEntries();
			s_leaderboardEntryRivalHighScoreTop = new ServerLeaderboardEntry();
			s_premiumRoulettePrizeList = new ServerPrizeState();
			s_specialRoulettePrizeList = new ServerPrizeState();
			s_raidRoulettePrizeList = new ServerPrizeState();
			s_messageList = new List<ServerMessageEntry>();
			s_operatorMessageList = new List<ServerOperatorMessageEntry>();
			s_eventState = new ServerEventState();
			s_eventEntryList = new List<ServerEventEntry>();
			s_eventRewardList = new List<ServerEventReward>();
			s_redStartItemState = new List<ServerRedStarItemState>();
			s_redStartExchangeRingItemState = new List<ServerRedStarItemState>();
			s_redStartExchangeEnergyItemState = new List<ServerRedStarItemState>();
			s_redStartExchangeRaidbossEnergyItemState = new List<ServerRedStarItemState>();
			s_dailyChallengeState = new ServerDailyChallengeState();
			s_userTransformDataList = new List<ServerUserTransformData>();
			s_leagueData = new ServerLeagueData();
			s_migrationPassword = string.Empty;
			s_isCreated = true;
		}
	}

	private static void Reset()
	{
		Init();
	}

	private void Start()
	{
		if (!s_isCreated)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			Init();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void RequestServerLogin(string userId, string password, GameObject callbackObject)
	{
		StartCoroutine(ServerLogin.Process(userId, password, callbackObject));
	}

	public void RequestServerReLogin(GameObject callbackObject)
	{
		StartCoroutine(ServerReLogin.Process(callbackObject));
	}

	public void RequestServerMigration(string migrationID, string migrationPassword, GameObject callbackObject)
	{
		StartCoroutine(ServerMigration.Process(migrationID, migrationPassword, callbackObject));
	}

	public void RequestServerGetMigrationPassword(string userPassword, GameObject callbackObject)
	{
		StartCoroutine(ServerGetMigrationPassword.Process(userPassword, callbackObject));
	}

	public void RequestServerGetInformation(GameObject callbackObject)
	{
		StartCoroutine(ServerGetInformation.Process(callbackObject));
	}

	public void RequestServerGetVersion(GameObject callbackObject)
	{
		StartCoroutine(ServerGetVersion.Process(callbackObject));
	}

	public void RequestServerGetTicker(GameObject callbackObject)
	{
		StartCoroutine(ServerGetTicker.Process(callbackObject));
	}

	public void RequestServerGetCountry(GameObject callbackObject)
	{
		StartCoroutine(ServerGetCountry.Process(callbackObject));
	}

	public void RequestServerGetVariousParameter(GameObject callbackObject)
	{
		StartCoroutine(ServerGetVariousParameter.Process(callbackObject));
	}

	public void RequestServerLoginBonus(GameObject callbackObject)
	{
		StartCoroutine(ServerLoginBonus.Process(callbackObject));
	}

	public void RequestServerLoginBonusSelect(int rewardId, int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect, GameObject callbackObject)
	{
		StartCoroutine(ServerLoginBonusSelect.Process(rewardId, rewardDays, rewardSelect, firstRewardDays, firstRewardSelect, callbackObject));
	}

	public void RequestServerRetrievePlayerState(GameObject callbackObject)
	{
		StartCoroutine(ServerRetrievePlayerState.Process(callbackObject));
	}

	public void RequestServerGetCharacterState(GameObject callbackObject)
	{
		StartCoroutine(ServerGetCharacterState.Process(callbackObject));
	}

	public void RequestServerGetChaoState(GameObject callbackObject)
	{
		StartCoroutine(ServerGetChaoState.Process(callbackObject));
	}

	public void RequestServerSetUserName(string userName, GameObject callbackObject)
	{
		StartCoroutine(ServerSetUserName.Process(userName, callbackObject));
	}

	private void Event_ServerSetTutorialComplete(int tutorialId)
	{
	}

	public void RequestServerGetWheelOptions(GameObject callbackObject)
	{
		StartCoroutine(ServerGetWheelOptions.Process(callbackObject));
	}

	public void RequestServerGetWheelOptionsGeneral(int eventId, int spinId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetWheelOptionsGeneral.Process(eventId, spinId, callbackObject));
	}

	public void RequestServerGetWheelSpinInfo(GameObject callbackObject)
	{
		StartCoroutine(ServerGetWheelSpinInfo.Process(callbackObject));
	}

	public void RequestServerCommitWheelSpin(int count, GameObject callbackObject)
	{
		StartCoroutine(ServerCommitWheelSpin.Process(count, callbackObject));
	}

	public void RequestServerCommitWheelSpinGeneral(int eventId, int spinId, int spinCostItemId, int spinNum, GameObject callbackObject)
	{
		StartCoroutine(ServerCommitWheelSpinGeneral.Process(eventId, spinId, spinCostItemId, spinNum, callbackObject));
	}

	public void RequestServerGetDailyBattleStatus(GameObject callbackObject)
	{
		StartCoroutine(ServerGetDailyBattleStatus.Process(callbackObject));
	}

	public void RequestServerUpdateDailyBattleStatus(GameObject callbackObject)
	{
		StartCoroutine(ServerUpdateDailyBattleStatus.Process(callbackObject));
	}

	public void RequestServerPostDailyBattleResult(GameObject callbackObject)
	{
		StartCoroutine(ServerPostDailyBattleResult.Process(callbackObject));
	}

	public void RequestServerGetDailyBattleData(GameObject callbackObject)
	{
		StartCoroutine(ServerGetDailyBattleData.Process(callbackObject));
	}

	public void RequestServerGetPrizeDailyBattle(GameObject callbackObject)
	{
		StartCoroutine(ServerGetPrizeDailyBattle.Process(callbackObject));
	}

	public void RequestServerGetDailyBattleDataHistory(int count, GameObject callbackObject)
	{
		StartCoroutine(ServerGetDailyBattleDataHistory.Process(count, callbackObject));
	}

	public void RequestServerResetDailyBattleMatching(int type, GameObject callbackObject)
	{
		StartCoroutine(ServerResetDailyBattleMatching.Process(type, callbackObject));
	}

	public void RequestServerStartAct(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, List<string> distanceFriendIdList, bool tutorial, int? eventId, GameObject callbackObject)
	{
		StartCoroutine(ServerStartAct.Process(modifiersItem, modifiersBoostItem, distanceFriendIdList, tutorial, eventId, callbackObject));
	}

	public void RequestServerQuickModeStartAct(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, bool tutorial, GameObject callbackObject)
	{
		StartCoroutine(ServerQuickModeStartAct.Process(modifiersItem, modifiersBoostItem, tutorial, callbackObject));
	}

	public void RequestServerActRetry(GameObject callbackObject)
	{
		StartCoroutine(ServerActRetry.Process(callbackObject));
	}

	public void RequestServerPostGameResults(ServerGameResults results, GameObject callbackObject)
	{
		StartCoroutine(ServerPostGameResults.Process(results, callbackObject));
	}

	public void RequestServerQuickModePostGameResults(ServerQuickModeGameResults results, GameObject callbackObject)
	{
		StartCoroutine(ServerQuickModePostGameResults.Process(results, callbackObject));
	}

	public void RequestServerGetMenuData(GameObject callbackObject)
	{
		StartCoroutine(ServerGetMenuData.Process(callbackObject));
	}

	public void RequestServerGetMileageReward(int episode, int chapter, GameObject callbackObject)
	{
		StartCoroutine(ServerGetMileageReward.Process(episode, chapter, callbackObject));
	}

	public void RequestServerGetCostList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetCostList.Process(callbackObject));
	}

	public void RequestServerActRetryFree(GameObject callbackObject)
	{
		StartCoroutine(ServerActRetryFree.Process(callbackObject));
	}

	public void RequestServerGetFreeItemList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetFreeItemList.Process(callbackObject));
	}

	public void RequestServerGetCampaignList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetCampaignList.Process(callbackObject));
	}

	public void RequestServerGetMileageData(string[] distanceFriendList, GameObject callbackObject)
	{
		StartCoroutine(ServerGetMileageData.Process(distanceFriendList, callbackObject));
	}

	public void RequestServerGetDailyMissionData(GameObject callbackObject)
	{
		StartCoroutine(ServerGetDailyMissionData.Process(callbackObject));
	}

	public void RequestServerGetRingItemList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetRingItemList.Process(callbackObject));
	}

	public void RequestServerUpgradeCharacter(int characterId, int abilityId, GameObject callbackObject)
	{
		StartCoroutine(ServerUpgradeCharacter.Process(characterId, abilityId, callbackObject));
	}

	public void RequestServerUnlockedCharacter(CharaType charaType, ServerItem item, GameObject callbackObject)
	{
		StartCoroutine(ServerUnlockedCharacter.Process(charaType, item, callbackObject));
	}

	public void RequestServerChangeCharacter(int mainCharaId, int subCharaId, GameObject callbackObject)
	{
		StartCoroutine(ServerChangeCharacter.Process(mainCharaId, subCharaId, callbackObject));
	}

	public void RequestServerUseSubCharacter(bool useFlag, GameObject callbackObject)
	{
		StartCoroutine(ServerUseSubCharacter.Process(useFlag, callbackObject));
	}

	public void RequestServerGetLeaderboardEntries(int mode, int first, int count, int index, int rankingType, int eventId, string[] friendIdList, GameObject callbackObject)
	{
		StartCoroutine(ServerGetLeaderboardEntries.Process(mode, first, count, index, rankingType, eventId, friendIdList, callbackObject));
	}

	public void RequestServerGetWeeklyLeaderboardOptions(int mode, GameObject callbackObject)
	{
		StartCoroutine(ServerGetWeeklyLeaderboardOptions.Process(mode, callbackObject));
	}

	public void RequestServerGetLeagueData(int mode, GameObject callbackObject)
	{
		StartCoroutine(ServerGetLeagueData.Process(mode, callbackObject));
	}

	public void RequestServerGetLeagueOperatorData(int mode, GameObject callbackObject)
	{
		StartCoroutine(ServerGetLeagueOperatorData.Process(mode, callbackObject));
	}

	private void Event_ServerGetFriendsList(int first, int count)
	{
	}

	private void Event_ServerGetGameFriendsList(int first, int count)
	{
	}

	public void RequestServerRequestEnergy(string friendId, GameObject gameObject)
	{
		StartCoroutine(ServerRequestEnergy.Process(friendId, gameObject));
	}

	public void RequestServerGetFacebookIncentive(int incentiveType, int achievementCount, GameObject callbackObject)
	{
		StartCoroutine(ServerGetFacebookIncentive.Process(incentiveType, achievementCount, callbackObject));
	}

	public void RequestServerSetFacebookScopedId(string userId, GameObject callbackObject)
	{
		StartCoroutine(ServerSetFacebookScopedId.Process(userId, callbackObject));
	}

	public void RequestServerGetFriendUserIdList(List<string> friendFBIdList, GameObject callbackObject)
	{
		StartCoroutine(ServerGetFriendUserIdList.Process(friendFBIdList, callbackObject));
	}

	public void RequestServerSetInviteHistory(string facebookIdHash, GameObject callbackObject)
	{
		StartCoroutine(ServerSetInviteHistory.Process(facebookIdHash, callbackObject));
	}

	public void RequestServerSetInviteCode(string friendId, GameObject callbackObject)
	{
		StartCoroutine(ServerSetInviteCode.Process(friendId, callbackObject));
	}

	private void Event_ServerSendInvite(string friendId)
	{
	}

	public void RequestServerSendEnergy(string friendId, GameObject gameObject)
	{
		StartCoroutine(ServerSendEnergy.Process(friendId, gameObject));
	}

	public void RequestServerUpdateMessage(List<int> messageIdList, List<int> operatorMessageIdList, GameObject callbackObject)
	{
		StartCoroutine(ServerUpdateMessage.Process(messageIdList, operatorMessageIdList, callbackObject));
	}

	public void RequestServerGetMessageList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetMessageList.Process(callbackObject));
	}

	public void RequestServerPreparePurchase(int itemId, GameObject callbackObject)
	{
		StartCoroutine(ServerPreparePurchase.Process(itemId, callbackObject));
	}

	public void RequestServerPurchase(bool isSuccess, GameObject callbackObject)
	{
		StartCoroutine(ServerPurchase.Process(isSuccess, callbackObject));
	}

	public void RequestServerGetRedStarExchangeList(int itemType, GameObject callbackObject)
	{
		StartCoroutine(ServerGetRedStarExchangeList.Process(itemType, callbackObject));
	}

	public void RequestServerRedStarExchange(int storeItemId, GameObject callbackObject)
	{
		StartCoroutine(ServerRedStarExchange.Process(storeItemId, callbackObject));
	}

	public void RequestServerBuyIos(string receiptData, GameObject callbackObject)
	{
		StartCoroutine(ServerBuyIos.Process(receiptData, callbackObject));
	}

	public void RequestServerBuyAndroid(string receiptData, string signature, GameObject callbackObject)
	{
		StartCoroutine(ServerBuyAndroid.Process(receiptData, signature, callbackObject));
	}

	public void RequestServerGetRingExchangeList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetRingExchangeList.Process(callbackObject));
	}

	public void RequestServerSetBirthday(string birthday, GameObject callbackObject)
	{
		StartCoroutine(ServerSetBirthday.Process(birthday, callbackObject));
	}

	public void RequestServerRingExchange(int itemId, int itemNum, GameObject callbackObject)
	{
		StartCoroutine(ServerRingExchange.Process(itemId, itemNum, callbackObject));
	}

	public void RequestServerGetItemStockNum(int eventId, List<int> itemId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetItemStockNum.Process(eventId, itemId, callbackObject));
	}

	public void RequestServerGetItemStockNum(int eventId, int itemId, GameObject callbackObject)
	{
		List<int> list = new List<int>();
		list.Add(itemId);
		StartCoroutine(ServerGetItemStockNum.Process(eventId, list, callbackObject));
	}

	public void RequestServerGetItemStockNum(int eventId, ServerItem.Id itemId, GameObject callbackObject)
	{
		List<int> list = new List<int>();
		list.Add((int)itemId);
		StartCoroutine(ServerGetItemStockNum.Process(eventId, list, callbackObject));
	}

	public void RequestServerGetChaoWheelOptions(GameObject callbackObject)
	{
		StartCoroutine(ServerGetChaoWheelOptions.Process(callbackObject));
	}

	public void RequestServerGetPrizeChaoWheelSpin(int chaoWheelSpinType, GameObject callbackObject)
	{
		StartCoroutine(ServerGetPrizeChaoWheelSpin.Process(chaoWheelSpinType, callbackObject));
	}

	public void RequestServerGetPrizeWheelSpinGeneral(int eventId, int spinType, GameObject callbackObject)
	{
		StartCoroutine(ServerGetPrizeWheelSpinGeneral.Process(eventId, spinType, callbackObject));
	}

	public void RequestServerCommitChaoWheelSpin(int count, GameObject callbackObject)
	{
		StartCoroutine(ServerCommitChaoWheelSpin.Process(count, callbackObject));
	}

	public void RequestServerGetChaoRentalStates(string[] frindId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetChaoRentalStates.Process(frindId, callbackObject));
	}

	public void RequestServerEquipChao(int mainChaoId, int subChaoId, GameObject callbackObject)
	{
		StartCoroutine(ServerEquipChao.Process(mainChaoId, subChaoId, callbackObject));
	}

	public void RequestServerGetFirstLaunchChao(GameObject callbackObject)
	{
		StartCoroutine(ServerGetFirstLaunchChao.Process(callbackObject));
	}

	public void RequestServerAddSpecialEgg(int numSpecialEgg, GameObject callbackObject)
	{
		StartCoroutine(ServerAddSpecialEgg.Process(numSpecialEgg, callbackObject));
	}

	public void RequestServerEquipItem(List<ItemType> items, GameObject callbackObject)
	{
		StartCoroutine(ServerEquipItem.Process(items, callbackObject));
	}

	public void RequestServerOptionUserResult(GameObject callbackObject)
	{
		StartCoroutine(ServerGetOptionUserResult.Process(callbackObject));
	}

	public void RequestServerAtomSerial(string campaignId, string serial, bool new_user, GameObject callbackObject)
	{
		StartCoroutine(ServerAtomSerial.Process(campaignId, serial, new_user, callbackObject));
	}

	public void RequestServerSendApollo(int type, string[] value, GameObject callbackObject)
	{
		StartCoroutine(ServerSendApollo.Process(type, value, callbackObject));
	}

	public void RequestServerSetNoahId(string noahId, GameObject callbackObject)
	{
		StartCoroutine(ServerSetNoahId.Process(noahId, callbackObject));
	}

	public void RequestServerGetEventList(GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventList.Process(callbackObject));
	}

	public void RequestServerGetEventReward(int eventId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventReward.Process(eventId, callbackObject));
	}

	public void RequestServerEventStartAct(int eventId, int energyExpend, long raidBossId, List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, GameObject callbackObject)
	{
		StartCoroutine(ServerEventStartAct.Process(eventId, energyExpend, raidBossId, modifiersItem, modifiersBoostItem, callbackObject));
	}

	public void RequestServerEventUpdateGameResults(ServerEventGameResults results, GameObject callbackObject)
	{
		StartCoroutine(ServerEventUpdateGameResults.Process(results, callbackObject));
	}

	public void RequestServerEventPostGameResults(int eventId, int numRaidBossRings, GameObject callbackObject)
	{
		StartCoroutine(ServerEventPostGameResults.Process(eventId, numRaidBossRings, callbackObject));
	}

	public void RequestServerGetEventState(int eventId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventState.Process(eventId, callbackObject));
	}

	public void RequestServerGetEventUserRaidBossList(int eventId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventUserRaidBossList.Process(eventId, callbackObject));
	}

	public void RequestServerGetEventUserRaidBossState(int eventId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventUserRaidBossState.Process(eventId, callbackObject));
	}

	public void RequestServerGetEventRaidBossUserList(int eventId, long raidBossId, GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventRaidBossUserList.Process(eventId, raidBossId, callbackObject));
	}

	public void RequestServerGetEventRaidBossDesiredList(int eventId, long raidBossId, List<string> friendIdList, GameObject callbackObject)
	{
		StartCoroutine(ServerGetEventRaidBossDesiredList.Process(eventId, raidBossId, friendIdList, callbackObject));
	}

	public void RequestServerDrawRaidBoss(int eventId, long score, GameObject callbackObject)
	{
		StartCoroutine(ServerDrawRaidBoss.Process(eventId, score, callbackObject));
	}

	private void Event_SPLoginUpgrade_Success()
	{
		Reset();
	}

	public static bool IsRSREnable()
	{
		if (s_redStartItemState != null)
		{
			return s_redStartItemState.Count > 0;
		}
		return false;
	}

	public static void DebugInit()
	{
		Init();
	}
}
