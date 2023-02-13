using Message;
using UnityEngine;

public abstract class ErrorHandleBase
{
	public abstract void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg);

	public abstract void StartErrorHandle();

	public abstract void Update();

	public abstract bool IsEnd();

	public abstract void EndErrorHandle();
}
