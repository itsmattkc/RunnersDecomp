using UnityEngine;

public class SpawnableBehavior : MonoBehaviour
{
	public virtual void SetParameters(SpawnableParameter srcParameter)
	{
	}

	public virtual SpawnableParameter GetParameter()
	{
		return null;
	}

	public virtual SpawnableParameter GetParameterForExport()
	{
		return GetParameter();
	}
}
