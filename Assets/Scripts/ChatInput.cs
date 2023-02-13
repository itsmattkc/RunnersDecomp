using UnityEngine;

[AddComponentMenu("NGUI/Examples/Chat Input")]
[RequireComponent(typeof(UIInput))]
public class ChatInput : MonoBehaviour
{
	public UITextList textList;

	public bool fillWithDummyData;

	private UIInput mInput;

	private bool mIgnoreNextEnter;

	private void Start()
	{
		mInput = GetComponent<UIInput>();
		if (fillWithDummyData && textList != null)
		{
			for (int i = 0; i < 30; i++)
			{
				textList.Add(((i % 2 != 0) ? "[AAAAAA]" : "[FFFFFF]") + "This is an example paragraph for the text list, testing line " + i + "[-]");
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Return))
		{
			if (!mIgnoreNextEnter && !mInput.selected)
			{
				mInput.label.maxLineCount = 1;
				mInput.selected = true;
			}
			mIgnoreNextEnter = false;
		}
	}

	public void OnSubmit()
	{
		if (textList != null)
		{
			string text = NGUITools.StripSymbols(mInput.value);
			if (!string.IsNullOrEmpty(text))
			{
				textList.Add(text);
				mInput.value = string.Empty;
				mInput.selected = false;
			}
		}
		mIgnoreNextEnter = true;
	}
}
