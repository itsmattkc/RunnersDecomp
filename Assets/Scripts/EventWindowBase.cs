using System.Collections.Generic;
using UnityEngine;

public abstract class EventWindowBase : MonoBehaviour
{
	[SerializeField]
	protected List<GameObject> anchorObjects;

	protected Dictionary<string, UILabel> m_objectLabels;

	protected Dictionary<string, UISprite> m_objectSprites;

	protected Dictionary<string, UITexture> m_objectTextures;

	protected Dictionary<string, GameObject> m_objects;

	protected bool m_isSetObject;

	public bool enabledAnchorObjects
	{
		get
		{
			bool result = false;
			if (anchorObjects != null)
			{
				foreach (GameObject anchorObject in anchorObjects)
				{
					if (anchorObject.activeSelf)
					{
						return true;
					}
				}
				return result;
			}
			return result;
		}
		set
		{
			if (anchorObjects == null)
			{
				return;
			}
			foreach (GameObject anchorObject in anchorObjects)
			{
				anchorObject.SetActive(value);
			}
		}
	}

	protected abstract void SetObject();

	public void AnimationFinishi()
	{
		enabledAnchorObjects = false;
	}
}
