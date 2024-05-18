using Assets.Scripts.Core;
using Steamworks;
using System.IO;
using UMP;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{	
	public class MediaListener : IMediaListener
	{
		UMPRenderer renderer;
		MediaPlayer standalone;
		public MediaListener(UMPRenderer renderer, MediaPlayer standalone)
		{
			this.standalone = standalone;
			this.renderer = renderer;
		}

		public void OnPlayerBuffering(float percentage)
		{
		}

		public void OnPlayerEncounteredError()
		{
		}

		public void OnPlayerEndReached()
		{
			this.renderer.Quit();
		}

		public void OnPlayerImageReady(Texture2D videoTexture)
		{
		}

		public void OnPlayerOpening()
		{
		}

		public void OnPlayerPaused()
		{
		}

		public void OnPlayerPlaying()
		{
		}

		public void OnPlayerPrepared(int videoWidth, int videoHeight)
		{
			int numSubtitleTracks = standalone.SpuTracks.Length;

			Debug.Log($"Detected {numSubtitleTracks} subtitle tracks");

			for(int i = 0; i < numSubtitleTracks; i++)
			{
				MediaTrackInfo info = standalone.SpuTracks[i];
				standalone.SpuTrack = info;
				Debug.Log($"MediaTrackInfo {i}: {info}");
			}

			Debug.Log($"Using MediaTrackInfo: {standalone.SpuTrack}");
		}

		public void OnPlayerStopped()
		{
		}
	}

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

		MediaPlayer standalone;
		//UniversalMediaPlayer ump_player;

		//float dummyTime = 0;
		//private void Update()
		//{

		//	dummyTime += Time.deltaTime;


		//	Debug.Log($"Update Running... {dummyTime}");
		//	if (standalone != null)
		//	{
		//		if(dummyTime > 2.0f)
		//		{
		//			Debug.Log("Stopping video");
		//			standalone.Stop();
		//		}
		//	}
		//}

		// Stop video playback immediately when user tries to quit game
		// Need to prevent game freezing on quit
		void OnApplicationQuit()
		{
			Quit();
		}

		// Called externally when user clicks to skip video,
		// or from callback when video finishes, see MediaListener above.
		public void Quit()
		{
			Debug.Log($"Quitting UMP Playback...");

			if(standalone != null)
			{
				// Stop and release the video player
				standalone.Stop();
				//standalone.Release();
			}

			//if(ump_player != null)
			//{
			//	ump_player.Stop();
			//	//ump_player.Release();
			//	//Object.Destroy(ump_player);
			//}

			base.enabled = false;
		}

		public void Init(MovieInfo movieInfo)
		{
			string subtitlePath = Path.ChangeExtension(movieInfo.PathWithExt, ".ass");

			Debug.Log($"Playing movie using UMP at path {movieInfo.PathWithExt} with subtitle {subtitlePath}");


			// Add the UMP component. According to the UMP PDF you normally do this in editor
			//UniversalMediaPlayer ump_player = gameObject.AddComponent<UniversalMediaPlayer>();

			try
			{
				PlayerOptionsStandalone player_options = new PlayerOptionsStandalone(new string[] {
					//$@"--sub-file={subtitlePath}", // autodetect should find sub file so this should be unnnecessray
					//$@"--sub-track=1",
				})
				{
					//LogDetail = LogLevels.Debug,
					// MUST disable hardware decoding for subtitles to render!
					HardwareDecoding = PlayerOptionsStandalone.States.Disable,
				};

				standalone = new MediaPlayer(this, new GameObject[] { movieInfo.Layer.gameObject }, player_options);

				// autodetect should find sub file so this should be unnnecessray
				//standalone.SetSubtitleFile(new System.Uri(subtitlePath));

				// Embedded Subtitles only valid after video prepared
				//ump_player.AddPreparedEvent(OnPlayerPrepared);
				standalone.AddMediaListener(new MediaListener(this, standalone));

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
