using UnityEngine;

public class ObjBossEventBossMotion : ObjBossMotion
{
	private static readonly BossMotionParam[] MOTION_DATA = new BossMotionParam[5]
	{
		new BossMotionParam("Appear", 6),
		new BossMotionParam("Pass", 6),
		new BossMotionParam("Escape", 6),
		new BossMotionParam("Damage", 4),
		new BossMotionParam("Attack", 3)
	};

	protected override void OnSetup()
	{
	}

	public void SetMotion(EventBossMotion id, bool flag = true)
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
			EventBossMotion motionID = (EventBossMotion)MOTION_DATA[(uint)id].m_motionID;
			if (motionID != EventBossMotion.NONE)
			{
				SetMotion(motionID, false);
			}
		}
	}
}
