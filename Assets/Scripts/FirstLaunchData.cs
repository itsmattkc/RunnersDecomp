using UnityEngine;

public class FirstLaunchData : MonoBehaviour
{
	public enum Type
	{
		TYPE_NONE = -1,
		TYPE_GET_CHAOEGG,
		TYPE_MILEAGE_START,
		TYPE_MILEAGE_AFTER_TUTORIAL,
		TYPE_MILEAGE_BOSS_LOSE_FIRST,
		TYPE_MILEAGE_BOSS_LOSE,
		TYPE_NUM
	}

	private static FirstLaunchData m_instance;

	private bool[] m_isLaunched = new bool[5];

	public bool IsDone(Type type)
	{
		if (type >= Type.TYPE_NUM)
		{
			Debug.Log("FirstLaunchData.IsDone: Invalid parameter");
			return false;
		}
		return m_isLaunched[(int)type];
	}

	public void Register(Type type, bool isLaunched)
	{
		if (type >= Type.TYPE_NUM)
		{
			Debug.Log("FirstLaunchData.Register: Invalid parameter");
			return;
		}
		m_isLaunched[(int)type] = isLaunched;
		StoreSaveData();
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			m_instance = this;
			LoadSaveData();
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		m_instance = null;
	}

	private void LoadSaveData()
	{
	}

	private void StoreSaveData()
	{
	}
}
