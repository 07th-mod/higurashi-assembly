using Assets.Scripts.Core;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace MOD.Scripts.Core.Movie
{
	public class VideoPlayerMovieRenderer : MonoBehaviour, IMovieRenderer
	{

		private static string[] WindowsExtensionsList = { ".mp4", ".webm", ".ogv", ".vp8", ".asf", ".avi", ".dv", ".m4v", ".mov", ".mpg", ".mpeg", ".wmv" };
		private static string[] MacLinuxExtensionsList = { ".webm", ".ogv", ".vp8" };

		enum PlayState
		{
			Idle,
			Loading,
			Playing,
			Finished,
		}

		PlayState state = PlayState.Idle;

		private AudioSource audioSource;

		private MovieInfo movieInfo;

		private VideoPlayer videoPlayer;

		public static bool GetFullVideoPath(string pathNoExtension, out string pathWithExtension, out string debugErrorMessage)
		{
			debugErrorMessage = "No Error";

			string[] extensionList = WindowsExtensionsList;
			if (Application.platform != RuntimePlatform.WindowsPlayer)
			{
				extensionList = MacLinuxExtensionsList;
			}

			foreach (string ext in extensionList)
			{
				string fullPathToTest = pathNoExtension + ext;
				Debug.Log($"Testing path {fullPathToTest}");
				if (File.Exists(fullPathToTest))
				{
					pathWithExtension = fullPathToTest;
					return true;
				}
			}

			debugErrorMessage = $"Error: No video with extension [{string.Join("|", extensionList)}] found at [{pathNoExtension}(.EXT))]";
			Debug.Log(debugErrorMessage);

			pathWithExtension = null;
			return false;
		}

		public void OnVideoPlaybackError(string reason)
		{
			Debug.LogError($"VideoPlayer Error: {reason}");
			UI.MODToaster.Show($"Movie Playback Failed - check game log");
			Finished();
			state = PlayState.Finished;
		}

		void VideoPlayerErrorEventHandler(VideoPlayer source, string message)
		{
			OnVideoPlaybackError(message);
		}

		public void Update()
		{
			switch (state)
			{
				case PlayState.Idle:
					{
						if(GetFullVideoPath(movieInfo.PathNoExtension, out string pathWithExtension, out _))
						{
							videoPlayer = gameObject.AddComponent<VideoPlayer>();
							videoPlayer.errorReceived += VideoPlayerErrorEventHandler;

							videoPlayer.url = pathWithExtension;
							videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
							videoPlayer.targetMaterialRenderer = movieInfo.Layer.MODMeshRenderer;
							videoPlayer.targetMaterialProperty = "_Primary";

							videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
							videoPlayer.SetTargetAudioSource(0, audioSource);

							Debug.Log($"ModPlayMovie: Beginning Movie Playback of [{pathWithExtension}]....");
							videoPlayer.Play();

							state = PlayState.Loading;
						}
						else
						{
							OnVideoPlaybackError($"ModPlayMovie: Couldn't find video file - exiting playback");
						}
					}
					break;

				case PlayState.Loading:
					if (videoPlayer.isPrepared)
					{
						state = PlayState.Playing;
					}
					break;

				case PlayState.Playing:
					if(!videoPlayer.isPlaying)
					{
						Finished();
						state = PlayState.Finished;
					}
					break;

				case PlayState.Finished:
					break;
			}
		}

		public void OnDestroy()
		{
			Quit();
			if (videoPlayer != null)
			{
				Object.Destroy(videoPlayer);
			}
		}

		public void Quit()
		{
			if (videoPlayer != null)
			{
				videoPlayer.Stop();
			}
			if (audioSource != null)
			{
				audioSource.Stop();
			}
			base.enabled = false;
		}

		public void Finished()
		{
			Quit();
			GameSystem.Instance.PopStateStack();
		}

		public VideoPlayerMovieRenderer()
		{
			base.enabled = false;
		}

		public void Init(MovieInfo movieInfo)
		{
			this.movieInfo = movieInfo;

			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.volume = this.movieInfo.Volume;
			base.enabled = true;
		}
	}
}
