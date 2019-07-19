using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Table")]
public class UITable : UIWidgetContainer
{
	public delegate void OnReposition();

	public enum Direction
	{
		Down,
		Up
	}

	public enum Sorting
	{
		None,
		Alphabetic,
		Horizontal,
		Vertical,
		Custom
	}

	public int columns;

	public Direction direction;

	public Sorting sorting;

	public UIWidget.Pivot pivot;

	public UIWidget.Pivot cellAlignment;

	public bool hideInactive = true;

	public bool keepWithinPanel;

	public Vector2 padding = Vector2.zero;

	public OnReposition onReposition;

	public Comparison<Transform> onCustomSort;

	protected UIPanel mPanel;

	protected bool mInitDone;

	protected bool mReposition;

	[CompilerGenerated]
	private static Comparison<Transform> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Comparison<Transform> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Comparison<Transform> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Comparison<Transform> _003C_003Ef__mg_0024cache3;

	public bool repositionNow
	{
		set
		{
			if (value)
			{
				mReposition = true;
				base.enabled = true;
			}
		}
	}

	public List<Transform> GetChildList()
	{
		Transform transform = base.transform;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!hideInactive || ((bool)child && NGUITools.GetActive(child.gameObject)))
			{
				list.Add(child);
			}
		}
		if (sorting != 0)
		{
			if (sorting == Sorting.Alphabetic)
			{
				list.Sort(UIGrid.SortByName);
			}
			else if (sorting == Sorting.Horizontal)
			{
				list.Sort(UIGrid.SortHorizontal);
			}
			else if (sorting == Sorting.Vertical)
			{
				list.Sort(UIGrid.SortVertical);
			}
			else if (onCustomSort != null)
			{
				list.Sort(onCustomSort);
			}
			else
			{
				Sort(list);
			}
		}
		return list;
	}

	protected virtual void Sort(List<Transform> list)
	{
		list.Sort(UIGrid.SortByName);
	}

	protected virtual void Start()
	{
		Init();
		Reposition();
		base.enabled = false;
	}

	protected virtual void Init()
	{
		mInitDone = true;
		mPanel = NGUITools.FindInParents<UIPanel>(base.gameObject);
	}

	protected virtual void LateUpdate()
	{
		if (mReposition)
		{
			Reposition();
		}
		base.enabled = false;
	}

	private void OnValidate()
	{
		if (!Application.isPlaying && NGUITools.GetActive(this))
		{
			Reposition();
		}
	}

	protected void RepositionVariableSize(List<Transform> children)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = (columns <= 0) ? 1 : (children.Count / columns + 1);
		int num4 = (columns <= 0) ? children.Count : columns;
		Bounds[,] array = new Bounds[num3, num4];
		Bounds[] array2 = new Bounds[num4];
		Bounds[] array3 = new Bounds[num3];
		int num5 = 0;
		int num6 = 0;
		int i = 0;
		for (int count = children.Count; i < count; i++)
		{
			Transform transform = children[i];
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform, !hideInactive);
			Vector3 localScale = transform.localScale;
			bounds.min = Vector3.Scale(bounds.min, localScale);
			bounds.max = Vector3.Scale(bounds.max, localScale);
			array[num6, num5] = bounds;
			array2[num5].Encapsulate(bounds);
			array3[num6].Encapsulate(bounds);
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
			}
		}
		num5 = 0;
		num6 = 0;
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(cellAlignment);
		int j = 0;
		for (int count2 = children.Count; j < count2; j++)
		{
			Transform transform2 = children[j];
			Bounds bounds2 = array[num6, num5];
			Bounds bounds3 = array2[num5];
			Bounds bounds4 = array3[num6];
			Vector3 localPosition = transform2.localPosition;
			float num7 = num;
			Vector3 extents = bounds2.extents;
			float num8 = num7 + extents.x;
			Vector3 center = bounds2.center;
			localPosition.x = num8 - center.x;
			float x = localPosition.x;
			Vector3 max = bounds2.max;
			float x2 = max.x;
			Vector3 min = bounds2.min;
			float num9 = x2 - min.x;
			Vector3 max2 = bounds3.max;
			float num10 = num9 - max2.x;
			Vector3 min2 = bounds3.min;
			localPosition.x = x - (Mathf.Lerp(0f, num10 + min2.x, pivotOffset.x) - padding.x);
			if (direction == Direction.Down)
			{
				float num11 = 0f - num2;
				Vector3 extents2 = bounds2.extents;
				float num12 = num11 - extents2.y;
				Vector3 center2 = bounds2.center;
				localPosition.y = num12 - center2.y;
				float y = localPosition.y;
				Vector3 max3 = bounds2.max;
				float y2 = max3.y;
				Vector3 min3 = bounds2.min;
				float num13 = y2 - min3.y;
				Vector3 max4 = bounds4.max;
				float num14 = num13 - max4.y;
				Vector3 min4 = bounds4.min;
				localPosition.y = y + (Mathf.Lerp(num14 + min4.y, 0f, pivotOffset.y) - padding.y);
			}
			else
			{
				float num15 = num2;
				Vector3 extents3 = bounds2.extents;
				float num16 = num15 + extents3.y;
				Vector3 center3 = bounds2.center;
				localPosition.y = num16 - center3.y;
				float y3 = localPosition.y;
				Vector3 max5 = bounds2.max;
				float y4 = max5.y;
				Vector3 min5 = bounds2.min;
				float num17 = y4 - min5.y;
				Vector3 max6 = bounds4.max;
				float num18 = num17 - max6.y;
				Vector3 min6 = bounds4.min;
				localPosition.y = y3 - (Mathf.Lerp(0f, num18 + min6.y, pivotOffset.y) - padding.y);
			}
			float num19 = num;
			Vector3 size = bounds3.size;
			num = num19 + (size.x + padding.x * 2f);
			transform2.localPosition = localPosition;
			if (++num5 >= columns && columns > 0)
			{
				num5 = 0;
				num6++;
				num = 0f;
				float num20 = num2;
				Vector3 size2 = bounds4.size;
				num2 = num20 + (size2.y + padding.y * 2f);
			}
		}
		if (pivot == UIWidget.Pivot.TopLeft)
		{
			return;
		}
		pivotOffset = NGUIMath.GetPivotOffset(pivot);
		Bounds bounds5 = NGUIMath.CalculateRelativeWidgetBounds(base.transform);
		Vector3 size3 = bounds5.size;
		float num21 = Mathf.Lerp(0f, size3.x, pivotOffset.x);
		Vector3 size4 = bounds5.size;
		float num22 = Mathf.Lerp(0f - size4.y, 0f, pivotOffset.y);
		Transform transform3 = base.transform;
		for (int k = 0; k < transform3.childCount; k++)
		{
			Transform child = transform3.GetChild(k);
			SpringPosition component = child.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.target.x -= num21;
				component.target.y -= num22;
				continue;
			}
			Vector3 localPosition2 = child.localPosition;
			localPosition2.x -= num21;
			localPosition2.y -= num22;
			child.localPosition = localPosition2;
		}
	}

	[ContextMenu("Execute")]
	public virtual void Reposition()
	{
		if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
		{
			Init();
		}
		mReposition = false;
		Transform transform = base.transform;
		List<Transform> childList = GetChildList();
		if (childList.Count > 0)
		{
			RepositionVariableSize(childList);
		}
		if (keepWithinPanel && mPanel != null)
		{
			mPanel.ConstrainTargetToBounds(transform, immediate: true);
			UIScrollView component = mPanel.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.UpdateScrollbars(recalculateBounds: true);
			}
		}
		if (onReposition != null)
		{
			onReposition();
		}
	}
}
