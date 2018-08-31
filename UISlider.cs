using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/NGUI Slider")]
[ExecuteInEditMode]
public class UISlider : UIProgressBar
{
	private enum Direction
	{
		Horizontal,
		Vertical,
		Upgraded
	}

	[HideInInspector]
	[SerializeField]
	private Transform foreground;

	[HideInInspector]
	[SerializeField]
	private float rawValue = 1f;

	[HideInInspector]
	[SerializeField]
	private Direction direction = Direction.Upgraded;

	[HideInInspector]
	[SerializeField]
	protected bool mInverted;

	[Obsolete("Use 'value' instead")]
	public float sliderValue
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = value;
		}
	}

	[Obsolete("Use 'fillDirection' instead")]
	public bool inverted
	{
		get
		{
			return base.isInverted;
		}
		set
		{
		}
	}

	protected override void Upgrade()
	{
		if (direction != Direction.Upgraded)
		{
			mValue = rawValue;
			if (foreground != null)
			{
				mFG = foreground.GetComponent<UIWidget>();
			}
			if (direction == Direction.Horizontal)
			{
				mFill = (mInverted ? FillDirection.RightToLeft : FillDirection.LeftToRight);
			}
			else
			{
				mFill = ((!mInverted) ? FillDirection.BottomToTop : FillDirection.TopToBottom);
			}
			direction = Direction.Upgraded;
		}
	}

	protected override void OnStart()
	{
		GameObject go = (!(mBG != null) || (!(mBG.GetComponent<Collider>() != null) && !(mBG.GetComponent<Collider2D>() != null))) ? base.gameObject : mBG.gameObject;
		UIEventListener uIEventListener = UIEventListener.Get(go);
		UIEventListener uIEventListener2 = uIEventListener;
		uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnPressBackground));
		UIEventListener uIEventListener3 = uIEventListener;
		uIEventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener3.onDrag, new UIEventListener.VectorDelegate(OnDragBackground));
		if (thumb != null && (thumb.GetComponent<Collider>() != null || thumb.GetComponent<Collider2D>() != null) && (mFG == null || thumb != mFG.cachedTransform))
		{
			UIEventListener uIEventListener4 = UIEventListener.Get(thumb.gameObject);
			UIEventListener uIEventListener5 = uIEventListener4;
			uIEventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener5.onPress, new UIEventListener.BoolDelegate(OnPressForeground));
			UIEventListener uIEventListener6 = uIEventListener4;
			uIEventListener6.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uIEventListener6.onDrag, new UIEventListener.VectorDelegate(OnDragForeground));
		}
	}

	protected void OnPressBackground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = ScreenToValue(UICamera.lastTouchPosition);
			if (!isPressed && onDragFinished != null)
			{
				onDragFinished();
			}
		}
	}

	protected void OnDragBackground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = ScreenToValue(UICamera.lastTouchPosition);
		}
	}

	protected void OnPressForeground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			if (isPressed)
			{
				mOffset = ((!(mFG == null)) ? (base.value - ScreenToValue(UICamera.lastTouchPosition)) : 0f);
			}
			else if (onDragFinished != null)
			{
				onDragFinished();
			}
		}
	}

	protected void OnDragForeground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme != UICamera.ControlScheme.Controller)
		{
			mCam = UICamera.currentCamera;
			base.value = mOffset + ScreenToValue(UICamera.lastTouchPosition);
		}
	}

	protected void OnKey(KeyCode key)
	{
		if (base.enabled)
		{
			float num = (!((float)numberOfSteps > 1f)) ? 0.125f : (1f / (float)(numberOfSteps - 1));
			switch (mFill)
			{
			case FillDirection.LeftToRight:
				switch (key)
				{
				case KeyCode.LeftArrow:
					base.value = mValue - num;
					break;
				case KeyCode.RightArrow:
					base.value = mValue + num;
					break;
				}
				break;
			case FillDirection.RightToLeft:
				switch (key)
				{
				case KeyCode.LeftArrow:
					base.value = mValue + num;
					break;
				case KeyCode.RightArrow:
					base.value = mValue - num;
					break;
				}
				break;
			case FillDirection.BottomToTop:
				switch (key)
				{
				case KeyCode.DownArrow:
					base.value = mValue - num;
					break;
				case KeyCode.UpArrow:
					base.value = mValue + num;
					break;
				}
				break;
			case FillDirection.TopToBottom:
				switch (key)
				{
				case KeyCode.DownArrow:
					base.value = mValue + num;
					break;
				case KeyCode.UpArrow:
					base.value = mValue - num;
					break;
				}
				break;
			}
		}
	}
}
