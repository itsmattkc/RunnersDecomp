using UnityEngine;

public class ObjBossEffect : MonoBehaviour
{
	private const string CHAO_SENAME = "act_boss_abnormal";

	protected Vector3 m_hit_offset = Vector3.zero;

	private uint m_chaoSEID;

	private void OnDestroy()
	{
	}

	public void SetHitOffset(Vector3 hit_offset)
	{
		m_hit_offset = hit_offset;
	}

	public void PlayChaoEffect()
	{
		OnPlayChaoEffect();
	}

	protected virtual void OnPlayChaoEffect()
	{
	}

	protected void PlayChaoEffectSE()
	{
		if (m_chaoSEID == 0)
		{
			m_chaoSEID = (uint)ObjUtil.PlaySE("act_boss_abnormal");
		}
	}
}
