public class CannonCamera : GimmickCameraBase
{
	public CannonCamera()
		: base(CameraType.CANNON)
	{
	}

	protected override GimmickCameraParameter GetGimmickCameraParameter(CameraManager manager)
	{
		CameraManager.CannonEditParameter cannonParameter = manager.CannonParameter;
		GimmickCameraParameter result = default(GimmickCameraParameter);
		result.m_waitTime = cannonParameter.m_waitTime;
		result.m_upScrollViewPort = cannonParameter.m_upScrollViewPort;
		result.m_leftScrollViewPort = cannonParameter.m_leftScrollViewPort;
		result.m_depthScrollViewPort = cannonParameter.m_depthScrollViewPort;
		result.m_scrollTime = cannonParameter.m_scrollTime;
		return result;
	}
}
