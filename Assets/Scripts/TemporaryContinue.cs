using Message;
using UnityEngine;

public class TemporaryContinue : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		GUI.TextArea(new Rect(400f, 100f, 200f, 50f), "Continue?");
		if (GUI.Button(new Rect(200f, 300f, 200f, 100f), "Yes"))
		{
			MsgContinueResult value = new MsgContinueResult(true);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value, SendMessageOptions.DontRequireReceiver);
			Object.Destroy(base.gameObject);
		}
		if (GUI.Button(new Rect(600f, 300f, 200f, 100f), "No"))
		{
			MsgContinueResult value2 = new MsgContinueResult(false);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value2, SendMessageOptions.DontRequireReceiver);
			Object.Destroy(base.gameObject);
		}
	}
}
