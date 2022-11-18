using Assets.Scripts.Core;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{
	public class AVProMovieRenderer : MonoBehaviour, IMovieRenderer
	{
		public MeshRenderer Renderer;

		public bool isStarted;

		public void OnAvProVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
		{
			if (base.enabled)
			{
				if (errorCode != 0)
				{
					Debug.LogError("Encounted video error, stopping video playback.");
					GameSystem.Instance.PopStateStack();
				}
				else
				{
					switch (et)
					{
					case MediaPlayerEvent.EventType.FirstFrameReady:
						Renderer.enabled = true;
						isStarted = true;
						break;
					case MediaPlayerEvent.EventType.FinishedPlaying:
						Quit();
						GameSystem.Instance.PopStateStack();
						break;
					}
				}
			}
		}

		public void Quit()
		{
			MediaPlayer component = GetComponent<MediaPlayer>();
			if (component != null)
			{
				component.CloseVideo();
			}
			if (Renderer != null)
			{
				Renderer.enabled = false;
			}
			base.enabled = false;
		}

		public void Init(MovieInfo movieInfo)
		{
			MediaPlayer mediaPlayer = base.gameObject.AddComponent<MediaPlayer>();
			mediaPlayer.Events.AddListener(OnAvProVideoEvent);
			mediaPlayer.m_AutoOpen = true;
			mediaPlayer.m_AutoStart = true;
			mediaPlayer.m_Volume = movieInfo.Volume;
			mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, movieInfo.PathWithExt);
			MODApplyToMaterial mODApplyToMaterial = base.gameObject.AddComponent<MODApplyToMaterial>();
			mODApplyToMaterial._material = movieInfo.Layer.MODMaterial;
			mODApplyToMaterial._texturePropertyName = "_Primary";
			mODApplyToMaterial._media = mediaPlayer;
			Renderer = movieInfo.Layer.MODMeshRenderer;
			Renderer.enabled = false;
			base.gameObject.AddComponent<AudioOutput>().ChangeMediaPlayer(mediaPlayer);
		}
	}
}
