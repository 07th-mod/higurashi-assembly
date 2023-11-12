namespace Assets.Scripts.Core.Buriko
{
	public enum BurikoOperations
	{
		NullOp = -1,
		StoreValueToLocalWork,
		LoadValueFromLocalWork,
		SetLocalFlag,
		SetGlobalFlag,
		GetLocalFlag,
		GetGlobalFlag,
		CallScript,
		JumpScript,
		CallSection,
		JumpSection,
		Return,
		Wait,
		WaitForInput,
		SetValidityOfSkipping,
		SetValidityOfSaving,
		SetValidityOfInput,
		SetValidityOfUserEffectSpeed,
		OutputLine,
		OutputLineAll,
		ClearMessage,
		SetFontOfMessage,
		SetSpeedOfMessage,
		DisableWindow,
		DisplayWindow,
		HideWindow,
		SpringText,
		SetColorOfMessage,
		SetStyleOfMessageSwinging,
		Select,
		PlayBGM,
		StopBGM,
		ChangeVolumeOfBGM,
		FadeOutBGM,
		FadeOutMultiBGM,
		PlaySE,
		StopSE,
		FadeOutSE,
		WaitToFinishSEPlaying,
		PlayVoice,
		WaitToFinishVoicePlaying,
		StopAllSound,
		PreloadBitmap,
		StartShakingOfAllObjects,
		TerminateShakingOfAllObjects,
		StartShakingOfWindow,
		TerminateShakingOfWindow,
		StartShakingOfBustshot,
		TerminateShakingOfBustshot,
		StartShakingOfSprite,
		TerminateShakingOfSprite,
		ShakeScreen,
		ShakeScreenSx,
		DrawBG,
		FadeBG,
		DrawBGWithMask,
		RotateBG,
		DrawScene,
		DrawSceneWithMask,
		ChangeScene,
		FadeScene,
		FadeSceneWithMask,
		DrawBustshot,
		MoveBustshot,
		FadeBustshot,
		ChangeBustshot,
		DrawBustshotWithFiltering,
		DrawFace,
		FadeFace,
		ExecutePlannedControl,
		DrawSprite,
		DrawSpriteWithFiltering,
		FadeSprite,
		FadeSpriteWithFiltering,
		MoveSprite,
		MoveSpriteEx,
		ControlMotionOfSprite,
		GetPositionOfSprite,
		EnableHorizontalGradation,
		DisableGradation,
		EnlargeScreen,
		DisableEffector,
		EnableBlur,
		DisableBlur,
		DrawFilm,
		FadeFilm,
		SetValidityOfFilmToFace,
		FadeAllBustshots,
		FadeBustshotWithFiltering,
		SetDrawingPointOfMessage,
		Negative,
		EnableJumpingOfReturnIcon,
		SetValidityOfWindowDisablingWhenGraphicsControl,
		SetValidityOfInterface,
		ViewTips,
		ViewChapterScreen,
		ViewExtras,
		ChapterPreview,
		SavePoint,
		GetRandomNumber,
		PlaceViewTip,
		PlaceViewTip2,
		DrawStandgraphic,
		DrawBustFace,
		PlusStandgraphic1,
		PlusStandgraphic2,
		PlusStandgraphic3,
		FadeAllBustshots2,
		FadeAllBustshots3,
		BlurOffOn,
		TitleScreen,
		OpenGallery,
		HideGallery,
		RevealGallery,
		CloseGallery,
		SetSkipAll,
		SetTextFade,
		Break,
		LanguagePrompt,
		GetAchievement,
		CheckTipsAchievements,
		SetFontId,
		SetCharSpacing,
		SetLineSpacing,
		SetFontSize,
		SetWindowPos,
		SetWindowSize,
		SetWindowMargins,
		SetNameFormat,
		SetScreenAspect,
		SetGuiPosition,
		SetValidityOfLoading,
		ActivateScreenEffectForcedly,
		DrawFragment,
		StopFragment,
		DrawSpriteFixedSize,
		DrawSpriteWithFilteringFixedSize,
		Update,
		ShiftSection,
		FragmentViewChapterScreen,
		FragmentListScreen,
		SetWindowBackground,
		JumpScriptSection,
		LoadTitleScreen,
		MoveScreen,
		SetNameHistoryFormat,
		SetWindowFadeTime,
		SetWindowFadeOutTime,
		CallScriptSection,
		ChangeBustshotWithFiltering,
		MakeLayerPersistent,
		EndLayerPersistence,
		GetLayerPriority,
		SetNameColor,
		ResetNameColor,
		SetJpFontSize,
		SetJpNameFormat,
		SetJpLineSpacing,
		IsUnityEditor,
		DrawSceneWithScroll,
		PlayVideo,
		OmakeScreenSection,
		DisableLoopingOfBGM,
		DebugMessage,
		ModEnableNVLModeInADVMode,
		ModDisableNVLModeInADVMode,
		ModADVModeSettingLoad,
		ModNVLModeSettingLoad,
		ModNVLADVModeSettingLoad,
		ModCallScriptSection,
		ModDrawCharacter,
		ModDrawCharacterWithFiltering,
		ModPlayVoiceLS,
		ModPlayMovie,
		ModSetConfigFontSize,
		ModSetChapterJumpFontSize,
		ModSetHighestChapterFlag,
		ModGetHighestChapterFlag,
		ModSetMainFontOutlineWidth,
		ModSetLayerFilter,
		ModAddArtset,
		ModClearArtsets,
		ModRyukishiModeSettingLoad,
		ModRyukishiSetGuiPosition,
		ModPlayBGM,
		ModFadeOutBGM,
		ModAddBGMset,
		ModAddSEset,
		ModAddAudioset,
		ModGenericCall,
	}
}
