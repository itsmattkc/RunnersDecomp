namespace Message
{
	public class MsgAssetBundleResponseFailed : MessageBase
	{
		public AssetBundleRequest m_request;

		public AssetBundleResult m_result;

		public MsgAssetBundleResponseFailed(AssetBundleRequest request, AssetBundleResult result)
			: base(61519)
		{
			m_request = request;
			m_result = result;
		}
	}
}
