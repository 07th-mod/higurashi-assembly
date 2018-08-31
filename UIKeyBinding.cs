using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	public enum Action
	{
		PressAndClick,
		Select,
		All
	}

	public enum Modifier
	{
		None,
		Shift,
		Control,
		Alt
	}

	public KeyCode keyCode;

	public Modifier modifier;

	public Action action;

	private bool mIgnoreUp;

	private bool mIsInput;

	private bool mPress;

	private void Start()
	{
		UIInput component = GetComponent<UIInput>();
		mIsInput = (component != null);
		if (component != null)
		{
			EventDelegate.Add(component.onSubmit, OnSubmit);
		}
	}

	private void OnSubmit()
	{
		if (UICamera.currentKey == keyCode && IsModifierActive())
		{
			mIgnoreUp = true;
		}
	}

	private bool IsModifierActive()
	{
		if (modifier == Modifier.None)
		{
			return true;
		}
		if (modifier == Modifier.Alt)
		{
			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
			{
				return true;
			}
		}
		else if (modifier == Modifier.Control)
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				return true;
			}
		}
		else if (modifier == Modifier.Shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
		{
			return true;
		}
		return false;
	}

	private void Update()
	{
		if (keyCode != 0 && IsModifierActive())
		{
			if (action == Action.PressAndClick || action == Action.All)
			{
				if (UICamera.inputHasFocus)
				{
					return;
				}
				UICamera.currentTouch = UICamera.controller;
				UICamera.currentScheme = UICamera.ControlScheme.Mouse;
				UICamera.currentTouch.current = base.gameObject;
				if (Input.GetKeyDown(keyCode))
				{
					mPress = true;
					UICamera.Notify(base.gameObject, "OnPress", true);
				}
				if (Input.GetKeyUp(keyCode))
				{
					UICamera.Notify(base.gameObject, "OnPress", false);
					if (mPress)
					{
						UICamera.Notify(base.gameObject, "OnClick", null);
						mPress = false;
					}
				}
				UICamera.currentTouch.current = null;
			}
			if ((action == Action.Select || action == Action.All) && Input.GetKeyUp(keyCode))
			{
				if (mIsInput)
				{
					if (!mIgnoreUp && !UICamera.inputHasFocus)
					{
						UICamera.selectedObject = base.gameObject;
					}
					mIgnoreUp = false;
				}
				else
				{
					UICamera.selectedObject = base.gameObject;
				}
			}
		}
	}
}
