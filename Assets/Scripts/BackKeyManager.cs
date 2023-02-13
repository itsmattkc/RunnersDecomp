using System.Collections.Generic;
using UnityEngine;

public class BackKeyManager : MonoBehaviour
{
	private static BackKeyManager instance;

	private List<GameObject> m_windowCallBackList = new List<GameObject>();

	private List<GameObject> m_eventCallBackList = new List<GameObject>();

	private List<GameObject> m_tutorialEventCallBackList = new List<GameObject>();

	private GameObject m_mileageCallBack;

	private GameObject m_mainMenuUICallBack;

	private float m_timer;

	private bool m_invalidFlag;

	private bool m_tutorialFlag;

	private bool m_transSceneFlag = true;

	private bool m_sequenceTransitionFlag;

	private bool m_touchedPrevFrame;

	public static BackKeyManager Instance
	{
		get
		{
			return instance;
		}
	}

	public static bool InvalidFlag
	{
		get
		{
			if (instance != null)
			{
				return instance.m_invalidFlag;
			}
			return false;
		}
		set
		{
			if (instance != null)
			{
				instance.m_invalidFlag = value;
			}
		}
	}

	public static bool TutorialFlag
	{
		get
		{
			if (instance != null)
			{
				return instance.m_tutorialFlag;
			}
			return false;
		}
		set
		{
			if (instance != null)
			{
				instance.m_tutorialFlag = value;
			}
		}
	}

	public static bool MenuSequenceTransitionFlag
	{
		get
		{
			if (instance != null)
			{
				return instance.m_sequenceTransitionFlag;
			}
			return false;
		}
		set
		{
			if (instance != null)
			{
				instance.m_sequenceTransitionFlag = value;
			}
		}
	}

	public static void StartScene()
	{
		if (instance != null)
		{
			instance.m_invalidFlag = false;
			instance.m_tutorialFlag = false;
			instance.m_transSceneFlag = false;
			instance.m_sequenceTransitionFlag = false;
		}
	}

	public static void EndScene()
	{
		if (instance != null)
		{
			instance.m_windowCallBackList.Clear();
			instance.m_eventCallBackList.Clear();
			instance.m_tutorialEventCallBackList.Clear();
			instance.m_mileageCallBack = null;
			instance.m_mainMenuUICallBack = null;
			instance.m_transSceneFlag = true;
		}
	}

	public static void AddWindowCallBack(GameObject obj)
	{
		if (instance != null && !instance.m_windowCallBackList.Contains(obj))
		{
			instance.m_windowCallBackList.Add(obj);
		}
	}

	public static void RemoveWindowCallBack(GameObject obj)
	{
		if (instance != null && instance.m_windowCallBackList.Contains(obj))
		{
			instance.m_windowCallBackList.Remove(obj);
		}
	}

	public static void AddEventCallBack(GameObject obj)
	{
		if (instance != null && !instance.m_eventCallBackList.Contains(obj))
		{
			instance.m_eventCallBackList.Add(obj);
		}
	}

	public static void AddMainMenuUI(GameObject obj)
	{
		if (instance != null && instance.m_mainMenuUICallBack == null)
		{
			instance.m_mainMenuUICallBack = obj;
		}
	}

	public static void AddMileageCallBack(GameObject obj)
	{
		if (instance != null && instance.m_mileageCallBack == null)
		{
			instance.m_mileageCallBack = obj;
		}
	}

	public static void AddTutorialEventCallBack(GameObject obj)
	{
		if (instance != null && !instance.m_tutorialEventCallBackList.Contains(obj))
		{
			instance.m_tutorialEventCallBackList.Add(obj);
		}
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (m_timer > 0f)
		{
			m_timer -= RealTime.deltaTime;
		}
		bool flag = (UICamera.touchCount > 0) ? true : false;
		if (Input.GetKey(KeyCode.Escape) && m_timer <= 0f && !flag && !m_touchedPrevFrame)
		{
			m_timer = 0.6f;
			if (NetworkErrorWindow.Created)
			{
				NetworkErrorWindow.OnClickPlatformBackButton();
				return;
			}
			if (m_invalidFlag || m_transSceneFlag || CheckConnectNetwork())
			{
				return;
			}
			if (m_tutorialFlag)
			{
				WindowBase.BackButtonMessage msg = new WindowBase.BackButtonMessage();
				if (m_sequenceTransitionFlag)
				{
					msg.StaySequence();
				}
				SentWindowMessege(ref msg);
				SentMileageMessege(ref msg);
				if (msg.IsFlag(WindowBase.BackButtonMessage.Flags.STAY_SEQUENCE))
				{
					return;
				}
				foreach (GameObject tutorialEventCallBack in m_tutorialEventCallBackList)
				{
					if (tutorialEventCallBack != null)
					{
						tutorialEventCallBack.SendMessage("OnClickPlatformBackButtonTutorialEvent", null, SendMessageOptions.DontRequireReceiver);
					}
				}
				return;
			}
			WindowBase.BackButtonMessage msg2 = new WindowBase.BackButtonMessage();
			if (m_sequenceTransitionFlag)
			{
				msg2.StaySequence();
			}
			SentWindowMessege(ref msg2);
			SendMainMenuUI(ref msg2);
			SentMileageMessege(ref msg2);
			if (!msg2.IsFlag(WindowBase.BackButtonMessage.Flags.STAY_SEQUENCE))
			{
				foreach (GameObject eventCallBack in m_eventCallBackList)
				{
					if (eventCallBack != null)
					{
						eventCallBack.SendMessage("OnClickPlatformBackButtonEvent", null, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
		m_touchedPrevFrame = flag;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private bool CheckConnectNetwork()
	{
		if (NetMonitor.Instance != null)
		{
			return !NetMonitor.Instance.IsIdle();
		}
		return false;
	}

	private void SentWindowMessege(ref WindowBase.BackButtonMessage msg)
	{
		if (m_windowCallBackList.Count <= 0)
		{
			return;
		}
		foreach (GameObject windowCallBack in m_windowCallBackList)
		{
			if (windowCallBack.activeSelf)
			{
				windowCallBack.SendMessage("OnClickPlatformBackButton", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void SendMainMenuUI(ref WindowBase.BackButtonMessage msg)
	{
		if (!msg.IsFlag(WindowBase.BackButtonMessage.Flags.STAY_SEQUENCE) && m_mainMenuUICallBack != null)
		{
			m_mainMenuUICallBack.SendMessage("OnClickPlatformBackButton", msg, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SentMileageMessege(ref WindowBase.BackButtonMessage msg)
	{
		if (!msg.IsFlag(WindowBase.BackButtonMessage.Flags.STAY_SEQUENCE) && m_mileageCallBack != null)
		{
			m_mileageCallBack.SendMessage("OnClickPlatformBackButton", msg, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SendEventMessage()
	{
		foreach (GameObject eventCallBack in m_eventCallBackList)
		{
			if (eventCallBack != null)
			{
				eventCallBack.SendMessage("OnPlatformBackButtonClicked", null, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
