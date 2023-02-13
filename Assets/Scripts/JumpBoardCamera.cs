public class JumpBoardCamera : GimmickCameraBase
{
	public JumpBoardCamera()
		: base(CameraType.JUMPBOARD)
	{
	}

	protected override GimmickCameraParameter GetGimmickCameraParameter(CameraManager manager)
	{
		CameraManager.JumpBoardEditParameter jumpBoardParameter = manager.JumpBoardParameter;
		GimmickCameraParameter result = default(GimmickCameraParameter);
		result.m_waitTime = jumpBoardParameter.m_waitTime;
		result.m_upScrollViewPort = jumpBoardParameter.m_upScrollViewPort;
		result.m_leftScrollViewPort = jumpBoardParameter.m_leftScrollViewPort;
		result.m_depthScrollViewPort = jumpBoardParameter.m_depthScrollViewPort;
		result.m_scrollTime = jumpBoardParameter.m_scrollTime;
		return result;
	}
}
