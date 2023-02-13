using Message;
using UnityEngine;

namespace Chao
{
	public class StageChao : MonoBehaviour
	{
		private void Start()
		{
			base.enabled = false;
		}

		private void OnMsgExitStage(MsgExitStage msg)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
