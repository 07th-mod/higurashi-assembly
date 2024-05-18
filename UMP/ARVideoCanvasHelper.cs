using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UMP
{
	public class ARVideoCanvasHelper : MonoBehaviour
	{
		private const string SHADER_BORDER_U_WIDTH = "_BorderUWidth";

		private const string SHADER_BORDER_V_WIDTH = "_BorderVWidth";

		[SerializeField]
		private Camera _mainCamera;

		[SerializeField]
		private UniversalMediaPlayer _mediaPlayer;

		[SerializeField]
		public int _defaultWidth = 640;

		[SerializeField]
		public int _defaultHeight = 640;

		private MeshRenderer _meshRenderer;

		private RawImage _rawImageRenderer;

		private Material _objectMaterial;

		private Vector2 _objectSize;

		private Vector2 _videoSize;

		private Vector2 _calcSize;

		private Vector2 _borderUVSize;

		private IEnumerator _updateCanvasRatioEnum;

		private void Start()
		{
			_rawImageRenderer = base.gameObject.GetComponent<RawImage>();
			_meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
			if (_rawImageRenderer == null && _meshRenderer == null)
			{
				Debug.LogError("Object need have MeshRenderer or RawImage component!");
				return;
			}
			_objectMaterial = ((!(_rawImageRenderer != null)) ? _meshRenderer.material : _rawImageRenderer.material);
			if (_mediaPlayer != null)
			{
				_mediaPlayer.AddImageReadyEvent(OnPlayerImageReady);
				_mediaPlayer.AddStoppedEvent(OnPlayerStopped);
			}
			ShowVideoCanvasBorder(state: true);
		}

		private void OnDestroy()
		{
			if (_mediaPlayer != null)
			{
				_mediaPlayer.RemoveImageReadyEvent(OnPlayerImageReady);
				_mediaPlayer.RemoveStoppedEvent(OnPlayerStopped);
			}
			if (_updateCanvasRatioEnum != null)
			{
				StopCoroutine(_updateCanvasRatioEnum);
			}
			ShowVideoCanvasBorder(state: false);
		}

		private IEnumerator UpdateVideoCanvasRatio()
		{
			while (true)
			{
				_objectSize = GetPixelSizeOfMeshRenderer(_meshRenderer, _mainCamera);
				if (_objectSize == Vector2.zero)
				{
					_objectSize = GetPixelSizeOfRawImage(_rawImageRenderer);
				}
				_calcSize = Vector2.zero;
				_calcSize.x = _objectSize.y / _videoSize.y * _videoSize.x;
				if (_calcSize.x < _objectSize.x)
				{
					_calcSize.y = _objectSize.y;
				}
				else
				{
					_calcSize = new Vector2(_objectSize.x, _objectSize.x / _videoSize.x * _videoSize.y);
				}
				_borderUVSize = new Vector2((1f - _calcSize.x / _objectSize.x) * 0.5f, (1f - _calcSize.y / _objectSize.y) * 0.5f);
				_objectMaterial.SetFloat("_BorderUWidth", _borderUVSize.x);
				_objectMaterial.SetFloat("_BorderVWidth", _borderUVSize.y);
				yield return null;
			}
		}

		private void ShowVideoCanvasBorder(bool state)
		{
			_objectMaterial.SetFloat("_BorderUWidth", state ? 1 : 0);
			_objectMaterial.SetFloat("_BorderVWidth", state ? 1 : 0);
		}

		private void OnPlayerImageReady(Texture2D image)
		{
			_videoSize = new Vector2(image.width, image.height);
			if (_updateCanvasRatioEnum == null)
			{
				_updateCanvasRatioEnum = UpdateVideoCanvasRatio();
				StartCoroutine(_updateCanvasRatioEnum);
			}
		}

		private void OnPlayerStopped()
		{
			if (!_mediaPlayer.Loop)
			{
				if (_updateCanvasRatioEnum != null)
				{
					StopCoroutine(_updateCanvasRatioEnum);
					_updateCanvasRatioEnum = null;
				}
				ShowVideoCanvasBorder(state: true);
			}
		}

		public static Vector2 GetPixelSizeOfMeshRenderer(MeshRenderer meshRenderer, Camera camera)
		{
			if (meshRenderer == null)
			{
				return Vector2.zero;
			}
			Vector3 min = meshRenderer.bounds.min;
			float x = min.x;
			Vector3 min2 = meshRenderer.bounds.min;
			float y = min2.y;
			Vector3 min3 = meshRenderer.bounds.min;
			Vector3 vector = camera.WorldToScreenPoint(new Vector3(x, y, min3.z));
			Vector3 max = meshRenderer.bounds.max;
			float x2 = max.x;
			Vector3 max2 = meshRenderer.bounds.max;
			float y2 = max2.y;
			Vector3 min4 = meshRenderer.bounds.min;
			Vector3 vector2 = camera.WorldToScreenPoint(new Vector3(x2, y2, min4.z));
			float num = Mathf.Abs(vector2.x - vector.x);
			float num2 = Mathf.Abs(vector2.y - vector.y);
			return new Vector2((int)num, (int)num2);
		}

		public static Vector2 GetPixelSizeOfRawImage(RawImage rawImage)
		{
			if (rawImage == null)
			{
				return Vector2.zero;
			}
			return new Vector2(rawImage.rectTransform.rect.width, rawImage.rectTransform.rect.height);
		}
	}
}
