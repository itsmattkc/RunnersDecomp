using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

public class UnmanagedProcess {

    private static string sendString;
    private static string responseString;

    private static NativeScoreResult scoreResult = new NativeScoreResult();

    private static int BoolToInt(bool e)
    {
        return e ? 1 : 0;
    }

    public static int CalcLoginString(SendBaseNativeParam baseInfo, string userId,
        string password, string migrationPassword, int platform, string device, int language,
        int salesLocale, int storeId, int platformSns, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;

        Hashtable lineAuth = new Hashtable();
        lineAuth["userId"] = userId;
        lineAuth["password"] = password;
        lineAuth["migrationPassword"] = migrationPassword;

        tbl["lineAuth"] = lineAuth;

        tbl["platform"] = platform.ToString();
        tbl["device"] = device;
        tbl["language"] = language.ToString();
        tbl["salesLocate"] = salesLocale.ToString();
        tbl["storeId"] = storeId.ToString();
        tbl["platform_sns"] = platformSns.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static void GetSendString(StringBuilder result, int stringSize)
    {
        result.Append(sendString);
    }

    public static bool IsEnableEncrypto()
    {
        return true;
    }

    public static int CalcResponseString(string responseText, string encryptInitVector)
    {
        responseString = CryptoUtility.Decrypt(responseText);

        return responseString.Length;
    }

    public static void GetResponseString(StringBuilder result, string responseText,
        string encryptInitVector)
    {
        result.Append(responseString);
    }

    public static void ResetResultScore()
    {
        // FIXME: Guessed behavior, haven't verified
        scoreResult.stageScore = 0;
        scoreResult.ringScore = 0;
        scoreResult.animalScore = 0;
        scoreResult.distanceScore = 0;
        scoreResult.finalScore = 0;
        scoreResult.redStarRingCount = 0;
        scoreResult.eventObjCount = 0;
    }

    public static NativeScoreResult GetResultScore()
    {
        return scoreResult;
    }

    public static int CalcSendAppoloString(SendBaseNativeParam baseInfo, int type, string[] value,
        int paramSize, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["type"] = type.ToString();
        tbl["value"] = value;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcOnlySendBaseparamString(SendBaseNativeParam baseInfo,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetRedStarExchangeListString(SendBaseNativeParam baseInfo, int itemType,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["itemType"] = itemType.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static void BootGameAction()
    {
        // FIXME: I think these are anti-cheat related, will require looking at disassembly
        Debug.LogWarning("[WARNING] BootGameAction: stub");
    }

    public static void BeforeGameAction()
    {
        // FIXME: I think these are anti-cheat related, will require looking at disassembly
        Debug.LogWarning("[WARNING] BeforeGameAction: stub");
    }

    public static int CalcGetPrizeChaoWheelSpinString(SendBaseNativeParam baseInfo,
        int chaoWheelSpinType, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["chaoWheelSpinType"] = chaoWheelSpinType.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcWeeklyLeaderboardOptionsString(SendBaseNativeParam baseInfo, int mode,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["mode"] = mode.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcLeagueDataString(SendBaseNativeParam baseInfo, int mode,
        string encryptInitVector)
    {
        // Produces the same result
        return CalcWeeklyLeaderboardOptionsString(baseInfo, mode, encryptInitVector);
    }

    public static int CalcGetWeeklyLeaderboardEntryString(SendBaseNativeParam baseInfo,
        LeaderboardEntryNativeParam leaderboardEntryParam, string[] friendIdList,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["mode"] = leaderboardEntryParam.mode.ToString();
        tbl["first"] = leaderboardEntryParam.first.ToString();
        tbl["count"] = leaderboardEntryParam.count.ToString();
        tbl["type"] = leaderboardEntryParam.type.ToString();
        tbl["friendIdList"] = friendIdList;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcSetUserNameString(SendBaseNativeParam baseInfo, string userName,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["userName"] = userName;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcUpgradeCharacterString(SendBaseNativeParam baseInfo, int characterId,
        int abilityId, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["characterId"] = characterId.ToString();
        tbl["abilityId"] = abilityId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcUnlockedCharacterString(SendBaseNativeParam baseInfo, int characterId,
        int itemId, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["characterId"] = characterId.ToString();
        tbl["itemId"] = itemId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcChangeCharacterString(SendBaseNativeParam baseInfo, int mainCharacterId,
        int subCharacterId, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["mainCharacterId"] = mainCharacterId.ToString();
        tbl["subCharacterId"] = subCharacterId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetMigrationPasswordString(SendBaseNativeParam baseInfo,
        string userPassword, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["userPassword"] = userPassword;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }


    public static int CalcGetMileageRewardString(SendBaseNativeParam baseInfo, int episode,
        int chapter, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["episode"] = episode.ToString();
        tbl["chapter"] = chapter.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }


    public static int CalcActStartString(SendBaseNativeParam baseInfo,
        ActStartNativeParam actStartParam, string[] distanceFriendList, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["modifire"] = actStartParam.modifire;
        tbl["distanceFriendList"] = distanceFriendList;
        tbl["tutorial"] = BoolToInt(actStartParam.tutorial);

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static void CheckHalfWayResultScore(PostGameResultNativeParam param,
        StageScoreData[] scoreDatas, int dataSize)
    {
        // FIXME: I'm not sure what to do with this information yet
        Debug.LogWarning("[WARNING] CheckHalfWayResultScore: stub");
    }

    public static int CalcMigrationString(SendBaseNativeParam baseInfo, string userId,
        string migrationPassword, string migrationUserPassword, int platform, string device,
        int language, int salesLocale, int storeId, int platformSns, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;

        Hashtable lineAuth = new Hashtable();

        lineAuth["userId"] = userId;
        lineAuth["migrationPassword"] = migrationPassword;
        lineAuth["migrationUserPassword"] = migrationUserPassword;

        tbl["lineAuth"] = lineAuth;

        tbl["platform"] = platform.ToString();
        tbl["device"] = device;
        tbl["language"] = language.ToString();
        tbl["salesLocate"] = salesLocale.ToString();
        tbl["storeId"] = storeId.ToString();
        tbl["platform_sns"] = platformSns.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcQuickModeActStartString(SendBaseNativeParam baseInfo,
        QuickModeActStartNativeParam actStartParam, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["modifire"] = actStartParam.modifire;
        tbl["tutorial"] = actStartParam.m_tutorial;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static void CheckHalfWayQuickModeResultScore(QuickModePostGameResultNativeParam param,
        StageScoreData[] scoreDatas, int dataSize)
    {
        // FIXME: I'm not sure what to do with this information yet
        Debug.LogWarning("[WARNING] CheckHalfWayQuickModeResultScore: stub");
    }

    public static int CalcActRetryString(SendBaseNativeParam baseInfo, string encryptInitVector)
    {
        return CalcOnlySendBaseparamString(baseInfo, encryptInitVector);
    }

    public static void CheckQuickModeResultTimer(QuickModeTimerNativeParam param)
    {
        // FIXME: I'm not sure what to do with this information yet
        Debug.LogWarning("[WARNING] CheckQuickModeResultTimer: stub");
    }

    public static int CalcQuickModePostGameResultString(SendBaseNativeParam baseInfo,
        QuickModePostGameResultNativeParam param, StageScoreData[] scoreDatas, int dataSize,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["closed"] = BoolToInt(param.closed);
        tbl["score"] = param.score.ToString();
        tbl["numRings"] = param.numRings.ToString();
        tbl["numFailureRings"] = param.numFailureRings.ToString();
        tbl["numRedStarRings"] = param.numRedStarRings.ToString();
        tbl["distance"] = param.distance.ToString();
        tbl["dailyChallengeValue"] = param.dailyChallengeValue.ToString();
        tbl["dailyChallengeComplete"] = BoolToInt(param.dailyChallengeComplete);
        tbl["numAnimals"] = param.numAnimals.ToString();

        // FIXME: Hardcoded - in reality this should be generated
        tbl["cheatResult"] = "00000000";

        tbl["maxCombo"] = param.maxComboCount.ToString();

        Debug.LogWarning("[WARNING] CalcQuickModePostGameResultString: cheatResult is hardcoded");

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcPostGameResultString(SendBaseNativeParam baseInfo,
        PostGameResultNativeParam param, StageScoreData[] scoreDatas, int dataSize,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["closed"] = BoolToInt(param.closed);
        tbl["score"] = param.score.ToString();
        tbl["stageMaxScore"] = param.stageMaxScore.ToString();
        tbl["numRings"] = param.numRings.ToString();
        tbl["numFailureRings"] = param.numFailureRings.ToString();
        tbl["numRedStarRings"] = param.numRedStarRings.ToString();
        tbl["distance"] = param.distance.ToString();
        tbl["dailyChallengeValue"] = param.dailyChallengeValue.ToString();
        tbl["dailyChallengeComplete"] = BoolToInt(param.dailyChallengeComplete);
        tbl["numAnimals"] = param.numAnimals.ToString();
        tbl["reachPoint"] = param.reachPoint.ToString();
        tbl["chapterClear"] = BoolToInt(param.chapterClear);
        tbl["numBossAttack"] = param.numBossAttack.ToString();
        tbl["getChaoEgg"] = BoolToInt(param.getChaoEgg);

        // FIXME: Hardcoded - in reality this should be generated
        tbl["cheatResult"] = "00000000";

        tbl["bossDestroyed"] = BoolToInt(param.bossDestroyed);
        tbl["maxCombo"] = param.maxComboCount.ToString();

        Debug.LogWarning("[WARNING] CalcPostGameResultString: cheatResult is hardcoded");

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetItemStockNum(SendBaseNativeParam baseInfo, int eventId,
        int[] itemIdList, int listSize, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["eventId"] = eventId.ToString();
        tbl["itemIdList"] = itemIdList;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcCommitWheelSpinString(SendBaseNativeParam baseInfo, int count,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["count"] = count.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcCommitChaoWheelSpinString(SendBaseNativeParam baseInfo, int count,
        string encryptInitVector)
    {
        return CalcCommitWheelSpinString(baseInfo, count, encryptInitVector);
    }

    public static int CalcRedStarExchangeString(SendBaseNativeParam baseInfo, int itemId,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["itemId"] = itemId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcEquipChaoString(SendBaseNativeParam baseInfo, int mainChaoId,
        int subChaoId, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["mainChaoId"] = mainChaoId.ToString();
        tbl["subChaoId"] = subChaoId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcLoginBonusSelectString(SendBaseNativeParam baseInfo, int rewardId,
        int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["rewardId"] = rewardId.ToString();
        tbl["rewardDays"] = rewardDays.ToString();
        tbl["rewardSelect"] = rewardSelect.ToString();
        tbl["firstRewardDays"] = firstRewardDays.ToString();
        tbl["firstRewardSelect"] = firstRewardSelect.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetPrizeWheelSpinGeneralString(SendBaseNativeParam baseInfo, int eventId,
        int spinType, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["eventId"] = eventId.ToString();
        tbl["raidbossWheelSpinType"] = spinType.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetFacebookIncentiveString(SendBaseNativeParam baseInfo, int type,
        int achievementCount, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["type"] = type.ToString();
        tbl["achievementCount"] = achievementCount.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcLeagueOperatorDataString(SendBaseNativeParam baseInfo, int mode,
        int league, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["mode"] = mode.ToString();
        tbl["league"] = league.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcInviteCodeString(SendBaseNativeParam baseInfo, string friendId,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["friendId"] = friendId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcSetFacebookScopedIdString(SendBaseNativeParam baseInfo,
        string facebookId, string cryptoInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["facebookId"] = facebookId;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetFriendUserIdListString(SendBaseNativeParam baseInfo,
        string[] friendIdList, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["facebookIdList"] = friendIdList;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcSendEnergyString(SendBaseNativeParam baseInfo, string friendId,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["friendId"] = friendId;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcBuyIosString(SendBaseNativeParam baseInfo, string receiptData,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["receipt_data"] = receiptData;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcBuyAndroidString(SendBaseNativeParam baseInfo, string receiptData,
        string receiptSignature, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["receipt_data"] = receiptData;
        tbl["receipt_signature"] = receiptSignature;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcSetBirthdayString(SendBaseNativeParam baseInfo, string birthday,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["birthday"] = birthday;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcAddSpecialEggString(SendBaseNativeParam baseInfo, int numSpecialEgg,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["numSpecialEgg"] = numSpecialEgg.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetWheelSpinGeneralString(SendBaseNativeParam baseInfo, int eventId,
        int spinId, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["eventId"] = eventId.ToString();
        tbl["id"] = spinId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcCommitWheelSpinGeneralString(SendBaseNativeParam baseInfo, int eventId,
        int spinCostItemId, int spinId, int spinNum, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["eventId"] = eventId.ToString();
        tbl["costItemId"] = spinCostItemId.ToString();
        tbl["id"] = spinId.ToString();
        tbl["num"] = spinNum.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcSetSerialCodeString(SendBaseNativeParam baseInfo, string campaignId,
        string serialCode, bool newUser, string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["campaignId"] = campaignId;
        tbl["serialCode"] = serialCode;
        tbl["newUser"] = BoolToInt(newUser);

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcSetNoahIdString(SendBaseNativeParam baseInfo, string noahId,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["noahId"] = noahId;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetEventRewardString(SendBaseNativeParam baseInfo, int eventId,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["eventId"] = eventId.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetDailyBattleDataHistoryString(SendBaseNativeParam baseInfo, int count,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["count"] = count.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcResetDailyBattleMatchingString(SendBaseNativeParam baseInfo, int type,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["seq"] = baseInfo.seq;
        tbl["type"] = type.ToString();

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }

    public static int CalcGetMessageString(SendBaseNativeParam baseInfo, int[] messageIdList,
        int messageIdSize, int[] operationMessageIdList, int operationMessageIdSize,
        string encryptInitVector)
    {
        Hashtable tbl = new Hashtable();

        tbl["sessionId"] = baseInfo.sessionId;
        tbl["version"] = baseInfo.version;
        tbl["messageId"] = messageIdList;
        tbl["operationMessageId"] = operationMessageIdList;

        sendString = CryptoUtility.Encrypt(NewJSON.JsonEncode(tbl));

        return sendString.Length;
    }
}
