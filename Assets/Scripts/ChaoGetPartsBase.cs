using UnityEngine;

public abstract class ChaoGetPartsBase : MonoBehaviour
{
	public enum AnimType
	{
		NONE = -1,
		GET_ANIM_CONTINUE,
		GET_ANIM_FINISH,
		OUT_ANIM,
		NUM
	}

	public enum BtnType
	{
		NONE = -1,
		OK,
		NEXT,
		EQUIP_OK,
		NUM
	}

	public delegate void AnimationEndCallback(AnimType animType);

	protected AnimationEndCallback m_callback;

	protected int m_chaoId = -1;

	public int ChaoId
	{
		get
		{
			return m_chaoId;
		}
	}

	public void SetCallback(AnimationEndCallback callback)
	{
		m_callback = callback;
	}

	public abstract void Setup(GameObject chaoGetObjectRoot);

	public abstract void PlayGetAnimation(Animation anim);

	public abstract BtnType GetButtonType();

	public abstract void PlayEndAnimation(Animation anim);

	public abstract void PlaySE(string seType);

	public abstract EasySnsFeed CreateEasySnsFeed(GameObject rootObject);
}
