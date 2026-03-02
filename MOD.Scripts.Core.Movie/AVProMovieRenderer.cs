using Assets.Scripts.Core;
using Assets.Scripts.Core.Scene;
using Assets.Scripts.UI.Choice;
using RenderHeads.Media.AVProVideo;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{
	public class AVProMovieRenderer : MonoBehaviour, IMovieRenderer
	{
		public MeshRenderer Renderer;

		public bool isStarted;

		private Layer MovieInfoLayer;

		MediaPlayer mediaPlayer;

		ChoiceButton cb;

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
					case MediaPlayerEvent.EventType.Started:
						MovieInfoLayer.MoveLayer(100, 100, -1, 0.5f, 0, 0, isBlocking: false, adjustAlpha: true);
						break;
					case MediaPlayerEvent.EventType.FinishedPlaying:
						Quit();
						GameSystem.Instance.PopStateStack();
						break;
					}
				}
			}
		}

		private void Update()
		{
			//GameSystem.Instance.TextController.ForceText($"Time passed: {mediaPlayer.Control.GetCurrentTimeMs()}");
			//GameSystem.Instance.MainUIController.ShowMessageBox();
			//Debug.Log($"{cb.ButtonTextMesh.alignment}");


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
			mediaPlayer = base.gameObject.AddComponent<MediaPlayer>();
			mediaPlayer.DisplayDebugGUI = true;
			mediaPlayer.Events.AddListener(OnAvProVideoEvent);
			mediaPlayer.m_AutoOpen = true;
			mediaPlayer.m_AutoStart = true;
			mediaPlayer.m_Volume = movieInfo.Volume;
			mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, movieInfo.PathWithExt);
			MODApplyToMaterial mODApplyToMaterial = base.gameObject.AddComponent<MODApplyToMaterial>();
			mODApplyToMaterial._material = movieInfo.Layer.MODMaterial;
			mODApplyToMaterial._texturePropertyName = "_Primary";
			mODApplyToMaterial._media = mediaPlayer;
			MovieInfoLayer = movieInfo.Layer;

			//GameSystem.Instance.DisplayChoices(new List<string>() { "aasdf" }, 1);
			MODLogger.Log("Spawning Text", true);
			TextSpawner spawner = new TextSpawner();
			cb = spawner.SpawnText("This is a long line");

			//GameSystem.Instance.TextController.ForceText($"Time passed: ");

			Renderer = movieInfo.Layer.MODMeshRenderer;
			Renderer.enabled = false;
			base.gameObject.AddComponent<AudioOutput>().ChangeMediaPlayer(mediaPlayer);
		}
	}
}
