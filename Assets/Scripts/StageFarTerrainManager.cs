using Message;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Game/Level")]
public class StageFarTerrainManager : MonoBehaviour
{
	private const float nextOffset = 1000f;

	private const float drawOffset = 1500f;

	private const float destroyOffset = 1700f;

	private const string defaultModelName = "_far";

	public GameObject m_originalFarModel;

	private List<GameObject> m_nowDrawingModel;

	private PlayerInformation m_playerInfo;

	private int m_nowSpawnedNumModels;

	private float m_nextSpawnOffset;

	private static readonly Vector3 originalPosition = new Vector3(-1500f, 0f, 7920f) * 0.1f;

	private static readonly Vector3 originalRotation = new Vector3(0f, 0f, 0f);

	private void Start()
	{
		m_playerInfo = GameObject.Find("PlayerInformation").GetComponent<PlayerInformation>();
		m_nowDrawingModel = new List<GameObject>();
	}

	public void SetupModel(string stageName)
	{
		string text = stageName + "_far";
		if (TenseEffectManager.Instance != null)
		{
			TenseEffectManager.Type tenseType = TenseEffectManager.Instance.GetTenseType();
			text += ((tenseType != 0) ? "B" : "A");
		}
		m_nowSpawnedNumModels = 0;
		m_originalFarModel = ResourceManager.Instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, text);
		InstantiateModel(originalPosition, Quaternion.Euler(originalRotation));
		Vector3 position = originalPosition;
		position.x += 1500f;
		InstantiateModel(position, Quaternion.Euler(originalRotation));
		Vector3 vector = originalPosition;
		m_nextSpawnOffset = vector.x + 1000f;
	}

	private void Update()
	{
		if (!m_playerInfo || !m_originalFarModel || m_nowSpawnedNumModels <= 0)
		{
			return;
		}
		Vector3 position = m_playerInfo.Position;
		if (position.x > m_nextSpawnOffset)
		{
			Vector3 position2 = originalPosition;
			position2.x += 1500f * (float)m_nowSpawnedNumModels;
			InstantiateModel(position2, Quaternion.Euler(originalRotation));
			m_nextSpawnOffset += 1000f;
		}
		for (int num = m_nowDrawingModel.Count - 1; num >= 0; num--)
		{
			float x = position.x;
			Vector3 position3 = m_nowDrawingModel[num].transform.position;
			if (x - position3.x > 1700f)
			{
				Object.Destroy(m_nowDrawingModel[num]);
				m_nowDrawingModel.Remove(m_nowDrawingModel[num]);
			}
		}
	}

	private void InstantiateModel(Vector3 position, Quaternion rotation)
	{
		if (!(m_originalFarModel == null))
		{
			GameObject gameObject = Object.Instantiate(m_originalFarModel, position, rotation) as GameObject;
			if ((bool)gameObject)
			{
				gameObject.isStatic = true;
				gameObject.SetActive(true);
				m_nowDrawingModel.Add(gameObject);
				m_nowSpawnedNumModels++;
			}
		}
	}

	private void OnMsgStageReplace(MsgStageReplace msg)
	{
		for (int num = m_nowDrawingModel.Count - 1; num >= 0; num--)
		{
			Object.Destroy(m_nowDrawingModel[num]);
		}
		m_nowDrawingModel.Clear();
		SetupModel(msg.m_stageName);
	}
}
