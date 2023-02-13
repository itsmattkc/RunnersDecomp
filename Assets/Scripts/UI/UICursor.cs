using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(UISprite))]
	public class UICursor : MonoBehaviour
	{
		private static UICursor mInstance;

		public Camera uiCamera;

		private Transform mTrans;

		private UISprite mSprite;

		private UIAtlas mAtlas;

		private string mSpriteName;

		private void Awake()
		{
			mInstance = this;
		}

		private void OnDestroy()
		{
			mInstance = null;
		}

		private void Start()
		{
			mTrans = base.transform;
			mSprite = GetComponentInChildren<UISprite>();
			mAtlas = mSprite.atlas;
			mSpriteName = mSprite.spriteName;
			mSprite.depth = 100;
			if (uiCamera == null)
			{
				uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
		}

		private void Update()
		{
			if (!(mSprite.atlas != null))
			{
				return;
			}
			Vector3 mousePosition = Input.mousePosition;
			if (uiCamera != null)
			{
				mousePosition.x = Mathf.Clamp01(mousePosition.x / (float)Screen.width);
				mousePosition.y = Mathf.Clamp01(mousePosition.y / (float)Screen.height);
				mTrans.position = uiCamera.ViewportToWorldPoint(mousePosition);
				if (uiCamera.isOrthoGraphic)
				{
					Vector3 scale = new Vector3(mSprite.width, mSprite.height, 1f);
					mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mTrans.localPosition, scale);
				}
			}
			else
			{
				mousePosition.x -= (float)Screen.width * 0.5f;
				mousePosition.y -= (float)Screen.height * 0.5f;
				Vector3 scale2 = new Vector3(mSprite.width, mSprite.height, 1f);
				mTrans.localPosition = NGUIMath.ApplyHalfPixelOffset(mousePosition, scale2);
			}
		}

		public static void Clear()
		{
			Set(mInstance.mAtlas, mInstance.mSpriteName);
		}

		public static void Set(UIAtlas atlas, string sprite)
		{
			if (mInstance != null)
			{
				mInstance.mSprite.atlas = atlas;
				mInstance.mSprite.spriteName = sprite;
				mInstance.mSprite.MakePixelPerfect();
				mInstance.Update();
			}
		}
	}
}
