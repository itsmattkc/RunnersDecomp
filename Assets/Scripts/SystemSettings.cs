using SaveData;
using UnityEngine;

public class SystemSettings : MonoBehaviour
{
	public enum QualityLevel
	{
		Normal,
		Low
	}

	private class SystemInformation
	{
		public QualityLevel m_qualityLevel;

		public string m_deviceModel;

		public int m_targetFrameRate = 60;

		public int m_unityQualityLevel;
	}

	private static SystemInformation m_information = new SystemInformation();

	private static SystemSettings instance = null;

	public static int TargetFrameRate
	{
		get
		{
			if (m_information != null)
			{
				return m_information.m_targetFrameRate;
			}
			return 60;
		}
		set
		{
			if (m_information != null)
			{
				m_information.m_targetFrameRate = value;
			}
		}
	}

	public static string DeviceModel
	{
		get
		{
			if (m_information != null)
			{
				return m_information.m_deviceModel;
			}
			return null;
		}
	}

	public static SystemSettings Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (Object.FindObjectOfType(typeof(SystemSettings)) as SystemSettings);
			}
			return instance;
		}
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.AutoRotation;
		InitInformation();
	}

	private void Update()
	{
	}

	private void InitInformation()
	{
		m_information.m_deviceModel = SystemInfo.deviceModel;
		m_information.m_targetFrameRate = 60;
		m_information.m_unityQualityLevel = QualitySettings.GetQualityLevel();
	}

	public static void ChangeQualityLevel(QualityLevel level)
	{
		if (m_information != null)
		{
			m_information.m_qualityLevel = level;
			switch (level)
			{
			case QualityLevel.Normal:
				m_information.m_targetFrameRate = 60;
				m_information.m_unityQualityLevel = 1;
				QualitySettings.SetQualityLevel(m_information.m_unityQualityLevel);
				break;
			case QualityLevel.Low:
				m_information.m_targetFrameRate = 30;
				m_information.m_unityQualityLevel = 0;
				QualitySettings.SetQualityLevel(m_information.m_unityQualityLevel);
				break;
			}
		}
	}

	public static void ChangeQualityLevelBySaveData()
	{
		SystemData systemSaveData = SystemSaveManager.GetSystemSaveData();
		if (systemSaveData != null)
		{
			ChangeQualityLevel(systemSaveData.lightMode ? QualityLevel.Low : QualityLevel.Normal);
		}
	}

	private bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}
}
