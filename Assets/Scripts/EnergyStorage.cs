using System;
using UnityEngine;

public class EnergyStorage : MonoBehaviour
{
	private DateTime m_renew_at_time = DateTime.MinValue;

	private TimeSpan m_refresh_time = new TimeSpan(0, 30, 0);

	private uint m_energy_count = 1u;

	private uint m_energyRecoveryMax = 1u;

	public uint Count
	{
		get
		{
			return m_energy_count;
		}
	}

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		ServerSettingState settingState = ServerInterface.SettingState;
		if (settingState != null)
		{
			m_energyRecoveryMax = (uint)settingState.m_energyRecoveryMax;
		}
		OnUpdateSaveDataDisplay();
	}

	private bool CanAddEnergy()
	{
		return m_energy_count < m_energyRecoveryMax && m_renew_at_time <= GetCurrentTime();
	}

	public bool IsFillUpCount()
	{
		return m_energy_count >= m_energyRecoveryMax;
	}

	public void Update()
	{
		bool flag = CanAddEnergy();
		if (flag)
		{
			while (flag)
			{
				m_energy_count++;
				m_renew_at_time += m_refresh_time;
				flag = CanAddEnergy();
			}
			ReflectChallengeCount();
			HudMenuUtility.SendMsgUpdateChallengeDisply();
		}
	}

	public TimeSpan GetRestTimeForRenew()
	{
		return m_renew_at_time - GetCurrentTime();
	}

	private DateTime GetCurrentTime()
	{
		return NetBase.GetCurrentTime();
	}

	private void ReflectChallengeCount()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			instance.PlayerData.ChallengeCount = m_energy_count;
			instance.SavePlayerData();
		}
		if (ServerInterface.PlayerState != null)
		{
			ServerInterface.PlayerState.m_energyRenewsAt = m_renew_at_time;
		}
	}

	private void OnEnergyAwarded(uint energyToAward)
	{
		m_energy_count += energyToAward;
	}

	private void OnUpdateSaveDataDisplay()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			m_energy_count = instance.PlayerData.ChallengeCount;
		}
		if (ServerInterface.PlayerState != null)
		{
			m_renew_at_time = ServerInterface.PlayerState.m_energyRenewsAt;
		}
		if (ServerInterface.SettingState != null)
		{
			int seconds = (int)ServerInterface.SettingState.m_energyRefreshTime;
			m_refresh_time = new TimeSpan(0, 0, seconds);
		}
	}
}
