using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UMP.Wrappers
{
	internal class WrapperStandalone : IWrapperAudio, IWrapperNative, IWrapperPlayer, IWrapperSpu
	{
		private delegate void ManageLogCallback(string msg);

		[NativeFunction("UMPNativeInit")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UMPNativeInitDel();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeUpdateIndex")]
		private delegate void UMPNativeUpdateIndexDel(int index);

		[NativeFunction("UMPNativeSetTexture")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void UMPNativeSetTextureDel(int index, IntPtr texture);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeSetPixelsBuffer")]
		private delegate void UMPNativeSetPixelsBufferDel(int index, IntPtr buffer, int width, int height);

		[NativeFunction("UMPNativeGetPixeslBuffer")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetPixelsBufferDel(int index);

		[NativeFunction("UMPNativePixelsBufferRelease")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void UMPNativePixelsBufferReleaseDel(int index);

		[NativeFunction("UMPNativeGetLogMessage")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetLogMessageDel(int index);

		[NativeFunction("UMPNativeGetLogLevel")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UMPNativeGetLogLevelDel(int index);

		[NativeFunction("UMPNativeGetState")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UMPNativeGetStateDel(int index);

		[NativeFunction("UMPNativeGetStateFloatValue")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate float UMPNativeGetStateFloatValueDel(int index);

		[NativeFunction("UMPNativeGetStateLongValue")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate long UMPNativeGetStateLongValueDel(int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeGetStateStringValue")]
		private delegate IntPtr UMPNativeGetStateStringValueDel(int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeSetPixelsVerticalFlip")]
		private delegate void UMPNativeSetPixelsVerticalFlipDel(int index, bool flip);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeSetAudioParams")]
		private delegate void UMPNativeSetAudioParamsDel(int index, int numChannels, int sampleRate);

		[NativeFunction("UMPNativeGetAudioParams")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetAudioParamsDel(int index, char separator);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeGetAudioChannels")]
		private delegate int UMPNativeGetAudioChannelsDel(int index);

		[NativeFunction("UMPNativeGetAudioSamples")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UMPNativeGetAudioSamplesDel(int index, IntPtr decodedSamples, int samplesLength, AudioOutput.AudioChannels audioChannel);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeClearAudioSamples")]
		private delegate bool UMPNativeClearAudioSamplesDel(int index, int samplesLength);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeDirectRender")]
		private delegate void UMPNativeDirectRenderDel(int index);

		[NativeFunction("UMPNativeGetUnityRenderCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetUnityRenderCallbackDel();

		[NativeFunction("UMPNativeGetVideoLockCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetVideoLockCallbackDel();

		[NativeFunction("UMPNativeGetVideoDisplayCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetVideoDisplayCallbackDel();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeGetVideoFormatCallback")]
		private delegate IntPtr UMPNativeGetVideoFormatCallbackDel();

		[NativeFunction("UMPNativeGetVideoCleanupCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetVideoCleanupCallbackDel();

		[NativeFunction("UMPNativeGetAudioSetupCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetAudioSetupCallbackDel();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeGetAudioPlayCallback")]
		private delegate IntPtr UMPNativeGetAudioPlayCallbackDel();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeMediaPlayerEventCallback")]
		private delegate IntPtr UMPNativeMediaPlayerEventCallbackDel();

		[NativeFunction("UMPNativeSetBufferSizeCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void UMPNativeSetBufferSizeCallbackDel(int index, IntPtr callback);

		[NativeFunction("UMPNativeGetLogMessageCallback")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr UMPNativeGetLogMessageCallbackDel();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeSetUnityLogMessageCallback")]
		private delegate void UMPNativeSetUnityLogMessageCallbackDel(IntPtr callback);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("UMPNativeUpdateFramesCounter")]
		private delegate void UMPNativeUpdateFramesCounterCallbackDel(int index, int counter);

		[NativeFunction("UMPNativeGetFramesCounter")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int UMPNativeGetFramesCounterCallbackDel(int index);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_set_aspect_ratio")]
		private delegate void SetVideoAspectRatio(IntPtr playerObject, string cropGeometry);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_log_unset")]
		private delegate void LogUnset(IntPtr instance);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_log_set")]
		private delegate void LogSet(IntPtr instance, IntPtr callback, IntPtr data);

		[NativeFunction("libvlc_new")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr CreateNewInstance(int argc, string[] argv);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_release")]
		private delegate void ReleaseInstance(IntPtr instance);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_free")]
		private delegate void Free(IntPtr ptr);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_callback_t")]
		private delegate void EventCallback(IntPtr args);

		[NativeFunction("libvlc_event_attach")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int AttachEvent(IntPtr eventManagerInstance, EventTypes eventType, IntPtr callback, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_event_detach")]
		public delegate int DetachEvent(IntPtr eventManagerInstance, EventTypes eventType, IntPtr callback, IntPtr userData);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_new_location")]
		private delegate IntPtr CreateNewMediaFromLocation(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string path);

		[NativeFunction("libvlc_media_new_path")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr CreateNewMediaFromPath(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string path);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_add_option")]
		private delegate void AddOptionToMedia(IntPtr mediaInstance, [MarshalAs(UnmanagedType.LPStr)] string option);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_release")]
		private delegate void ReleaseMedia(IntPtr mediaInstance);

		[NativeFunction("libvlc_media_get_mrl")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr GetMediaMrl(IntPtr mediaInstance);

		[NativeFunction("libvlc_media_get_parsed_status")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate ParsedStatus GetMediaParsedStatus(IntPtr mediaInstance);

		[NativeFunction("libvlc_media_get_meta")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr GetMediaMetadata(IntPtr mediaInstance, MediaMetadatas meta);

		[NativeFunction("libvlc_media_get_stats")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int GetMediaStats(IntPtr mediaInstance, out MediaStats statsInformationsPointer);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_parse")]
		private delegate void ParseMedia(IntPtr mediaInstance);

		[NativeFunction("libvlc_media_parse_async")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void ParseMediaAsync(IntPtr mediaInstance);

		[NativeFunction("libvlc_media_is_parsed")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int IsParsedMedia(IntPtr mediaInstance);

		[NativeFunction("libvlc_media_get_tracks_info")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int GetMediaTracksInformations(IntPtr mediaInstance, out IntPtr tracksInformationsPointer);

		[NativeFunction("libvlc_media_player_new")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr CreateMediaPlayer(IntPtr instance);

		[NativeFunction("libvlc_media_player_release")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void ReleaseMediaPlayer(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_set_media")]
		private delegate void SetMediaToMediaPlayer(IntPtr playerObject, IntPtr mediaInstance);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_event_manager")]
		private delegate IntPtr GetMediaPlayerEventManager(IntPtr playerObject);

		[NativeFunction("libvlc_media_player_is_playing")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int IsPlaying(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_play")]
		private delegate int Play(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_pause")]
		private delegate void Pause(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_stop")]
		private delegate void Stop(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_set_callbacks")]
		private delegate void SetVideoCallbacks(IntPtr playerObject, IntPtr @lock, IntPtr unlock, IntPtr display, IntPtr opaque);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_set_format")]
		private delegate void SetVideoFormat(IntPtr playerObject, [MarshalAs(UnmanagedType.LPStr)] string chroma, uint width, uint height, uint pitch);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_set_format_callbacks")]
		private delegate void SetVideoFormatCallbacks(IntPtr playerObject, IntPtr setup, IntPtr cleanup);

		[NativeFunction("libvlc_media_player_get_length")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate long GetLength(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_get_time")]
		private delegate long GetTime(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_set_time")]
		private delegate void SetTime(IntPtr playerObject, long time);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_get_position")]
		private delegate float GetMediaPosition(IntPtr playerObject);

		[NativeFunction("libvlc_media_player_set_position")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetMediaPosition(IntPtr playerObject, float position);

		[NativeFunction("libvlc_media_player_will_play")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int CouldPlay(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_media_player_get_rate")]
		private delegate float GetRate(IntPtr playerObject);

		[NativeFunction("libvlc_media_player_set_rate")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int SetRate(IntPtr playerObject, float rate);

		[NativeFunction("libvlc_media_player_get_state")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate MediaStates GetMediaPlayerState(IntPtr playerObject);

		[NativeFunction("libvlc_media_player_get_fps")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate float GetFramesPerSecond(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_get_size")]
		private delegate int GetVideoSize(IntPtr playerObject, uint num, out uint px, out uint py);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_get_scale")]
		private delegate float GetVideoScale(IntPtr playerObject);

		[NativeFunction("libvlc_video_set_scale")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate float SetVideoScale(IntPtr playerObject, float f_factor);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_take_snapshot")]
		private delegate int TakeSnapshot(IntPtr playerObject, uint num, string fileName, uint width, uint height);

		[NativeFunction("libvlc_video_get_spu_count")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int GetVideoSpuCount(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_get_spu_description")]
		private delegate IntPtr GetVideoSpuDescription(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_get_spu")]
		private delegate int GetVideoSpu(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_video_set_spu")]
		private delegate int SetVideoSpu(IntPtr playerObject, int spu);

		[NativeFunction("libvlc_video_set_subtitle_file")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int SetVideoSubtitleFile(IntPtr playerObject, [MarshalAs(UnmanagedType.LPStr)] string path);

		[NativeFunction("libvlc_audio_set_format")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void SetAudioFormat(IntPtr playerObject, [MarshalAs(UnmanagedType.LPStr)] string format, int rate, int channels);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_set_callbacks")]
		private delegate void SetAudioCallbacks(IntPtr playerObject, IntPtr play, IntPtr pause, IntPtr resume, IntPtr flush, IntPtr drain, IntPtr opaque);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_set_format_callbacks")]
		private delegate void SetAudioFormatCallbacks(IntPtr playerObject, IntPtr setup, IntPtr cleanup);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_get_track_count")]
		private delegate int GetAudioTracksCount(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_get_track_description")]
		private delegate IntPtr GetAudioTracksDescriptions(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_track_description_release")]
		private delegate IntPtr ReleaseTracksDescriptions(IntPtr trackDescription);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_get_track")]
		private delegate int GetAudioTrack(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_set_track")]
		private delegate int SetAudioTrack(IntPtr playerObject, int trackId);

		[NativeFunction("libvlc_audio_get_delay")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate long GetAudioDelay(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_set_delay")]
		private delegate void SetAudioDelay(IntPtr playerObject, long channel);

		[NativeFunction("libvlc_audio_get_volume")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int GetVolume(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_set_volume")]
		private delegate int SetVolume(IntPtr playerObject, int volume);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_set_mute")]
		private delegate void SetMute(IntPtr playerObject, int status);

		[NativeFunction("libvlc_audio_get_mute")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int IsMute(IntPtr playerObject);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_output_list_get")]
		private delegate IntPtr GetAudioOutputsDescriptions(IntPtr instance);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_output_list_release")]
		private delegate void ReleaseAudioOutputDescription(IntPtr audioOutput);

		[NativeFunction("libvlc_audio_output_set")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int SetAudioOutput(IntPtr playerObject, IntPtr audioOutputName);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		[NativeFunction("libvlc_audio_output_device_set")]
		private delegate void SetAudioOutputDevice(IntPtr playerObject, [MarshalAs(UnmanagedType.LPStr)] string audioOutputName, [MarshalAs(UnmanagedType.LPStr)] string deviceName);

		[NativeFunction("libvlc_audio_output_device_list_get")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr GetAudioOutputDeviceList(IntPtr playerObject, [MarshalAs(UnmanagedType.LPStr)] string aout);

		[NativeFunction("libvlc_audio_output_device_list_release")]
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate IntPtr ReleaseAudioOutputDeviceList(IntPtr p_list);

		private IntPtr _libVLCHandler = IntPtr.Zero;

		private IntPtr _libUMPHandler = IntPtr.Zero;

		private int _nativeIndex;

		private ManageLogCallback _manageLogCallback;

		public int NativeIndex => _nativeIndex;

		public WrapperStandalone(PlayerOptionsStandalone options)
		{
			UMPSettings instance = UMPSettings.Instance;
			NativeInterop.LoadLibrary("libvlccore", instance.LibrariesPath);
			_libVLCHandler = NativeInterop.LoadLibrary("libvlc", instance.LibrariesPath);
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				string librariesPath = instance.GetLibrariesPath(UMPSettings.RuntimePlatform, externalSpace: false);
				_libUMPHandler = NativeInterop.LoadLibrary("UniversalMediaPlayer", librariesPath);
			}
			_manageLogCallback = DebugLogHandler;
			NativeSetUnityLogMessageCallback(Marshal.GetFunctionPointerForDelegate(_manageLogCallback));
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				_nativeIndex = NativeInterop.GetLibraryDelegate<UMPNativeInitDel>(_libUMPHandler)();
			}
			else
			{
				_nativeIndex = UMPNativeInit();
			}
		}

		[DllImport("UniversalMediaPlayer")]
		private static extern int UMPNativeInit();

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativeUpdateIndex(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativeSetTexture(int index, IntPtr texture);

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativeSetPixelsBuffer(int index, IntPtr buffer, int width, int height);

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetPixelsBuffer(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativePixelsBufferRelease(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetLogMessage(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern int UMPNativeGetLogLevel(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern int UMPNativeGetState(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern float UMPNativeGetStateFloatValue(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern long UMPNativeGetStateLongValue(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetStateStringValue(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern int UMPNativeSetPixelsVerticalFlip(int index, bool flip);

		[DllImport("UniversalMediaPlayer")]
		internal static extern void UMPNativeSetAudioParams(int index, int numChannels, int sampleRate);

		[DllImport("UniversalMediaPlayer")]
		internal static extern IntPtr UMPNativeGetAudioParams(int index, char separator);

		[DllImport("UniversalMediaPlayer")]
		internal static extern int UMPNativeGetAudioSamples(int index, IntPtr decodedSamples, int samplesLength, AudioOutput.AudioChannels audioChannel);

		[DllImport("UniversalMediaPlayer")]
		internal static extern bool UMPNativeClearAudioSamples(int index, int samplesLength);

		[DllImport("UniversalMediaPlayer")]
		internal static extern void UMPNativeDirectRender(int index);

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetUnityRenderCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetVideoLockCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetVideoDisplayCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetVideoFormatCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetVideoCleanupCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetAudioSetupCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetAudioPlayCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeMediaPlayerEventCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativeSetBufferSizeCallback(int index, IntPtr callback);

		[DllImport("UniversalMediaPlayer")]
		private static extern IntPtr UMPNativeGetLogMessageCallback();

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativeSetUnityLogMessageCallback(IntPtr callback);

		[DllImport("UniversalMediaPlayer")]
		private static extern void UMPNativeUpdateFramesCounter(int index, int counter);

		[DllImport("UniversalMediaPlayer")]
		private static extern int UMPNativeGetFramesCounter(int index);

		private void DebugLogHandler(string msg)
		{
			Debug.LogError(msg);
		}

		public void NativeUpdateIndex()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeUpdateIndexDel>(_libUMPHandler)(_nativeIndex);
			}
			else
			{
				UMPNativeUpdateIndex(_nativeIndex);
			}
		}

		public void NativeSetTexture(IntPtr texture)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeSetTextureDel>(_libUMPHandler)(_nativeIndex, texture);
			}
			else
			{
				UMPNativeSetTexture(_nativeIndex, texture);
			}
		}

		public void NativeSetPixelsBuffer(IntPtr buffer, int width, int height)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeSetPixelsBufferDel>(_libUMPHandler)(_nativeIndex, buffer, width, height);
			}
			else
			{
				UMPNativeSetPixelsBuffer(_nativeIndex, buffer, width, height);
			}
		}

		public IntPtr NativeGetPixelsBuffer()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetPixelsBufferDel>(_libUMPHandler)(_nativeIndex);
			}
			return UMPNativeGetPixelsBuffer(_nativeIndex);
		}

		public void NativePixelsBufferRelease()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativePixelsBufferReleaseDel>(_libUMPHandler)(_nativeIndex);
			}
			else
			{
				UMPNativePixelsBufferRelease(_nativeIndex);
			}
		}

		public string NativeGetLogMessage()
		{
			IntPtr zero = IntPtr.Zero;
			zero = ((UMPSettings.RuntimePlatform != UMPSettings.Platforms.Linux) ? UMPNativeGetLogMessage(_nativeIndex) : NativeInterop.GetLibraryDelegate<UMPNativeGetLogMessageDel>(_libUMPHandler)(_nativeIndex));
			return (!(zero != IntPtr.Zero)) ? null : Marshal.PtrToStringAnsi(zero);
		}

		public int NativeGetLogLevel()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetLogLevelDel>(_libUMPHandler)(_nativeIndex);
			}
			return UMPNativeGetLogLevel(_nativeIndex);
		}

		public int NativeGetState()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetStateDel>(_libUMPHandler)(_nativeIndex);
			}
			return UMPNativeGetState(_nativeIndex);
		}

		private float NativeGetStateFloatValue()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetStateFloatValueDel>(_libUMPHandler)(_nativeIndex);
			}
			return UMPNativeGetStateFloatValue(_nativeIndex);
		}

		private long NativeGetStateLongValue()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetStateLongValueDel>(_libUMPHandler)(_nativeIndex);
			}
			return UMPNativeGetStateLongValue(_nativeIndex);
		}

		private string NativeGetStateStringValue()
		{
			IntPtr zero = IntPtr.Zero;
			zero = ((UMPSettings.RuntimePlatform != UMPSettings.Platforms.Linux) ? UMPNativeGetStateStringValue(_nativeIndex) : NativeInterop.GetLibraryDelegate<UMPNativeGetStateStringValueDel>(_libUMPHandler)(_nativeIndex));
			return (!(zero != IntPtr.Zero)) ? null : Marshal.PtrToStringAnsi(zero);
		}

		public void NativeSetPixelsVerticalFlip(bool flip)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeSetPixelsVerticalFlipDel>(_libUMPHandler)(_nativeIndex, flip);
			}
			else
			{
				UMPNativeSetPixelsVerticalFlip(_nativeIndex, flip);
			}
		}

		public void NativeSetAudioParams(int numChannels, int sampleRate)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeSetAudioParamsDel>(_libUMPHandler)(_nativeIndex, numChannels, sampleRate);
			}
			else
			{
				UMPNativeSetAudioParams(_nativeIndex, numChannels, sampleRate);
			}
		}

		public string NativeGetAudioParams(char separator)
		{
			IntPtr zero = IntPtr.Zero;
			zero = ((UMPSettings.RuntimePlatform != UMPSettings.Platforms.Linux) ? UMPNativeGetAudioParams(_nativeIndex, separator) : NativeInterop.GetLibraryDelegate<UMPNativeGetAudioParamsDel>(_libUMPHandler)(_nativeIndex, separator));
			return (!(zero != IntPtr.Zero)) ? null : Marshal.PtrToStringAnsi(zero);
		}

		public int NativeGetAudioSamples(IntPtr decodedSamples, int samplesLength, AudioOutput.AudioChannels audioChannel)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetAudioSamplesDel>(_libUMPHandler)(_nativeIndex, decodedSamples, samplesLength, audioChannel);
			}
			return UMPNativeGetAudioSamples(_nativeIndex, decodedSamples, samplesLength, audioChannel);
		}

		public bool NativeClearAudioSamples(int samplesLength)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeClearAudioSamplesDel>(_libUMPHandler)(_nativeIndex, samplesLength);
			}
			return UMPNativeClearAudioSamples(_nativeIndex, samplesLength);
		}

		public IntPtr NativeGetUnityRenderCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetUnityRenderCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetUnityRenderCallback();
		}

		public IntPtr NativeGetVideoLockCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetVideoLockCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetVideoLockCallback();
		}

		public IntPtr NativeGetVideoDisplayCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetVideoDisplayCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetVideoDisplayCallback();
		}

		public IntPtr NativeGetVideoFormatCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetVideoFormatCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetVideoFormatCallback();
		}

		public IntPtr NativeGetVideoCleanupCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetVideoCleanupCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetVideoCleanupCallback();
		}

		public IntPtr NativeGetAudioSetupCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetAudioSetupCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetAudioSetupCallback();
		}

		public IntPtr NativeGetAudioPlayCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetAudioPlayCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetAudioPlayCallback();
		}

		public IntPtr NativeMediaPlayerEventCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeMediaPlayerEventCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeMediaPlayerEventCallback();
		}

		public void NativeSetBufferSizeCallback(IntPtr callback)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeSetBufferSizeCallbackDel>(_libUMPHandler)(_nativeIndex, callback);
			}
			else
			{
				UMPNativeSetBufferSizeCallback(_nativeIndex, callback);
			}
		}

		public IntPtr NativeGetLogMessageCallback()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetLogMessageCallbackDel>(_libUMPHandler)();
			}
			return UMPNativeGetLogMessageCallback();
		}

		public void NativeSetUnityLogMessageCallback(IntPtr callback)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeSetUnityLogMessageCallbackDel>(_libUMPHandler)(callback);
			}
			else
			{
				UMPNativeSetUnityLogMessageCallback(callback);
			}
		}

		public void NativeUpdateFramesCounter(int counter)
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				NativeInterop.GetLibraryDelegate<UMPNativeUpdateFramesCounterCallbackDel>(_libUMPHandler)(_nativeIndex, counter);
			}
			else
			{
				UMPNativeUpdateFramesCounter(_nativeIndex, counter);
			}
		}

		public int NativeGetFramesCounter()
		{
			if (UMPSettings.RuntimePlatform == UMPSettings.Platforms.Linux)
			{
				return NativeInterop.GetLibraryDelegate<UMPNativeGetFramesCounterCallbackDel>(_libUMPHandler)(_nativeIndex);
			}
			return UMPNativeGetFramesCounter(_nativeIndex);
		}

		public void PlayerSetDataSource(string path, object playerObject = null)
		{
		}

		public bool PlayerPlay(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<Play>(_libVLCHandler)((IntPtr)playerObject) == 0;
		}

		public void PlayerPause(object playerObject)
		{
			NativeInterop.GetLibraryDelegate<Pause>(_libVLCHandler)((IntPtr)playerObject);
		}

		public void PlayerStop(object playerObject)
		{
			NativeInterop.GetLibraryDelegate<Stop>(_libVLCHandler)((IntPtr)playerObject);
		}

		public void PlayerRelease(object playerObject)
		{
			NativeInterop.GetLibraryDelegate<ReleaseMediaPlayer>(_libVLCHandler)((IntPtr)playerObject);
		}

		public bool PlayerIsPlaying(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<IsPlaying>(_libVLCHandler)((IntPtr)playerObject) == 1;
		}

		public bool PlayerWillPlay(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<CouldPlay>(_libVLCHandler)((IntPtr)playerObject) == 1;
		}

		public long PlayerGetLength(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetLength>(_libVLCHandler)((IntPtr)playerObject);
		}

		public float PlayerGetBufferingPercentage(object playerObject)
		{
			return 0f;
		}

		public long PlayerGetTime(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetTime>(_libVLCHandler)((IntPtr)playerObject);
		}

		public void PlayerSetTime(long time, object playerObject)
		{
			NativeInterop.GetLibraryDelegate<SetTime>(_libVLCHandler)((IntPtr)playerObject, time);
		}

		public float PlayerGetPosition(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetMediaPosition>(_libVLCHandler)((IntPtr)playerObject);
		}

		public void PlayerSetPosition(float pos, object playerObject)
		{
			NativeInterop.GetLibraryDelegate<SetMediaPosition>(_libVLCHandler)((IntPtr)playerObject, pos);
		}

		public float PlayerGetRate(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetRate>(_libVLCHandler)((IntPtr)playerObject);
		}

		public bool PlayerSetRate(float rate, object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<SetRate>(_libVLCHandler)((IntPtr)playerObject, rate) == 0;
		}

		public int PlayerGetVolume(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetVolume>(_libVLCHandler)((IntPtr)playerObject);
		}

		public int PlayerSetVolume(int volume, object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<SetVolume>(_libVLCHandler)((IntPtr)playerObject, volume);
		}

		public bool PlayerGetMute(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<IsMute>(_libVLCHandler)((IntPtr)playerObject) == 1;
		}

		public void PlayerSetMute(bool mute, object playerObject)
		{
			NativeInterop.GetLibraryDelegate<SetMute>(_libVLCHandler)((IntPtr)playerObject, mute ? 1 : 0);
		}

		public int PlayerVideoWidth(object playerObject)
		{
			uint px = 0u;
			uint py = 0u;
			NativeInterop.GetLibraryDelegate<GetVideoSize>(_libVLCHandler)((IntPtr)playerObject, 0u, out px, out py);
			return (int)px;
		}

		public int PlayerVideoHeight(object playerObject)
		{
			uint px = 0u;
			uint py = 0u;
			NativeInterop.GetLibraryDelegate<GetVideoSize>(_libVLCHandler)((IntPtr)playerObject, 0u, out px, out py);
			return (int)py;
		}

		public int PlayerVideoFramesCounter(object playerObject)
		{
			return 0;
		}

		public PlayerState PlayerGetState()
		{
			return (PlayerState)NativeGetState();
		}

		public object PlayerGetStateValue()
		{
			object obj = NativeGetStateFloatValue();
			if ((float)obj < 0f)
			{
				obj = NativeGetStateLongValue();
				if ((long)obj < 0)
				{
					obj = NativeGetStateStringValue();
				}
			}
			return obj;
		}

		public MediaTrackInfo[] PlayerSpuGetTracks(object playerObject)
		{
			List<MediaTrackInfo> list = new List<MediaTrackInfo>();
			int num = NativeInterop.GetLibraryDelegate<GetVideoSpuCount>(_libVLCHandler)((IntPtr)playerObject);
			IntPtr intPtr = NativeInterop.GetLibraryDelegate<GetVideoSpuDescription>(_libVLCHandler)((IntPtr)playerObject);
			for (int i = 0; i < num; i++)
			{
				if (intPtr != IntPtr.Zero)
				{
					TrackDescription trackDescription = (TrackDescription)Marshal.PtrToStructure(intPtr, typeof(TrackDescription));
					list.Add(new MediaTrackInfo(trackDescription.Id, trackDescription.Name));
					intPtr = trackDescription.NextDescription;
				}
			}
			return list.ToArray();
		}

		public int PlayerSpuGetTrack(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetVideoSpu>(_libVLCHandler)((IntPtr)playerObject);
		}

		public int PlayerSpuSetTrack(int spuIndex, object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<SetVideoSpu>(_libVLCHandler)((IntPtr)playerObject, spuIndex);
		}

		public MediaTrackInfo[] PlayerAudioGetTracks(object playerObject)
		{
			List<MediaTrackInfo> list = new List<MediaTrackInfo>();
			int num = NativeInterop.GetLibraryDelegate<GetAudioTracksCount>(_libVLCHandler)((IntPtr)playerObject);
			IntPtr intPtr = NativeInterop.GetLibraryDelegate<GetAudioTracksDescriptions>(_libVLCHandler)((IntPtr)playerObject);
			for (int i = 0; i < num; i++)
			{
				if (intPtr != IntPtr.Zero)
				{
					TrackDescription trackDescription = (TrackDescription)Marshal.PtrToStructure(intPtr, typeof(TrackDescription));
					list.Add(new MediaTrackInfo(trackDescription.Id, trackDescription.Name));
					intPtr = trackDescription.NextDescription;
				}
			}
			return list.ToArray();
		}

		public int PlayerAudioGetTrack(object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<GetAudioTrack>(_libVLCHandler)((IntPtr)playerObject);
		}

		public int PlayerAudioSetTrack(int audioIndex, object playerObject)
		{
			return NativeInterop.GetLibraryDelegate<SetAudioTrack>(_libVLCHandler)((IntPtr)playerObject, audioIndex);
		}

		public IntPtr ExpandedLibVLCNew(string[] args)
		{
			if (args == null)
			{
				args = new string[0];
			}
			return NativeInterop.GetLibraryDelegate<CreateNewInstance>(_libVLCHandler)(args.Length, args);
		}

		public void ExpandedLibVLCRelease(IntPtr libVLCInst)
		{
			NativeInterop.GetLibraryDelegate<ReleaseInstance>(_libVLCHandler)(libVLCInst);
		}

		public IntPtr ExpandedMediaNewLocation(IntPtr libVLCInst, string path)
		{
			return NativeInterop.GetLibraryDelegate<CreateNewMediaFromLocation>(_libVLCHandler)(libVLCInst, path);
		}

		public void ExpandedSetMedia(IntPtr mpInstance, IntPtr libVLCMediaInst)
		{
			NativeInterop.GetLibraryDelegate<SetMediaToMediaPlayer>(_libVLCHandler)(mpInstance, libVLCMediaInst);
		}

		public void ExpandedAddOption(IntPtr libVLCMediaInst, string option)
		{
			NativeInterop.GetLibraryDelegate<AddOptionToMedia>(_libVLCHandler)(libVLCMediaInst, option);
		}

		public void ExpandedMediaGetStats(IntPtr mpInstance, out MediaStats mediaStats)
		{
			NativeInterop.GetLibraryDelegate<GetMediaStats>(_libVLCHandler)(mpInstance, out mediaStats);
		}

		public TrackInfo[] ExpandedMediaGetTracksInfo(IntPtr mpInstance)
		{
			IntPtr tracksInformationsPointer = default(IntPtr);
			int num = NativeInterop.GetLibraryDelegate<GetMediaTracksInformations>(_libVLCHandler)(mpInstance, out tracksInformationsPointer);
			if (num < 0)
			{
				return null;
			}
			IntPtr ptr = tracksInformationsPointer;
			TrackInfo[] array = new TrackInfo[num];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (TrackInfo)Marshal.PtrToStructure(ptr, typeof(TrackInfo));
				ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(TrackInfo)));
			}
			ExpandedFree(tracksInformationsPointer);
			return array;
		}

		public void ExpandedMediaRelease(IntPtr libVLCMediaInst)
		{
			NativeInterop.GetLibraryDelegate<ReleaseMedia>(_libVLCHandler)(libVLCMediaInst);
		}

		public IntPtr ExpandedMediaPlayerNew(IntPtr libVLCInst)
		{
			return NativeInterop.GetLibraryDelegate<CreateMediaPlayer>(_libVLCHandler)(libVLCInst);
		}

		public void ExpandedFree(IntPtr instance)
		{
			NativeInterop.GetLibraryDelegate<Free>(_libVLCHandler)(instance);
		}

		public IntPtr ExpandedEventManager(IntPtr mpInstance)
		{
			return NativeInterop.GetLibraryDelegate<GetMediaPlayerEventManager>(_libVLCHandler)(mpInstance);
		}

		public int ExpandedEventAttach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData)
		{
			return NativeInterop.GetLibraryDelegate<AttachEvent>(_libVLCHandler)(eventManagerInst, eventType, callback, userData);
		}

		public void ExpandedEventDetach(IntPtr eventManagerInst, EventTypes eventType, IntPtr callback, IntPtr userData)
		{
			NativeInterop.GetLibraryDelegate<DetachEvent>(_libVLCHandler)(eventManagerInst, eventType, callback, userData);
		}

		public void ExpandedLogSet(IntPtr libVLC, IntPtr callback, IntPtr data)
		{
			NativeInterop.GetLibraryDelegate<LogSet>(_libVLCHandler)(libVLC, callback, data);
		}

		public void ExpandedLogUnset(IntPtr libVLC)
		{
			NativeInterop.GetLibraryDelegate<LogUnset>(_libVLCHandler)(libVLC);
		}

		public void ExpandedVideoSetCallbacks(IntPtr mpInstance, IntPtr @lock, IntPtr unlock, IntPtr display, IntPtr opaque)
		{
			NativeInterop.GetLibraryDelegate<SetVideoCallbacks>(_libVLCHandler)(mpInstance, @lock, unlock, display, opaque);
		}

		public void ExpandedVideoSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup)
		{
			NativeInterop.GetLibraryDelegate<SetVideoFormatCallbacks>(_libVLCHandler)(mpInstance, setup, cleanup);
		}

		public void ExpandedVideoSetFormat(IntPtr mpInstance, string chroma, int width, int height, int pitch)
		{
			NativeInterop.GetLibraryDelegate<SetVideoFormat>(_libVLCHandler)(mpInstance, chroma, (uint)width, (uint)height, (uint)pitch);
		}

		public void ExpandedAudioSetCallbacks(IntPtr mpInstance, IntPtr play, IntPtr pause, IntPtr resume, IntPtr flush, IntPtr drain, IntPtr opaque)
		{
			NativeInterop.GetLibraryDelegate<SetAudioCallbacks>(_libVLCHandler)(mpInstance, play, pause, resume, flush, drain, opaque);
		}

		public void ExpandedAudioSetFormatCallbacks(IntPtr mpInstance, IntPtr setup, IntPtr cleanup)
		{
			NativeInterop.GetLibraryDelegate<SetAudioFormatCallbacks>(_libVLCHandler)(mpInstance, setup, cleanup);
		}

		public void ExpandedAudioSetFormat(IntPtr mpInstance, string format, int rate, int channels)
		{
			NativeInterop.GetLibraryDelegate<SetAudioFormat>(_libVLCHandler)(mpInstance, format, rate, channels);
		}

		public long ExpandedGetAudioDelay(IntPtr mpInstance)
		{
			return NativeInterop.GetLibraryDelegate<GetAudioDelay>(_libVLCHandler)(mpInstance);
		}

		public void ExpandedSetAudioDelay(IntPtr mpInstance, long channel)
		{
			NativeInterop.GetLibraryDelegate<SetAudioDelay>(_libVLCHandler)(mpInstance, channel);
		}

		public int ExpandedAudioOutputSet(IntPtr mpInstance, string psz_name)
		{
			return NativeInterop.GetLibraryDelegate<SetAudioOutput>(_libVLCHandler)(mpInstance, Marshal.StringToHGlobalAnsi(psz_name));
		}

		public IntPtr ExpandedAudioOutputListGet(IntPtr mpInstance)
		{
			return NativeInterop.GetLibraryDelegate<GetAudioOutputsDescriptions>(_libVLCHandler)(mpInstance);
		}

		public void ExpandedAudioOutputListRelease(IntPtr outputListInst)
		{
			NativeInterop.GetLibraryDelegate<ReleaseAudioOutputDescription>(_libVLCHandler)(outputListInst);
		}

		public void ExpandedAudioOutputDeviceSet(IntPtr mpInstance, string psz_audio_output, string psz_device_id)
		{
			NativeInterop.GetLibraryDelegate<SetAudioOutputDevice>(_libVLCHandler)(mpInstance, psz_audio_output, psz_device_id);
		}

		public IntPtr ExpandedAudioOutputDeviceListGet(IntPtr mpInstance, string aout)
		{
			return NativeInterop.GetLibraryDelegate<GetAudioOutputDeviceList>(_libVLCHandler)(mpInstance, aout);
		}

		public void ExpandedAudioOutputDeviceListRelease(IntPtr deviceListInst)
		{
			NativeInterop.GetLibraryDelegate<ReleaseAudioOutputDeviceList>(_libVLCHandler)(deviceListInst);
		}

		public int ExpandedSpuSetFile(IntPtr mpInstance, string path)
		{
			return NativeInterop.GetLibraryDelegate<SetVideoSubtitleFile>(_libVLCHandler)(mpInstance, path);
		}

		public float ExpandedVideoGetScale(IntPtr mpInstance)
		{
			return NativeInterop.GetLibraryDelegate<GetVideoScale>(_libVLCHandler)(mpInstance);
		}

		public void ExpandedVideoSetScale(IntPtr mpInstance, float factor)
		{
			NativeInterop.GetLibraryDelegate<SetVideoScale>(_libVLCHandler)(mpInstance, factor);
		}

		public void ExpandedVideoTakeSnapshot(IntPtr mpInstance, uint stream, string filePath, uint width, uint height)
		{
			NativeInterop.GetLibraryDelegate<TakeSnapshot>(_libVLCHandler)(mpInstance, stream, filePath, width, height);
		}

		public float ExpandedVideoFrameRate(IntPtr mpInstance)
		{
			return NativeInterop.GetLibraryDelegate<GetFramesPerSecond>(_libVLCHandler)(mpInstance);
		}
	}
}
