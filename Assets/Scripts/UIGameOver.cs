using System.Collections;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/GameMode/Stage")]
public class UIGameOver : MonoBehaviour
{
	private const float windowWidth = 250f;

	private const float windowHeight = 400f;

	private Rect m_windowRect;

	public float m_totalDistance;

	private float m_drawDistance;

	private bool m_onButton;

	private bool m_buttonPressed;

	private GUISkin m_skin;

	private void Start()
	{
		float left = ((float)Screen.width - 250f) * 0.5f;
		float top = ((float)Screen.height - 400f) * 0.5f;
		m_windowRect = new Rect(left, top, 250f, 400f);
		StartCoroutine("waitForDrawDistance", 2f);
	}

	private void Update()
	{
		float num = m_totalDistance - m_drawDistance;
		float num2 = (!(num > 100f)) ? 40 : 400;
		m_drawDistance += Time.deltaTime * num2;
		if (m_drawDistance >= m_totalDistance)
		{
			m_drawDistance = m_totalDistance;
			m_onButton = true;
		}
	}

	public void SetSkin(GUISkin skin)
	{
		m_skin = skin;
		m_skin.window.fontSize = 20;
		m_skin.window.normal.textColor = Color.white;
		m_skin.button.fontSize = 20;
		m_skin.button.normal.textColor = Color.white;
		m_skin.label.fontSize = 20;
		m_skin.label.normal.textColor = Color.white;
	}

	public void OnGUI()
	{
		GUI.backgroundColor = Color.green;
		m_windowRect = GUI.Window(1, m_windowRect, DoMyWindow, "Game Over", m_skin.window);
		if (m_onButton)
		{
		}
	}

	private void DoMyWindow(int windowID)
	{
		string text = "Total Distance:" + Mathf.FloorToInt(m_drawDistance);
		GUIContent gUIContent = new GUIContent();
		gUIContent.text = text;
		Rect position = new Rect(5f, 40f, 180f, 40f);
		GUI.Label(position, gUIContent, m_skin.label);
		float left = 10f;
		float num = 100f;
		if (GUI.Button(new Rect(left, num, 230f, 40f), "Retry", m_skin.button) && !m_buttonPressed)
		{
			Application.LoadLevel("s_w01StageTestmap");
			m_buttonPressed = true;
		}
		num += 180f;
		if (GUI.Button(new Rect(left, num, 230f, 40f), "Go Title", m_skin.button) && !m_buttonPressed)
		{
			Application.LoadLevel("s_title1st");
			m_buttonPressed = true;
		}
	}

	private IEnumerator waitForDrawDistance(float time)
	{
		yield return new WaitForSeconds(time);
	}
}
