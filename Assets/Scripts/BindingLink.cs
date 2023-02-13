using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public class BindingLink : IDisposable
{
	public void Dispose()
	{
	}

	public bool IsSecure()
	{
		return UnmanagedProcess.IsEnableEncrypto();
	}

	public void ResetNativeResultScore()
	{
		UnmanagedProcess.ResetResultScore();
	}

	public NativeScoreResult GetNativeResultScore()
	{
		NativeScoreResult resultScore = UnmanagedProcess.GetResultScore();
		Debug.Log("Kenzan-score.stageScore = " + resultScore.stageScore, DebugTraceManager.TraceType.SERVER);
		Debug.Log("Kenzan-score.ringScore = " + resultScore.ringScore, DebugTraceManager.TraceType.SERVER);
		Debug.Log("Kenzan-score.animalScore = " + resultScore.animalScore, DebugTraceManager.TraceType.SERVER);
		Debug.Log("Kenzan-score.distanceScore = " + resultScore.distanceScore, DebugTraceManager.TraceType.SERVER);
		Debug.Log("Kenzan-score.finalScore = " + resultScore.finalScore);
		Debug.Log("Kenzan-score.redStarRingCount = " + resultScore.redStarRingCount, DebugTraceManager.TraceType.SERVER);
		return resultScore;
	}

	public void CheckNativeHalfWayResultScore(ServerGameResults gameResult)
	{
		PostGameResultNativeParam param = default(PostGameResultNativeParam);
		param.Init(gameResult);
		StageScoreManager instance = StageScoreManager.Instance;
		StageScorePool scorePool = instance.ScorePool;

		StageScoreData[] dataClone = new StageScoreData[StageScorePool.ArrayCount];
		scorePool.scoreDatas.CopyTo(dataClone, 0);

		UnmanagedProcess.CheckHalfWayResultScore(param, scorePool.scoreDatas, scorePool.StoredCount);

		string line = "The following scores were changed by CheckHalfWayResultScore:";
		for (int i=0; i<scorePool.StoredCount; i++) {
			if (scorePool.scoreDatas[i].scoreValue != dataClone[i].scoreValue) {
				line += "\n  " + i + "score changed from" + dataClone[i].scoreValue + "to" + scorePool.scoreDatas[i].scoreValue;
			}
		}
		Debug.Log(line);
	}

	public void CheckNativeHalfWayQuickModeResultScore(ServerQuickModeGameResults gameResult)
	{
		QuickModePostGameResultNativeParam param = default(QuickModePostGameResultNativeParam);
		param.Init(gameResult);
		StageScoreManager instance = StageScoreManager.Instance;
		StageScorePool scorePool = instance.ScorePool;

		UnmanagedProcess.CheckHalfWayQuickModeResultScore(param, scorePool.scoreDatas, scorePool.StoredCount);
	}

	public void CheckNativeQuickModeResultTimer(int gold, int silver, int bronze, int continueCount, int main, int sub, int total, long playTime)
	{
		QuickModeTimerNativeParam param = default(QuickModeTimerNativeParam);
		param.Init(gold, silver, bronze, continueCount, main, sub, total, playTime);
		UnmanagedProcess.CheckQuickModeResultTimer(param);
	}

	public void BootGameCheatCheck()
	{
		UnmanagedProcess.BootGameAction();
	}

	public void BeforeGameCheatCheck()
	{
		UnmanagedProcess.BeforeGameAction();
	}

	public string DecodeServerResponseText(string responseText)
	{
		int capacity = UnmanagedProcess.CalcResponseString(responseText, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = capacity;
		UnmanagedProcess.GetResponseString(stringBuilder, responseText, CryptoUtility.code);
		Debug.Log("DecodeServerResponseText() = " + stringBuilder, DebugTraceManager.TraceType.SERVER);
		return stringBuilder.ToString();
	}

	public string GetOnlySendBaseParamString()
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcOnlySendBaseparamString(baseInfo, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetLoginString(string userId, string password, string migrationPassword, int platform, string device, int language, int salesLocale, int storeId, int platformSns)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcLoginString(baseInfo, userId, password, migrationPassword, platform, device, language, salesLocale, storeId, platformSns, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetMigrationString(string userId, string migrationPassword, string migrationUserPassword, int platform, string device, int language, int salesLocale, int storeId, int platformSns)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcMigrationString(baseInfo, userId, migrationPassword, migrationUserPassword, platform, device, language, salesLocale, storeId, platformSns, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetMigrationPasswordString(string userPassword)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetMigrationPasswordString(baseInfo, userPassword, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSetUserNameString(string userName)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSetUserNameString(baseInfo, userName, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetActStartString(List<int> modifire, List<string> distanceFriendList, bool tutorial, int eventId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		ActStartNativeParam actStartParam = default(ActStartNativeParam);
		actStartParam.Init(modifire, tutorial, eventId);
		int num = UnmanagedProcess.CalcActStartString(baseInfo, actStartParam, distanceFriendList.ToArray(), CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetQuickModeActStartString(List<int> modifire, int tutorial)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		QuickModeActStartNativeParam actStartParam = default(QuickModeActStartNativeParam);
		actStartParam.Init(modifire, tutorial);
		int num = UnmanagedProcess.CalcQuickModeActStartString(baseInfo, actStartParam, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetActRetryString()
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcActRetryString(baseInfo, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetPostGameResultString(ServerGameResults gameResult)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		PostGameResultNativeParam param = default(PostGameResultNativeParam);
		param.Init(gameResult);
		StageScoreManager instance = StageScoreManager.Instance;
		StageScorePool scorePool = instance.ScorePool;
		int num = UnmanagedProcess.CalcPostGameResultString(baseInfo, param, scorePool.scoreDatas, scorePool.StoredCount, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetQuickModePostGameResultString(ServerQuickModeGameResults gameResult)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		QuickModePostGameResultNativeParam param = default(QuickModePostGameResultNativeParam);
		param.Init(gameResult);
		StageScoreManager instance = StageScoreManager.Instance;
		StageScorePool scorePool = instance.ScorePool;
		int num = UnmanagedProcess.CalcQuickModePostGameResultString(baseInfo, param, scorePool.scoreDatas, scorePool.StoredCount, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetMileageRewardString(int episode, int chapter)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetMileageRewardString(baseInfo, episode, chapter, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetUpgradeCharacterString(int characterId, int abilityId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcUpgradeCharacterString(baseInfo, characterId, abilityId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetUnlockedCharacterString(int characterId, int itemId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcUnlockedCharacterString(baseInfo, characterId, itemId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetChangeCharacterString(int mainCharacterId, int subCharacterId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcChangeCharacterString(baseInfo, mainCharacterId, subCharacterId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetWeeklyLeaderboardEntries(int mode, int first, int count, int type, List<string> friendIdList, int eventId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		LeaderboardEntryNativeParam leaderboardEntryParam = default(LeaderboardEntryNativeParam);
		leaderboardEntryParam.Init(mode, first, count, type, eventId);
		int num = UnmanagedProcess.CalcGetWeeklyLeaderboardEntryString(baseInfo, leaderboardEntryParam, friendIdList.ToArray(), CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetLeagueOperatorDataString(int mode, int league)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcLeagueOperatorDataString(baseInfo, mode, league, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetLeagueDataString(int mode)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcLeagueDataString(baseInfo, mode, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetWeeklyLeaderboardOptionsString(int mode)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcWeeklyLeaderboardOptionsString(baseInfo, mode, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSetInviteCodeString(string friendId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcInviteCodeString(baseInfo, friendId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetFacebookIncentiveString(int type, int achievementCount)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetFacebookIncentiveString(baseInfo, type, achievementCount, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSetFacebookScopedIdString(string facebookId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSetFacebookScopedIdString(baseInfo, facebookId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetFacebookFriendUserIdList(List<string> facebookIdList)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetFriendUserIdListString(baseInfo, facebookIdList.ToArray(), CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSendEnergyString(string friendId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSendEnergyString(baseInfo, friendId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetMessageString(List<int> messageId, List<int> operationMessageId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		List<int> list = new List<int>();
		if (messageId != null)
		{
			foreach (int item in messageId)
			{
				list.Add(item);
			}
		}
		List<int> list2 = new List<int>();
		if (operationMessageId != null)
		{
			foreach (int item2 in operationMessageId)
			{
				list2.Add(item2);
			}
		}
		int num = UnmanagedProcess.CalcGetMessageString(baseInfo, list.ToArray(), list.Count, list2.ToArray(), list2.Count, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetRedStarExchangeListString(int itemType)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetRedStarExchangeListString(baseInfo, itemType, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetRedStarExchangeString(int itemId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcRedStarExchangeString(baseInfo, itemId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetBuyIosString(string receiptData)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcBuyIosString(baseInfo, receiptData, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetBuyAndroidString(string receiptData, string receiptSignature)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcBuyAndroidString(baseInfo, receiptData, receiptSignature, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSetBirthdayString(string birthday)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSetBirthdayString(baseInfo, birthday, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetCommitWheelSpinString(int count)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcCommitWheelSpinString(baseInfo, count, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetCommitChaoWheelSpinString(int count)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcCommitChaoWheelSpinString(baseInfo, count, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetEquipChaoString(int mainChaoId, int subChaoId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcEquipChaoString(baseInfo, mainChaoId, subChaoId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetAddSpecialEggString(int numSpecialEgg)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcAddSpecialEggString(baseInfo, numSpecialEgg, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetPrizeChaoWheelSpinString(int chaoWheelSpinType)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetPrizeChaoWheelSpinString(baseInfo, chaoWheelSpinType, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetWheelSpinGeneralString(int eventId, int spinId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetWheelSpinGeneralString(baseInfo, eventId, spinId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetCommitWheelSpinGeneralString(int eventId, int spinCostItemId, int spinId, int spinNum)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcCommitWheelSpinGeneralString(baseInfo, eventId, spinCostItemId, spinId, spinNum, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetPrizeWheelSpinGeneralString(int eventId, int spinType)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetPrizeWheelSpinGeneralString(baseInfo, eventId, spinType, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSendApolloString(int type, List<string> value)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSendAppoloString(baseInfo, type, value.ToArray(), value.Count, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSetSerialCodeString(string campaignId, string serialCode, bool newUser)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSetSerialCodeString(baseInfo, campaignId, serialCode, newUser, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetSetNoahIdString(string noahId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcSetNoahIdString(baseInfo, noahId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetEventRewardString(int eventId)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetEventRewardString(baseInfo, eventId, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetGetItemStockNumString(int eventId, int[] itemIdList)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int listSize = 0;
		if (itemIdList != null)
		{
			listSize = itemIdList.Length;
		}
		int num = UnmanagedProcess.CalcGetItemStockNum(baseInfo, eventId, itemIdList, listSize, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string GetDailyBattleDataHistoryString(int count)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcGetDailyBattleDataHistoryString(baseInfo, count, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string ResetDailyBattleMatchingString(int type)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcResetDailyBattleMatchingString(baseInfo, type, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	public string LoginBonusSelectString(int rewardId, int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect)
	{
		SendBaseNativeParam baseInfo = default(SendBaseNativeParam);
		baseInfo.Init();
		int num = UnmanagedProcess.CalcLoginBonusSelectString(baseInfo, rewardId, rewardDays, rewardSelect, firstRewardDays, firstRewardSelect, CryptoUtility.code);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Capacity = num;
		UnmanagedProcess.GetSendString(stringBuilder, num);
		return stringBuilder.ToString();
	}

	private static void DebugLogCallbackFromCPlusPlus(string log)
	{
		Debug.Log("From C++ [" + log + "]", DebugTraceManager.TraceType.SERVER);
	}
}
