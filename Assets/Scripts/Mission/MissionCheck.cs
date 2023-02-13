using DataTable;
using System;

namespace Mission
{
	public abstract class MissionCheck
	{
		protected MissionData m_data;

		protected MissionCompleteFunc m_funcMissionComplete;

		protected bool m_isCompleted;

		public abstract void ProcEvent(MissionEvent missionEvent);

		public virtual void Update(float deltaTime)
		{
		}

		public virtual void SetInitialValue(long initialValue)
		{
		}

		public virtual long GetValue()
		{
			return 0L;
		}

		public void SetData(MissionData data)
		{
			m_data = data;
		}

		public MissionData GetData()
		{
			return m_data;
		}

		public virtual void SetDataExtra()
		{
		}

		public bool IsCompleted()
		{
			return m_isCompleted;
		}

		public int GetIndex()
		{
			return m_data.id;
		}

		public void SetOnCompleteFunc(MissionCompleteFunc func)
		{
			m_funcMissionComplete = (MissionCompleteFunc)Delegate.Combine(m_funcMissionComplete, func);
		}

		public virtual void DebugComplete(int missionNo)
		{
			SetCompleted();
		}

		protected void SetCompleted()
		{
			m_isCompleted = true;
			if (m_funcMissionComplete != null)
			{
				m_funcMissionComplete(m_data);
			}
		}
	}
}
