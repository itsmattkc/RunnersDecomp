using System.Collections.Generic;

namespace Message
{
	public class MsgGetChaoWheelSpinInfoSucceed : MessageBase
	{
		public List<ServerWheelSpinInfo> m_wheelSpinInfos;

		public MsgGetChaoWheelSpinInfoSucceed()
			: base(61466)
		{
		}
	}
}
