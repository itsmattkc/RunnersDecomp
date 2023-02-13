using UnityEngine;

public class SingletonGameObject<T> : MonoBehaviour where T : MonoBehaviour
{
	[SerializeField]
	[Header("シーン切替時の削除設定(true:削除)")]
	private bool m_isOnLoadDestroy;

	private static bool s_isDestroy;

	private static T s_instance;

	public static T Instance
	{
		get
		{
			if ((Object)s_instance == (Object)null && !s_isDestroy)
			{
				s_instance = (T)Object.FindObjectOfType(typeof(T));
				if ((Object)s_instance != (Object)null)
				{
					string text = typeof(T).ToString();
					if (text.IndexOf("Debug") != -1 || text.IndexOf("debug") != -1)
					{
						Object.Destroy(s_instance.gameObject);
						s_instance = (T)null;
						s_isDestroy = true;
						Debug.Log("debug SingletonGameObject auto delete !!! :" + text);
					}
				}
			}
			return s_instance;
		}
	}

	private void Awake()
	{
		if (this != Instance)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		s_instance = (T)Object.FindObjectOfType(typeof(T));
		if (!m_isOnLoadDestroy)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public static void Remove()
	{
		if ((Object)s_instance != (Object)null)
		{
			Object.Destroy(s_instance.gameObject);
			s_instance = (T)null;
			s_isDestroy = true;
		}
	}
}
