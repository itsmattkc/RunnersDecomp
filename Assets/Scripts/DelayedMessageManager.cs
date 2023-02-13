using System.Collections.Generic;
using UnityEngine;

public class DelayedMessageManager : MonoBehaviour
{
	private class DelayMessageData
	{
		public string m_objectName;

		public string m_methodName;

		public GameObject m_object;

		public object m_option;

		public DelayMessageData(string objectName, string methodName, object option)
		{
			m_objectName = objectName;
			m_methodName = methodName;
			m_option = option;
		}

		public DelayMessageData(GameObject gameObject, string methodName, object option)
		{
			m_object = gameObject;
			m_methodName = methodName;
			m_option = option;
		}
	}

	private List<DelayMessageData> m_datas;

	private List<DelayMessageData> m_sendToTagDatas;

	private static DelayedMessageManager instance;

	public static DelayedMessageManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObjectUtil.FindGameObjectComponent<DelayedMessageManager>("DelayedMessageManager");
			}
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	private void Start()
	{
		m_datas = new List<DelayMessageData>();
		m_sendToTagDatas = new List<DelayMessageData>();
	}

	private void Update()
	{
		if (m_sendToTagDatas.Count > 0)
		{
			foreach (DelayMessageData sendToTagData in m_sendToTagDatas)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(sendToTagData.m_objectName);
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					AddDelayedMessage(gameObject, sendToTagData.m_methodName, sendToTagData.m_option);
				}
			}
			m_sendToTagDatas.Clear();
		}
		if (m_datas.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < m_datas.Count; j++)
		{
			DelayMessageData delayMessageData = m_datas[j];
			if (delayMessageData.m_object == null && delayMessageData.m_objectName != null)
			{
				delayMessageData.m_object = GameObject.Find(delayMessageData.m_objectName);
			}
			if (delayMessageData.m_object != null)
			{
				delayMessageData.m_object.SendMessage(delayMessageData.m_methodName, delayMessageData.m_option, SendMessageOptions.DontRequireReceiver);
			}
		}
		m_datas.Clear();
	}

	public void AddDelayedMessage(string objectName, string methodName, object option)
	{
		DelayMessageData item = new DelayMessageData(objectName, methodName, option);
		m_datas.Add(item);
	}

	public void AddDelayedMessage(GameObject gameObject, string methodName, object option)
	{
		DelayMessageData item = new DelayMessageData(gameObject, methodName, option);
		m_datas.Add(item);
	}

	public void AddDelayedMessageToTag(string objectName, string methodName, object option)
	{
		DelayMessageData item = new DelayMessageData(objectName, methodName, option);
		m_sendToTagDatas.Add(item);
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(this);
		return false;
	}
}
