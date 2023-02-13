using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AtlasListTrace : MonoBehaviour
{
	private class SpriteInfo
	{
		private UISprite m_sprite;

		private UIAtlas m_atlas;

		public UISprite sprite
		{
			get
			{
				return m_sprite;
			}
			private set
			{
			}
		}

		public UIAtlas atlas
		{
			get
			{
				return m_atlas;
			}
			private set
			{
			}
		}

		public SpriteInfo(UISprite sprite, UIAtlas atlas)
		{
			m_sprite = sprite;
			m_atlas = atlas;
		}
	}

	public bool m_showAll;

	private void Start()
	{
		StartCoroutine(ProcessCoroutine());
	}

	private IEnumerator ProcessCoroutine()
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		GameObject rootObject = base.gameObject;
		if (rootObject == null)
		{
			yield break;
		}
		List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();
		SearchSpriteInfoList(rootObject, ref spriteInfoList);
		if (spriteInfoList.Count <= 0)
		{
			yield break;
		}
		StringBuilder log = new StringBuilder();
		log.AppendLine("-----" + rootObject.name + "'s AtlasList-----");
		foreach (SpriteInfo info in spriteInfoList)
		{
			if (info == null)
			{
				continue;
			}
			UISprite sprite = info.sprite;
			if (!(sprite == null))
			{
				UIAtlas atlas = info.atlas;
				if (!(atlas == null))
				{
					log.AppendLine("[" + atlas.name + "] is fount from [" + sprite.name + "]");
				}
			}
		}
		Debug.Log(log.ToString());
	}

	private void Update()
	{
	}

	private void SearchSpriteInfoList(GameObject parentObject, ref List<SpriteInfo> spriteInfoList)
	{
		if (parentObject == null || spriteInfoList == null)
		{
			return;
		}
		int childCount = parentObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = parentObject.transform.GetChild(i).gameObject;
			if (gameObject == null)
			{
				continue;
			}
			SearchSpriteInfoList(gameObject, ref spriteInfoList);
			UISprite uISprite = null;
			UIAtlas uIAtlas = null;
			uISprite = gameObject.GetComponent<UISprite>();
			if (uISprite == null)
			{
				continue;
			}
			uIAtlas = uISprite.atlas;
			if (uIAtlas == null)
			{
				continue;
			}
			bool flag = false;
			foreach (SpriteInfo spriteInfo in spriteInfoList)
			{
				if (spriteInfo != null && spriteInfo.atlas.name == uIAtlas.name)
				{
					flag = true;
				}
			}
			if (!flag || m_showAll)
			{
				SpriteInfo item = new SpriteInfo(uISprite, uIAtlas);
				spriteInfoList.Add(item);
			}
		}
	}
}
