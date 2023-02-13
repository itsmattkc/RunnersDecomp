using UnityEngine;

public class LoadURLComponent : MonoBehaviour
{
	private static LoadURLComponent m_instance;

	private void Start()
	{
		if (m_instance == null)
		{
			Debug.Log("LoadURLComponent:Created");
			m_instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			Init();
		}
		else
		{
			Debug.Log("LoadURLComponent:Destroyed");
			Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
	}

	private void Init()
	{
		DebugSaveServerUrl.LoadURL();
		Debug.Log("LoadURLComponent:LoadURL");
	}
}
