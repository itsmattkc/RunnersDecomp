using UnityEngine;

public abstract class SettingBase : MonoBehaviour
{
	public void Setup(string anthorPath)
	{
		OnSetup(anthorPath);
	}

	public void PlayStart()
	{
		OnPlayStart();
	}

	public bool IsEndPlay()
	{
		return OnIsEndPlay();
	}

	private void Update()
	{
		OnUpdate();
	}

	protected abstract void OnSetup(string anthorPath);

	protected abstract void OnPlayStart();

	protected abstract bool OnIsEndPlay();

	protected abstract void OnUpdate();
}
