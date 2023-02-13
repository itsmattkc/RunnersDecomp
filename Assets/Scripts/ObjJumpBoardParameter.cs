using System;

[Serializable]
public class ObjJumpBoardParameter : SpawnableParameter
{
	public float m_succeedFirstSpeed;

	public float m_succeedAngle;

	public float m_succeedOutOfcontrol;

	public float m_missFirstSpeed;

	public float m_missAngle;

	public float m_missOutOfcontrol;

	public ObjJumpBoardParameter()
		: base("ObjJumpBoard")
	{
		m_succeedFirstSpeed = 20f;
		m_succeedAngle = 45f;
		m_succeedOutOfcontrol = 0.2f;
		m_missFirstSpeed = 10f;
		m_missAngle = 30f;
		m_missOutOfcontrol = 0.2f;
	}
}
