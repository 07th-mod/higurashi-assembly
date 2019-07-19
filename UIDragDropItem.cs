using System.Collections;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	public enum Restriction
	{
		None,
		Horizontal,
		Vertical,
		PressAndHold
	}

	public Restriction restriction;

	public bool cloneOnDrag;

	[HideInInspector]
	public float pressAndHoldDelay = 1f;

	protected Transform mTrans;

	protected Transform mParent;

	protected Collider mCollider;

	protected Collider2D mCollider2D;

	protected UIButton mButton;

	protected UIRoot mRoot;

	protected UIGrid mGrid;

	protected UITable mTable;

	protected int mTouchID = int.MinValue;

	protected float mDragStartTime;

	protected UIDragScrollView mDragScrollView;

	protected bool mPressed;

	protected bool mDragging;

	protected virtual void Start()
	{
		mTrans = base.transform;
		mCollider = GetComponent<Collider>();
		mCollider2D = GetComponent<Collider2D>();
		mButton = GetComponent<UIButton>();
		mDragScrollView = GetComponent<UIDragScrollView>();
	}

	protected void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			mDragStartTime = RealTime.time + pressAndHoldDelay;
			mPressed = true;
		}
		else
		{
			mPressed = false;
		}
	}

	protected virtual void Update()
	{
		if (restriction == Restriction.PressAndHold && mPressed && !mDragging && mDragStartTime < RealTime.time)
		{
			StartDragging();
		}
	}

	protected void OnDragStart()
	{
		if (!base.enabled || mTouchID != int.MinValue)
		{
			return;
		}
		if (restriction != 0)
		{
			if (restriction == Restriction.Horizontal)
			{
				Vector2 totalDelta = UICamera.currentTouch.totalDelta;
				if (Mathf.Abs(totalDelta.x) < Mathf.Abs(totalDelta.y))
				{
					return;
				}
			}
			else if (restriction == Restriction.Vertical)
			{
				Vector2 totalDelta2 = UICamera.currentTouch.totalDelta;
				if (Mathf.Abs(totalDelta2.x) > Mathf.Abs(totalDelta2.y))
				{
					return;
				}
			}
			else if (restriction == Restriction.PressAndHold)
			{
				return;
			}
		}
		StartDragging();
	}

	protected virtual void StartDragging()
	{
		if (mDragging)
		{
			return;
		}
		if (cloneOnDrag)
		{
			GameObject gameObject = NGUITools.AddChild(base.transform.parent.gameObject, base.gameObject);
			gameObject.transform.localPosition = base.transform.localPosition;
			gameObject.transform.localRotation = base.transform.localRotation;
			gameObject.transform.localScale = base.transform.localScale;
			UIButtonColor component = gameObject.GetComponent<UIButtonColor>();
			if (component != null)
			{
				component.defaultColor = GetComponent<UIButtonColor>().defaultColor;
			}
			UICamera.currentTouch.dragged = gameObject;
			UIDragDropItem component2 = gameObject.GetComponent<UIDragDropItem>();
			component2.mDragging = true;
			component2.Start();
			component2.OnDragDropStart();
		}
		else
		{
			mDragging = true;
			OnDragDropStart();
		}
	}

	protected void OnDrag(Vector2 delta)
	{
		if (mDragging && base.enabled && mTouchID == UICamera.currentTouchID)
		{
			OnDragDropMove(delta * mRoot.pixelSizeAdjustment);
		}
	}

	protected void OnDragEnd()
	{
		if (base.enabled && mTouchID == UICamera.currentTouchID)
		{
			StopDragging(UICamera.hoveredObject);
		}
	}

	public void StopDragging(GameObject go)
	{
		if (mDragging)
		{
			mDragging = false;
			OnDragDropRelease(go);
		}
	}

	protected virtual void OnDragDropStart()
	{
		if (mDragScrollView != null)
		{
			mDragScrollView.enabled = false;
		}
		if (mButton != null)
		{
			mButton.isEnabled = false;
		}
		else if (mCollider != null)
		{
			mCollider.enabled = false;
		}
		else if (mCollider2D != null)
		{
			mCollider2D.enabled = false;
		}
		mTouchID = UICamera.currentTouchID;
		mParent = mTrans.parent;
		mRoot = NGUITools.FindInParents<UIRoot>(mParent);
		mGrid = NGUITools.FindInParents<UIGrid>(mParent);
		mTable = NGUITools.FindInParents<UITable>(mParent);
		if (UIDragDropRoot.root != null)
		{
			mTrans.parent = UIDragDropRoot.root;
		}
		Vector3 localPosition = mTrans.localPosition;
		localPosition.z = 0f;
		mTrans.localPosition = localPosition;
		TweenPosition component = GetComponent<TweenPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
		SpringPosition component2 = GetComponent<SpringPosition>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		NGUITools.MarkParentAsChanged(base.gameObject);
		if (mTable != null)
		{
			mTable.repositionNow = true;
		}
		if (mGrid != null)
		{
			mGrid.repositionNow = true;
		}
	}

	protected virtual void OnDragDropMove(Vector2 delta)
	{
		mTrans.localPosition += (Vector3)delta;
	}

	protected virtual void OnDragDropRelease(GameObject surface)
	{
		if (!cloneOnDrag)
		{
			mTouchID = int.MinValue;
			if (mButton != null)
			{
				mButton.isEnabled = true;
			}
			else if (mCollider != null)
			{
				mCollider.enabled = true;
			}
			else if (mCollider2D != null)
			{
				mCollider2D.enabled = true;
			}
			UIDragDropContainer uIDragDropContainer = (!(bool)surface) ? null : NGUITools.FindInParents<UIDragDropContainer>(surface);
			if (uIDragDropContainer != null)
			{
				mTrans.parent = ((!(uIDragDropContainer.reparentTarget != null)) ? uIDragDropContainer.transform : uIDragDropContainer.reparentTarget);
				Vector3 localPosition = mTrans.localPosition;
				localPosition.z = 0f;
				mTrans.localPosition = localPosition;
			}
			else
			{
				mTrans.parent = mParent;
			}
			mParent = mTrans.parent;
			mGrid = NGUITools.FindInParents<UIGrid>(mParent);
			mTable = NGUITools.FindInParents<UITable>(mParent);
			if (mDragScrollView != null)
			{
				StartCoroutine(EnableDragScrollView());
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
			if (mTable != null)
			{
				mTable.repositionNow = true;
			}
			if (mGrid != null)
			{
				mGrid.repositionNow = true;
			}
			OnDragDropEnd();
		}
		else
		{
			NGUITools.Destroy(base.gameObject);
		}
	}

	protected virtual void OnDragDropEnd()
	{
	}

	protected IEnumerator EnableDragScrollView()
	{
		yield return new WaitForEndOfFrame();
		if (mDragScrollView != null)
		{
			mDragScrollView.enabled = true;
		}
	}
}
