using App;
using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class NetServerMigration : NetBase
{
	private int m_apolloPlatform2 = 1;

	public string paramUserId
	{
		get;
		set;
	}

	public string paramPassword
	{
		get;
		set;
	}

	public string paramMigrationId
	{
		get;
		set;
	}

	public string paramMigrationPassword
	{
		get;
		set;
	}

	public string resultSessionId
	{
		get;
		private set;
	}

	public long resultEnergyRefreshTime
	{
		get;
		private set;
	}

	public ServerItemState resultInviteBaseIncentive
	{
		get;
		private set;
	}

	public ServerItemState resultRentalBaseIncentive
	{
		get;
		private set;
	}

	public string userName
	{
		get;
		private set;
	}

	public string userId
	{
		get;
		private set;
	}

	public string password
	{
		get;
		private set;
	}

	public string countryCode
	{
		get;
		private set;
	}

	public NetServerMigration()
		: this(string.Empty, string.Empty)
	{
	}

	public NetServerMigration(string migrationId, string migrationPassword)
	{
		paramUserId = "0";
		paramPassword = "0";
		paramMigrationId = migrationId;
		paramMigrationPassword = migrationPassword;
	}

	protected override void DoRequest()
	{
		SetAction("Login/migration");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string userId = (paramUserId == null) ? string.Empty : paramUserId;
			string migrationPassword = (paramMigrationId == null) ? string.Empty : paramMigrationId;
			string migrationUserPassword = (paramMigrationPassword == null) ? string.Empty : paramMigrationPassword;
			int platform = 2;
			string empty = string.Empty;
			string deviceModel = SystemInfo.deviceModel;
			empty = deviceModel.Replace(" ", "_");
			int language = (int)Env.language;
			int salesLocale = 0;
			int storeId = 2;
			int apolloPlatform = m_apolloPlatform2;
			string migrationString = instance.GetMigrationString(userId, migrationPassword, migrationUserPassword, platform, empty, language, salesLocale, storeId, apolloPlatform);
			WriteJsonString(migrationString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_SessionId(jdata);
		GetResponse_SettingData(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultSessionId = "dummy";
	}

	private void SetParameter_LineAuth()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>(1);
		dictionary.Add("userId", (paramUserId == null) ? string.Empty : paramUserId);
		dictionary.Add("password", (paramPassword == null) ? string.Empty : paramPassword);
		dictionary.Add("migrationPassword", (paramMigrationId == null) ? string.Empty : paramMigrationId);
		dictionary.Add("migrationUserPassword", (paramMigrationPassword == null) ? string.Empty : paramMigrationPassword);
		WriteActionParamObject("lineAuth", dictionary);
		dictionary.Clear();
		dictionary = null;
	}

	private void SetParameter_Platform()
	{
		int num = 2;
		WriteActionParamValue("platform", num);
	}

	private void SetParameter_Device()
	{
		string empty = string.Empty;
		string deviceModel = SystemInfo.deviceModel;
		empty = deviceModel.Replace(" ", "_");
		WriteActionParamValue("device", empty);
	}

	private void SetParameter_Language()
	{
		int language = (int)Env.language;
		WriteActionParamValue("language", language);
	}

	private void SetParameter_Locate()
	{
		WriteActionParamValue("salesLocate", 0);
	}

	private void SetParameter_StoreId()
	{
		int num = 2;
		WriteActionParamValue("storeId", num);
	}

	private void SetParameter_SnsPlatform()
	{
		int apolloPlatform = m_apolloPlatform2;
		WriteActionParamValue("platform_sns", apolloPlatform);
	}

	private void GetResponse_SessionId(JsonData jdata)
	{
		resultSessionId = NetUtil.GetJsonString(jdata, "sessionId");
	}

	private void GetResponse_SettingData(JsonData jdata)
	{
		resultEnergyRefreshTime = NetUtil.GetJsonLong(jdata, "energyRecoveryTime");
		resultInviteBaseIncentive = NetUtil.AnalyzeItemStateJson(jdata, "inviteBasicIncentiv");
		resultRentalBaseIncentive = NetUtil.AnalyzeItemStateJson(jdata, "chaoRentalBasicIncentiv");
		userName = NetUtil.GetJsonString(jdata, "userName");
		userId = NetUtil.GetJsonString(jdata, "userId");
		password = NetUtil.GetJsonString(jdata, "password");
		countryCode = NetUtil.GetJsonString(jdata, "country_code");
	}
}
