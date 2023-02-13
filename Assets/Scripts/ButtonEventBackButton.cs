using System.Collections.Generic;
using UnityEngine;

public class ButtonEventBackButton : MonoBehaviour
{
	public struct BtnData
	{
		public GameObject obj;

		public ButtonInfoTable.ButtonType btn_type;

		public BtnData(GameObject obj, ButtonInfoTable.ButtonType btn_type)
		{
			this.obj = obj;
			this.btn_type = btn_type;
		}
	}

	public delegate void ButtonClickedCallback(ButtonInfoTable.ButtonType buttonType);

	private ButtonInfoTable m_info_table = new ButtonInfoTable();

	private GameObject m_menu_anim_obj;

	private List<BtnData> m_btn_obj_list;

	private ButtonClickedCallback m_callback;

	public void Initialize(ButtonClickedCallback callback)
	{
		m_callback = callback;
		m_menu_anim_obj = HudMenuUtility.GetMenuAnimUIObject();
		m_btn_obj_list = new List<BtnData>();
		if (m_menu_anim_obj != null)
		{
			for (uint num = 0u; num < 49; num++)
			{
				SetupBackButton((ButtonInfoTable.ButtonType)num);
			}
		}
	}

	public void SetupBackButton(ButtonInfoTable.ButtonType buttonType)
	{
		for (int i = 0; i < m_btn_obj_list.Count; i++)
		{
			BtnData btnData = m_btn_obj_list[i];
			if (btnData.btn_type == buttonType)
			{
				return;
			}
		}
		if (string.IsNullOrEmpty(m_info_table.m_button_info[(int)buttonType].clickButtonPath))
		{
			return;
		}
		Transform transform = m_menu_anim_obj.transform.Find(m_info_table.m_button_info[(int)buttonType].clickButtonPath);
		if (transform == null)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		if (gameObject == null)
		{
			return;
		}
		BtnData item = new BtnData(gameObject, buttonType);
		m_btn_obj_list.Add(item);
		UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
		if (component == null)
		{
			gameObject.AddComponent<UIButtonMessage>();
			component = gameObject.GetComponent<UIButtonMessage>();
		}
		if (component != null)
		{
			component.enabled = true;
			component.trigger = UIButtonMessage.Trigger.OnClick;
			component.target = base.gameObject;
			component.functionName = "OnButtonClicked";
		}
		UIPlayAnimation[] components = gameObject.GetComponents<UIPlayAnimation>();
		if (components != null)
		{
			UIPlayAnimation[] array = components;
			foreach (UIPlayAnimation uIPlayAnimation in array)
			{
				uIPlayAnimation.target = null;
			}
		}
	}

	private void OnButtonClicked(GameObject obj)
	{
		if (obj == null || m_callback == null || m_btn_obj_list == null)
		{
			return;
		}
		int count = m_btn_obj_list.Count;
		for (int i = 0; i < count; i++)
		{
			BtnData btnData = m_btn_obj_list[i];
			if (!(obj != btnData.obj))
			{
				BtnData btnData2 = m_btn_obj_list[i];
				ButtonInfoTable.ButtonType btn_type = btnData2.btn_type;
				m_callback(btn_type);
				m_info_table.PlaySE(btn_type);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
