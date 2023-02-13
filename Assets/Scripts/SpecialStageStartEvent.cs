using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class SpecialStageStartEvent : MonoBehaviour
{
	private enum Mode
	{
		Idle,
		Load,
		WaitLoad,
		Start,
		WaitEnd,
		NextPicture,
		DailyMission,
		End
	}

	private Mode m_mode;

	private ResourceSceneLoader m_sceneLoader;

	private int m_textWindowId = -1;

	private List<string> m_loadTexList = new List<string>();

	private WindowEventData m_windowEventData;

	private DailyWindowUI m_dailyWindowUI;

	private bool m_isNotPlaybackDefaultBgm;

	private bool m_isEnd;

	private bool m_alertFlag;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	private void OnDestroy()
	{
		SetColision(false);
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
		if (m_windowEventData != null)
		{
			m_windowEventData = null;
		}
		if (m_dailyWindowUI != null)
		{
			m_dailyWindowUI = null;
		}
	}

	public static SpecialStageStartEvent Create(GameObject obj)
	{
		if (obj != null)
		{
			SpecialStageStartEvent specialStageStartEvent = obj.GetComponent<SpecialStageStartEvent>();
			if (specialStageStartEvent == null)
			{
				specialStageStartEvent = obj.AddComponent<SpecialStageStartEvent>();
			}
			else if (GeneralWindow.IsCreated("SpecialStageStartEvent"))
			{
				GeneralWindow.Close();
			}
			if (specialStageStartEvent != null)
			{
				specialStageStartEvent.m_mode = Mode.Load;
				specialStageStartEvent.SetColision(true);
			}
			return specialStageStartEvent;
		}
		return null;
	}

	public void CloseWindow()
	{
		SetColision(false);
	}

	private bool StartEvent()
	{
		m_windowEventData = EventManager.Instance.GetWindowEvenData(m_textWindowId);
		m_isNotPlaybackDefaultBgm = false;
		m_isEnd = false;
		if (m_windowEventData != null)
		{
			GeneralWindow.CInfo.Event[] array = new GeneralWindow.CInfo.Event[m_windowEventData.body.Length];
			for (int i = 0; i < m_windowEventData.body.Length; i++)
			{
				WindowBodyData windowBodyData = m_windowEventData.body[i];
				GeneralWindow.CInfo.Event.FaceWindow[] array2 = null;
				if (windowBodyData.product != null)
				{
					array2 = new GeneralWindow.CInfo.Event.FaceWindow[windowBodyData.product.Length];
					for (int j = 0; j < windowBodyData.product.Length; j++)
					{
						WindowProductData windowProductData = windowBodyData.product[j];
						array2[j] = new GeneralWindow.CInfo.Event.FaceWindow
						{
							texture = GetTexture(windowProductData.face_id),
							name = ((windowProductData.name_cell_id == null) ? null : MileageMapText.GetName(windowProductData.name_cell_id)),
							effectType = windowProductData.effect,
							animType = windowProductData.anim,
							reverseType = windowProductData.reverse,
							showingType = windowProductData.showing
						};
					}
				}
				array[i] = new GeneralWindow.CInfo.Event
				{
					faceWindows = array2,
					arrowType = windowBodyData.arrow,
					bgmCueName = windowBodyData.bgm,
					seCueName = windowBodyData.se,
					message = GetText(windowBodyData.text_cell_id)
				};
			}
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "SpecialStageStartEvent";
			info.buttonType = GeneralWindow.ButtonType.OkNextSkip;
			info.caption = MileageMapText.GetMapCommonText(m_windowEventData.title_cell_id);
			info.events = array;
			info.isNotPlaybackDefaultBgm = m_isNotPlaybackDefaultBgm;
			info.isSpecialEvent = true;
			GeneralWindow.Create(info);
			return true;
		}
		return false;
	}

	private string GetText(string cellID)
	{
		string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_SPECIFIC, "Production", cellID);
		if (text == null)
		{
			text = "NoText";
		}
		return text;
	}

	private bool LoadTexture()
	{
		m_loadTexList.Clear();
		m_loadTexList = GetResourceNameList();
		if (m_loadTexList.Count > 0)
		{
			CreateLoadObject();
			if (m_sceneLoader != null)
			{
				foreach (string loadTex in m_loadTexList)
				{
					ResourceSceneLoader.ResourceInfo info = new ResourceSceneLoader.ResourceInfo(ResourceCategory.EVENT_RESOURCE, loadTex, true, false, false);
					m_sceneLoader.AddLoadAndResourceManager(info);
				}
				return true;
			}
		}
		return false;
	}

	private void SetTextureData()
	{
		GameObject gameObject = new GameObject("FaceTextures");
		if (!(gameObject != null))
		{
			return;
		}
		if (gameObject != null)
		{
			gameObject.transform.parent = base.transform;
		}
		foreach (string loadTex in m_loadTexList)
		{
			GameObject gameObject2 = GameObject.Find(loadTex);
			if (gameObject2 != null)
			{
				gameObject2.transform.parent = gameObject.transform;
			}
		}
	}

	private void DestroyTextureData()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "FaceTextures");
		if (gameObject != null)
		{
			Object.DestroyImmediate(gameObject);
			Resources.UnloadUnusedAssets();
		}
	}

	private void CreateLoadObject()
	{
		GameObject gameObject = new GameObject("SceneLoader");
		if (gameObject != null)
		{
			m_sceneLoader = gameObject.AddComponent<ResourceSceneLoader>();
		}
	}

	private Texture GetTexture(int id)
	{
		string faceTextureName = MileageMapUtility.GetFaceTextureName(id);
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, faceTextureName);
		if (gameObject != null)
		{
			AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
			if (component != null)
			{
				return component.m_tex;
			}
			return null;
		}
		return MileageMapUtility.GetFaceTexture(id);
	}

	private List<string> GetResourceNameList()
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		if (EventManager.Instance != null)
		{
			WindowEventData windowEvenData = EventManager.Instance.GetWindowEvenData(m_textWindowId);
			if (windowEvenData != null)
			{
				WindowBodyData[] body = windowEvenData.body;
				foreach (WindowBodyData windowBodyData in body)
				{
					for (int j = 0; j < windowBodyData.face_count; j++)
					{
						if (j < windowBodyData.product.Length && !list2.Contains(windowBodyData.product[j].face_id))
						{
							list2.Add(windowBodyData.product[j].face_id);
						}
					}
				}
			}
		}
		if (list2.Count > 0)
		{
			foreach (int item in list2)
			{
				Texture faceTexture = MileageMapUtility.GetFaceTexture(item);
				if (faceTexture == null)
				{
					string faceTextureName = MileageMapUtility.GetFaceTextureName(item);
					GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, faceTextureName);
					if (gameObject == null)
					{
						list.Add(faceTextureName);
					}
				}
			}
			return list;
		}
		return list;
	}

	private string GetResourceName()
	{
		return "EventResourcePictureCardShowTextures" + m_textWindowId.ToString("D2");
	}

	public void SavePictureEvent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.pictureShowEventId = EventManager.Instance.Id;
				systemdata.pictureShowProgress = m_textWindowId;
			}
			instance.SaveSystemData();
		}
	}

	public void StartPlayDailyMissionResult()
	{
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			m_dailyWindowUI = GameObjectUtil.FindChildGameObjectComponent<DailyWindowUI>(menuAnimUIObject, "DailyWindowUI");
			if (m_dailyWindowUI != null)
			{
				m_dailyWindowUI.gameObject.SetActive(true);
				m_dailyWindowUI.PlayStart();
			}
		}
	}

	private int GetNextWindowId()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null && EventManager.Instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			EventProductionData puductionData = EventManager.Instance.GetPuductionData();
			if (systemdata != null && puductionData != null)
			{
				int pictureShowProgress = systemdata.pictureShowProgress;
				for (int i = 0; i < puductionData.startCollectCount.Length; i++)
				{
					int num = puductionData.startCollectCount[i];
					if (EventManager.Instance.CollectCount >= num && i > pictureShowProgress)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public void Update()
	{
		switch (m_mode)
		{
		case Mode.Idle:
			base.enabled = false;
			break;
		case Mode.Load:
			SetColision(true);
			m_textWindowId = GetNextWindowId();
			if (m_textWindowId == -1)
			{
				if (IsDailyMissionComplete())
				{
					StartPlayDailyMissionResult();
					m_mode = Mode.DailyMission;
				}
				else
				{
					m_mode = Mode.End;
				}
			}
			else if (LoadTexture())
			{
				m_mode = Mode.WaitLoad;
			}
			else
			{
				m_mode = Mode.Start;
			}
			break;
		case Mode.WaitLoad:
			if (m_sceneLoader != null && m_sceneLoader.Loaded)
			{
				SetTextureData();
				Object.Destroy(m_sceneLoader.gameObject);
				m_mode = Mode.Start;
			}
			break;
		case Mode.Start:
			if (StartEvent())
			{
				m_mode = Mode.WaitEnd;
			}
			else
			{
				m_mode = Mode.End;
			}
			break;
		case Mode.WaitEnd:
			if (GeneralWindow.IsCreated("SpecialStageStartEvent") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				DestroyTextureData();
				SavePictureEvent();
				if (IsPictureEvent())
				{
					m_mode = Mode.NextPicture;
				}
				else if (IsDailyMissionComplete())
				{
					StartPlayDailyMissionResult();
					m_mode = Mode.DailyMission;
				}
				else
				{
					m_mode = Mode.End;
				}
			}
			break;
		case Mode.NextPicture:
			m_mode = Mode.Load;
			break;
		case Mode.DailyMission:
			if (m_dailyWindowUI != null && m_dailyWindowUI.IsEnd)
			{
				m_dailyWindowUI = null;
				m_mode = Mode.End;
			}
			break;
		case Mode.End:
			m_isEnd = true;
			m_mode = Mode.Idle;
			break;
		}
	}

	private void SetColision(bool flag)
	{
		if (flag)
		{
			if (!m_alertFlag)
			{
				HudMenuUtility.SetConnectAlertSimpleUI(true);
			}
		}
		else if (m_alertFlag)
		{
			HudMenuUtility.SetConnectAlertSimpleUI(false);
		}
		m_alertFlag = flag;
	}

	private bool IsDailyMissionComplete()
	{
		return false;
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (!m_isEnd && msg != null)
		{
			msg.StaySequence();
		}
	}

	public static bool IsPictureEvent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null && EventManager.Instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				if (systemdata.pictureShowEventId != EventManager.Instance.Id)
				{
					return true;
				}
				int pictureShowProgress = systemdata.pictureShowProgress;
				EventProductionData puductionData = EventManager.Instance.GetPuductionData();
				if (puductionData != null)
				{
					int num = 0;
					int[] startCollectCount = puductionData.startCollectCount;
					foreach (int num2 in startCollectCount)
					{
						if (EventManager.Instance.CollectCount >= num2 && num > pictureShowProgress)
						{
							return true;
						}
						num++;
					}
				}
			}
		}
		return false;
	}

	public static List<int> GetProductFaceIdList()
	{
		List<int> list = new List<int>();
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null && EventManager.Instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			EventProductionData puductionData = EventManager.Instance.GetPuductionData();
			if (systemdata != null && puductionData != null)
			{
				int pictureShowProgress = systemdata.pictureShowProgress;
				for (int i = 0; i < puductionData.startCollectCount.Length; i++)
				{
					int num = puductionData.startCollectCount[i];
					if (EventManager.Instance.CollectCount >= num && i > pictureShowProgress)
					{
						list.Add(i);
					}
				}
			}
		}
		List<int> list2 = new List<int>();
		if (list.Count > 0 && EventManager.Instance != null)
		{
			foreach (int item in list)
			{
				WindowEventData windowEvenData = EventManager.Instance.GetWindowEvenData(item);
				if (windowEvenData != null)
				{
					WindowBodyData[] body = windowEvenData.body;
					foreach (WindowBodyData windowBodyData in body)
					{
						for (int k = 0; k < windowBodyData.face_count; k++)
						{
							if (k < windowBodyData.product.Length && !list2.Contains(windowBodyData.product[k].face_id))
							{
								list2.Add(windowBodyData.product[k].face_id);
							}
						}
					}
				}
			}
			return list2;
		}
		return list2;
	}
}
