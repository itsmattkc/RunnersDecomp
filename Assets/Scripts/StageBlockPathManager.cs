using Message;
using System.Collections.Generic;
using UnityEngine;

public class StageBlockPathManager : MonoBehaviour
{
	private string m_pathStageName;

	private PathManager m_pathManager;

	private List<GameObject> m_blockPathController;

	private void Start()
	{
	}

	private void OnDestroy()
	{
		if (m_blockPathController == null)
		{
			return;
		}
		foreach (GameObject item in m_blockPathController)
		{
			Object.Destroy(item);
		}
		m_blockPathController = null;
	}

	public void SetPathManager(PathManager manager)
	{
		m_pathManager = manager;
	}

	public void Setup()
	{
		m_blockPathController = new List<GameObject>();
	}

	public BlockPathController GetCurrentController()
	{
		foreach (GameObject item in m_blockPathController)
		{
			BlockPathController component = item.GetComponent<BlockPathController>();
			if ((bool)component && component.IsNowCurrent())
			{
				return component;
			}
		}
		return null;
	}

	public PathComponent GetCurentSVPath(ref float? distance)
	{
		return GetCurentPath(BlockPathController.PathType.SV, ref distance);
	}

	public PathComponent GetCurentDrillPath(ref float? distance)
	{
		return GetCurentPath(BlockPathController.PathType.DRILL, ref distance);
	}

	public PathComponent GetCurentLaserPath(ref float? distance)
	{
		return GetCurentPath(BlockPathController.PathType.LASER, ref distance);
	}

	private void ActivateBlock(int block, int blockActivateID, Vector3 originPoint)
	{
		GameObject gameObject = new GameObject("BlockPathController");
		gameObject.transform.parent = base.transform;
		BlockPathController blockPathController = gameObject.AddComponent<BlockPathController>();
		if (m_pathManager != null)
		{
			string stageName = StageTypeUtil.GetStageName(m_pathManager.GetSVPathName());
			blockPathController.Initialize(stageName, block, blockActivateID, m_pathManager, originPoint);
		}
		if (m_blockPathController != null)
		{
			m_blockPathController.Add(gameObject);
		}
	}

	private void DeactivateBlock(int activateId)
	{
		BlockPathController controllerByActivateID = GetControllerByActivateID(activateId);
		if (controllerByActivateID != null)
		{
			m_blockPathController.Remove(controllerByActivateID.gameObject);
			Object.Destroy(controllerByActivateID.gameObject);
		}
	}

	private BlockPathController GetControllerByActivateID(int activateID)
	{
		foreach (GameObject item in m_blockPathController)
		{
			BlockPathController component = item.GetComponent<BlockPathController>();
			if ((bool)component && component.ActivateID == activateID)
			{
				return component;
			}
		}
		return null;
	}

	public PathComponent GetCurentPath(BlockPathController.PathType pathType, ref float? distance)
	{
		BlockPathController currentController = GetCurrentController();
		if (currentController == null)
		{
			return null;
		}
		PathEvaluator evaluator = currentController.GetEvaluator(pathType);
		if (evaluator != null && evaluator.IsValid())
		{
			if (distance.HasValue)
			{
				distance = evaluator.Distance;
			}
			return evaluator.GetPathObject();
		}
		return null;
	}

	public PathEvaluator GetCurentPathEvaluator(BlockPathController.PathType pathType)
	{
		BlockPathController currentController = GetCurrentController();
		if (currentController == null)
		{
			return null;
		}
		PathEvaluator evaluator = currentController.GetEvaluator(pathType);
		if (evaluator != null && evaluator.IsValid())
		{
			return evaluator;
		}
		return null;
	}

	public void OnActivateBlock(MsgActivateBlock msg)
	{
		if (msg != null)
		{
			ActivateBlock(msg.m_blockNo, msg.m_activateID, msg.m_originPosition);
		}
	}

	private void OnDeactivateBlock(MsgDeactivateBlock msg)
	{
		if (msg != null)
		{
			DeactivateBlock(msg.m_activateID);
		}
	}

	private void OnDeactivateAllBlock(MsgDeactivateAllBlock msg)
	{
		foreach (GameObject item in m_blockPathController)
		{
			Object.Destroy(item);
		}
		m_blockPathController.Clear();
	}

	private void OnChangeCurerntBlock(MsgChangeCurrentBlock msg)
	{
		if (msg != null)
		{
			BlockPathController currentController = GetCurrentController();
			BlockPathController controllerByActivateID = GetControllerByActivateID(msg.m_activateID);
			if (currentController != null)
			{
				currentController.SetCurrent(false);
			}
			if (controllerByActivateID != null)
			{
				controllerByActivateID.SetCurrent(true);
			}
		}
	}
}
