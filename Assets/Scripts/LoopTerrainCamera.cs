public class LoopTerrainCamera : GimmickCameraBase
{
	public LoopTerrainCamera()
		: base(CameraType.LOOP_TERRAIN)
	{
	}

	protected override GimmickCameraParameter GetGimmickCameraParameter(CameraManager manager)
	{
		CameraManager.LoopTerrainEditParameter loopTerrainParameter = manager.LoopTerrainParameter;
		GimmickCameraParameter result = default(GimmickCameraParameter);
		result.m_waitTime = loopTerrainParameter.m_waitTime;
		result.m_upScrollViewPort = loopTerrainParameter.m_upScrollViewPort;
		result.m_leftScrollViewPort = loopTerrainParameter.m_leftScrollViewPort;
		result.m_depthScrollViewPort = loopTerrainParameter.m_depthScrollViewPort;
		result.m_scrollTime = loopTerrainParameter.m_scrollTime;
		return result;
	}

	public override void Update(CameraManager manager, float deltaTime)
	{
		if (m_mode != 0)
		{
			base.Update(manager, deltaTime);
		}
	}
}
