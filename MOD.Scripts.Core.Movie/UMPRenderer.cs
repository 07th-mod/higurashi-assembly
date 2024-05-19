using Assets.Scripts.Core;
using MOD.Scripts.UI;
using System.IO;
using System.Text;
using UMP;
using UnityEngine;

namespace MOD.Scripts.Core.Movie
{	
	public class MediaListener : IMediaListener
	{
		UMPRenderer renderer;
		MediaPlayer mediaPlayer;
		public MediaListener(UMPRenderer renderer, MediaPlayer standalone)
		{
			this.mediaPlayer = standalone;
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
			renderer.LeaveAndQuit();
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
			int numSubtitleTracks = mediaPlayer.SpuTracks.Length;

			StringBuilder sb = new StringBuilder();

			sb.AppendLine($">> Detected {numSubtitleTracks} subtitle tracks: ");

			for (int i = 0; i < numSubtitleTracks; i++)
			{
				MediaTrackInfo info = mediaPlayer.SpuTracks[i];
				sb.AppendLine($" - Subtitle [{i}]: {info}");
			}

			sb.AppendLine($">> Will Use Subtitle: {mediaPlayer.SpuTrack}");
		}

		public void OnPlayerStopped()
		{
		}
	}

	public class UMPRenderer : MonoBehaviour, IMovieRenderer
	{
		static MediaPlayer mediaPlayer;

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

			if(mediaPlayer != null)
			{
				// Stop the media player
				mediaPlayer.Stop();

				// TODO: Release causes the game to lag (while it unloads the VLC libs etc.?)
				// Could just keep mediaPlayer in memory as each time a video is played, this will introduce lag
				// Or perhaps Unity will unload automatically?
				// mediaPlayer.Release();
			}

			base.enabled = false;
		}

		public void LeaveAndQuit()
		{
			Quit();
			GameSystem.Instance.PopStateStack();
		}

		public static void InitMediaPlayerGameObject(MonoBehaviour gameSystemMono)
		{
			if (mediaPlayer != null)
			{
				return;
			}

			try
			{
				Debug.Log("Initializing Universal Media Player game object...");

				// Pass in any raw vlc arguments here. See https://wiki.videolan.org/VLC_command-line_help/
				// Please note the VLC library used in this player may be very old, and some arguments may not work.
				string[] rawVLCArguments = new string[] {
					//$@"--sub-file=example.srt", // autodetect should find sub file so this should be unnnecessray
					//$@"--sub-track=1",
				};

				PlayerOptionsStandalone player_options = new PlayerOptionsStandalone(rawVLCArguments)
				{
					// MUST disable hardware decoding for subtitles to render!
					HardwareDecoding = PlayerOptionsStandalone.States.Disable,

					// Other possible options listed here:
					//LogDetail = LogLevels.Disable;
					//FlipVertically = true;
					//FileCaching = 300;
					//LiveCaching = 300;
					//DiskCaching = 300;
					//NetworkCaching = 300;
					//CrAverage = 40;
					//ClockSynchro = States.Default;
					//ClockJitter = 5000;
				};

				mediaPlayer = new MediaPlayer(gameSystemMono, new GameObject[]{}, player_options);
			}
			catch (System.Exception ex)
			{
				Debug.Log($"Exception: {ex}");
				MODToaster.Show($"Failed to init Universal Media Player", toastDuration: 8);
				return;
			}
		}

		public void Init(MovieInfo movieInfo)
		{
			Debug.Log($"Playing movie using UMP at path {movieInfo.PathWithExt}");

			try
			{
				if (mediaPlayer == null)
				{
					InitMediaPlayerGameObject(GameSystem.Instance);
				}

				// This sets up the video playback on the background layer by setting Layer's "_Primary" texture
				// See MediaPlayerHelper.ApplyTextureToRenderingObjects() for details
				mediaPlayer.VideoOutputObjects = new GameObject[] { movieInfo.Layer.gameObject };

				// Optional - don't use subtitle autodetect and manually specify subtitle path
				// Autodetect should find sub file so this should be unnnecessray
				// string subtitlePath = Path.ChangeExtension(movieInfo.PathWithExt, ".ass");
				// standalone.SetSubtitleFile(new System.Uri(subtitlePath));

				// Embedded Subtitles only valid after video prepared
				mediaPlayer.AddMediaListener(new MediaListener(this, mediaPlayer));

				// Specify path of file to play with UMP
				mediaPlayer.DataSource = movieInfo.PathWithExt;

				// Skip video forward for testing purposes - time in milliseconds
				// mediaPlayer.Time = 135 * 1000;

				// Begin video playback
				mediaPlayer.Play();
			}
			catch (System.Exception ex)
			{
				Debug.Log($"Exception: {ex}");
				MODToaster.Show($"Playback of movie {movieInfo.PathWithExt} failed", toastDuration: 8);
				LeaveAndQuit();
				return;
			}
		}
	}
}
