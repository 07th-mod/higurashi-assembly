using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Popup List")]
[ExecuteInEditMode]
public class UIPopupList : UIWidgetContainer
{
	public enum Position
	{
		Auto,
		Above,
		Below
	}

	public enum OpenOn
	{
		ClickOrTap,
		RightClick,
		DoubleClick,
		Manual
	}

	public delegate void LegacyEvent(string val);

	public static UIPopupList current;

	private const float animSpeed = 0.15f;

	public UIAtlas atlas;

	public UIFont bitmapFont;

	public Font trueTypeFont;

	public int fontSize = 16;

	public FontStyle fontStyle;

	public string backgroundSprite;

	public string highlightSprite;

	public Position position;

	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	public List<string> items = new List<string>();

	public List<object> itemData = new List<object>();

	public Vector2 padding = new Vector3(4f, 4f);

	public Color textColor = Color.white;

	public Color backgroundColor = Color.white;

	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	public bool isAnimated = true;

	public bool isLocalized;

	public OpenOn openOn;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector]
	[SerializeField]
	private string mSelectedItem;

	private UIPanel mPanel;

	private GameObject mChild;

	private UISprite mBackground;

	private UISprite mHighlight;

	private UILabel mHighlightedLabel;

	private List<UILabel> mLabelList = new List<UILabel>();

	private float mBgBorder;

	[HideInInspector]
	[SerializeField]
	private GameObject eventReceiver;

	[HideInInspector]
	[SerializeField]
	private string functionName = "OnSelectionChange";

	[HideInInspector]
	[SerializeField]
	private float textScale;

	[HideInInspector]
	[SerializeField]
	private UIFont font;

	[HideInInspector]
	[SerializeField]
	private UILabel textLabel;

	private LegacyEvent mLegacyEvent;

	private bool mUseDynamicFont;

	private bool mTweening;

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (trueTypeFont != null)
			{
				return trueTypeFont;
			}
			if (bitmapFont != null)
			{
				return bitmapFont;
			}
			return font;
		}
		set
		{
			if (value is Font)
			{
				trueTypeFont = (value as Font);
				bitmapFont = null;
				font = null;
			}
			else if (value is UIFont)
			{
				bitmapFont = (value as UIFont);
				trueTypeFont = null;
				font = null;
			}
		}
	}

	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public LegacyEvent onSelectionChange
	{
		get
		{
			return mLegacyEvent;
		}
		set
		{
			mLegacyEvent = value;
		}
	}

	public bool isOpen => mChild != null;

	public string value
	{
		get
		{
			return mSelectedItem;
		}
		set
		{
			mSelectedItem = value;
			if (mSelectedItem != null && mSelectedItem != null)
			{
				TriggerCallbacks();
			}
		}
	}

	public object data
	{
		get
		{
			int num = items.IndexOf(mSelectedItem);
			return (num >= itemData.Count) ? null : itemData[num];
		}
	}

	[Obsolete("Use 'value' instead")]
	public string selection
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
		}
	}

	private bool handleEvents
	{
		get
		{
			UIKeyNavigation component = GetComponent<UIKeyNavigation>();
			return component == null || !component.enabled;
		}
		set
		{
			UIKeyNavigation component = GetComponent<UIKeyNavigation>();
			if (component != null)
			{
				component.enabled = !value;
			}
		}
	}

	private bool isValid => bitmapFont != null || trueTypeFont != null;

	private int activeFontSize => (!(trueTypeFont != null) && !(bitmapFont == null)) ? bitmapFont.defaultSize : fontSize;

	private float activeFontScale => (!(trueTypeFont != null) && !(bitmapFont == null)) ? ((float)fontSize / (float)bitmapFont.defaultSize) : 1f;

	public void Clear()
	{
		items.Clear();
		itemData.Clear();
	}

	public void AddItem(string text)
	{
		items.Add(text);
		itemData.Add(null);
	}

	public void AddItem(string text, object data)
	{
		items.Add(text);
		itemData.Add(data);
	}

	protected void TriggerCallbacks()
	{
		if (current != this)
		{
			UIPopupList uIPopupList = current;
			current = this;
			if (mLegacyEvent != null)
			{
				mLegacyEvent(mSelectedItem);
			}
			if (EventDelegate.IsValid(onChange))
			{
				EventDelegate.Execute(onChange);
			}
			else if (eventReceiver != null && !string.IsNullOrEmpty(functionName))
			{
				eventReceiver.SendMessage(functionName, mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			current = uIPopupList;
		}
	}

	private void OnEnable()
	{
		if (EventDelegate.IsValid(onChange))
		{
			eventReceiver = null;
			functionName = null;
		}
		if (font != null)
		{
			if (font.isDynamic)
			{
				trueTypeFont = font.dynamicFont;
				fontStyle = font.dynamicFontStyle;
				mUseDynamicFont = true;
			}
			else if (bitmapFont == null)
			{
				bitmapFont = font;
				mUseDynamicFont = false;
			}
			font = null;
		}
		if (textScale != 0f)
		{
			fontSize = ((!(bitmapFont != null)) ? 16 : Mathf.RoundToInt((float)bitmapFont.defaultSize * textScale));
			textScale = 0f;
		}
		if (trueTypeFont == null && bitmapFont != null && bitmapFont.isDynamic)
		{
			trueTypeFont = bitmapFont.dynamicFont;
			bitmapFont = null;
		}
	}

	private void OnValidate()
	{
		Font x = trueTypeFont;
		UIFont uIFont = bitmapFont;
		bitmapFont = null;
		trueTypeFont = null;
		if (x != null && (uIFont == null || !mUseDynamicFont))
		{
			bitmapFont = null;
			trueTypeFont = x;
			mUseDynamicFont = true;
		}
		else if (uIFont != null)
		{
			if (uIFont.isDynamic)
			{
				trueTypeFont = uIFont.dynamicFont;
				fontStyle = uIFont.dynamicFontStyle;
				fontSize = uIFont.defaultSize;
				mUseDynamicFont = true;
			}
			else
			{
				bitmapFont = uIFont;
				mUseDynamicFont = false;
			}
		}
		else
		{
			trueTypeFont = x;
			mUseDynamicFont = true;
		}
	}

	private void Start()
	{
		if (textLabel != null)
		{
			EventDelegate.Add(onChange, textLabel.SetCurrentSelection);
			textLabel = null;
		}
		if (Application.isPlaying)
		{
			if (string.IsNullOrEmpty(mSelectedItem))
			{
				if (items.Count > 0)
				{
					this.value = items[0];
				}
			}
			else
			{
				string value = mSelectedItem;
				mSelectedItem = null;
				this.value = value;
			}
		}
	}

	private void OnLocalize()
	{
		if (isLocalized)
		{
			TriggerCallbacks();
		}
	}

	private void Highlight(UILabel lbl, bool instant)
	{
		if (mHighlight != null)
		{
			mHighlightedLabel = lbl;
			UISpriteData atlasSprite = mHighlight.GetAtlasSprite();
			if (atlasSprite != null)
			{
				Vector3 highlightPosition = GetHighlightPosition();
				if (instant || !isAnimated)
				{
					mHighlight.cachedTransform.localPosition = highlightPosition;
				}
				else
				{
					TweenPosition.Begin(mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
					if (!mTweening)
					{
						mTweening = true;
						StartCoroutine(UpdateTweenPosition());
					}
				}
			}
		}
	}

	private Vector3 GetHighlightPosition()
	{
		if (mHighlightedLabel == null || mHighlight == null)
		{
			return Vector3.zero;
		}
		UISpriteData atlasSprite = mHighlight.GetAtlasSprite();
		if (atlasSprite == null)
		{
			return Vector3.zero;
		}
		float pixelSize = atlas.pixelSize;
		float num = (float)atlasSprite.borderLeft * pixelSize;
		float y = (float)atlasSprite.borderTop * pixelSize;
		return mHighlightedLabel.cachedTransform.localPosition + new Vector3(0f - num, y, 1f);
	}

	private IEnumerator UpdateTweenPosition()
	{
		if (this.mHighlight != null && this.mHighlightedLabel != null)
		{
			TweenPosition tp = this.mHighlight.GetComponent<TweenPosition>();
			while (tp != null && tp.enabled)
			{
				tp.to = this.GetHighlightPosition();
				yield return null;
			}
		}
		this.mTweening = false;
		yield break;
	}

	private void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			Highlight(component, instant: false);
		}
	}

	private void Select(UILabel lbl, bool instant)
	{
		Highlight(lbl, instant);
		UIEventListener component = lbl.gameObject.GetComponent<UIEventListener>();
		value = (component.parameter as string);
		UIPlaySound[] components = GetComponents<UIPlaySound>();
		int i = 0;
		for (int num = components.Length; i < num; i++)
		{
			UIPlaySound uIPlaySound = components[i];
			if (uIPlaySound.trigger == UIPlaySound.Trigger.OnClick)
			{
				NGUITools.PlaySound(uIPlaySound.audioClip, uIPlaySound.volume, 1f);
			}
		}
	}

	private void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			Select(go.GetComponent<UILabel>(), instant: true);
		}
	}

	private void OnItemClick(GameObject go)
	{
		Close();
	}

	private void OnKey(KeyCode key)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && handleEvents)
		{
			int num = mLabelList.IndexOf(mHighlightedLabel);
			if (num == -1)
			{
				num = 0;
			}
			switch (key)
			{
			case KeyCode.UpArrow:
				if (num > 0)
				{
					Select(mLabelList[--num], instant: false);
				}
				break;
			case KeyCode.DownArrow:
				if (num + 1 < mLabelList.Count)
				{
					Select(mLabelList[++num], instant: false);
				}
				break;
			case KeyCode.Escape:
				OnSelect(isSelected: false);
				break;
			}
		}
	}

	private void OnDisable()
	{
		Close();
	}

	private void OnSelect(bool isSelected)
	{
		if (!isSelected)
		{
			Close();
		}
	}

	public void Close()
	{
		if (mChild != null)
		{
			mLabelList.Clear();
			handleEvents = false;
			if (isAnimated)
			{
				UIWidget[] componentsInChildren = mChild.GetComponentsInChildren<UIWidget>();
				int i = 0;
				for (int num = componentsInChildren.Length; i < num; i++)
				{
					UIWidget uIWidget = componentsInChildren[i];
					Color color = uIWidget.color;
					color.a = 0f;
					TweenColor.Begin(uIWidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
				}
				Collider[] componentsInChildren2 = mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				for (int num2 = componentsInChildren2.Length; j < num2; j++)
				{
					componentsInChildren2[j].enabled = false;
				}
				UnityEngine.Object.Destroy(mChild, 0.15f);
			}
			else
			{
				UnityEngine.Object.Destroy(mChild);
			}
			mBackground = null;
			mHighlight = null;
			mChild = null;
		}
	}

	private void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	private void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	private void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject gameObject = widget.gameObject;
		Transform cachedTransform = widget.cachedTransform;
		float num = (float)activeFontSize * activeFontScale + mBgBorder * 2f;
		cachedTransform.localScale = new Vector3(1f, num / (float)widget.height, 1f);
		TweenScale.Begin(gameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)widget.height + num, localPosition.z);
			TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	private void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		AnimateColor(widget);
		AnimatePosition(widget, placeAbove, bottom);
	}

	private void OnClick()
	{
		if (openOn != OpenOn.DoubleClick && openOn != OpenOn.Manual && (openOn != OpenOn.RightClick || UICamera.currentTouchID == -2))
		{
			Show();
		}
	}

	private void OnDoubleClick()
	{
		if (openOn == OpenOn.DoubleClick)
		{
			Show();
		}
	}

	public void Show()
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && mChild == null && atlas != null && isValid && items.Count > 0)
		{
			mLabelList.Clear();
			if (mPanel == null)
			{
				mPanel = UIPanel.Find(base.transform);
				if (mPanel == null)
				{
					return;
				}
			}
			handleEvents = true;
			Transform transform = base.transform;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform.parent, transform);
			mChild = new GameObject("Drop-down List");
			mChild.layer = base.gameObject.layer;
			Transform transform2 = mChild.transform;
			transform2.parent = transform.parent;
			transform2.localPosition = bounds.min;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
			mBackground = NGUITools.AddSprite(mChild, atlas, backgroundSprite);
			mBackground.pivot = UIWidget.Pivot.TopLeft;
			mBackground.depth = NGUITools.CalculateNextDepth(mPanel.gameObject);
			mBackground.color = backgroundColor;
			Vector4 border = mBackground.border;
			mBgBorder = border.y;
			mBackground.cachedTransform.localPosition = new Vector3(0f, border.y, 0f);
			mHighlight = NGUITools.AddSprite(mChild, atlas, highlightSprite);
			mHighlight.pivot = UIWidget.Pivot.TopLeft;
			mHighlight.color = highlightColor;
			UISpriteData atlasSprite = mHighlight.GetAtlasSprite();
			if (atlasSprite != null)
			{
				float num = (float)atlasSprite.borderTop;
				float num2 = (float)activeFontSize;
				float activeFontScale = this.activeFontScale;
				float num3 = num2 * activeFontScale;
				float num4 = 0f;
				float num5 = 0f - padding.y;
				List<UILabel> list = new List<UILabel>();
				if (!items.Contains(mSelectedItem))
				{
					mSelectedItem = null;
				}
				int i = 0;
				for (int count = items.Count; i < count; i++)
				{
					string text = items[i];
					UILabel uILabel = NGUITools.AddWidget<UILabel>(mChild);
					uILabel.name = i.ToString();
					uILabel.pivot = UIWidget.Pivot.TopLeft;
					uILabel.bitmapFont = bitmapFont;
					uILabel.trueTypeFont = trueTypeFont;
					uILabel.fontSize = fontSize;
					uILabel.fontStyle = fontStyle;
					uILabel.text = ((!isLocalized) ? text : Localization.Get(text));
					uILabel.color = textColor;
					Transform cachedTransform = uILabel.cachedTransform;
					float num6 = border.x + padding.x;
					Vector2 pivotOffset = uILabel.pivotOffset;
					cachedTransform.localPosition = new Vector3(num6 - pivotOffset.x, num5, -1f);
					uILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
					uILabel.alignment = alignment;
					list.Add(uILabel);
					num5 -= num3;
					num5 -= padding.y;
					float a = num4;
					Vector2 printedSize = uILabel.printedSize;
					num4 = Mathf.Max(a, printedSize.x);
					UIEventListener uIEventListener = UIEventListener.Get(uILabel.gameObject);
					uIEventListener.onHover = OnItemHover;
					uIEventListener.onPress = OnItemPress;
					uIEventListener.onClick = OnItemClick;
					uIEventListener.parameter = text;
					if (mSelectedItem == text || (i == 0 && string.IsNullOrEmpty(mSelectedItem)))
					{
						Highlight(uILabel, instant: true);
					}
					mLabelList.Add(uILabel);
				}
				float a2 = num4;
				Vector3 size = bounds.size;
				num4 = Mathf.Max(a2, size.x * activeFontScale - (border.x + padding.x) * 2f);
				float num7 = num4;
				Vector3 vector = new Vector3(num7 * 0.5f, (0f - num2) * 0.5f, 0f);
				Vector3 vector2 = new Vector3(num7, num3 + padding.y, 1f);
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					UILabel uILabel2 = list[j];
					NGUITools.AddWidgetCollider(uILabel2.gameObject);
					uILabel2.autoResizeBoxCollider = false;
					BoxCollider component = uILabel2.GetComponent<BoxCollider>();
					if (component != null)
					{
						Vector3 center = component.center;
						vector.z = center.z;
						component.center = vector;
						component.size = vector2;
					}
					else
					{
						BoxCollider2D component2 = uILabel2.GetComponent<BoxCollider2D>();
						component2.offset = vector;
						component2.size = vector2;
					}
				}
				int width = Mathf.RoundToInt(num4);
				num4 += (border.x + padding.x) * 2f;
				num5 -= border.y;
				mBackground.width = Mathf.RoundToInt(num4);
				mBackground.height = Mathf.RoundToInt(0f - num5 + border.y);
				int k = 0;
				for (int count3 = list.Count; k < count3; k++)
				{
					UILabel uILabel3 = list[k];
					uILabel3.overflowMethod = UILabel.Overflow.ShrinkContent;
					uILabel3.width = width;
				}
				float num8 = 2f * atlas.pixelSize;
				float f = num4 - (border.x + padding.x) * 2f + (float)atlasSprite.borderLeft * num8;
				float f2 = num3 + num * num8;
				mHighlight.width = Mathf.RoundToInt(f);
				mHighlight.height = Mathf.RoundToInt(f2);
				bool flag = position == Position.Above;
				if (position == Position.Auto)
				{
					UICamera uICamera = UICamera.FindCameraForLayer(base.gameObject.layer);
					if (uICamera != null)
					{
						Vector3 vector3 = uICamera.cachedCamera.WorldToViewportPoint(transform.position);
						flag = (vector3.y < 0.5f);
					}
				}
				if (isAnimated)
				{
					float bottom = num5 + num3;
					Animate(mHighlight, flag, bottom);
					int l = 0;
					for (int count4 = list.Count; l < count4; l++)
					{
						Animate(list[l], flag, bottom);
					}
					AnimateColor(mBackground);
					AnimateScale(mBackground, flag, bottom);
				}
				if (flag)
				{
					Transform transform3 = transform2;
					Vector3 min = bounds.min;
					float x = min.x;
					Vector3 max = bounds.max;
					float y = max.y - num5 - border.y;
					Vector3 min2 = bounds.min;
					transform3.localPosition = new Vector3(x, y, min2.z);
				}
			}
		}
		else
		{
			OnSelect(isSelected: false);
		}
	}
}
