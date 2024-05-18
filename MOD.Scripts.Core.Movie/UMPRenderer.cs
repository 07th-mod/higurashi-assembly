using Assets.Scripts.Core;
using RenderHeads.Media.AVProVideo;
using UMP;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{
	public class UMPRenderer : MonoBehaviour, IMovieRenderer
	{
		//public MeshRenderer Renderer;

		//public bool isStarted;

		//public void OnAvProVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
		//{
		//	if (base.enabled)
		//	{
		//		if (errorCode != 0)
		//		{
		//			Debug.LogError("Encounted video error, stopping video playback.");
		//			GameSystem.Instance.PopStateStack();
		//		}
		//		else
		//		{
		//			switch (et)
		//			{
		//			case MediaPlayerEvent.EventType.FirstFrameReady:
		//				Renderer.enabled = true;
		//				isStarted = true;
		//				break;
		//			case MediaPlayerEvent.EventType.FinishedPlaying:
		//				Quit();
		//				GameSystem.Instance.PopStateStack();
		//				break;
		//			}
		//		}
		//	}
		//}

		public void Quit()
		{
			Debug.Log($"Quitting");

			//MediaPlayer component = GetComponent<MediaPlayer>();
			//if (component != null)
			//{
			//	component.CloseVideo();
			//}
			//if (Renderer != null)
			//{
			//	Renderer.enabled = false;
			//}
			//base.enabled = false;
		}

		public void Init(MovieInfo movieInfo)
		{

			Debug.Log($"Playing movie using UMP at path {movieInfo.PathWithExt}");

			// Add the UMP component. According to the UMP PDF you normally do this in editor
			UniversalMediaPlayer ump_player = gameObject.AddComponent<UniversalMediaPlayer>();

			try
			{
				PlayerOptionsStandalone player_options = new PlayerOptionsStandalone(new string[0]);
				MediaPlayerStandalone standalone = new MediaPlayerStandalone(ump_player, new GameObject[] { movieInfo.Layer.gameObject }, player_options);


				// Specify path of file to play with UMP
				//ump_player.Path = movieInfo.PathWithExt;
				standalone.DataSource = movieInfo.PathWithExt;


				// Tell UMP to render to the background layer
				/// From the UMP code: "Get/Set simple array that consist with Unity 'GameObject' that have 'Mesh Renderer' (with some material) or 'Raw Image' component"
				//ump_player.RenderingObjects = new GameObject[] { movieInfo.Layer.gameObject };

				//
				//ump_player.Play();
				standalone.Play();
			}
			catch (System.Exception ex)
			{
				Debug.Log($"Exception: {ex}");
				return;
			}


			//MediaPlayer mediaPlayer = base.gameObject.AddComponent<MediaPlayer>();
			//mediaPlayer.Events.AddListener(OnAvProVideoEvent);
			//mediaPlayer.m_AutoOpen = true;
			//mediaPlayer.m_AutoStart = true;
			//mediaPlayer.m_Volume = movieInfo.Volume;
			//mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, movieInfo.PathWithExt);
			//MODApplyToMaterial mODApplyToMaterial = base.gameObject.AddComponent<MODApplyToMaterial>();
			//mODApplyToMaterial._material = movieInfo.Layer.MODMaterial;
			//mODApplyToMaterial._texturePropertyName = "_Primary";
			//mODApplyToMaterial._media = mediaPlayer;
			//Renderer = movieInfo.Layer.MODMeshRenderer;
			//Renderer.enabled = false;
			//base.gameObject.AddComponent<AudioOutput>().ChangeMediaPlayer(mediaPlayer);
		}
	}
}
