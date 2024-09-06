using Assets.Scripts.Core;
using Assets.Scripts.Core.State;
using MOD.Scripts.Core.Movie;
using MOD.Scripts.UI;
using System.IO;
using UnityEngine;

namespace MOD.Scripts.Core.State
{
	public class StateMovie : IGameState
	{
		private bool isLeaving;

		private IMovieRenderer MovieEntity;

		private GameObject gameObject;

		private MovieInfo movieInfo;

		public StateMovie(string moviename)
		{
			// TODO: retest if new Proton supports AVProMovieRenderer and also check native video playback on Ch9 & 10
			// Only use AVProMovieRenderer on "Real" Windows
			// - It is entirely unsupported on Linux Desktop
			// - It says it has some support on MacOS, but only macOS 10.13 and above, 64bit only, Metal only. For now we assume it is not supported.
			// - It doesn't work on Wine/Proton
			// - See https://www.renderheads.com/content/docs/AVProVideo/articles/requirements.html, but note that we use an extremely old version of AVProVideo
			bool windowsPlaybackMode = Application.platform == RuntimePlatform.WindowsPlayer && !MODSystem.instance.IsWine;

			// Determine the playback extension, either ".mp4" (Windows) or ".ogv" (Linux, Mac, Wine)
			string movieExtension = windowsPlaybackMode ? ".mp4" : ".ogv";

			// Show error message if movie is missing
			if(!File.Exists(MovieInfo.GetPathFromNameWithExt(moviename, movieExtension)))
			{
				string errorMessage = $"ERROR: Movie file {moviename}{movieExtension} not found";
				Debug.Log(errorMessage);
				MODToaster.Show(errorMessage, toastDuration:10);
			}

			// Attempt to play the movie, even if it wasn't found
			Debug.Log($"[StateMovie] Begin Movie Playback using {(windowsPlaybackMode ? "AVProMovieRenderer" : "Unity TextureMovieRenderer")} of file {moviename}{movieExtension} OS: {SystemInfo.operatingSystem} Wine: {MODSystem.instance.IsWine}");
			movieInfo = new MovieInfo(moviename, movieExtension);
			gameObject = new GameObject();
			SetupBackgroundLayerForVideo();

			if (windowsPlaybackMode)
			{
				MovieEntity = gameObject.AddComponent<AVProMovieRenderer>();
			}
			else
			{
				MovieEntity = gameObject.AddComponent<TextureMovieRenderer>();
			}
			MovieEntity.Init(movieInfo);
		}

		public void OnRestoreState()
		{
		}

		public GameState GetStateType()
		{
			return GameState.Movie;
		}

		public bool InputHandler()
		{
			if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetMouseButtonDown(0))
			{
				RequestLeave();
			}
			return false;
		}

		public void OnLeaveState()
		{
			Leave();
			Object.Destroy(gameObject);
		}

		public void RequestLeave()
		{
			if (!isLeaving)
			{
				Leave();
				GameSystem.Instance.PopStateStack();
			}
		}

		public void RequestLeaveImmediate()
		{
			Debug.LogWarning("StateMovie leave immediate - untested");
			GameSystem.Instance.PopStateStack();
		}

		public void Leave()
		{
			if (!isLeaving)
			{
				isLeaving = true;
				if (MovieEntity != null)
				{
					MovieEntity.Quit();
				}
				movieInfo.Layer.Initialize();
			}
		}

		private void SetupBackgroundLayerForVideo()
		{
			movieInfo.Layer.ReleaseTextures();
			movieInfo.Layer.DrawLayer("black", 0, 0, 0, null, 1f, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
		}
	}
}
