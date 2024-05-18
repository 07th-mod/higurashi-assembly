using Assets.Scripts.Core;
using Assets.Scripts.Core.State;
using MOD.Scripts.Core.Movie;
using MOD.Scripts.UI;
using System.Collections.Generic;
using System.IO;
using UMP;
using UnityEngine;

namespace MOD.Scripts.Core.State
{
	public class StateMovie : IGameState
	{
		const string windowsMovieExtension = ".mp4";
		const string linuxMovieExtension = ".ogv";

		private bool isLeaving;

		private IMovieRenderer MovieEntity;

		private GameObject gameObject;

		private MovieInfo movieInfo;

		public StateMovie(string moviename)
		{
			// Windows and Wine/Proton will both show up as WindowsPlayer
			bool isWindowsOrWine = Application.platform == RuntimePlatform.WindowsPlayer;

			// Only play the .mp4 file with AVProVideo on Windows-like Platforms, if the file exists
			// On Wine, it is expected that only the Linux .ogv video file is installed, as playing using AVProVideo/.mp4 files on Wine is not supported
			bool windowsPlaybackMode = isWindowsOrWine && File.Exists(MovieInfo.GetPathFromNameWithExt(moviename, windowsMovieExtension));

			// The below just shows a message if no video file found to play (it does not affect playback behavior)
			if (!windowsPlaybackMode && !File.Exists(MovieInfo.GetPathFromNameWithExt(moviename, linuxMovieExtension)))
			{
				string movieFiles = $"{moviename}{linuxMovieExtension}";
				if(isWindowsOrWine)
				{
					movieFiles = $"{moviename}{windowsMovieExtension} or " + movieFiles;
				}

				string errorMessage = $"ERROR: Movie file {movieFiles} not found";
				Debug.Log(errorMessage);
				MODToaster.Show(errorMessage, toastDuration:10);
			}

			movieInfo = new MovieInfo(moviename, windowsPlaybackMode ? windowsMovieExtension : linuxMovieExtension);
			gameObject = new GameObject();
			SetupBackgroundLayerForVideo();

			//if (windowsPlaybackMode)
			//{
			//	MovieEntity = gameObject.AddComponent<AVProMovieRenderer>();
			//}
			//else
			//{
			//	MovieEntity = gameObject.AddComponent<TextureMovieRenderer>();
			//}

			MovieEntity = gameObject.AddComponent<UMPRenderer>();

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
