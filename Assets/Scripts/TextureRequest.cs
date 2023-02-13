using UnityEngine;

public abstract class TextureRequest
{
	public abstract void LoadDone(Texture tex);

	public abstract bool IsEnableLoad();

	public abstract string GetFileName();
}
