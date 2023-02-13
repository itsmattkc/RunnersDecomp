using System.Collections.Generic;
using UnityEngine;

public class HudEventQuota : MonoBehaviour
{
	public delegate void PlayEndCallback();

	private List<QuotaInfo> m_quotaList = new List<QuotaInfo>();

	private GameObject m_rootObject;

	[SerializeField]
	private GameObject m_prefabObject;

	private Animation m_animation;

	private int m_currentAnimIndex;

	private bool m_isPlayEnd = true;

	private PlayEndCallback m_callback;

	public bool IsPlayEnd
	{
		get
		{
			return m_isPlayEnd;
		}
		private set
		{
		}
	}

	public void AddQuota(QuotaInfo info)
	{
		if (m_quotaList != null)
		{
			m_quotaList.Add(info);
		}
	}

	public void ClearQuota()
	{
		if (m_quotaList != null)
		{
			m_quotaList.Clear();
		}
	}

	public void Setup(GameObject rootObject, Animation animation, string swapAnimName1, string swapAnimName2)
	{
		m_rootObject = rootObject;
		m_animation = animation;
		if (m_prefabObject == null)
		{
			m_prefabObject = FindPrefabObject();
		}
		int count = m_quotaList.Count;
		if (count <= 0)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_rootObject, "next_arrow1");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_rootObject, "slot1");
		if (gameObject2 != null)
		{
			GameObject quotaPlate = CopyAttachPrefab(gameObject2, m_prefabObject);
			m_quotaList[0].Setup(quotaPlate, m_animation, string.Empty);
			m_quotaList[0].SetupDisplay();
		}
		if (count < 2)
		{
			return;
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_rootObject, "slot2");
		if (gameObject3 != null)
		{
			GameObject quotaPlate2 = CopyAttachPrefab(gameObject3, m_prefabObject);
			m_quotaList[1].Setup(quotaPlate2, m_animation, swapAnimName1);
		}
		for (int i = 2; i < count; i++)
		{
			GameObject quotaPlate3 = m_quotaList[i - 2].QuotaPlate;
			if (!(quotaPlate3 == null))
			{
				string empty = string.Empty;
				empty = ((i % 2 != 0) ? swapAnimName1 : swapAnimName2);
				m_quotaList[i].Setup(quotaPlate3, m_animation, empty);
			}
		}
	}

	public void PlayStart(PlayEndCallback callback)
	{
		if (m_quotaList.Count <= 0)
		{
			m_isPlayEnd = true;
			callback();
			return;
		}
		m_callback = callback;
		m_isPlayEnd = false;
		m_currentAnimIndex = 0;
		m_quotaList[m_currentAnimIndex].PlayStart();
	}

	public void PlayStop()
	{
		m_isPlayEnd = true;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_isPlayEnd || m_quotaList.Count <= m_currentAnimIndex)
		{
			return;
		}
		QuotaInfo quotaInfo = m_quotaList[m_currentAnimIndex];
		if (quotaInfo == null)
		{
			return;
		}
		quotaInfo.Update();
		if (!quotaInfo.IsPlayEnd())
		{
			return;
		}
		m_currentAnimIndex++;
		if (m_quotaList.Count <= m_currentAnimIndex)
		{
			m_isPlayEnd = true;
			if (m_callback != null)
			{
				m_callback();
			}
		}
		else
		{
			m_quotaList[m_currentAnimIndex].PlayStart();
		}
	}

	private static GameObject FindPrefabObject()
	{
		GameObject result = null;
		GameObject gameObject = GameObject.Find("ResourceManager");
		if (gameObject == null)
		{
			return result;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "EventResourceStage");
		if (gameObject2 == null)
		{
			return result;
		}
		return GameObjectUtil.FindChildGameObject(gameObject2, "ui_event_mission_scroll");
	}

	private static GameObject CopyAttachPrefab(GameObject parentObject, GameObject prefabObject)
	{
		GameObject result = null;
		if (parentObject == null)
		{
			return result;
		}
		if (prefabObject == null)
		{
			return result;
		}
		result = (GameObject)Object.Instantiate(prefabObject);
		Vector3 localPosition = result.transform.localPosition;
		Vector3 localScale = result.transform.localScale;
		result.transform.parent = parentObject.transform;
		result.transform.localPosition = localPosition;
		result.transform.localScale = localScale;
		result.SetActive(true);
		return result;
	}
}
