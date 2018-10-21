using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace TMPro
{
	[AddComponentMenu("Mesh/TextMesh Pro")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(TextContainer))]
	public class TextMeshPro : MonoBehaviour
	{
		private enum TextInputSources
		{
			Text,
			SetText,
			SetCharArray
		}

		[SerializeField]
		private string m_text;

		[SerializeField]
		private TextMeshProFont m_fontAsset;

		private Material m_fontMaterial;

		private Material m_sharedMaterial;

		[SerializeField]
		private FontStyles m_fontStyle;

		private FontStyles m_style;

		[SerializeField]
		private bool m_isOverlay;

		[SerializeField]
		private Color m_fontColor = Color.white;

		private Color32 m_fontColor32 = Color.white;

		[SerializeField]
		private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		[SerializeField]
		private bool m_enableVertexGradient;

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
		private float m_charSpacingMax;

		[SerializeField]
		private float m_lineSpacingMax;

		private float m_currentFontSize;

		[SerializeField]
		private float m_characterSpacing;

		private float m_cSpacing;

		private float m_monoSpacing;

		[SerializeField]
		private Vector4 m_textRectangle;

		[SerializeField]
		private float m_lineSpacing;

		private float m_lineSpacingDelta;

		private float m_lineHeight;

		[SerializeField]
		private float m_paragraphSpacing;

		[SerializeField]
		private float m_lineLength;

		[SerializeField]
		private TMP_Compatibility.AnchorPositions m_anchor;

		[SerializeField]
		private TextAlignmentOptions m_textAlignment;

		private TextAlignmentOptions m_lineJustification;

		[SerializeField]
		private bool m_enableKerning;

		private bool m_anchorDampening;

		private float m_baseDampeningWidth;

		[SerializeField]
		private bool m_overrideHtmlColors;

		[SerializeField]
		private bool m_enableExtraPadding;

		[SerializeField]
		private bool checkPaddingRequired;

		[SerializeField]
		private bool m_enableWordWrapping;

		private bool m_isCharacterWrappingEnabled;

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
		private bool havePropertiesChanged;

		[SerializeField]
		private bool hasFontAssetChanged;

		[SerializeField]
		private bool m_isRichText = true;

		[SerializeField]
		private TextInputSources m_inputSource;

		private string old_text;

		private float old_arg0;

		private float old_arg1;

		private float old_arg2;

		private int m_fontIndex;

		private float m_fontScale;

		private bool m_isRecalculateScaleRequired;

		private Vector3 m_lossyScale;

		private float m_xAdvance;

		private float m_indent;

		private float m_maxXAdvance;

		private Vector3 m_anchorOffset;

		private TMP_TextInfo m_textInfo;

		private char[] m_htmlTag = new char[128];

		[SerializeField]
		private Renderer m_renderer;

		private MeshFilter m_meshFilter;

		private Mesh m_mesh;

		private Transform m_transform;

		private Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		private float m_tabSpacing;

		private float m_spacing;

		private Vector2[] m_spacePositions = new Vector2[8];

		private float m_baselineOffset;

		private float m_padding;

		private Vector4 m_alignmentPadding;

		private bool m_isUsingBold;

		private Vector2 k_InfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

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

		private int m_firstVisibleCharacterOfLine;

		private int m_lastVisibleCharacterOfLine;

		private int m_lineNumber;

		private int m_pageNumber;

		private float m_maxAscender;

		private float m_maxDescender;

		private float m_maxFontScale;

		private float m_lineOffset;

		private Extents m_meshExtents;

		private float m_preferredWidth;

		private float m_preferredHeight;

		private Vector3[] m_vertices;

		private Vector3[] m_normals;

		private Vector4[] m_tangents;

		private Vector2[] m_uvs;

		private Vector2[] m_uv2s;

		private Color32[] m_vertColors;

		private int[] m_triangles;

		private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000f, 1000f, 0f));

		[SerializeField]
		private bool m_ignoreCulling = true;

		[SerializeField]
		private bool m_isOrthographic;

		[SerializeField]
		private bool m_isCullingEnabled;

		private int m_sortingLayerID;

		private int m_maxVisibleCharacters = -1;

		private int m_maxVisibleLines = -1;

		[SerializeField]
		private int m_pageToDisplay;

		private bool m_isNewPage;

		private bool m_isTextTruncated;

		[SerializeField]
		private TextMeshProFont[] m_fontAssetArray;

		private List<Material> m_sharedMaterials = new List<Material>(16);

		private int m_selectedFontAsset;

		private MaterialPropertyBlock m_maskingPropertyBlock;

		private bool m_isMaskingEnabled;

		private bool isMaskUpdateRequired;

		private bool m_isMaterialBlockSet;

		[SerializeField]
		private MaskingTypes m_maskType;

		private Matrix4x4 m_EnvMapMatrix = default(Matrix4x4);

		private TextRenderFlags m_renderMode;

		[SerializeField]
		private bool m_isNewTextObject;

		private TextContainer m_textContainer;

		private float m_marginWidth;

		[SerializeField]
		private bool m_enableAutoSizing;

		private float m_maxFontSize;

		private float m_minFontSize;

		private Stopwatch m_StopWatch;

		private bool isDebugOutputDone;

		private int m_recursiveCount;

		private int loopCountA;

		private int loopCountB;

		private int loopCountC;

		private int loopCountD;

		private int loopCountE;

		private GameObject m_prefabParent;

		public string text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_inputSource = TextInputSources.Text;
				havePropertiesChanged = true;
				isInputParsingRequired = true;
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
					havePropertiesChanged = true;
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
				havePropertiesChanged = true;
			}
		}

		public Material fontSharedMaterial
		{
			get
			{
				return m_renderer.sharedMaterial;
			}
			set
			{
				if (m_sharedMaterial != value)
				{
					SetSharedFontMaterial(value);
					havePropertiesChanged = true;
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
				m_isOverlay = value;
				SetShaderType();
				havePropertiesChanged = true;
			}
		}

		public Color color
		{
			get
			{
				return m_fontColor;
			}
			set
			{
				if (!m_fontColor.Compare(value))
				{
					havePropertiesChanged = true;
					m_fontColor = value;
				}
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
				havePropertiesChanged = true;
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
				havePropertiesChanged = true;
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
				if (!m_faceColor.Compare(value))
				{
					SetFaceColor(value);
					havePropertiesChanged = true;
					m_faceColor = value;
				}
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
				if (!m_outlineColor.Compare(value))
				{
					SetOutlineColor(value);
					havePropertiesChanged = true;
					m_outlineColor = value;
				}
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
				havePropertiesChanged = true;
				checkPaddingRequired = true;
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
				if (m_fontSize != value)
				{
					havePropertiesChanged = true;
					m_fontSize = value;
				}
			}
		}

		public float fontScale => m_fontScale;

		public FontStyles fontStyle
		{
			get
			{
				return m_fontStyle;
			}
			set
			{
				m_fontStyle = value;
				havePropertiesChanged = true;
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
					havePropertiesChanged = true;
					m_characterSpacing = value;
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
				havePropertiesChanged = true;
				isInputParsingRequired = true;
			}
		}

		[Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
		public float lineLength
		{
			get
			{
				return m_lineLength;
			}
			set
			{
				UnityEngine.Debug.Log("lineLength set called.");
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
				havePropertiesChanged = true;
			}
		}

		public Bounds bounds
		{
			get
			{
				if (m_mesh != null)
				{
					return m_mesh.bounds;
				}
				return default(Bounds);
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
					havePropertiesChanged = true;
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
					havePropertiesChanged = true;
					m_paragraphSpacing = value;
				}
			}
		}

		[Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
		public TMP_Compatibility.AnchorPositions anchor
		{
			get
			{
				return m_anchor;
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
					havePropertiesChanged = true;
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
					havePropertiesChanged = true;
					m_enableKerning = value;
				}
			}
		}

		public bool anchorDampening
		{
			get
			{
				return m_anchorDampening;
			}
			set
			{
				if (m_anchorDampening != value)
				{
					havePropertiesChanged = true;
					m_anchorDampening = value;
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
					havePropertiesChanged = true;
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
					havePropertiesChanged = true;
					checkPaddingRequired = true;
					m_enableExtraPadding = value;
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
					havePropertiesChanged = true;
					isInputParsingRequired = true;
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
					havePropertiesChanged = true;
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
					havePropertiesChanged = true;
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
					havePropertiesChanged = true;
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
				havePropertiesChanged = true;
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
				havePropertiesChanged = true;
			}
		}

		public int sortingLayerID
		{
			get
			{
				return m_renderer.sortingLayerID;
			}
			set
			{
				m_renderer.sortingLayerID = value;
			}
		}

		public int sortingOrder
		{
			get
			{
				return m_renderer.sortingOrder;
			}
			set
			{
				m_renderer.sortingOrder = value;
			}
		}

		public bool hasChanged
		{
			get
			{
				return havePropertiesChanged;
			}
			set
			{
				havePropertiesChanged = value;
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
				havePropertiesChanged = true;
			}
		}

		public TextContainer textContainer => m_textContainer;

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
					havePropertiesChanged = true;
					m_maxVisibleCharacters = value;
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
					havePropertiesChanged = true;
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
				havePropertiesChanged = true;
				m_pageToDisplay = value;
			}
		}

		public float preferredWidth => m_preferredWidth;

		public Vector2[] spacePositions => m_spacePositions;

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

		public MaskingTypes maskType
		{
			get
			{
				return m_maskType;
			}
			set
			{
				m_maskType = value;
				havePropertiesChanged = true;
				isMaskUpdateRequired = true;
			}
		}

		public TMP_TextInfo textInfo => m_textInfo;

		public Mesh mesh => m_mesh;

		private void Awake()
		{
			m_textContainer = GetComponent<TextContainer>();
			if (m_textContainer == null)
			{
				m_textContainer = base.gameObject.AddComponent<TextContainer>();
			}
			m_renderer = GetComponent<Renderer>();
			if (m_renderer == null)
			{
				m_renderer = base.gameObject.AddComponent<Renderer>();
			}
			m_transform = base.gameObject.transform;
			m_meshFilter = GetComponent<MeshFilter>();
			if (m_meshFilter == null)
			{
				m_meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			if (m_mesh == null)
			{
				m_mesh = new Mesh();
				m_mesh.hideFlags = HideFlags.HideAndDontSave;
				m_meshFilter.mesh = m_mesh;
			}
			m_meshFilter.hideFlags = HideFlags.HideInInspector;
			LoadFontAsset();
			m_char_buffer = new int[m_max_characters];
			m_cached_GlyphInfo = new GlyphInfo();
			m_vertices = new Vector3[0];
			m_isFirstAllocation = true;
			m_textInfo = new TMP_TextInfo();
			m_textInfo.wordInfo = new List<TMP_WordInfo>();
			m_textInfo.lineInfo = new TMP_LineInfo[m_max_numberOfLines];
			m_textInfo.pageInfo = new TMP_PageInfo[16];
			m_textInfo.meshInfo = default(TMP_MeshInfo);
			m_fontAssetArray = new TextMeshProFont[16];
			if (m_fontAsset == null)
			{
				UnityEngine.Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.");
			}
			else
			{
				if (m_fontSizeMin == 0f)
				{
					m_fontSizeMin = m_fontSize / 2f;
				}
				if (m_fontSizeMax == 0f)
				{
					m_fontSizeMax = m_fontSize * 2f;
				}
				isInputParsingRequired = true;
				havePropertiesChanged = true;
				ForceMeshUpdate();
			}
		}

		private void OnEnable()
		{
			if (m_meshFilter.sharedMesh == null)
			{
				m_meshFilter.mesh = m_mesh;
			}
			TMPro_EventManager.OnPreRenderObject_Event += OnPreRenderObject;
		}

		private void OnDisable()
		{
			TMPro_EventManager.OnPreRenderObject_Event -= OnPreRenderObject;
		}

		private void OnDestroy()
		{
			if (m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
		}

		private void Reset()
		{
			if (m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
			Awake();
		}

		private void LoadFontAsset()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (m_fontAsset == null)
			{
				m_fontAsset = (Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont);
				if (m_fontAsset == null)
				{
					UnityEngine.Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".");
					return;
				}
				if (m_fontAsset.characterDictionary == null)
				{
					UnityEngine.Debug.Log("Dictionary is Null!");
				}
				m_renderer.sharedMaterial = m_fontAsset.material;
				m_sharedMaterial = m_fontAsset.material;
				m_sharedMaterial.SetFloat("_CullMode", 0f);
				m_sharedMaterial.SetFloat("_ZTestMode", 4f);
				m_renderer.receiveShadows = false;
				m_renderer.castShadows = false;
			}
			else
			{
				if (m_fontAsset.characterDictionary == null)
				{
					m_fontAsset.ReadFontDefinition();
				}
				if (m_renderer.sharedMaterial == null || m_renderer.sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_renderer.sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
				{
					m_renderer.sharedMaterial = m_fontAsset.material;
					m_sharedMaterial = m_fontAsset.material;
				}
				else
				{
					m_sharedMaterial = m_renderer.sharedMaterial;
				}
				m_sharedMaterial.SetFloat("_ZTestMode", 4f);
				if (m_sharedMaterial.passCount > 1)
				{
					m_renderer.receiveShadows = false;
					m_renderer.castShadows = true;
				}
				else
				{
					m_renderer.receiveShadows = false;
					m_renderer.castShadows = false;
				}
			}
			m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
			m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
			m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
			if (!m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo))
			{
				UnityEngine.Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.");
			}
			m_sharedMaterials.Add(m_sharedMaterial);
		}

		private void ScheduleUpdate()
		{
		}

		private void UpdateEnvMapMatrix()
		{
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) && !(m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null))
			{
				Vector3 euler = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
				m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(euler), Vector3.one);
				m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
			}
		}

		private void EnableMasking()
		{
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
			{
				m_sharedMaterial.EnableKeyword("MASK_SOFT");
				m_sharedMaterial.DisableKeyword("MASK_HARD");
				m_sharedMaterial.DisableKeyword("MASK_OFF");
				m_isMaskingEnabled = true;
				UpdateMask();
			}
		}

		private void DisableMasking()
		{
			if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
			{
				m_sharedMaterial.EnableKeyword("MASK_OFF");
				m_sharedMaterial.DisableKeyword("MASK_HARD");
				m_sharedMaterial.DisableKeyword("MASK_SOFT");
				m_isMaskingEnabled = false;
				UpdateMask();
			}
		}

		private void UpdateMask()
		{
			if (m_isMaskingEnabled)
			{
				if (m_isMaskingEnabled && m_fontMaterial == null)
				{
					CreateMaterialInstance();
				}
				Vector4 margins = m_textContainer.margins;
				float x = margins.x;
				Vector4 margins2 = m_textContainer.margins;
				float num = Mathf.Min(Mathf.Min(x, margins2.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
				Vector4 margins3 = m_textContainer.margins;
				float y = margins3.y;
				Vector4 margins4 = m_textContainer.margins;
				float num2 = Mathf.Min(Mathf.Min(y, margins4.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
				num = ((!(num > 0f)) ? 0f : num);
				num2 = ((!(num2 > 0f)) ? 0f : num2);
				float width = m_textContainer.width;
				Vector4 margins5 = m_textContainer.margins;
				float num3 = width - Mathf.Max(margins5.x, 0f);
				Vector4 margins6 = m_textContainer.margins;
				float z = (num3 - Mathf.Max(margins6.z, 0f)) / 2f + num;
				float height = m_textContainer.height;
				Vector4 margins7 = m_textContainer.margins;
				float num4 = height - Mathf.Max(margins7.y, 0f);
				Vector4 margins8 = m_textContainer.margins;
				float w = (num4 - Mathf.Max(margins8.w, 0f)) / 2f + num2;
				Vector2 pivot = m_textContainer.pivot;
				float num5 = (0.5f - pivot.x) * m_textContainer.width;
				Vector4 margins9 = m_textContainer.margins;
				float num6 = Mathf.Max(margins9.x, 0f);
				Vector4 margins10 = m_textContainer.margins;
				float x2 = num5 + (num6 - Mathf.Max(margins10.z, 0f)) / 2f;
				Vector2 pivot2 = m_textContainer.pivot;
				float num7 = (0.5f - pivot2.y) * m_textContainer.height;
				Vector4 margins11 = m_textContainer.margins;
				float num8 = 0f - Mathf.Max(margins11.y, 0f);
				Vector4 margins12 = m_textContainer.margins;
				Vector2 vector = new Vector2(x2, num7 + (num8 + Mathf.Max(margins12.w, 0f)) / 2f);
				Vector4 vector2 = new Vector4(vector.x, vector.y, z, w);
				m_fontMaterial.SetVector(ShaderUtilities.ID_MaskCoord, vector2);
				m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessX, num);
				m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessY, num2);
			}
		}

		private void SetMeshArrays(int size)
		{
			int num = size * 4;
			int num2 = size * 6;
			m_vertices = new Vector3[num];
			m_normals = new Vector3[num];
			m_tangents = new Vector4[num];
			m_uvs = new Vector2[num];
			m_uv2s = new Vector2[num];
			m_vertColors = new Color32[num];
			m_triangles = new int[num2];
			for (int i = 0; i < size; i++)
			{
				int num3 = i * 4;
				int num4 = i * 6;
				m_vertices[0 + num3] = Vector3.zero;
				m_vertices[1 + num3] = Vector3.zero;
				m_vertices[2 + num3] = Vector3.zero;
				m_vertices[3 + num3] = Vector3.zero;
				m_uvs[0 + num3] = Vector2.zero;
				m_uvs[1 + num3] = Vector2.zero;
				m_uvs[2 + num3] = Vector2.zero;
				m_uvs[3 + num3] = Vector2.zero;
				m_normals[0 + num3] = new Vector3(0f, 0f, -1f);
				m_normals[1 + num3] = new Vector3(0f, 0f, -1f);
				m_normals[2 + num3] = new Vector3(0f, 0f, -1f);
				m_normals[3 + num3] = new Vector3(0f, 0f, -1f);
				m_tangents[0 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				m_tangents[1 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				m_tangents[2 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				m_tangents[3 + num3] = new Vector4(-1f, 0f, 0f, 1f);
				m_triangles[0 + num4] = 0 + num3;
				m_triangles[1 + num4] = 1 + num3;
				m_triangles[2 + num4] = 2 + num3;
				m_triangles[3 + num4] = 3 + num3;
				m_triangles[4 + num4] = 2 + num3;
				m_triangles[5 + num4] = 1 + num3;
			}
			m_mesh.vertices = m_vertices;
			m_mesh.uv = m_uvs;
			m_mesh.normals = m_normals;
			m_mesh.tangents = m_tangents;
			m_mesh.triangles = m_triangles;
			m_mesh.bounds = m_default_bounds;
		}

		private void SetFontMaterial(Material mat)
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			m_renderer.material = mat;
			m_fontMaterial = m_renderer.material;
			m_sharedMaterial = m_fontMaterial;
			m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
			m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
		}

		private void SetSharedFontMaterial(Material mat)
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			m_renderer.sharedMaterial = mat;
			m_sharedMaterial = m_renderer.sharedMaterial;
			m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
			m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
		}

		private void SetOutlineThickness(float thickness)
		{
			thickness = Mathf.Clamp01(thickness);
			m_renderer.material.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
			if (m_fontMaterial == null)
			{
				m_fontMaterial = m_renderer.material;
			}
			m_fontMaterial = m_renderer.material;
		}

		private void SetFaceColor(Color32 color)
		{
			m_renderer.material.SetColor(ShaderUtilities.ID_FaceColor, color);
			if (m_fontMaterial == null)
			{
				m_fontMaterial = m_renderer.material;
			}
		}

		private void SetOutlineColor(Color32 color)
		{
			m_renderer.material.SetColor(ShaderUtilities.ID_OutlineColor, color);
			if (m_fontMaterial == null)
			{
				m_fontMaterial = m_renderer.material;
			}
		}

		private void CreateMaterialInstance()
		{
			Material material = new Material(m_sharedMaterial);
			material.shaderKeywords = m_sharedMaterial.shaderKeywords;
			material.name += " Instance";
			m_fontMaterial = material;
		}

		private void SetShaderType()
		{
			if (m_isOverlay)
			{
				m_renderer.material.SetFloat("_ZTestMode", 8f);
				m_renderer.material.renderQueue = 4000;
				m_sharedMaterial = m_renderer.material;
			}
			else
			{
				m_renderer.material.SetFloat("_ZTestMode", 4f);
				m_renderer.material.renderQueue = -1;
				m_sharedMaterial = m_renderer.material;
			}
		}

		private void SetCulling()
		{
			if (m_isCullingEnabled)
			{
				m_renderer.material.SetFloat("_CullMode", 2f);
			}
			else
			{
				m_renderer.material.SetFloat("_CullMode", 0f);
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
			if (text != null)
			{
				if (chars.Length <= text.Length)
				{
					int num = (text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256);
					chars = new int[num];
				}
				int num2 = 0;
				for (int i = 0; i < text.Length; i++)
				{
					if (text[i] == '\\' && i < text.Length - 1)
					{
						switch (text[i + 1])
						{
						case 't':
							break;
						case 'n':
							goto IL_00a4;
						default:
							goto IL_00b7;
						}
						chars[num2] = 9;
						i++;
						num2++;
						continue;
					}
					goto IL_00b7;
					IL_00a4:
					chars[num2] = 10;
					i++;
					num2++;
					continue;
					IL_00b7:
					chars[num2] = text[i];
					num2++;
				}
				chars[num2] = 0;
			}
		}

		private void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray != null && m_charArray_Length != 0)
			{
				if (charBuffer.Length <= m_charArray_Length)
				{
					int num = (m_charArray_Length <= 1024) ? Mathf.NextPowerOfTwo(m_charArray_Length + 1) : (m_charArray_Length + 256);
					charBuffer = new int[num];
				}
				int num2 = 0;
				for (int i = 0; i < m_charArray_Length; i++)
				{
					if (charArray[i] == '\\' && i < m_charArray_Length - 1)
					{
						switch (charArray[i + 1])
						{
						case 't':
							break;
						case 'n':
							goto IL_00a7;
						default:
							goto IL_00ba;
						}
						charBuffer[num2] = 9;
						i++;
						num2++;
						continue;
					}
					goto IL_00ba;
					IL_00a7:
					charBuffer[num2] = 10;
					i++;
					num2++;
					continue;
					IL_00ba:
					charBuffer[num2] = charArray[i];
					num2++;
				}
				charBuffer[num2] = 0;
			}
		}

		private int GetArraySizes(int[] chars)
		{
			int num = 0;
			int num2 = 0;
			int endIndex = 0;
			m_isUsingBold = false;
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
					if (num3 != 32 && num3 != 9 && num3 != 10 && num3 != 13)
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
			m_isUsingBold = false;
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
					if (num3 != 32 && num3 != 9 && num3 != 10 && num3 != 13)
					{
						num++;
					}
					m_VisibleCharacters.Add((char)num3);
					num2++;
				}
			}
			if (m_textInfo.characterInfo == null || num2 > m_textInfo.characterInfo.Length)
			{
				m_textInfo.characterInfo = new TMP_CharacterInfo[(num2 <= 1024) ? Mathf.NextPowerOfTwo(num2) : (num2 + 256)];
			}
			if (num * 4 > m_vertices.Length)
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

		private void OnDidApplyAnimationProperties()
		{
			havePropertiesChanged = true;
			isMaskUpdateRequired = true;
		}

		private void OnPreRenderObject()
		{
		}

		private void OnWillRenderObject()
		{
			if (!(m_fontAsset == null))
			{
				if (m_transform.hasChanged)
				{
					m_transform.hasChanged = false;
					if (m_transform.lossyScale != m_lossyScale)
					{
						havePropertiesChanged = true;
						m_lossyScale = m_transform.lossyScale;
					}
					if (m_textContainer != null && m_textContainer.hasChanged)
					{
						isMaskUpdateRequired = true;
						if (m_isTextTruncated)
						{
							isInputParsingRequired = true;
							m_isTextTruncated = false;
						}
						m_textContainer.hasChanged = false;
						havePropertiesChanged = true;
					}
				}
				if (havePropertiesChanged || m_fontAsset.propertiesChanged)
				{
					if (hasFontAssetChanged || m_fontAsset.propertiesChanged)
					{
						LoadFontAsset();
						hasFontAssetChanged = false;
						if (m_fontAsset == null || m_renderer.sharedMaterial == null)
						{
							return;
						}
						m_fontAsset.propertiesChanged = false;
					}
					if (isMaskUpdateRequired)
					{
						UpdateMask();
						isMaskUpdateRequired = false;
					}
					if (isInputParsingRequired)
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
					if (m_enableAutoSizing)
					{
						m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);
					}
					m_maxFontSize = m_fontSizeMax;
					m_minFontSize = m_fontSizeMin;
					m_lineSpacingDelta = 0f;
					m_recursiveCount = 0;
					m_isCharacterWrappingEnabled = false;
					m_isTextTruncated = false;
					GenerateTextMesh();
					havePropertiesChanged = false;
				}
			}
		}

		private void GenerateTextMesh()
		{
			if (m_fontAsset.characterDictionary == null)
			{
				UnityEngine.Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + GetInstanceID());
			}
			else
			{
				if (m_textInfo != null)
				{
					m_textInfo.Clear();
				}
				if (m_char_buffer == null || m_char_buffer[0] == 0)
				{
					if (m_vertices != null)
					{
						Array.Clear(m_vertices, 0, m_vertices.Length);
						m_mesh.vertices = m_vertices;
					}
					m_preferredWidth = 0f;
					m_preferredHeight = 0f;
				}
				else
				{
					int num = SetArraySizes(m_char_buffer);
					m_fontIndex = 0;
					m_fontAssetArray[m_fontIndex] = m_fontAsset;
					m_fontScale = m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * ((!m_isOrthographic) ? 0.1f : 1f);
					float fontScale = m_fontScale;
					m_maxFontScale = 0f;
					float num2 = 0f;
					m_currentFontSize = m_fontSize;
					float num3 = 0f;
					int num4 = 0;
					m_style = m_fontStyle;
					m_lineJustification = m_textAlignment;
					if (checkPaddingRequired)
					{
						checkPaddingRequired = false;
						m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
						m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
						m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
					}
					float num5 = 0f;
					float num6 = 1f;
					m_baselineOffset = 0f;
					bool flag = false;
					Vector3 start = Vector3.zero;
					Vector3 zero = Vector3.zero;
					m_fontColor32 = m_fontColor;
					m_htmlColor = m_fontColor32;
					m_lineOffset = 0f;
					m_lineHeight = 0f;
					m_cSpacing = 0f;
					m_monoSpacing = 0f;
					float num7 = 0f;
					m_xAdvance = 0f;
					m_indent = 0f;
					m_maxXAdvance = 0f;
					m_lineNumber = 0;
					m_pageNumber = 0;
					m_characterCount = 0;
					m_visibleCharacterCount = 0;
					m_firstVisibleCharacterOfLine = 0;
					m_lastVisibleCharacterOfLine = 0;
					int num8 = 0;
					Vector3[] corners = m_textContainer.corners;
					Vector4 margins = m_textContainer.margins;
					m_marginWidth = m_textContainer.rect.width - margins.z - margins.x;
					float num9 = m_textContainer.rect.height - margins.y - margins.w;
					m_preferredWidth = 0f;
					m_preferredHeight = 0f;
					bool flag2 = true;
					bool flag3 = false;
					m_SavedWordWrapState = default(WordWrapState);
					m_SavedLineState = default(WordWrapState);
					int num10 = 0;
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
					m_isNewPage = false;
					float num11 = 0f;
					int endIndex = 0;
					for (int j = 0; m_char_buffer[j] != 0; j++)
					{
						num4 = m_char_buffer[j];
						loopCountE++;
						if (m_isRichText && num4 == 60 && ValidateHtmlTag(m_char_buffer, j + 1, out endIndex))
						{
							j = endIndex;
							if (m_isRecalculateScaleRequired)
							{
								m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * ((!m_isOrthographic) ? 0.1f : 1f);
								m_isRecalculateScaleRequired = false;
							}
							continue;
						}
						bool flag4 = false;
						if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
						{
							if (char.IsLower((char)num4))
							{
								num4 -= 32;
							}
						}
						else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
						{
							if (char.IsUpper((char)num4))
							{
								num4 += 32;
							}
						}
						else if ((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
						{
							if (char.IsLower((char)num4))
							{
								m_fontScale = m_currentFontSize * 0.8f / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * ((!m_isOrthographic) ? 0.1f : 1f);
								num4 -= 32;
							}
							else
							{
								m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * ((!m_isOrthographic) ? 0.1f : 1f);
							}
						}
						m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(num4, out m_cached_GlyphInfo);
						if (m_cached_GlyphInfo == null)
						{
							if (char.IsLower((char)num4))
							{
								if (m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(num4 - 32, out m_cached_GlyphInfo))
								{
									num4 -= 32;
								}
							}
							else if (char.IsUpper((char)num4) && m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(num4 + 32, out m_cached_GlyphInfo))
							{
								num4 += 32;
							}
							if (m_cached_GlyphInfo == null)
							{
								m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(88, out m_cached_GlyphInfo);
								if (m_cached_GlyphInfo == null)
								{
									UnityEngine.Debug.LogWarning("Character with ASCII value of " + num4 + " was not found in the Font Asset Glyph Table.");
									continue;
								}
								UnityEngine.Debug.LogWarning("Character with ASCII value of " + num4 + " was not found in the Font Asset Glyph Table.");
								num4 = 88;
								flag4 = true;
							}
						}
						m_textInfo.characterInfo[m_characterCount].character = (char)num4;
						m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
						m_textInfo.characterInfo[m_characterCount].style = m_style;
						m_textInfo.characterInfo[m_characterCount].index = (short)j;
						if (m_enableKerning && m_characterCount >= 1)
						{
							int character = m_textInfo.characterInfo[m_characterCount - 1].character;
							KerningPairKey kerningPairKey = new KerningPairKey(character, num4);
							m_fontAssetArray[m_fontIndex].kerningDictionary.TryGetValue(kerningPairKey.key, out KerningPair value);
							if (value != null)
							{
								m_xAdvance += value.XadvanceOffset * m_fontScale;
							}
						}
						if (m_monoSpacing != 0f && m_xAdvance != 0f)
						{
							m_xAdvance -= (m_cached_GlyphInfo.width / 2f + m_cached_GlyphInfo.xOffset) * m_fontScale;
						}
						if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
						{
							num5 = m_fontAssetArray[m_fontIndex].BoldStyle * 2f;
							num6 = 1.07f;
						}
						else
						{
							num5 = m_fontAssetArray[m_fontIndex].NormalStyle * 2f;
							num6 = 1f;
						}
						Vector3 vector = new Vector3(0f + m_xAdvance + (m_cached_GlyphInfo.xOffset - m_padding - num5) * m_fontScale, (m_cached_GlyphInfo.yOffset + m_baselineOffset + m_padding) * m_fontScale - m_lineOffset, 0f);
						Vector3 vector2 = new Vector3(vector.x, vector.y - (m_cached_GlyphInfo.height + m_padding * 2f) * m_fontScale, 0f);
						Vector3 vector3 = new Vector3(vector2.x + (m_cached_GlyphInfo.width + m_padding * 2f + num5 * 2f) * m_fontScale, vector.y, 0f);
						Vector3 vector4 = new Vector3(vector3.x, vector2.y, 0f);
						if ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
						{
							float num12 = (float)(int)m_fontAssetArray[m_fontIndex].ItalicStyle * 0.01f;
							Vector3 b = new Vector3(num12 * ((m_cached_GlyphInfo.yOffset + m_padding + num5) * m_fontScale), 0f, 0f);
							Vector3 b2 = new Vector3(num12 * ((m_cached_GlyphInfo.yOffset - m_cached_GlyphInfo.height - m_padding - num5) * m_fontScale), 0f, 0f);
							vector += b;
							vector2 += b2;
							vector3 += b;
							vector4 += b2;
						}
						m_textInfo.characterInfo[m_characterCount].topLeft = vector;
						m_textInfo.characterInfo[m_characterCount].bottomLeft = vector2;
						m_textInfo.characterInfo[m_characterCount].topRight = vector3;
						m_textInfo.characterInfo[m_characterCount].bottomRight = vector4;
						float num13 = (m_fontAsset.fontInfo.Ascender + m_baselineOffset + m_alignmentPadding.y) * m_fontScale;
						if (m_lineNumber == 0)
						{
							m_maxAscender = ((!(m_maxAscender > num13)) ? num13 : m_maxAscender);
						}
						if (m_lineOffset == 0f)
						{
							num11 = ((!(num11 > num13)) ? num13 : num11);
						}
						float num14 = (m_fontAsset.fontInfo.Descender + m_baselineOffset + m_alignmentPadding.w) * m_fontScale - m_lineOffset;
						m_textInfo.characterInfo[m_characterCount].isVisible = false;
						if (num4 != 32 && num4 != 9 && num4 != 10 && num4 != 13)
						{
							int num15 = m_visibleCharacterCount * 4;
							m_textInfo.characterInfo[m_characterCount].isVisible = true;
							m_textInfo.characterInfo[m_characterCount].vertexIndex = (short)(0 + num15);
							m_vertices[0 + num15] = m_textInfo.characterInfo[m_characterCount].bottomLeft;
							m_vertices[1 + num15] = m_textInfo.characterInfo[m_characterCount].topLeft;
							m_vertices[2 + num15] = m_textInfo.characterInfo[m_characterCount].bottomRight;
							m_vertices[3 + num15] = m_textInfo.characterInfo[m_characterCount].topRight;
							if (m_baselineOffset == 0f)
							{
								m_maxFontScale = Mathf.Max(m_maxFontScale, m_fontScale);
							}
							if (m_xAdvance + m_cached_GlyphInfo.xAdvance * m_fontScale > m_marginWidth + 0.0001f && !m_textContainer.isDefaultWidth)
							{
								num8 = m_characterCount - 1;
								if (enableWordWrapping && m_characterCount != m_firstVisibleCharacterOfLine)
								{
									if (num10 == m_SavedWordWrapState.previous_WordBreak)
									{
										if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
										{
											m_maxFontSize = m_fontSize;
											float num16 = Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.01f);
											m_fontSize -= num16;
											m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 100f + 0.5f) / 100f;
											GenerateTextMesh();
											return;
										}
										if (!m_isCharacterWrappingEnabled)
										{
											m_isCharacterWrappingEnabled = true;
										}
										else
										{
											flag3 = true;
										}
										m_recursiveCount++;
										if (m_recursiveCount > 5)
										{
											continue;
										}
									}
									j = RestoreWordWrappingState(ref m_SavedWordWrapState);
									num10 = j;
									if (m_lineNumber > 0 && m_maxFontScale != 0f && m_lineHeight == 0f && m_maxFontScale != num2 && !m_isNewPage)
									{
										float num17 = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);
										float num18 = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + num17 + m_lineSpacing + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - num17) * num2;
										m_lineOffset += num18 - num7;
										AdjustLineOffset(m_firstVisibleCharacterOfLine, m_characterCount - 1, num18 - num7);
									}
									m_isNewPage = false;
									float num19 = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
									float num20 = (m_fontAsset.fontInfo.Ascender + m_baselineOffset + m_alignmentPadding.y) * m_fontScale - m_lineOffset;
									num19 = ((!(num19 > num20)) ? num20 : num19);
									float num21 = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_maxFontScale - m_lineOffset;
									float num22 = (m_fontAsset.fontInfo.Descender + m_baselineOffset + m_alignmentPadding.w) * m_fontScale - m_lineOffset;
									num21 = ((!(num21 < num22)) ? num22 : num21);
									if (m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].isVisible)
									{
										m_maxDescender = ((!(m_maxDescender < num21)) ? num21 : m_maxDescender);
									}
									m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstVisibleCharacterOfLine;
									m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = ((m_characterCount - 1 <= 0) ? 1 : (m_characterCount - 1));
									m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine;
									m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num21);
									m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num19);
									m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - m_padding * m_maxFontScale;
									m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing;
									m_firstVisibleCharacterOfLine = m_characterCount;
									m_preferredWidth += m_xAdvance;
									if (m_enableWordWrapping)
									{
										m_preferredHeight = m_maxAscender - m_maxDescender;
									}
									else
									{
										m_preferredHeight = Mathf.Max(m_preferredHeight, num19 - num21);
									}
									SaveWordWrappingState(ref m_SavedLineState, j, m_characterCount - 1);
									m_lineNumber++;
									if (m_lineNumber >= m_textInfo.lineInfo.Length)
									{
										ResizeLineExtents(m_lineNumber);
									}
									if (m_lineHeight == 0f)
									{
										num7 = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
										m_lineOffset += num7;
									}
									else
									{
										m_lineOffset += (m_lineHeight + m_lineSpacing) * fontScale;
									}
									num2 = m_fontScale;
									m_xAdvance = 0f + m_indent;
									m_maxFontScale = 0f;
									continue;
								}
								if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
								{
									m_maxFontSize = m_fontSize;
									float num23 = Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.01f);
									m_fontSize -= num23;
									m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 100f + 0.5f) / 100f;
									m_recursiveCount = 0;
									GenerateTextMesh();
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
									if (m_characterCount >= 1)
									{
										m_char_buffer[j - 1] = 8230;
										m_char_buffer[j] = 0;
										GenerateTextMesh();
										return;
									}
									m_textInfo.characterInfo[m_characterCount].isVisible = false;
									m_visibleCharacterCount--;
									break;
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
							Color32 color = flag4 ? ((Color32)Color.red) : ((!m_overrideHtmlColors) ? m_htmlColor : m_fontColor32);
							if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
							{
								color.a = ((m_fontColor32.a >= color.a) ? ((byte)(color.a >> 1)) : ((byte)(m_fontColor32.a >> 1)));
								color.a += 128;
							}
							else
							{
								color.a = ((m_fontColor32.a >= color.a) ? ((byte)(color.a >> 1)) : ((byte)(m_fontColor32.a >> 1)));
							}
							if (!m_enableVertexGradient)
							{
								m_vertColors[0 + num15] = color;
								m_vertColors[1 + num15] = color;
								m_vertColors[2 + num15] = color;
								m_vertColors[3 + num15] = color;
							}
							else
							{
								if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
								{
									m_vertColors[0 + num15] = color;
									m_vertColors[1 + num15] = color;
									m_vertColors[2 + num15] = color;
									m_vertColors[3 + num15] = color;
								}
								else
								{
									m_vertColors[0 + num15] = m_fontColorGradient.bottomLeft;
									m_vertColors[1 + num15] = m_fontColorGradient.topLeft;
									m_vertColors[2 + num15] = m_fontColorGradient.bottomRight;
									m_vertColors[3 + num15] = m_fontColorGradient.topRight;
								}
								m_vertColors[0 + num15].a = color.a;
								m_vertColors[1 + num15].a = color.a;
								m_vertColors[2 + num15].a = color.a;
								m_vertColors[3 + num15].a = color.a;
							}
							if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
							{
								num5 = 0f;
							}
							Vector2 vector5 = new Vector2((m_cached_GlyphInfo.x - m_padding - num5) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, 1f - (m_cached_GlyphInfo.y + m_padding + num5 + m_cached_GlyphInfo.height) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);
							Vector2 vector6 = new Vector2(vector5.x, 1f - (m_cached_GlyphInfo.y - m_padding - num5) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);
							Vector2 vector7 = new Vector2((m_cached_GlyphInfo.x + m_padding + num5 + m_cached_GlyphInfo.width) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, vector5.y);
							Vector2 vector8 = new Vector2(vector7.x, vector6.y);
							m_uvs[0 + num15] = vector5;
							m_uvs[1 + num15] = vector6;
							m_uvs[2 + num15] = vector7;
							m_uvs[3 + num15] = vector8;
							m_visibleCharacterCount++;
							if (m_textInfo.characterInfo[m_characterCount].isVisible)
							{
								m_lastVisibleCharacterOfLine = m_characterCount;
							}
						}
						else if (num4 == 9 || num4 == 32)
						{
							m_textInfo.lineInfo[m_lineNumber].spaceCount++;
							m_textInfo.spaceCount++;
						}
						m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
						m_textInfo.characterInfo[m_characterCount].pageNumber = (short)m_pageNumber;
						m_textInfo.lineInfo[m_lineNumber].characterCount++;
						if ((num4 != 10 && num4 != 13 && num4 != 8230) || m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
						{
							m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;
						}
						if (m_maxAscender - num14 + m_alignmentPadding.w * 2f * m_fontScale > num9 && !m_textContainer.isDefaultHeight)
						{
							if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax)
							{
								m_lineSpacingDelta -= 1f;
								GenerateTextMesh();
							}
							else if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
							{
								m_maxFontSize = m_fontSize;
								float num24 = Mathf.Max((m_fontSize - m_minFontSize) / 2f, 0.025f);
								m_fontSize -= num24;
								m_fontSize = (float)(int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 100f + 0.5f) / 100f;
								m_recursiveCount = 0;
								GenerateTextMesh();
							}
							else
							{
								switch (m_overflowMode)
								{
								case TextOverflowModes.Overflow:
									if (m_isMaskingEnabled)
									{
										DisableMasking();
									}
									goto IL_1d5c;
								case TextOverflowModes.Ellipsis:
									break;
								case TextOverflowModes.Masking:
									if (!m_isMaskingEnabled)
									{
										EnableMasking();
									}
									goto IL_1d5c;
								case TextOverflowModes.ScrollRect:
									if (!m_isMaskingEnabled)
									{
										EnableMasking();
									}
									goto IL_1d5c;
								case TextOverflowModes.Truncate:
									goto IL_1c6e;
								case TextOverflowModes.Page:
									goto IL_1cd1;
								default:
									goto IL_1d5c;
								}
								if (m_isMaskingEnabled)
								{
									DisableMasking();
								}
								if (m_lineNumber > 0)
								{
									m_char_buffer[m_textInfo.characterInfo[num8].index] = 8230;
									m_char_buffer[m_textInfo.characterInfo[num8].index + 1] = 0;
									GenerateTextMesh();
									m_isTextTruncated = true;
								}
								else
								{
									m_char_buffer[0] = 0;
									GenerateTextMesh();
									m_isTextTruncated = true;
								}
							}
							return;
						}
						goto IL_1d5c;
						IL_1cd1:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (num4 != 13 && num4 != 10)
						{
							j = RestoreWordWrappingState(ref m_SavedLineState);
							if (j == 0)
							{
								m_char_buffer[0] = 0;
								GenerateTextMesh();
								m_isTextTruncated = true;
								return;
							}
							m_isNewPage = true;
							m_xAdvance = 0f + m_indent;
							m_lineOffset = 0f;
							m_pageNumber++;
							continue;
						}
						goto IL_1d5c;
						IL_1d5c:
						if (num4 == 9)
						{
							m_xAdvance += m_fontAsset.fontInfo.TabWidth * m_fontScale * 5f;
						}
						else if (m_monoSpacing != 0f)
						{
							m_xAdvance += (m_monoSpacing + m_cached_GlyphInfo.width / 2f + m_cached_GlyphInfo.xOffset) * m_fontScale + m_characterSpacing + m_cSpacing;
						}
						else
						{
							m_xAdvance += m_cached_GlyphInfo.xAdvance * num6 * m_fontScale + m_characterSpacing + m_cSpacing;
						}
						m_textInfo.characterInfo[m_characterCount].xAdvance = m_xAdvance;
						if (num4 == 13)
						{
							m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + m_alignmentPadding.z * m_fontScale);
							m_preferredWidth = 0f;
							m_xAdvance = 0f + m_indent;
						}
						if (num4 == 10 || m_characterCount == num - 1)
						{
							if (m_lineNumber > 0 && m_maxFontScale != 0f && m_lineHeight == 0f && m_maxFontScale != num2 && !m_isNewPage)
							{
								float num25 = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);
								float num26 = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + num25 + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - num25) * num2;
								m_lineOffset += num26 - num7;
								AdjustLineOffset(m_firstVisibleCharacterOfLine, m_characterCount, num26 - num7);
							}
							m_isNewPage = false;
							float num27 = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
							float num28 = (m_fontAsset.fontInfo.Ascender + m_baselineOffset + m_alignmentPadding.y) * m_fontScale - m_lineOffset;
							num27 = ((!(num27 > num28)) ? num28 : num27);
							float num29 = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_maxFontScale - m_lineOffset;
							float num30 = (m_fontAsset.fontInfo.Descender + m_baselineOffset + m_alignmentPadding.w) * m_fontScale - m_lineOffset;
							num29 = ((!(num29 < num30)) ? num30 : num29);
							if (m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].isVisible)
							{
								m_maxDescender = ((!(m_maxDescender < num29)) ? num29 : m_maxDescender);
							}
							m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstVisibleCharacterOfLine;
							m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount;
							m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine;
							m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, num29);
							m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, num27);
							m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - m_padding * m_maxFontScale;
							m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing;
							m_firstVisibleCharacterOfLine = m_characterCount + 1;
							if (num4 == 10 && m_characterCount != num - 1)
							{
								m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + m_alignmentPadding.z * m_fontScale);
								m_preferredWidth = 0f;
							}
							else
							{
								m_preferredWidth = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + m_alignmentPadding.z * m_fontScale);
							}
							if (m_enableWordWrapping)
							{
								m_preferredHeight = m_maxAscender - m_maxDescender;
							}
							else
							{
								m_preferredHeight = Mathf.Max(m_preferredHeight, num27 - num29);
							}
							if (num4 == 10)
							{
								SaveWordWrappingState(ref m_SavedLineState, j, m_characterCount);
								SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
								m_lineNumber++;
								if (m_lineNumber >= m_textInfo.lineInfo.Length)
								{
									ResizeLineExtents(m_lineNumber);
								}
								if (m_lineHeight == 0f)
								{
									num7 = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_paragraphSpacing + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
									m_lineOffset += num7;
								}
								else
								{
									m_lineOffset += (m_lineHeight + m_paragraphSpacing + m_lineSpacing) * fontScale;
								}
								num2 = m_fontScale;
								m_maxFontScale = 0f;
								m_xAdvance = 0f + m_indent;
								num8 = m_characterCount - 1;
							}
						}
						m_textInfo.characterInfo[m_characterCount].baseLine = m_textInfo.characterInfo[m_characterCount].topRight.y - (m_cached_GlyphInfo.yOffset + m_padding) * m_fontScale;
						m_textInfo.characterInfo[m_characterCount].topLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale;
						m_textInfo.characterInfo[m_characterCount].bottomLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Descender - m_alignmentPadding.w) * m_fontScale;
						m_textInfo.characterInfo[m_characterCount].padding = m_padding * m_fontScale;
						m_textInfo.characterInfo[m_characterCount].aspectRatio = m_cached_GlyphInfo.width / m_cached_GlyphInfo.height;
						m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;
						if (m_textInfo.characterInfo[m_characterCount].isVisible)
						{
							m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].bottomLeft.x), Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].bottomLeft.y));
							m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].topRight.x), Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].topLeft.y));
						}
						if (num4 != 13 && num4 != 10 && m_pageNumber < 16)
						{
							m_textInfo.pageInfo[m_pageNumber].ascender = num11;
							m_textInfo.pageInfo[m_pageNumber].descender = ((!(num14 < m_textInfo.pageInfo[m_pageNumber].descender)) ? m_textInfo.pageInfo[m_pageNumber].descender : num14);
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
							if (num4 == 9 || num4 == 32)
							{
								SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
								m_isCharacterWrappingEnabled = false;
								flag2 = false;
							}
							else if (((flag2 || m_isCharacterWrappingEnabled) && m_characterCount < num - 1 && !m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(num4) && !m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(m_VisibleCharacters[m_characterCount + 1])) || flag3)
							{
								SaveWordWrappingState(ref m_SavedWordWrapState, j, m_characterCount);
							}
						}
						m_characterCount++;
						continue;
						IL_1c6e:
						if (m_isMaskingEnabled)
						{
							DisableMasking();
						}
						if (m_lineNumber > 0)
						{
							m_char_buffer[m_textInfo.characterInfo[num8].index + 1] = 0;
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
					}
					num3 = m_maxFontSize - m_minFontSize;
					if ((!m_textContainer.isDefaultWidth || !m_textContainer.isDefaultHeight) && !m_isCharacterWrappingEnabled && m_enableAutoSizing && num3 > 0.25f && m_fontSize < m_fontSizeMax)
					{
						m_minFontSize = m_fontSize;
						m_fontSize += Mathf.Max(m_maxFontSize + m_fontSize / 2f, 0.01f);
						m_fontSize = (float)(int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 100f + 0.5f) / 100f;
						GenerateTextMesh();
					}
					else
					{
						if (m_characterCount < m_textInfo.characterInfo.Length)
						{
							m_textInfo.characterInfo[m_characterCount].character = '\0';
						}
						m_isCharacterWrappingEnabled = false;
						if (m_renderMode != TextRenderFlags.GetPreferredSizes)
						{
							if (m_visibleCharacterCount == 0)
							{
								if (m_vertices != null)
								{
									Array.Clear(m_vertices, 0, m_vertices.Length);
									m_mesh.vertices = m_vertices;
								}
							}
							else
							{
								int index = m_visibleCharacterCount * 4;
								Array.Clear(m_vertices, index, m_vertices.Length - index);
								switch (m_textAlignment)
								{
								case TextAlignmentOptions.TopLeft:
								case TextAlignmentOptions.Top:
								case TextAlignmentOptions.TopRight:
								case TextAlignmentOptions.TopJustified:
									if (m_overflowMode != TextOverflowModes.Page)
									{
										m_anchorOffset = corners[1] + new Vector3(0f + margins.x, 0f - m_maxAscender - margins.y, 0f);
									}
									else
									{
										m_anchorOffset = corners[1] + new Vector3(0f + margins.x, 0f - m_textInfo.pageInfo[m_pageToDisplay].ascender - margins.y, 0f);
									}
									break;
								case TextAlignmentOptions.Left:
								case TextAlignmentOptions.Center:
								case TextAlignmentOptions.Right:
								case TextAlignmentOptions.Justified:
									if (m_overflowMode != TextOverflowModes.Page)
									{
										m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f - (m_maxAscender + margins.y + m_maxDescender - margins.w) / 2f, 0f);
									}
									else
									{
										m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f - (m_textInfo.pageInfo[m_pageToDisplay].ascender + margins.y + m_textInfo.pageInfo[m_pageToDisplay].descender - margins.w) / 2f, 0f);
									}
									break;
								case TextAlignmentOptions.BottomLeft:
								case TextAlignmentOptions.Bottom:
								case TextAlignmentOptions.BottomRight:
								case TextAlignmentOptions.BottomJustified:
									if (m_overflowMode != TextOverflowModes.Page)
									{
										m_anchorOffset = corners[0] + new Vector3(0f + margins.x, 0f - m_maxDescender + margins.w, 0f);
									}
									else
									{
										m_anchorOffset = corners[0] + new Vector3(0f + margins.x, 0f - m_textInfo.pageInfo[m_pageToDisplay].descender + margins.w, 0f);
									}
									break;
								case TextAlignmentOptions.BaselineLeft:
								case TextAlignmentOptions.Baseline:
								case TextAlignmentOptions.BaselineRight:
									m_anchorOffset = (corners[0] + corners[1]) / 2f + new Vector3(0f + margins.x, 0f, 0f);
									break;
								}
								Vector3 vector9 = Vector3.zero;
								Vector3 zero2 = Vector3.zero;
								int num31 = 0;
								int num32 = 0;
								int num33 = 0;
								int num34 = 0;
								bool flag5 = false;
								int num35 = 0;
								int num36 = 0;
								for (int k = 0; k < m_characterCount; k++)
								{
									int lineNumber = m_textInfo.characterInfo[k].lineNumber;
									char character2 = m_textInfo.characterInfo[k].character;
									TMP_LineInfo tMP_LineInfo = m_textInfo.lineInfo[lineNumber];
									TextAlignmentOptions alignment = tMP_LineInfo.alignment;
									num33 = lineNumber + 1;
									switch (alignment)
									{
									case TextAlignmentOptions.TopLeft:
									case TextAlignmentOptions.Left:
									case TextAlignmentOptions.BottomLeft:
									case TextAlignmentOptions.BaselineLeft:
										vector9 = Vector3.zero;
										break;
									case TextAlignmentOptions.Top:
									case TextAlignmentOptions.Center:
									case TextAlignmentOptions.Bottom:
									case TextAlignmentOptions.Baseline:
										vector9 = new Vector3(m_marginWidth / 2f - tMP_LineInfo.maxAdvance / 2f, 0f, 0f);
										break;
									case TextAlignmentOptions.TopRight:
									case TextAlignmentOptions.Right:
									case TextAlignmentOptions.BottomRight:
									case TextAlignmentOptions.BaselineRight:
										vector9 = new Vector3(m_marginWidth - tMP_LineInfo.maxAdvance, 0f, 0f);
										break;
									case TextAlignmentOptions.TopJustified:
									case TextAlignmentOptions.Justified:
									case TextAlignmentOptions.BottomJustified:
									{
										num4 = m_textInfo.characterInfo[k].character;
										char character3 = m_textInfo.characterInfo[tMP_LineInfo.lastCharacterIndex].character;
										if (char.IsWhiteSpace(character3) && !char.IsControl(character3) && lineNumber < m_lineNumber)
										{
											float num37 = corners[3].x - margins.z - (corners[0].x + margins.x) - tMP_LineInfo.maxAdvance;
											vector9 = ((lineNumber == num34 && k != 0) ? ((num4 != 9 && num4 != 32) ? (vector9 + new Vector3(num37 * m_wordWrappingRatios / (float)(tMP_LineInfo.characterCount - tMP_LineInfo.spaceCount - 1), 0f, 0f)) : (vector9 + new Vector3(num37 * (1f - m_wordWrappingRatios) / (float)(tMP_LineInfo.spaceCount - 1), 0f, 0f))) : Vector3.zero);
										}
										else
										{
											vector9 = Vector3.zero;
										}
										break;
									}
									}
									zero2 = m_anchorOffset + vector9;
									if (m_textInfo.characterInfo[k].isVisible)
									{
										Extents lineExtents = tMP_LineInfo.lineExtents;
										float num38 = m_uvLineOffset * (float)lineNumber % 1f + m_uvOffset.x;
										switch (m_horizontalMapping)
										{
										case TextureMappingOptions.Character:
											m_uv2s[num31].x = 0f + m_uvOffset.x;
											m_uv2s[num31 + 1].x = 0f + m_uvOffset.x;
											m_uv2s[num31 + 2].x = 1f + m_uvOffset.x;
											m_uv2s[num31 + 3].x = 1f + m_uvOffset.x;
											break;
										case TextureMappingOptions.Line:
											if (m_textAlignment != TextAlignmentOptions.Justified)
											{
												m_uv2s[num31].x = (m_vertices[num31].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num38;
												m_uv2s[num31 + 1].x = (m_vertices[num31 + 1].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num38;
												m_uv2s[num31 + 2].x = (m_vertices[num31 + 2].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num38;
												m_uv2s[num31 + 3].x = (m_vertices[num31 + 3].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num38;
											}
											else
											{
												m_uv2s[num31].x = (m_vertices[num31].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
												m_uv2s[num31 + 1].x = (m_vertices[num31 + 1].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
												m_uv2s[num31 + 2].x = (m_vertices[num31 + 2].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
												m_uv2s[num31 + 3].x = (m_vertices[num31 + 3].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
											}
											break;
										case TextureMappingOptions.Paragraph:
											m_uv2s[num31].x = (m_vertices[num31].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
											m_uv2s[num31 + 1].x = (m_vertices[num31 + 1].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
											m_uv2s[num31 + 2].x = (m_vertices[num31 + 2].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
											m_uv2s[num31 + 3].x = (m_vertices[num31 + 3].x + vector9.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + num38;
											break;
										case TextureMappingOptions.MatchAspect:
										{
											switch (m_verticalMapping)
											{
											case TextureMappingOptions.Character:
												m_uv2s[num31].y = 0f + m_uvOffset.y;
												m_uv2s[num31 + 1].y = 1f + m_uvOffset.y;
												m_uv2s[num31 + 2].y = 0f + m_uvOffset.y;
												m_uv2s[num31 + 3].y = 1f + m_uvOffset.y;
												break;
											case TextureMappingOptions.Line:
												m_uv2s[num31].y = (m_vertices[num31].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num38;
												m_uv2s[num31 + 1].y = (m_vertices[num31 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num38;
												m_uv2s[num31 + 2].y = m_uv2s[num31].y;
												m_uv2s[num31 + 3].y = m_uv2s[num31 + 1].y;
												break;
											case TextureMappingOptions.Paragraph:
												m_uv2s[num31].y = (m_vertices[num31].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num38;
												m_uv2s[num31 + 1].y = (m_vertices[num31 + 1].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + num38;
												m_uv2s[num31 + 2].y = m_uv2s[num31].y;
												m_uv2s[num31 + 3].y = m_uv2s[num31 + 1].y;
												break;
											case TextureMappingOptions.MatchAspect:
												UnityEngine.Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
												break;
											}
											float num39 = (1f - (m_uv2s[num31].y + m_uv2s[num31 + 1].y) * m_textInfo.characterInfo[k].aspectRatio) / 2f;
											m_uv2s[num31].x = m_uv2s[num31].y * m_textInfo.characterInfo[k].aspectRatio + num39 + num38;
											m_uv2s[num31 + 1].x = m_uv2s[num31].x;
											m_uv2s[num31 + 2].x = m_uv2s[num31 + 1].y * m_textInfo.characterInfo[k].aspectRatio + num39 + num38;
											m_uv2s[num31 + 3].x = m_uv2s[num31 + 2].x;
											break;
										}
										}
										switch (m_verticalMapping)
										{
										case TextureMappingOptions.Character:
											m_uv2s[num31].y = 0f + m_uvOffset.y;
											m_uv2s[num31 + 1].y = 1f + m_uvOffset.y;
											m_uv2s[num31 + 2].y = 0f + m_uvOffset.y;
											m_uv2s[num31 + 3].y = 1f + m_uvOffset.y;
											break;
										case TextureMappingOptions.Line:
											m_uv2s[num31].y = (m_vertices[num31].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
											m_uv2s[num31 + 1].y = (m_vertices[num31 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
											m_uv2s[num31 + 2].y = m_uv2s[num31].y;
											m_uv2s[num31 + 3].y = m_uv2s[num31 + 1].y;
											break;
										case TextureMappingOptions.Paragraph:
											m_uv2s[num31].y = (m_vertices[num31].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
											m_uv2s[num31 + 1].y = (m_vertices[num31 + 1].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
											m_uv2s[num31 + 2].y = m_uv2s[num31].y;
											m_uv2s[num31 + 3].y = m_uv2s[num31 + 1].y;
											break;
										case TextureMappingOptions.MatchAspect:
										{
											float num40 = (1f - (m_uv2s[num31].x + m_uv2s[num31 + 2].x) / m_textInfo.characterInfo[k].aspectRatio) / 2f;
											m_uv2s[num31].y = num40 + m_uv2s[num31].x / m_textInfo.characterInfo[k].aspectRatio + m_uvOffset.y;
											m_uv2s[num31 + 1].y = num40 + m_uv2s[num31 + 2].x / m_textInfo.characterInfo[k].aspectRatio + m_uvOffset.y;
											m_uv2s[num31 + 2].y = m_uv2s[num31].y;
											m_uv2s[num31 + 3].y = m_uv2s[num31 + 1].y;
											break;
										}
										}
										float scale = m_textInfo.characterInfo[k].scale;
										Vector3 lossyScale = m_transform.lossyScale;
										float scale2 = scale * lossyScale.z;
										float x = m_uv2s[num31].x;
										float y = m_uv2s[num31].y;
										float x2 = m_uv2s[num31 + 3].x;
										float y2 = m_uv2s[num31 + 3].y;
										float num41 = Mathf.Floor(x);
										float num42 = Mathf.Floor(y);
										x -= num41;
										x2 -= num41;
										y -= num42;
										y2 -= num42;
										m_uv2s[num31] = PackUV(x, y, scale2);
										m_uv2s[num31 + 1] = PackUV(x, y2, scale2);
										m_uv2s[num31 + 2] = PackUV(x2, y, scale2);
										m_uv2s[num31 + 3] = PackUV(x2, y2, scale2);
										if ((m_maxVisibleCharacters != -1 && k >= m_maxVisibleCharacters) || (m_maxVisibleLines != -1 && lineNumber >= m_maxVisibleLines) || (m_overflowMode == TextOverflowModes.Page && m_textInfo.characterInfo[k].pageNumber != m_pageToDisplay))
										{
											m_vertices[num31] *= 0f;
											m_vertices[num31 + 1] *= 0f;
											m_vertices[num31 + 2] *= 0f;
											m_vertices[num31 + 3] *= 0f;
										}
										else
										{
											m_vertices[num31] += zero2;
											m_vertices[num31 + 1] += zero2;
											m_vertices[num31 + 2] += zero2;
											m_vertices[num31 + 3] += zero2;
										}
										num31 += 4;
									}
									m_textInfo.characterInfo[k].bottomLeft += zero2;
									m_textInfo.characterInfo[k].topRight += zero2;
									m_textInfo.characterInfo[k].topLine += zero2.y;
									m_textInfo.characterInfo[k].bottomLine += zero2.y;
									m_textInfo.characterInfo[k].baseLine += zero2.y;
									m_textInfo.lineInfo[lineNumber].ascender = ((!(m_textInfo.characterInfo[k].topLine > m_textInfo.lineInfo[lineNumber].ascender)) ? m_textInfo.lineInfo[lineNumber].ascender : m_textInfo.characterInfo[k].topLine);
									m_textInfo.lineInfo[lineNumber].descender = ((!(m_textInfo.characterInfo[k].bottomLine < m_textInfo.lineInfo[lineNumber].descender)) ? m_textInfo.lineInfo[lineNumber].descender : m_textInfo.characterInfo[k].bottomLine);
									if (lineNumber != num34 || k == m_characterCount - 1)
									{
										if (lineNumber != num34)
										{
											m_textInfo.lineInfo[num34].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num34].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[num34].descender);
											m_textInfo.lineInfo[num34].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[num34].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[num34].ascender);
										}
										if (k == m_characterCount - 1)
										{
											m_textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lineNumber].descender);
											m_textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[lineNumber].ascender);
										}
									}
									if (char.IsLetterOrDigit(character2) && k < m_characterCount - 1)
									{
										if (!flag5)
										{
											flag5 = true;
											num35 = k;
										}
									}
									else if (((char.IsPunctuation(character2) || char.IsWhiteSpace(character2) || k == m_characterCount - 1) && flag5) || k == 0)
									{
										num36 = ((k != m_characterCount - 1 || !char.IsLetterOrDigit(character2)) ? (k - 1) : k);
										flag5 = false;
										num32++;
										m_textInfo.lineInfo[lineNumber].wordCount++;
										TMP_WordInfo item = default(TMP_WordInfo);
										item.firstCharacterIndex = num35;
										item.lastCharacterIndex = num36;
										item.characterCount = num36 - num35 + 1;
										m_textInfo.wordInfo.Add(item);
									}
									if ((m_textInfo.characterInfo[k].style & FontStyles.Underline) == FontStyles.Underline && k != m_textInfo.lineInfo[lineNumber].lastCharacterIndex)
									{
										if (!flag)
										{
											flag = true;
											start = new Vector3(m_textInfo.characterInfo[k].bottomLeft.x, m_textInfo.characterInfo[k].baseLine + font.fontInfo.Underline * m_fontScale, 0f);
										}
									}
									else if (flag)
									{
										flag = false;
										zero = ((k != m_characterCount - 1 && (m_textInfo.characterInfo[k].character == ' ' || m_textInfo.characterInfo[k].character == '\n')) ? new Vector3(m_textInfo.characterInfo[k - 1].topRight.x, m_textInfo.characterInfo[k - 1].baseLine + font.fontInfo.Underline * m_fontScale, 0f) : new Vector3(m_textInfo.characterInfo[k].topRight.x, m_textInfo.characterInfo[k].baseLine + font.fontInfo.Underline * m_fontScale, 0f));
										DrawUnderlineMesh(start, zero, ref index);
									}
									num34 = lineNumber;
								}
								m_textInfo.characterCount = (short)m_characterCount;
								m_textInfo.lineCount = (short)num33;
								m_textInfo.wordCount = ((num32 == 0 || m_characterCount <= 0) ? 1 : ((short)num32));
								m_textInfo.pageCount = m_pageNumber;
								m_textInfo.meshInfo.vertices = m_vertices;
								m_textInfo.meshInfo.uv0s = m_uvs;
								m_textInfo.meshInfo.uv2s = m_uv2s;
								m_textInfo.meshInfo.vertexColors = m_vertColors;
								if (m_renderMode == TextRenderFlags.Render)
								{
									m_mesh.MarkDynamic();
									m_mesh.vertices = m_vertices;
									m_mesh.uv = m_uvs;
									m_mesh.uv2 = m_uv2s;
									m_mesh.colors32 = m_vertColors;
								}
								m_mesh.RecalculateBounds();
								if ((m_textContainer.isDefaultWidth || m_textContainer.isDefaultHeight) && m_textContainer.isAutoFitting)
								{
									if (m_textContainer.isDefaultWidth)
									{
										m_textContainer.width = m_preferredWidth + margins.x + margins.z;
									}
									if (m_textContainer.isDefaultHeight)
									{
										m_textContainer.height = m_preferredHeight + margins.y + margins.w;
									}
									if (m_isMaskingEnabled)
									{
										isMaskUpdateRequired = true;
									}
									GenerateTextMesh();
								}
							}
						}
					}
				}
			}
		}

		private void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index)
		{
			int num = index + 12;
			if (num > m_vertices.Length)
			{
				SetMeshArrays(num / 4 + 16);
				GenerateTextMesh();
			}
			else
			{
				start.y = Mathf.Min(start.y, end.y);
				end.y = Mathf.Min(start.y, end.y);
				float num2 = m_cached_Underline_GlyphInfo.width / 2f * m_fontScale;
				if (end.x - start.x < m_cached_Underline_GlyphInfo.width * m_fontScale)
				{
					num2 = (end.x - start.x) / 2f;
				}
				float height = m_cached_Underline_GlyphInfo.height;
				m_vertices[index] = start + new Vector3(0f, 0f - (height + m_padding) * m_fontScale, 0f);
				m_vertices[index + 1] = start + new Vector3(0f, m_padding * m_fontScale, 0f);
				m_vertices[index + 2] = m_vertices[index] + new Vector3(num2, 0f, 0f);
				m_vertices[index + 3] = start + new Vector3(num2, m_padding * m_fontScale, 0f);
				m_vertices[index + 4] = m_vertices[index + 2];
				m_vertices[index + 5] = m_vertices[index + 3];
				m_vertices[index + 6] = end + new Vector3(0f - num2, (0f - (height + m_padding)) * m_fontScale, 0f);
				m_vertices[index + 7] = end + new Vector3(0f - num2, m_padding * m_fontScale, 0f);
				m_vertices[index + 8] = m_vertices[index + 6];
				m_vertices[index + 9] = m_vertices[index + 7];
				m_vertices[index + 10] = end + new Vector3(0f, (0f - (height + m_padding)) * m_fontScale, 0f);
				m_vertices[index + 11] = end + new Vector3(0f, m_padding * m_fontScale, 0f);
				Vector2 vector = new Vector2((m_cached_Underline_GlyphInfo.x - m_padding) / m_fontAsset.fontInfo.AtlasWidth, 1f - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);
				Vector2 vector2 = new Vector2(vector.x, 1f - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);
				Vector2 vector3 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width / 2f) / m_fontAsset.fontInfo.AtlasWidth, vector.y);
				Vector2 vector4 = new Vector2(vector3.x, vector2.y);
				Vector2 vector5 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, vector.y);
				Vector2 vector6 = new Vector2(vector5.x, vector2.y);
				m_uvs[0 + index] = vector;
				m_uvs[1 + index] = vector2;
				m_uvs[2 + index] = vector3;
				m_uvs[3 + index] = vector4;
				m_uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
				m_uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
				m_uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
				m_uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
				m_uvs[8 + index] = vector3;
				m_uvs[9 + index] = vector4;
				m_uvs[10 + index] = vector5;
				m_uvs[11 + index] = vector6;
				float num3 = 0f;
				float x = (m_vertices[index + 2].x - start.x) / (end.x - start.x);
				float fontScale = m_fontScale;
				Vector3 lossyScale = m_transform.lossyScale;
				float num4 = fontScale * lossyScale.z;
				float scale = num4;
				m_uv2s[0 + index] = PackUV(0f, 0f, num4);
				m_uv2s[1 + index] = PackUV(0f, 1f, num4);
				m_uv2s[2 + index] = PackUV(x, 0f, num4);
				m_uv2s[3 + index] = PackUV(x, 1f, num4);
				num3 = (m_vertices[index + 4].x - start.x) / (end.x - start.x);
				x = (m_vertices[index + 6].x - start.x) / (end.x - start.x);
				m_uv2s[4 + index] = PackUV(num3, 0f, scale);
				m_uv2s[5 + index] = PackUV(num3, 1f, scale);
				m_uv2s[6 + index] = PackUV(x, 0f, scale);
				m_uv2s[7 + index] = PackUV(x, 1f, scale);
				num3 = (m_vertices[index + 8].x - start.x) / (end.x - start.x);
				x = (m_vertices[index + 6].x - start.x) / (end.x - start.x);
				m_uv2s[8 + index] = PackUV(num3, 0f, num4);
				m_uv2s[9 + index] = PackUV(num3, 1f, num4);
				m_uv2s[10 + index] = PackUV(1f, 0f, num4);
				m_uv2s[11 + index] = PackUV(1f, 1f, num4);
				Color32 color = new Color32(m_fontColor32.r, m_fontColor32.g, m_fontColor32.b, (byte)((int)m_fontColor32.a / 2));
				m_vertColors[0 + index] = color;
				m_vertColors[1 + index] = color;
				m_vertColors[2 + index] = color;
				m_vertColors[3 + index] = color;
				m_vertColors[4 + index] = color;
				m_vertColors[5 + index] = color;
				m_vertColors[6 + index] = color;
				m_vertColors[7 + index] = color;
				m_vertColors[8 + index] = color;
				m_vertColors[9 + index] = color;
				m_vertColors[10 + index] = color;
				m_vertColors[11 + index] = color;
				index += 12;
			}
		}

		private void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
		{
		}

		private void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 vector = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				if (m_textInfo.characterInfo[i].isVisible)
				{
					int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;
					m_textInfo.characterInfo[i].bottomLeft -= vector;
					m_textInfo.characterInfo[i].topRight -= vector;
					m_textInfo.characterInfo[i].bottomLine -= vector.y;
					m_textInfo.characterInfo[i].topLine -= vector.y;
					m_vertices[0 + vertexIndex] -= vector;
					m_vertices[1 + vertexIndex] -= vector;
					m_vertices[2 + vertexIndex] -= vector;
					m_vertices[3 + vertexIndex] -= vector;
				}
			}
		}

		private void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = m_visibleCharacterCount;
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
			state.lineOffset = m_lineOffset;
			state.baselineOffset = m_baselineOffset;
			state.fontStyle = m_style;
			state.vertexColor = m_htmlColor;
			state.meshExtents = m_meshExtents;
			state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
			state.textInfo = m_textInfo;
		}

		private int RestoreWordWrappingState(ref WordWrapState state)
		{
			m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;
			m_textInfo = ((state.textInfo == null) ? m_textInfo : state.textInfo);
			m_currentFontSize = state.currentFontSize;
			m_fontScale = state.fontScale;
			m_baselineOffset = state.baselineOffset;
			m_style = state.fontStyle;
			m_htmlColor = state.vertexColor;
			m_characterCount = state.total_CharacterCount + 1;
			m_visibleCharacterCount = state.visible_CharacterCount;
			m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			m_meshExtents = state.meshExtents;
			m_xAdvance = state.xAdvance;
			m_maxAscender = state.maxAscender;
			m_maxDescender = state.maxDescender;
			m_preferredWidth = state.preferredWidth;
			m_preferredHeight = state.preferredHeight;
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
				}
				else
				{
					array[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
					array[i].ascender = 0f - k_InfinityVector.x;
					array[i].descender = k_InfinityVector.x;
				}
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
			int num3 = 0;
			int startIndex2 = 0;
			int num4 = 0;
			int decimalPointIndex = 0;
			endIndex = startIndex;
			bool flag = false;
			int num5 = 1;
			for (int i = startIndex; chars[i] != 0; i++)
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
					break;
				}
				m_htmlTag[num] = (char)chars[i];
				num++;
				if (chars[i] == 61)
				{
					num5 = 0;
				}
				num2 += chars[i] * num * num5;
				num3 += chars[i] * num * (1 - num5);
				switch (chars[i])
				{
				case 61:
					startIndex2 = num;
					break;
				case 46:
					decimalPointIndex = num - 1;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (m_htmlTag[0] == '#' && num == 7)
			{
				m_htmlColor = HexCharsToColor(m_htmlTag, num);
				return true;
			}
			if (m_htmlTag[0] != '#' || num != 9)
			{
				switch (num2)
				{
				case 98:
					m_style |= FontStyles.Bold;
					return true;
				case 105:
					m_style |= FontStyles.Italic;
					return true;
				case 117:
					m_style |= FontStyles.Underline;
					return true;
				case 241:
					return true;
				case 243:
					m_style &= (FontStyles)(-2);
					return true;
				case 257:
					m_style &= (FontStyles)(-3);
					return true;
				case 281:
					m_style &= (FontStyles)(-5);
					return true;
				case 643:
					m_currentFontSize *= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
					m_baselineOffset = m_fontAsset.fontInfo.SubscriptOffset;
					m_isRecalculateScaleRequired = true;
					return true;
				case 679:
				{
					float num6 = ConvertToFloat(m_htmlTag, startIndex2, num - 1, decimalPointIndex);
					m_xAdvance = num6 * m_fontScale * m_fontAsset.fontInfo.TabWidth;
					return true;
				}
				case 685:
					m_currentFontSize *= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
					m_baselineOffset = m_fontAsset.fontInfo.SuperscriptOffset;
					m_isRecalculateScaleRequired = true;
					return true;
				case 1019:
					if (m_overflowMode == TextOverflowModes.Page)
					{
						m_xAdvance = 0f + m_indent;
						m_lineOffset = 0f;
						m_pageNumber++;
						m_isNewPage = true;
					}
					return true;
				case 1020:
					m_currentFontSize /= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
					m_baselineOffset = 0f;
					m_isRecalculateScaleRequired = true;
					return true;
				case 1076:
					m_currentFontSize /= ((!(m_fontAsset.fontInfo.SubSize > 0f)) ? 1f : m_fontAsset.fontInfo.SubSize);
					m_baselineOffset = 0f;
					m_isRecalculateScaleRequired = true;
					return true;
				case 1095:
				{
					num4 = num - 1;
					float num7 = 0f;
					if (m_htmlTag[5] == '%')
					{
						num7 = ConvertToFloat(m_htmlTag, startIndex2, num4, decimalPointIndex);
						m_currentFontSize = m_fontSize * num7 / 100f;
						m_isRecalculateScaleRequired = true;
						return true;
					}
					if (m_htmlTag[5] == '+')
					{
						num7 = ConvertToFloat(m_htmlTag, startIndex2, num4, decimalPointIndex);
						m_currentFontSize = m_fontSize + num7;
						m_isRecalculateScaleRequired = true;
						return true;
					}
					if (m_htmlTag[5] == '-')
					{
						num7 = ConvertToFloat(m_htmlTag, startIndex2, num4, decimalPointIndex);
						m_currentFontSize = m_fontSize + num7;
						m_isRecalculateScaleRequired = true;
						return true;
					}
					num7 = ConvertToFloat(m_htmlTag, startIndex2, num4, decimalPointIndex);
					if (num7 == 73493f)
					{
						return false;
					}
					m_currentFontSize = num7;
					m_isRecalculateScaleRequired = true;
					return true;
				}
				case 1118:
					UnityEngine.Debug.Log("Font Tag used.");
					return true;
				case 1531:
				{
					float num6 = ConvertToFloat(m_htmlTag, startIndex2, num - 1, decimalPointIndex);
					m_xAdvance += num6 * m_fontScale * m_fontAsset.fontInfo.TabWidth;
					return true;
				}
				case 1550:
					m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
					return true;
				case 1585:
					m_currentFontSize = m_fontSize;
					m_isRecalculateScaleRequired = true;
					return true;
				case 1590:
					switch (num3)
					{
					case 4008:
						m_lineJustification = TextAlignmentOptions.Left;
						return true;
					case 5247:
						m_lineJustification = TextAlignmentOptions.Right;
						return true;
					case 6496:
						m_lineJustification = TextAlignmentOptions.Center;
						return true;
					case 10897:
						m_lineJustification = TextAlignmentOptions.Justified;
						return true;
					default:
						return false;
					}
				case 1659:
					if (m_htmlTag[6] == '#' && num == 13)
					{
						m_htmlColor = HexCharsToColor(m_htmlTag, num);
						return true;
					}
					if (m_htmlTag[6] != '#' || num != 15)
					{
						switch (num3)
						{
						case 2872:
							m_htmlColor = Color.red;
							return true;
						case 3979:
							m_htmlColor = Color.blue;
							return true;
						case 4956:
							m_htmlColor = Color.black;
							return true;
						case 5128:
							m_htmlColor = Color.green;
							return true;
						case 5247:
							m_htmlColor = Color.white;
							return true;
						case 6373:
							m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
							return true;
						case 6632:
							m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
							return true;
						case 6722:
							m_htmlColor = Color.yellow;
							return true;
						default:
							return false;
						}
					}
					m_htmlColor = HexCharsToColor(m_htmlTag, num);
					return true;
				case 6691:
				case 1901:
					m_lineHeight = ConvertToFloat(m_htmlTag, startIndex2, num - 1, decimalPointIndex);
					return true;
				case 2030:
					return true;
				case 2154:
					m_cSpacing = ConvertToFloat(m_htmlTag, startIndex2, num - 1, decimalPointIndex);
					return true;
				case 2160:
					m_lineJustification = m_textAlignment;
					return true;
				case 2164:
					m_monoSpacing = ConvertToFloat(m_htmlTag, startIndex2, num - 1, decimalPointIndex);
					return true;
				case 2249:
					m_htmlColor = m_fontColor32;
					return true;
				case 2275:
					m_indent = ConvertToFloat(m_htmlTag, startIndex2, num - 1, decimalPointIndex);
					m_xAdvance = m_indent;
					return true;
				case 2287:
					UnityEngine.Debug.Log("Sprite Tag used.");
					return true;
				case 7840:
				case 2521:
					m_lineHeight = 0f;
					return true;
				case 2844:
					m_monoSpacing = 0f;
					return true;
				case 2824:
					m_cSpacing = 0f;
					return true;
				case 2964:
					m_indent = 0f;
					return true;
				case 2995:
					m_style |= FontStyles.UpperCase;
					return true;
				case 3778:
					m_style &= (FontStyles)(-17);
					return true;
				case 4800:
					m_style |= FontStyles.SmallCaps;
					return true;
				case 5807:
					m_currentFontSize = m_fontSize;
					m_style &= (FontStyles)(-33);
					m_isRecalculateScaleRequired = true;
					return true;
				default:
					return false;
				}
			}
			m_htmlColor = HexCharsToColor(m_htmlTag, num);
			return true;
		}

		public void UpdateMeshPadding()
		{
			m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
			havePropertiesChanged = true;
		}

		public void ForceMeshUpdate()
		{
			havePropertiesChanged = true;
			OnWillRenderObject();
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
			if (!(text == old_text) || arg0 != old_arg0 || arg1 != old_arg1 || arg2 != old_arg2)
			{
				if (m_input_CharArray.Length < text.Length)
				{
					m_input_CharArray = new char[Mathf.NextPowerOfTwo(text.Length + 1)];
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
						switch (text[i + 1])
						{
						case '0':
							old_arg0 = arg0;
							AddFloatToCharArray(arg0, ref index, precision);
							break;
						case '1':
							old_arg1 = arg1;
							AddFloatToCharArray(arg1, ref index, precision);
							break;
						case '2':
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
				havePropertiesChanged = true;
			}
		}

		public void SetCharArray(char[] charArray)
		{
			if (charArray != null && charArray.Length != 0)
			{
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
						case 't':
							break;
						case 'n':
							goto IL_0086;
						default:
							goto IL_009d;
						}
						m_char_buffer[num2] = 9;
						i++;
						num2++;
						continue;
					}
					goto IL_009d;
					IL_0086:
					m_char_buffer[num2] = 10;
					i++;
					num2++;
					continue;
					IL_009d:
					m_char_buffer[num2] = charArray[i];
					num2++;
				}
				m_char_buffer[num2] = 0;
				m_inputSource = TextInputSources.SetCharArray;
				havePropertiesChanged = true;
				isInputParsingRequired = true;
			}
		}
	}
}
