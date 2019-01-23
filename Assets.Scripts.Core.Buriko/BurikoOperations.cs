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
		SpringText,
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
		ModGetHighestChapterFlag
	}
}
