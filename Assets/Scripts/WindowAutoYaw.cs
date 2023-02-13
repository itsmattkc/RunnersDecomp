using UnityEngine;

[AddComponentMenu("NGUI/Examples/Window Auto-Yaw")]
public class WindowAutoYaw : MonoBehaviour
{
	public int updateOrder;

	public Camera uiCamera;

	public float yawAmount = 20f;

	private Transform mTrans;

	private void OnDisable()
	{
		mTrans.localRotation = Quaternion.identity;
	}

	private void Start()
	{
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		mTrans = base.transform;
		UpdateManager.AddCoroutine(this, updateOrder, CoroutineUpdate);
	}

	private void CoroutineUpdate(float delta)
	{
		if (uiCamera != null)
		{
			Vector3 vector = uiCamera.WorldToViewportPoint(mTrans.position);
			mTrans.localRotation = Quaternion.Euler(0f, (vector.x * 2f - 1f) * yawAmount, 0f);
		}
	}
}
