using UnityEngine;

public class StageEffect : MonoBehaviour
{
	private GameObject m_stageEffect;

	private GameObject m_cameraObject;

	private bool m_resetPos;

	private void OnDestroy()
	{
		if (m_stageEffect != null)
		{
			Object.Destroy(m_stageEffect);
			m_stageEffect = null;
		}
	}

	private void Update()
	{
		if (m_stageEffect != null && m_cameraObject != null)
		{
			if (m_resetPos)
			{
				base.transform.localPosition = -m_cameraObject.transform.position;
				return;
			}
			Transform transform = base.transform;
			Vector3 position = m_cameraObject.transform.position;
			transform.localPosition = new Vector3(0f, 0f, 0f - position.z);
		}
	}

	public void Setup(GameObject originalObj)
	{
		if (m_stageEffect == null && originalObj != null)
		{
			GameObject gameObject = Object.Instantiate(originalObj, Vector3.zero, Quaternion.identity) as GameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.localPosition = originalObj.transform.localPosition;
				gameObject.transform.localRotation = originalObj.transform.localRotation;
				m_stageEffect = gameObject;
			}
		}
		if (m_cameraObject == null)
		{
			m_cameraObject = base.transform.parent.gameObject;
		}
	}

	public void ResetPos(bool reset)
	{
		m_resetPos = reset;
	}

	public static StageEffect CreateStageEffect(string stageName)
	{
		StageEffect stageEffect = null;
		if (ResourceManager.Instance != null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, "ef_stage_" + stageName);
			if (gameObject != null)
			{
				GameObject gameObject2 = GameObject.FindGameObjectWithTag("MainCamera");
				if (gameObject2 != null)
				{
					GameObject gameObject3 = new GameObject("StageEffect");
					if (gameObject3 != null)
					{
						gameObject3.SetActive(true);
						gameObject3.transform.parent = gameObject2.transform;
						gameObject3.transform.localPosition = Vector3.zero;
						gameObject3.transform.localRotation = Quaternion.identity;
						stageEffect = gameObject3.AddComponent<StageEffect>();
						if (stageEffect != null)
						{
							stageEffect.Setup(gameObject);
						}
					}
				}
			}
		}
		return stageEffect;
	}
}
