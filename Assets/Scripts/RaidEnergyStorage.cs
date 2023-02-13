using System;
using UnityEngine;

public class RaidEnergyStorage : MonoBehaviour
{
	private const float REFRESH_TIME = 30f;

	private const float UPDATE_TIME = 0.2f;

	private const uint FILL_UP_BORDER_COUNT = 3u;

	[SerializeField]
	private UILabel m_lblEnergy;

	[SerializeField]
	private UILabel m_lblOverEnergy;

	[SerializeField]
	private UILabel m_lblTime;

	private DateTime m_renew_at_time = DateTime.MinValue;

	private TimeSpan m_refresh_time = new TimeSpan(0, 0, 1);

	private TimeSpan m_current_time = new TimeSpan(0, 0, 0);

	private uint m_energy_count = 3u;

	private uint m_energyStock_count;

	private uint m_energyAdd_count;

	private uint m_energyAdd_max;

	private int m_last_count = -1;

	private long m_count_upd;

	private float m_update_time;

	private float m_time;

	public uint energyCount
	{
		get
		{
			return m_energy_count + m_energyStock_count + m_energyAdd_count;
		}
	}

	public static bool CheckEnergy(ref int energyFree, ref int energyBuy, ref DateTime atTime)
	{
		if ((long)(energyFree + energyBuy) < 3L)
		{
			bool flag = atTime <= NetBase.GetCurrentTime();
			int num = 0;
			if (flag)
			{
				while (flag)
				{
					num++;
					if ((long)(energyFree + energyBuy + num) >= 3L)
					{
						atTime = DateTime.MinValue;
						flag = false;
					}
					else
					{
						atTime += new TimeSpan(0, 0, 1800);
						flag = (atTime <= NetBase.GetCurrentTime());
					}
				}
				if (num > 0)
				{
					energyFree += num;
					if ((long)(energyFree + energyBuy) >= 3L)
					{
						atTime = DateTime.MinValue;
					}
				}
				return true;
			}
		}
		return false;
	}

	private bool CanAddEnergy()
	{
		if (!IsFillUpCount())
		{
			return m_renew_at_time <= GetCurrentTime();
		}
		return false;
	}

	public bool IsFillUpCount()
	{
		return m_refresh_time.Ticks <= 0;
	}

	public void Init()
	{
		OnUpdateSaveDataDisplay();
		m_count_upd = 0L;
		if (!IsFillUpCount())
		{
			m_current_time = GetRestTimeForRenew();
			if (m_lblEnergy != null)
			{
				m_lblEnergy.text = energyCount.ToString();
				if (m_last_count <= energyCount || m_last_count < 0)
				{
					m_lblEnergy.gameObject.SetActive(true);
					m_last_count = (int)energyCount;
				}
			}
			if (m_lblOverEnergy != null)
			{
				m_lblOverEnergy.gameObject.SetActive(false);
			}
			if (m_lblTime != null)
			{
				if (m_current_time.Minutes >= 0 && m_current_time.Seconds >= 0)
				{
					m_lblTime.text = string.Format("{0:D2}:{1:D2}", m_current_time.Minutes, m_current_time.Seconds);
				}
				m_lblTime.gameObject.SetActive(true);
			}
			m_update_time = 0.2f;
			return;
		}
		if (m_lblEnergy != null)
		{
			m_lblEnergy.gameObject.SetActive(false);
		}
		if (m_lblOverEnergy != null)
		{
			m_lblOverEnergy.gameObject.SetActive(true);
			if (m_last_count <= energyCount || m_last_count < 0)
			{
				m_lblOverEnergy.text = energyCount.ToString();
				m_last_count = (int)energyCount;
			}
		}
		if (m_lblTime != null)
		{
			m_lblTime.gameObject.SetActive(false);
		}
		m_update_time = 1f;
	}

	private void InitEnergy()
	{
		if (!IsFillUpCount())
		{
			m_current_time = GetRestTimeForRenew();
			if (m_lblEnergy != null)
			{
				m_lblEnergy.gameObject.SetActive(true);
				m_lblEnergy.text = energyCount.ToString();
				m_last_count = (int)energyCount;
			}
			if (m_lblOverEnergy != null)
			{
				m_lblOverEnergy.gameObject.SetActive(false);
			}
			if (m_lblTime != null)
			{
				m_lblTime.gameObject.SetActive(true);
				if (m_current_time.Minutes >= 0 && m_current_time.Seconds >= 0)
				{
					m_lblTime.text = string.Format("{0:D2}:{1:D2}", m_current_time.Minutes, m_current_time.Seconds);
				}
			}
		}
		else
		{
			if (m_lblEnergy != null)
			{
				m_lblEnergy.gameObject.SetActive(false);
			}
			if (m_lblOverEnergy != null)
			{
				m_lblOverEnergy.gameObject.SetActive(true);
				m_lblOverEnergy.text = energyCount.ToString();
				m_last_count = (int)energyCount;
			}
			if (m_lblTime != null)
			{
				m_lblTime.gameObject.SetActive(false);
			}
		}
	}

