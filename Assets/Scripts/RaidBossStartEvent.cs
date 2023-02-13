using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RaidBossStartEvent : MonoBehaviour
{
	public enum ProductType
	{
		MileagePage,
		EventTopPage
	}

	private enum Mode
	{
		Idle,
		CheckId,
		WaitCommonResource,
		Load,
		WaitLoad,
		Start,
		WaitEnd,
		Next,
		End
	}

	private const string WINDOW_NAME = "RaidBossStartEvent";

	private Mode m_mode;

	private ResourceSceneLoader m_sceneLoader;

	private int m_productProgress = -1;

	private int m_textWindowId = -1;

	private List<string> m_loadTexList = new List<string>();

	private WindowEventData m_windowEventData;

	private ProductType m_productType;

	private bool m_isNotPlaybackDefaultBgm;

	private bool m_isEnd;

	private bool m_alertFlag;

	private bool m_firstBattle;

	private bool m_commonResourceLoaded;

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
	}

	public static RaidBossStartEvent Create(GameObject obj, ProductType type)
	{
		if (obj != null)
		{
			RaidBossStartEvent raidBossStartEvent = obj.GetComponent<RaidBossStartEvent>();
			if (raidBossStartEvent == null)
			{
				raidBossStartEvent = obj.AddComponent<RaidBossStartEvent>();
			}
			else if (GeneralWindow.IsCreated("RaidBossStartEvent"))
			{
				GeneralWindow.Close();
			}
			if (raidBossStartEvent != null)
			{
				raidBossStartEvent.ResetParam(type);
			}
			return raidBossStartEvent;
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
			info.name = "RaidBossStartEvent";
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

	private int GetTextWindowId()
	{
		if (EventManager.Instance != null)
		{
			EventRaidProductionData raidProductionData = EventManager.Instance.GetRaidProductionData();
			if (raidProductionData != null)
			{
				if (m_productType == ProductType.MileagePage)
				{
					EventProductionData mileagePage = raidProductionData.mileagePage;
					if (mileagePage != null && m_productProgress < mileagePage.textWindowId.Length)
					{
						return mileagePage.textWindowId[m_productProgress];
					}
				}
				else if (m_firstBattle)
				{
					EventProductionData firstBattle = raidProductionData.firstBattle;
					if (firstBattle != null && 0 < firstBattle.textWindowId.Length)
					{
						return firstBattle.textWindowId[0];
					}
				}
				else
				{
					EventProductionData eventTop = raidProductionData.eventTop;
					if (eventTop != null && m_productProgress < eventTop.textWindowId.Length)
					{
						return eventTop.textWindowId[m_productProgress];
					}
				}
			}
		}
		return -1;
	}

	public void SaveEventTopPagePictureEvent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.pictureShowEventId = EventManager.Instance.Id;
				systemdata.pictureShowProgress = m_productProgress;
			}
			instance.SaveSystemData();
		}
	}

	public void SaveMileagePagePictureEvent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				systemdata.pictureShowEventId = EventManager.Instance.Id;
				systemdata.pictureShowEmergeRaidBossProgress = m_productProgress;
			}
			instance.SaveSystemData();
		}
	}

	public static bool IsTopMenuProduction()
	{
		if (EventManager.Instance != null && (bool)SystemSaveManager.Instance)
		{
			ServerEventUserRaidBossState raidBossState = EventManager.Instance.RaidBossState;
			SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
			if (systemdata == null || raidBossState == null)
			{
				return false;
			}
			EventRaidProductionData raidProductionData = EventManager.Instance.GetRaidProductionData();
			if (raidProductionData != null)
			{
				EventProductionData mileagePage = raidProductionData.mileagePage;
				if (mileagePage != null)
				{
					int pictureShowEmergeRaidBossProgress = systemdata.pictureShowEmergeRaidBossProgress;
					int numRaidBossEncountered = raidBossState.NumRaidBossEncountered;
					int numBeatedEncounter = raidBossState.NumBeatedEncounter;
					for (int i = 0; i < mileagePage.startCollectCount.Length; i++)
					{
						int num = mileagePage.startCollectCount[i];
						if (numRaidBossEncountered >= num && numBeatedEncounter >= num - 1 && i > pictureShowEmergeRaidBossProgress)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private int GetNextWindowProductIndex(ProductType type, ref bool firstBattle)
	{
		int result = -1;
		if (EventManager.Instance != null && !EventManager.Instance.IsChallengeEvent())
		{
			return result;
		}
		if (SystemSaveManager.Instance == null || EventManager.Instance == null)
		{
			return result;
		}
		SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
		EventRaidProductionData raidProductionData = EventManager.Instance.GetRaidProductionData();
		ServerEventUserRaidBossState raidBossState = EventManager.Instance.RaidBossState;
		if (systemdata == null || raidProductionData == null || raidBossState == null)
		{
			return result;
		}
		List<ServerEventRaidBossState> userRaidBossList = EventManager.Instance.UserRaidBossList;
		if (userRaidBossList == null)
		{
			return result;
		}
		int numRaidBossEncountered = raidBossState.NumRaidBossEncountered;
		int numBeatedEncounter = raidBossState.NumBeatedEncounter;
		switch (type)
		{
		case ProductType.MileagePage:
		{
			EventProductionData mileagePage = raidProductionData.mileagePage;
			if (mileagePage == null)
			{
				break;
			}
			int pictureShowEmergeRaidBossProgress = systemdata.pictureShowEmergeRaidBossProgress;
			for (int j = 0; j < mileagePage.startCollectCount.Length; j++)
			{
				int num2 = mileagePage.startCollectCount[j];
				if (numRaidBossEncountered >= num2 && numBeatedEncounter >= num2 - 1 && j > pictureShowEmergeRaidBossProgress)
				{
					return j;
				}
			}
			break;
		}
		case ProductType.EventTopPage:
		{
			if (systemdata.pictureShowRaidBossFirstBattle == 1)
			{
				firstBattle = true;
				return 0;
			}
			EventProductionData eventTop = raidProductionData.eventTop;
			if (eventTop == null)
			{
				break;
			}
			int pictureShowProgress = systemdata.pictureShowProgress;
			for (int i = 0; i < eventTop.startCollectCount.Length; i++)
			{
				int num = eventTop.startCollectCount[i];
				if (numRaidBossEncountered >= num && numBeatedEncounter >= num - 1 && i > pictureShowProgress)
				{
					return i;
				}
			}
			break;
		}
		}
		return result;
	}

	public void SavePictureEvent()
	{
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		SystemData systemdata = instance.GetSystemdata();
		if (systemdata != null)
		{
			switch (m_productType)
			{
			case ProductType.MileagePage:
				systemdata.pictureShowEventId = EventManager.Instance.Id;
				systemdata.pictureShowEmergeRaidBossProgress = m_productProgress;
				break;
			case ProductType.EventTopPage:
				if (m_firstBattle)
				{
					systemdata.pictureShowEventId = EventManager.Instance.Id;
					systemdata.pictureShowRaidBossFirstBattle = 0;
				}
				else
				{
					systemdata.pictureShowEventId = EventManager.Instance.Id;
					systemdata.pictureShowProgress = m_productProgress;
				}
				break;
			}
		}
		instance.SaveSystemData();
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
			m_productProgress = GetNextWindowProductIndex(m_productType, ref m_firstBattle);
			m_mode = Mode.CheckId;
			break;
		case Mode.CheckId:
			if (m_productProgress == -1)
			{
				m_mode = Mode.End;
				break;
			}
			m_textWindowId = GetTextWindowId();
			if (!m_commonResourceLoaded)
			{
				GameObjectUtil.SendMessageFindGameObject("MainMenuButtonEvent", "OnMenuEventClicked", base.gameObject, SendMessageOptions.DontRequireReceiver);
			}
			m_mode = Mode.WaitCommonResource;
			break;
		case Mode.WaitCommonResource:
			if (m_commonResourceLoaded)
			{
				if (LoadTexture())
				{
					m_mode = Mode.WaitLoad;
				}
				else
				{
					m_mode = Mode.Start;
				}
			}
			break;
		case Mode.WaitLoad:
			if (m_sceneLoader != null && m_sceneLoader.Loaded)
			{
				Object.Destroy(m_sceneLoader.gameObject);
				m_mode = Mode.Start;
			}
			break;
		case Mode.Start:
			if (!RaidBossWindow.IsOpenAdvent())
			{
				if (StartEvent())
				{
					m_mode = Mode.WaitEnd;
				}
				else
				{
					m_mode = Mode.End;
				}
			}
			break;
		case Mode.WaitEnd:
			if (GeneralWindow.IsCreated("RaidBossStartEvent") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				DestroyTextureData();
				SavePictureEvent();
				m_firstBattle = false;
				m_productProgress = GetNextWindowProductIndex(m_productType, ref m_firstBattle);
				m_mode = Mode.CheckId;
			}
			break;
		case Mode.Next:
			m_mode = Mode.Load;
			break;
		case Mode.End:
			m_isEnd = true;
			m_mode = Mode.Idle;
			break;
		}
	}

	private void ResetParam(ProductType type)
	{
		m_mode = Mode.Load;
		m_productType = type;
		SetColision(true);
		if (type == ProductType.EventTopPage)
		{
			m_commonResourceLoaded = true;
		}
		m_isEnd = false;
		base.enabled = true;
	}

	private void SetColision(bool flag)
	{
		if (flag)
		{
			if (!m_alertFlag)
			{
				HudMenuUtility.SetConnectAlertMenuButtonUI(true);
			}
		}
		else if (m_alertFlag)
		{
			HudMenuUtility.SetConnectAlertMenuButtonUI(false);
		}
		m_alertFlag = flag;
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (!m_isEnd && msg != null)
		{
			msg.StaySequence();
		}
	}

	public void OnButtonEventCallBack()
	{
		m_commonResourceLoaded = true;
	}

	public static List<int> GetProductFaceIdList(bool mileageProduct)
	{
		if (SystemSaveManager.Instance == null || EventManager.Instance == null)
		{
			return null;
		}
		SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
		EventRaidProductionData raidProductionData = EventManager.Instance.GetRaidProductionData();
		ServerEventUserRaidBossState raidBossState = EventManager.Instance.RaidBossState;
		if (systemdata == null || raidProductionData == null || raidBossState == null)
		{
			return null;
		}
		List<ServerEventRaidBossState> userRaidBossList = EventManager.Instance.UserRaidBossList;
		if (userRaidBossList == null)
		{
			return null;
		}
		List<int> list = new List<int>();
		int numRaidBossEncountered = raidBossState.NumRaidBossEncountered;
		int numBeatedEncounter = raidBossState.NumBeatedEncounter;
		if (mileageProduct)
		{
			EventProductionData mileagePage = raidProductionData.mileagePage;
			if (mileagePage != null)
			{
				int pictureShowEmergeRaidBossProgress = systemdata.pictureShowEmergeRaidBossProgress;
				for (int i = 0; i < mileagePage.startCollectCount.Length; i++)
				{
					int num = mileagePage.startCollectCount[i];
					if (numRaidBossEncountered >= num && numBeatedEncounter >= num - 1 && i > pictureShowEmergeRaidBossProgress)
					{
						list.Add(mileagePage.textWindowId[i]);
					}
				}
			}
		}
		EventProductionData firstBattle = raidProductionData.firstBattle;
		if (firstBattle != null && systemdata.pictureShowRaidBossFirstBattle == 1)
		{
			list.Add(firstBattle.textWindowId[0]);
		}
		EventProductionData eventTop = raidProductionData.eventTop;
		int pictureShowProgress = systemdata.pictureShowProgress;
		for (int j = 0; j < eventTop.startCollectCount.Length; j++)
		{
			int num2 = eventTop.startCollectCount[j];
			if (numRaidBossEncountered >= num2 && numBeatedEncounter >= num2 - 1 && j > pictureShowProgress)
			{
				list.Add(eventTop.textWindowId[j]);
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
						for (int l = 0; l < windowBodyData.face_count; l++)
						{
							if (l < windowBodyData.product.Length && !list2.Contains(windowBodyData.product[l].face_id))
							{
								list2.Add(windowBodyData.product[l].face_id);
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
