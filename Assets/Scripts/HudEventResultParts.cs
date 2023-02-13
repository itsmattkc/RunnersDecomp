using UnityEngine;

public abstract class HudEventResultParts : MonoBehaviour
{
	public abstract void Init(GameObject resultRootObject, long beforeTotalPoint, HudEventResult.AnimationEndCallback callback);

	public abstract void PlayAnimation(HudEventResult.AnimType animType);

	public virtual bool IsBackkeyEnable()
	{
		return true;
	}
}
