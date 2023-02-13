using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Examples/Play Idle Animations")]
public class PlayIdleAnimations : MonoBehaviour
{
	private Animation mAnim;

	private AnimationClip mIdle;

	private List<AnimationClip> mBreaks = new List<AnimationClip>();

	private float mNextBreak;

	private int mLastIndex;

	private void Start()
	{
		mAnim = GetComponentInChildren<Animation>();
		if (mAnim == null)
		{
			Debug.LogWarning(NGUITools.GetHierarchy(base.gameObject) + " has no Animation component");
			Object.Destroy(this);
			return;
		}
		foreach (AnimationState item in mAnim)
		{
			if (item.clip.name == "idle")
			{
				item.layer = 0;
				mIdle = item.clip;
				mAnim.Play(mIdle.name);
			}
			else if (item.clip.name.StartsWith("idle"))
			{
				item.layer = 1;
				mBreaks.Add(item.clip);
			}
		}
		if (mBreaks.Count == 0)
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (!(mNextBreak < Time.time))
		{
			return;
		}
		if (mBreaks.Count == 1)
		{
			AnimationClip animationClip = mBreaks[0];
			mNextBreak = Time.time + animationClip.length + Random.Range(5f, 15f);
			mAnim.CrossFade(animationClip.name);
			return;
		}
		int num = Random.Range(0, mBreaks.Count - 1);
		if (mLastIndex == num)
		{
			num++;
			if (num >= mBreaks.Count)
			{
				num = 0;
			}
		}
		mLastIndex = num;
		AnimationClip animationClip2 = mBreaks[num];
		mNextBreak = Time.time + animationClip2.length + Random.Range(2f, 8f);
		mAnim.CrossFade(animationClip2.name);
	}
}
