using System.Collections.Generic;
using UI;
using UnityEngine;

public abstract class UIRectItemSlot : MonoBehaviour
{
	public UISprite icon;

	public UIWidget background;

	public UILabel label;

	public AudioClip grabSound;

	public AudioClip placeSound;

	public AudioClip errorSound;

	private UIInvGameItem mItem;

	private string mText = string.Empty;

	private static UIInvGameItem mDraggedItem;

	protected abstract UIInvGameItem observedItem
	{
		get;
	}

	protected abstract UIInvGameItem Replace(UIInvGameItem item);

	private void OnTooltip(bool show)
	{
		UIInvGameItem uIInvGameItem = (!show) ? null : mItem;
		if (uIInvGameItem != null)
		{
			UIInvBaseItem baseItem = uIInvGameItem.baseItem;
			if (baseItem != null)
			{
				string text = "[" + NGUITools.EncodeColor(uIInvGameItem.color) + "]" + uIInvGameItem.name + "[-]\n";
				string text2 = text;
				text = text2 + "[AFAFAF]Level " + uIInvGameItem.itemLevel + " " + baseItem.slot;
				List<UIInvStat> list = uIInvGameItem.CalculateStats();
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					UIInvStat uIInvStat = list[i];
					if (uIInvStat.amount != 0)
					{
						text = ((uIInvStat.amount >= 0) ? (text + "\n[00FF00]+" + uIInvStat.amount) : (text + "\n[FF0000]" + uIInvStat.amount));
						if (uIInvStat.modifier == UIInvStat.Modifier.Percent)
						{
							text += "%";
						}
						text = text + " " + uIInvStat.id;
						text += "[-]";
					}
				}
				if (!string.IsNullOrEmpty(baseItem.description))
				{
					text = text + "\n[FF9900]" + baseItem.description;
				}
				UITooltip.ShowText(text);
				return;
			}
		}
		UITooltip.ShowText(null);
	}

	private void OnClick()
	{
		if (mDraggedItem != null)
		{
			OnDrop(null);
		}
		else if (mItem != null)
		{
			mDraggedItem = Replace(null);
			if (mDraggedItem != null)
			{
				NGUITools.PlaySound(grabSound);
			}
			UpdateCursor();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (mDraggedItem == null && mItem != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			mDraggedItem = Replace(null);
			NGUITools.PlaySound(grabSound);
			UpdateCursor();
		}
	}

	private void OnDrop(GameObject go)
	{
		UIInvGameItem uIInvGameItem = Replace(mDraggedItem);
		if (mDraggedItem == uIInvGameItem)
		{
			NGUITools.PlaySound(errorSound);
		}
		else if (uIInvGameItem != null)
		{
			NGUITools.PlaySound(grabSound);
		}
		else
		{
			NGUITools.PlaySound(placeSound);
		}
		mDraggedItem = uIInvGameItem;
		UpdateCursor();
	}

	private void UpdateCursor()
	{
		if (mDraggedItem != null && mDraggedItem.baseItem != null)
		{
			UI.UICursor.Set(mDraggedItem.baseItem.iconAtlas, mDraggedItem.baseItem.iconName);
		}
		else
		{
			UI.UICursor.Clear();
		}
	}

	private void Update()
	{
		UIInvGameItem observedItem = this.observedItem;
		if (mItem == observedItem)
		{
			return;
		}
		mItem = observedItem;
		UIInvBaseItem uIInvBaseItem = (observedItem == null) ? null : observedItem.baseItem;
		if (label != null)
		{
			string text = (observedItem == null) ? null : observedItem.name;
			if (string.IsNullOrEmpty(mText))
			{
				mText = label.text;
			}
			label.text = ((text == null) ? mText : text);
		}
		if (icon != null)
		{
			if (uIInvBaseItem == null || uIInvBaseItem.iconAtlas == null)
			{
				icon.enabled = false;
			}
			else
			{
				icon.atlas = uIInvBaseItem.iconAtlas;
				icon.spriteName = uIInvBaseItem.iconName;
				icon.enabled = true;
				icon.MakePixelPerfect();
			}
		}
		if (background != null)
		{
			background.color = ((observedItem == null) ? Color.white : observedItem.color);
		}
	}
}
