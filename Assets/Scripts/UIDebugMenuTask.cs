using System.Collections.Generic;
using UnityEngine;

public class UIDebugMenuTask : MonoBehaviour
{
	private UIDebugMenuTask m_parent;

	private Dictionary<string, UIDebugMenuTask> m_childList;

	private bool m_isActive;

	private void Start()
	{
		m_childList = new Dictionary<string, UIDebugMenuTask>();
		m_isActive = false;
		OnStartFromTask();
	}

	private void OnGUI()
	{
		if (IsActive())
		{
			OnGuiFromTask();
		}
	}

	public bool IsActive()
	{
		return m_isActive;
	}

	public void AddChild(string childName, GameObject child)
	{
		if (m_childList != null && !(child == null))
		{
			UIDebugMenuTask component = child.GetComponent<UIDebugMenuTask>();
			AddChild(childName, component);
		}
	}

	public void AddChild(string childName, UIDebugMenuTask child)
	{
		if (m_childList != null && !(child == null))
		{
			child.SetParent(this);
			m_childList.Add(childName, child);
		}
	}

	public void TransitionFrom()
	{
		Debug.Log(string.Format("Transition From:{0}", ToString()));
		m_isActive = true;
		OnTransitionFrom();
	}

	public void TransitionToParent()
	{
		if (m_parent != null)
		{
			m_parent.TransitionFrom();
			TransitionTo();
		}
	}

	public void TransitionToChild(string childName)
	{
		if (m_childList.ContainsKey(childName))
		{
			UIDebugMenuTask uIDebugMenuTask = m_childList[childName];
			uIDebugMenuTask.TransitionFrom();
			TransitionTo();
			m_isActive = false;
			Debug.Log(string.Format("Transition to ChildMenu:{0}", childName));
		}
	}

	protected virtual void OnStartFromTask()
	{
	}

	protected virtual void OnGuiFromTask()
	{
	}

	protected virtual void OnTransitionFrom()
	{
	}

	protected virtual void OnTransitionTo()
	{
	}

	private void SetParent(UIDebugMenuTask parent)
	{
		m_parent = parent;
	}

	private void TransitionTo()
	{
		m_isActive = false;
		Debug.Log(string.Format("Transition To:{0}", ToString()));
		OnTransitionTo();
	}
}
