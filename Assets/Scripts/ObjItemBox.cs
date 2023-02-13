using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjItemBox")]
public class ObjItemBox : SpawnableObject
{
	private const string ModelName = "obj_cmn_itembox";

	private uint m_item_type;

	private GameObject m_item_obj;

	private bool m_end;

	protected override string GetModelName()
	{
		return "obj_cmn_itembox";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
	}

	public void CreateItem(ItemType item_type)
	{
		m_item_type = (uint)item_type;
		if (m_item_type < 8)
		{
			string itemFileName = ItemTypeName.GetItemFileName((ItemType)m_item_type);
			if (itemFileName.Length > 0)
			{
				m_item_obj = AttachObject(ResourceCategory.OBJECT_RESOURCE, itemFileName, Vector3.zero, Quaternion.Euler(Vector3.zero));
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_end && (bool)other)
		{
			if (StageItemManager.Instance != null)
			{
				StageItemManager.Instance.OnAddItem(new MsgAddItemToManager((ItemType)m_item_type));
			}
			TakeItemBox();
		}
	}

	private void TakeItemBox()
	{
		m_end = true;
		ObjUtil.PlayEffectCollisionCenter(base.gameObject, "ef_com_itembox_open01", 1f);
		ObjUtil.PlaySE("obj_itembox");
		ObjUtil.SendGetItemIcon((ItemType)m_item_type);
		if ((bool)m_item_obj)
		{
			Object.Destroy(m_item_obj);
		}
		Object.Destroy(base.gameObject);
	}
}
