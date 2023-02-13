using UnityEngine;

[AddComponentMenu("NGUI/Examples/Drag and Drop Item")]
public class DragDropItem : MonoBehaviour
{
	public GameObject prefab;

	private Transform mTrans;

	private bool mPressed;

	private int mTouchID;

	private bool mIsDragging;

	private bool mSticky;

	private Transform mParent;

	private UIRoot mRoot;

	private void UpdateTable()
	{
		UITable uITable = NGUITools.FindInParents<UITable>(base.gameObject);
		if (uITable != null)
		{
			uITable.repositionNow = true;
		}
	}

	private void Drop()
	{
		Collider collider = UICamera.lastHit.collider;
		DragDropContainer dragDropContainer = (!(collider != null)) ? null : collider.gameObject.GetComponent<DragDropContainer>();
		if (dragDropContainer != null)
		{
			mTrans.parent = dragDropContainer.transform;
			Vector3 localPosition = mTrans.localPosition;
			localPosition.z = 0f;
			mTrans.localPosition = localPosition;
		}
		else
		{
			mTrans.parent = mParent;
		}
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].depth = componentsInChildren[i].depth - 100;
		}
		UpdateTable();
		NGUITools.MarkParentAsChanged(base.gameObject);
	}

	private void Awake()
	{
		mTrans = base.transform;
	}

	private void OnDrag(Vector2 delta)
	{
		if (!mPressed || UICamera.currentTouchID != mTouchID || !base.enabled)
		{
			return;
		}
		if (!mIsDragging)
		{
			mIsDragging = true;
			mParent = mTrans.parent;
			mRoot = NGUITools.FindInParents<UIRoot>(mTrans.gameObject);
			if (DragDropRoot.root != null)
			{
				mTrans.parent = DragDropRoot.root;
			}
			Vector3 localPosition = mTrans.localPosition;
			localPosition.z = 0f;
			mTrans.localPosition = localPosition;
			UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].depth = componentsInChildren[i].depth + 100;
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
		}
		else
		{
			mTrans.localPosition += (Vector3)delta * mRoot.pixelSizeAdjustment;
		}
	}

	private void OnPress(bool isPressed)
	{
		if (!base.enabled)
		{
			return;
		}
		if (isPressed)
		{
			if (mPressed)
			{
				return;
			}
			mPressed = true;
			mTouchID = UICamera.currentTouchID;
			if (!UICamera.current.stickyPress)
			{
				mSticky = true;
				UICamera.current.stickyPress = true;
			}
		}
		else
		{
			mPressed = false;
			if (mSticky)
			{
				mSticky = false;
				UICamera.current.stickyPress = false;
			}
		}
		mIsDragging = false;
		Collider collider = base.collider;
		if (collider != null)
		{
			collider.enabled = !isPressed;
		}
		if (!isPressed)
		{
			Drop();
		}
	}
}
