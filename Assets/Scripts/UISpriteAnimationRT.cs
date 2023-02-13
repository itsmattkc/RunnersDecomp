using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite AnimationRT")]
[ExecuteInEditMode]
public class UISpriteAnimationRT : MonoBehaviour
{
	[SerializeField]
	private int m_frameRate = 30;

	[SerializeField]
	private string m_namePrefix = string.Empty;

	[SerializeField]
	private bool m_loop = true;

	private UISprite mSprite;

	private float mDelta;

	private int mIndex;

	private bool mActive = true;

	private List<string> mSpriteNames = new List<string>();

	public int frames
	{
		get
		{
			return mSpriteNames.Count;
		}
	}

	public int framesPerSecond
	{
		get
		{
			return m_frameRate;
		}
		set
		{
			m_frameRate = value;
		}
	}

	public string namePrefix
	{
		get
		{
			return m_namePrefix;
		}
		set
		{
			if (m_namePrefix != value)
			{
				m_namePrefix = value;
				RebuildSpriteList();
			}
		}
	}

	public bool loop
	{
		get
		{
			return m_loop;
		}
		set
		{
			m_loop = value;
		}
	}

	public bool isPlaying
	{
		get
		{
			return mActive;
		}
	}

	private void Start()
	{
		RebuildSpriteList();
	}

	private void Update()
	{
		if (!mActive || mSpriteNames.Count <= 1 || !Application.isPlaying || !((float)m_frameRate > 0f))
		{
			return;
		}
		mDelta += Time.unscaledDeltaTime;
		float num = 1f / (float)m_frameRate;
		if (num < mDelta)
		{
			mDelta = ((!(num > 0f)) ? 0f : (mDelta - num));
			if (++mIndex >= mSpriteNames.Count)
			{
				mIndex = 0;
				mActive = loop;
			}
			if (mActive)
			{
				mSprite.spriteName = mSpriteNames[mIndex];
				mSprite.MakePixelPerfect();
			}
		}
	}

	private void RebuildSpriteList()
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}
		mSpriteNames.Clear();
		if (!(mSprite != null) || !(mSprite.atlas != null))
		{
			return;
		}
		List<UISpriteData> spriteList = mSprite.atlas.spriteList;
		int i = 0;
		for (int count = spriteList.Count; i < count; i++)
		{
			UISpriteData uISpriteData = spriteList[i];
			if (string.IsNullOrEmpty(m_namePrefix) || uISpriteData.name.StartsWith(m_namePrefix))
			{
				mSpriteNames.Add(uISpriteData.name);
			}
		}
		mSpriteNames.Sort();
	}

	public void Reset()
	{
		mActive = true;
		mIndex = 0;
		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
			mSprite.MakePixelPerfect();
		}
	}
}
