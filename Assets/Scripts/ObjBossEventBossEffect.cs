using Boss;
using UnityEngine;

public class ObjBossEventBossEffect : ObjBossEffect
{
	private const float HITEFFECT_AREA = 0.5f;

	private static readonly string[] BOOST_EFFECT_NAME = new string[3]
	{
		"ef_raid_speedup_lv1_fog01",
		"ef_raid_speedup_lv2_fog01",
		"ef_raid_speedup_lv3_fog01"
	};

	private void OnDestroy()
	{
	}

	public static string GetBoostEffect(WispBoostLevel level)
	{
		string result = string.Empty;
		switch (level)
		{
		case WispBoostLevel.LEVEL1:
		case WispBoostLevel.LEVEL2:
		case WispBoostLevel.LEVEL3:
			result = BOOST_EFFECT_NAME[(int)level];
			break;
		}
		return result;
	}

	public void PlayHitEffect(WispBoostLevel level)
	{
		if (m_hit_offset.y > 0.5f)
		{
			m_hit_offset.y = Mathf.Min(m_hit_offset.y, 0.5f);
		}
		else
		{
			m_hit_offset.y = Mathf.Max(m_hit_offset.y, -0.5f);
		}
		Vector3 pos = base.transform.position + new Vector3(0f, m_hit_offset.y, 0f);
		string text = string.Empty;
		switch (level)
		{
		case WispBoostLevel.LEVEL1:
			text = "ef_raid_speedup_lv1_hit01";
			break;
		case WispBoostLevel.LEVEL2:
			text = "ef_raid_speedup_lv2_hit01";
			break;
		case WispBoostLevel.LEVEL3:
			text = "ef_raid_speedup_lv3_hit01";
			break;
		}
		if (text == string.Empty)
		{
			ObjUtil.PlayEffect("ef_bo_em_damage01", pos, Quaternion.identity, 1f);
		}
		else
		{
			ObjUtil.PlayEffect(text, pos, Quaternion.identity, 1f);
		}
	}

	public void PlayEscapeEffect(ObjBossEventBossState context)
	{
		ObjUtil.PlayEffectChild(base.gameObject, EventBossObjectTable.GetItemData(EventBossObjectTableItem.EscapeEffect), Vector3.zero, Quaternion.identity, 5f);
	}

	protected override void OnPlayChaoEffect()
	{
	}
}
