using UnityEngine;

namespace RenderHeads.Media.AVProVideo
{
	[AddComponentMenu("AVPro Video/Apply To Mesh", 300)]
	public class ApplyToMesh : MonoBehaviour
	{
		public Vector2 _offset = Vector2.zero;

		public Vector2 _scale = Vector2.one;

		public MeshRenderer _mesh;

		public MediaPlayer _media;

		public Texture2D _defaultTexture;

		private static int _propStereo;

		private static int _propAlphaPack;

		private static int _propApplyGamma;

		private void Awake()
		{
			if (_propStereo == 0 || _propAlphaPack == 0)
			{
				_propStereo = Shader.PropertyToID("Stereo");
				_propAlphaPack = Shader.PropertyToID("AlphaPack");
				_propApplyGamma = Shader.PropertyToID("_ApplyGamma");
			}
		}

		private void LateUpdate()
		{
			bool flag = false;
			if (_media != null && _media.TextureProducer != null)
			{
				Texture texture = _media.TextureProducer.GetTexture();
				if (texture != null)
				{
					ApplyMapping(texture, _media.TextureProducer.RequiresVerticalFlip());
					flag = true;
				}
			}
			if (!flag)
			{
				ApplyMapping(_defaultTexture, requiresYFlip: false);
			}
		}

		private void ApplyMapping(Texture texture, bool requiresYFlip)
		{
			if (_mesh != null)
			{
				Material[] materials = _mesh.materials;
				if (materials != null)
				{
					foreach (Material material in materials)
					{
						if (material != null)
						{
							material.mainTexture = texture;
							if (texture != null)
							{
								if (requiresYFlip)
								{
									material.mainTextureScale = new Vector2(_scale.x, 0f - _scale.y);
									material.mainTextureOffset = Vector2.up + _offset;
								}
								else
								{
									material.mainTextureScale = _scale;
									material.mainTextureOffset = _offset;
								}
							}
							if (_media != null)
							{
								if (material.HasProperty(_propStereo))
								{
									Helper.SetupStereoMaterial(material, _media.m_StereoPacking, _media.m_DisplayDebugStereoColorTint);
								}
								if (material.HasProperty(_propAlphaPack))
								{
									Helper.SetupAlphaPackedMaterial(material, _media.m_AlphaPacking);
								}
								if (material.HasProperty(_propApplyGamma) && _media.Info != null)
								{
									Helper.SetupGammaMaterial(material, _media.Info.PlayerSupportsLinearColorSpace());
								}
							}
						}
					}
				}
			}
		}

		private void OnEnable()
		{
			if (_mesh == null)
			{
				_mesh = GetComponent<MeshRenderer>();
				if (_mesh == null)
				{
					Debug.LogWarning("[AVProVideo] No mesh renderer set or found in gameobject");
				}
			}
			if (_mesh != null)
			{
				LateUpdate();
			}
		}

		private void OnDisable()
		{
			ApplyMapping(_defaultTexture, requiresYFlip: false);
		}
	}
}
