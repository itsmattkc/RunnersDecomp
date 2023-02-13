using System.Collections.Generic;

public class SocialTaskManager
{
	private List<SocialTaskBase> m_taskList;

	public SocialTaskManager()
	{
		m_taskList = new List<SocialTaskBase>();
	}

	public void RequestProcess(SocialTaskBase task)
	{
		if (task != null)
		{
			string taskName = task.GetTaskName();
			Debug.Log("SocialTaskManager:Request Process  " + taskName);
			m_taskList.Add(task);
		}
	}

	public void Update()
	{
		if (m_taskList.Count <= 0)
		{
			return;
		}
		List<SocialTaskBase> list = new List<SocialTaskBase>();
		SocialTaskBase socialTaskBase = m_taskList[0];
		socialTaskBase.Update();
		if (socialTaskBase.IsDone())
		{
			string taskName = socialTaskBase.GetTaskName();
			Debug.Log("SocialTaskManager:" + taskName + " is Done");
			list.Add(socialTaskBase);
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (SocialTaskBase item in list)
		{
			m_taskList.Remove(item);
		}
		list.Clear();
	}
}
