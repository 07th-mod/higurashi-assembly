using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Display uGUI", 200)]
	[ExecuteInEditMode]
	public class DisplayUGUI : MaskableGraphic
	{
		[SerializeField]
		public MediaPlayer _mediaPlayer;

		[SerializeField]
		public Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		[SerializeField]
		public bool _setNativeSize;

		[SerializeField]
		public ScaleMode _scaleMode = ScaleMode.ScaleToFit;

		[SerializeField]
		public bool _noDefaultDisplay = true;

		[SerializeField]
		public bool _displayInEditor = true;

		[SerializeField]
		public Texture _defaultTexture;

		private int _lastWidth;

		private int _lastHeight;

		private bool _flipY;

		private Texture _lastTexture;

		private static Shader _shaderStereoPacking;

		private static Shader _shaderAlphaPacking;

		private static int _propAlphaPack;

		private static int _propVertScale;

		private static int _propStereo;

		private static int _propApplyGamma;

		private bool _userMaterial = true;

		private Material _material;

		public override Texture mainTexture
		{
			get
			{
				Texture texture = Texture2D.whiteTexture;
				if (HasValidTexture())
				{
					texture = _mediaPlayer.TextureProducer.GetTexture();
				}
				else
				{
					if (_noDefaultDisplay)
					{
						texture = null;
					}
					else if (_defaultTexture != null)
					{
						texture = _defaultTexture;
					}
					if (texture == null && !_displayInEditor)
					{
					}
				}
				return texture;
			}
		}

		public MediaPlayer CurrentMediaPlayer
		{
			get
			{
				return _mediaPlayer;
			}
			set
			{
				if (_mediaPlayer != value)
				{
					_mediaPlayer = value;
					SetMaterialDirty();
				}
			}
		}

		public Rect uvRect
		{
			get
			{
				return m_UVRect;
			}
			set
			{
				if (!(m_UVRect == value))
				{
					m_UVRect = value;
					SetVerticesDirty();
				}
			}
		}

		protected override void Awake()
		{
			if (_propAlphaPack == 0)
			{
				_propStereo = Shader.PropertyToID("Stereo");
				_propAlphaPack = Shader.PropertyToID("AlphaPack");
				_propVertScale = Shader.PropertyToID("_VertScale");
				_propApplyGamma = Shader.PropertyToID("_ApplyGamma");
			}
			if (_shaderAlphaPacking == null)
			{
				_shaderAlphaPacking = Shader.Find("AVProVideo/UI/Transparent Packed");
			}
			if (_shaderStereoPacking == null)
			{
				_shaderStereoPacking = Shader.Find("AVProVideo/UI/Stereo");
			}
			base.Awake();
		}

		protected override void Start()
		{
			_userMaterial = (m_Material != null);
			base.Start();
		}

		protected override void OnDestroy()
		{
			if (_material != null)
			{
				material = null;
				UnityEngine.Object.Destroy(_material);
				_material = null;
			}
			base.OnDestroy();
		}

		private Shader GetRequiredShader()
		{
			Shader shader = null;
			switch (_mediaPlayer.m_StereoPacking)
			{
			case StereoPacking.TopBottom:
			case StereoPacking.LeftRight:
				shader = _shaderStereoPacking;
				break;
			}
			switch (_mediaPlayer.m_AlphaPacking)
			{
			case AlphaPacking.TopBottom:
			case AlphaPacking.LeftRight:
				shader = _shaderAlphaPacking;
				break;
			}
			if (shader == null && _mediaPlayer.Info != null && QualitySettings.activeColorSpace == ColorSpace.Linear && !_mediaPlayer.Info.PlayerSupportsLinearColorSpace())
			{
				shader = _shaderAlphaPacking;
			}
			return shader;
		}

		public bool HasValidTexture()
		{
			return _mediaPlayer != null && _mediaPlayer.TextureProducer != null && _mediaPlayer.TextureProducer.GetTexture() != null;
		}

		private void UpdateInternalMaterial()
		{
			if (_mediaPlayer != null)
			{
				Shader x = null;
				if (_material != null)
				{
					x = _material.shader;
				}
				Shader requiredShader = GetRequiredShader();
				if (x != requiredShader)
				{
					if (_material != null)
					{
						material = null;
						UnityEngine.Object.Destroy(_material);
						_material = null;
					}
					if (requiredShader != null)
					{
						_material = new Material(requiredShader);
					}
				}
				material = _material;
			}
		}

		private void LateUpdate()
		{
			if (_setNativeSize)
			{
				SetNativeSize();
			}
			if (_lastTexture != mainTexture)
			{
				_lastTexture = mainTexture;
				SetVerticesDirty();
			}
			if (HasValidTexture() && mainTexture != null && (mainTexture.width != _lastWidth || mainTexture.height != _lastHeight))
			{
				_lastWidth = mainTexture.width;
				_lastHeight = mainTexture.height;
				SetVerticesDirty();
			}
			if (!_userMaterial && Application.isPlaying)
			{
				UpdateInternalMaterial();
			}
			if (material != null && _mediaPlayer != null)
			{
				if (material.HasProperty(_propAlphaPack))
				{
					Helper.SetupAlphaPackedMaterial(material, _mediaPlayer.m_AlphaPacking);
					if (_flipY && _mediaPlayer.m_AlphaPacking != 0)
					{
						material.SetFloat(_propVertScale, -1f);
					}
					else
					{
						material.SetFloat(_propVertScale, 1f);
					}
				}
				if (material.HasProperty(_propStereo))
				{
					Helper.SetupStereoMaterial(material, _mediaPlayer.m_StereoPacking, _mediaPlayer.m_DisplayDebugStereoColorTint);
				}
				if (material.HasProperty(_propApplyGamma) && _mediaPlayer.Info != null)
				{
					Helper.SetupGammaMaterial(material, _mediaPlayer.Info.PlayerSupportsLinearColorSpace());
				}
			}
			SetMaterialDirty();
		}

		[ContextMenu("Set Native Size")]
		public override void SetNativeSize()
		{
			Texture mainTexture = this.mainTexture;
			if (mainTexture != null)
			{
				int num = Mathf.RoundToInt((float)mainTexture.width * uvRect.width);
				int num2 = Mathf.RoundToInt((float)mainTexture.height * uvRect.height);
				if (_mediaPlayer != null)
				{
					if (_mediaPlayer.m_AlphaPacking == AlphaPacking.LeftRight || _mediaPlayer.m_StereoPacking == StereoPacking.LeftRight)
					{
						num /= 2;
					}
					else if (_mediaPlayer.m_AlphaPacking == AlphaPacking.TopBottom || _mediaPlayer.m_StereoPacking == StereoPacking.TopBottom)
					{
						num2 /= 2;
					}
				}
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			List<UIVertex> list = new List<UIVertex>();
			_OnFillVBO(list);
			List<int> indicies = new List<int>(new int[6]
			{
				0,
				1,
				2,
				2,
				3,
				0
			});
			vh.AddUIVertexStream(list, indicies);
		}

		[Obsolete("This method is not called from Unity 5.2 and above")]
		protected override void OnFillVBO(List<UIVertex> vbo)
		{
			_OnFillVBO(vbo);
		}

		private void _OnFillVBO(List<UIVertex> vbo)
		{
			_flipY = false;
			if (HasValidTexture())
			{
				_flipY = _mediaPlayer.TextureProducer.RequiresVerticalFlip();
			}
			Rect uvRect = m_UVRect;
			Vector4 drawingDimensions = GetDrawingDimensions(_scaleMode, ref uvRect);
			vbo.Clear();
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = new Vector2(drawingDimensions.x, drawingDimensions.y);
			simpleVert.uv0 = new Vector2(uvRect.xMin, uvRect.yMin);
			if (_flipY)
			{
				simpleVert.uv0 = new Vector2(uvRect.xMin, 1f - uvRect.yMin);
			}
			simpleVert.color = color;
			vbo.Add(simpleVert);
			simpleVert.position = new Vector2(drawingDimensions.x, drawingDimensions.w);
			simpleVert.uv0 = new Vector2(uvRect.xMin, uvRect.yMax);
			if (_flipY)
			{
				simpleVert.uv0 = new Vector2(uvRect.xMin, 1f - uvRect.yMax);
			}
			simpleVert.color = color;
			vbo.Add(simpleVert);
			simpleVert.position = new Vector2(drawingDimensions.z, drawingDimensions.w);
			simpleVert.uv0 = new Vector2(uvRect.xMax, uvRect.yMax);
			if (_flipY)
			{
				simpleVert.uv0 = new Vector2(uvRect.xMax, 1f - uvRect.yMax);
			}
			simpleVert.color = color;
			vbo.Add(simpleVert);
			simpleVert.position = new Vector2(drawingDimensions.z, drawingDimensions.y);
			simpleVert.uv0 = new Vector2(uvRect.xMax, uvRect.yMin);
			if (_flipY)
			{
				simpleVert.uv0 = new Vector2(uvRect.xMax, 1f - uvRect.yMin);
			}
			simpleVert.color = color;
			vbo.Add(simpleVert);
		}

		private Vector4 GetDrawingDimensions(ScaleMode scaleMode, ref Rect uvRect)
		{
			Vector4 result = Vector4.zero;
			if (mainTexture != null)
			{
				Vector4 zero = Vector4.zero;
				Vector2 vector = new Vector2((float)mainTexture.width, (float)mainTexture.height);
				if (_mediaPlayer != null)
				{
					if (_mediaPlayer.m_AlphaPacking == AlphaPacking.LeftRight || _mediaPlayer.m_StereoPacking == StereoPacking.LeftRight)
					{
						vector.x /= 2f;
					}
					else if (_mediaPlayer.m_AlphaPacking == AlphaPacking.TopBottom || _mediaPlayer.m_StereoPacking == StereoPacking.TopBottom)
					{
						vector.y /= 2f;
					}
				}
				Rect pixelAdjustedRect = GetPixelAdjustedRect();
				int num = Mathf.RoundToInt(vector.x);
				int num2 = Mathf.RoundToInt(vector.y);
				Vector4 vector2 = new Vector4(zero.x / (float)num, zero.y / (float)num2, ((float)num - zero.z) / (float)num, ((float)num2 - zero.w) / (float)num2);
				if (vector.sqrMagnitude > 0f)
				{
					switch (scaleMode)
					{
					case ScaleMode.ScaleToFit:
					{
						float num7 = vector.x / vector.y;
						float num8 = pixelAdjustedRect.width / pixelAdjustedRect.height;
						if (num7 > num8)
						{
							float height = pixelAdjustedRect.height;
							pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num7);
							float y = pixelAdjustedRect.y;
							float num9 = height - pixelAdjustedRect.height;
							Vector2 pivot = base.rectTransform.pivot;
							pixelAdjustedRect.y = y + num9 * pivot.y;
						}
						else
						{
							float width = pixelAdjustedRect.width;
							pixelAdjustedRect.width = pixelAdjustedRect.height * num7;
							float x = pixelAdjustedRect.x;
							float num10 = width - pixelAdjustedRect.width;
							Vector2 pivot2 = base.rectTransform.pivot;
							pixelAdjustedRect.x = x + num10 * pivot2.x;
						}
						break;
					}
					case ScaleMode.ScaleAndCrop:
					{
						float num3 = vector.x / vector.y;
						float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
						if (num4 > num3)
						{
							float num5 = num3 / num4;
							uvRect = new Rect(0f, (1f - num5) * 0.5f, 1f, num5);
						}
						else
						{
							float num6 = num4 / num3;
							uvRect = new Rect(0.5f - num6 * 0.5f, 0f, num6, 1f);
						}
						break;
					}
					}
				}
				result = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector2.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector2.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector2.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector2.w);
			}
			return result;
		}
	}
}
