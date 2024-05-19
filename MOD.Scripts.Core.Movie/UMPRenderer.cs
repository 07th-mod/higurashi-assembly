using Assets.Scripts.Core;
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
			this.renderer.Quit();

			// TODO: while this is how auto-exit is done on the other renderers,
			// there could actually be a case where state is exited twice here
			// (once by video ending and once by user clicking on same frame)
			GameSystem.Instance.PopStateStack();
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
		MediaPlayer mediaPlayer;

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
				// Could just keep mediaPlayer in memory as each time a video is played, this will introduce lag.
				// mediaPlayer.Release();
			}

			base.enabled = false;
		}

		public void Init(MovieInfo movieInfo)
		{

			Debug.Log($"Playing movie using UMP at path {movieInfo.PathWithExt}");

			try
			{
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

				mediaPlayer = new MediaPlayer(this, new GameObject[] { movieInfo.Layer.gameObject }, player_options);

				// Optional - don't use subtitle autodetect and manually specify subtitle path
				// Autodetect should find sub file so this should be unnnecessray
				// string subtitlePath = Path.ChangeExtension(movieInfo.PathWithExt, ".ass");
				// standalone.SetSubtitleFile(new System.Uri(subtitlePath));

				// Embedded Subtitles only valid after video prepared
				mediaPlayer.AddMediaListener(new MediaListener(this, mediaPlayer));

				// Specify path of file to play with UMP
				mediaPlayer.DataSource = movieInfo.PathWithExt;

				// Begin video playback
				mediaPlayer.Play();
			}
			catch (System.Exception ex)
			{
				Debug.Log($"Exception: {ex}");
				return;
			}
		}
	}
}
