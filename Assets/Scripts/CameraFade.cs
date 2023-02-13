using System;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
	private static CameraFade mInstance;

	public GUIStyle m_BackgroundStyle = new GUIStyle();

	public Texture2D m_FadeTexture;

	public Color m_CurrentScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	public Color m_TargetScreenOverlayColor = new Color(0f, 0f, 0f, 0f);

	public Color m_DeltaColor = new Color(0f, 0f, 0f, 0f);

	public int m_FadeGUIDepth = -1000;

	public float m_FadeDelay;

	public Action m_OnFadeFinish;

	private static CameraFade instance
	{
		get
		{
			if (mInstance == null)
			{
				GameObject gameObject = new GameObject("CameraFade");
				if (gameObject != null)
				{
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					mInstance = gameObject.AddComponent<CameraFade>();
				}
			}
			return mInstance;
		}
	}

	private void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
			instance.init();
		}
	}

	public void init()
	{
		instance.m_FadeTexture = new Texture2D(1, 1);
		instance.m_BackgroundStyle.normal.background = instance.m_FadeTexture;
	}

	private void OnGUI()
	{
		if (Time.time > instance.m_FadeDelay && instance.m_CurrentScreenOverlayColor != instance.m_TargetScreenOverlayColor)
		{
			if (Mathf.Abs(instance.m_CurrentScreenOverlayColor.a - instance.m_TargetScreenOverlayColor.a) < Mathf.Abs(instance.m_DeltaColor.a) * Time.deltaTime)
			{
				instance.m_CurrentScreenOverlayColor = instance.m_TargetScreenOverlayColor;
				SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor);
				instance.m_DeltaColor = new Color(0f, 0f, 0f, 0f);
				if (instance.m_OnFadeFinish != null)
				{
					instance.m_OnFadeFinish();
				}
			}
			else
			{
				SetScreenOverlayColor(instance.m_CurrentScreenOverlayColor + instance.m_DeltaColor * Time.deltaTime);
			}
		}
		if (m_CurrentScreenOverlayColor.a > 0f)
		{
			GUI.depth = instance.m_FadeGUIDepth;
			GUI.Label(new Rect(-20f, -20f, Screen.width + 20, Screen.height + 20), instance.m_FadeTexture, instance.m_BackgroundStyle);
		}
	}

	private static void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		instance.m_CurrentScreenOverlayColor = newScreenOverlayColor;
		instance.m_FadeTexture.SetPixel(0, 0, instance.m_CurrentScreenOverlayColor);
		instance.m_FadeTexture.Apply();
	}

	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration)
	{
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	public static void StartAlphaFade(Color nowScreenOverlayColor, Color newScreenOverlayColor, bool isFadeIn, float fadeDuration)
	{
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(nowScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(nowScreenOverlayColor.r, nowScreenOverlayColor.g, nowScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay)
	{
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		instance.m_FadeDelay = Time.time + fadeDelay;
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	public static void StartAlphaFade(Color newScreenOverlayColor, bool isFadeIn, float fadeDuration, float fadeDelay, Action OnFadeFinish)
	{
		if (fadeDuration <= 0f)
		{
			SetScreenOverlayColor(newScreenOverlayColor);
			return;
		}
		instance.m_OnFadeFinish = OnFadeFinish;
		instance.m_FadeDelay = Time.time + fadeDelay;
		if (isFadeIn)
		{
			instance.m_TargetScreenOverlayColor = new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f);
			SetScreenOverlayColor(newScreenOverlayColor);
		}
		else
		{
			instance.m_TargetScreenOverlayColor = newScreenOverlayColor;
			SetScreenOverlayColor(new Color(newScreenOverlayColor.r, newScreenOverlayColor.g, newScreenOverlayColor.b, 0f));
		}
		instance.m_DeltaColor = (instance.m_TargetScreenOverlayColor - instance.m_CurrentScreenOverlayColor) / fadeDuration;
	}

	private void OnApplicationQuit()
	{
		mInstance = null;
	}
}
