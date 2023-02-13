using System.Collections.Generic;

namespace SaveData
{
	public class ChaoData
	{
		public struct ChaoDataInfo
		{
			public int chao_id;

			public int level;
		}

		public int CHAO_MAX_NUM = 2;

		private ChaoDataInfo[] m_info;

		public ChaoDataInfo[] Info
		{
			get
			{
				return m_info;
			}
			set
			{
				m_info = value;
			}
		}

		public ChaoData()
		{
			m_info = new ChaoDataInfo[CHAO_MAX_NUM];
			for (int i = 0; i < CHAO_MAX_NUM; i++)
			{
				m_info[i].chao_id = -1;
				m_info[i].level = -1;
			}
		}

		public ChaoData(List<ServerChaoState> chaoStates)
		{
			int count = chaoStates.Count;
			if (count <= 0)
			{
				return;
			}
			m_info = new ChaoDataInfo[count];
			for (int i = 0; i < count; i++)
			{
				ServerChaoState serverChaoState = chaoStates[i];
				if (serverChaoState.Status > ServerChaoState.ChaoStatus.NotOwned)
				{
					ServerItem serverItem = new ServerItem((ServerItem.Id)serverChaoState.Id);
					m_info[i].chao_id = serverItem.chaoId;
					m_info[i].level = serverChaoState.Level;
				}
				else
				{
					m_info[i].chao_id = -1;
					m_info[i].level = -1;
				}
			}
			CHAO_MAX_NUM = count;
		}

		public uint GetChaoCount()
		{
			uint num = 0u;
			ChaoDataInfo[] info = Info;
			for (int i = 0; i < info.Length; i++)
			{
				ChaoDataInfo chaoDataInfo = info[i];
				if (chaoDataInfo.chao_id != -1)
				{
					num++;
				}
			}
			return num;
		}
	}
}
