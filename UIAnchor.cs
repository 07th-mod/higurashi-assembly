using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Anchor")]
[ExecuteInEditMode]
public class UIAnchor : MonoBehaviour
{
	public enum Side
	{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center
	}

	public Camera uiCamera;

	public GameObject container;

	public Side side = Side.Center;

	public bool runOnlyOnce = true;

	public Vector2 relativeOffset = Vector2.zero;

	public Vector2 pixelOffset = Vector2.zero;

	[HideInInspector]
	[SerializeField]
	private UIWidget widgetContainer;

	private Transform mTrans;

	private Animation mAnim;

	private Rect mRect = default(Rect);

	private UIRoot mRoot;

	private bool mStarted;

	private void Awake()
	{
		mTrans = base.transform;
		mAnim = GetComponent<Animation>();
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(ScreenSizeChanged));
	}

	private void OnDestroy()
	{
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(ScreenSizeChanged));
	}

	private void ScreenSizeChanged()
	{
		if (mStarted && runOnlyOnce)
		{
			Update();
		}
	}

	private void Start()
	{
		if (container == null && widgetContainer != null)
		{
			container = widgetContainer.gameObject;
			widgetContainer = null;
		}
		mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		if (uiCamera == null)
		{
			uiCamera = NGUITools.FindCameraForLayer(base.gameObject.layer);
		}
		Update();
		mStarted = true;
	}

	private void Update()
	{
		if (mAnim != null && mAnim.enabled && mAnim.isPlaying)
		{
			return;
		}
		bool flag = false;
		UIWidget uIWidget = (!(container == null)) ? container.GetComponent<UIWidget>() : null;
		UIPanel uIPanel = (!(container == null) || !(uIWidget == null)) ? container.GetComponent<UIPanel>() : null;
		if (uIWidget != null)
		{
			Bounds bounds = uIWidget.CalculateBounds(container.transform.parent);
			ref Rect reference = ref mRect;
			Vector3 min = bounds.min;
			reference.x = min.x;
			ref Rect reference2 = ref mRect;
			Vector3 min2 = bounds.min;
			reference2.y = min2.y;
			ref Rect reference3 = ref mRect;
			Vector3 size = bounds.size;
			reference3.width = size.x;
			ref Rect reference4 = ref mRect;
			Vector3 size2 = bounds.size;
			reference4.height = size2.y;
		}
		else if (uIPanel != null)
		{
			if (uIPanel.clipping == UIDrawCall.Clipping.None)
			{
				float num = (!(mRoot != null)) ? 0.5f : ((float)mRoot.activeHeight / (float)Screen.height * 0.5f);
				mRect.xMin = (float)(-Screen.width) * num;
				mRect.yMin = (float)(-Screen.height) * num;
				mRect.xMax = 0f - mRect.xMin;
				mRect.yMax = 0f - mRect.yMin;
			}
			else
			{
				Vector4 finalClipRegion = uIPanel.finalClipRegion;
				mRect.x = finalClipRegion.x - finalClipRegion.z * 0.5f;
				mRect.y = finalClipRegion.y - finalClipRegion.w * 0.5f;
				mRect.width = finalClipRegion.z;
				mRect.height = finalClipRegion.w;
			}
		}
		else if (container != null)
		{
			Transform parent = container.transform.parent;
			Bounds bounds2 = (!(parent != null)) ? NGUIMath.CalculateRelativeWidgetBounds(container.transform) : NGUIMath.CalculateRelativeWidgetBounds(parent, container.transform);
			ref Rect reference5 = ref mRect;
			Vector3 min3 = bounds2.min;
			reference5.x = min3.x;
			ref Rect reference6 = ref mRect;
			Vector3 min4 = bounds2.min;
			reference6.y = min4.y;
			ref Rect reference7 = ref mRect;
			Vector3 size3 = bounds2.size;
			reference7.width = size3.x;
			ref Rect reference8 = ref mRect;
			Vector3 size4 = bounds2.size;
			reference8.height = size4.y;
		}
		else
		{
			if (!(uiCamera != null))
			{
				return;
			}
			flag = true;
			mRect = uiCamera.pixelRect;
		}
		float x = (mRect.xMin + mRect.xMax) * 0.5f;
		float y = (mRect.yMin + mRect.yMax) * 0.5f;
		Vector3 vector = new Vector3(x, y, 0f);
		if (side != Side.Center)
		{
			if (side == Side.Right || side == Side.TopRight || side == Side.BottomRight)
			{
				vector.x = mRect.xMax;
			}
			else if (side == Side.Top || side == Side.Center || side == Side.Bottom)
			{
				vector.x = x;
			}
			else
			{
				vector.x = mRect.xMin;
			}
			if (side == Side.Top || side == Side.TopRight || side == Side.TopLeft)
			{
				vector.y = mRect.yMax;
			}
			else if (side == Side.Left || side == Side.Center || side == Side.Right)
			{
				vector.y = y;
			}
			else
			{
				vector.y = mRect.yMin;
			}
		}
		float width = mRect.width;
		float height = mRect.height;
		vector.x += pixelOffset.x + relativeOffset.x * width;
		vector.y += pixelOffset.y + relativeOffset.y * height;
		if (flag)
		{
			if (uiCamera.orthographic)
			{
				vector.x = Mathf.Round(vector.x);
				vector.y = Mathf.Round(vector.y);
			}
			Vector3 vector2 = uiCamera.WorldToScreenPoint(mTrans.position);
			vector.z = vector2.z;
			vector = uiCamera.ScreenToWorldPoint(vector);
		}
		else
		{
			vector.x = Mathf.Round(vector.x);
			vector.y = Mathf.Round(vector.y);
			if (uIPanel != null)
			{
				vector = uIPanel.cachedTransform.TransformPoint(vector);
			}
			else if (container != null)
			{
				Transform parent2 = container.transform.parent;
				if (parent2 != null)
				{
					vector = parent2.TransformPoint(vector);
				}
			}
			Vector3 position = mTrans.position;
			vector.z = position.z;
		}
		if (mTrans.position != vector)
		{
			mTrans.position = vector;
		}
		if (runOnlyOnce && Application.isPlaying)
		{
			base.enabled = false;
		}
	}
}
