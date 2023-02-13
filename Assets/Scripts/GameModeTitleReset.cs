using UnityEngine;

public class GameModeTitleReset : MonoBehaviour
{
	private bool m_isLoadedLevel;

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_isLoadedLevel)
		{
			Application.LoadLevel(TitleDefine.TitleSceneName);
			m_isLoadedLevel = true;
		}
	}
}
