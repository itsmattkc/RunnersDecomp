using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjItemPoint")]
public class ObjItemPoint : SpawnableObject
{
	public int m_debugItemID = -1;

	private int m_tbl_id = -1;

	private bool m_createItemBox;

	protected override void OnSpawned()
	{
	}

	private void Update()
	{
		if (!m_createItemBox)
		{
			ItemType itemType = GetItemType();
			GameObject gameObject = null;
			if (itemType == ItemType.REDSTAR_RING && StageModeManager.Instance != null && StageModeManager.Instance.FirstTutorial)
			{
				itemType = ItemType.TRAMPOLINE;
			}
			switch (itemType)
			{
			case ItemType.REDSTAR_RING:
				gameObject = CreateObjRedStarRing();
				break;
			case ItemType.TIMER_BRONZE:
				gameObject = CreateObjTimerObj(TimerType.BRONZE);
				break;
			case ItemType.TIMER_SILVER:
				gameObject = CreateObjTimerObj(TimerType.SILVER);
				break;
			case ItemType.TIMER_GOLD:
				gameObject = CreateObjTimerObj(TimerType.GOLD);
				break;
			default:
				gameObject = CreateObjItemBox(itemType);
				break;
			}
			m_createItemBox = true;
			if (gameObject == null)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void SetID(int tbl_id)
	{
		if (m_tbl_id == -1)
		{
			m_tbl_id = tbl_id;
		}
	}

	public bool IsCreateItemBox()
	{
		return m_createItemBox;
	}

	private GameObject CreateObjItemBox(ItemType item_type)
	{
		if ((uint)item_type < 8u)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjItemBox");
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(true);
					gameObject2.transform.parent = base.transform;
					SpawnableObject component = gameObject2.GetComponent<SpawnableObject>();
					if ((bool)component)
					{
						component.AttachModelObject();
					}
					ObjItemBox objItemBox = GameObjectUtil.FindChildGameObjectComponent<ObjItemBox>(base.gameObject, gameObject2.name);
					if ((bool)objItemBox)
					{
						objItemBox.CreateItem(item_type);
					}
					return gameObject2;
				}
			}
		}
		return null;
	}

	private GameObject CreateObjRedStarRing()
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjRedStarRing");
		if ((bool)gameObject)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.SetActive(true);
				gameObject2.transform.parent = base.transform;
				SpawnableObject component = gameObject2.GetComponent<SpawnableObject>();
				if ((bool)component)
				{
					component.AttachModelObject();
				}
				SphereCollider component2 = gameObject2.GetComponent<SphereCollider>();
				if ((bool)component2)
				{
					Transform transform = gameObject2.transform;
					Vector3 center = component2.center;
					transform.localPosition = new Vector3(0f, 0f - center.y, 0f);
				}
				return gameObject2;
			}
		}
		return null;
	}

	private GameObject CreateObjTimerObj(TimerType type)
	{
		string objectName = ObjTimerUtil.GetObjectName(type);
		if (!string.IsNullOrEmpty(objectName))
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, objectName);
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(true);
					gameObject2.transform.parent = base.transform;
					SpawnableObject component = gameObject2.GetComponent<SpawnableObject>();
					if ((bool)component)
					{
						component.AttachModelObject();
					}
					ObjTimerBase component2 = gameObject2.GetComponent<ObjTimerBase>();
					if (component2 != null)
					{
						component2.SetMoveType(ObjTimerBase.MoveType.Still);
					}
					SphereCollider component3 = gameObject2.GetComponent<SphereCollider>();
					if ((bool)component3)
					{
						Transform transform = gameObject2.transform;
						Vector3 center = component3.center;
						transform.localPosition = new Vector3(0f, 0f - center.y, 0f);
					}
					return gameObject2;
				}
			}
		}
		return null;
	}

	private ItemType GetItemType()
	{
		if (StageItemManager.Instance != null)
		{
			ItemTable itemTable = StageItemManager.Instance.GetItemTable();
			if (itemTable != null)
			{
				return itemTable.GetItemType(m_tbl_id);
			}
		}
		return ItemType.TRAMPOLINE;
	}
}
