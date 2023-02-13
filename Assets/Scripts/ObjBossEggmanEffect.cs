using Boss;
using UnityEngine;

public class ObjBossEggmanEffect : ObjBossEffect
{
	public enum BoostType
	{
		Normal,
		Attack
	}

	private const float HITEFFECT_AREA = 0.5f;

	private static Vector3 APPEAR_EFFECT_OFFSET = new Vector3(-1f, 0.5f, 0f);

	private static Vector3 BOOST_EFFECT_ROT = new Vector3(-90f, 0f, 0f);

	private GameObject m_sweat_effect;

	private GameObject m_boost_effectL;

	private GameObject m_boost_effectR;

	private int m_boostType = -1;

	private void OnDestroy()
	{
		PlaySweatEffectEnd();
		DestroyBoostEffect();
	}

	public void PlayHitEffect()
	{
		if (m_hit_offset.y > 0.5f)
		{
			m_hit_offset.y = Mathf.Min(m_hit_offset.y, 0.5f);
		}
		else
		{
			m_hit_offset.y = Mathf.Max(m_hit_offset.y, -0.5f);
		}
		ObjUtil.PlayEffect("ef_bo_em_damage01", base.transform.position + new Vector3(0f, m_hit_offset.y, 0f), Quaternion.identity, 1f);
	}

	public void PlayFoundEffect()
	{
		ObjUtil.PlayEffectChild(base.gameObject, "ef_bo_em_found01", APPEAR_EFFECT_OFFSET, Quaternion.identity, 2f);
	}

	public void PlaySweatEffectStart()
	{
		PlaySweatEffectEnd();
		m_sweat_effect = ObjUtil.PlayEffectChild(base.gameObject, "ef_bo_em_sweat01", Vector3.zero, Quaternion.identity);
	}

	public void PlaySweatEffectEnd()
	{
		if ((bool)m_sweat_effect)
		{
			Object.Destroy(m_sweat_effect);
			m_sweat_effect = null;
		}
	}

	public void PlayEscapeEffect(ObjBossEggmanState context)
	{
		ObjUtil.PlayEffectChild(base.gameObject, "ef_bo_em_blackfog01", Vector3.zero, Quaternion.identity, 5f);
	}

	public void PlayBoostEffect(BoostType type)
	{
		if (m_boostType != (int)type)
		{
			DestroyBoostEffect();
			switch (type)
			{
			case BoostType.Normal:
				PlayBoostEffect("ef_bo_em_vernier_s01");
				break;
			case BoostType.Attack:
				PlayBoostEffect("ef_bo_em_vernier_l01");
				break;
			}
			m_boostType = (int)type;
		}
	}

	public void DestroyBoostEffect()
	{
		if ((bool)m_boost_effectL)
		{
			Object.Destroy(m_boost_effectL);
			m_boost_effectL = null;
		}
		if ((bool)m_boost_effectR)
		{
			Object.Destroy(m_boost_effectR);
			m_boost_effectR = null;
		}
	}

	private void PlayBoostEffect(string effectName)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Booster_L");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Booster_R");
		if ((bool)gameObject && (bool)gameObject2)
		{
			Quaternion local_rot = Quaternion.Euler(BOOST_EFFECT_ROT);
			m_boost_effectL = ObjUtil.PlayEffectChild(gameObject, effectName, Vector3.zero, local_rot, true);
			m_boost_effectR = ObjUtil.PlayEffectChild(gameObject2, effectName, Vector3.zero, local_rot, true);
		}
	}

	protected override void OnPlayChaoEffect()
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (!(instance == null))
		{
			if (instance.HasChaoAbility(ChaoAbility.BOSS_SUPER_RING_RATE))
			{
				PlayChaoEffectSE();
				ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_beam_atk_ht_sr02", Vector3.zero, -1f, false);
				ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_beam_atk_lp_sr02", Vector3.zero, -1f, false);
			}
			if (instance.HasChaoAbility(ChaoAbility.BOSS_RED_RING_RATE))
			{
				PlayChaoEffectSE();
				ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_beam_atk_ht_sr01", Vector3.zero, -1f, false);
				ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_beam_atk_lp_sr01", Vector3.zero, -1f, false);
			}
			if (instance.HasChaoAbility(ChaoAbility.BOSS_STAGE_TIME))
			{
				PlayChaoEffectSE();
				ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_magic_atk_ht_sr01", Vector3.zero, -1f, false);
				ObjUtil.PlayChaoEffect(base.gameObject, "ef_ch_magic_atk_lp_sr01", Vector3.zero, -1f, false);
			}
		}
	}
}
