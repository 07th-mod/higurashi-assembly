using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Panel")]
[ExecuteInEditMode]
public class UIPanel : UIRect
{
	public enum RenderQueue
	{
		Automatic,
		StartAt,
		Explicit
	}

	public delegate void OnGeometryUpdated();

	public delegate void OnClippingMoved(UIPanel panel);

	public static List<UIPanel> list = new List<UIPanel>();

	public OnGeometryUpdated onGeometryUpdated;

	public bool showInPanelTool = true;

	public bool generateNormals;

	public bool widgetsAreStatic;

	public bool cullWhileDragging = true;

	public bool alwaysOnScreen;

	public bool anchorOffset;

	public bool softBorderPadding = true;

	public RenderQueue renderQueue;

	public int startingRenderQueue = 3000;

	[NonSerialized]
	public List<UIWidget> widgets = new List<UIWidget>();

	[NonSerialized]
	public List<UIDrawCall> drawCalls = new List<UIDrawCall>();

	[NonSerialized]
	public Matrix4x4 worldToLocal = Matrix4x4.identity;

	[NonSerialized]
	public Vector4 drawCallClipRange = new Vector4(0f, 0f, 1f, 1f);

	public OnClippingMoved onClipMove;

	[HideInInspector]
	[SerializeField]
	private Texture2D mClipTexture;

	[HideInInspector]
	[SerializeField]
	private float mAlpha = 1f;

	[HideInInspector]
	[SerializeField]
	private UIDrawCall.Clipping mClipping;

	[HideInInspector]
	[SerializeField]
	private Vector4 mClipRange = new Vector4(0f, 0f, 300f, 200f);

	[HideInInspector]
	[SerializeField]
	private Vector2 mClipSoftness = new Vector2(4f, 4f);

	[HideInInspector]
	[SerializeField]
	private int mDepth;

	[HideInInspector]
	[SerializeField]
	private int mSortingOrder;

	private bool mRebuild;

	private bool mResized;

	[SerializeField]
	private Vector2 mClipOffset = Vector2.zero;

	private float mCullTime;

	private float mUpdateTime;

	private int mMatrixFrame = -1;

	private int mAlphaFrameID;

	private int mLayer = -1;

	private static float[] mTemp = new float[4];

	private Vector2 mMin = Vector2.zero;

	private Vector2 mMax = Vector2.zero;

	private bool mHalfPixelOffset;

	private bool mSortWidgets;

	private bool mUpdateScroll;

	private UIPanel mParentPanel;

	private static Vector3[] mCorners = new Vector3[4];

	private static int mUpdateFrame = -1;

	private UIDrawCall.OnRenderCallback mOnRender;

	private bool mForced;

	[CompilerGenerated]
	private static Comparison<UIPanel> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Comparison<UIPanel> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Comparison<UIWidget> _003C_003Ef__mg_0024cache2;

