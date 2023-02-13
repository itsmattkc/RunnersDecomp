public class ServerSettingState
{
	public long m_energyRefreshTime;

	public int m_energyRecoveryMax;

	public int m_onePlayCmCount;

	public int m_onePlayContinueCount;

	public int m_cmSkipCount;

	public bool m_isPurchased;

	public ServerItemState m_invitBaseIncentive;

	public ServerItemState m_rentalBaseIncentive;

	public int m_subCharaRingPayment;

	public string m_userName;

	public string m_userId;

	public int m_monthPurchase;

	public string m_birthday;

	public int m_countryId;

	public string m_countryCode;

	public ServerSettingState()
	{
		m_energyRefreshTime = 0L;
		m_energyRecoveryMax = 1;
		m_invitBaseIncentive = new ServerItemState();
		m_rentalBaseIncentive = new ServerItemState();
		m_subCharaRingPayment = 300;
		m_userName = string.Empty;
		m_userId = string.Empty;
		m_monthPurchase = 0;
		m_birthday = string.Empty;
		m_countryId = 0;
		m_countryCode = string.Empty;
		m_onePlayCmCount = 0;
		m_onePlayContinueCount = 0;
		m_isPurchased = false;
	}

	public void CopyTo(ServerSettingState to)
	{
		to.m_energyRefreshTime = m_energyRefreshTime;
		to.m_energyRecoveryMax = m_energyRecoveryMax;
		m_invitBaseIncentive.CopyTo(to.m_invitBaseIncentive);
		m_rentalBaseIncentive.CopyTo(to.m_rentalBaseIncentive);
		to.m_subCharaRingPayment = m_subCharaRingPayment;
		to.m_userName = string.Copy(m_userName);
		to.m_userId = string.Copy(m_userId);
		to.m_monthPurchase = m_monthPurchase;
		to.m_birthday = m_birthday;
		to.m_countryId = m_countryId;
		to.m_countryCode = m_countryCode;
		to.m_onePlayCmCount = m_onePlayCmCount;
		to.m_onePlayContinueCount = m_onePlayContinueCount;
		to.m_isPurchased = m_isPurchased;
	}
}
