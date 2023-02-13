using UnityEngine;

public class MainMenuReset : MonoBehaviour
{
	private bool m_isLoadedScene;

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_isLoadedScene)
		{
			Application.LoadLevel("MainMenu");
		}
	}
}