	public static int nextUnusedDepth
	{
		get
		{
			int num = int.MinValue;
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				num = Mathf.Max(num, list[i].depth);
			}
			return (num != int.MinValue) ? (num + 1) : 0;
		}
	}

	public override bool canBeAnchored => mClipping != UIDrawCall.Clipping.None;

	public override float alpha
	{
		get
		{
			return mAlpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (mAlpha != num)
			{
				mAlphaFrameID = -1;
				mResized = true;
				mAlpha = num;
				SetDirty();
			}
		}
	}

	public int depth
	{
		get
		{
			return mDepth;
		}
		set
		{
			if (mDepth != value)
			{
				mDepth = value;
				list.Sort(CompareFunc);
			}
		}
	}

	public int sortingOrder
	{
		get
		{
			return mSortingOrder;
		}
		set
		{
			if (mSortingOrder != value)
			{
				mSortingOrder = value;
				UpdateDrawCalls();
			}
		}
	}

	public float width
	{
		get
		{
			Vector2 viewSize = GetViewSize();
			return viewSize.x;
		}
	}

	public float height
	{
		get
		{
			Vector2 viewSize = GetViewSize();
			return viewSize.y;
		}
	}

	public bool halfPixelOffset => mHalfPixelOffset;

	public bool usedForUI => base.anchorCamera != null && mCam.orthographic;

	public Vector3 drawCallOffset
	{
		get
		{
			if (mHalfPixelOffset && base.anchorCamera != null && mCam.orthographic)
			{
				Vector2 windowSize = GetWindowSize();
				float num = 1f / windowSize.y / mCam.orthographicSize;
				return new Vector3(0f - num, num);
			}
			return Vector3.zero;
		}
	}

	public UIDrawCall.Clipping clipping
	{
		get
		{
			return mClipping;
		}
		set
		{
			if (mClipping != value)
			{
				mResized = true;
				mClipping = value;
				mMatrixFrame = -1;
			}
		}
	}

	public UIPanel parentPanel => mParentPanel;

	public int clipCount
	{
		get
		{
			int num = 0;
			UIPanel uIPanel = this;
			while (uIPanel != null)
			{
				if (uIPanel.mClipping == UIDrawCall.Clipping.SoftClip || uIPanel.mClipping == UIDrawCall.Clipping.TextureMask)
				{
					num++;
				}
				uIPanel = uIPanel.mParentPanel;
			}
			return num;
		}
	}

	public bool hasClipping => mClipping == UIDrawCall.Clipping.SoftClip || mClipping == UIDrawCall.Clipping.TextureMask;

	public bool hasCumulativeClipping => clipCount != 0;

	[Obsolete("Use 'hasClipping' or 'hasCumulativeClipping' instead")]
	public bool clipsChildren
	{
		get
		{
			return hasCumulativeClipping;
		}
	}

	public Vector2 clipOffset
	{
		get
		{
			return mClipOffset;
		}
		set
		{
			if (Mathf.Abs(mClipOffset.x - value.x) > 0.001f || Mathf.Abs(mClipOffset.y - value.y) > 0.001f)
			{
				mClipOffset = value;
				InvalidateClipping();
				if (onClipMove != null)
				{
					onClipMove(this);
				}
			}
		}
	}

	public Texture2D clipTexture
	{
		get
		{
			return mClipTexture;
		}
		set
		{
			if (mClipTexture != value)
			{
				mClipTexture = value;
			}
		}
	}

	[Obsolete("Use 'finalClipRegion' or 'baseClipRegion' instead")]
	public Vector4 clipRange
	{
		get
		{
			return baseClipRegion;
		}
		set
		{
			baseClipRegion = value;
		}
	}

	public Vector4 baseClipRegion
	{
		get
		{
			return mClipRange;
		}
		set
		{
			if (Mathf.Abs(mClipRange.x - value.x) > 0.001f || Mathf.Abs(mClipRange.y - value.y) > 0.001f || Mathf.Abs(mClipRange.z - value.z) > 0.001f || Mathf.Abs(mClipRange.w - value.w) > 0.001f)
			{
				mResized = true;
				mCullTime = ((mCullTime != 0f) ? (RealTime.time + 0.15f) : 0.001f);
				mClipRange = value;
				mMatrixFrame = -1;
				UIScrollView component = GetComponent<UIScrollView>();
				if (component != null)
				{
					component.UpdatePosition();
				}
				if (onClipMove != null)
				{
					onClipMove(this);
				}
			}
		}
	}

	public Vector4 finalClipRegion
	{
		get
		{
			Vector2 viewSize = GetViewSize();
			if (mClipping != 0)
			{
				return new Vector4(mClipRange.x + mClipOffset.x, mClipRange.y + mClipOffset.y, viewSize.x, viewSize.y);
			}
			return new Vector4(0f, 0f, viewSize.x, viewSize.y);
		}
	}

	public Vector2 clipSoftness
	{
		get
		{
			return mClipSoftness;
		}
		set
		{
			if (mClipSoftness != value)
			{
				mClipSoftness = value;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (mClipping == UIDrawCall.Clipping.None)
			{
				Vector3[] worldCorners = this.worldCorners;
				Transform cachedTransform = base.cachedTransform;
				for (int i = 0; i < 4; i++)
				{
					worldCorners[i] = cachedTransform.InverseTransformPoint(worldCorners[i]);
				}
				return worldCorners;
			}
			float num = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
			float num2 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
			float x = num + mClipRange.z;
			float y = num2 + mClipRange.w;
			mCorners[0] = new Vector3(num, num2);
			mCorners[1] = new Vector3(num, y);
			mCorners[2] = new Vector3(x, y);
			mCorners[3] = new Vector3(x, num2);
			return mCorners;
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (mClipping != 0)
			{
				float num = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
				float num2 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
				float x = num + mClipRange.z;
				float y = num2 + mClipRange.w;
				Transform cachedTransform = base.cachedTransform;
				mCorners[0] = cachedTransform.TransformPoint(num, num2, 0f);
				mCorners[1] = cachedTransform.TransformPoint(num, y, 0f);
				mCorners[2] = cachedTransform.TransformPoint(x, y, 0f);
				mCorners[3] = cachedTransform.TransformPoint(x, num2, 0f);
			}
			else
			{
				if (base.anchorCamera != null)
				{
					return mCam.GetWorldCorners(base.cameraRayDistance);
				}
				Vector2 viewSize = GetViewSize();
				float num3 = -0.5f * viewSize.x;
				float num4 = -0.5f * viewSize.y;
				float x2 = num3 + viewSize.x;
				float y2 = num4 + viewSize.y;
				mCorners[0] = new Vector3(num3, num4);
				mCorners[1] = new Vector3(num3, y2);
				mCorners[2] = new Vector3(x2, y2);
				mCorners[3] = new Vector3(x2, num4);
				if ((anchorOffset && mCam == null) || mCam.transform.parent != base.cachedTransform)
				{
					Vector3 position = base.cachedTransform.position;
					for (int i = 0; i < 4; i++)
					{
						mCorners[i] += position;
					}
				}
			}
			return mCorners;
		}
	}

	public static int CompareFunc(UIPanel a, UIPanel b)
	{
		if (a != b && a != null && b != null)
		{
			if (a.mDepth < b.mDepth)
			{
				return -1;
			}
			if (a.mDepth > b.mDepth)
			{
				return 1;
			}
			return (a.GetInstanceID() >= b.GetInstanceID()) ? 1 : (-1);
		}
		return 0;
	}

	private void InvalidateClipping()
	{
		mResized = true;
		mMatrixFrame = -1;
		mCullTime = ((mCullTime != 0f) ? (RealTime.time + 0.15f) : 0.001f);
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			UIPanel uIPanel = list[i];
			if (uIPanel != this && uIPanel.parentPanel == this)
			{
				uIPanel.InvalidateClipping();
			}
		}
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (mClipping != 0)
		{
			float num = mClipOffset.x + mClipRange.x - 0.5f * mClipRange.z;
			float num2 = mClipOffset.y + mClipRange.y - 0.5f * mClipRange.w;
			float num3 = num + mClipRange.z;
			float num4 = num2 + mClipRange.w;
			float x = (num + num3) * 0.5f;
			float y = (num2 + num4) * 0.5f;
			Transform cachedTransform = base.cachedTransform;
			UIRect.mSides[0] = cachedTransform.TransformPoint(num, y, 0f);
			UIRect.mSides[1] = cachedTransform.TransformPoint(x, num4, 0f);
			UIRect.mSides[2] = cachedTransform.TransformPoint(num3, y, 0f);
			UIRect.mSides[3] = cachedTransform.TransformPoint(x, num2, 0f);
			if (relativeTo != null)
			{
				for (int i = 0; i < 4; i++)
				{
					UIRect.mSides[i] = relativeTo.InverseTransformPoint(UIRect.mSides[i]);
				}
			}
			return UIRect.mSides;
		}
		if (base.anchorCamera != null && anchorOffset)
		{
			Vector3[] sides = mCam.GetSides(base.cameraRayDistance);
			Vector3 position = base.cachedTransform.position;
			for (int j = 0; j < 4; j++)
			{
				sides[j] += position;
			}
			if (relativeTo != null)
			{
				for (int k = 0; k < 4; k++)
				{
					sides[k] = relativeTo.InverseTransformPoint(sides[k]);
				}
			}
			return sides;
		}
		return base.GetSides(relativeTo);
	}

	public override void Invalidate(bool includeChildren)
	{
		mAlphaFrameID = -1;
		base.Invalidate(includeChildren);
	}

	public override float CalculateFinalAlpha(int frameID)
	{
		if (mAlphaFrameID != frameID)
		{
			mAlphaFrameID = frameID;
			UIRect parent = base.parent;
			finalAlpha = ((!(base.parent != null)) ? mAlpha : (parent.CalculateFinalAlpha(frameID) * mAlpha));
		}
		return finalAlpha;
	}

	public override void SetRect(float x, float y, float width, float height)
	{
		int num = Mathf.FloorToInt(width + 0.5f);
		int num2 = Mathf.FloorToInt(height + 0.5f);
		num = num >> 1 << 1;
		num2 = num2 >> 1 << 1;
		Transform cachedTransform = base.cachedTransform;
		Vector3 localPosition = cachedTransform.localPosition;
		localPosition.x = Mathf.Floor(x + 0.5f);
		localPosition.y = Mathf.Floor(y + 0.5f);
		if (num < 2)
		{
			num = 2;
		}
		if (num2 < 2)
		{
			num2 = 2;
		}
		baseClipRegion = new Vector4(localPosition.x, localPosition.y, num, num2);
		if (base.isAnchored)
		{
			cachedTransform = cachedTransform.parent;
			if ((bool)leftAnchor.target)
			{
				leftAnchor.SetHorizontal(cachedTransform, x);
			}
			if ((bool)rightAnchor.target)
			{
				rightAnchor.SetHorizontal(cachedTransform, x + width);
			}
			if ((bool)bottomAnchor.target)
			{
				bottomAnchor.SetVertical(cachedTransform, y);
			}
			if ((bool)topAnchor.target)
			{
				topAnchor.SetVertical(cachedTransform, y + height);
			}
		}
	}

	public bool IsVisible(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		UpdateTransformMatrix();
		a = worldToLocal.MultiplyPoint3x4(a);
		b = worldToLocal.MultiplyPoint3x4(b);
		c = worldToLocal.MultiplyPoint3x4(c);
		d = worldToLocal.MultiplyPoint3x4(d);
		mTemp[0] = a.x;
		mTemp[1] = b.x;
		mTemp[2] = c.x;
		mTemp[3] = d.x;
		float num = Mathf.Min(mTemp);
		float num2 = Mathf.Max(mTemp);
		mTemp[0] = a.y;
		mTemp[1] = b.y;
		mTemp[2] = c.y;
		mTemp[3] = d.y;
		float num3 = Mathf.Min(mTemp);
		float num4 = Mathf.Max(mTemp);
		if (num2 < mMin.x)
		{
			return false;
		}
		if (num4 < mMin.y)
		{
			return false;
		}
		if (num > mMax.x)
		{
			return false;
		}
		if (num3 > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(Vector3 worldPos)
	{
		if (mAlpha < 0.001f)
		{
			return false;
		}
		if (mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip)
		{
			return true;
		}
		UpdateTransformMatrix();
		Vector3 vector = worldToLocal.MultiplyPoint3x4(worldPos);
		if (vector.x < mMin.x)
		{
			return false;
		}
		if (vector.y < mMin.y)
		{
			return false;
		}
		if (vector.x > mMax.x)
		{
			return false;
		}
		if (vector.y > mMax.y)
		{
			return false;
		}
		return true;
	}

	public bool IsVisible(UIWidget w)
	{
		UIPanel uIPanel = this;
		Vector3[] array = null;
		while (uIPanel != null)
		{
			if ((uIPanel.mClipping == UIDrawCall.Clipping.None || uIPanel.mClipping == UIDrawCall.Clipping.ConstrainButDontClip) && !w.hideIfOffScreen)
			{
				uIPanel = uIPanel.mParentPanel;
				continue;
			}
			if (array == null)
			{
				array = w.worldCorners;
			}
			if (!uIPanel.IsVisible(array[0], array[1], array[2], array[3]))
			{
				return false;
			}
			uIPanel = uIPanel.mParentPanel;
		}
		return true;
	}

	public bool Affects(UIWidget w)
	{
		if (w == null)
		{
			return false;
		}
		UIPanel panel = w.panel;
		if (panel == null)
		{
			return false;
		}
		UIPanel uIPanel = this;
		while (uIPanel != null)
		{
			if (uIPanel == panel)
			{
				return true;
			}
			if (!uIPanel.hasCumulativeClipping)
			{
				return false;
			}
			uIPanel = uIPanel.mParentPanel;
		}
		return false;
	}

	[ContextMenu("Force Refresh")]
	public void RebuildAllDrawCalls()
	{
		mRebuild = true;
	}

	public void SetDirty()
	{
		int i = 0;
		for (int count = drawCalls.Count; i < count; i++)
		{
			drawCalls[i].isDirty = true;
		}
		Invalidate(includeChildren: true);
	}

	private void Awake()
	{
		mGo = base.gameObject;
		mTrans = base.transform;
		mHalfPixelOffset = (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.XBOX360 || Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.WindowsEditor);
		if (mHalfPixelOffset)
		{
			mHalfPixelOffset = (SystemInfo.graphicsShaderLevel < 40 && SystemInfo.graphicsDeviceVersion.Contains("Direct3D"));
		}
	}

	private void FindParent()
	{
		Transform parent = base.cachedTransform.parent;
		mParentPanel = ((!(parent != null)) ? null : NGUITools.FindInParents<UIPanel>(parent.gameObject));
	}

	public override void ParentHasChanged()
	{
		base.ParentHasChanged();
		FindParent();
	}

	protected override void OnStart()
	{
		mLayer = mGo.layer;
	}

	protected override void OnEnable()
	{
		mRebuild = true;
		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		OnStart();
		base.OnEnable();
		mMatrixFrame = -1;
	}

	protected override void OnInit()
	{
		base.OnInit();
		FindParent();
		if (GetComponent<Rigidbody>() == null && mParentPanel == null)
		{
			UICamera uICamera = (!(base.anchorCamera != null)) ? null : mCam.GetComponent<UICamera>();
			if (uICamera != null && (uICamera.eventType == UICamera.EventType.UI_3D || uICamera.eventType == UICamera.EventType.World_3D))
			{
				Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
			}
		}
		mRebuild = true;
		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		list.Add(this);
		list.Sort(CompareFunc);
	}

	protected override void OnDisable()
	{
		int i = 0;
		for (int count = drawCalls.Count; i < count; i++)
		{
			UIDrawCall uIDrawCall = drawCalls[i];
			if (uIDrawCall != null)
			{
				UIDrawCall.Destroy(uIDrawCall);
			}
		}
		drawCalls.Clear();
		list.Remove(this);
		mAlphaFrameID = -1;
		mMatrixFrame = -1;
		if (list.Count == 0)
		{
			UIDrawCall.ReleaseAll();
			mUpdateFrame = -1;
		}
		base.OnDisable();
	}

	private void UpdateTransformMatrix()
	{
		int frameCount = Time.frameCount;
		if (mMatrixFrame != frameCount)
		{
			mMatrixFrame = frameCount;
			worldToLocal = base.cachedTransform.worldToLocalMatrix;
			Vector2 vector = GetViewSize() * 0.5f;
			float num = mClipOffset.x + mClipRange.x;
			float num2 = mClipOffset.y + mClipRange.y;
			mMin.x = num - vector.x;
			mMin.y = num2 - vector.y;
			mMax.x = num + vector.x;
			mMax.y = num2 + vector.y;
		}
	}

	protected override void OnAnchor()
	{
		if (mClipping == UIDrawCall.Clipping.None)
		{
			return;
		}
		Transform cachedTransform = base.cachedTransform;
		Transform parent = cachedTransform.parent;
		Vector2 viewSize = GetViewSize();
		Vector2 vector = cachedTransform.localPosition;
		float num;
		float num2;
		float num3;
		float num4;
		if (leftAnchor.target == bottomAnchor.target && leftAnchor.target == rightAnchor.target && leftAnchor.target == topAnchor.target)
		{
			Vector3[] sides = leftAnchor.GetSides(parent);
			if (sides != null)
			{
				num = NGUIMath.Lerp(sides[0].x, sides[2].x, leftAnchor.relative) + (float)leftAnchor.absolute;
				num2 = NGUIMath.Lerp(sides[0].x, sides[2].x, rightAnchor.relative) + (float)rightAnchor.absolute;
				num3 = NGUIMath.Lerp(sides[3].y, sides[1].y, bottomAnchor.relative) + (float)bottomAnchor.absolute;
				num4 = NGUIMath.Lerp(sides[3].y, sides[1].y, topAnchor.relative) + (float)topAnchor.absolute;
			}
			else
			{
				Vector2 vector2 = GetLocalPos(leftAnchor, parent);
				num = vector2.x + (float)leftAnchor.absolute;
				num3 = vector2.y + (float)bottomAnchor.absolute;
				num2 = vector2.x + (float)rightAnchor.absolute;
				num4 = vector2.y + (float)topAnchor.absolute;
			}
		}
		else
		{
			if ((bool)leftAnchor.target)
			{
				Vector3[] sides2 = leftAnchor.GetSides(parent);
				if (sides2 != null)
				{
					num = NGUIMath.Lerp(sides2[0].x, sides2[2].x, leftAnchor.relative) + (float)leftAnchor.absolute;
				}
				else
				{
					Vector3 localPos = GetLocalPos(leftAnchor, parent);
					num = localPos.x + (float)leftAnchor.absolute;
				}
			}
			else
			{
				num = mClipRange.x - 0.5f * viewSize.x;
			}
			if ((bool)rightAnchor.target)
			{
				Vector3[] sides3 = rightAnchor.GetSides(parent);
				if (sides3 != null)
				{
					num2 = NGUIMath.Lerp(sides3[0].x, sides3[2].x, rightAnchor.relative) + (float)rightAnchor.absolute;
				}
				else
				{
					Vector3 localPos2 = GetLocalPos(rightAnchor, parent);
					num2 = localPos2.x + (float)rightAnchor.absolute;
				}
			}
			else
			{
				num2 = mClipRange.x + 0.5f * viewSize.x;
			}
			if ((bool)bottomAnchor.target)
			{
				Vector3[] sides4 = bottomAnchor.GetSides(parent);
				if (sides4 != null)
				{
					num3 = NGUIMath.Lerp(sides4[3].y, sides4[1].y, bottomAnchor.relative) + (float)bottomAnchor.absolute;
				}
				else
				{
					Vector3 localPos3 = GetLocalPos(bottomAnchor, parent);
					num3 = localPos3.y + (float)bottomAnchor.absolute;
				}
			}
			else
			{
				num3 = mClipRange.y - 0.5f * viewSize.y;
			}
			if ((bool)topAnchor.target)
			{
				Vector3[] sides5 = topAnchor.GetSides(parent);
				if (sides5 != null)
				{
					num4 = NGUIMath.Lerp(sides5[3].y, sides5[1].y, topAnchor.relative) + (float)topAnchor.absolute;
				}
				else
				{
					Vector3 localPos4 = GetLocalPos(topAnchor, parent);
					num4 = localPos4.y + (float)topAnchor.absolute;
				}
			}
			else
			{
				num4 = mClipRange.y + 0.5f * viewSize.y;
			}
		}
		num -= vector.x + mClipOffset.x;
		num2 -= vector.x + mClipOffset.x;
		num3 -= vector.y + mClipOffset.y;
		num4 -= vector.y + mClipOffset.y;
		float x = Mathf.Lerp(num, num2, 0.5f);
		float y = Mathf.Lerp(num3, num4, 0.5f);
		float num5 = num2 - num;
		float num6 = num4 - num3;
		float num7 = Mathf.Max(2f, mClipSoftness.x);
		float num8 = Mathf.Max(2f, mClipSoftness.y);
		if (num5 < num7)
		{
			num5 = num7;
		}
		if (num6 < num8)
		{
			num6 = num8;
		}
		baseClipRegion = new Vector4(x, y, num5, num6);
	}

	private void LateUpdate()
	{
		if (mUpdateFrame == Time.frameCount)
		{
			return;
		}
		mUpdateFrame = Time.frameCount;
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			list[i].UpdateSelf();
		}
		int num = 3000;
		int j = 0;
		for (int count2 = list.Count; j < count2; j++)
		{
			UIPanel uIPanel = list[j];
			if (uIPanel.renderQueue == RenderQueue.Automatic)
			{
				uIPanel.startingRenderQueue = num;
				uIPanel.UpdateDrawCalls();
				num += uIPanel.drawCalls.Count;
			}
			else if (uIPanel.renderQueue == RenderQueue.StartAt)
			{
				uIPanel.UpdateDrawCalls();
				if (uIPanel.drawCalls.Count != 0)
				{
					num = Mathf.Max(num, uIPanel.startingRenderQueue + uIPanel.drawCalls.Count);
				}
			}
			else
			{
				uIPanel.UpdateDrawCalls();
				if (uIPanel.drawCalls.Count != 0)
				{
					num = Mathf.Max(num, uIPanel.startingRenderQueue + 1);
				}
			}
		}
	}

	private void UpdateSelf()
	{
		mUpdateTime = RealTime.time;
		UpdateTransformMatrix();
		UpdateLayers();
		UpdateWidgets();
		if (mRebuild)
		{
			mRebuild = false;
			FillAllDrawCalls();
		}
		else
		{
			int num = 0;
			while (num < drawCalls.Count)
			{
				UIDrawCall uIDrawCall = drawCalls[num];
				if (uIDrawCall.isDirty && !FillDrawCall(uIDrawCall))
				{
					UIDrawCall.Destroy(uIDrawCall);
					drawCalls.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
		if (mUpdateScroll)
		{
			mUpdateScroll = false;
			UIScrollView component = GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars();
			}
		}
	}

	public void SortWidgets()
	{
		mSortWidgets = false;
		widgets.Sort(UIWidget.PanelCompareFunc);
	}

	private void FillAllDrawCalls()
	{
		for (int i = 0; i < drawCalls.Count; i++)
		{
			UIDrawCall.Destroy(drawCalls[i]);
		}
		drawCalls.Clear();
		Material material = null;
		Texture texture = null;
		Shader shader = null;
		UIDrawCall uIDrawCall = null;
		int num = 0;
		if (mSortWidgets)
		{
			SortWidgets();
		}
		for (int j = 0; j < widgets.Count; j++)
		{
			UIWidget uIWidget = widgets[j];
			if (uIWidget.isVisible && uIWidget.hasVertices)
			{
				Material material2 = uIWidget.material;
				Texture mainTexture = uIWidget.mainTexture;
				Shader shader2 = uIWidget.shader;
				if (material != material2 || texture != mainTexture || shader != shader2)
				{
					if (uIDrawCall != null && uIDrawCall.verts.size != 0)
					{
						drawCalls.Add(uIDrawCall);
						uIDrawCall.UpdateGeometry(num);
						uIDrawCall.onRender = mOnRender;
						mOnRender = null;
						num = 0;
						uIDrawCall = null;
					}
					material = material2;
					texture = mainTexture;
					shader = shader2;
				}
				if (!(material != null) && !(shader != null) && !(texture != null))
				{
					continue;
				}
				if (uIDrawCall == null)
				{
					uIDrawCall = UIDrawCall.Create(this, material, texture, shader);
					uIDrawCall.depthStart = uIWidget.depth;
					uIDrawCall.depthEnd = uIDrawCall.depthStart;
					uIDrawCall.panel = this;
				}
				else
				{
					int depth = uIWidget.depth;
					if (depth < uIDrawCall.depthStart)
					{
						uIDrawCall.depthStart = depth;
					}
					if (depth > uIDrawCall.depthEnd)
					{
						uIDrawCall.depthEnd = depth;
					}
				}
				uIWidget.drawCall = uIDrawCall;
				num++;
				if (generateNormals)
				{
					uIWidget.WriteToBuffers(uIDrawCall.verts, uIDrawCall.uvs, uIDrawCall.cols, uIDrawCall.norms, uIDrawCall.tans);
				}
				else
				{
					uIWidget.WriteToBuffers(uIDrawCall.verts, uIDrawCall.uvs, uIDrawCall.cols, null, null);
				}
				if (uIWidget.mOnRender != null)
				{
					if (mOnRender == null)
					{
						mOnRender = uIWidget.mOnRender;
					}
					else
					{
						mOnRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(mOnRender, uIWidget.mOnRender);
					}
				}
			}
			else
			{
				uIWidget.drawCall = null;
			}
		}
		if (uIDrawCall != null && uIDrawCall.verts.size != 0)
		{
			drawCalls.Add(uIDrawCall);
			uIDrawCall.UpdateGeometry(num);
			uIDrawCall.onRender = mOnRender;
			mOnRender = null;
		}
	}

	private bool FillDrawCall(UIDrawCall dc)
	{
		if (dc != null)
		{
			dc.isDirty = false;
			int num = 0;
			int num2 = 0;
			while (num2 < widgets.Count)
			{
				UIWidget uIWidget = widgets[num2];
				if (uIWidget == null)
				{
					widgets.RemoveAt(num2);
					continue;
				}
				if (uIWidget.drawCall == dc)
				{
					if (uIWidget.isVisible && uIWidget.hasVertices)
					{
						num++;
						if (generateNormals)
						{
							uIWidget.WriteToBuffers(dc.verts, dc.uvs, dc.cols, dc.norms, dc.tans);
						}
						else
						{
							uIWidget.WriteToBuffers(dc.verts, dc.uvs, dc.cols, null, null);
						}
						if (uIWidget.mOnRender != null)
						{
							if (mOnRender == null)
							{
								mOnRender = uIWidget.mOnRender;
							}
							else
							{
								mOnRender = (UIDrawCall.OnRenderCallback)Delegate.Combine(mOnRender, uIWidget.mOnRender);
							}
						}
					}
					else
					{
						uIWidget.drawCall = null;
					}
				}
				num2++;
			}
			if (dc.verts.size != 0)
			{
				dc.UpdateGeometry(num);
				dc.onRender = mOnRender;
				mOnRender = null;
				return true;
			}
		}
		return false;
	}

	private void UpdateDrawCalls()
	{
		Transform cachedTransform = base.cachedTransform;
		bool usedForUI = this.usedForUI;
		if (clipping != 0)
		{
			drawCallClipRange = finalClipRegion;
			drawCallClipRange.z *= 0.5f;
			drawCallClipRange.w *= 0.5f;
		}
		else
		{
			drawCallClipRange = Vector4.zero;
		}
		if (drawCallClipRange.z == 0f)
		{
			drawCallClipRange.z = (float)Screen.width * 0.5f;
		}
		if (drawCallClipRange.w == 0f)
		{
			drawCallClipRange.w = (float)Screen.height * 0.5f;
		}
		if (halfPixelOffset)
		{
			drawCallClipRange.x -= 0.5f;
			drawCallClipRange.y += 0.5f;
		}
		Vector3 vector;
		if (usedForUI)
		{
			Transform parent = base.cachedTransform.parent;
			vector = base.cachedTransform.localPosition;
			if (parent != null)
			{
				vector = parent.TransformPoint(vector);
			}
			vector += drawCallOffset;
		}
		else
		{
			vector = cachedTransform.position;
		}
		Quaternion rotation = cachedTransform.rotation;
		Vector3 lossyScale = cachedTransform.lossyScale;
		for (int i = 0; i < drawCalls.Count; i++)
		{
			UIDrawCall uIDrawCall = drawCalls[i];
			Transform cachedTransform2 = uIDrawCall.cachedTransform;
			cachedTransform2.position = vector;
			cachedTransform2.rotation = rotation;
			cachedTransform2.localScale = lossyScale;
			uIDrawCall.renderQueue = ((renderQueue != RenderQueue.Explicit) ? (startingRenderQueue + i) : startingRenderQueue);
			uIDrawCall.alwaysOnScreen = (alwaysOnScreen && (mClipping == UIDrawCall.Clipping.None || mClipping == UIDrawCall.Clipping.ConstrainButDontClip));
			uIDrawCall.sortingOrder = mSortingOrder;
			uIDrawCall.clipTexture = mClipTexture;
		}
	}

	private void UpdateLayers()
	{
		if (mLayer != base.cachedGameObject.layer)
		{
			mLayer = mGo.layer;
			NGUITools.SetChildLayer(base.cachedTransform, mLayer);
			ResetAnchors();
			for (int i = 0; i < drawCalls.Count; i++)
			{
				drawCalls[i].gameObject.layer = mLayer;
			}
		}
	}

	private void UpdateWidgets()
	{
		bool flag = !cullWhileDragging && mCullTime > mUpdateTime;
		bool flag2 = false;
		if (mForced != flag)
		{
			mForced = flag;
			mResized = true;
		}
		bool hasCumulativeClipping = this.hasCumulativeClipping;
		int i = 0;
		for (int count = widgets.Count; i < count; i++)
		{
			UIWidget uIWidget = widgets[i];
			if (!(uIWidget.panel == this) || !uIWidget.enabled)
			{
				continue;
			}
			int frameCount = Time.frameCount;
			if (uIWidget.UpdateTransform(frameCount) || mResized)
			{
				bool visibleByAlpha = flag || uIWidget.CalculateCumulativeAlpha(frameCount) > 0.001f;
				uIWidget.UpdateVisibility(visibleByAlpha, flag || (!hasCumulativeClipping && !uIWidget.hideIfOffScreen) || IsVisible(uIWidget));
			}
			if (!uIWidget.UpdateGeometry(frameCount))
			{
				continue;
			}
			flag2 = true;
			if (!mRebuild)
			{
				if (uIWidget.drawCall != null)
				{
					uIWidget.drawCall.isDirty = true;
				}
				else
				{
					FindDrawCall(uIWidget);
				}
			}
		}
		if (flag2 && onGeometryUpdated != null)
		{
			onGeometryUpdated();
		}
		mResized = false;
	}

	public UIDrawCall FindDrawCall(UIWidget w)
	{
		Material material = w.material;
		Texture mainTexture = w.mainTexture;
		int depth = w.depth;
		for (int i = 0; i < drawCalls.Count; i++)
		{
			UIDrawCall uIDrawCall = drawCalls[i];
			int num = (i != 0) ? (drawCalls[i - 1].depthEnd + 1) : int.MinValue;
			int num2 = (i + 1 != drawCalls.Count) ? (drawCalls[i + 1].depthStart - 1) : int.MaxValue;
			if (num > depth || num2 < depth)
			{
				continue;
			}
			if (uIDrawCall.baseMaterial == material && uIDrawCall.mainTexture == mainTexture)
			{
				if (w.isVisible)
				{
					w.drawCall = uIDrawCall;
					if (w.hasVertices)
					{
						uIDrawCall.isDirty = true;
					}
					return uIDrawCall;
				}
			}
			else
			{
				mRebuild = true;
			}
			return null;
		}
		mRebuild = true;
		return null;
	}

	public void AddWidget(UIWidget w)
	{
		mUpdateScroll = true;
		if (widgets.Count == 0)
		{
			widgets.Add(w);
		}
		else if (mSortWidgets)
		{
			widgets.Add(w);
			SortWidgets();
		}
		else if (UIWidget.PanelCompareFunc(w, widgets[0]) == -1)
		{
			widgets.Insert(0, w);
		}
		else
		{
			int num = widgets.Count;
			while (num > 0)
			{
				if (UIWidget.PanelCompareFunc(w, widgets[--num]) == -1)
				{
					continue;
				}
				widgets.Insert(num + 1, w);
				break;
			}
		}
		FindDrawCall(w);
	}

	public void RemoveWidget(UIWidget w)
	{
		if (widgets.Remove(w) && w.drawCall != null)
		{
			int depth = w.depth;
			if (depth == w.drawCall.depthStart || depth == w.drawCall.depthEnd)
			{
				mRebuild = true;
			}
			w.drawCall.isDirty = true;
			w.drawCall = null;
		}
	}

	public void Refresh()
	{
		mRebuild = true;
		mUpdateFrame = -1;
		if (list.Count > 0)
		{
			list[0].LateUpdate();
		}
	}

	public virtual Vector3 CalculateConstrainOffset(Vector2 min, Vector2 max)
	{
		Vector4 finalClipRegion = this.finalClipRegion;
		float num = finalClipRegion.z * 0.5f;
		float num2 = finalClipRegion.w * 0.5f;
		Vector2 minRect = new Vector2(min.x, min.y);
		Vector2 maxRect = new Vector2(max.x, max.y);
		Vector2 minArea = new Vector2(finalClipRegion.x - num, finalClipRegion.y - num2);
		Vector2 maxArea = new Vector2(finalClipRegion.x + num, finalClipRegion.y + num2);
		if (softBorderPadding && clipping == UIDrawCall.Clipping.SoftClip)
		{
			minArea.x += mClipSoftness.x;
			minArea.y += mClipSoftness.y;
			maxArea.x -= mClipSoftness.x;
			maxArea.y -= mClipSoftness.y;
		}
		return NGUIMath.ConstrainRect(minRect, maxRect, minArea, maxArea);
	}

	public bool ConstrainTargetToBounds(Transform target, ref Bounds targetBounds, bool immediate)
	{
		Vector3 vector = targetBounds.min;
		Vector3 vector2 = targetBounds.max;
		float num = 1f;
		if (mClipping == UIDrawCall.Clipping.None)
		{
			UIRoot root = base.root;
			if (root != null)
			{
				num = root.pixelSizeAdjustment;
			}
		}
		if (num != 1f)
		{
			vector /= num;
			vector2 /= num;
		}
		Vector3 vector3 = CalculateConstrainOffset(vector, vector2) * num;
		if (vector3.sqrMagnitude > 0f)
		{
			if (immediate)
			{
				target.localPosition += vector3;
				targetBounds.center += vector3;
				SpringPosition component = target.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else
			{
				SpringPosition springPosition = SpringPosition.Begin(target.gameObject, target.localPosition + vector3, 13f);
				springPosition.ignoreTimeScale = true;
				springPosition.worldSpace = false;
			}
			return true;
		}
		return false;
	}

	public bool ConstrainTargetToBounds(Transform target, bool immediate)
	{
		Bounds targetBounds = NGUIMath.CalculateRelativeWidgetBounds(base.cachedTransform, target);
		return ConstrainTargetToBounds(target, ref targetBounds, immediate);
	}

	public static UIPanel Find(Transform trans)
	{
		return Find(trans, /*createIfMissing:*/ false, -1);
	}

	public static UIPanel Find(Transform trans, bool createIfMissing)
	{
		return Find(trans, createIfMissing, -1);
	}

	public static UIPanel Find(Transform trans, bool createIfMissing, int layer)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(trans);
		if (uIPanel != null)
		{
			return uIPanel;
		}
		return (!createIfMissing) ? null : NGUITools.CreateUI(trans, /*advanced3D:*/ false, layer);
	}

	private Vector2 GetWindowSize()
	{
		UIRoot root = base.root;
		Vector2 vector = NGUITools.screenSize;
		if (root != null)
		{
			vector *= root.GetPixelSizeAdjustment(Mathf.RoundToInt(vector.y));
		}
		return vector;
	}

	public Vector2 GetViewSize()
	{
		if (mClipping != 0)
		{
			return new Vector2(mClipRange.z, mClipRange.w);
		}
		return NGUITools.screenSize;
	}
}
