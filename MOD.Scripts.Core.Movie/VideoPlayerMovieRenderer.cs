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

		public bool GetFullVideoPath(string pathNoExtension, out string pathWithExtension)
		{
			string[] extensionList = WindowsExtensionsList;
			// For now, assume Proton/Wine does not support .mp4 playback. See:
			// - https://github.com/07th-mod/hou-plus/issues/12
			// - https://github.com/07th-mod/higurashi-assembly/commit/67746c80b14b795a0a9cdd8caa06d651519e28bc
			if (Application.platform != RuntimePlatform.WindowsPlayer || MODSystem.instance.IsWine)
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

			Debug.Log($"ModPlayMovie Error: No video file with extension [{string.Join("|", extensionList)}] found at [{pathNoExtension}]");

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
						if(GetFullVideoPath(movieInfo.PathNoExtension, out string pathWithExtension))
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
