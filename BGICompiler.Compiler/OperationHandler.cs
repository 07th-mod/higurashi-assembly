using Antlr.Runtime.Tree;
using Assets.Scripts.Core.Buriko;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BGICompiler.Compiler
{
	public class OperationHandler
	{
		private BinaryWriter output;

		private static Dictionary<string, OpType> paramLookup = new Dictionary<string, OpType>();

		private static bool _isReady = false;

		private int line;

		private string name;

		public OperationHandler()
		{
			output = BGItoMG.Instance.Output;
			if (!_isReady)
			{
				FillParamValues();
			}
			_isReady = true;
		}

		public void FillParamValues()
		{
			paramLookup.Add("StoreValueToLocalWork", new OpType(BurikoOperations.StoreValueToLocalWork, "vi"));
			paramLookup.Add("LoadValueFromLocalWork", new OpType(BurikoOperations.LoadValueFromLocalWork, "v"));
			paramLookup.Add("SetLocalFlag", new OpType(BurikoOperations.SetLocalFlag, "vi"));
			paramLookup.Add("SetGlobalFlag", new OpType(BurikoOperations.SetGlobalFlag, "vi"));
			paramLookup.Add("GetLocalFlag", new OpType(BurikoOperations.GetLocalFlag, "v"));
			paramLookup.Add("GetGlobalFlag", new OpType(BurikoOperations.GetGlobalFlag, "v"));
			paramLookup.Add("CallScript", new OpType(BurikoOperations.CallScript, "s"));
			paramLookup.Add("JumpScript", new OpType(BurikoOperations.JumpScript, "s"));
			paramLookup.Add("CallSection", new OpType(BurikoOperations.CallSection, "s"));
			paramLookup.Add("JumpSection", new OpType(BurikoOperations.JumpSection, "s"));
			paramLookup.Add("Wait", new OpType(BurikoOperations.Wait, "i"));
			paramLookup.Add("WaitForInput", new OpType(BurikoOperations.WaitForInput, string.Empty));
			paramLookup.Add("SetValidityOfInput", new OpType(BurikoOperations.SetValidityOfInput, "b"));
			paramLookup.Add("SetValidityOfSkipping", new OpType(BurikoOperations.SetValidityOfSkipping, "b"));
			paramLookup.Add("SetValidityOfSaving", new OpType(BurikoOperations.SetValidityOfSaving, "b"));
			paramLookup.Add("SetValidityOfUserEffectSpeed", new OpType(BurikoOperations.SetValidityOfUserEffectSpeed, "b"));
			paramLookup.Add("OutputLine", new OpType(BurikoOperations.OutputLine, "ssssi"));
			paramLookup.Add("OutputLineAll", new OpType(BurikoOperations.OutputLineAll, "ssi"));
			paramLookup.Add("ClearMessage", new OpType(BurikoOperations.ClearMessage, string.Empty));
			paramLookup.Add("SetFontOfMessage", new OpType(BurikoOperations.SetFontOfMessage, "iii"));
			paramLookup.Add("SetSpeedOfMessage", new OpType(BurikoOperations.SetSpeedOfMessage, "bi"));
			paramLookup.Add("SetStyleOfMessageSwinging", new OpType(BurikoOperations.SetStyleOfMessageSwinging, "i"));
			paramLookup.Add("DisableWindow", new OpType(BurikoOperations.DisableWindow, string.Empty));
			paramLookup.Add("SpringText", new OpType(BurikoOperations.SpringText, "ii"));
			paramLookup.Add("Select", new OpType(BurikoOperations.Select, "iv"));
			paramLookup.Add("PlayBGM", new OpType(BurikoOperations.PlayBGM, "isii"));
			paramLookup.Add("StopBGM", new OpType(BurikoOperations.StopBGM, "i"));
			paramLookup.Add("ChangeVolumeOfBGM", new OpType(BurikoOperations.ChangeVolumeOfBGM, "iii"));
			paramLookup.Add("FadeOutBGM", new OpType(BurikoOperations.FadeOutBGM, "iib"));
			paramLookup.Add("FadeOutMultiBGM", new OpType(BurikoOperations.FadeOutMultiBGM, "iiib"));
			paramLookup.Add("PlaySE", new OpType(BurikoOperations.PlaySE, "isii"));
			paramLookup.Add("StopSE", new OpType(BurikoOperations.StopSE, "i"));
			paramLookup.Add("FadeOutSE", new OpType(BurikoOperations.FadeOutSE, "ii"));
			paramLookup.Add("WaitToFinishSEPlaying", new OpType(BurikoOperations.WaitToFinishSEPlaying, "i"));
			paramLookup.Add("PlayVoice", new OpType(BurikoOperations.PlayVoice, "isi"));
			paramLookup.Add("WaitToFinishVoicePlaying", new OpType(BurikoOperations.WaitToFinishVoicePlaying, "i"));
			paramLookup.Add("PreloadBitmap", new OpType(BurikoOperations.PreloadBitmap, "s"));
			paramLookup.Add("StartShakingOfAllObjects", new OpType(BurikoOperations.StartShakingOfAllObjects, "iiiiib"));
			paramLookup.Add("TerminateShakingOfAllObjects", new OpType(BurikoOperations.TerminateShakingOfAllObjects, "ib"));
			paramLookup.Add("StartShakingOfWindow", new OpType(BurikoOperations.StartShakingOfWindow, "iiiiib"));
			paramLookup.Add("TerminateShakingOfWindow", new OpType(BurikoOperations.TerminateShakingOfWindow, "ib"));
			paramLookup.Add("StartShakingOfBustshot", new OpType(BurikoOperations.StartShakingOfBustshot, "iiiiiib"));
			paramLookup.Add("TerminateShakingOfBustshot", new OpType(BurikoOperations.TerminateShakingOfBustshot, "iib"));
			paramLookup.Add("StartShakingOfSprite", new OpType(BurikoOperations.StartShakingOfSprite, "iiiiiib"));
			paramLookup.Add("TerminateShakingOfSprite", new OpType(BurikoOperations.TerminateShakingOfSprite, "iib"));
			paramLookup.Add("ShakeScreen", new OpType(BurikoOperations.ShakeScreen, "iiiii"));
			paramLookup.Add("ShakeScreenSx", new OpType(BurikoOperations.ShakeScreenSx, "ii"));
			paramLookup.Add("DrawBG", new OpType(BurikoOperations.DrawBG, "sib"));
			paramLookup.Add("FadeBG", new OpType(BurikoOperations.FadeBG, "ib"));
			paramLookup.Add("DrawBGWithMask", new OpType(BurikoOperations.DrawBGWithMask, "ssiib"));
			paramLookup.Add("DrawScene", new OpType(BurikoOperations.DrawScene, "si"));
			paramLookup.Add("DrawSceneWithMask", new OpType(BurikoOperations.DrawSceneWithMask, "ssiii"));
			paramLookup.Add("ChangeScene", new OpType(BurikoOperations.ChangeScene, "sii"));
			paramLookup.Add("FadeScene", new OpType(BurikoOperations.FadeScene, "i"));
			paramLookup.Add("FadeSceneWithMask", new OpType(BurikoOperations.FadeSceneWithMask, "siii"));
			paramLookup.Add("DrawBustshot", new OpType(BurikoOperations.DrawBustshot, "isiiibiiiiiiiiib"));
			paramLookup.Add("MoveBustshot", new OpType(BurikoOperations.MoveBustshot, "isiiiiib"));
			paramLookup.Add("FadeBustshot", new OpType(BurikoOperations.FadeBustshot, "ibiiiiib"));
			paramLookup.Add("DrawBustshotWithFiltering", new OpType(BurikoOperations.DrawBustshotWithFiltering, "issiiibiiiiiiib"));
			paramLookup.Add("DrawFace", new OpType(BurikoOperations.DrawFace, "sib"));
			paramLookup.Add("FadeFace", new OpType(BurikoOperations.FadeFace, "ib"));
			paramLookup.Add("ExecutePlannedControl", new OpType(BurikoOperations.ExecutePlannedControl, "b"));
			paramLookup.Add("DrawSprite", new OpType(BurikoOperations.DrawSprite, "issiiiiiibbiiiib"));
			paramLookup.Add("DrawSpriteWithFiltering", new OpType(BurikoOperations.DrawSpriteWithFiltering, "issiiibbiiiib"));
			paramLookup.Add("FadeSprite", new OpType(BurikoOperations.FadeSprite, "iib"));
			paramLookup.Add("FadeSpriteWithFiltering", new OpType(BurikoOperations.FadeSpriteWithFiltering, "isiib"));
			paramLookup.Add("MoveSprite", new OpType(BurikoOperations.MoveSprite, "iiiiiiiiib"));
			paramLookup.Add("MoveSpriteEx", new OpType(BurikoOperations.MoveSpriteEx, "isiiviiiiib"));
			paramLookup.Add("ControlMotionOfSprite", new OpType(BurikoOperations.ControlMotionOfSprite, "iivi"));
			paramLookup.Add("GetPositionOfSprite", new OpType(BurikoOperations.GetPositionOfSprite, "vi"));
			paramLookup.Add("EnableHorizontalGradation", new OpType(BurikoOperations.EnableHorizontalGradation, "iib"));
			paramLookup.Add("DisableGradation", new OpType(BurikoOperations.DisableGradation, "ib"));
			paramLookup.Add("EnlargeScreen", new OpType(BurikoOperations.EnlargeScreen, "iiiibib"));
			paramLookup.Add("DisableEffector", new OpType(BurikoOperations.DisableEffector, "ib"));
			paramLookup.Add("EnableBlur", new OpType(BurikoOperations.EnableBlur, "iib"));
			paramLookup.Add("DisableBlur", new OpType(BurikoOperations.DisableBlur, "ib"));
			paramLookup.Add("DrawFilm", new OpType(BurikoOperations.DrawFilm, "iiiiiiib"));
			paramLookup.Add("FadeFilm", new OpType(BurikoOperations.FadeFilm, "ib"));
			paramLookup.Add("SetValidityOfFilmToFace", new OpType(BurikoOperations.SetValidityOfFilmToFace, "b"));
			paramLookup.Add("FadeAllBustshots", new OpType(BurikoOperations.FadeAllBustshots, "ib"));
			paramLookup.Add("FadeBustshotWithFiltering", new OpType(BurikoOperations.FadeBustshotWithFiltering, "isibiiib"));
			paramLookup.Add("SetDrawingPointOfMessage", new OpType(BurikoOperations.SetDrawingPointOfMessage, "ii"));
			paramLookup.Add("Negative", new OpType(BurikoOperations.Negative, "ib"));
			paramLookup.Add("EnableJumpingOfReturnIcon", new OpType(BurikoOperations.EnableJumpingOfReturnIcon, string.Empty));
			paramLookup.Add("SetValidityOfWindowDisablingWhenGraphicsControl", new OpType(BurikoOperations.SetValidityOfWindowDisablingWhenGraphicsControl, "b"));
			paramLookup.Add("SetValidityOfInterface", new OpType(BurikoOperations.SetValidityOfInterface, "b"));
			paramLookup.Add("ShowExtras", new OpType(BurikoOperations.ViewExtras, string.Empty));
			paramLookup.Add("ShowChapterScreen", new OpType(BurikoOperations.ViewChapterScreen, string.Empty));
			paramLookup.Add("ShowTips", new OpType(BurikoOperations.ViewTips, "i"));
			paramLookup.Add("ShowChapterPreview", new OpType(BurikoOperations.ChapterPreview, string.Empty));
			paramLookup.Add("SavePoint", new OpType(BurikoOperations.SavePoint, "ss"));
			paramLookup.Add("SetValidityOfTextFade", new OpType(BurikoOperations.SetTextFade, "b"));
			paramLookup.Add("LanguagePrompt", new OpType(BurikoOperations.LanguagePrompt, string.Empty));
			paramLookup.Add("GetAchievement", new OpType(BurikoOperations.GetAchievement, "s"));
			paramLookup.Add("CheckTipsAchievements", new OpType(BurikoOperations.CheckTipsAchievements, string.Empty));
			paramLookup.Add("SetFontId", new OpType(BurikoOperations.SetFontId, "i"));
			paramLookup.Add("SetCharSpacing", new OpType(BurikoOperations.SetCharSpacing, "i"));
			paramLookup.Add("SetLineSpacing", new OpType(BurikoOperations.SetLineSpacing, "i"));
			paramLookup.Add("SetFontSize", new OpType(BurikoOperations.SetFontSize, "i"));
			paramLookup.Add("SetWindowPos", new OpType(BurikoOperations.SetWindowPos, "ii"));
			paramLookup.Add("SetWindowSize", new OpType(BurikoOperations.SetWindowSize, "ii"));
			paramLookup.Add("SetWindowMargins", new OpType(BurikoOperations.SetWindowMargins, "iiii"));
			paramLookup.Add("PlaceViewTip", new OpType(BurikoOperations.PlaceViewTip, "i"));
			paramLookup.Add("PlaceViewTip2", new OpType(BurikoOperations.PlaceViewTip2, "ii"));
			paramLookup.Add("DrawStandgraphic", new OpType(BurikoOperations.DrawStandgraphic, "isbiib"));
			paramLookup.Add("DrawBustFace", new OpType(BurikoOperations.DrawBustFace, "isbi"));
			paramLookup.Add("PlusStandgraphic1", new OpType(BurikoOperations.PlusStandgraphic1, "is"));
			paramLookup.Add("PlusStandgraphic2", new OpType(BurikoOperations.PlusStandgraphic2, "is"));
			paramLookup.Add("PlusStandgraphic3", new OpType(BurikoOperations.PlusStandgraphic3, "is"));
			paramLookup.Add("FadeAllBustshots2", new OpType(BurikoOperations.FadeAllBustshots2, "ib"));
			paramLookup.Add("FadeAllBustshots3", new OpType(BurikoOperations.FadeAllBustshots3, "ib"));
			paramLookup.Add("BlurOffOn", new OpType(BurikoOperations.BlurOffOn, "ii"));
			paramLookup.Add("TitleScreen", new OpType(BurikoOperations.TitleScreen, string.Empty));
			paramLookup.Add("OpenGallery", new OpType(BurikoOperations.OpenGallery, string.Empty));
			paramLookup.Add("HideGallery", new OpType(BurikoOperations.HideGallery, string.Empty));
			paramLookup.Add("RevealGallery", new OpType(BurikoOperations.RevealGallery, string.Empty));
			paramLookup.Add("CloseGallery", new OpType(BurikoOperations.CloseGallery, string.Empty));
			paramLookup.Add("SetSkipAll", new OpType(BurikoOperations.SetSkipAll, "b"));
			paramLookup.Add("SetNameFormat", new OpType(BurikoOperations.SetNameFormat, "s"));
			paramLookup.Add("SetScreenAspect", new OpType(BurikoOperations.SetScreenAspect, "s"));
			paramLookup.Add("SetGUIPosition", new OpType(BurikoOperations.SetGuiPosition, "ii"));
			paramLookup.Add("Break", new OpType(BurikoOperations.Break, string.Empty));
			paramLookup.Add("ModEnableNVLModeInADVMode", new OpType(BurikoOperations.ModEnableNVLModeInADVMode, string.Empty));
			paramLookup.Add("ModDisableNVLModeInADVMode", new OpType(BurikoOperations.ModDisableNVLModeInADVMode, string.Empty));
			paramLookup.Add("ModADVModeSettingLoad", new OpType(BurikoOperations.ModADVModeSettingLoad, "siiiiiiiiiiii"));
			paramLookup.Add("ModNVLModeSettingLoad", new OpType(BurikoOperations.ModNVLModeSettingLoad, "siiiiiiiiiiii"));
			paramLookup.Add("ModNVLADVModeSettingLoad", new OpType(BurikoOperations.ModNVLADVModeSettingLoad, "siiiiiiiiiiii"));
			paramLookup.Add("ModCallScriptSection", new OpType(BurikoOperations.ModCallScriptSection, "ss"));
			paramLookup.Add("ModDrawCharacter", new OpType(BurikoOperations.ModDrawCharacter, "iissiiibiiiiiiiiib"));
			paramLookup.Add("ModDrawCharacterWithFiltering", new OpType(BurikoOperations.ModDrawCharacterWithFiltering, "iisssiiibiiiiiiib"));
			paramLookup.Add("ModPlayVoiceLS", new OpType(BurikoOperations.ModPlayVoiceLS, "iisib"));
			paramLookup.Add("ModPlayMovie", new OpType(BurikoOperations.ModPlayMovie, "s"));
			paramLookup.Add("ModSetConfigFontSize", new OpType(BurikoOperations.ModSetConfigFontSize, "i"));
			paramLookup.Add("ModSetChapterJumpFontSize", new OpType(BurikoOperations.ModSetChapterJumpFontSize, "ii"));
			paramLookup.Add("ModSetHighestChapterFlag", new OpType(BurikoOperations.ModSetHighestChapterFlag, "ii"));
			paramLookup.Add("ModGetHighestChapterFlag", new OpType(BurikoOperations.ModGetHighestChapterFlag, "i"));
			paramLookup.Add("ModSetMainFontOutlineWidth", new OpType(BurikoOperations.ModSetMainFontOutlineWidth, "i"));
			paramLookup.Add("ModSetLayerFilter", new OpType(BurikoOperations.ModSetLayerFilter, "iis"));
			paramLookup.Add("ModAddArtset", new OpType(BurikoOperations.ModAddArtset, "sss"));
			paramLookup.Add("ModClearArtsets", new OpType(BurikoOperations.ModClearArtsets, string.Empty));
			paramLookup.Add("ModRyukishiModeSettingLoad", new OpType(BurikoOperations.ModRyukishiModeSettingLoad, "siiiiiiiiiiii"));
			paramLookup.Add("ModRyukishiSetGuiPosition", new OpType(BurikoOperations.ModRyukishiSetGuiPosition, "ii"));
			paramLookup.Add("ModPlayBGM", new OpType(BurikoOperations.ModPlayBGM, "isiii"));
			paramLookup.Add("ModFadeOutBGM", new OpType(BurikoOperations.ModFadeOutBGM, "iibi"));
			paramLookup.Add("ModAddBGMset", new OpType(BurikoOperations.ModAddBGMset, "sss"));
			paramLookup.Add("ModAddSEset", new OpType(BurikoOperations.ModAddSEset, "sss"));
			paramLookup.Add("ModAddAudioset", new OpType(BurikoOperations.ModAddAudioset, "ssiiii"));
			paramLookup.Add("ModGenericCall", new OpType(BurikoOperations.ModGenericCall, "ss"));
		}

		public void ParamCheck(string op, BGIParameters param)
		{
			if (!paramLookup.TryGetValue(op, out OpType value) || param.CheckParamSig(value.Parameters))
			{
				return;
			}
			throw new Exception(string.Format("{0}: {1} - {2}", line - 1, name, "Parameters were not of expected type!"));
		}

		public void OutputOpType(OpType op)
		{
			output.Write((short)op.OpCode);
		}

		public void CmdOpNull()
		{
			output.Write((short)(-1));
		}

		public void OutputCmd(string name, BGIParameters param)
		{
			OpType op = paramLookup[name];
			ParamCheck(name, param);
			OutputOpType(op);
			param.OutputAllParams();
		}

		public void ParseOperation(ITree tree)
		{
			name = tree.GetChild(0).Text;
			line = tree.Line;
			BGIParameters param = (tree.ChildCount <= 1) ? new BGIParameters() : new BGIParameters(tree.GetChild(1));
			if (paramLookup.ContainsKey(name))
			{
				OutputCmd(name, param);
			}
			else
			{
				Debug.LogError("Unhandled Operation " + name);
				CmdOpNull();
			}
		}
	}
}
