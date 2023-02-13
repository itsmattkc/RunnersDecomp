using UnityEngine;

public class ObjLoopTerrainSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjLoopTerrainParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjLoopTerrainParameter objLoopTerrainParameter = srcParameter as ObjLoopTerrainParameter;
		if (objLoopTerrainParameter == null)
		{
			return;
		}
		m_parameter = objLoopTerrainParameter;
		ObjLoopTerrain component = GetComponent<ObjLoopTerrain>();
		if (component != null)
		{
			if (objLoopTerrainParameter.m_pathName.Length > 0)
			{
				component.SetPathName(objLoopTerrainParameter.m_pathName);
			}
			component.SetZOffset(objLoopTerrainParameter.m_pathZOffset);
		}
		BoxCollider component2 = GetComponent<BoxCollider>();
		if ((bool)component2)
		{
			component2.size = objLoopTerrainParameter.Size;
			component2.center = objLoopTerrainParameter.Center;
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}

	public override SpawnableParameter GetParameterForExport()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			m_parameter.Size = component.size;
			m_parameter.Center = component.center;
		}
		return m_parameter;
	}
}
