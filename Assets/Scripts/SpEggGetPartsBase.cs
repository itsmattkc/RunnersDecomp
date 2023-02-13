using UnityEngine;

public abstract class SpEggGetPartsBase
{
	protected int m_chaoId = -1;

	public int ChaoId
	{
		get
		{
			return m_chaoId;
		}
	}

	public abstract void Setup(GameObject spEggGetObjectRoot);

	public abstract void PlaySE(string seType);
}
