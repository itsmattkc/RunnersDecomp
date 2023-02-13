using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Game/Level/SimpleSpawnableObject")]
public class SimpleSpawnableObject : SpawnableBehavior
{
	public SpawnableParameter m_paramter;

	private void Start()
	{
		base.enabled = false;
	}

	public override SpawnableParameter GetParameter()
	{
		return m_paramter;
	}

	public override SpawnableParameter GetParameterForExport()
	{
		return m_paramter;
	}
}
