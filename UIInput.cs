using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard,
		AutoCorrect,
		Password
	}

	public enum Validation
	{
		None,
		Integer,
		Float,
		Alphanumeric,
		Username,
		Name
	}

	public enum KeyboardType
	{
		Default,
		ASCIICapable,
		NumbersAndPunctuation,
		URL,
		NumberPad,
		PhonePad,
		NamePhonePad,
		EmailAddress
	}

	public enum OnReturnKey
	{
		Default,
		Submit,
		NewLine
	}

	public delegate char OnValidate(string text, int charIndex, char addedChar);

	public static UIInput current;

	public static UIInput selection;

	public UILabel label;

	public InputType inputType;

	public OnReturnKey onReturnKey;

	public KeyboardType keyboardType;

	public bool hideInput;

	[NonSerialized]
	public bool selectAllTextOnFocus = true;

	public Validation validation;

	public int characterLimit;

	public string savedAs;

	[HideInInspector]
	[SerializeField]
	private GameObject selectOnTab;

	public Color activeTextColor = Color.white;

	public Color caretColor = new Color(1f, 1f, 1f, 0.8f);

	public Color selectionColor = new Color(1f, 223f / 255f, 47f / 85f, 0.5f);

	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public OnValidate onValidate;

	[HideInInspector]
	[SerializeField]
	protected string mValue;

	[NonSerialized]
	protected string mDefaultText = string.Empty;

	[NonSerialized]
	protected Color mDefaultColor = Color.white;

	[NonSerialized]
	protected float mPosition;

	[NonSerialized]
	protected bool mDoInit = true;

	[NonSerialized]
	protected UIWidget.Pivot mPivot;

	[NonSerialized]
	protected bool mLoadSavedValue = true;

	protected static int mDrawStart;

	protected static string mLastIME = string.Empty;

	[NonSerialized]
	protected int mSelectionStart;

	[NonSerialized]
	protected int mSelectionEnd;

	[NonSerialized]
	protected UITexture mHighlight;

	[NonSerialized]
	protected UITexture mCaret;

	[NonSerialized]
	protected Texture2D mBlankTex;

	[NonSerialized]
	protected float mNextBlink;

	[NonSerialized]
	protected float mLastAlpha;

	[NonSerialized]
	protected string mCached = string.Empty;

	[NonSerialized]
	protected int mSelectMe = -1;

	[NonSerialized]
	private UIInputOnGUI mOnGUI;

	public string defaultText
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mDefaultText;
		}
		set
		{
			if (mDoInit)
			{
				Init();
			}
			mDefaultText = value;
			UpdateLabel();
		}
	}

	public bool inputShouldBeHidden => hideInput && label != null && !label.multiLine && inputType != InputType.Password;

	[Obsolete("Use UIInput.value instead")]
	public string text
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

	public string value
	{
		get
		{
			if (mDoInit)
			{
				Init();
			}
			return mValue;
		}
		set
		{
			if (mDoInit)
			{
				Init();
			}
			mDrawStart = 0;
			if (Application.platform == RuntimePlatform.BlackBerryPlayer)
			{
				value = value.Replace("\\b", "\b");
			}
			value = Validate(value);
			if (!(mValue != value))
			{
				return;
			}
			mValue = value;
			mLoadSavedValue = false;
			if (isSelected)
			{
				if (string.IsNullOrEmpty(value))
				{
					mSelectionStart = 0;
					mSelectionEnd = 0;
				}
				else
				{
					mSelectionStart = value.Length;
					mSelectionEnd = mSelectionStart;
				}
			}
			else
			{
				SaveToPlayerPrefs(value);
			}
			UpdateLabel();
			ExecuteOnChange();
		}
	}

	[Obsolete("Use UIInput.isSelected instead")]
	public bool selected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
		}
	}

	public bool isSelected
	{
		get
		{
			return selection == this;
		}
		set
		{
			if (!value)
			{
				if (isSelected)
				{
					UICamera.selectedObject = null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public int cursorPosition
	{
		get
		{
			return (!isSelected) ? value.Length : mSelectionEnd;
		}
		set
		{
			if (isSelected)
			{
				mSelectionEnd = value;
				UpdateLabel();
			}
		}
	}

	public int selectionStart
	{
		get
		{
			return (!isSelected) ? value.Length : mSelectionStart;
		}
		set
		{
			if (isSelected)
			{
				mSelectionStart = value;
				UpdateLabel();
			}
		}
	}

	public int selectionEnd
	{
		get
		{
			return (!isSelected) ? value.Length : mSelectionEnd;
		}
		set
		{
			if (isSelected)
			{
				mSelectionEnd = value;
				UpdateLabel();
			}
		}
	}

	public UITexture caret => mCaret;

	public string Validate(string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(val.Length);
		for (int i = 0; i < val.Length; i++)
		{
			char c = val[i];
			if (onValidate != null)
			{
				c = onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			else if (validation != 0)
			{
				c = Validate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			if (c != 0)
			{
				stringBuilder.Append(c);
			}
		}
		if (characterLimit > 0 && stringBuilder.Length > characterLimit)
		{
			return stringBuilder.ToString(0, characterLimit);
		}
		return stringBuilder.ToString();
	}

	private void Start()
	{
		if (selectOnTab != null)
		{
			UIKeyNavigation component = GetComponent<UIKeyNavigation>();
			if (component == null)
			{
				component = base.gameObject.AddComponent<UIKeyNavigation>();
				component.onDown = selectOnTab;
			}
			selectOnTab = null;
			NGUITools.SetDirty(this);
		}
		if (mLoadSavedValue && !string.IsNullOrEmpty(savedAs))
		{
			LoadValue();
		}
		else
		{
			value = mValue.Replace("\\n", "\n");
		}
	}

	protected void Init()
	{
		if (mDoInit && label != null)
		{
			mDoInit = false;
			mDefaultText = label.text;
			mDefaultColor = label.color;
			label.supportEncoding = false;
			if (label.alignment == NGUIText.Alignment.Justified)
			{
				label.alignment = NGUIText.Alignment.Left;
				Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
			}
			mPivot = label.pivot;
			Vector3 localPosition = label.cachedTransform.localPosition;
			mPosition = localPosition.x;
			UpdateLabel();
		}
	}

	protected void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			if (string.IsNullOrEmpty(val))
			{
				PlayerPrefs.DeleteKey(savedAs);
			}
			else
			{
				PlayerPrefs.SetString(savedAs, val);
			}
		}
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			if (mOnGUI == null)
			{
				mOnGUI = base.gameObject.AddComponent<UIInputOnGUI>();
			}
			OnSelectEvent();
			return;
		}
		if (mOnGUI != null)
		{
			UnityEngine.Object.Destroy(mOnGUI);
			mOnGUI = null;
		}
		OnDeselectEvent();
	}

	protected void OnSelectEvent()
	{
		selection = this;
		if (mDoInit)
		{
			Init();
		}
		if (label != null && NGUITools.GetActive(this))
		{
			mSelectMe = Time.frameCount;
		}
	}

	protected void OnDeselectEvent()
	{
		if (mDoInit)
		{
			Init();
		}
		if (label != null && NGUITools.GetActive(this))
		{
			mValue = value;
			if (string.IsNullOrEmpty(mValue))
			{
				label.text = mDefaultText;
				label.color = mDefaultColor;
			}
			else
			{
				label.text = mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			RestoreLabelPivot();
		}
		selection = null;
		UpdateLabel();
	}

	protected virtual void Update()
	{
		if (!isSelected)
		{
			return;
		}
		if (mDoInit)
		{
			Init();
		}
		if (mSelectMe != -1 && mSelectMe != Time.frameCount)
		{
			mSelectMe = -1;
			mSelectionEnd = ((!string.IsNullOrEmpty(mValue)) ? mValue.Length : 0);
			mDrawStart = 0;
			mSelectionStart = ((!selectAllTextOnFocus) ? mSelectionEnd : 0);
			label.color = activeTextColor;
			Vector2 compositionCursorPos = (!(UICamera.current != null) || !(UICamera.current.cachedCamera != null)) ? label.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(label.worldCorners[0]);
			compositionCursorPos.y = (float)Screen.height - compositionCursorPos.y;
			Input.imeCompositionMode = IMECompositionMode.On;
			Input.compositionCursorPos = compositionCursorPos;
			UpdateLabel();
			if (string.IsNullOrEmpty(Input.inputString))
			{
				return;
			}
		}
		string compositionString = Input.compositionString;
		if (string.IsNullOrEmpty(compositionString) && !string.IsNullOrEmpty(Input.inputString))
		{
			string inputString = Input.inputString;
			for (int i = 0; i < inputString.Length; i++)
			{
				char c = inputString[i];
				if (c >= ' ' && c != '\uf700' && c != '\uf701' && c != '\uf702' && c != '\uf703')
				{
					Insert(c.ToString());
				}
			}
		}
		if (mLastIME != compositionString)
		{
			mSelectionEnd = ((!string.IsNullOrEmpty(compositionString)) ? (mValue.Length + compositionString.Length) : mSelectionStart);
			mLastIME = compositionString;
			UpdateLabel();
			ExecuteOnChange();
		}
		if (mCaret != null && mNextBlink < RealTime.time)
		{
			mNextBlink = RealTime.time + 0.5f;
			mCaret.enabled = !mCaret.enabled;
		}
		if (isSelected && mLastAlpha != label.finalAlpha)
		{
			UpdateLabel();
		}
	}

	protected void DoBackspace()
	{
		if (string.IsNullOrEmpty(mValue))
		{
			return;
		}
		if (mSelectionStart == mSelectionEnd)
		{
			if (mSelectionStart < 1)
			{
				return;
			}
			mSelectionEnd--;
		}
		Insert(string.Empty);
	}

	public virtual bool ProcessEvent(Event ev)
	{
		if (label == null)
		{
			return false;
		}
		RuntimePlatform platform = Application.platform;
		bool flag = (platform != 0 && platform != RuntimePlatform.OSXPlayer) ? ((ev.modifiers & EventModifiers.Control) != 0) : ((ev.modifiers & EventModifiers.Command) != 0);
		if ((ev.modifiers & EventModifiers.Alt) != 0)
		{
			flag = false;
		}
		bool flag2 = (ev.modifiers & EventModifiers.Shift) != EventModifiers.None;
		switch (ev.keyCode)
		{
		case KeyCode.Backspace:
			ev.Use();
			DoBackspace();
			return true;
		case KeyCode.Delete:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				if (mSelectionStart == mSelectionEnd)
				{
					if (mSelectionStart >= mValue.Length)
					{
						return true;
					}
					mSelectionEnd++;
				}
				Insert(string.Empty);
			}
			return true;
		case KeyCode.LeftArrow:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				mSelectionEnd = Mathf.Max(mSelectionEnd - 1, 0);
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.RightArrow:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				mSelectionEnd = Mathf.Min(mSelectionEnd + 1, mValue.Length);
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.PageUp:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				mSelectionEnd = 0;
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.PageDown:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				mSelectionEnd = mValue.Length;
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.Home:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				if (label.multiLine)
				{
					mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.Home);
				}
				else
				{
					mSelectionEnd = 0;
				}
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.End:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				if (label.multiLine)
				{
					mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.End);
				}
				else
				{
					mSelectionEnd = mValue.Length;
				}
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.UpArrow:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.UpArrow);
				if (mSelectionEnd != 0)
				{
					mSelectionEnd += mDrawStart;
				}
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.DownArrow:
			ev.Use();
			if (!string.IsNullOrEmpty(mValue))
			{
				mSelectionEnd = label.GetCharacterIndex(mSelectionEnd, KeyCode.DownArrow);
				if (mSelectionEnd != label.processedText.Length)
				{
					mSelectionEnd += mDrawStart;
				}
				else
				{
					mSelectionEnd = mValue.Length;
				}
				if (!flag2)
				{
					mSelectionStart = mSelectionEnd;
				}
				UpdateLabel();
			}
			return true;
		case KeyCode.A:
			if (flag)
			{
				ev.Use();
				mSelectionStart = 0;
				mSelectionEnd = mValue.Length;
				UpdateLabel();
			}
			return true;
		case KeyCode.C:
			if (flag)
			{
				ev.Use();
				NGUITools.clipboard = GetSelection();
			}
			return true;
		case KeyCode.V:
			if (flag)
			{
				ev.Use();
				Insert(NGUITools.clipboard);
			}
			return true;
		case KeyCode.X:
			if (flag)
			{
				ev.Use();
				NGUITools.clipboard = GetSelection();
				Insert(string.Empty);
			}
			return true;
		case KeyCode.Return:
		case KeyCode.KeypadEnter:
			ev.Use();
			if (onReturnKey == OnReturnKey.NewLine || (onReturnKey == OnReturnKey.Default && label.multiLine && !flag && label.overflowMethod != UILabel.Overflow.ClampContent && validation == Validation.None))
			{
				Insert("\n");
			}
			else
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentKey = ev.keyCode;
				Submit();
				UICamera.currentKey = KeyCode.None;
			}
			return true;
		default:
			return false;
		}
	}

	protected virtual void Insert(string text)
	{
		string leftText = GetLeftText();
		string rightText = GetRightText();
		int length = rightText.Length;
		StringBuilder stringBuilder = new StringBuilder(leftText.Length + rightText.Length + text.Length);
		stringBuilder.Append(leftText);
		int i = 0;
		for (int length2 = text.Length; i < length2; i++)
		{
			char c = text[i];
			if (c == '\b')
			{
				DoBackspace();
				continue;
			}
			if (characterLimit > 0 && stringBuilder.Length + length >= characterLimit)
			{
				break;
			}
			if (onValidate != null)
			{
				c = onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			else if (validation != 0)
			{
				c = Validate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			if (c != 0)
			{
				stringBuilder.Append(c);
			}
		}
		mSelectionStart = stringBuilder.Length;
		mSelectionEnd = mSelectionStart;
		int j = 0;
		for (int length3 = rightText.Length; j < length3; j++)
		{
			char c2 = rightText[j];
			if (onValidate != null)
			{
				c2 = onValidate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			else if (validation != 0)
			{
				c2 = Validate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			if (c2 != 0)
			{
				stringBuilder.Append(c2);
			}
		}
		mValue = stringBuilder.ToString();
		UpdateLabel();
		ExecuteOnChange();
	}

	protected string GetLeftText()
	{
		int num = Mathf.Min(mSelectionStart, mSelectionEnd);
		return (!string.IsNullOrEmpty(mValue) && num >= 0) ? mValue.Substring(0, num) : string.Empty;
	}

	protected string GetRightText()
	{
		int num = Mathf.Max(mSelectionStart, mSelectionEnd);
		return (!string.IsNullOrEmpty(mValue) && num < mValue.Length) ? mValue.Substring(num) : string.Empty;
	}

	protected string GetSelection()
	{
		if (string.IsNullOrEmpty(mValue) || mSelectionStart == mSelectionEnd)
		{
			return string.Empty;
		}
		int num = Mathf.Min(mSelectionStart, mSelectionEnd);
		int num2 = Mathf.Max(mSelectionStart, mSelectionEnd);
		return mValue.Substring(num, num2 - num);
	}

	protected int GetCharUnderMouse()
	{
		Vector3[] worldCorners = label.worldCorners;
		Ray currentRay = UICamera.currentRay;
		float enter;
		return new Plane(worldCorners[0], worldCorners[1], worldCorners[2]).Raycast(currentRay, out enter) ? (mDrawStart + label.GetCharacterIndexAtPosition(currentRay.GetPoint(enter), precise: false)) : 0;
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (isPressed && isSelected && label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			selectionEnd = GetCharUnderMouse();
			if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				selectionStart = mSelectionEnd;
			}
		}
	}

	protected virtual void OnDrag(Vector2 delta)
	{
		if (label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			selectionEnd = GetCharUnderMouse();
		}
	}

	private void OnDisable()
	{
		Cleanup();
	}

	protected virtual void Cleanup()
	{
		if ((bool)mHighlight)
		{
			mHighlight.enabled = false;
		}
		if ((bool)mCaret)
		{
			mCaret.enabled = false;
		}
		if ((bool)mBlankTex)
		{
			NGUITools.Destroy(mBlankTex);
			mBlankTex = null;
		}
	}

	public void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			mValue = value;
			if (current == null)
			{
				current = this;
				EventDelegate.Execute(onSubmit);
				current = null;
			}
			SaveToPlayerPrefs(mValue);
		}
	}

	public void UpdateLabel()
	{
		if (!(label != null))
		{
			return;
		}
		if (mDoInit)
		{
			Init();
		}
		bool isSelected = this.isSelected;
		string value = this.value;
		bool flag = string.IsNullOrEmpty(value) && string.IsNullOrEmpty(Input.compositionString);
		label.color = ((!flag || isSelected) ? activeTextColor : mDefaultColor);
		string text;
		if (flag)
		{
			text = ((!isSelected) ? mDefaultText : string.Empty);
			RestoreLabelPivot();
		}
		else
		{
			if (inputType == InputType.Password)
			{
				text = string.Empty;
				string str = "*";
				if (label.bitmapFont != null && label.bitmapFont.bmFont != null && label.bitmapFont.bmFont.GetGlyph(42) == null)
				{
					str = "x";
				}
				int i = 0;
				for (int length = value.Length; i < length; i++)
				{
					text += str;
				}
			}
			else
			{
				text = value;
			}
			int num = isSelected ? Mathf.Min(text.Length, cursorPosition) : 0;
			string str2 = text.Substring(0, num);
			if (isSelected)
			{
				str2 += Input.compositionString;
			}
			text = str2 + text.Substring(num, text.Length - num);
			if (isSelected && label.overflowMethod == UILabel.Overflow.ClampContent && label.maxLineCount == 1)
			{
				int num2 = label.CalculateOffsetToFit(text);
				if (num2 == 0)
				{
					mDrawStart = 0;
					RestoreLabelPivot();
				}
				else if (num < mDrawStart)
				{
					mDrawStart = num;
					SetPivotToLeft();
				}
				else if (num2 < mDrawStart)
				{
					mDrawStart = num2;
					SetPivotToLeft();
				}
				else
				{
					num2 = label.CalculateOffsetToFit(text.Substring(0, num));
					if (num2 > mDrawStart)
					{
						mDrawStart = num2;
						SetPivotToRight();
					}
				}
				if (mDrawStart != 0)
				{
					text = text.Substring(mDrawStart, text.Length - mDrawStart);
				}
			}
			else
			{
				mDrawStart = 0;
				RestoreLabelPivot();
			}
		}
		label.text = text;
		if (isSelected)
		{
			int num3 = mSelectionStart - mDrawStart;
			int num4 = mSelectionEnd - mDrawStart;
			if (mBlankTex == null)
			{
				mBlankTex = new Texture2D(2, 2, TextureFormat.ARGB32, mipmap: false);
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						mBlankTex.SetPixel(k, j, Color.white);
					}
				}
				mBlankTex.Apply();
			}
			if (num3 != num4)
			{
				if (mHighlight == null)
				{
					mHighlight = NGUITools.AddWidget<UITexture>(label.cachedGameObject);
					mHighlight.name = "Input Highlight";
					mHighlight.mainTexture = mBlankTex;
					mHighlight.fillGeometry = false;
					mHighlight.pivot = label.pivot;
					mHighlight.SetAnchor(label.cachedTransform);
				}
				else
				{
					mHighlight.pivot = label.pivot;
					mHighlight.mainTexture = mBlankTex;
					mHighlight.MarkAsChanged();
					mHighlight.enabled = true;
				}
			}
			if (mCaret == null)
			{
				mCaret = NGUITools.AddWidget<UITexture>(label.cachedGameObject);
				mCaret.name = "Input Caret";
				mCaret.mainTexture = mBlankTex;
				mCaret.fillGeometry = false;
				mCaret.pivot = label.pivot;
				mCaret.SetAnchor(label.cachedTransform);
			}
			else
			{
				mCaret.pivot = label.pivot;
				mCaret.mainTexture = mBlankTex;
				mCaret.MarkAsChanged();
				mCaret.enabled = true;
			}
			if (num3 != num4)
			{
				label.PrintOverlay(num3, num4, mCaret.geometry, mHighlight.geometry, caretColor, selectionColor);
				mHighlight.enabled = mHighlight.geometry.hasVertices;
			}
			else
			{
				label.PrintOverlay(num3, num4, mCaret.geometry, null, caretColor, selectionColor);
				if (mHighlight != null)
				{
					mHighlight.enabled = false;
				}
			}
			mNextBlink = RealTime.time + 0.5f;
			mLastAlpha = label.finalAlpha;
		}
		else
		{
			Cleanup();
		}
	}

	protected void SetPivotToLeft()
	{
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(mPivot);
		pivotOffset.x = 0f;
		label.pivot = NGUIMath.GetPivot(pivotOffset);
	}

	protected void SetPivotToRight()
	{
		Vector2 pivotOffset = NGUIMath.GetPivotOffset(mPivot);
		pivotOffset.x = 1f;
		label.pivot = NGUIMath.GetPivot(pivotOffset);
	}

	protected void RestoreLabelPivot()
	{
		if (label != null && label.pivot != mPivot)
		{
			label.pivot = mPivot;
		}
	}

	protected char Validate(string text, int pos, char ch)
	{
		if (validation == Validation.None || !base.enabled)
		{
			return ch;
		}
		if (validation == Validation.Integer)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
		}
		else if (validation == Validation.Float)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
			if (ch == '.' && !text.Contains("."))
			{
				return ch;
			}
		}
		else if (validation == Validation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (validation == Validation.Username)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return (char)(ch - 65 + 97);
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (validation == Validation.Name)
		{
			char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
			char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
			if (ch >= 'a' && ch <= 'z')
			{
				if (c == ' ')
				{
					return (char)(ch - 97 + 65);
				}
				return ch;
			}
			if (ch >= 'A' && ch <= 'Z')
			{
				if (c != ' ' && c != '\'')
				{
					return (char)(ch - 65 + 97);
				}
				return ch;
			}
			switch (ch)
			{
			case '\'':
				if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
				{
					return ch;
				}
				break;
			case ' ':
				if (c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
				{
					return ch;
				}
				break;
			}
		}
		return '\0';
	}

	protected void ExecuteOnChange()
	{
		if (current == null && EventDelegate.IsValid(onChange))
		{
			current = this;
			EventDelegate.Execute(onChange);
			current = null;
		}
	}

	public void RemoveFocus()
	{
		isSelected = false;
	}

	public void SaveValue()
	{
		SaveToPlayerPrefs(mValue);
	}

	public void LoadValue()
	{
		if (!string.IsNullOrEmpty(savedAs))
		{
			string text = mValue.Replace("\\n", "\n");
			mValue = string.Empty;
			value = ((!PlayerPrefs.HasKey(savedAs)) ? text : PlayerPrefs.GetString(savedAs));
		}
	}
}
