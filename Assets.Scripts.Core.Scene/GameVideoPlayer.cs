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

		public void Initialize(string path, Vector2Int size)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			material = new Material(Shader.Find("MGShader/LayerShader"));
			meshRenderer.material = material;
			meshFilter.mesh = MGHelper.CreateMesh(640, 480, LayerAlignment.AlignCenter);
			renderTexture = new RenderTexture(size.x, size.y, 32);
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