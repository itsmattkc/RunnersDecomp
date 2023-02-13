using DataTable;
using SaveData;
using System;
using UnityEngine;

namespace Chao
{
	[Serializable]
	public class ChaoParameter : MonoBehaviour
	{
		[SerializeField]
		private int m_mainChaoId = -1;

		[SerializeField]
		private int m_mainChaoLevel;

		[SerializeField]
		private int m_subChaoId = -1;

		[SerializeField]
		private int m_subChaoLevel;

		public ChaoParameterData m_data;

		public ChaoParameterData Data
		{
			get
			{
				return m_data;
			}
			set
			{
				m_data = value;
			}
		}

		private void Start()
		{
			base.enabled = false;
		}

		public bool IsChaoParameterDataDebugFlag()
		{
			return false;
		}

		private void SetDebugChao()
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				int setChaoId = GetSetChaoId(instance.PlayerData, true);
				if (setChaoId >= 0)
				{
					m_mainChaoId = setChaoId;
					m_mainChaoLevel = GetChaoLevel(instance.ChaoData, m_mainChaoId);
				}
				else
				{
					SetMainChao(instance.PlayerData, instance.ChaoData);
				}
				int setChaoId2 = GetSetChaoId(instance.PlayerData, false);
				if (setChaoId2 >= 0)
				{
					m_subChaoId = setChaoId2;
					m_subChaoLevel = GetChaoLevel(instance.ChaoData, m_subChaoId);
				}
				else
				{
					SetSubChao(instance.PlayerData, instance.ChaoData);
				}
			}
		}

		private void SetMainChao(PlayerData playerData, SaveData.ChaoData chaoData)
		{
			if (playerData != null)
			{
				playerData.MainChaoID = m_mainChaoId;
				m_mainChaoLevel = Mathf.Clamp(m_mainChaoLevel, 0, ChaoTable.ChaoMaxLevel());
				SetChaoLevel(chaoData, m_mainChaoId, m_mainChaoLevel);
			}
		}

		private void SetSubChao(PlayerData playerData, SaveData.ChaoData chaoData)
		{
			if (playerData != null)
			{
				playerData.SubChaoID = m_subChaoId;
				m_subChaoLevel = Mathf.Clamp(m_subChaoLevel, 0, ChaoTable.ChaoMaxLevel());
				SetChaoLevel(chaoData, m_subChaoId, m_subChaoLevel);
			}
		}

		private void SetChaoLevel(SaveData.ChaoData chaoData, int chaoId, int level)
		{
			if (chaoData == null)
			{
				return;
			}
			for (int i = 0; i < chaoData.CHAO_MAX_NUM; i++)
			{
				if (chaoData.Info[i].chao_id == -1)
				{
					chaoData.Info[i].chao_id = chaoId;
					chaoData.Info[i].level = level;
					break;
				}
			}
		}

		private int GetChaoLevel(SaveData.ChaoData chaoData, int chaoId)
		{
			if (chaoData != null)
			{
				for (int i = 0; i < chaoData.CHAO_MAX_NUM; i++)
				{
					if (chaoData.Info[i].chao_id == chaoId)
					{
						return chaoData.Info[i].level;
					}
				}
			}
			return 0;
		}

		private int GetSetChaoId(PlayerData playerData, bool mainChaoFlag)
		{
			if (playerData != null)
			{
				if (mainChaoFlag)
				{
					return playerData.MainChaoID;
				}
				return playerData.SubChaoID;
			}
			return -1;
		}
	}
}