	private void UpdateEnergy()
	{
		m_update_time -= Time.deltaTime;
		if (!(m_update_time <= 0f))
		{
			return;
		}
		m_count_upd++;
		if (!IsFillUpCount())
		{
			m_current_time = GetRestTimeForRenew();
			if (m_lblEnergy != null)
			{
				m_lblEnergy.gameObject.SetActive(true);
				if (m_last_count <= energyCount || m_count_upd > 3)
				{
					m_lblEnergy.text = energyCount.ToString();
					m_last_count = (int)energyCount;
					m_count_upd = 0L;
				}
			}
			if (m_lblOverEnergy != null)
			{
				m_lblOverEnergy.gameObject.SetActive(false);
			}
			if (m_lblTime != null)
			{
				m_lblTime.gameObject.SetActive(true);
				if (m_current_time.Minutes >= 0 && m_current_time.Seconds >= 0)
				{
					m_lblTime.text = string.Format("{0:D2}:{1:D2}", m_current_time.Minutes, m_current_time.Seconds);
				}
			}
			m_update_time = 0.2f;
			return;
		}
		if (m_lblEnergy != null)
		{
			m_lblEnergy.gameObject.SetActive(false);
		}
		if (m_lblOverEnergy != null)
		{
			m_lblOverEnergy.gameObject.SetActive(true);
			if (m_last_count <= energyCount || m_count_upd > 3)
			{
				m_lblOverEnergy.text = energyCount.ToString();
				m_last_count = (int)energyCount;
				m_count_upd = 0L;
			}
		}
		if (m_lblTime != null)
		{
			m_lblTime.gameObject.SetActive(false);
		}
		m_update_time = 1f;
	}

	private void Update()
	{
		m_time += Time.deltaTime;
		if (!(m_time > 0.1f))
		{
			return;
		}
		bool flag = CanAddEnergy();
		if (flag)
		{
			while (flag)
			{
				m_energyAdd_count++;
				if (m_energyAdd_count >= m_energyAdd_max)
				{
					m_renew_at_time = DateTime.MinValue;
					m_refresh_time = new TimeSpan(0, 0, 0);
					flag = false;
				}
				else
				{
					m_renew_at_time += m_refresh_time;
					flag = CanAddEnergy();
				}
			}
			ReflectChallengeCount();
		}
		UpdateEnergy();
	}

	public TimeSpan GetRestTimeForRenew()
	{
		return m_renew_at_time - GetCurrentTime();
	}

	private DateTime GetCurrentTime()
	{
		return NetBase.GetCurrentTime();
	}

	public void ReflectChallengeCount()
	{
		if (EventManager.Instance != null)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Debug.Log("+ RaidEnergyStorage ReflectChallengeCount ChallengeCount:" + EventManager.Instance.RaidbossChallengeCount + " !!!!!!!!!!!!!!!!! time:" + realtimeSinceStartup);
			if (EventManager.Instance.RaidBossState != null)
			{
				EventManager.Instance.RaidBossState.RaidBossEnergy = (int)(m_energy_count + m_energyAdd_count);
				EventManager.Instance.RaidBossState.EnergyRenewsAt = m_renew_at_time;
			}
			Debug.Log("- RaidEnergyStorage ReflectChallengeCount ChallengeCount:" + EventManager.Instance.RaidbossChallengeCount + " !!!!!!!!!!!!!!!!! time:" + realtimeSinceStartup);
		}
	}

	private void OnUpdateSaveDataDisplay()
	{
		m_time = 0f;
		m_energy_count = 0u;
		m_energyStock_count = 0u;
		m_energyAdd_count = 0u;
		m_energyAdd_max = 0u;
		m_renew_at_time = DateTime.MinValue;
		m_refresh_time = new TimeSpan(0, 0, 1);
		m_last_count = -1;
		EventManager instance = EventManager.Instance;
		if (instance != null)
		{
			ServerEventUserRaidBossState raidBossState = instance.RaidBossState;
			if (raidBossState != null)
			{
				m_energy_count = (uint)raidBossState.RaidBossEnergy;
				m_energyStock_count = (uint)raidBossState.RaidbossEnergyBuy;
				if (m_energy_count + m_energyStock_count < 3)
				{
					m_energyAdd_max = 3 - (m_energy_count + m_energyStock_count);
					m_refresh_time = new TimeSpan(0, 0, 1800);
					m_renew_at_time = raidBossState.EnergyRenewsAt;
				}
				else
				{
					m_renew_at_time = DateTime.MinValue;
					m_refresh_time = new TimeSpan(0, 0, 0);
				}
			}
		}
		InitEnergy();
	}
}
