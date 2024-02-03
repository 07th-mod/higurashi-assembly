using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts.Core.Scene
{
	public class GameVideoPlayer : MonoBehaviour
	{
		private VideoPlayer player;

		private bool isActive;

		public static GameVideoPlayer CreateVideoPlayer(GameObject parent, string videoPath, Vector2Int size)
		{
			string videoClipPath = AssetManager.Instance.GetVideoClipPath(videoPath);
			GameObject gameObject = new GameObject("VideoPlayer");
			gameObject.transform.SetParent(parent.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = new Vector3(0f, 0f, -5f);
			GameVideoPlayer gameVideoPlayer = gameObject.AddComponent<GameVideoPlayer>();
			gameVideoPlayer.Initialize(videoClipPath, size);
			return gameVideoPlayer;
		}

		// Note: In the vanilla game, the PlayVideo(path, video_size_x, video_size_y) takes two extra arguments (which I think is the resolution the video should play at)
		// However after mod changes, these are no longer used, and can be set to 0.
		// Note: Previously the video playback logic was much more complicated, but I replaced the Unity example code (more or less) and it seems to work.
		// See https://docs.unity3d.com/ScriptReference/Video.VideoPlayer.html
		public void Initialize(string path, Vector2Int size)
		{
			// Note: Unity recommends using Camera.main instead of Camera.current, however I could only get this to work with Camera.current
			GameObject camera = Camera.current.gameObject;

			player = camera.AddComponent<VideoPlayer>();
			player.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
			player.url = path;
			player.isLooping = false;
			player.aspectRatio = VideoAspectRatio.FitInside;
			player.SetDirectAudioVolume(0, GameSystem.Instance.AudioController.GetVolumeByType(Assets.Scripts.Core.Audio.AudioType.BGM) * (25f / 32f));
			player.loopPointReached += OnFinish;

			GameSystem.Instance.AddWait(new Wait(0f, WaitTypes.WaitForInput, EndPlayback));
			isActive = true;
		}

		public void EndPlayback()
		{
			isActive = false;
			// In the vanilla game, the video player ('player') was attached to the base game object, so it would be destroyed automatically
			// However now that the video player is attached to the camera, we need to delete it manually
			Object.Destroy(player);
			Object.Destroy(base.gameObject);
		}

		public void OnFinish(VideoPlayer vp)
		{
			if (isActive)
			{
				isActive = false;
				GameSystem.Instance.ClearWait();
				Object.Destroy(base.gameObject);
			}
		}
	}
}
