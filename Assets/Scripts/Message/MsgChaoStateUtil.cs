using UnityEngine;

namespace Message
{
	public class MsgChaoStateUtil
	{
		public static void SendMsgChaoState(MsgChaoState.State state)
		{
			MsgChaoState msgChaoState = new MsgChaoState(state);
			if (msgChaoState != null)
			{
				GameObjectUtil.SendMessageToTagObjects("Chao", "OnMsgReceive", msgChaoState, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
