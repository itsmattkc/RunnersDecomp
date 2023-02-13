using UnityEngine;

public class ObjBossEggmanMotion : ObjBossMotion
{
	private static readonly BossMotionParam[] MOTION_DATA = new BossMotionParam[10]
	{
		new BossMotionParam("Appear", 11),
		new BossMotionParam("BomStart", 11),
		new BossMotionParam("MissileStart", 9),
		new BossMotionParam("MoveR", 11),
		new BossMotionParam("Notice", 11),
		new BossMotionParam("Pass", 11),
		new BossMotionParam("Escape", 11),
		new BossMotionParam("EscapeStart", 11),
		new BossMotionParam("Damage", 2),
		new BossMotionParam("Attack", 2)
	};

	protected override void OnSetup()
	{
	}

	public void SetMotion(BossMotion id, bool flag = true)
	{
		if (m_animator == null)
		{
			m_animator = GetComponentInChildren<Animator>();
		}
		if (!m_animator)
		{
			return;
		}
		if ((long)id >= (long)MOTION_DATA.Length)
		{
			return;
		}
		string flagName = MOTION_DATA[(uint)id].m_flagName;
		if (!(flagName != string.Empty))
		{
			return;
		}
		m_animator.SetBool(flagName, flag);
		if (m_debugDrawMotionInfo)
		{
			Debug.Log("SetMotion " + flagName + " flag=" + flag);
		}
		if (flag)
		{
			BossMotion motionID = (BossMotion)MOTION_DATA[(uint)id].m_motionID;
			if (motionID != BossMotion.NONE)
			{
				SetMotion(motionID, false);
			}
		}
	}
}
