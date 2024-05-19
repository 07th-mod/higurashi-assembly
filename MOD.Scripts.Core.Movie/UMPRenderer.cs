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
		int subtitleTrack;
		int audioTrack;
		public MediaListener(UMPRenderer renderer, MediaPlayer standalone, int subtitleTrack, int audioTrack)
		{
			this.mediaPlayer = standalone;
			this.renderer = renderer;
			this.subtitleTrack = subtitleTrack;
			this.audioTrack = audioTrack;
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

		private void PrintMediaInfo(MediaTrackInfo[] tracks, string trackTypeDescription)
		{
			int numTracks = tracks.Length;
			StringBuilder sb = new StringBuilder();

			sb.AppendLine($">> Detected {numTracks} {trackTypeDescription} tracks: ");

			for (int i = 0; i < numTracks; i++)
			{
				MediaTrackInfo info = tracks[i];
				sb.AppendLine($" - {trackTypeDescription} [{i}]: {info}");
			}

			sb.AppendLine($">> {trackTypeDescription} track currently set to: {mediaPlayer.SpuTrack}");
			Debug.Log(sb);
		}

		// UMP:
		// Track 0 = Disable
		// Track 1 = First track

		// Our mod:
		// -1 = Disabled
		// 0 = Default Track
		// 1 = First Track
		private bool GetUMPTrack(int modTrackNumber, MediaTrackInfo[] possibleTracks, string description, out MediaTrackInfo trackToSet)
		{
			// In the mod, 0 means 'use default value', so don't change the track at all
			if(modTrackNumber == 0)
			{
				trackToSet = null;
				return false;
			}

			int umpTrackNumber = modTrackNumber;

			// In the mod, -1 means 'disable', which means we need to set the value to 0
			if (modTrackNumber == 0)
			{
				umpTrackNumber = 0;
			}

			// Other track value are the same, but make sure it is in bounds before trying to set it
			if(possibleTracks.Length > umpTrackNumber)
			{
				trackToSet = possibleTracks[umpTrackNumber];
				return true;
			}

			Debug.Log($"Warning: {description} track index {umpTrackNumber} is out of range (There are only {possibleTracks.Length} tracks). Will use default track.");
			trackToSet = null;
			return false;
		}

		public void OnPlayerPrepared(int videoWidth, int videoHeight)
		{
			// Set subtitle track
			if(GetUMPTrack(subtitleTrack, mediaPlayer.SpuTracks, "Subtitle", out MediaTrackInfo subtitleToSet))
			{
				mediaPlayer.SpuTrack = subtitleToSet;
			}

			// Set audio track
			if(GetUMPTrack(audioTrack, mediaPlayer.AudioTracks, "Audio", out MediaTrackInfo audioToSet))
			{
				mediaPlayer.AudioTrack = audioToSet;
			}

			PrintMediaInfo(mediaPlayer.SpuTracks, "Subtitle");
			PrintMediaInfo(mediaPlayer.AudioTracks, "Audio");
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

		// For now, you can't set options at runtime
		private static PlayerOptionsStandalone GetUMPPlayerOptions()
		{
			// Pass in any raw vlc arguments here. See https://wiki.videolan.org/VLC_command-line_help/
			// Please note the VLC library used in this player may be very old, and some arguments may not work.
			string[] rawVLCArguments = new string[] {
					//$@"--sub-file=example.srt", // autodetect should find sub file so this should be unnnecessray
					//$@"--sub-track=1",
				};

			return new PlayerOptionsStandalone(rawVLCArguments)
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
				mediaPlayer = new MediaPlayer(gameSystemMono, new GameObject[]{}, GetUMPPlayerOptions());
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
				mediaPlayer.AddMediaListener(new MediaListener(this, mediaPlayer, movieInfo.subtitleTrack, movieInfo.audioTrack));

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
