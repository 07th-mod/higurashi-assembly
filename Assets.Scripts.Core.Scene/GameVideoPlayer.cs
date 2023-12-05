using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using UnityEngine;
using UnityEngine.Video;

namespace Assets.Scripts.Core.Scene
{
	public class GameVideoPlayer : MonoBehaviour
	{
		private VideoPlayer player;

		private Mesh mesh;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private Material material;

		private RenderTexture renderTexture;

		private const string shaderDefaultName = "MGShader/LayerShader";

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

		private int RoundNearest2(float value)
		{
			return Mathf.RoundToInt(Mathf.Round(value / 2f) * 2f);
		}

		private Vector2Int CalculateMeshSize(Vector2Int videoResolution)
		{
			// Calculate screen width/heigh in same coordinate system as expected by Initialize(...)
			// 4:3 full screen is 640x480
			// 16:9 full screen is 854x480
			float screen_aspect = GameSystem.Instance.AspectRatio;
			float screen_width = RoundNearest2(screen_aspect * 480);
			float screen_height = 480;

			// Calculate video aspect. We don't care about actual video resolution
			// (hopefully that is handled by the video player automatically...)
			float video_width = videoResolution.x;
			float video_height = videoResolution.y;
			float video_aspect = video_width / video_height;

			// These default values are currently never used,
			// but would stretch video to screen regardless of aspect
			float mesh_width = screen_width;
			float mesh_height = screen_height;

			// Compare the video aspect vs screen aspect to determine what kind of
			// Letterboxing should be performed
			if (video_aspect > screen_aspect)
			{
				// Horizontal Letterboxing
				mesh_width = screen_width;
				mesh_height = screen_width / video_aspect;
			}
			else
			{
				// Vertical Letterboxing
				mesh_width = screen_height * video_aspect;
				mesh_height = screen_height;
			}

			// Make sure returned mesh size is a multiple of 2 (just to be safe)
			return new Vector2Int(RoundNearest2(mesh_width), RoundNearest2(mesh_height));
		}

		public void Initialize(string path, Vector2Int size)
		{
			// NOTE: the video will be drawn ontop of whatever is already on the screen
			// so you should add a DrawScene( "black", 500 ); just before the movie is played.
			Vector2Int meshSize = CalculateMeshSize(size);

			meshFilter = base.gameObject.AddComponent<MeshFilter>();
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			material = new Material(Shader.Find("MGShader/LayerShader"));
			meshRenderer.material = material;
			meshFilter.mesh = MGHelper.CreateMesh(meshSize.x, meshSize.y, LayerAlignment.AlignCenter, false, 0);
			renderTexture = new RenderTexture(meshSize.x, meshSize.y, 32);
			material.SetTexture("_Primary", renderTexture);
			player = base.gameObject.AddComponent<VideoPlayer>();
			player.renderMode = VideoRenderMode.RenderTexture;
			player.targetTexture = renderTexture;
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
