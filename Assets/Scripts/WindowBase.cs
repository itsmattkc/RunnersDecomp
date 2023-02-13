using App.Utility;
using UnityEngine;

public abstract class WindowBase : MonoBehaviour
{
	public class BackButtonMessage
	{
		public enum Flags
		{
			STAY_SEQUENCE
		}

		private Bitset32 m_flags;

		public void StaySequence()
		{
			SetFlag(Flags.STAY_SEQUENCE, true);
		}

		public bool IsFlag(Flags flag)
		{
			return m_flags.Test((int)flag);
		}

		private void SetFlag(Flags flag, bool value)
		{
			m_flags.Set((int)flag, value);
		}
	}

	public void Destroy()
	{
		RemoveBackKeyCallBack();
	}

	private void Awake()
	{
		EntryBackKeyCallBack();
	}

	public abstract void OnClickPlatformBackButton(BackButtonMessage msg);

	public void EntryBackKeyCallBack()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	public void RemoveBackKeyCallBack()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}
}
