using Assets.Scripts.Core;
using Assets.Scripts.Core.State;
using MOD.Scripts.Core.Movie;
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
			movieInfo = new MovieInfo(moviename);
			gameObject = new GameObject();
			SetupBackgroundLayerForVideo();
			MovieEntity = CreateMovieRenderer();
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

		private IMovieRenderer CreateMovieRenderer()
		{
			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				return gameObject.AddComponent<AVProMovieRenderer>();
			}

			throw new System.Exception("Video playback not support on linux/osx - tell the devleopers to update the mod!");
			//return gameObject.AddComponent<TextureMovieRenderer>();
		}

		private void SetupBackgroundLayerForVideo()
		{
			movieInfo.Layer.ReleaseTextures();
			movieInfo.Layer.DrawLayer("black", 0, 0, 0, null, null, 1f, /*isBustshot:*/ false, 0, 0f, /*isBlocking:*/ false);
		}
	}
}
