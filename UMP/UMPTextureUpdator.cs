using UnityEngine;
using UnityEngine.UI;

namespace UMP
{
	public class UMPTextureUpdator : MonoBehaviour
	{
		[SerializeField]
		private RawImage _image;

		[SerializeField]
		private UniversalMediaPlayer _player;

		private Texture2D _texture;

		private long _framesCounter;

		private void Start()
		{
			_player.AddImageReadyEvent(OnImageReady);
			_player.AddStoppedEvent(OnStop);
		}

		private void Update()
		{
			if (_texture != null && _framesCounter != _player.FramesCounter)
			{
				_texture.LoadRawTextureData(_player.FramePixels);
				_texture.Apply();
				_framesCounter = _player.FramesCounter;
			}
		}

		private void OnDestroy()
		{
			_player.RemoveStoppedEvent(OnStop);
		}

		private void OnImageReady(Texture image)
		{
			if (_texture != null)
			{
				Object.Destroy(_texture);
			}
			_texture = MediaPlayerHelper.GenVideoTexture(image.width, image.height);
			_texture.Apply();
			_image.texture = _texture;
		}

		private void OnStop()
		{
			if (_texture != null)
			{
				Object.Destroy(_texture);
			}
			_texture = null;
		}
	}
}
