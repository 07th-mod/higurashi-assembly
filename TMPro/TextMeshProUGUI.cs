using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI/TextMeshPro Text", 12)]
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[RequireComponent(typeof(CanvasRenderer))]
	[RequireComponent(typeof(RectTransform))]
	public class TextMeshProUGUI : MaskableGraphic, ILayoutElement
	{
		private enum TextInputSources
		{
			Text,
			SetText,
			SetCharArray
		}

		private enum AutoLayoutPhase
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private string m_text;

		[SerializeField]
		private TextMeshProFont m_fontAsset;

		private TextMeshProFont m_currentFontAsset;

		[SerializeField]
		private Material m_fontMaterial;

		private Material m_currentMaterial;

		[SerializeField]
		private Material m_sharedMaterial;

		[SerializeField]
		private FontStyles m_fontStyle;

		private FontStyles m_style;

		private bool m_isOverlay;

		[SerializeField]
		private Color32 m_fontColor32 = Color.white;

		[SerializeField]
		private Color m_fontColor = Color.white;

		[SerializeField]
		private bool m_enableVertexGradient;

		[SerializeField]
		private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		[SerializeField]
		private Color32 m_faceColor = Color.white;

		[SerializeField]
		private Color32 m_outlineColor = Color.black;

		private float m_outlineWidth;

		[SerializeField]
		private float m_fontSize = 36f;

		[SerializeField]
		private float m_fontSizeMin;

		[SerializeField]
		private float m_fontSizeMax;

		[SerializeField]
		private float m_fontSizeBase = 36f;

		[SerializeField]
		private float m_lineSpacingMax;

		[SerializeField]
		private float m_charWidthMaxAdj;

		private float m_charWidthAdjDelta;

		private float m_currentFontSize;

		[SerializeField]
		private float m_characterSpacing;

		private float m_cSpacing;

		private float m_monoSpacing;

		[SerializeField]
		private float m_lineSpacing;

		private float m_lineSpacingDelta;

		private float m_lineHeight;

		[SerializeField]
		private float m_paragraphSpacing;

		[FormerlySerializedAs("m_lineJustification")]
		[SerializeField]
		private TextAlignmentOptions m_textAlignment;

		private TextAlignmentOptions m_lineJustification;

		[SerializeField]
		private bool m_enableKerning;

		[SerializeField]
		private bool m_overrideHtmlColors;

		[SerializeField]
		private bool m_enableExtraPadding;

		[SerializeField]
		private bool checkPaddingRequired;

		[SerializeField]
		private bool m_enableWordWrapping;

		private bool m_isCharacterWrappingEnabled;

		private bool m_isNonBreakingSpace;

		private bool m_isIgnoringAlignment;

		[SerializeField]
		private TextOverflowModes m_overflowMode;

		[SerializeField]
		private float m_wordWrappingRatios = 0.4f;

		[SerializeField]
		private TextureMappingOptions m_horizontalMapping;

		[SerializeField]
		private TextureMappingOptions m_verticalMapping;

		[SerializeField]
		private Vector2 m_uvOffset = Vector2.zero;

		[SerializeField]
		private float m_uvLineOffset;

		[SerializeField]
		private bool isInputParsingRequired;

		[SerializeField]
		private bool m_havePropertiesChanged;

		[SerializeField]
		private bool hasFontAssetChanged;

		[SerializeField]
		private bool m_isRichText = true;

		[SerializeField]
		private bool m_parseCtrlCharacters = true;

		[SerializeField]
		private TextInputSources m_inputSource;

		private string old_text;

		private float old_arg0;

		private float old_arg1;

		private float old_arg2;

		private int m_fontIndex;

		private float m_fontScale;

		private bool m_isRecalculateScaleRequired;

		private Vector3 m_previousLossyScale;

		private float m_xAdvance;

		private float m_maxXAdvance;

		private float tag_LineIndent;

		private float tag_Indent;

		private bool tag_NoParsing;

		private Vector3 m_anchorOffset;

		private TMP_TextInfo m_textInfo;

		private char[] m_htmlTag = new char[64];

		private CanvasRenderer m_uiRenderer;

		private Canvas m_canvas;

		private RectTransform m_rectTransform;

		private Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		private Color32[] m_colorStack = new Color32[32];

		private int m_colorStackIndex;

		private float m_tabSpacing;

		private float m_spacing;

		private float m_baselineOffset;

		private float m_padding;

		private bool m_isUsingBold;

		private Vector2 k_InfinityVector = new Vector2(1000000f, 1000000f);

		private bool m_isFirstAllocation;

		private int m_max_characters = 8;

		private int m_max_numberOfLines = 4;

		private int[] m_char_buffer;

		private char[] m_input_CharArray = new char[256];

		private int m_charArray_Length;

		private List<char> m_VisibleCharacters = new List<char>();

		private readonly float[] k_Power = new float[10]
		{
			0.5f,
			0.05f,
			0.005f,
			0.0005f,
			5E-05f,
			5E-06f,
			5E-07f,
			5E-08f,
			5E-09f,
			5E-10f
		};

		private GlyphInfo m_cached_GlyphInfo;

		private GlyphInfo m_cached_Underline_GlyphInfo;

		private WordWrapState m_SavedWordWrapState = default(WordWrapState);

		private WordWrapState m_SavedLineState = default(WordWrapState);

		private int m_characterCount;

		private int m_visibleCharacterCount;

		private int m_visibleSpriteCount;

		private int m_firstCharacterOfLine;

		private int m_firstVisibleCharacterOfLine;

		private int m_lastCharacterOfLine;

		private int m_lastVisibleCharacterOfLine;

		private int m_lineNumber;

		private int m_pageNumber;

		private float m_maxAscender;

		private float m_maxDescender;

		private float m_maxFontScale;

		private float m_lineOffset;

		private Extents m_meshExtents;

		private bool m_isCalculateSizeRequired;

		private Mesh m_mesh;

		private Bounds m_bounds;

		[SerializeField]
		private bool m_ignoreCulling = true;

		[SerializeField]
		private bool m_isOrthographic;

		[SerializeField]
		private bool m_isCullingEnabled;

		private int m_maxVisibleCharacters = 99999;

		private int m_maxVisibleWords = 99999;

		private int m_maxVisibleLines = 99999;

		[SerializeField]
		private int m_pageToDisplay = 1;

		private bool m_isNewPage;

		private bool m_isTextTruncated;

		private Dictionary<int, TextMeshProFont> m_fontAsset_Dict = new Dictionary<int, TextMeshProFont>();

		private Dictionary<int, Material> m_fontMaterial_Dict = new Dictionary<int, Material>();

		private bool m_isMaskingEnabled;

		[SerializeField]
		private Material m_baseMaterial;

		private Material m_lastBaseMaterial;

		[SerializeField]
		private bool m_isNewBaseMaterial;

		private Material m_maskingMaterial;

		private bool m_isScrollRegionSet;

		private int m_stencilID;

		[SerializeField]
		private Vector4 m_maskOffset;

		private Matrix4x4 m_EnvMapMatrix = default(Matrix4x4);

		private TextRenderFlags m_renderMode;

		private bool m_isParsingText;

		private TMP_LinkInfo tag_LinkInfo = default(TMP_LinkInfo);

		private TMP_Settings m_settings;

		private int[] m_styleStack = new int[16];

		private int m_styleStackIndex;

		private int m_spriteCount;

		private bool m_isSprite;

		private int m_spriteIndex;

		private InlineGraphicManager m_inlineGraphics;

		[SerializeField]
		private Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

		private float m_marginLeft;

		private float m_marginRight;

		private float m_marginWidth;

		private float m_marginHeight;

		private bool m_marginsHaveChanged;

		private bool IsRectTransformDriven;

		private float m_width = -1f;

		private bool m_rectTransformDimensionsChanged;

		private Vector3[] m_rectCorners = new Vector3[4];

		[SerializeField]
		private bool m_enableAutoSizing;

		private float m_maxFontSize;

		private float m_minFontSize;

		private bool m_isAwake;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		private int m_recursiveCount;

		private int loopCountA;

		public Texture texture;

		private float m_flexibleHeight;

		private float m_flexibleWidth;

		private float m_minHeight;

		private float m_minWidth;

		private float m_preferredWidth = 9999f;

		private float m_preferredHeight = 9999f;

		private float m_renderedWidth;

		private float m_renderedHeight;

		private int m_layoutPriority;

		private bool m_isRebuildingLayout;

		private bool m_isLayoutDirty;

		private AutoLayoutPhase m_LayoutPhase;

		private bool m_currentAutoSizeMode;

		public string text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_inputSource = TextInputSources.Text;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				isInputParsingRequired = true;
				MarkLayoutForRebuild();
				m_text = value;
			}
		}

		public TextMeshProFont font
		{
			get
			{
				return m_fontAsset;
			}
			set
			{
				if (m_fontAsset != value)
				{
					m_fontAsset = value;
					LoadFontAsset();
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					MarkLayoutForRebuild();
				}
			}
		}

		public Material fontMaterial
		{
			get
			{
				if (m_fontMaterial == null)
				{
					SetFontMaterial(m_sharedMaterial);
					return m_sharedMaterial;
				}
				return m_sharedMaterial;
			}
			set
			{
				SetFontMaterial(value);
				m_havePropertiesChanged = true;
			}
		}

		public Material fontSharedMaterial
		{
			get
			{
				return m_uiRenderer.GetMaterial();
			}
			set
			{
				if (m_uiRenderer.GetMaterial() != value)
				{
					m_isNewBaseMaterial = true;
					SetSharedFontMaterial(value);
					m_havePropertiesChanged = true;
				}
			}
		}

		protected Material fontBaseMaterial
		{
			get
			{
				return m_baseMaterial;
			}
			set
			{
				if (m_baseMaterial != value)
				{
					SetFontBaseMaterial(value);
					m_havePropertiesChanged = true;
				}
			}
		}

		public bool isOverlay
		{
			get
			{
				return m_isOverlay;
			}
			set
			{
				if (m_isOverlay != value)
				{
					m_isOverlay = value;
					SetShaderDepth();
					m_havePropertiesChanged = true;
				}
			}
		}

		public new Color color
		{
			get
			{
				return m_fontColor;
			}
			set
			{
				if (m_fontColor != value)
				{
					m_havePropertiesChanged = true;
					m_fontColor = value;
				}
			}
		}

		public float alpha
		{
			get
			{
				return m_fontColor.a;
			}
			set
			{
				Color fontColor = m_fontColor;
				fontColor.a = value;
				m_fontColor = fontColor;
				m_havePropertiesChanged = true;
			}
		}

		public VertexGradient colorGradient
		{
			get
			{
				return m_fontColorGradient;
			}
			set
			{
				m_havePropertiesChanged = true;
				m_fontColorGradient = value;
			}
		}

		public bool enableVertexGradient
		{
			get
			{
				return m_enableVertexGradient;
			}
			set
			{
				m_havePropertiesChanged = true;
				m_enableVertexGradient = value;
			}
		}

		public Color32 faceColor
		{
			get
			{
				return m_faceColor;
			}
			set
			{
				SetFaceColor(value);
				m_havePropertiesChanged = true;
				m_faceColor = value;
			}
		}

		public Color32 outlineColor
		{
			get
			{
				return m_outlineColor;
			}
			set
			{
				SetOutlineColor(value);
				m_havePropertiesChanged = true;
				m_outlineColor = value;
			}
		}

		public float outlineWidth
		{
			get
			{
				return m_outlineWidth;
			}
			set
			{
				SetOutlineThickness(value);
				m_havePropertiesChanged = true;
				m_outlineWidth = value;
			}
		}

		public float fontSize
		{
			get
			{
				return m_fontSize;
			}
			set
			{
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				MarkLayoutForRebuild();
				m_fontSize = value;
				if (!m_enableAutoSizing)
				{
					m_fontSizeBase = m_fontSize;
				}
			}
		}

		public FontStyles fontStyle
		{
			get
			{
				return m_fontStyle;
			}
			set
			{
				m_fontStyle = value;
				m_havePropertiesChanged = true;
				checkPaddingRequired = true;
			}
		}

		public float characterSpacing
		{
			get
			{
				return m_characterSpacing;
			}
			set
			{
				if (m_characterSpacing != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					MarkLayoutForRebuild();
					m_characterSpacing = value;
				}
			}
		}

		public float lineSpacing
		{
			get
			{
				return m_lineSpacing;
			}
			set
			{
				if (m_lineSpacing != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					MarkLayoutForRebuild();
					m_lineSpacing = value;
				}
			}
		}

		public float paragraphSpacing
		{
			get
			{
				return m_paragraphSpacing;
			}
			set
			{
				if (m_paragraphSpacing != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					MarkLayoutForRebuild();
					m_paragraphSpacing = value;
				}
			}
		}

		public bool richText
		{
			get
			{
				return m_isRichText;
			}
			set
			{
				m_isRichText = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				MarkLayoutForRebuild();
				isInputParsingRequired = true;
			}
		}

		public bool parseCtrlCharacters
		{
			get
			{
				return m_parseCtrlCharacters;
			}
			set
			{
				m_parseCtrlCharacters = value;
				m_havePropertiesChanged = true;
				m_isCalculateSizeRequired = true;
				MarkLayoutForRebuild();
				isInputParsingRequired = true;
			}
		}

		public TextOverflowModes OverflowMode
		{
			get
			{
				return m_overflowMode;
			}
			set
			{
				m_overflowMode = value;
				m_havePropertiesChanged = true;
				m_isRecalculateScaleRequired = true;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (texture == null)
				{
					return Graphic.s_WhiteTexture;
				}
				return texture;
			}
		}

		public Bounds bounds
		{
			get
			{
				if (m_mesh != null)
				{
					return m_bounds;
				}
				return default(Bounds);
			}
		}

		public TextAlignmentOptions alignment
		{
			get
			{
				return m_textAlignment;
			}
			set
			{
				if (m_textAlignment != value)
				{
					m_havePropertiesChanged = true;
					m_textAlignment = value;
				}
			}
		}

		public bool enableKerning
		{
			get
			{
				return m_enableKerning;
			}
			set
			{
				if (m_enableKerning != value)
				{
					m_havePropertiesChanged = true;
					m_isCalculateSizeRequired = true;
					MarkLayoutForRebuild();
					m_enableKerning = value;
				}
			}
		}

		public bool overrideColorTags
		{
			get
			{
				return m_overrideHtmlColors;
			}
			set
			{
				if (m_overrideHtmlColors != value)
				{
					m_havePropertiesChanged = true;
					m_overrideHtmlColors = value;
				}
			}
		}

		public bool extraPadding
		{
			get
			{
				return m_enableExtraPadding;
			}
			set
			{
				if (m_enableExtraPadding != value)
				{
					m_havePropertiesChanged = true;
					checkPaddingRequired = true;
					m_enableExtraPadding = value;
					m_isCalculateSizeRequired = true;
					MarkLayoutForRebuild();
				}
			}
		}

		public bool enableWordWrapping
		{
			get
			{
				return m_enableWordWrapping;
			}
			set
			{
				if (m_enableWordWrapping != value)
				{
					m_havePropertiesChanged = true;
					isInputParsingRequired = true;
					m_isRecalculateScaleRequired = true;
					m_enableWordWrapping = value;
				}
			}
		}

		public TextureMappingOptions horizontalMapping
		{
			get
			{
				return m_horizontalMapping;
			}
			set
			{
				if (m_horizontalMapping != value)
				{
					m_havePropertiesChanged = true;
					m_horizontalMapping = value;
				}
			}
		}

		public TextureMappingOptions verticalMapping
		{
			get
			{
				return m_verticalMapping;
			}
			set
			{
				if (m_verticalMapping != value)
				{
					m_havePropertiesChanged = true;
					m_verticalMapping = value;
				}
			}
		}

		public bool ignoreVisibility
		{
			get
			{
				return m_ignoreCulling;
			}
			set
			{
				if (m_ignoreCulling != value)
				{
					m_havePropertiesChanged = true;
					m_ignoreCulling = value;
				}
			}
		}

		public bool isOrthographic
		{
			get
			{
				return m_isOrthographic;
			}
			set
			{
				m_havePropertiesChanged = true;
				m_isOrthographic = value;
			}
		}

		public bool enableCulling
		{
			get
			{
				return m_isCullingEnabled;
			}
			set
			{
				m_isCullingEnabled = value;
				SetCulling();
				m_havePropertiesChanged = true;
			}
		}

		public TextRenderFlags renderMode
		{
			get
			{
				return m_renderMode;
			}
			set
			{
				m_renderMode = value;
				m_havePropertiesChanged = true;
			}
		}

		public bool havePropertiesChanged
		{
			get
			{
				return m_havePropertiesChanged;
			}
			set
			{
				m_havePropertiesChanged = value;
			}
		}

		public Vector4 margin
		{
			get
			{
				return m_margin;
			}
			set
			{
				m_margin = value;
				m_havePropertiesChanged = true;
				m_marginsHaveChanged = true;
			}
		}

		public int maxVisibleCharacters
		{
			get
			{
				return m_maxVisibleCharacters;
			}
			set
			{
				if (m_maxVisibleCharacters != value)
				{
					m_havePropertiesChanged = true;
					m_maxVisibleCharacters = value;
				}
			}
		}

		public int maxVisibleWords
		{
			get
			{
				return m_maxVisibleWords;
			}
			set
			{
				if (m_maxVisibleWords != value)
				{
					m_havePropertiesChanged = true;
					m_maxVisibleWords = value;
				}
			}
		}

		public int maxVisibleLines
		{
			get
			{
				return m_maxVisibleLines;
			}
			set
			{
				if (m_maxVisibleLines != value)
				{
					m_havePropertiesChanged = true;
					isInputParsingRequired = true;
					m_maxVisibleLines = value;
				}
			}
		}

		public int pageToDisplay
		{
			get
			{
				return m_pageToDisplay;
			}
			set
			{
				m_havePropertiesChanged = true;
				m_pageToDisplay = value;
			}
		}

		public new RectTransform rectTransform
		{
			get
			{
				if (m_rectTransform == null)
				{
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform;
			}
		}

		public bool enableAutoSizing
		{
			get
			{
				return m_enableAutoSizing;
			}
			set
			{
				m_enableAutoSizing = value;
			}
		}

		public float fontSizeMin
		{
			get
			{
				return m_fontSizeMin;
			}
			set
			{
				m_fontSizeMin = value;
			}
		}

		public float fontSizeMax
		{
			get
			{
				return m_fontSizeMax;
			}
			set
			{
				m_fontSizeMax = value;
			}
		}

		public float flexibleHeight => m_flexibleHeight;

		public float flexibleWidth => m_flexibleWidth;

		public float minHeight => m_minHeight;

		public float minWidth => m_minWidth;

		public float preferredWidth => (m_preferredWidth != 9999f) ? m_preferredWidth : m_renderedWidth;

		public float preferredHeight => (m_preferredHeight != 9999f) ? m_preferredHeight : m_renderedHeight;

		public int layoutPriority => m_layoutPriority;

		public Vector4 maskOffset
		{
			get
			{
				return m_maskOffset;
			}
			set
			{
				m_maskOffset = value;
				UpdateMask();
				m_havePropertiesChanged = true;
			}
		}

		public TMP_TextInfo textInfo => m_textInfo;

		public Mesh mesh => m_mesh;

		public new CanvasRenderer canvasRenderer => m_uiRenderer;

		public InlineGraphicManager inlineGraphicManager => m_inlineGraphics;

		protected override void Awake()
		{
			m_isAwake = true;
			m_canvas = (GetComponentInParent(typeof(Canvas)) as Canvas);
			m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			if (m_rectTransform == null)
			{
				m_rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			m_uiRenderer = GetComponent<CanvasRenderer>();
			if (m_uiRenderer == null)
			{
				m_uiRenderer = base.gameObject.AddComponent<CanvasRenderer>();
			}
			if (m_mesh == null)
			{
				m_mesh = new Mesh();
				m_mesh.hideFlags = HideFlags.HideAndDontSave;
			}
			if (m_settings == null)
			{
				m_settings = TMP_Settings.LoadDefaultSettings();
			}
			if (m_settings != null)
			{
			}
			LoadFontAsset();
			TMP_StyleSheet.LoadDefaultStyleSheet();
			m_char_buffer = new int[m_max_characters];
			m_cached_GlyphInfo = new GlyphInfo();
			m_isFirstAllocation = true;
			m_textInfo = new TMP_TextInfo();
			m_textInfo.meshInfo.mesh = m_mesh;
			if (m_fontAsset == null)
			{
				Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.", this);
				return;
			}
			if (m_fontSizeMin == 0f)
			{
				m_fontSizeMin = m_fontSize / 2f;
			}
			if (m_fontSizeMax == 0f)
			{
				m_fontSizeMax = m_fontSize * 2f;
			}
			isInputParsingRequired = true;
			m_havePropertiesChanged = true;
			m_rectTransformDimensionsChanged = true;
			ForceMeshUpdate();
		}

		protected override void OnEnable()
		{
			if (!m_isRegisteredForEvents)
			{
				TMPro_EventManager.WILL_RENDER_CANVASES.Add(OnPreRenderCanvas);
				m_isRegisteredForEvents = true;
			}
			GraphicRegistry.RegisterGraphicForCanvas(base.canvas, this);
			if (m_canvas == null)
			{
				m_canvas = (GetComponentInParent(typeof(Canvas)) as Canvas);
			}
			if (m_uiRenderer.GetMaterial() == null)
			{
				if (m_sharedMaterial != null)
				{
					m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
				}
				else
				{
					m_isNewBaseMaterial = true;
					fontSharedMaterial = m_baseMaterial;
					RecalculateMasking();
				}
				m_havePropertiesChanged = true;
				m_rectTransformDimensionsChanged = true;
			}
			LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
			RecalculateClipping();
		}

		protected override void OnDisable()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(base.canvas, this);
			m_uiRenderer.Clear();
			LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
			RecalculateClipping();
		}

		protected override void OnDestroy()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(base.canvas, this);
			if (m_maskingMaterial != null)
			{
				MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
				m_maskingMaterial = null;
			}
			if (m_fontMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(m_fontMaterial);
			}
			TMPro_EventManager.WILL_RENDER_CANVASES.Remove(OnPreRenderCanvas);
			m_isRegisteredForEvents = false;
		}

		protected override void OnTransformParentChanged()
		{
			int stencilID = m_stencilID;
			m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (stencilID != m_stencilID)
			{
				RecalculateMasking();
			}
			RecalculateClipping();
			LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
			m_havePropertiesChanged = true;
		}

		private void LoadFontAsset()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (m_fontAsset == null)
			{
				if (m_settings == null)
				{
					m_settings = TMP_Settings.LoadDefaultSettings();
				}
				if (m_settings != null && m_settings.fontAsset != null)
				{
					m_fontAsset = m_settings.fontAsset;
				}
				else
				{
					m_fontAsset = (Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont);
				}
				if (m_fontAsset == null)
				{
					Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".", this);
					return;
				}
				if (m_fontAsset.characterDictionary == null)
				{
					Debug.Log("Dictionary is Null!");
				}
				m_baseMaterial = m_fontAsset.material;
				m_sharedMaterial = m_baseMaterial;
				m_isNewBaseMaterial = true;
			}
			else
			{
				if (m_fontAsset.characterDictionary == null)
				{
					m_fontAsset.ReadFontDefinition();
				}
				m_sharedMaterial = m_baseMaterial;
				m_isNewBaseMaterial = true;
				if (m_sharedMaterial == null || m_sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_sharedMaterial.mainTexture.GetInstanceID())
				{
					m_sharedMaterial = m_fontAsset.material;
					m_baseMaterial = m_sharedMaterial;
					m_isNewBaseMaterial = true;
				}
			}
			if (!m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo))
			{
				Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.", this);
			}
			m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (m_stencilID == 0)
			{
				if (m_maskingMaterial != null)
				{
					MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
					m_maskingMaterial = null;
				}
				m_sharedMaterial = m_baseMaterial;
			}
			else
			{
				if (m_maskingMaterial == null)
				{
					m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
				}
				else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
				{
					MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
					m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
				}
				m_sharedMaterial = m_maskingMaterial;
			}
			m_isNewBaseMaterial = false;
			SetShaderDepth();
			if (m_uiRenderer == null)
			{
				m_uiRenderer = GetComponent<CanvasRenderer>();
			}
			m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		}

		private void ScheduleUpdate()
		{
		}

		private void UpdateEnvMapMatrix()
		{
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) && !(m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null))
			{
				Debug.Log("Updating Env Matrix...");
				Vector3 euler = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
				m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
				m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
			}
		}

		private void EnableMasking()
		{
			if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
				m_uiRenderer.SetMaterial(m_fontMaterial, m_sharedMaterial.mainTexture);
			}
			m_sharedMaterial = m_fontMaterial;
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_ClipRect))
			{
				m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
				m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
				m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_TEX);
				UpdateMask();
			}
			m_isMaskingEnabled = true;
		}

		private void DisableMasking()
		{
			if (m_fontMaterial != null)
			{
				if (m_stencilID > 0)
				{
					m_sharedMaterial = m_maskingMaterial;
				}
				else
				{
					m_sharedMaterial = m_baseMaterial;
				}
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
				UnityEngine.Object.DestroyImmediate(m_fontMaterial);
			}
			m_isMaskingEnabled = false;
		}

		private void UpdateMask()
		{
			Debug.Log("Updating Mask...");
			if (m_rectTransform != null)
			{
				if (!ShaderUtilities.isInitialized)
				{
					ShaderUtilities.GetShaderPropertyIDs();
				}
				m_isScrollRegionSet = true;
				float num = Mathf.Min(Mathf.Min(m_margin.x, m_margin.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
				float num2 = Mathf.Min(Mathf.Min(m_margin.y, m_margin.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
				num = ((!(num > 0f)) ? 0f : num);
				num2 = ((!(num2 > 0f)) ? 0f : num2);
				float z = (m_rectTransform.rect.width - Mathf.Max(m_margin.x, 0f) - Mathf.Max(m_margin.z, 0f)) / 2f + num;
				float w = (m_rectTransform.rect.height - Mathf.Max(m_margin.y, 0f) - Mathf.Max(m_margin.w, 0f)) / 2f + num2;
				Vector3 localPosition = m_rectTransform.localPosition;
				Vector2 pivot = m_rectTransform.pivot;
				float x = (0.5f - pivot.x) * m_rectTransform.rect.width + (Mathf.Max(m_margin.x, 0f) - Mathf.Max(m_margin.z, 0f)) / 2f;
				Vector2 pivot2 = m_rectTransform.pivot;
				Vector2 vector = localPosition + new Vector3(x, (0.5f - pivot2.y) * m_rectTransform.rect.height + (0f - Mathf.Max(m_margin.y, 0f) + Mathf.Max(m_margin.w, 0f)) / 2f);
				Vector4 value = new Vector4(vector.x, vector.y, z, w);
				m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, value);
			}
		}

		private void SetFontMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (m_uiRenderer == null)
			{
				m_uiRenderer = GetComponent<CanvasRenderer>();
			}
			if (m_fontMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(m_fontMaterial);
			}
			if (m_maskingMaterial != null)
			{
				MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
				m_maskingMaterial = null;
			}
			m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			m_fontMaterial = CreateMaterialInstance(mat);
			if (m_stencilID > 0)
			{
				m_fontMaterial = MaterialManager.SetStencil(m_fontMaterial, m_stencilID);
			}
			m_sharedMaterial = m_fontMaterial;
			SetShaderDepth();
			m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		}

		private void SetSharedFontMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (m_uiRenderer == null)
			{
				m_uiRenderer = GetComponent<CanvasRenderer>();
			}
			if (mat == null)
			{
				mat = m_baseMaterial;
				m_isNewBaseMaterial = true;
			}
			m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (m_stencilID == 0)
			{
				if (m_maskingMaterial != null)
				{
					MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
					m_maskingMaterial = null;
				}
				m_baseMaterial = mat;
			}
			else
			{
				if (m_maskingMaterial == null)
				{
					m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, m_stencilID);
				}
				else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
				{
					MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
					m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, m_stencilID);
				}
				mat = m_maskingMaterial;
			}
			m_isNewBaseMaterial = false;
			m_sharedMaterial = mat;
			SetShaderDepth();
			m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		}

		private void SetFontBaseMaterial(Material mat)
		{
			Debug.Log("Changing Base Material from [" + ((!(m_lastBaseMaterial == null)) ? m_lastBaseMaterial.name : "Null") + "] to [" + mat.name + "].");
			m_baseMaterial = mat;
			m_lastBaseMaterial = mat;
		}

		private void SetOutlineThickness(float thickness)
		{
			if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
			{
				m_sharedMaterial = m_fontMaterial;
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			else if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
				m_sharedMaterial = m_fontMaterial;
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			thickness = Mathf.Clamp01(thickness);
			m_sharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		}

		private void SetFaceColor(Color32 color)
		{
			if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
			{
				m_sharedMaterial = m_fontMaterial;
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			else if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
				m_sharedMaterial = m_fontMaterial;
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			m_sharedMaterial.SetColor(ShaderUtilities.ID_FaceColor, color);
		}

		private void SetOutlineColor(Color32 color)
		{
			if (m_fontMaterial != null && m_sharedMaterial.GetInstanceID() != m_fontMaterial.GetInstanceID())
			{
				m_sharedMaterial = m_fontMaterial;
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			else if (m_fontMaterial == null)
			{
				m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
				m_sharedMaterial = m_fontMaterial;
				m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			}
			m_sharedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, color);
		}

		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.hideFlags = HideFlags.DontSave;
			material.name += " (Instance)";
			return material;
		}

		private void SetShaderDepth()
		{
			if (!(m_canvas == null) && !(m_sharedMaterial == null))
			{
				if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || m_isOverlay)
				{
					m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0f);
				}
				else
				{
					m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4f);
				}
			}
		}

		private void SetCulling()
		{
			if (m_isCullingEnabled)
			{
				m_uiRenderer.GetMaterial().SetFloat("_CullMode", 2f);
			}
			else
			{
				m_uiRenderer.GetMaterial().SetFloat("_CullMode", 0f);
			}
		}

		private void SetPerspectiveCorrection()
		{
			if (m_isOrthographic)
			{
				m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
			}
			else
			{
				m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
			}
		}

		private void SetMeshArrays(int size)
		{
			m_textInfo.meshInfo.ResizeMeshInfo(size);
			m_uiRenderer.SetMesh(m_textInfo.meshInfo.mesh);
		}

		private void AddIntToCharArray(int number, ref int index, int precision)
		{
			if (number < 0)
			{
				m_input_CharArray[index++] = '-';
				number = -number;
			}
			int num = index;
			do
			{
				m_input_CharArray[num++] = (char)(number % 10 + 48);
				number /= 10;
			}
			while (number > 0);
			int num3 = num;
			while (index + 1 < num)
			{
				num--;
				char c = m_input_CharArray[index];
				m_input_CharArray[index] = m_input_CharArray[num];
				m_input_CharArray[num] = c;
				index++;
			}
			index = num3;
		}

		private void AddFloatToCharArray(float number, ref int index, int precision)
		{
			if (number < 0f)
			{
				m_input_CharArray[index++] = '-';
				number = 0f - number;
			}
			number += k_Power[Mathf.Min(9, precision)];
			int num = (int)number;
			AddIntToCharArray(num, ref index, precision);
			if (precision > 0)
			{
				m_input_CharArray[index++] = '.';
				number -= (float)num;
				for (int i = 0; i < precision; i++)
				{
					number *= 10f;
					int num2 = (int)number;
					m_input_CharArray[index++] = (char)(num2 + 48);
					number -= (float)num2;
				}
			}
		}

		private void StringToCharArray(string text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
				return;
			}
			if (chars.Length <= text.Length)
			{
				int num = (text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256);
				chars = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (m_parseCtrlCharacters && text[i] == '\\' && text.Length > i + 1)
				{
					switch (text[i + 1])
					{
					case 'U':
						if (text.Length > i + 9)
						{
							chars[num2] = GetUTF32(i + 2);
							i += 9;
							num2++;
							continue;
						}
						break;
					case '\\':
						if (text.Length <= i + 2)
						{
							break;
						}
						chars[num2] = text[i + 1];
						chars[num2 + 1] = text[i + 2];
						i += 2;
						num2 += 2;
						continue;
					case 'n':
						chars[num2] = 10;
						i++;
						num2++;
						continue;
					case 'r':
						chars[num2] = 13;
						i++;
						num2++;
						continue;
					case 't':
						chars[num2] = 9;
						i++;
						num2++;
						continue;
					case 'u':
						if (text.Length > i + 5)
						{
							chars[num2] = (ushort)GetUTF16(i + 2);
							i += 5;
							num2++;
							continue;
						}
						break;
					}
				}
				if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
				{
					chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
					i++;
					num2++;
				}
				else
				{
					chars[num2] = text[i];
					num2++;
				}
			}
			chars[num2] = 0;
		}

		private void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray == null || m_charArray_Length == 0)
			{
				return;
			}
			if (charBuffer.Length <= m_charArray_Length)
			{
				int num = (m_charArray_Length <= 1024) ? Mathf.NextPowerOfTwo(m_charArray_Length + 1) : (m_charArray_Length + 256);
				charBuffer = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < m_charArray_Length; i++)
			{
				if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
				{
					charBuffer[num2] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
					i++;
					num2++;
				}
				else
				{
					charBuffer[num2] = charArray[i];
					num2++;
				}
			}
			charBuffer[num2] = 0;
		}

		private int GetArraySizes(int[] chars)
		{
			int num = 0;
			int num2 = 0;
			int endIndex = 0;
			m_isUsingBold = false;
			m_isParsingText = false;
			m_VisibleCharacters.Clear();
			for (int i = 0; chars[i] != 0; i++)
			{
				int num3 = chars[i];
				if (m_isRichText && num3 == 60 && ValidateHtmlTag(chars, i + 1, out endIndex))
				{
					i = endIndex;
					if ((m_style & FontStyles.Underline) == FontStyles.Underline)
					{
						num += 3;
					}
					if ((m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						m_isUsingBold = true;
					}
				}
				else
				{
					if (num3 != 9 && num3 != 10 && num3 != 13 && num3 != 32 && num3 != 160)
					{
						num++;
					}
					m_VisibleCharacters.Add((char)num3);
					num2++;
				}
			}
			return num2;
		}

		private int SetArraySizes(int[] chars)
		{
			int num = 0;
			int num2 = 0;
			int endIndex = 0;
			int num3 = 0;
			m_isUsingBold = false;
			m_isParsingText = false;
			m_isSprite = false;
			m_fontIndex = 0;
			m_VisibleCharacters.Clear();
			for (int i = 0; chars[i] != 0; i++)
			{
				int num4 = chars[i];
				if (m_isRichText && num4 == 60 && ValidateHtmlTag(chars, i + 1, out endIndex))
				{
					i = endIndex;
					if ((m_style & FontStyles.Underline) == FontStyles.Underline)
					{
						num += 3;
					}
					if ((m_style & FontStyles.Bold) == FontStyles.Bold)
					{
						m_isUsingBold = true;
					}
					if (m_isSprite)
					{
						num3++;
						num2++;
						m_VisibleCharacters.Add((char)(57344 + m_spriteIndex));
						m_isSprite = false;
					}
				}
				else
				{
					if (num4 != 9 && num4 != 10 && num4 != 13 && num4 != 32 && num4 != 160)
					{
						num++;
					}
					m_VisibleCharacters.Add((char)num4);
					num2++;
				}
			}
			if (num3 > 0)
			{
				if (m_inlineGraphics == null)
				{
					m_inlineGraphics = (GetComponent<InlineGraphicManager>() ?? base.gameObject.AddComponent<InlineGraphicManager>());
				}
				m_inlineGraphics.AllocatedVertexBuffers(num3);
			}
			else if (m_inlineGraphics != null)
			{
				m_inlineGraphics.ClearUIVertex();
			}
			m_spriteCount = num3;
			if (m_textInfo.characterInfo == null || num2 > m_textInfo.characterInfo.Length)
			{
				m_textInfo.characterInfo = new TMP_CharacterInfo[(num2 <= 1024) ? Mathf.NextPowerOfTwo(num2) : (num2 + 256)];
			}
			if (m_textInfo.meshInfo.vertices == null)
			{
				m_textInfo.meshInfo = new TMP_MeshInfo(m_mesh, num);
			}
			if (num * 4 > m_textInfo.meshInfo.vertices.Length)
			{
				if (m_isFirstAllocation)
				{
					SetMeshArrays(num);
					m_isFirstAllocation = false;
				}
				else
				{
					SetMeshArrays((num <= 1024) ? Mathf.NextPowerOfTwo(num) : (num + 256));
				}
			}
			return num2;
		}

		private void MarkLayoutForRebuild()
		{
			if (m_rectTransform == null)
			{
				m_rectTransform = GetComponent<RectTransform>();
			}
			LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
		}

		private void ParseInputText()
		{
			isInputParsingRequired = false;
			switch (m_inputSource)
			{
			case TextInputSources.Text:
				StringToCharArray(m_text, ref m_char_buffer);
				break;
			case TextInputSources.SetText:
				SetTextArrayToCharArray(m_input_CharArray, ref m_char_buffer);
				break;
			}
		}

		private void ComputeMarginSize()
		{
			if (m_rectTransform != null)
			{
				m_marginWidth = m_rectTransform.rect.width - m_margin.x - m_margin.z;
				m_marginHeight = m_rectTransform.rect.height - m_margin.y - m_margin.w;
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			m_havePropertiesChanged = true;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			SetShaderDepth();
			if (base.gameObject.activeInHierarchy)
			{
				ComputeMarginSize();
				if (m_rectTransform != null)
				{
					m_rectTransform.hasChanged = true;
				}
				else
				{
					m_rectTransform = GetComponent<RectTransform>();
					m_rectTransform.hasChanged = true;
				}
				if (m_isRebuildingLayout)
				{
					m_isLayoutDirty = true;
				}
				else
				{
					m_havePropertiesChanged = true;
				}
			}
		}

		private void OnPreRenderCanvas()
		{
			if (!base.isActiveAndEnabled || m_fontAsset == null)
			{
				return;
			}
			loopCountA = 0;
			if (m_rectTransform.hasChanged || m_marginsHaveChanged)
			{
				if (m_inlineGraphics != null)
				{
					m_inlineGraphics.UpdatePivot(m_rectTransform.pivot);
				}
				if (m_rectTransformDimensionsChanged || m_marginsHaveChanged)
				{
					ComputeMarginSize();
					if (m_marginsHaveChanged)
					{
						m_isScrollRegionSet = false;
					}
					m_rectTransformDimensionsChanged = false;
					m_marginsHaveChanged = false;
					m_isCalculateSizeRequired = true;
					m_havePropertiesChanged = true;
				}
				if (m_isMaskingEnabled)
				{
					UpdateMask();
				}
				m_rectTransform.hasChanged = false;
				Vector3 lossyScale = m_rectTransform.lossyScale;
				if (lossyScale != m_previousLossyScale)
				{
					if (!m_havePropertiesChanged && m_previousLossyScale.z != 0f && m_text != string.Empty)
					{
						UpdateSDFScale(m_previousLossyScale.z, lossyScale.z);
					}
					else
					{
						m_havePropertiesChanged = true;
					}
					m_previousLossyScale = lossyScale;
				}
			}
			if (!m_havePropertiesChanged && !m_fontAsset.propertiesChanged && !m_isLayoutDirty)
			{
				return;
			}
			if (m_canvas == null)
			{
				m_canvas = GetComponentInParent<Canvas>();
			}
			if (m_canvas == null)
			{
				return;
			}
			if (hasFontAssetChanged || m_fontAsset.propertiesChanged)
			{
				LoadFontAsset();
				hasFontAssetChanged = false;
				if (m_fontAsset == null || m_uiRenderer.GetMaterial() == null)
				{
					return;
				}
				m_fontAsset.propertiesChanged = false;
			}
			if (isInputParsingRequired || m_isTextTruncated)
			{
				ParseInputText();
			}
			if (m_enableAutoSizing)
			{
				m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);
			}
			m_maxFontSize = m_fontSizeMax;
			m_minFontSize = m_fontSizeMin;
			m_lineSpacingDelta = 0f;
			m_charWidthAdjDelta = 0f;
			m_recursiveCount = 0;
			m_isCharacterWrappingEnabled = false;
			m_isTextTruncated = false;
			m_isLayoutDirty = false;
			GenerateTextMesh();
			m_havePropertiesChanged = false;
		}

		private void GenerateTextMesh()
		{
			if (m_fontAsset.characterDictionary == null)
			{
				Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + GetInstanceID());
				return;
			}
			if (m_textInfo != null)
			{
				m_textInfo.Clear();
			}
			if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == 0)
			{
				m_uiRenderer.SetMesh(null);
				if (m_inlineGraphics != null)
				{
					m_inlineGraphics.ClearUIVertex();
				}
				m_preferredWidth = 0f;
				m_preferredHeight = 0f;
				m_renderedWidth = 0f;
				m_renderedHeight = 0f;
				LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
				return;
			}
			m_currentFontAsset = m_fontAsset;
			m_currentMaterial = m_sharedMaterial;
			int num = SetArraySizes(m_char_buffer);
			m_fontScale = m_fontSize / m_currentFontAsset.fontInfo.PointSize;
			float num2 = m_maxFontScale = m_fontScale;
			float num3 = 0f;
			float num4 = 1f;
			m_currentFontSize = m_fontSize;
			float num5 = 0f;
			int num6 = 0;
			m_style = m_fontStyle;
			m_lineJustification = m_textAlignment;
			if (checkPaddingRequired)
			{
				checkPaddingRequired = false;
				m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
				m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
			}
			float num7 = 0f;
			float num8 = 1f;
			m_baselineOffset = 0f;
			bool flag = false;
			Vector3 start = Vector3.zero;
			Vector3 zero = Vector3.zero;
			bool flag2 = false;
			Vector3 start2 = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			m_fontColor32 = m_fontColor;
			m_htmlColor = m_fontColor32;
			m_colorStackIndex = 0;
			Array.Clear(m_colorStack, 0, m_colorStack.Length);
			m_styleStackIndex = 0;
			Array.Clear(m_styleStack, 0, m_styleStack.Length);
			m_lineOffset = 0f;
			m_lineHeight = 0f;
			m_cSpacing = 0f;
			m_monoSpacing = 0f;
			float num9 = 0f;
			m_xAdvance = 0f;
			m_maxXAdvance = 0f;
			tag_LineIndent = 0f;
			tag_Indent = 0f;
			tag_NoParsing = false;
			m_isIgnoringAlignment = false;
			m_characterCount = 0;
			m_visibleCharacterCount = 0;
			m_visibleSpriteCount = 0;
			m_firstCharacterOfLine = 0;
			m_lastCharacterOfLine = 0;
			m_firstVisibleCharacterOfLine = 0;
			m_lastVisibleCharacterOfLine = 0;
			m_lineNumber = 0;
			bool flag3 = true;
			m_pageNumber = 0;
			int num10 = Mathf.Clamp(m_pageToDisplay - 1, 0, m_textInfo.pageInfo.Length - 1);
			int num11 = 0;
			m_rectTransform.GetLocalCorners(m_rectCorners);
			Vector4 margin = m_margin;
			float marginWidth = m_marginWidth;
			float marginHeight = m_marginHeight;
			m_marginLeft = 0f;
			m_marginRight = 0f;
			m_width = -1f;
			m_renderedWidth = 0f;
			m_renderedHeight = 0f;
			bool flag4 = true;
			bool flag5 = false;
			m_SavedLineState = default(WordWrapState);
			m_SavedWordWrapState = default(WordWrapState);
			int num12 = 0;
			m_meshExtents = new Extents(k_InfinityVector, -k_InfinityVector);
			if (m_textInfo.lineInfo == null)
			{
				m_textInfo.lineInfo = new TMP_LineInfo[2];
			}
			for (int i = 0; i < m_textInfo.lineInfo.Length; i++)
			{
				m_textInfo.lineInfo[i] = default(TMP_LineInfo);
				m_textInfo.lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
				m_textInfo.lineInfo[i].ascender = 0f - k_InfinityVector.x;
				m_textInfo.lineInfo[i].descender = k_InfinityVector.x;
			}
			m_maxAscender = 0f;
			m_maxDescender = 0f;
			float num13 = 0f;
			float num14 = 0f;
			bool flag6 = false;
			m_isNewPage = false;
			loopCountA++;
			int endIndex = 0;
			for (int j = 0; m_char_buffer[j] != 0; j++)
			{
				num6 = m_char_buffer[j];
				m_isSprite = false;
				num4 = 1f;
				if (m_isRichText && num6 == 60)
				{
					m_isParsingText = true;
					if (ValidateHtmlTag(m_char_buffer, j + 1, out endIndex))
					{
						j = endIndex;
						if (m_isRecalculateScaleRequired)
						{
							m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize;
							m_isRecalculateScaleRequired = false;
						}
						if (!m_isSprite)
						{
							continue;
						}
					}
				}
				m_isParsingText = false;
				bool flag7 = false;
				if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
				{
					if (char.IsLower((char)num6))
					{
						num6 = char.ToUpper((char)num6);
					}
				}
				else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
				{
					if (char.IsUpper((char)num6))
					{
						num6 = char.ToLower((char)num6);
					}
				}
				else if ((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
				{
					if (char.IsLower((char)num6))
					{
						m_fontScale = m_currentFontSize * 0.8f / m_currentFontAsset.fontInfo.PointSize;
						num6 = char.ToUpper((char)num6);
					}
					else
					{
						m_fontScale = m_currentFontSize / m_currentFontAsset.fontInfo.PointSize;
					}
				}
				if (m_isSprite)
				{
					SpriteInfo sprite = m_inlineGraphics.GetSprite(m_spriteIndex);
					if (sprite == null)
					{
						continue;
					}
					num6 = 57344 + m_spriteIndex;
					m_cached_GlyphInfo = new GlyphInfo();
					m_cached_GlyphInfo.x = sprite.x;
					m_cached_GlyphInfo.y = sprite.y;
					m_cached_GlyphInfo.width = sprite.width;
					m_cached_GlyphInfo.height = sprite.height;
					m_cached_GlyphInfo.xOffset = sprite.pivot.x + sprite.xOffset;
					m_cached_GlyphInfo.yOffset = sprite.pivot.y + sprite.yOffset;
					num4 = m_fontAsset.fontInfo.Ascender / sprite.height * sprite.scale;
					m_cached_GlyphInfo.xAdvance = sprite.xAdvance * num4;
					m_textInfo.characterInfo[m_characterCount].type = TMP_CharacterType.Sprite;
				}
				else
				{
					m_currentFontAsset.characterDictionary.TryGetValue(num6, out m_cached_GlyphInfo);
					if (m_cached_GlyphInfo == null)
					{
						if (char.IsLower((char)num6))
						{
							if (m_currentFontAsset.characterDictionary.TryGetValue(char.ToUpper((char)num6), out m_cached_GlyphInfo))
							{
								num6 = char.ToUpper((char)num6);
							}
						}
						else if (char.IsUpper((char)num6) && m_currentFontAsset.characterDictionary.TryGetValue(char.ToLower((char)num6), out m_cached_GlyphInfo))
						{
							num6 = char.ToLower((char)num6);
						}
						if (m_cached_GlyphInfo == null)
						{
							m_currentFontAsset.characterDictionary.TryGetValue(88, out m_cached_GlyphInfo);
							if (m_cached_GlyphInfo == null)
							{
								Debug.LogWarning("Character with ASCII value of " + num6 + " was not found in the Font Asset Glyph Table.", this);
								continue;
							}
							Debug.LogWarning("Character with ASCII value of " + num6 + " was not found in the Font Asset Glyph Table.", this);
							num6 = 88;
							flag7 = true;
						}
					}
					m_textInfo.characterInfo[m_characterCount].type = TMP_CharacterType.Character;
				}
				m_textInfo.characterInfo[m_characterCount].character = (char)num6;
				m_textInfo.characterInfo[m_characterCount].pointSize = m_currentFontSize;
				m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
				m_textInfo.characterInfo[m_characterCount].style = m_style;
				m_textInfo.characterInfo[m_characterCount].index = (short)j;
				if (m_enableKerning && m_characterCount >= 1)
				{
					int character = m_textInfo.characterInfo[m_characterCount - 1].character;
					KerningPairKey kerningPairKey = new KerningPairKey(character, num6);
					m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out KerningPair value);
					if (value != null)
					{
						m_xAdvance += value.XadvanceOffset * m_fontScale;
					}
				}
				float num15 = 0f;
				if (m_monoSpacing != 0f)
				{
					num15 = (m_monoSpacing / 2f - (m_cached_GlyphInfo.width / 2f + m_cached_GlyphInfo.xOffset) * m_fontScale) * (1f - m_charWidthAdjDelta);
					m_xAdvance += num15;
				}
				if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
				{
					num7 = m_currentFontAsset.BoldStyle * 2f;
					num8 = 1f + m_currentFontAsset.boldSpacing * 0.01f;
				}
				else
				{
					num7 = m_currentFontAsset.NormalStyle * 2f;
					num8 = 1f;
				}
				float num16 = (!m_isSprite) ? m_padding : 0f;
				Vector3 vector = new Vector3(m_xAdvance + (m_cached_GlyphInfo.xOffset - num16 - num7) * m_fontScale * num4 * (1f - m_charWidthAdjDelta), (m_cached_GlyphInfo.yOffset + num16) * m_fontScale * num4 - m_lineOffset + m_baselineOffset, 0f);
				Vector3 vector2 = new Vector3(vector.x, vector.y - (m_cached_GlyphInfo.height + num16 * 2f) * m_fontScale * num4, 0f);
				Vector3 vector3 = new Vector3(vector2.x + (m_cached_GlyphInfo.width + num16 * 2f + num7 * 2f) * m_fontScale * num4 * (1f - m_charWidthAdjDelta), vector.y, 0f);
				Vector3 vector4 = new Vector3(vector3.x, vector2.y, 0f);
				if ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
				{
					float num17 = (float)(int)m_currentFontAsset.ItalicStyle * 0.01f;
					Vector3 b = new Vector3(num17 * ((m_cached_GlyphInfo.yOffset + num16 + num7) * m_fontScale * num4), 0f, 0f);
					Vector3 b2 = new Vector3(num17 * ((m_cached_GlyphInfo.yOffset - m_cached_GlyphInfo.height - num16 - num7) * m_fontScale * num4), 0f, 0f);
					vector += b;
					vector2 += b2;
					vector3 += b;
					vector4 += b2;
				}
				m_textInfo.characterInfo[m_characterCount].bottomLeft = vector2;
				m_textInfo.characterInfo[m_characterCount].topLeft = vector;
				m_textInfo.characterInfo[m_characterCount].topRight = vector3;
				m_textInfo.characterInfo[m_characterCount].bottomRight = vector4;
				m_textInfo.characterInfo[m_characterCount].baseLine = 0f - m_lineOffset + m_baselineOffset;
				m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;
				float num18 = m_fontAsset.fontInfo.Ascender * m_fontScale + m_baselineOffset;
				if ((num6 == 10 || num6 == 13) && m_characterCount > m_firstVisibleCharacterOfLine)
				{
					num18 = m_baselineOffset;
				}
				float num19 = m_fontAsset.fontInfo.Descender * m_fontScale - m_lineOffset + m_baselineOffset;
				if (m_isSprite)
				{
					num18 = Mathf.Max(num18, vector.y - num16 * m_fontScale * num4);
					num19 = Mathf.Min(num19, vector4.y - num16 * m_fontScale * num4);
				}
				if (m_lineNumber == 0)
				{
					m_maxAscender = ((!(m_maxAscender > num18)) ? num18 : m_maxAscender);
				}
				if (m_lineOffset == 0f)
				{
					num13 = ((!(num13 > num18)) ? num18 : num13);
				}
				if (m_baselineOffset == 0f)
				{
					m_maxFontScale = Mathf.Max(m_maxFontScale, m_fontScale);
				}
				m_textInfo.characterInfo[m_characterCount].isVisible = false;
				if ((num6 != 10 && num6 != 13 && num6 != 32 && num6 != 160) || m_isSprite)
				{
					m_textInfo.characterInfo[m_characterCount].isVisible = true;
					float num20 = (m_width == -1f) ? (marginWidth + 0.0001f - m_marginLeft - m_marginRight) : Mathf.Min(marginWidth + 0.0001f - m_marginLeft - m_marginRight, m_width);
					m_textInfo.lineInfo[m_lineNumber].width = num20;
					m_textInfo.lineInfo[m_lineNumber].marginLeft = m_marginLeft;
					if (m_xAdvance + m_cached_GlyphInfo.xAdvance * (1f - m_charWidthAdjDelta) * m_fontScale > num20)
					{
						num11 = m_characterCount - 1;
						if (enableWordWrapping && m_characterCount != m_firstCharacterOfLine)
						{
							if (num12 == m_SavedWordWrapState.previous_WordBreak || flag4)
							{
								if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
								{
									if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100f)
									{
										loopCountA = 0;
										m_charWidthAdjDelta += 0.01f;
										GenerateTextMesh();
										return;
									}
									m_maxFontSize = m_fontSize;
									m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
									m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
									if (loopCountA <= 20)
									{
										GenerateTextMesh();
									}
									return;
								}
								if (!m_isCharacterWrappingEnabled)
								{
									m_isCharacterWrappingEnabled = true;
								}
								else
								{
									flag5 = true;
								}
								m_recursiveCount++;
								if (m_recursiveCount > 20)
								{
									continue;
								}
							}
							j = RestoreWordWrappingState(ref m_SavedWordWrapState);
							num12 = j;
							FaceInfo fontInfo = m_currentFontAsset.fontInfo;
							float num21 = (m_lineHeight != 0f) ? (m_lineHeight - (fontInfo.Ascender - fontInfo.Descender)) : (fontInfo.LineHeight - (fontInfo.Ascender - fontInfo.Descender));
							if (m_lineNumber > 0 && m_maxFontScale != 0f && m_lineHeight == 0f && num3 != m_maxFontScale && !m_isNewPage)
							{
								float num22 = 0f - fontInfo.Descender * num2 + (fontInfo.Ascender + num21 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * m_maxFontScale;
								m_lineOffset += num22 - num9;
								AdjustLineOffset(m_firstCharacterOfLine, m_characterCount - 1, num22 - num9);
								m_SavedWordWrapState.lineOffset = m_lineOffset;
							}
							m_isNewPage = false;
							float num23 = m_fontAsset.fontInfo.Ascender * m_maxFontScale - m_lineOffset;
							float num24 = m_fontAsset.fontInfo.Ascender * m_fontScale - m_lineOffset + m_baselineOffset;
							num23 = ((!(num23 > num24)) ? num24 : num23);
							float num25 = m_fontAsset.fontInfo.Descender * m_maxFontScale - m_lineOffset;
							float num26 = m_fontAsset.fontInfo.Descender * m_fontScale - m_lineOffset + m_baselineOffset;
							num25 = ((!(num25 < num26)) ? num26 : num25);
							m_maxDescender = ((!(m_maxDescender < num25)) ? num25 : m_maxDescender);
							if (!flag6)
							{
								num14 = m_maxDescender;
							}
							if (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines)
							{
								flag6 = true;
							}
							m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
							m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
							m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = ((m_characterCount - 1 > 0) ? (m_characterCount - 1) : 0);
							m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine;
							m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
							m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num25);
							m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num23);
							m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - num16 * m_maxFontScale;
							m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;
							m_textInfo.lineInfo[m_lineNumber].maxScale = m_maxFontScale;
							m_firstCharacterOfLine = m_characterCount;
							m_renderedWidth += m_xAdvance;
							if (m_enableWordWrapping)
							{
								m_renderedHeight = m_maxAscender - m_maxDescender;
							}
							else
							{
								m_renderedHeight = Mathf.Max(m_renderedHeight, num23 - num25);
							}
							SaveWordWrappingState(ref m_SavedLineState, j, m_characterCount - 1);
							m_lineNumber++;
							flag3 = true;
							if (m_lineNumber >= m_textInfo.lineInfo.Length)
							{
								ResizeLineExtents(m_lineNumber);
							}
							FontStyles style = m_textInfo.characterInfo[m_characterCount].style;
							float num27 = ((style & FontStyles.Subscript) != FontStyles.Subscript && (style & FontStyles.Superscript) != FontStyles.Superscript) ? m_textInfo.characterInfo[m_characterCount].scale : m_maxFontScale;
							num9 = 0f - fontInfo.Descender * m_maxFontScale + (fontInfo.Ascender + num21 + m_lineSpacing + m_lineSpacingDelta) * num27;
							m_lineOffset += num9;
							num2 = m_maxFontScale;
							num3 = num27;
							m_maxFontScale = 0f;
							num4 = 1f;
							m_xAdvance = tag_Indent;
							continue;
						}
						if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
						{
							if (m_charWidthAdjDelta < m_charWidthMaxAdj / 100f)
							{
								loopCountA = 0;
								m_charWidthAdjDelta += 0.01f;
								GenerateTextMesh();
								return;
							}
							m_maxFontSize = m_fontSize;
							m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
							m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
							m_recursiveCount = 0;
							if (loopCountA <= 20)
							{
								GenerateTextMesh();
							}
							return;
						}
						switch (m_overflowMode)
						{
						case TextOverflowModes.Overflow:
							if (m_isMaskingEnabled)
							{
								DisableMasking();
							}
							break;
						case TextOverflowModes.Ellipsis:
							if (m_isMaskingEnabled)
							{
								DisableMasking();
							}
							m_isTextTruncated = true;
							if (m_characterCount < 1)
							{
								m_textInfo.characterInfo[m_characterCount].isVisible = false;
								m_visibleCharacterCount--;
								break;
							}
							m_char_buffer[j - 1] = 8230;
							m_char_buffer[j] = 0;
							GenerateTextMesh();
							return;
						case TextOverflowModes.Masking:
							if (!m_isMaskingEnabled)
							{
								EnableMasking();
							}
							break;
						case TextOverflowModes.ScrollRect:
							if (!m_isMaskingEnabled)
							{
								EnableMasking();
							}
							break;
						case TextOverflowModes.Truncate:
							if (m_isMaskingEnabled)
							{
								DisableMasking();
							}
							m_textInfo.characterInfo[m_characterCount].isVisible = false;
							break;
						}
					}
					if (num6 != 9)
					{
						Color32 vertexColor = flag7 ? ((Color32)Color.red) : ((!m_overrideHtmlColors) ? m_htmlColor : m_fontColor32);
						if (!m_isSprite)
						{
							SaveGlyphVertexInfo(num7, vertexColor);
						}
						else
						{
							SaveSpriteVertexInfo(vertexColor);
						}
					}
					else
					{
						m_textInfo.characterInfo[m_characterCount].isVisible = false;
						m_lastVisibleCharacterOfLine = m_characterCount;
						m_textInfo.lineInfo[m_lineNumber].spaceCount++;
						m_textInfo.spaceCount++;
					}
					if (m_textInfo.characterInfo[m_characterCount].isVisible)
					{
						if (m_isSprite)
						{
							m_visibleSpriteCount++;
						}
						else
						{
							m_visibleCharacterCount++;
						}
						if (flag3)
						{
							flag3 = false;
							m_firstVisibleCharacterOfLine = m_characterCount;
						}
						m_lastVisibleCharacterOfLine = m_characterCount;
					}
				}
				else if (num6 == 9 || num6 == 32 || num6 == 160)
				{
					m_textInfo.lineInfo[m_lineNumber].spaceCount++;
					m_textInfo.spaceCount++;
				}
				m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
				m_textInfo.characterInfo[m_characterCount].pageNumber = (short)m_pageNumber;
				if ((num6 != 10 && num6 != 13 && num6 != 8230) || m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
				{
					m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
				}
				if (m_maxAscender - num19 > marginHeight + 0.0001f)
				{
					if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax && m_lineNumber > 0)
					{
						m_lineSpacingDelta -= 1f;
						GenerateTextMesh();
						return;
					}
					if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
					{
						m_maxFontSize = m_fontSize;
						m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.05f);
						m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20f + 0.5f) / 20f;
						m_recursiveCount = 0;
						if (loopCountA <= 20)
						{
							GenerateTextMesh();
						}
						return;
					}
					switch (m_overflowMode)
					{
					case TextOverflowModes.Overflow:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						break;
					case TextOverflowModes.Ellipsis:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (m_lineNumber > 0)
						{
							m_char_buffer[m_textInfo.characterInfo[num11].index] = 8230;
							m_char_buffer[m_textInfo.characterInfo[num11].index + 1] = 0;
							GenerateTextMesh();
							m_isTextTruncated = true;
						}
						else
						{
							m_char_buffer[0] = 0;
							GenerateTextMesh();
							m_isTextTruncated = true;
						}
						return;
					case TextOverflowModes.Masking:
						if (!m_isMaskingEnabled)
						{
							EnableMasking();
						}
						break;
					case TextOverflowModes.ScrollRect:
						if (!m_isMaskingEnabled)
						{
							EnableMasking();
						}
						break;
					case TextOverflowModes.Truncate:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (m_lineNumber > 0)
						{
							m_char_buffer[m_textInfo.characterInfo[num11].index + 1] = 0;
							GenerateTextMesh();
							m_isTextTruncated = true;
						}
						else
						{
							m_char_buffer[0] = 0;
							GenerateTextMesh();
							m_isTextTruncated = true;
						}
						return;
					case TextOverflowModes.Page:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (num6 == 13 || num6 == 10)
						{
							break;
						}
						j = RestoreWordWrappingState(ref m_SavedLineState);
						if (j == 0)
						{
							m_char_buffer[0] = 0;
							GenerateTextMesh();
							m_isTextTruncated = true;
							return;
						}
						m_isNewPage = true;
						m_xAdvance = tag_Indent;
						m_lineOffset = 0f;
						m_lineNumber++;
						m_pageNumber++;
						continue;
					}
				}
				if (num6 == 9)
				{
					m_xAdvance += m_fontAsset.fontInfo.TabWidth * m_fontScale;
				}
				else if (m_monoSpacing != 0f)
				{
					m_xAdvance += (m_monoSpacing - num15 + m_characterSpacing * m_fontScale + m_cSpacing) * (1f - m_charWidthAdjDelta);
				}
				else
				{
					m_xAdvance += ((m_cached_GlyphInfo.xAdvance * num8 + m_characterSpacing) * m_fontScale + m_cSpacing) * (1f - m_charWidthAdjDelta);
				}
				m_textInfo.characterInfo[m_characterCount].xAdvance = m_xAdvance;
				if (num6 == 13)
				{
					m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance);
					m_renderedWidth = 0f;
					m_xAdvance = tag_Indent;
				}
				if (num6 == 10 || m_characterCount == num - 1)
				{
					FaceInfo fontInfo2 = m_currentFontAsset.fontInfo;
					float num28 = (m_lineHeight != 0f) ? (m_lineHeight - (fontInfo2.Ascender - fontInfo2.Descender)) : (fontInfo2.LineHeight - (fontInfo2.Ascender - fontInfo2.Descender));
					if (m_lineNumber > 0 && m_maxFontScale != 0f && m_lineHeight == 0f && num3 != m_maxFontScale && !m_isNewPage)
					{
						float num29 = 0f - fontInfo2.Descender * num2 + (fontInfo2.Ascender + num28 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * m_maxFontScale;
						m_lineOffset += num29 - num9;
						AdjustLineOffset(m_firstCharacterOfLine, m_characterCount, num29 - num9);
					}
					m_isNewPage = false;
					float num30 = m_fontAsset.fontInfo.Ascender * m_maxFontScale - m_lineOffset;
					float num31 = m_fontAsset.fontInfo.Ascender * m_fontScale - m_lineOffset + m_baselineOffset;
					num30 = ((!(num30 > num31)) ? num31 : num30);
					float num32 = m_fontAsset.fontInfo.Descender * m_maxFontScale - m_lineOffset;
					float num33 = m_fontAsset.fontInfo.Descender * m_fontScale - m_lineOffset + m_baselineOffset;
					num32 = ((!(num32 < num33)) ? num33 : num32);
					m_maxDescender = ((!(m_maxDescender < num32)) ? num32 : m_maxDescender);
					if (!flag6)
					{
						num14 = m_maxDescender;
					}
					if (m_characterCount >= m_maxVisibleCharacters || m_lineNumber >= m_maxVisibleLines)
					{
						flag6 = true;
					}
					m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstCharacterOfLine;
					m_textInfo.lineInfo[m_lineNumber].firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
					m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount;
					m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = ((m_lastVisibleCharacterOfLine < m_firstVisibleCharacterOfLine) ? m_firstVisibleCharacterOfLine : m_lastVisibleCharacterOfLine);
					m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
					m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num32);
					m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num30);
					m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - num16 * m_maxFontScale;
					m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;
					m_textInfo.lineInfo[m_lineNumber].maxScale = m_maxFontScale;
					m_firstCharacterOfLine = m_characterCount + 1;
					if (num6 == 10 && m_characterCount != num - 1)
					{
						m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance);
						m_renderedWidth = 0f;
					}
					else
					{
						m_renderedWidth = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance);
					}
					m_renderedHeight = m_maxAscender - m_maxDescender;
					if (num6 == 10)
					{
						SaveWordWrappingState(ref m_SavedLineState, j, m_characterCount);
						SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
						m_lineNumber++;
						flag3 = true;
						if (m_lineNumber >= m_textInfo.lineInfo.Length)
						{
							ResizeLineExtents(m_lineNumber);
						}
						float num34 = ((m_style & FontStyles.Subscript) != FontStyles.Subscript && (m_style & FontStyles.Superscript) != FontStyles.Superscript) ? m_fontScale : m_maxFontScale;
						num9 = 0f - fontInfo2.Descender * m_maxFontScale + (fontInfo2.Ascender + num28 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * num34;
						m_lineOffset += num9;
						num2 = m_maxFontScale;
						num3 = num34;
						m_maxFontScale = 0f;
						num4 = 1f;
						m_xAdvance = tag_LineIndent + tag_Indent;
						num11 = m_characterCount - 1;
					}
				}
				m_textInfo.characterInfo[m_characterCount].topLine = m_textInfo.characterInfo[m_characterCount].baseLine + m_currentFontAsset.fontInfo.Ascender * m_fontScale;
				m_textInfo.characterInfo[m_characterCount].bottomLine = m_textInfo.characterInfo[m_characterCount].baseLine + m_currentFontAsset.fontInfo.Descender * m_fontScale;
				m_textInfo.characterInfo[m_characterCount].padding = num16 * m_fontScale;
				m_textInfo.characterInfo[m_characterCount].aspectRatio = m_cached_GlyphInfo.width / m_cached_GlyphInfo.height;
				if (m_textInfo.characterInfo[m_characterCount].isVisible)
				{
					m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].vertex_BL.position.x), Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].vertex_BL.position.y));
					m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].vertex_TR.position.x), Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].vertex_TL.position.y));
				}
				if (num6 != 13 && num6 != 10 && m_pageNumber < 16)
				{
					m_textInfo.pageInfo[m_pageNumber].ascender = num13;
					m_textInfo.pageInfo[m_pageNumber].descender = ((!(num19 < m_textInfo.pageInfo[m_pageNumber].descender)) ? m_textInfo.pageInfo[m_pageNumber].descender : num19);
					if (m_pageNumber == 0 && m_characterCount == 0)
					{
						m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
					}
					else if (m_characterCount > 0 && m_pageNumber != m_textInfo.characterInfo[m_characterCount - 1].pageNumber)
					{
						m_textInfo.pageInfo[m_pageNumber - 1].lastCharacterIndex = m_characterCount - 1;
						m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
					}
					else if (m_characterCount == num - 1)
					{
						m_textInfo.pageInfo[m_pageNumber].lastCharacterIndex = m_characterCount;
					}
				}
				if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
				{
					if ((num6 == 9 || num6 == 32) && !m_isNonBreakingSpace)
					{
						SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
						m_isCharacterWrappingEnabled = false;
						flag4 = false;
					}
					else if (!m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num6) && m_characterCount < num - 1 && !m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(m_VisibleCharacters[m_characterCount + 1]) && num6 > 11904 && num6 < 40959)
					{
						SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
						m_isCharacterWrappingEnabled = false;
						flag4 = false;
					}
					else if (flag4 || m_isCharacterWrappingEnabled || flag5)
					{
						SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
					}
				}
				m_characterCount++;
			}
			num5 = m_maxFontSize - m_minFontSize;
			if (!m_isCharacterWrappingEnabled && m_enableAutoSizing && num5 > 0.051f && m_fontSize < m_fontSizeMax)
			{
				m_minFontSize = m_fontSize;
				m_fontSize += Mathf.Max((m_maxFontSize - m_fontSize) / 2f, 0.05f);
				m_fontSize = (float)(int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 20f + 0.5f) / 20f;
				if (loopCountA <= 20)
				{
					GenerateTextMesh();
				}
				return;
			}
			m_isCharacterWrappingEnabled = false;
			m_renderedHeight += ((!(m_margin.y > 0f)) ? 0f : m_margin.y);
			if (m_renderMode == TextRenderFlags.GetPreferredSizes)
			{
				return;
			}
			if (!IsRectTransformDriven)
			{
				m_preferredWidth = m_renderedWidth;
				m_preferredHeight = m_renderedHeight;
			}
			if (m_visibleCharacterCount == 0 && m_visibleSpriteCount == 0)
			{
				m_uiRenderer.SetMesh(null);
				return;
			}
			int index = m_visibleCharacterCount * 4;
			Array.Clear(m_textInfo.meshInfo.vertices, index, m_textInfo.meshInfo.vertices.Length - index);
			switch (m_textAlignment)
			{
			case TextAlignmentOptions.TopLeft:
			case TextAlignmentOptions.Top:
			case TextAlignmentOptions.TopRight:
			case TextAlignmentOptions.TopJustified:
				if (m_overflowMode != TextOverflowModes.Page)
				{
					m_anchorOffset = m_rectCorners[1] + new Vector3(margin.x, 0f - m_maxAscender - margin.y, 0f);
				}
				else
				{
					m_anchorOffset = m_rectCorners[1] + new Vector3(margin.x, 0f - m_textInfo.pageInfo[num10].ascender - margin.y, 0f);
				}
				break;
			case TextAlignmentOptions.Left:
			case TextAlignmentOptions.Center:
			case TextAlignmentOptions.Right:
			case TextAlignmentOptions.Justified:
				if (m_overflowMode != TextOverflowModes.Page)
				{
					m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2f + new Vector3(margin.x, 0f - (m_maxAscender + margin.y + num14 - margin.w) / 2f, 0f);
				}
				else
				{
					m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2f + new Vector3(margin.x, 0f - (m_textInfo.pageInfo[num10].ascender + margin.y + m_textInfo.pageInfo[num10].descender - margin.w) / 2f, 0f);
				}
				break;
			case TextAlignmentOptions.BottomLeft:
			case TextAlignmentOptions.Bottom:
			case TextAlignmentOptions.BottomRight:
			case TextAlignmentOptions.BottomJustified:
				if (m_overflowMode != TextOverflowModes.Page)
				{
					m_anchorOffset = m_rectCorners[0] + new Vector3(margin.x, 0f - num14 + margin.w, 0f);
				}
				else
				{
					m_anchorOffset = m_rectCorners[0] + new Vector3(margin.x, 0f - m_textInfo.pageInfo[num10].descender + margin.w, 0f);
				}
				break;
			case TextAlignmentOptions.BaselineLeft:
			case TextAlignmentOptions.Baseline:
			case TextAlignmentOptions.BaselineRight:
			case TextAlignmentOptions.BaselineJustified:
				m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2f + new Vector3(margin.x, 0f, 0f);
				break;
			case TextAlignmentOptions.MidlineLeft:
			case TextAlignmentOptions.Midline:
			case TextAlignmentOptions.MidlineRight:
			case TextAlignmentOptions.MidlineJustified:
				m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2f + new Vector3(margin.x, 0f - (m_meshExtents.max.y + margin.y + m_meshExtents.min.y - margin.w) / 2f, 0f);
				break;
			}
			Vector3 vector5 = Vector3.zero;
			Vector3 vector6 = Vector3.zero;
			int num35 = 0;
			int num36 = 0;
			int num37 = 0;
			int num38 = 0;
			int num39 = 0;
			bool flag8 = false;
			int num40 = 0;
			int num41 = 0;
			bool flag9 = (!(m_canvas.worldCamera == null)) ? true : false;
			Vector3 lossyScale = m_rectTransform.lossyScale;
			float z = lossyScale.z;
			RenderMode renderMode = m_canvas.renderMode;
			float scaleFactor = m_canvas.scaleFactor;
			int num42 = 0;
			Color32 underlineColor = Color.white;
			Color32 underlineColor2 = Color.white;
			float num43 = 0f;
			float num44 = 0f;
			float num45 = 0f;
			float num46 = float.PositiveInfinity;
			int num47 = 0;
			float num48 = 0f;
			float num49 = 0f;
			float b3 = 0f;
			TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
			for (int k = 0; k < m_characterCount; k++)
			{
				int lineNumber = characterInfo[k].lineNumber;
				char character2 = characterInfo[k].character;
				TMP_LineInfo tMP_LineInfo = m_textInfo.lineInfo[lineNumber];
				TextAlignmentOptions alignment = tMP_LineInfo.alignment;
				num38 = lineNumber + 1;
				switch (alignment)
				{
				case TextAlignmentOptions.TopLeft:
				case TextAlignmentOptions.Left:
				case TextAlignmentOptions.BottomLeft:
				case TextAlignmentOptions.BaselineLeft:
				case TextAlignmentOptions.MidlineLeft:
					vector5 = new Vector3(tMP_LineInfo.marginLeft, 0f, 0f);
					break;
				case TextAlignmentOptions.Top:
				case TextAlignmentOptions.Center:
				case TextAlignmentOptions.Bottom:
				case TextAlignmentOptions.Baseline:
				case TextAlignmentOptions.Midline:
					vector5 = new Vector3(tMP_LineInfo.marginLeft + tMP_LineInfo.width / 2f - tMP_LineInfo.maxAdvance / 2f, 0f, 0f);
					break;
				case TextAlignmentOptions.TopRight:
				case TextAlignmentOptions.Right:
				case TextAlignmentOptions.BottomRight:
				case TextAlignmentOptions.BaselineRight:
				case TextAlignmentOptions.MidlineRight:
					vector5 = new Vector3(tMP_LineInfo.marginLeft + tMP_LineInfo.width - tMP_LineInfo.maxAdvance, 0f, 0f);
					break;
				case TextAlignmentOptions.TopJustified:
				case TextAlignmentOptions.Justified:
				case TextAlignmentOptions.BottomJustified:
				case TextAlignmentOptions.BaselineJustified:
				case TextAlignmentOptions.MidlineJustified:
				{
					num6 = m_textInfo.characterInfo[k].character;
					char character3 = m_textInfo.characterInfo[tMP_LineInfo.lastCharacterIndex].character;
					if (char.IsControl(character3) || lineNumber >= m_lineNumber)
					{
						vector5 = new Vector3(tMP_LineInfo.marginLeft, 0f, 0f);
						break;
					}
					float num50 = tMP_LineInfo.width - tMP_LineInfo.maxAdvance;
					if (lineNumber != num39 || k == 0)
					{
						vector5 = new Vector3(tMP_LineInfo.marginLeft, 0f, 0f);
					}
					else if (num6 == 9 || num6 == 32 || num6 == 160)
					{
						int num51 = (tMP_LineInfo.spaceCount - 1 <= 0) ? 1 : (tMP_LineInfo.spaceCount - 1);
						vector5 += new Vector3(num50 * (1f - m_wordWrappingRatios) / (float)num51, 0f, 0f);
					}
					else
					{
						vector5 += new Vector3(num50 * m_wordWrappingRatios / (float)(tMP_LineInfo.characterCount - tMP_LineInfo.spaceCount - 1), 0f, 0f);
					}
					break;
				}
				}
				vector6 = m_anchorOffset + vector5;
				if (characterInfo[k].isVisible)
				{
					TMP_CharacterType type = characterInfo[k].type;
					switch (type)
					{
					case TMP_CharacterType.Character:
					{
						Extents lineExtents = tMP_LineInfo.lineExtents;
						float num52 = m_uvLineOffset * (float)lineNumber % 1f + m_uvOffset.x;
						switch (m_horizontalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[k].vertex_BL.uv2.x = m_uvOffset.x;
							characterInfo[k].vertex_TL.uv2.x = m_uvOffset.x;
							characterInfo[k].vertex_TR.uv2.x = 1f + m_uvOffset.x;
							characterInfo[k].vertex_BR.uv2.x = 1f + m_uvOffset.x;
							break;
						case TextureMappingOptions.Line:
							if (m_textAlignment != TextAlignmentOptions.Justified)
							{
								characterInfo[k].vertex_BL.uv2.x = (characterInfo[k].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num52;
								characterInfo[k].vertex_TL.uv2.x = (characterInfo[k].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num52;
								characterInfo[k].vertex_TR.uv2.x = (characterInfo[k].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num52;
								characterInfo[k].vertex_BR.uv2.x = (characterInfo[k].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num52;
							}
							else
							{
								characterInfo[k].vertex_BL.uv2.x = (characterInfo[k].vertex_BL.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
								characterInfo[k].vertex_TL.uv2.x = (characterInfo[k].vertex_TL.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
								characterInfo[k].vertex_TR.uv2.x = (characterInfo[k].vertex_TR.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
								characterInfo[k].vertex_BR.uv2.x = (characterInfo[k].vertex_BR.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
							}
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[k].vertex_BL.uv2.x = (characterInfo[k].vertex_BL.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
							characterInfo[k].vertex_TL.uv2.x = (characterInfo[k].vertex_TL.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
							characterInfo[k].vertex_TR.uv2.x = (characterInfo[k].vertex_TR.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
							characterInfo[k].vertex_BR.uv2.x = (characterInfo[k].vertex_BR.position.x + vector5.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num52;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							switch (m_verticalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[k].vertex_BL.uv2.y = m_uvOffset.y;
								characterInfo[k].vertex_TL.uv2.y = 1f + m_uvOffset.y;
								characterInfo[k].vertex_TR.uv2.y = m_uvOffset.y;
								characterInfo[k].vertex_BR.uv2.y = 1f + m_uvOffset.y;
								break;
							case TextureMappingOptions.Line:
								characterInfo[k].vertex_BL.uv2.y = (characterInfo[k].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num52;
								characterInfo[k].vertex_TL.uv2.y = (characterInfo[k].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num52;
								characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
								characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.Paragraph:
								characterInfo[k].vertex_BL.uv2.y = (characterInfo[k].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num52;
								characterInfo[k].vertex_TL.uv2.y = (characterInfo[k].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num52;
								characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
								characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.MatchAspect:
								Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
								break;
							}
							float num53 = (1f - (characterInfo[k].vertex_BL.uv2.y + characterInfo[k].vertex_TL.uv2.y) * characterInfo[k].aspectRatio) / 2f;
							characterInfo[k].vertex_BL.uv2.x = characterInfo[k].vertex_BL.uv2.y * characterInfo[k].aspectRatio + num53 + num52;
							characterInfo[k].vertex_TL.uv2.x = characterInfo[k].vertex_BL.uv2.x;
							characterInfo[k].vertex_TR.uv2.x = characterInfo[k].vertex_TL.uv2.y * characterInfo[k].aspectRatio + num53 + num52;
							characterInfo[k].vertex_BR.uv2.x = characterInfo[k].vertex_TR.uv2.x;
							break;
						}
						}
						switch (m_verticalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[k].vertex_BL.uv2.y = m_uvOffset.y;
							characterInfo[k].vertex_TL.uv2.y = 1f + m_uvOffset.y;
							characterInfo[k].vertex_TR.uv2.y = 1f + m_uvOffset.y;
							characterInfo[k].vertex_BR.uv2.y = m_uvOffset.y;
							break;
						case TextureMappingOptions.Line:
							characterInfo[k].vertex_BL.uv2.y = (characterInfo[k].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
							characterInfo[k].vertex_TL.uv2.y = (characterInfo[k].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
							characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
							characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[k].vertex_BL.uv2.y = (characterInfo[k].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
							characterInfo[k].vertex_TL.uv2.y = (characterInfo[k].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
							characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
							characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							float num54 = (1f - (characterInfo[k].vertex_BL.uv2.x + characterInfo[k].vertex_TR.uv2.x) / characterInfo[k].aspectRatio) / 2f;
							characterInfo[k].vertex_BL.uv2.y = num54 + characterInfo[k].vertex_BL.uv2.x / characterInfo[k].aspectRatio + m_uvOffset.y;
							characterInfo[k].vertex_TL.uv2.y = num54 + characterInfo[k].vertex_TR.uv2.x / characterInfo[k].aspectRatio + m_uvOffset.y;
							characterInfo[k].vertex_BR.uv2.y = characterInfo[k].vertex_BL.uv2.y;
							characterInfo[k].vertex_TR.uv2.y = characterInfo[k].vertex_TL.uv2.y;
							break;
						}
						}
						float num55 = characterInfo[k].scale * (1f - m_charWidthAdjDelta);
						if ((characterInfo[k].style & FontStyles.Bold) == FontStyles.Bold)
						{
							num55 *= -1f;
						}
						switch (renderMode)
						{
						case RenderMode.ScreenSpaceOverlay:
							num55 *= z / scaleFactor;
							break;
						case RenderMode.ScreenSpaceCamera:
							num55 *= ((!flag9) ? 1f : z);
							break;
						case RenderMode.WorldSpace:
							num55 *= z;
							break;
						}
						float x = characterInfo[k].vertex_BL.uv2.x;
						float y = characterInfo[k].vertex_BL.uv2.y;
						float x2 = characterInfo[k].vertex_TR.uv2.x;
						float y2 = characterInfo[k].vertex_TR.uv2.y;
						float num56 = Mathf.Floor(x);
						float num57 = Mathf.Floor(y);
						x -= num56;
						x2 -= num56;
						y -= num57;
						y2 -= num57;
						characterInfo[k].vertex_BL.uv2 = PackUV(x, y, num55);
						characterInfo[k].vertex_TL.uv2 = PackUV(x, y2, num55);
						characterInfo[k].vertex_TR.uv2 = PackUV(x2, y2, num55);
						characterInfo[k].vertex_BR.uv2 = PackUV(x2, y, num55);
						break;
					}
					}
					if (k < m_maxVisibleCharacters && lineNumber < m_maxVisibleLines && m_overflowMode != TextOverflowModes.Page)
					{
						characterInfo[k].vertex_BL.position += vector6;
						characterInfo[k].vertex_TL.position += vector6;
						characterInfo[k].vertex_TR.position += vector6;
						characterInfo[k].vertex_BR.position += vector6;
					}
					else if (k < m_maxVisibleCharacters && lineNumber < m_maxVisibleLines && m_overflowMode == TextOverflowModes.Page && characterInfo[k].pageNumber == num10)
					{
						characterInfo[k].vertex_BL.position += vector6;
						characterInfo[k].vertex_TL.position += vector6;
						characterInfo[k].vertex_TR.position += vector6;
						characterInfo[k].vertex_BR.position += vector6;
					}
					else
					{
						characterInfo[k].vertex_BL.position *= 0f;
						characterInfo[k].vertex_TL.position *= 0f;
						characterInfo[k].vertex_TR.position *= 0f;
						characterInfo[k].vertex_BR.position *= 0f;
					}
					switch (type)
					{
					case TMP_CharacterType.Character:
						FillCharacterVertexBuffers(k, num35);
						num35 += 4;
						break;
					case TMP_CharacterType.Sprite:
						FillSpriteVertexBuffers(k, num36);
						num36 += 4;
						break;
					}
				}
				m_textInfo.characterInfo[k].bottomLeft += vector6;
				m_textInfo.characterInfo[k].topLeft += vector6;
				m_textInfo.characterInfo[k].topRight += vector6;
				m_textInfo.characterInfo[k].bottomRight += vector6;
				m_textInfo.characterInfo[k].topLine += vector6.y;
				m_textInfo.characterInfo[k].bottomLine += vector6.y;
				m_textInfo.characterInfo[k].baseLine += vector6.y;
				if (character2 != '\n' && character2 != '\r')
				{
					m_textInfo.lineInfo[lineNumber].ascender = ((!(m_textInfo.characterInfo[k].topLine > m_textInfo.lineInfo[lineNumber].ascender)) ? m_textInfo.lineInfo[lineNumber].ascender : m_textInfo.characterInfo[k].topLine);
					m_textInfo.lineInfo[lineNumber].descender = ((!(m_textInfo.characterInfo[k].bottomLine < m_textInfo.lineInfo[lineNumber].descender)) ? m_textInfo.lineInfo[lineNumber].descender : m_textInfo.characterInfo[k].bottomLine);
				}
				if (lineNumber != num39 || k == m_characterCount - 1)
				{
					if (lineNumber != num39)
					{
						m_textInfo.lineInfo[num39].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num39].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[num39].descender);
						m_textInfo.lineInfo[num39].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num39].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[num39].ascender);
					}
					if (k == m_characterCount - 1)
					{
						m_textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lineNumber].descender);
						m_textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[lineNumber].ascender);
					}
				}
				if (char.IsLetterOrDigit(character2) || character2 == '\'' || character2 == '’')
				{
					if (!flag8)
					{
						flag8 = true;
						num40 = k;
					}
					if (flag8 && k == m_characterCount - 1)
					{
						num41 = k;
						num37++;
						m_textInfo.lineInfo[lineNumber].wordCount++;
						TMP_WordInfo item = default(TMP_WordInfo);
						item.firstCharacterIndex = num40;
						item.lastCharacterIndex = num41;
						item.characterCount = num41 - num40 + 1;
						m_textInfo.wordInfo.Add(item);
					}
				}
				else if (flag8 || (k == 0 && (char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || k == m_characterCount - 1)))
				{
					num41 = ((k != m_characterCount - 1 || !char.IsLetterOrDigit(character2)) ? (k - 1) : k);
					flag8 = false;
					num37++;
					m_textInfo.lineInfo[lineNumber].wordCount++;
					TMP_WordInfo item2 = default(TMP_WordInfo);
					item2.firstCharacterIndex = num40;
					item2.lastCharacterIndex = num41;
					item2.characterCount = num41 - num40 + 1;
					m_textInfo.wordInfo.Add(item2);
				}
				if ((m_textInfo.characterInfo[k].style & FontStyles.Underline) == FontStyles.Underline)
				{
					bool flag10 = true;
					int pageNumber = m_textInfo.characterInfo[k].pageNumber;
					if (k > m_maxVisibleCharacters || lineNumber > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && pageNumber + 1 != m_pageToDisplay))
					{
						flag10 = false;
					}
					if (character2 != '\n' && character2 != '\r' && character2 != ' ' && character2 != '\u00a0')
					{
						num45 = Mathf.Max(num45, m_textInfo.characterInfo[k].scale);
						num46 = Mathf.Min((pageNumber != num47) ? float.PositiveInfinity : num46, m_textInfo.characterInfo[k].baseLine + font.fontInfo.Underline * num45);
						num47 = pageNumber;
					}
					if (!flag && flag10 && k <= tMP_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (k != tMP_LineInfo.lastVisibleCharacterIndex || (character2 != ' ' && character2 != '\u00a0')))
					{
						flag = true;
						num43 = m_textInfo.characterInfo[k].scale;
						if (num45 == 0f)
						{
							num45 = num43;
						}
						start = new Vector3(m_textInfo.characterInfo[k].bottomLeft.x, num46, 0f);
						underlineColor = m_textInfo.characterInfo[k].color;
					}
					if (flag && m_characterCount == 1)
					{
						flag = false;
						zero = new Vector3(m_textInfo.characterInfo[k].topRight.x, num46, 0f);
						num44 = m_textInfo.characterInfo[k].scale;
						DrawUnderlineMesh(start, zero, ref index, num43, num44, num45, underlineColor);
						num42++;
						num45 = 0f;
						num46 = float.PositiveInfinity;
					}
					else if (flag && (k == tMP_LineInfo.lastCharacterIndex || k >= tMP_LineInfo.lastVisibleCharacterIndex))
					{
						if (character2 == '\n' || character2 == '\r' || character2 == ' ' || character2 == '\u00a0')
						{
							int lastVisibleCharacterIndex = tMP_LineInfo.lastVisibleCharacterIndex;
							zero = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, num46, 0f);
							num44 = m_textInfo.characterInfo[lastVisibleCharacterIndex].scale;
						}
						else
						{
							zero = new Vector3(m_textInfo.characterInfo[k].topRight.x, num46, 0f);
							num44 = m_textInfo.characterInfo[k].scale;
						}
						flag = false;
						DrawUnderlineMesh(start, zero, ref index, num43, num44, num45, underlineColor);
						num42++;
						num45 = 0f;
						num46 = float.PositiveInfinity;
					}
					else if (flag && !flag10)
					{
						flag = false;
						zero = new Vector3(m_textInfo.characterInfo[k - 1].topRight.x, num46, 0f);
						num44 = m_textInfo.characterInfo[k - 1].scale;
						DrawUnderlineMesh(start, zero, ref index, num43, num44, num45, underlineColor);
						num42++;
						num45 = 0f;
						num46 = float.PositiveInfinity;
					}
				}
				else if (flag)
				{
					flag = false;
					zero = new Vector3(m_textInfo.characterInfo[k - 1].topRight.x, num46, 0f);
					num44 = m_textInfo.characterInfo[k - 1].scale;
					DrawUnderlineMesh(start, zero, ref index, num43, num44, num45, underlineColor);
					num42++;
					num45 = 0f;
					num46 = float.PositiveInfinity;
				}
				if ((m_textInfo.characterInfo[k].style & FontStyles.Strikethrough) == FontStyles.Strikethrough)
				{
					bool flag11 = true;
					if (k > m_maxVisibleCharacters || lineNumber > m_maxVisibleLines || (m_overflowMode == TextOverflowModes.Page && m_textInfo.characterInfo[k].pageNumber + 1 != m_pageToDisplay))
					{
						flag11 = false;
					}
					if (!flag2 && flag11 && k <= tMP_LineInfo.lastVisibleCharacterIndex && character2 != '\n' && character2 != '\r' && (k != tMP_LineInfo.lastVisibleCharacterIndex || (character2 != ' ' && character2 != '\u00a0')))
					{
						flag2 = true;
						num48 = m_textInfo.characterInfo[k].pointSize;
						num49 = m_textInfo.characterInfo[k].scale;
						start2 = new Vector3(m_textInfo.characterInfo[k].bottomLeft.x, m_textInfo.characterInfo[k].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2.75f * num49, 0f);
						underlineColor2 = m_textInfo.characterInfo[k].color;
						b3 = m_textInfo.characterInfo[k].baseLine;
					}
					if (flag2 && m_characterCount == 1)
					{
						flag2 = false;
						zero2 = new Vector3(m_textInfo.characterInfo[k].topRight.x, m_textInfo.characterInfo[k].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * num49, 0f);
						DrawUnderlineMesh(start2, zero2, ref index, num49, num49, num49, underlineColor2);
						num42++;
					}
					else if (flag2 && k == tMP_LineInfo.lastCharacterIndex)
					{
						if (character2 != '\n' && character2 != '\r' && character2 != ' ' && character2 != '\u00a0')
						{
							zero2 = new Vector3(m_textInfo.characterInfo[k].topRight.x, m_textInfo.characterInfo[k].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * num49, 0f);
						}
						else
						{
							int lastVisibleCharacterIndex2 = tMP_LineInfo.lastVisibleCharacterIndex;
							zero2 = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex2].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex2].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * num49, 0f);
						}
						flag2 = false;
						DrawUnderlineMesh(start2, zero2, ref index, num49, num49, num49, underlineColor2);
						num42++;
					}
					else if (flag2 && k < m_characterCount && (m_textInfo.characterInfo[k + 1].pointSize != num48 || !TMP_Math.Equals(m_textInfo.characterInfo[k + 1].baseLine + vector6.y, b3)))
					{
						flag2 = false;
						int lastVisibleCharacterIndex3 = tMP_LineInfo.lastVisibleCharacterIndex;
						zero2 = ((k > lastVisibleCharacterIndex3) ? new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex3].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex3].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * num49, 0f) : new Vector3(m_textInfo.characterInfo[k].topRight.x, m_textInfo.characterInfo[k].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * num49, 0f));
						DrawUnderlineMesh(start2, zero2, ref index, num49, num49, num49, underlineColor2);
						num42++;
					}
					else if (flag2 && !flag11)
					{
						flag2 = false;
						zero2 = new Vector3(m_textInfo.characterInfo[k - 1].topRight.x, m_textInfo.characterInfo[k - 1].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * num49, 0f);
						DrawUnderlineMesh(start2, zero2, ref index, num49, num49, num49, underlineColor2);
						num42++;
					}
				}
				else if (flag2)
				{
					flag2 = false;
					zero2 = new Vector3(m_textInfo.characterInfo[k - 1].topRight.x, m_textInfo.characterInfo[k - 1].baseLine + (font.fontInfo.Ascender + font.fontInfo.Descender) / 2f * m_fontScale, 0f);
					DrawUnderlineMesh(start2, zero2, ref index, num49, num49, num49, underlineColor2);
					num42++;
				}
				num39 = lineNumber;
			}
			m_textInfo.characterCount = (short)m_characterCount;
			m_textInfo.spriteCount = m_spriteCount;
			m_textInfo.lineCount = (short)num38;
			m_textInfo.wordCount = ((num37 == 0 || m_characterCount <= 0) ? 1 : ((short)num37));
			m_textInfo.pageCount = m_pageNumber + 1;
			if (m_renderMode == TextRenderFlags.Render)
			{
				m_mesh.vertices = m_textInfo.meshInfo.vertices;
				m_mesh.uv = m_textInfo.meshInfo.uvs0;
				m_mesh.uv2 = m_textInfo.meshInfo.uvs2;
				m_mesh.colors32 = m_textInfo.meshInfo.colors32;
				m_mesh.RecalculateBounds();
				m_uiRenderer.SetMesh(m_mesh);
				if (m_inlineGraphics != null)
				{
					m_inlineGraphics.DrawSprite(m_inlineGraphics.uiVertex, m_visibleSpriteCount);
				}
			}
			m_bounds = new Bounds(new Vector3((m_meshExtents.max.x + m_meshExtents.min.x) / 2f, (m_meshExtents.max.y + m_meshExtents.min.y) / 2f, 0f) + vector6, new Vector3(m_meshExtents.max.x - m_meshExtents.min.x, m_meshExtents.max.y - m_meshExtents.min.y, 0f));
			TMPro_EventManager.ON_TEXT_CHANGED(this);
		}

		private void SaveGlyphVertexInfo(float style_padding, Color32 vertexColor)
		{
			m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
			vertexColor.a = ((m_fontColor32.a >= vertexColor.a) ? vertexColor.a : m_fontColor32.a);
			if (!m_enableVertexGradient)
			{
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
			}
			else
			{
				if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
				{
					m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
					m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
					m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
					m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
				}
				else
				{
					m_textInfo.characterInfo[m_characterCount].vertex_BL.color = m_fontColorGradient.bottomLeft;
					m_textInfo.characterInfo[m_characterCount].vertex_TL.color = m_fontColorGradient.topLeft;
					m_textInfo.characterInfo[m_characterCount].vertex_TR.color = m_fontColorGradient.topRight;
					m_textInfo.characterInfo[m_characterCount].vertex_BR.color = m_fontColorGradient.bottomRight;
				}
				m_textInfo.characterInfo[m_characterCount].vertex_BL.color.a = vertexColor.a;
				m_textInfo.characterInfo[m_characterCount].vertex_TL.color.a = vertexColor.a;
				m_textInfo.characterInfo[m_characterCount].vertex_TR.color.a = vertexColor.a;
				m_textInfo.characterInfo[m_characterCount].vertex_BR.color.a = vertexColor.a;
			}
			if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
			{
				style_padding = 0f;
			}
			Vector2 uv = new Vector2((m_cached_GlyphInfo.x - m_padding - style_padding) / m_currentFontAsset.fontInfo.AtlasWidth, 1f - (m_cached_GlyphInfo.y + m_padding + style_padding + m_cached_GlyphInfo.height) / m_currentFontAsset.fontInfo.AtlasHeight);
			Vector2 uv2 = new Vector2(uv.x, 1f - (m_cached_GlyphInfo.y - m_padding - style_padding) / m_currentFontAsset.fontInfo.AtlasHeight);
			Vector2 uv3 = new Vector2((m_cached_GlyphInfo.x + m_padding + style_padding + m_cached_GlyphInfo.width) / m_currentFontAsset.fontInfo.AtlasWidth, uv.y);
			Vector2 uv4 = new Vector2(uv3.x, uv2.y);
			m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv2;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv4;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv3;
			Vector3 normal = new Vector3(0f, 0f, -1f);
			m_textInfo.characterInfo[m_characterCount].vertex_BL.normal = normal;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.normal = normal;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.normal = normal;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.normal = normal;
			Vector4 tangent = new Vector4(-1f, 0f, 0f, 1f);
			m_textInfo.characterInfo[m_characterCount].vertex_BL.tangent = tangent;
			m_textInfo.characterInfo[m_characterCount].vertex_TL.tangent = tangent;
			m_textInfo.characterInfo[m_characterCount].vertex_TR.tangent = tangent;
			m_textInfo.characterInfo[m_characterCount].vertex_BR.tangent = tangent;
		}

		private void SaveSpriteVertexInfo(Color32 vertexColor)
		{
			Vector2 uv = new Vector2(m_cached_GlyphInfo.x / (float)m_inlineGraphics.spriteAsset.spriteSheet.width, m_cached_GlyphInfo.y / (float)m_inlineGraphics.spriteAsset.spriteSheet.height);
			Vector2 uv2 = new Vector2(uv.x, (m_cached_GlyphInfo.y + m_cached_GlyphInfo.height) / (float)m_inlineGraphics.spriteAsset.spriteSheet.height);
			Vector2 uv3 = new Vector2((m_cached_GlyphInfo.x + m_cached_GlyphInfo.width) / (float)m_inlineGraphics.spriteAsset.spriteSheet.width, uv.y);
			Vector2 uv4 = new Vector2(uv3.x, uv2.y);
			Color32 color = Color.white;
			color.a = ((m_fontColor32.a >= vertexColor.a) ? vertexColor.a : m_fontColor32.a);
			TMP_Vertex tMP_Vertex = default(TMP_Vertex);
			tMP_Vertex.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
			tMP_Vertex.uv = uv;
			tMP_Vertex.color = color;
			m_textInfo.characterInfo[m_characterCount].vertex_BL = tMP_Vertex;
			tMP_Vertex.position = m_textInfo.characterInfo[m_characterCount].topLeft;
			tMP_Vertex.uv = uv2;
			tMP_Vertex.color = color;
			m_textInfo.characterInfo[m_characterCount].vertex_TL = tMP_Vertex;
			tMP_Vertex.position = m_textInfo.characterInfo[m_characterCount].topRight;
			tMP_Vertex.uv = uv4;
			tMP_Vertex.color = color;
			m_textInfo.characterInfo[m_characterCount].vertex_TR = tMP_Vertex;
			tMP_Vertex.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
			tMP_Vertex.uv = uv3;
			tMP_Vertex.color = color;
			m_textInfo.characterInfo[m_characterCount].vertex_BR = tMP_Vertex;
		}

		private void FillCharacterVertexBuffers(int i, int index_X4)
		{
			TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
			m_textInfo.characterInfo[i].vertexIndex = (short)index_X4;
			m_textInfo.meshInfo.vertices[index_X4] = characterInfo[i].vertex_BL.position;
			m_textInfo.meshInfo.vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			m_textInfo.meshInfo.vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			m_textInfo.meshInfo.vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			m_textInfo.meshInfo.uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			m_textInfo.meshInfo.uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			m_textInfo.meshInfo.uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			m_textInfo.meshInfo.uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			m_textInfo.meshInfo.uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			m_textInfo.meshInfo.uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			m_textInfo.meshInfo.uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			m_textInfo.meshInfo.uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			m_textInfo.meshInfo.colors32[index_X4] = characterInfo[i].vertex_BL.color;
			m_textInfo.meshInfo.colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			m_textInfo.meshInfo.colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			m_textInfo.meshInfo.colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
		}

		private void FillSpriteVertexBuffers(int i, int spriteIndex_X4)
		{
			m_textInfo.characterInfo[i].vertexIndex = (short)spriteIndex_X4;
			TMP_CharacterInfo[] characterInfo = m_textInfo.characterInfo;
			UIVertex[] uiVertex = m_inlineGraphics.uiVertex;
			UIVertex uIVertex = default(UIVertex);
			uIVertex.position = characterInfo[i].vertex_BL.position;
			uIVertex.uv0 = characterInfo[i].vertex_BL.uv;
			uIVertex.color = characterInfo[i].vertex_BL.color;
			uiVertex[spriteIndex_X4] = uIVertex;
			uIVertex.position = characterInfo[i].vertex_TL.position;
			uIVertex.uv0 = characterInfo[i].vertex_TL.uv;
			uIVertex.color = characterInfo[i].vertex_TL.color;
			uiVertex[spriteIndex_X4 + 1] = uIVertex;
			uIVertex.position = characterInfo[i].vertex_TR.position;
			uIVertex.uv0 = characterInfo[i].vertex_TR.uv;
			uIVertex.color = characterInfo[i].vertex_TR.color;
			uiVertex[spriteIndex_X4 + 2] = uIVertex;
			uIVertex.position = characterInfo[i].vertex_BR.position;
			uIVertex.uv0 = characterInfo[i].vertex_BR.uv;
			uIVertex.color = characterInfo[i].vertex_BR.color;
			uiVertex[spriteIndex_X4 + 3] = uIVertex;
			m_inlineGraphics.SetUIVertex(uiVertex);
		}

		private void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, Color32 underlineColor)
		{
			if (m_cached_Underline_GlyphInfo == null)
			{
				Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
				return;
			}
			int num = index + 12;
			if (num > m_textInfo.meshInfo.vertices.Length)
			{
				m_textInfo.meshInfo.ResizeMeshInfo(num / 4);
			}
			start.y = Mathf.Min(start.y, end.y);
			end.y = Mathf.Min(start.y, end.y);
			float num2 = m_cached_Underline_GlyphInfo.width / 2f * maxScale;
			if (end.x - start.x < m_cached_Underline_GlyphInfo.width * maxScale)
			{
				num2 = (end.x - start.x) / 2f;
			}
			float num3 = m_padding * startScale / maxScale;
			float num4 = m_padding * endScale / maxScale;
			float height = m_cached_Underline_GlyphInfo.height;
			Vector3[] vertices = m_textInfo.meshInfo.vertices;
			vertices[index] = start + new Vector3(0f, 0f - (height + m_padding) * maxScale, 0f);
			vertices[index + 1] = start + new Vector3(0f, m_padding * maxScale, 0f);
			vertices[index + 2] = vertices[index + 1] + new Vector3(num2, 0f, 0f);
			vertices[index + 3] = vertices[index] + new Vector3(num2, 0f, 0f);
			vertices[index + 4] = vertices[index + 3];
			vertices[index + 5] = vertices[index + 2];
			vertices[index + 6] = end + new Vector3(0f - num2, m_padding * maxScale, 0f);
			vertices[index + 7] = end + new Vector3(0f - num2, (0f - (height + m_padding)) * maxScale, 0f);
			vertices[index + 8] = vertices[index + 7];
			vertices[index + 9] = vertices[index + 6];
			vertices[index + 10] = end + new Vector3(0f, m_padding * maxScale, 0f);
			vertices[index + 11] = end + new Vector3(0f, (0f - (height + m_padding)) * maxScale, 0f);
			Vector2 vector = new Vector2((m_cached_Underline_GlyphInfo.x - num3) / m_fontAsset.fontInfo.AtlasWidth, 1f - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector2 = new Vector2(vector.x, 1f - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);
			Vector2 vector3 = new Vector2((m_cached_Underline_GlyphInfo.x - num3 + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			Vector2 vector5 = new Vector2((m_cached_Underline_GlyphInfo.x + num4 + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector6 = new Vector2(vector5.x, vector.y);
			Vector2 vector7 = new Vector2((m_cached_Underline_GlyphInfo.x + num4 + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, vector2.y);
			Vector2 vector8 = new Vector2(vector7.x, vector.y);
			Vector2[] uvs = m_textInfo.meshInfo.uvs0;
			uvs[index] = vector;
			uvs[1 + index] = vector2;
			uvs[2 + index] = vector3;
			uvs[3 + index] = vector4;
			uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
			uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
			uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
			uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
			uvs[8 + index] = vector6;
			uvs[9 + index] = vector5;
			uvs[10 + index] = vector7;
			uvs[11 + index] = vector8;
			float num5 = 0f;
			float x = (vertices[index + 2].x - start.x) / (end.x - start.x);
			Vector3 lossyScale = m_rectTransform.lossyScale;
			float num6 = maxScale * lossyScale.z;
			float scale = num6;
			Vector2[] uvs2 = m_textInfo.meshInfo.uvs2;
			uvs2[index] = PackUV(0f, 0f, num6);
			uvs2[1 + index] = PackUV(0f, 1f, num6);
			uvs2[2 + index] = PackUV(x, 1f, num6);
			uvs2[3 + index] = PackUV(x, 0f, num6);
			num5 = (vertices[index + 4].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[4 + index] = PackUV(num5, 0f, scale);
			uvs2[5 + index] = PackUV(num5, 1f, scale);
			uvs2[6 + index] = PackUV(x, 1f, scale);
			uvs2[7 + index] = PackUV(x, 0f, scale);
			num5 = (vertices[index + 8].x - start.x) / (end.x - start.x);
			x = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[8 + index] = PackUV(num5, 0f, num6);
			uvs2[9 + index] = PackUV(num5, 1f, num6);
			uvs2[10 + index] = PackUV(1f, 1f, num6);
			uvs2[11 + index] = PackUV(1f, 0f, num6);
			Color32[] colors = m_textInfo.meshInfo.colors32;
			colors[index] = underlineColor;
			colors[1 + index] = underlineColor;
			colors[2 + index] = underlineColor;
			colors[3 + index] = underlineColor;
			colors[4 + index] = underlineColor;
			colors[5 + index] = underlineColor;
			colors[6 + index] = underlineColor;
			colors[7 + index] = underlineColor;
			colors[8 + index] = underlineColor;
			colors[9 + index] = underlineColor;
			colors[10 + index] = underlineColor;
			colors[11 + index] = underlineColor;
			index += 12;
		}

		private void UpdateSDFScale(float prevScale, float newScale)
		{
			Vector2[] uvs = m_textInfo.meshInfo.uvs2;
			for (int i = 0; i < uvs.Length; i++)
			{
				uvs[i].y = uvs[i].y / prevScale * newScale;
			}
			m_mesh.uv2 = uvs;
			m_uiRenderer.SetMesh(m_mesh);
		}

		private void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 vector = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				m_textInfo.characterInfo[i].bottomLeft -= vector;
				m_textInfo.characterInfo[i].topLeft -= vector;
				m_textInfo.characterInfo[i].topRight -= vector;
				m_textInfo.characterInfo[i].bottomRight -= vector;
				m_textInfo.characterInfo[i].topLine -= vector.y;
				m_textInfo.characterInfo[i].baseLine -= vector.y;
				m_textInfo.characterInfo[i].bottomLine -= vector.y;
				if (m_textInfo.characterInfo[i].isVisible)
				{
					m_textInfo.characterInfo[i].vertex_BL.position -= vector;
					m_textInfo.characterInfo[i].vertex_TL.position -= vector;
					m_textInfo.characterInfo[i].vertex_TR.position -= vector;
					m_textInfo.characterInfo[i].vertex_BR.position -= vector;
				}
			}
		}

		private void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = m_visibleCharacterCount;
			state.firstCharacterIndex = m_firstCharacterOfLine;
			state.visible_SpriteCount = m_visibleSpriteCount;
			state.visible_LinkCount = m_textInfo.linkCount;
			state.firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = m_lastVisibleCharacterOfLine;
			state.xAdvance = m_xAdvance;
			state.maxAscender = m_maxAscender;
			state.maxDescender = m_maxDescender;
			state.preferredWidth = m_preferredWidth;
			state.preferredHeight = m_preferredHeight;
			state.fontScale = m_fontScale;
			state.maxFontScale = m_maxFontScale;
			state.currentFontSize = m_currentFontSize;
			state.lineNumber = m_lineNumber;
			state.lineOffset = m_lineOffset;
			state.baselineOffset = m_baselineOffset;
			state.fontStyle = m_style;
			state.vertexColor = m_htmlColor;
			state.colorStackIndex = m_colorStackIndex;
			state.meshExtents = m_meshExtents;
			state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
		}

		private int RestoreWordWrappingState(ref WordWrapState state)
		{
			m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;
			m_currentFontSize = state.currentFontSize;
			m_fontScale = state.fontScale;
			m_baselineOffset = state.baselineOffset;
			m_style = state.fontStyle;
			m_htmlColor = state.vertexColor;
			m_colorStackIndex = state.colorStackIndex;
			m_characterCount = state.total_CharacterCount + 1;
			m_visibleCharacterCount = state.visible_CharacterCount;
			m_visibleSpriteCount = state.visible_SpriteCount;
			m_textInfo.linkCount = state.visible_LinkCount;
			m_firstCharacterOfLine = state.firstCharacterIndex;
			m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			m_meshExtents = state.meshExtents;
			m_xAdvance = state.xAdvance;
			m_maxAscender = state.maxAscender;
			m_maxDescender = state.maxDescender;
			m_preferredWidth = state.preferredWidth;
			m_preferredHeight = state.preferredHeight;
			m_lineNumber = state.lineNumber;
			m_lineOffset = state.lineOffset;
			m_maxFontScale = state.maxFontScale;
			return state.previous_WordBreak;
		}

		private Vector2 PackUV(float x, float y, float scale)
		{
			x = x % 5f / 5f;
			y = y % 5f / 5f;
			return new Vector2(Mathf.Round(x * 4096f) + y, scale);
		}

		private void ResizeLineExtents(int size)
		{
			size = ((size <= 1024) ? Mathf.NextPowerOfTwo(size + 1) : (size + 256));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < m_textInfo.lineInfo.Length)
				{
					array[i] = m_textInfo.lineInfo[i];
					continue;
				}
				array[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
				array[i].ascender = 0f - k_InfinityVector.x;
				array[i].descender = k_InfinityVector.x;
			}
			m_textInfo.lineInfo = array;
		}

		private int HexToInt(char hex)
		{
			switch (hex)
			{
			case '0':
				return 0;
			case '1':
				return 1;
			case '2':
				return 2;
			case '3':
				return 3;
			case '4':
				return 4;
			case '5':
				return 5;
			case '6':
				return 6;
			case '7':
				return 7;
			case '8':
				return 8;
			case '9':
				return 9;
			case 'A':
				return 10;
			case 'B':
				return 11;
			case 'C':
				return 12;
			case 'D':
				return 13;
			case 'E':
				return 14;
			case 'F':
				return 15;
			case 'a':
				return 10;
			case 'b':
				return 11;
			case 'c':
				return 12;
			case 'd':
				return 13;
			case 'e':
				return 14;
			case 'f':
				return 15;
			default:
				return 15;
			}
		}

		private int GetUTF16(int i)
		{
			int num = HexToInt(text[i]) * 4096;
			num += HexToInt(text[i + 1]) * 256;
			num += HexToInt(text[i + 2]) * 16;
			return num + HexToInt(text[i + 3]);
		}

		private int GetUTF32(int i)
		{
			int num = 0;
			num += HexToInt(text[i]) * 268435456;
			num += HexToInt(text[i + 1]) * 16777216;
			num += HexToInt(text[i + 2]) * 1048576;
			num += HexToInt(text[i + 3]) * 65536;
			num += HexToInt(text[i + 4]) * 4096;
			num += HexToInt(text[i + 5]) * 256;
			num += HexToInt(text[i + 6]) * 16;
			return num + HexToInt(text[i + 7]);
		}

		private Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			switch (tagCount)
			{
			case 7:
			{
				byte r4 = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
				byte g4 = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
				byte b4 = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
				return new Color32(r4, g4, b4, byte.MaxValue);
			}
			case 9:
			{
				byte r3 = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
				byte g3 = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
				byte b3 = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
				byte a2 = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
				return new Color32(r3, g3, b3, a2);
			}
			case 13:
			{
				byte r2 = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
				byte g2 = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
				byte b2 = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
				return new Color32(r2, g2, b2, byte.MaxValue);
			}
			case 15:
			{
				byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
				byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
				byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
				byte a = (byte)(HexToInt(hexChars[13]) * 16 + HexToInt(hexChars[14]));
				return new Color32(r, g, b, a);
			}
			default:
				return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}

		private float ConvertToFloat(char[] chars, int startIndex, int endIndex, int decimalPointIndex)
		{
			if (startIndex == 0)
			{
				return 0f;
			}
			float num = 0f;
			float num2 = 1f;
			decimalPointIndex = ((decimalPointIndex <= 0) ? (endIndex + 1) : decimalPointIndex);
			if (chars[startIndex] == '-')
			{
				startIndex++;
				num2 = -1f;
			}
			if (chars[startIndex] == '+' || chars[startIndex] == '%')
			{
				startIndex++;
			}
			for (int i = startIndex; i < endIndex + 1; i++)
			{
				if (!char.IsDigit(chars[i]) && chars[i] != '.')
				{
					return -9999f;
				}
				switch (decimalPointIndex - i)
				{
				case 4:
					num += (float)((chars[i] - 48) * 1000);
					break;
				case 3:
					num += (float)((chars[i] - 48) * 100);
					break;
				case 2:
					num += (float)((chars[i] - 48) * 10);
					break;
				case 1:
					num += (float)(chars[i] - 48);
					break;
				case -1:
					num += (float)(chars[i] - 48) * 0.1f;
					break;
				case -2:
					num += (float)(chars[i] - 48) * 0.01f;
					break;
				case -3:
					num += (float)(chars[i] - 48) * 0.001f;
					break;
				}
			}
			return num * num2;
		}

		private bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
		{
			Array.Clear(m_htmlTag, 0, m_htmlTag.Length);
			int num = 0;
			int num2 = 0;
			TagAttribute tagAttribute = default(TagAttribute);
			TagAttribute tagAttribute2 = default(TagAttribute);
			byte b = 0;
			TagUnits tagUnits = TagUnits.Pixels;
			int num3 = 0;
			int num4 = 0;
			int decimalPointIndex = 0;
			int num5 = 0;
			endIndex = startIndex;
			bool flag = false;
			bool flag2 = false;
			for (int i = startIndex; i < chars.Length && chars[i] != 0; i++)
			{
				if (num >= m_htmlTag.Length)
				{
					break;
				}
				if (chars[i] == 60)
				{
					break;
				}
				if (chars[i] == 62)
				{
					flag = true;
					endIndex = i;
					m_htmlTag[num] = '\0';
					if (num4 == 0)
					{
						num4 = num - 1;
					}
					break;
				}
				m_htmlTag[num] = (char)chars[i];
				num++;
				if (b == 1)
				{
					if (chars[i] != 34)
					{
						if (tagAttribute.startIndex == 0)
						{
							tagAttribute.startIndex = num - 1;
						}
						tagAttribute.hashCode = (tagAttribute.hashCode << 5) - tagAttribute.hashCode + chars[i];
						tagAttribute.length++;
					}
					else if (tagAttribute.startIndex != 0)
					{
						b = 2;
					}
				}
				if (b == 3)
				{
					if (chars[i] != 34)
					{
						if (tagAttribute2.startIndex == 0)
						{
							tagAttribute2.startIndex = num - 1;
						}
						tagAttribute2.hashCode = (tagAttribute2.hashCode << 5) - tagAttribute2.hashCode + chars[i];
						tagAttribute2.length++;
					}
					else if (tagAttribute2.startIndex != 0)
					{
						b = 0;
					}
				}
				if (chars[i] == 61)
				{
					num3 = num;
					b = (byte)(b + 1);
				}
				else if (chars[i] == 46)
				{
					decimalPointIndex = num - 1;
				}
				else if (num3 != 0 && !flag2 && char.IsDigit((char)chars[i]))
				{
					flag2 = true;
				}
				else if (num3 != 0 && num5 == 0 && (chars[i] == 112 || chars[i] == 101 || chars[i] == 37))
				{
					num4 = num - 2;
					num5 = num - 1;
					if (chars[i] == 101)
					{
						tagUnits = TagUnits.FontUnits;
					}
					else if (chars[i] == 37)
					{
						tagUnits = TagUnits.Percentage;
					}
				}
				if (num3 == 0)
				{
					num2 = (num2 << 3) - num2 + chars[i];
				}
			}
			if (!flag)
			{
				return false;
			}
			if (tag_NoParsing && num2 != 53822163)
			{
				return false;
			}
			if (num2 == 53822163)
			{
				tag_NoParsing = false;
				return true;
			}
			if (m_htmlTag[0] == '#' && num == 7)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				m_colorStack[m_colorStackIndex] = m_htmlColor;
				m_colorStackIndex++;
				return true;
			}
			if (m_htmlTag[0] == '#' && num == 9)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				m_colorStack[m_colorStackIndex] = m_htmlColor;
				m_colorStackIndex++;
				return true;
			}
			float num6 = 0f;
			switch (num2)
			{
			case 98:
				m_style |= FontStyles.Bold;
				return true;
			case 427:
				if ((m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
				{
					m_style &= (FontStyles)(-2);
				}
				return true;
			case 105:
				m_style |= FontStyles.Italic;
				return true;
			case 434:
				m_style &= (FontStyles)(-3);
				return true;
			case 115:
				m_style |= FontStyles.Strikethrough;
				return true;
			case 444:
				if ((m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
				{
					m_style &= (FontStyles)(-65);
				}
				return true;
			case 117:
				m_style |= FontStyles.Underline;
				return true;
			case 446:
				if ((m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
				{
					m_style &= (FontStyles)(-5);
				}
				return true;
			case 6552:
				m_currentFontSize *= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
				m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
				m_baselineOffset = m_fontAsset.fontInfo.SubscriptOffset * m_fontScale;
				m_style |= FontStyles.Subscript;
				return true;
			case 22673:
				m_currentFontSize /= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
				m_baselineOffset = 0f;
				m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
				m_style &= (FontStyles)(-257);
				return true;
			case 6566:
				m_currentFontSize *= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
				m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
				m_baselineOffset = m_fontAsset.fontInfo.SuperscriptOffset * m_fontScale;
				m_style |= FontStyles.Superscript;
				return true;
			case 22687:
				m_currentFontSize /= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
				m_baselineOffset = 0f;
				m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
				m_style &= (FontStyles)(-129);
				return true;
			case 6380:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_xAdvance = num6;
					return true;
				case TagUnits.FontUnits:
					m_xAdvance = num6 * m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					return true;
				case TagUnits.Percentage:
					m_xAdvance = m_marginWidth * num6 / 100f;
					return true;
				default:
					return false;
				}
			case 22501:
				m_isIgnoringAlignment = false;
				return true;
			case 16034505:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_baselineOffset = num6;
					return true;
				case TagUnits.FontUnits:
					m_baselineOffset = num6 * m_fontScale * m_fontAsset.fontInfo.Ascender;
					return true;
				case TagUnits.Percentage:
					return false;
				default:
					return false;
				}
			case 54741026:
				m_baselineOffset = 0f;
				return true;
			case 43991:
				if (m_overflowMode == TextOverflowModes.Page)
				{
					m_xAdvance = tag_LineIndent + tag_Indent;
					m_lineOffset = 0f;
					m_pageNumber++;
					m_isNewPage = true;
				}
				return true;
			case 43969:
				m_isNonBreakingSpace = true;
				return true;
			case 156816:
				m_isNonBreakingSpace = false;
				return true;
			case 45545:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					if (m_htmlTag[5] == '+')
					{
						m_currentFontSize = m_fontSize + num6;
						m_isRecalculateScaleRequired = true;
						return true;
					}
					if (m_htmlTag[5] == '-')
					{
						m_currentFontSize = m_fontSize + num6;
						m_isRecalculateScaleRequired = true;
						return true;
					}
					m_currentFontSize = num6;
					m_isRecalculateScaleRequired = true;
					return true;
				case TagUnits.FontUnits:
					m_currentFontSize *= num6;
					m_isRecalculateScaleRequired = true;
					return true;
				case TagUnits.Percentage:
					m_currentFontSize = m_fontSize * num6 / 100f;
					m_isRecalculateScaleRequired = true;
					return true;
				default:
					return false;
				}
			case 158392:
				m_currentFontSize = m_fontSize;
				m_isRecalculateScaleRequired = true;
				return true;
			case 41311:
			{
				int hashCode = tagAttribute.hashCode;
				int hashCode2 = tagAttribute2.hashCode;
				if (m_fontAsset_Dict.TryGetValue(hashCode, out TextMeshProFont value))
				{
					if (value != m_currentFontAsset)
					{
						m_currentFontAsset = m_fontAsset_Dict[hashCode];
						m_isRecalculateScaleRequired = true;
					}
				}
				else
				{
					value = (Resources.Load("Fonts & Materials/" + new string(m_htmlTag, tagAttribute.startIndex, tagAttribute.length), typeof(TextMeshProFont)) as TextMeshProFont);
					if (!(value != null))
					{
						return false;
					}
					m_fontAsset_Dict.Add(hashCode, value);
					m_currentFontAsset = value;
					m_isRecalculateScaleRequired = true;
				}
				Material value2;
				if (hashCode2 == 0)
				{
					if (!m_fontMaterial_Dict.TryGetValue(m_currentFontAsset.materialHashCode, out value2))
					{
						m_fontMaterial_Dict.Add(m_currentFontAsset.materialHashCode, m_currentFontAsset.material);
					}
					if (m_currentMaterial != m_currentFontAsset.material)
					{
						m_currentMaterial = m_currentFontAsset.material;
					}
				}
				else if (m_fontMaterial_Dict.TryGetValue(hashCode2, out value2))
				{
					if (value2 != m_currentMaterial)
					{
						m_currentMaterial = value2;
					}
				}
				else
				{
					value2 = (Resources.Load("Fonts & Materials/" + new string(m_htmlTag, tagAttribute2.startIndex, tagAttribute2.length), typeof(Material)) as Material);
					if (!(value2 != null))
					{
						return false;
					}
					m_fontMaterial_Dict.Add(hashCode2, value2);
					m_currentMaterial = value2;
				}
				return true;
			}
			case 320078:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_xAdvance += num6;
					return true;
				case TagUnits.FontUnits:
					m_xAdvance += num6 * m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					return true;
				case TagUnits.Percentage:
					return false;
				default:
					return false;
				}
			case 276254:
				m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
				return true;
			case 1750458:
				return true;
			case 426:
				return true;
			case 43066:
				if (m_isParsingText)
				{
					tag_LinkInfo.hashCode = tagAttribute.hashCode;
					tag_LinkInfo.firstCharacterIndex = m_characterCount;
				}
				return true;
			case 155913:
				if (m_isParsingText)
				{
					tag_LinkInfo.lastCharacterIndex = m_characterCount - 1;
					tag_LinkInfo.characterCount = m_characterCount - tag_LinkInfo.firstCharacterIndex;
					m_textInfo.linkInfo.Add(tag_LinkInfo);
					m_textInfo.linkCount++;
				}
				return true;
			case 275917:
				switch (tagAttribute.hashCode)
				{
				case 3317767:
					m_lineJustification = TextAlignmentOptions.Left;
					return true;
				case 108511772:
					m_lineJustification = TextAlignmentOptions.Right;
					return true;
				case -1364013995:
					m_lineJustification = TextAlignmentOptions.Center;
					return true;
				case 1838536479:
					m_lineJustification = TextAlignmentOptions.Justified;
					return true;
				default:
					return false;
				}
			case 1065846:
				m_lineJustification = m_textAlignment;
				return true;
			case 327550:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_width = num6;
					break;
				case TagUnits.FontUnits:
					return false;
				case TagUnits.Percentage:
					m_width = m_marginWidth * num6 / 100f;
					break;
				}
				return true;
			case 1117479:
				m_width = -1f;
				return true;
			case 322689:
			{
				TMP_Style style = TMP_StyleSheet.Instance.GetStyle(tagAttribute.hashCode);
				if (style == null)
				{
					return false;
				}
				m_styleStack[m_styleStackIndex] = style.hashCode;
				m_styleStackIndex++;
				for (int k = 0; k < style.styleOpeningTagArray.Length; k++)
				{
					if (style.styleOpeningTagArray[k] == 60)
					{
						ValidateHtmlTag(style.styleOpeningTagArray, k + 1, out k);
					}
				}
				return true;
			}
			case 1112618:
			{
				TMP_Style style = TMP_StyleSheet.Instance.GetStyle(tagAttribute.hashCode);
				if (style == null)
				{
					m_styleStackIndex = ((m_styleStackIndex > 0) ? (m_styleStackIndex - 1) : 0);
					style = TMP_StyleSheet.Instance.GetStyle(m_styleStack[m_styleStackIndex]);
				}
				if (style == null)
				{
					return false;
				}
				for (int j = 0; j < style.styleClosingTagArray.Length; j++)
				{
					if (style.styleClosingTagArray[j] == 60)
					{
						ValidateHtmlTag(style.styleClosingTagArray, j + 1, out j);
					}
				}
				return true;
			}
			case 281955:
				if (m_htmlTag[6] == '#' && num == 13)
				{
					m_htmlColor = HexCharsToColor(m_htmlTag, num);
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				}
				if (m_htmlTag[6] == '#' && num == 15)
				{
					m_htmlColor = HexCharsToColor(m_htmlTag, num);
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				}
				switch (tagAttribute.hashCode)
				{
				case 112785:
					m_htmlColor = Color.red;
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case 3027034:
					m_htmlColor = Color.blue;
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case 93818879:
					m_htmlColor = Color.black;
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case 98619139:
					m_htmlColor = Color.green;
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case 113101865:
					m_htmlColor = Color.white;
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case -1008851410:
					m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case -976943172:
					m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				case -734239628:
					m_htmlColor = Color.yellow;
					m_colorStack[m_colorStackIndex] = m_htmlColor;
					m_colorStackIndex++;
					return true;
				default:
					return false;
				}
			case 1983971:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_cSpacing = num6;
					break;
				case TagUnits.FontUnits:
					m_cSpacing = num6;
					m_cSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					return false;
				}
				return true;
			case 7513474:
				m_cSpacing = 0f;
				return true;
			case 2152041:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_monoSpacing = num6;
					break;
				case TagUnits.FontUnits:
					m_monoSpacing = num6;
					m_monoSpacing *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					return false;
				}
				return true;
			case 7681544:
				m_monoSpacing = 0f;
				return true;
			case 280416:
				return false;
			case 1071884:
				m_colorStackIndex--;
				if (m_colorStackIndex <= 0)
				{
					m_htmlColor = m_fontColor32;
					m_colorStackIndex = 0;
				}
				else
				{
					m_htmlColor = m_colorStack[m_colorStackIndex - 1];
				}
				return true;
			case 2068980:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					tag_Indent = num6;
					break;
				case TagUnits.FontUnits:
					tag_Indent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					tag_Indent = m_marginWidth * tag_Indent / 100f;
					break;
				}
				m_xAdvance = tag_Indent;
				return true;
			case 7598483:
				tag_Indent = 0f;
				return true;
			case 1109386397:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					tag_LineIndent = num6;
					break;
				case TagUnits.FontUnits:
					tag_LineIndent = num6;
					tag_LineIndent *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					tag_LineIndent = m_marginWidth * tag_LineIndent / 100f;
					break;
				}
				m_xAdvance += tag_LineIndent;
				return true;
			case -445537194:
				tag_LineIndent = 0f;
				return true;
			case 2246877:
				if (m_inlineGraphics == null)
				{
					m_inlineGraphics = (GetComponent<InlineGraphicManager>() ?? base.gameObject.AddComponent<InlineGraphicManager>());
				}
				if (char.IsDigit(m_htmlTag[7]))
				{
					int index = (int)ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
					m_spriteIndex = m_inlineGraphics.GetSpriteIndexByIndex(index);
					if (m_spriteIndex == -1)
					{
						return false;
					}
				}
				else
				{
					m_spriteIndex = m_inlineGraphics.GetSpriteIndexByHashCode(tagAttribute.hashCode);
					if (m_spriteIndex == -1)
					{
						return false;
					}
				}
				m_isSprite = true;
				return true;
			case 13526026:
				m_style |= FontStyles.UpperCase;
				return true;
			case 52232547:
				m_style &= (FontStyles)(-17);
				return true;
			case 766244328:
				m_style |= FontStyles.SmallCaps;
				return true;
			case -1632103439:
				m_style &= (FontStyles)(-33);
				m_isRecalculateScaleRequired = true;
				return true;
			case 2109854:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				m_marginLeft = num6;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					m_marginLeft = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginLeft / 100f;
					break;
				}
				m_marginLeft = ((!(m_marginLeft >= 0f)) ? 0f : m_marginLeft);
				m_marginRight = m_marginLeft;
				return true;
			case 7639357:
				m_marginLeft = 0f;
				m_marginRight = 0f;
				return true;
			case 1100728678:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				m_marginLeft = num6;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_marginLeft *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					m_marginLeft = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginLeft / 100f;
					break;
				}
				m_marginLeft = ((!(m_marginLeft >= 0f)) ? 0f : m_marginLeft);
				return true;
			case -884817987:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				m_marginRight = num6;
				switch (tagUnits)
				{
				case TagUnits.FontUnits:
					m_marginRight *= m_fontScale * m_fontAsset.fontInfo.TabWidth / (float)(int)m_fontAsset.TabSize;
					break;
				case TagUnits.Percentage:
					m_marginRight = (m_marginWidth - ((m_width == -1f) ? 0f : m_width)) * m_marginRight / 100f;
					break;
				}
				m_marginRight = ((!(m_marginRight >= 0f)) ? 0f : m_marginRight);
				return true;
			case 1109349752:
				num6 = ConvertToFloat(m_htmlTag, num3, num4, decimalPointIndex);
				if (num6 == -9999f || num6 == 0f)
				{
					return false;
				}
				m_lineHeight = num6;
				switch (tagUnits)
				{
				case TagUnits.Pixels:
					m_lineHeight /= m_fontScale;
					break;
				case TagUnits.FontUnits:
					m_lineHeight *= m_fontAsset.fontInfo.LineHeight;
					break;
				case TagUnits.Percentage:
					m_lineHeight = m_fontAsset.fontInfo.LineHeight * m_lineHeight / 100f;
					break;
				}
				return true;
			case -445573839:
				m_lineHeight = 0f;
				return true;
			case 15115642:
				tag_NoParsing = true;
				return true;
			default:
				return false;
			}
		}

		private float GetPreferredWidth()
		{
			TextOverflowModes overflowMode = m_overflowMode;
			m_overflowMode = TextOverflowModes.Overflow;
			m_renderMode = TextRenderFlags.GetPreferredSizes;
			GenerateTextMesh();
			m_renderMode = TextRenderFlags.Render;
			m_overflowMode = overflowMode;
			Debug.Log("GetPreferredWidth() Called. Returning width of " + m_preferredWidth);
			return m_preferredWidth;
		}

		private float GetPreferredHeight()
		{
			TextOverflowModes overflowMode = m_overflowMode;
			m_overflowMode = TextOverflowModes.Overflow;
			m_renderMode = TextRenderFlags.GetPreferredSizes;
			GenerateTextMesh();
			m_renderMode = TextRenderFlags.Render;
			m_overflowMode = overflowMode;
			Debug.Log("GetPreferredHeight() Called. Returning height of " + m_preferredHeight);
			return m_preferredHeight;
		}

		public void CalculateLayoutInputHorizontal()
		{
			if (base.gameObject.activeInHierarchy)
			{
				IsRectTransformDriven = true;
				m_currentAutoSizeMode = m_enableAutoSizing;
				m_LayoutPhase = AutoLayoutPhase.Horizontal;
				m_isRebuildingLayout = true;
				m_minWidth = 0f;
				m_flexibleWidth = 0f;
				m_renderMode = TextRenderFlags.GetPreferredSizes;
				if (m_enableAutoSizing)
				{
					m_fontSize = m_fontSizeMax;
				}
				m_marginWidth = float.PositiveInfinity;
				m_marginHeight = float.PositiveInfinity;
				if (isInputParsingRequired || m_isTextTruncated)
				{
					ParseInputText();
				}
				GenerateTextMesh();
				m_renderMode = TextRenderFlags.Render;
				m_preferredWidth = m_renderedWidth;
				ComputeMarginSize();
				m_isLayoutDirty = true;
			}
		}

		public void CalculateLayoutInputVertical()
		{
			if (base.gameObject.activeInHierarchy)
			{
				IsRectTransformDriven = true;
				m_LayoutPhase = AutoLayoutPhase.Vertical;
				m_isRebuildingLayout = true;
				m_minHeight = 0f;
				m_flexibleHeight = 0f;
				m_renderMode = TextRenderFlags.GetPreferredSizes;
				if (m_enableAutoSizing)
				{
					m_currentAutoSizeMode = true;
					m_enableAutoSizing = false;
				}
				m_marginHeight = float.PositiveInfinity;
				GenerateTextMesh();
				m_enableAutoSizing = m_currentAutoSizeMode;
				m_renderMode = TextRenderFlags.Render;
				ComputeMarginSize();
				m_preferredHeight = m_renderedHeight;
				m_isLayoutDirty = true;
				m_isCalculateSizeRequired = false;
			}
		}

		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		public override void RecalculateMasking()
		{
			if (m_fontAsset == null || !m_isAwake)
			{
				return;
			}
			m_stencilID = MaterialManager.GetStencilID(base.gameObject);
			if (m_stencilID == 0)
			{
				if (m_maskingMaterial != null)
				{
					MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
					m_maskingMaterial = null;
					m_sharedMaterial = m_baseMaterial;
				}
				else if (m_fontMaterial != null)
				{
					m_sharedMaterial = MaterialManager.SetStencil(m_fontMaterial, 0);
				}
				else
				{
					m_sharedMaterial = m_baseMaterial;
				}
			}
			else
			{
				ShaderUtilities.GetShaderPropertyIDs();
				if (m_fontMaterial != null)
				{
					m_sharedMaterial = MaterialManager.SetStencil(m_fontMaterial, m_stencilID);
				}
				else if (m_maskingMaterial == null)
				{
					m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
					m_sharedMaterial = m_maskingMaterial;
				}
				else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
				{
					MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
					m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
					m_sharedMaterial = m_maskingMaterial;
				}
				if (m_isMaskingEnabled)
				{
					EnableMasking();
				}
			}
			m_uiRenderer.SetMaterial(m_sharedMaterial, m_sharedMaterial.mainTexture);
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
		}

		protected override void UpdateGeometry()
		{
		}

		protected override void UpdateMaterial()
		{
		}

		public void UpdateMeshPadding()
		{
			m_padding = ShaderUtilities.GetPadding(new Material[1]
			{
				m_uiRenderer.GetMaterial()
			}, m_enableExtraPadding, m_isUsingBold);
			m_havePropertiesChanged = true;
		}

		public void ForceMeshUpdate()
		{
			OnPreRenderCanvas();
		}

		public void UpdateFontAsset()
		{
			LoadFontAsset();
		}

		public TMP_TextInfo GetTextInfo(string text)
		{
			StringToCharArray(text, ref m_char_buffer);
			m_renderMode = TextRenderFlags.DontRender;
			GenerateTextMesh();
			m_renderMode = TextRenderFlags.Render;
			return textInfo;
		}

		public void SetText(string text, float arg0)
		{
			SetText(text, arg0, 255f, 255f);
		}

		public void SetText(string text, float arg0, float arg1)
		{
			SetText(text, arg0, arg1, 255f);
		}

		public void SetText(string text, float arg0, float arg1, float arg2)
		{
			if (text == old_text && arg0 == old_arg0 && arg1 == old_arg1 && arg2 == old_arg2)
			{
				return;
			}
			old_text = text;
			old_arg1 = 255f;
			old_arg2 = 255f;
			int precision = 0;
			int index = 0;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '{')
				{
					if (text[i + 2] == ':')
					{
						precision = text[i + 3] - 48;
					}
					switch (text[i + 1] - 48)
					{
					case 0:
						old_arg0 = arg0;
						AddFloatToCharArray(arg0, ref index, precision);
						break;
					case 1:
						old_arg1 = arg1;
						AddFloatToCharArray(arg1, ref index, precision);
						break;
					case 2:
						old_arg2 = arg2;
						AddFloatToCharArray(arg2, ref index, precision);
						break;
					}
					i = ((text[i + 2] != ':') ? (i + 2) : (i + 4));
				}
				else
				{
					m_input_CharArray[index] = c;
					index++;
				}
			}
			m_input_CharArray[index] = '\0';
			m_charArray_Length = index;
			m_inputSource = TextInputSources.SetText;
			isInputParsingRequired = true;
			m_havePropertiesChanged = true;
		}

		public void SetCharArray(char[] charArray)
		{
			if (charArray == null || charArray.Length == 0)
			{
				return;
			}
			if (m_char_buffer.Length <= charArray.Length)
			{
				int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
				m_char_buffer = new int[num];
			}
			int num2 = 0;
			for (int i = 0; i < charArray.Length; i++)
			{
				if (charArray[i] == '\\' && i < charArray.Length - 1)
				{
					switch (charArray[i + 1])
					{
					case 'n':
						m_char_buffer[num2] = 10;
						i++;
						num2++;
						continue;
					case 'r':
						m_char_buffer[num2] = 13;
						i++;
						num2++;
						continue;
					case 't':
						m_char_buffer[num2] = 9;
						i++;
						num2++;
						continue;
					}
				}
				m_char_buffer[num2] = charArray[i];
				num2++;
			}
			m_char_buffer[num2] = 0;
			m_inputSource = TextInputSources.SetCharArray;
			m_havePropertiesChanged = true;
			isInputParsingRequired = true;
		}
	}
}
